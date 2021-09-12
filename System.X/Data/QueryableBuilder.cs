using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System.X.Linq
{
    public sealed class QueryableBuilder<T>
    {
        IQueryable<T> Query { get; }


        public QueryableBuilder(IQueryable<T> source)
        {
            var pps = typeof(T).GetFields(Reflection.BindingFlags.GetProperty | Reflection.BindingFlags.Instance | Reflection.BindingFlags.Public);
            this.Query = source;
        }

        public QueryableBuilder<T> AndEquals<TKey>(string propertyName, string[] values)
        {
            var keySelector = GetKeySelector(propertyName);
            var type = keySelector.ReturnType;
            if (type == typeof(int))
            {
                int[] args = Array.ConvertAll(values, c => Convert.ToInt32(c));
                var equals = args.Select(arg => (Expression)Expression.Equal(keySelector.Body, Expression.Constant(arg, type)));
                var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
                Query.Where(Expression.Lambda<Func<T, bool>>(body, keySelector.Parameters));
                return this;
            }
            else if (type == typeof(int?))
            {
                int?[] args = Array.ConvertAll(values, c => Fn.ToInt32(c));
                var equals = args.Select(arg => (Expression)Expression.Equal(keySelector.Body, Expression.Constant(arg, type)));
                var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
                Query.Where((Expression.Lambda<Func<T, bool>>(body, keySelector.Parameters)));
                return this;
            }
            else if (type == typeof(DateTime))
            {
                DateTime[] args = Array.ConvertAll(values, c => Convert.ToDateTime(c));
                var equals = args.Select(arg => (Expression)Expression.Equal(keySelector.Body, Expression.Constant(arg, type)));
                var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
                Query.Where((Expression.Lambda<Func<T, bool>>(body, keySelector.Parameters)));
                return this;
            }
            else if (type == typeof(DateTime?))
            {
                DateTime?[] args = Array.ConvertAll(values, c => Fn.ToDateTime(c));
                var equals = args.Select(arg => (Expression)Expression.Equal(keySelector.Body, Expression.Constant(arg, type)));
                var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
                Query.Where((Expression.Lambda<Func<T, bool>>(body, keySelector.Parameters)));
                return this;
            }
            else if (type == typeof(bool))
            {
                bool[] args = Array.ConvertAll(values, c => Convert.ToBoolean(c));
                var equals = args.Select(arg => (Expression)Expression.Equal(keySelector.Body, Expression.Constant(arg, type)));
                var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
                Query.Where((Expression.Lambda<Func<T, bool>>(body, keySelector.Parameters)));
                return this;
            }

            var equals2 = values.Select(arg => (Expression)Expression.Equal(keySelector.Body, Expression.Constant(arg, type)));
            var body2 = equals2.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
            Query.Where((Expression.Lambda<Func<T, bool>>(body2, keySelector.Parameters)));
            return this;
        }
        internal static LambdaExpression GetKeySelector(string propertyName)
        {
            var signature = Expression.Parameter(typeof(T), "c");
            return Expression.Lambda(Expression.Property(signature, propertyName), signature);
        }
    }
}