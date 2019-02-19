using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace TranceSql.Processing
{
    /// <summary>
    /// Utility class for producing objects from ADO results.
    /// </summary>
    public static class EntityMapping
    {
        /// <summary>
        /// The internal entity creation delegate library.
        /// </summary>
        private static Dictionary<Type, Delegate> _creationDelegates = new Dictionary<Type, Delegate>();
        private static readonly object _creationDelegatesLocker = new object();

        /// <summary>
        /// The custom type maps registered for the application.
        /// </summary>
        private static List<ICustomEntityMap> _customMaps = new List<ICustomEntityMap>();


        // TODO: perhaps make this configurable per SQL manager instance at some point

        /// <summary>
        /// Gets or sets a global IEqualityComparer instance that will be used to determine 
        /// whether a column is a match for a property name. If null is specified, a default
        /// very permissive comparer will be used which ignores underscores '_' and is case
        /// insensitive.
        /// </summary>
        public static IEqualityComparer<string> ColumnPropertyComparer { get; set; }
            = DefaultCaseComparer.Comparer;

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
        /// <returns>
        /// List of column names and their ordinals.
        /// </returns>
        internal static IDictionary<string, int> MapDbReaderColumns(DbDataReader reader)
        {
            var result = new Dictionary<string, int>(ColumnPropertyComparer ?? DefaultCaseComparer.Comparer);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                // multiple columns with the same name get
                result[reader.GetName(i)] = i;
            }
            return result;
        }

        internal static class ReadHelper
        {
            private static MethodInfo _ordinalHelperMethod
                = typeof(ReadHelper).GetMethod(nameof(Get), new[] { typeof(DbDataReader), typeof(int) });

            public static MethodInfo GetOrdinalHelperMethod(Type dataType) => _ordinalHelperMethod.MakeGenericMethod(dataType);

            public static MethodInfo PropertyHelperMethod
                = typeof(ReadHelper).GetMethod(nameof(Get), new[] { typeof(DbDataReader), typeof(IDictionary<string, int>), typeof(string) });

            public static MethodInfo GetPropertyHelperMethod(Type dataType) => PropertyHelperMethod.MakeGenericMethod(dataType);

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
                    {
                        return default;
                    }

                    if (typeof(T).IsEnum)
                    {
                        if (result is string stringResult)
                        {
                            return (T)Enum.Parse(typeof(T), stringResult);
                        }
                    }

                    if (result is T r)
                    {
                        return r;
                    }

                    return (T)Convert.ChangeType(result, typeof(T));
                }
                else
                {
                    // unmatched columns
                    return default;
                }
            }

            /// <summary>Helper method called from dynamically generated entity map function
            /// to return a value from a DbDataReader or the type default if the value
            /// cannot be found.</summary>
            /// <typeparam name="T">Type to return.</typeparam>
            /// <param name="reader">Open DbDataReader instance to retrieve value from.</param>
            /// <param name="ordinal">Ordinal position of column to get.</param>
            /// <returns>Value of column or the default for the specified type if the column does not exist.</returns>
            public static T Get<T>(DbDataReader reader, int ordinal)
            {
                if (reader.FieldCount <= ordinal)
                {
                    throw new IndexOutOfRangeException($"The data result for the query includes {reader.FieldCount} columns, which is not enough to populate the requested tuple fields.");
                }

                object result = reader[ordinal];

                if (Convert.IsDBNull(result))
                {
                    return default;
                }

                if (typeof(T).IsEnum)
                {
                    if (result is string stringResult)
                    {
                        return (T)Enum.Parse(typeof(T), stringResult);
                    }
                }

                if (result is T r)
                {
                    return r;
                }

                return (T)Convert.ChangeType(result, typeof(T));

            }
        }

        /// <summary>
        /// Registers a CreateEntity delegate used by TranceSQL for mapping entities from a DbDataReader. If this method
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
            {
                throw new ArgumentException("Cannot register entity creator for type '" + typeof(T).Name + "' because it is a simple type which will be cast directly from a result.", "T");
            }

            if (!TryRegister<T>(() => createEntity))
            {
                throw new InvalidOperationException("The delegate library already contains a definition for the type '" + typeof(T).Name + "'. RegisterCustomEntityCreator must be called before the type has been bound.");
            }
        }

        private static bool TryRegister<T>(Func<CreateEntity<T>> createEntity)
        {
            if (!_creationDelegates.ContainsKey(typeof(T)))
            {
                lock (_creationDelegatesLocker)
                {
                    if (!_creationDelegates.ContainsKey(typeof(T)))
                    {
                        _creationDelegates[typeof(T)] = createEntity();
                        return true; // type register successfully
                    }
                }
            }

            return false; // type was already register
        }

        /// <summary>
        /// Ensures that the entity creation delegate has been created for the specified
        /// type. Calling this method at the beginning of your application ensures that
        /// the there will not be a performance penalty for the first call to request the
        /// specified type.
        /// </summary>
        /// <typeparam name="T">Type to warm up.</typeparam>
        public static void WarmUp<T>()
        {
            if (IsSimpleType<T>())
            {
                return;
            }

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
            // Ensure this type is registered
            TryRegister(() =>
            {
                if (IsValueTuple(typeof(T)))
                {
                    return CreateTupleFunc<T>();
                }
                else
                {
                    return CreateEntityFunc<T>();
                }
            });
            return (CreateEntity<T>)_creationDelegates[typeof(T)];
        }

        private static CreateEntity<T> CreateTupleFunc<T>()
        {
            var readerParam = Expression.Parameter(typeof(DbDataReader), "r");
            var mapParam = Expression.Parameter(typeof(IDictionary<string, int>), "m");
            var readMethod = typeof(DbDataReader).GetMethod("get_Item", new[] { typeof(int) });


            var tupleType = typeof(T);
            var newTuple = CreateTupleInitExpression(readerParam, readMethod, tupleType);

            return Expression.Lambda<CreateEntity<T>>(newTuple, new ParameterExpression[] { readerParam, mapParam }).Compile();
        }

        private static Expression CreateTupleInitExpression(
            ParameterExpression readerParam,
            MethodInfo readMethod,
            Type tupleType,
            int offset = 0)
        {
            var types = tupleType.GetGenericArguments();
            var ctor = tupleType.GetConstructor(types);
            var getExpressions = types.Select((type, i) =>
            {
                if (IsValueTuple(type))
                {
                    return CreateTupleInitExpression(readerParam, readMethod, type, i + offset);
                }
                else
                {
                    // default property mapping, first make a generic version of the helper method call
                    var ordinalHelper = ReadHelper.GetOrdinalHelperMethod(type);
                    // create a call to the ReadHelper.Get<Type>(reader, original)
                    var getValue = Expression.Call(ordinalHelper, readerParam, Expression.Constant(i + offset));
                    // cast the result of the ReadHelper.Get call to the property type
                    return Expression.Convert(getValue, type);
                }
            });

            var newTuple = Expression.New(ctor, getExpressions);
            return newTuple;
        }

        private static Type[] _valueTuples = new[]
        {
            typeof(ValueTuple<>),
            typeof(ValueTuple<,>),
            typeof(ValueTuple<,,>),
            typeof(ValueTuple<,,,>),
            typeof(ValueTuple<,,,,>),
            typeof(ValueTuple<,,,,,>),
            typeof(ValueTuple<,,,,,,>),
            typeof(ValueTuple<,,,,,,,>)
        };

        private static bool IsValueTuple(Type type)
        {
            return
                type.IsValueType &&
                type.IsGenericType &&
                _valueTuples.Contains(type.GetGenericTypeDefinition());
        }

        /// <summary>
        /// Sets up a filter which can be used to exclude properties from being set by
        /// the TranceSQL mapping. This could be used for example to check for a "Ignore"
        /// attribute. By default no filtering is preformed.
        /// </summary>
        public static Func<PropertyInfo, bool> PropertyFilter { get; set; } = p => true;

        /// <summary>
        /// Overrides the default the constructor resolver which is used to select which
        /// constructor to choose when dynamically creating a result class. By default if
        /// a parameterless constructor exists it will be chosen, if a single constructor
        /// which requires arguments exists it will be chosen, if multiple constructors are
        /// present and all require arguments, an exception will be thrown. To override,
        /// set this property at the beginning of the application before any mapping 
        /// operations have been performed.
        /// </summary>
        public static Func<Type, ConstructorInfo> ConstructorResolver { get; set; } = DefaultConstructorResolver;

        private static ConstructorInfo DefaultConstructorResolver(Type type)
        {
            var constructors = type.GetConstructors();

            if (!constructors.Any())
            {
                throw new ArgumentException($"Type {type.Name} does not define a constructor and cannot be created dynamically by TranceSQL's dynamic mapping.");
            }

            var emptyConstructorInfo = constructors.FirstOrDefault(c => !c.GetParameters().Any());
            if (emptyConstructorInfo != null)
            {
                return emptyConstructorInfo;
            }
            else
            {
                if (constructors.Count() != 1)
                {
                    throw new ArgumentException($"Type {type.Name} constructor is ambiguous, more than one constructor requiring arguments exists. Please provide a type with either a single constructor or a parameterless constructor. Alternatively, the default global {nameof(EntityMapping)}.{nameof(EntityMapping.ConstructorResolver)} method may be replace by one which knowns how to choose the correct constructor for your application.");
                }

                return constructors.Single();
            }
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

            // set up constructor
            NewExpression constructorExpression;
            var constructorInfo = ConstructorResolver(typeof(T));

            // Empty constructor was provided
            if (!constructorInfo.GetParameters().Any())
            {
                constructorExpression = Expression.New(constructorInfo);
            }
            else
            {
                var constructorArguments = new List<Expression>();
                foreach (var parameter in constructorInfo.GetParameters())
                {
                    // map constructor arguments to the SQL results
                    var helperMethodInfo = ReadHelper.GetPropertyHelperMethod(parameter.ParameterType);
                    Expression getValue = Expression.Call(helperMethodInfo, readerParam, mapParam, Expression.Constant(parameter.Name.ToCamelCase()));

                    constructorArguments.Add(Expression.Convert(getValue, parameter.ParameterType));
                }

                constructorExpression = Expression.New(constructorInfo, constructorArguments.ToArray());
            }

            // add public properties as bindings for class initialization
            List<MemberBinding> bindings = new List<MemberBinding>();
            var properties = typeof(T).GetAllProperties().Where(p => p.CanWrite && PropertyFilter(p));

            // filter properties if filter was provided
            if (propertyFilter != null)
            {
                properties = properties.Where(p => propertyFilter.Any(f => f.Equals(p.Name, StringComparison.OrdinalIgnoreCase)));
            }

            foreach (var property in properties)
            {
                var customMap = _customMaps.FirstOrDefault(m => m.DoesApply(property));
                if (customMap != null)
                {
                    // use the custom mapping to obtain an expression
                    var getValue = customMap.GetExpression(property, ReadHelper.PropertyHelperMethod, readerParam, mapParam);
                    // add a binding for this property to the expression
                    bindings.Add(Expression.Bind(property, getValue));
                }
                else
                {
                    // default property mapping, first make a generic version of the helper method call
                    var helperMethodInfo = ReadHelper.GetPropertyHelperMethod(property.PropertyType);
                    // create a call to the ReadHelper.Get<PropertyType>(reader, columnMap, column)
                    var getValue = Expression.Call(helperMethodInfo, readerParam, mapParam, Expression.Constant(property.Name));
                    // cast the result of the ReadHelper.Get call to the property type
                    var convertedValue = Expression.Convert(getValue, property.PropertyType);
                    bindings.Add(Expression.Bind(property, convertedValue));
                }
            }

            var memberInit = Expression.MemberInit(constructorExpression, bindings);
            return Expression.Lambda<CreateEntity<T>>(memberInit, new ParameterExpression[] { readerParam, mapParam }).Compile();
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
                var isDbNull = typeof(Convert).GetMethod(nameof(Convert.IsDBNull), BindingFlags.Public | BindingFlags.Static);
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
            typeof(DateTime),typeof(DateTime?),typeof(DateTimeOffset),typeof(DateTimeOffset?),typeof(string), typeof(Guid), typeof(Guid?),
            typeof(object)
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
        /// <returns>True if the type is a simple type.</returns>
        internal static bool IsSimpleType(Type type)
        {
            // Typically arrays/enumerable types are going to be loaded as a list of results
            // and not as an actual result type, however a few exceptions such as VARBINARY
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
