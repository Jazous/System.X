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
        ExpressionHelper() { }

        public Expression<Func<TSource, bool>> LambdaBuild<TSource>(string propertyName, QueryMode mode, params dynamic[] values)
        {
            if (mode != QueryMode.NotNull && (values == null || !values.Any()))
                return null;

            switch (mode)
            {
                case QueryMode.Equal: return Equal<TSource>(propertyName, values[0]);
                case QueryMode.NotEqual: return NotEqual<TSource>(propertyName, values[0]);
                case QueryMode.GreaterThan: return GreaterThan<TSource>(propertyName, values[0]);
                case QueryMode.GreaterThanOrEqual: return GreaterThanOrEqual<TSource>(propertyName, values[0]);
                case QueryMode.LessThan: return LessThan<TSource>(propertyName, values[0]);
                case QueryMode.LessThanOrEqual: return LessThanOrEqual<TSource>(propertyName, values[0]);
                case QueryMode.Between: return Between<TSource>(propertyName, values[0], values[1]);
                case QueryMode.Contains: break;
                case QueryMode.StartWith: break;
                case QueryMode.EndWith: break;
                case QueryMode.Except: break;
                case QueryMode.In: return ElementsIn<TSource>(propertyName, values);
                case QueryMode.NotNull: return NotEqual<TSource>(propertyName, null);
            }
            return null;
        }
        public Expression<Func<TSource, bool>> Equal<TSource>(string propertyName, dynamic value)
        {
            var prop = LambdaProperty<TSource>(propertyName);
            var body = Expression.Equal(prop.Body, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType));
            return Expression.Lambda<Func<TSource, bool>>(body, prop.Parameters);
        }
        public Expression<Func<TSource, bool>> NotEqual<TSource>(string propertyName, dynamic value)
        {
            var prop = LambdaProperty<TSource>(propertyName);
            var body = Expression.NotEqual(prop.Body, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType));
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
        public Expression<Func<TSource, bool>> Between<TSource>(string propertyName, dynamic startValue, dynamic endValue)
        {
            var prop = LambdaProperty<TSource>(propertyName);
            var cstart = Expression.GreaterThanOrEqual(prop.Body, Expression.Constant(Convert.ChangeType(startValue, prop.ReturnType), prop.ReturnType));
            var cend = Expression.LessThan(prop.Body, Expression.Constant(Convert.ChangeType(endValue, prop.ReturnType), prop.ReturnType));
            return Expression.Lambda<Func<TSource, bool>>(Expression.AndAlso(cstart, cend), prop.Parameters);
        }
        public Expression<Func<TSource, bool>> ElementsIn<TSource>(string propertyName, params dynamic[] values)
        {
            var prop = LambdaProperty<TSource>(propertyName);
            var equals = values.Select(value => (Expression)Expression.Equal(prop.Body, Expression.Constant(Convert.ChangeType(value, prop.ReturnType), prop.ReturnType))).ToArray();
            if (equals.Length == 1)
                return Expression.Lambda<Func<TSource, bool>>(equals[0], prop.Parameters);

            var body = equals.Aggregate((accumulate, equal) => Expression.Or(accumulate, equal));
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
    }
}