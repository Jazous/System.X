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
        static readonly Reflection.MethodInfo _string_contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        static readonly Reflection.MethodInfo _string_startwith = typeof(string).GetMethod("StartWith", new[] { typeof(string) });
        static readonly Reflection.MethodInfo _string_endwith = typeof(string).GetMethod("EndWith", new[] { typeof(string) });

        ExpressionHelper() { }

        public Expression<Func<T, bool>> Lambda<T>(string propertyName, QueryMode mode, params dynamic[] values)
        {
            switch (mode)
            {
                case QueryMode.EQ: return Equal<T>(propertyName, values);
                case QueryMode.NTEQ: return NotEqual<T>(propertyName, values);
                case QueryMode.GT: return GreaterThan<T>(propertyName, values[0]);
                case QueryMode.GTEQ: return GreaterThanOrEqual<T>(propertyName, values[0]);
                case QueryMode.LT: return LessThan<T>(propertyName, values[0]);
                case QueryMode.LTEQ: return LessThanOrEqual<T>(propertyName, values[0]);
                case QueryMode.LK: return Contains<T>(propertyName, values);
                case QueryMode.SW: return StartWith<T>(propertyName, values);
                case QueryMode.EW: return EndWith<T>(propertyName, values);
                case QueryMode.NTIN: return Except<T>(propertyName, values);
                case QueryMode.IN: return Include<T>(propertyName, values);
            }
            return null;
        }

        public Expression<Func<T, bool>> Equal<T>(string propertyName, params dynamic[] values)
        {
            var prop = LambdaProperty<T>(propertyName);

            if (values == null)
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(prop.Body, Expression.Constant(null, prop.ReturnType)), prop.Parameters);

            if (values.Length == 0)
                return c => false;

            if (values.Length == 1)
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(prop.Body, Expression.Constant(Convert.ChangeType(values[0], prop.ReturnType), prop.ReturnType)), prop.Parameters);

            var equals = values.Select(value => Expression.Equal(prop.Body, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType))).ToArray();
            var body = equals.Aggregate((accumulate, equal) => Expression.Or(accumulate, equal));
            return Expression.Lambda<Func<T, bool>>(body, prop.Parameters);
        }
        public Expression<Func<T, bool>> NotEqual<T>(string propertyName, params dynamic[] values)
        {
            var prop = LambdaProperty<T>(propertyName);

            if (values == null)
                return Expression.Lambda<Func<T, bool>>(Expression.NotEqual(prop.Body, Expression.Constant(null, prop.ReturnType)), prop.Parameters);

            if (values.Length == 0)
                return c => false;

            if (values.Length == 1)
                return Expression.Lambda<Func<T, bool>>(Expression.NotEqual(prop.Body, Expression.Constant(Convert.ChangeType(values[0], prop.ReturnType), prop.ReturnType)), prop.Parameters);

            var equals = values.Select(value => Expression.NotEqual(prop.Body, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType))).ToArray();
            var body = equals.Aggregate((accumulate, equal) => Expression.AndAlso(accumulate, equal));
            return Expression.Lambda<Func<T, bool>>(body, prop.Parameters);
        }
        public Expression<Func<T, bool>> GreaterThan<T>(string propertyName, dynamic value)
        {
            var prop = LambdaProperty<T>(propertyName);
            var body = Expression.GreaterThan(prop.Body, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType));
            return Expression.Lambda<Func<T, bool>>(body, prop.Parameters);
        }
        public Expression<Func<T, bool>> GreaterThanOrEqual<T>(string propertyName, dynamic value)
        {
            var prop = LambdaProperty<T>(propertyName);
            var body = Expression.GreaterThanOrEqual(prop.Body, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType));
            return Expression.Lambda<Func<T, bool>>(body, prop.Parameters);
        }
        public Expression<Func<T, bool>> LessThan<T>(string propertyName, dynamic value)
        {
            var prop = LambdaProperty<T>(propertyName);
            var body = Expression.LessThan(prop.Body, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType));
            return Expression.Lambda<Func<T, bool>>(body, prop.Parameters);
        }
        public Expression<Func<T, bool>> LessThanOrEqual<T>(string propertyName, dynamic value)
        {
            var prop = LambdaProperty<T>(propertyName);
            var body = Expression.LessThanOrEqual(prop.Body, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType));
            return Expression.Lambda<Func<T, bool>>(body, prop.Parameters);
        }
        public Expression<Func<T, bool>> Between<T>(string propertyName, dynamic val1, dynamic val2)
        {
            if (val1 == val2)
                return Equal<T>(propertyName, val1);

            var prop = LambdaProperty<T>(propertyName);
            var cstart = Expression.GreaterThanOrEqual(prop.Body, Expression.Constant(Convert.ChangeType(val1, prop.ReturnType), prop.ReturnType));
            var cend = Expression.LessThan(prop.Body, Expression.Constant(Convert.ChangeType(val2, prop.ReturnType), prop.ReturnType));
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(cstart, cend), prop.Parameters);
        }
        public Expression<Func<T, bool>> Include<T>(string propertyName, params dynamic[] values)
        {
            var prop = LambdaProperty<T>(propertyName);

            if (values == null)
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(prop.Body, Expression.Constant(null, prop.ReturnType)), prop.Parameters);

            if (values.Length == 0)
                return c => false;

            if (values.Length == 1)
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(prop.Body, Expression.Constant(values[0], prop.ReturnType)), prop.Parameters);

            var body = Expression.Call(null, _contains.MakeGenericMethod(prop.ReturnType), Constant(values, prop.ReturnType), prop.Body);
            return Expression.Lambda<Func<T, bool>>(body, prop.Parameters);
        }
        public Expression<Func<T, bool>> Except<T>(string propertyName, params dynamic[] values)
        {
            var prop = LambdaProperty<T>(propertyName);

            if (values == null)
                return Expression.Lambda<Func<T, bool>>(Expression.NotEqual(prop.Body, Expression.Constant(null, prop.ReturnType)), prop.Parameters);

            if (values.Length == 0)
                return c => false;

            if (values.Length == 1)
                return Expression.Lambda<Func<T, bool>>(Expression.NotEqual(prop.Body, Expression.Constant(values[0], prop.ReturnType)), prop.Parameters);

            var body = Expression.Call(null, _contains.MakeGenericMethod(prop.ReturnType), Constant(values, prop.ReturnType), prop.Body);
            return Expression.Lambda<Func<T, bool>>(Expression.Not(body), prop.Parameters);
        }
        public Expression<Func<T, bool>> Contains<T>(string propertyName, params dynamic[] values)
        {
            var prop = LambdaProperty<T>(propertyName);

            if (values == null)
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(prop.Body, Expression.Constant(null, prop.ReturnType)), prop.Parameters);

            if (values.Length == 0)
                return c => false;

            if (values.Length == 1)
                return Expression.Lambda<Func<T, bool>>(Expression.Call(prop.Body, _string_contains, Expression.Constant(Convert.ChangeType(values[0], prop.ReturnType), prop.ReturnType)), prop.Parameters);

            var calls = values.Select(value => Expression.Call(prop.Body, _string_contains, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType))).ToArray();
            var body = Expression.Or(calls[0], calls[1]);
            for (int i = 2; i < calls.Length; i++)
                body = Expression.Or(body, calls[i]);
            return Expression.Lambda<Func<T, bool>>(body, prop.Parameters);
        }
        public Expression<Func<T, bool>> StartWith<T>(string propertyName, params dynamic[] values)
        {
            var prop = LambdaProperty<T>(propertyName);

            if (values == null)
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(prop.Body, Expression.Constant(null, prop.ReturnType)), prop.Parameters);

            if (values.Length == 0)
                return c => false;

            if (values.Length == 1)
                return Expression.Lambda<Func<T, bool>>(Expression.Call(prop.Body, _string_startwith, Expression.Constant(Convert.ChangeType(values[0], prop.ReturnType), prop.ReturnType)), prop.Parameters);

            var calls = values.Select(value => Expression.Call(prop.Body, _string_startwith, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType))).ToArray();
            var body = Expression.Or(calls[0], calls[1]);
            for (int i = 2; i < calls.Length; i++)
                body = Expression.Or(body, calls[i]);
            return Expression.Lambda<Func<T, bool>>(body, prop.Parameters);
        }
        public Expression<Func<T, bool>> EndWith<T>(string propertyName, params dynamic[] values)
        {
            var prop = LambdaProperty<T>(propertyName);

            if (values == null)
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(prop.Body, Expression.Constant(null, prop.ReturnType)), prop.Parameters);

            if (values.Length == 0)
                return c => false;

            if (values.Length == 1)
                return Expression.Lambda<Func<T, bool>>(Expression.Call(prop.Body, _string_endwith, Expression.Constant(Convert.ChangeType(values[0], prop.ReturnType), prop.ReturnType)), prop.Parameters);

            var calls = values.Select(value => Expression.Call(prop.Body, _string_endwith, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType))).ToArray();
            var body = Expression.Or(calls[0], calls[1]);
            for (int i = 2; i < calls.Length; i++)
                body = Expression.Or(body, calls[i]);
            return Expression.Lambda<Func<T, bool>>(body, prop.Parameters);
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

        public ConstantExpression Constant<T>(T[] values, Type conversionType)
        {
            if (conversionType == null)
                return Expression.Constant(values, typeof(T[]));

            if (values == null)
                return Expression.Constant(null, conversionType);

            if (conversionType == typeof(int))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToInt32(value)));
            if (conversionType == typeof(int?))
                return Expression.Constant(Array.ConvertAll(values, value => value == null ? null : (int?)Convert.ToInt32(value)));
            if (conversionType == typeof(string))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToString(value)));
            if (conversionType == typeof(bool))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToBoolean(value)));
            if (conversionType == typeof(bool?))
                return Expression.Constant(Array.ConvertAll(values, value => value == null ? null : (bool?)Convert.ToBoolean(value)));
            if (conversionType == typeof(DateTime))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToDateTime(value)));
            if (conversionType == typeof(DateTime?))
                return Expression.Constant(Array.ConvertAll(values, value => value == null ? null : (DateTime?)Convert.ToDateTime(value)));
            if (conversionType == typeof(long))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToInt64(value)));
            if (conversionType == typeof(long?))
                return Expression.Constant(Array.ConvertAll(values, value => value == null ? null : (long?)Convert.ToInt64(value)));
            if (conversionType == typeof(decimal))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToDecimal(value)));
            if (conversionType == typeof(decimal?))
                return Expression.Constant(Array.ConvertAll(values, value => value == null ? null : (decimal?)Convert.ToDecimal(value)));

            if (conversionType == typeof(short))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToInt16(value)));
            if (conversionType == typeof(float))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToSingle(value)));
            if (conversionType == typeof(double))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToDouble(value)));
            if (conversionType == typeof(byte))
                return Expression.Constant(Array.ConvertAll(values, value => Convert.ToByte(value)));

            if (conversionType == typeof(short?))
                return Expression.Constant(Array.ConvertAll(values, value => value == null ? null : (short?)Convert.ToInt16(value)));
            if (conversionType == typeof(float?))
                return Expression.Constant(Array.ConvertAll(values, value => value == null ? null : (float?)Convert.ToSingle(value)));
            if (conversionType == typeof(double?))
                return Expression.Constant(Array.ConvertAll(values, value => value == null ? null : (double?)Convert.ToDouble(value)));
            if (conversionType == typeof(byte?))
                return Expression.Constant(Array.ConvertAll(values, value => value == null ? null : (byte?)Convert.ToByte(value)));

            return Expression.Constant(Array.ConvertAll(values, value => Convert.ChangeType(value, conversionType)));
        }
    }
}