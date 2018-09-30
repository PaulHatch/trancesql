﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Concurrent;

namespace TranceSql
{
    /// <summary>
    /// Utility class for producing objects from ADO results.
    /// </summary>
    public static class EntityMapping
    {
        static List<ICustomEntityMap> _customMaps = new List<ICustomEntityMap>();

        /// <summary>
        /// Globally registers an entity map. For more information, see the ICustomEntityMap
        /// interface documentation.
        /// </summary>
        /// <param name="map">The map to register.</param>
        public static void RegisterEntityMap(ICustomEntityMap map)
        {
            _customMaps.Add(map);
        }

        /// <summary>
        /// Creates a map of column names to column ordinals to allow faster mapping from
        /// a data reader to an object.
        /// </summary>
        /// <param name="reader">Reader to map.</param>
        /// <returns>List of column names and their ordinals.</returns>
        internal static IDictionary<string, int> MapDbReaderColumns(DbDataReader reader)
        {
            var result = new Dictionary<string, int>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                result.Add(reader.GetName(i), i);
            }
            return result;
        }

        internal static class ReadHelper
        {
            /// <summary>
            /// Helper method called from dynamically generated entity map function
            /// to return a value from a DbDataReader or the type default if the value
            /// cannot be found.
            /// </summary>
            /// <typeparam name="T">Type to return.</typeparam>
            /// <param name="reader">Open DbDataReader instance to retrieve value from.</param>
            /// <param name="columnMap">Hash table of column names and ordinals.</param>
            /// <param name="column">Name of column to get.</param>
            /// <returns>
            /// Value of column or the default for the specified type if the column does not exist.
            /// </returns>
            /// <remarks>
            /// Attempting to access a column in a database reader that does not exist throws an
            /// IndexOutOfRange exception. Since we do not know in advance which columns will be
            /// present the delegate created by the CreateEntityFunction method attempts to populate
            /// all public properties from the supplied DbDataReader instance. It uses this method
            /// to get the value to ensure that if a column does not exist the default value for the 
            /// property type is used rather than throwing an exception.
            /// </remarks>
            public static T Get<T>(DbDataReader reader, IDictionary<string, int> columnMap, string column)
            {
                if (columnMap.ContainsKey(column))
                {
                    // a match exists
                    object result = reader[columnMap[column]];

                    if (Convert.IsDBNull(result))
                        return default(T);

                    if (typeof(T).IsEnum)
                    {
                        var stringResult = result as string;
                        if (stringResult != null)
                        {
                            return (T)Enum.Parse(typeof(T), stringResult);
                        }
                    }

                    return (T)result;
                }
                else
                {
                    // unmatched columns
                    return default(T);
                }
            }
        }

        /// <summary>
        /// The internal entity creation delegate library.
        /// </summary>
        private static ConcurrentDictionary<Type, Delegate> creationDelegates = new ConcurrentDictionary<Type, Delegate>();

        /// <summary>
        /// Registers a CreateEntity delegate used by fluent SQL for mapping entities from a DbDataReader. If this method
        /// is called before the entity type is automatically mapped the method provided will be used instead of the default
        /// auto-generated mapper. Callers should review remarks section before utilizing this method.
        /// </summary>
        /// <typeparam name="T">Entity type to register.</typeparam>
        /// <param name="createEntity">Function delegate which returns an entity of type T.</param>
        /// <remarks>
        /// Please note that the delegate provided will not be called by the SQL command unless the DbDataReader instance has rows.
        /// The entity creator must only perform only 'generic' binding as it will be used globally whenever an instance of type 'T'
        /// is created from a DbDataReader row. If application specific custom binding is required, a <see cref="CreateEntity{T}"/>; 
        /// delegate can be passed as an argument to an execute result request.
        /// </remarks>
        public static void RegisterCustomEntityCreator<T>(CreateEntity<T> createEntity)
        {
            if (IsSimpleType<T>())
                throw new ArgumentException("Cannot register entity creator for type '" + typeof(T).Name + "' because it is a simple type which will be cast directly from a result.", "T");

            if (!creationDelegates.TryAdd(typeof(T), createEntity))
                throw new InvalidOperationException("The delegate library already contains a definition for the type '" + typeof(T).Name + "'. RegisterCustomEntityCreator must be called before the type has been bound.");
        }

