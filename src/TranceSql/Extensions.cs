using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace TranceSql
{
    internal static class Extensions
    {
        public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
        {
            return type.GetInterfaces()
                .Union(new[] { type })
                .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));
        }

        public static string ToCamelCase(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }

            if (value.Length == 1)
            {
                return value.ToLower();
            }

            return String.Concat(value.Substring(0, 1).ToUpper(), value.Substring(1));
        }

        public static Type GetCollectionType(this Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }
            else if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return type.GetGenericArguments().First();
            }
            else
            {
                return null;
            }
        }

        public static PropertyInfo GetPropertyInfo<T, TResult>(this Expression<Func<T, TResult>> propertySelector)
        {
            var expression = propertySelector.Body as MemberExpression;
            if (expression == null)
            {
                // Give it another chance, try to exclude the conversion. This allows us to use
                // "Expression<Func<T,Object>>" as a selector when we don't care about the return type.
                if (propertySelector.Body.NodeType == ExpressionType.Convert &&
                    propertySelector.Body.Type == typeof(object) &&
                    propertySelector.Body is UnaryExpression)
                {
                    var unary = propertySelector.Body as UnaryExpression;
                    expression = unary.Operand as MemberExpression;
                }
                if (expression == null)
                {
                    throw new ArgumentException("Expression must be a property selector");
                }
            }
            var property = expression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("Selector expression must be select a property. (Members and other values are not allowed)");
            }

            return property;
        }

        public static bool ImplementsInterface<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

    }
}
