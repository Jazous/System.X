using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Data;

namespace System.X.Linq
{
    public sealed class ExpressionHelper
    {
        internal static readonly ExpressionHelper Instance = new ExpressionHelper();
        static readonly Reflection.MethodInfo _contains = typeof(System.Linq.Enumerable).GetMethods(Reflection.BindingFlags.Static | Reflection.BindingFlags.Public | Reflection.BindingFlags.InvokeMethod).FirstOrDefault(c => c.Name == "Contains" && c.GetParameters().Length == 2);
        ExpressionHelper() { }

        public Expression<Func<TSource, bool>> LambdaBuild<TSource>(string propertyName, QueryMode mode, params dynamic[] values)
        {
            if (mode != QueryMode.IsNull && mode != QueryMode.IsNotNull && (values == null || !values.Any()))
                return null;

            switch (mode)
            {
                case QueryMode.Equal: return Equal<TSource>(propertyName, values);
                case QueryMode.NotEqual: return NotEqual<TSource>(propertyName, values);
                case QueryMode.GreaterThan: return GreaterThan<TSource>(propertyName, values[0]);
                case QueryMode.GreaterThanOrEqual: return GreaterThanOrEqual<TSource>(propertyName, values[0]);
                case QueryMode.LessThan: return LessThan<TSource>(propertyName, values[0]);
                case QueryMode.LessThanOrEqual: return LessThanOrEqual<TSource>(propertyName, values[0]);
                case QueryMode.Like: return Contains<TSource>(propertyName, values[0]);
                case QueryMode.StartWith: return StartWith<TSource>(propertyName, values[0]);
                case QueryMode.EndWith: return EndWith<TSource>(propertyName, values[0]);
                case QueryMode.NotIn: return Except<TSource>(propertyName, values);
                case QueryMode.In: return ElementsIn<TSource>(propertyName, values);
                case QueryMode.IsNull: return Equal<TSource>(propertyName, null);
                case QueryMode.IsNotNull: return NotEqual<TSource>(propertyName, null);
            }
            return null;
        }

        public Expression<Func<TSource, bool>> Equal<TSource>(string propertyName, params dynamic[] values)
        {
            var prop = LambdaProperty<TSource>(propertyName);

            if (values == null || values.Length == 0)
            {
                var equal = Expression.Equal(prop.Body, Expression.Constant(null, prop.ReturnType));
                return Expression.Lambda<Func<TSource, bool>>(equal, prop.Parameters);
            }
          
            var equals = values.Select(value => Expression.Equal(prop.Body, value == null ? Expression.Constant(null, prop.ReturnType) : Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType))).ToArray();
            if (equals.Length == 1)
                return Expression.Lambda<Func<TSource, bool>>(equals[0], prop.Parameters);

            var body = equals.Aggregate((accumulate, equal) => Expression.Or(accumulate, equal));
            return Expression.Lambda<Func<TSource, bool>>(body, prop.Parameters);
        }
        public Expression<Func<TSource, bool>> NotEqual<TSource>(string propertyName,params dynamic[] values)
        {
            var prop = LambdaProperty<TSource>(propertyName);

            if (values == null || values.Length == 0)
            {
                var equal = Expression.NotEqual(prop.Body, Expression.Constant(null, prop.ReturnType));
                return Expression.Lambda<Func<TSource, bool>>(equal, prop.Parameters);
            }

            var equals = values.Select(value => Expression.NotEqual(prop.Body, value == null ? Expression.Constant(null, prop.ReturnType) : Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType))).ToArray();
            if (equals.Length == 1)
                return Expression.Lambda<Func<TSource, bool>>(equals[0], prop.Parameters);