        /// <summary>
        /// Initializes the entity creator for the specified type using a filtered list of properties. This setting is
        /// applied globally.
        /// </summary>
        /// <typeparam name="T">Entity type to register.</typeparam>
        /// <param name="properties">
        /// Properties to be included. If a property's name is not included in this list, it will not
        /// be bound in the entity created registered for this type.
        /// </param>
        public static void RegisterFilteredEntityCreator<T>(params string[] properties)
        {
            if (IsSimpleType<T>())
                throw new ArgumentException("Cannot register entity creator for type '" + typeof(T).Name + "' because it is a simple type which will be cast directly from a result.", "T");

            if (!creationDelegates.TryAdd(typeof(T), CreateEntityFunc<T>(properties)))
                throw new InvalidOperationException("The delegate library already contains a definition for the type '" + typeof(T).Name + "'. RegisterFilteredEntityCreator must be called before the type has been bound.");
        }

        /// <summary>
        /// Ensures that the entity creation delegate has been created for the specified
        /// type. Calling this method at the beginning of your application ensures that
        /// the there will not be a performance penalty for the first call to request the
        /// specified type.
        /// </summary>
        /// <param name="T">Type to warm up.</param>
        public static void WarmUp<T>()
        {
            if (IsSimpleType<T>())
                return;

            GetEntityFunc<T>();
        }

        /// <summary>
        /// "ORM in a Can" - Gets a function delegate from the EntityMapper's internal 
        /// library which will return an instance of the specified entity class type from a
        /// DB reader data row. By default this function will bind public properties and 
        /// constructor parameters to a row from a DbDataReader.
        /// </summary>
        /// <typeparam name="T">Type to bind to.</typeparam>
        /// <returns>
        /// A function delegate which returns an instance of the specified type from a
        /// DB reader data row.
        /// </returns>
        internal static CreateEntity<T> GetEntityFunc<T>()
        {
            return (CreateEntity<T>)creationDelegates.GetOrAdd(typeof(T), CreateEntityFunc<T>());
        }

        /// <summary>
        /// Dynamically generates an entity creation delegate based on the specified type.
        /// </summary>
        /// <typeparam name="T">Type to bind to.</typeparam>
        /// <param name="propertyFilter">
        /// If provided, properties will only be bound if their names are included in this list.
        /// </param>
        /// <returns>
        /// A function delegate which returns an instance of the specified type from a
        /// DB reader data row.
        /// </returns>
        private static CreateEntity<T> CreateEntityFunc<T>(IEnumerable<string> propertyFilter = null)
        {
            var readerParam = Expression.Parameter(typeof(DbDataReader), "r");
            var mapParam = Expression.Parameter(typeof(IDictionary<string, int>), "m");
            var readMethod = typeof(DbDataReader).GetMethod("get_Item", new[] { typeof(string) });
            var genericHelperMethodInfo = typeof(ReadHelper).GetMethod("Get", new[] { typeof(DbDataReader), typeof(IDictionary<string, int>), typeof(string) });

            // set up constructor
            NewExpression constructorExpression;
            var constructors = typeof(T).GetConstructors();

            // use empty constructor if exists
            var emptyConstructorInfo = constructors.FirstOrDefault(c => !c.GetParameters().Any());
            if (emptyConstructorInfo != null)
                constructorExpression = Expression.New(emptyConstructorInfo);
            else
            {
                if (constructors.Count() != 1)
                    throw new ArgumentException("Type " + typeof(T).Name + " constructor is ambiguous. Please provide a type with either a single constructor or a parameterless constructor.");

                var constructorInfo = constructors.Single();
                var constructorArguments = new List<Expression>();
                foreach (var parameter in constructorInfo.GetParameters())
                {
                    // map constructor arguments to the SQL results
                    var helperMethodInfo = genericHelperMethodInfo.MakeGenericMethod(parameter.ParameterType);
                    Expression getValue = Expression.Call(helperMethodInfo, readerParam, mapParam, Expression.Constant(parameter.Name.ToCamelCase()));

                    constructorArguments.Add(Expression.Convert(getValue, parameter.ParameterType));
                }

                constructorExpression = Expression.New(constructorInfo, constructorArguments.ToArray());
            }

            // add public properties as bindings for class initialization
            List<MemberBinding> bindings = new List<MemberBinding>();
            var properties = typeof(T).GetAllProperties().Where(p => p.CanWrite);

            // filter properties if filter was provided
            if (propertyFilter != null)
                properties = properties.Where(p => propertyFilter.Any(f => f.Equals(p.Name, StringComparison.OrdinalIgnoreCase)));

            foreach (var property in properties)
            {
                var customMap = _customMaps.FirstOrDefault(m => m.DoesApply(property));
                if (customMap != null)
                {
                    // default property mapping
                    bindings.Add(Expression.Bind(property, customMap.GetExpression(property, genericHelperMethodInfo, readerParam, mapParam)));
                }
                else
                {
                    // handle custom mapping
                    var helperMethodInfo = genericHelperMethodInfo.MakeGenericMethod(property.PropertyType);
                    Expression getValue = Expression.Call(helperMethodInfo, readerParam, mapParam, Expression.Constant(property.Name));
                    bindings.Add(Expression.Bind(property, Expression.Convert(getValue, property.PropertyType)));
                }
            }

            var memberInit = Expression.MemberInit(constructorExpression, bindings);
            var func = Expression.Lambda<CreateEntity<T>>(memberInit, new ParameterExpression[] { readerParam, mapParam }).Compile();

            return func;
        }
        
        /// <summary>
        /// Returns an expression which converts the specified raw value to the specified
        /// type, if the type specified supports a null value, the result will include a
        /// condition operation to convert DbNull values to null.
        /// </summary>
        /// <param name="rawValue">Expression providing the raw value.</param>
        /// <param name="type">Type to convert expression to.</param>
        /// <returns>Converted and cast expression.</returns>
        private static Expression CastAndConvertDbNull(Expression rawValue, Type type)
        {
            if (TypeIsNullable(type))
            {
                // T is nullable type:
                // Convert.IsDbNull(rawValue) ? (T)null : (T)rawValue
                var isDbNull = typeof(Convert).GetMethod("IsDBNull", BindingFlags.Public | BindingFlags.Static);
                var test = Expression.Call(isDbNull, rawValue);
                return Expression.Condition(test,
                    Expression.TypeAs(Expression.Constant(null), type),
                    Expression.TypeAs(rawValue, type));
            }
            else
            {
                // No null-condition required:
                // (T)rawValue
                return Expression.TypeAs(rawValue, type);
            }
        }

        /// <summary>Checks if the specified type supports null values.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if the type supports null values.</returns>
        private static bool TypeIsNullable(Type type)
        {
            return (Nullable.GetUnderlyingType(type) != null) || !type.IsValueType;
        }

        /// <summary>List of types which convert directly from DB types.</summary>
        private static readonly Type[] simpleTypes = new[] { 
            typeof(byte), typeof(bool), typeof(short), typeof(int), typeof(long), typeof(float),typeof(double),typeof(decimal),
            typeof(byte?), typeof(bool?), typeof(short?), typeof(int?), typeof(long?), typeof(float?),typeof(double?),typeof(decimal?),
            typeof(DateTime),typeof(DateTime?),typeof(string), typeof(Guid), typeof(Guid?), typeof(object)
        };


        /// <summary>
        /// Test to see if a type falls in the mapper's definition of a 'simple type' which can be cast
        /// directly from a database value without mapping.
        /// </summary>
        /// <typeparam name="T">Type to check.</typeparam>
        /// <returns>True if the type is a simple type.</returns>
        internal static bool IsSimpleType<T>()
        {
            return IsSimpleType(typeof(T));
        }

        /// <summary>
        /// Test to see if a type falls in the mapper's definition of a 'simple type' which can be cast
        /// directly from a database value without mapping.
        /// </summary>
        /// <typeparam name="T">Type to check.</typeparam>
        /// <returns>True if the type is a simple type.</returns>
        internal static bool IsSimpleType(Type type)
        {
            // Typically arrays/enumerable types are going to be loaded as a list of results
            // and not as an actual result type, however a few exceptions such as varbinary
            // data will be returned as byte[] types, so check for arrays here. Enumerable
            // casting, e.g. IEnumerable<byte>, is not currently support by the EntityMapper.
            if (type.IsArray)
            {
                return simpleTypes.Contains(type.GetElementType());
            }
            else
            {
                return type.IsEnum || simpleTypes.Contains(type);
            }
        }


    }

}