            var body = equals.Aggregate((accumulate, equal) => Expression.AndAlso(accumulate, equal));
            return Expression.Lambda<Func<TSource, bool>>(body, prop.Parameters);
        }
        public Expression<Func<TSource, bool>> GreaterThan<TSource>(string propertyName, dynamic value)
        {
            var prop = LambdaProperty<TSource>(propertyName);
            var body = Expression.GreaterThan(prop.Body, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType));
            return Expression.Lambda<Func<TSource, bool>>(body, prop.Parameters);
        }
        public Expression<Func<TSource, bool>> GreaterThanOrEqual<TSource>(string propertyName, dynamic value)
        {
            var prop = LambdaProperty<TSource>(propertyName);
            var body = Expression.GreaterThanOrEqual(prop.Body, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType));
            return Expression.Lambda<Func<TSource, bool>>(body, prop.Parameters);
        }
        public Expression<Func<TSource, bool>> LessThan<TSource>(string propertyName, dynamic value)
        {
            var prop = LambdaProperty<TSource>(propertyName);
            var body = Expression.LessThan(prop.Body, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType));
            return Expression.Lambda<Func<TSource, bool>>(body, prop.Parameters);
        }
        public Expression<Func<TSource, bool>> LessThanOrEqual<TSource>(string propertyName, dynamic value)
        {
            var prop = LambdaProperty<TSource>(propertyName);
            var body = Expression.LessThanOrEqual(prop.Body, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType));
            return Expression.Lambda<Func<TSource, bool>>(body, prop.Parameters);
        }
        public Expression<Func<TSource, bool>> Between<TSource, TKey>(string propertyName, TKey val1, TKey val2)
        {
            if (val1.Equals(val2))
                return Equal<TSource>(propertyName, val1);

            var prop = LambdaProperty<TSource>(propertyName);
            var cstart = Expression.GreaterThanOrEqual(prop.Body, Expression.Constant(Convert.ChangeType(val1, prop.ReturnType), prop.ReturnType));
            var cend = Expression.LessThan(prop.Body, Expression.Constant(Convert.ChangeType(val2, prop.ReturnType), prop.ReturnType));
            return Expression.Lambda<Func<TSource, bool>>(Expression.AndAlso(cstart, cend), prop.Parameters);
        }
        public Expression<Func<TSource, bool>> ElementsIn<TSource>(string propertyName, params dynamic[] values)
        {
            if (values == null || values.Length == 0)
                return c => false;

            if (values.Length == 1)
                return Equal<TSource>(propertyName, values[0]);

            var prop = LambdaProperty<TSource>(propertyName);
            var body = Expression.Call(null, _contains.MakeGenericMethod(prop.ReturnType), Constant(values, prop.ReturnType), prop.Body);
            return Expression.Lambda<Func<TSource, bool>>(body, prop.Parameters);
        }
        public Expression<Func<TSource, bool>> Except<TSource>(string propertyName, params dynamic[] values)
        {
            if (values.Length == 1)
                return NotEqual<TSource>(propertyName, values[0]);

            var prop = LambdaProperty<TSource>(propertyName);
            var body = Expression.Call(null, _contains.MakeGenericMethod(prop.ReturnType), Constant(values, prop.ReturnType), prop.Body);
            return Expression.Lambda<Func<TSource, bool>>(Expression.Not(body), prop.Parameters);
        }
        public Expression<Func<TSource, bool>> Contains<TSource>(string propertyName, string value)
        {
            var prop = LambdaProperty<TSource>(propertyName);
            var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var body = Expression.Call(prop.Body, method, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType));
            return Expression.Lambda<Func<TSource, bool>>(body, prop.Parameters);
        }
        public Expression<Func<TSource, bool>> StartWith<TSource>(string propertyName, string value)
        {
            var prop = LambdaProperty<TSource>(propertyName);
            var method = typeof(string).GetMethod("StartWith", new[] { typeof(string) });
            var body = Expression.Call(prop.Body, method, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType));
            return Expression.Lambda<Func<TSource, bool>>(body, prop.Parameters);
        }
        public Expression<Func<TSource, bool>> EndWith<TSource>(string propertyName, string value)
        {
            var prop = LambdaProperty<TSource>(propertyName);
            var method = typeof(string).GetMethod("EndWith", new[] { typeof(string) });
            var body = Expression.Call(prop.Body, method, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType));
            return Expression.Lambda<Func<TSource, bool>>(body, prop.Parameters);
        }
        public LambdaExpression LambdaProperty<T>(string propertyName)
        {
            return LambdaProperty(typeof(T), propertyName);
        }
        public LambdaExpression LambdaProperty(Type type, string propertyName)
        {
            int index = propertyName.IndexOf('.');
            var signature = Expression.Parameter(type, "c");
            var prop = Expression.Property(signature, index == -1 ? propertyName : propertyName.Substring(0, index));
            var lambda = Expression.Lambda(prop, signature);
            if (index == -1)
                return lambda;
            return Expression.Lambda(Property(prop, propertyName.Substring(index + 1)), signature);
        }
        public MemberExpression Property<T>(string propertyName)
        {
            return Property(typeof(T), propertyName);
        }
        public MemberExpression Property(Type type, string propertyName)
        {
            int index = propertyName.IndexOf('.');
            var signature = Expression.Parameter(type, "c");
            var prop = Expression.Property(signature, index == -1 ? propertyName : propertyName.Substring(0, index));
            if (index == -1)
                return prop;
            return Property(prop, propertyName.Substring(index + 1));
        }
        MemberExpression Property(MemberExpression member, string propertyName)
        {
            int index = propertyName.IndexOf('.');
            var prop = Expression.Property(member, index == -1 ? propertyName : propertyName.Substring(0, index));
            if (index == -1)
                return prop;
            return Property(prop, propertyName.Substring(index + 1));
        }

        ConstantExpression Constant<T>(T[] values, Type type)
        {
            if (type == typeof(int))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToInt32(value)));
            else if (type == typeof(string))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToString(value)));
            if (type == typeof(bool))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToBoolean(value)));
            if (type == typeof(DateTime))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToDateTime(value)));
            else if (type == typeof(long))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToInt64(value)));
            else if (type == typeof(decimal))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToDecimal(value)));
            else if (type == typeof(short))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToInt16(value)));
            else if (type == typeof(float))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToSingle(value)));
            else if (type == typeof(double))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToDouble(value)));
            else if (type == typeof(byte))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToByte(value)));
            else
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ChangeType(value, type)));
        }
    }
}