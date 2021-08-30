using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq
{
    public static class XLinqExtension
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            dynamic keySelector = GetKeySelector<T>(propertyName);
            return Queryable.OrderBy(source, keySelector);
        }
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            dynamic keySelector = GetKeySelector<T>(propertyName);
            return Queryable.OrderByDescending(source, keySelector);
        }
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            dynamic keySelector = GetKeySelector<T>(propertyName);
            return Queryable.ThenBy(source, keySelector);
        }
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            dynamic keySelector = GetKeySelector<T>(propertyName);
            return Queryable.ThenByDescending(source, keySelector);
        }
        public static IQueryable<TSource> ElementsIn<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IEnumerable<TKey> values)
        {
            var equals = values.Select(value => (Expression)Expression.Equal(keySelector.Body, Expression.Constant(value, typeof(TKey))));
            var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
            return source.Where(Expression.Lambda<Func<TSource, bool>>(body, keySelector.Parameters));
        }
        public static IQueryable<TSource> Between<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, TKey startValue, TKey endValue)
        {
            var cstart = Expression.GreaterThanOrEqual(keySelector.Body, Expression.Constant(startValue, typeof(TKey)));
            var cend = Expression.LessThan(keySelector.Body, Expression.Constant(endValue, typeof(TKey)));
            return source.Where(Expression.Lambda<Func<TSource, bool>>(Expression.AndAlso(cstart, cend), keySelector.Parameters));
        }
        public static (List<T>, int) Page<T>(this IOrderedQueryable<T> source, int pageIndex, int pageSize)
        {
            return (new List<T>(source.Skip(pageIndex).Take(pageSize)), source.Count());
        }
        public static IQueryable<T> AndEquals<T>(this IQueryable<T> source, string propertyName, string[] values)
        {
            var keySelector = GetKeySelector<T>(propertyName);
            var type = keySelector.ReturnType;
            if (type == typeof(int))
            {
                int[] args = Array.ConvertAll(values, c => Convert.ToInt32(c));
                var equals = args.Select(arg => (Expression)Expression.Equal(keySelector.Body, Expression.Constant(arg, type)));
                var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
                return Queryable.Where(source, (Expression.Lambda<Func<T, bool>>(body, keySelector.Parameters)));
            }
            else if (type == typeof(int?))
            {
                int?[] args = Array.ConvertAll(values, c => Fn.ToInt32(c));
                var equals = args.Select(arg => (Expression)Expression.Equal(keySelector.Body, Expression.Constant(arg, type)));
                var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
                return Queryable.Where(source, (Expression.Lambda<Func<T, bool>>(body, keySelector.Parameters)));
            }
            else if (type == typeof(DateTime))
            {
                DateTime[] args = Array.ConvertAll(values, c => Convert.ToDateTime(c));
                var equals = args.Select(arg => (Expression)Expression.Equal(keySelector.Body, Expression.Constant(arg, type)));
                var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
                return Queryable.Where(source, (Expression.Lambda<Func<T, bool>>(body, keySelector.Parameters)));
            }
            else if (type == typeof(DateTime?))
            {
                DateTime?[] args = Array.ConvertAll(values, c => Fn.ToDateTime(c));
                var equals = args.Select(arg => (Expression)Expression.Equal(keySelector.Body, Expression.Constant(arg, type)));
                var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
                return Queryable.Where(source, (Expression.Lambda<Func<T, bool>>(body, keySelector.Parameters)));
            }
            else if (type == typeof(bool))
            {
                bool[] args = Array.ConvertAll(values, c => Convert.ToBoolean(c));
                var equals = args.Select(arg => (Expression)Expression.Equal(keySelector.Body, Expression.Constant(arg, type)));
                var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
                return Queryable.Where(source, (Expression.Lambda<Func<T, bool>>(body, keySelector.Parameters)));
            }

            var equals2 = values.Select(arg => (Expression)Expression.Equal(keySelector.Body, Expression.Constant(arg, type)));
            var body2 = equals2.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
            return Queryable.Where(source, (Expression.Lambda<Func<T, bool>>(body2, keySelector.Parameters)));
        }

        public static IEnumerable<TSource> ElementsIn<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, TKey[] values)
        {
            return source.Where(c => values.Contains(keySelector.Invoke(c)));
        }
        public static (List<T>, int) Page<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            if (source is ICollection<T>)
                return (new List<T>(source.Skip(pageIndex).Take(pageSize)), ((ICollection<T>)source).Count);
            return (new List<T>(source.Skip(pageIndex).Take(pageSize)), source.Count());
        }
        public static List<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var result = new List<TSource>();
            var keys = new List<TKey>();
            foreach (var item in source)
            {
                var key = keySelector.Invoke(item);
                if (keys.Contains(key))
                    continue;

                keys.Add(key);
                result.Add(item);
            }
            return result;
        }
        public static TSource GetValue<TKey, TSource>(this IDictionary<TKey, TSource> source, TKey key) where TSource : class
        {
            TSource result;
            return source.TryGetValue(key, out result) ? result : null;
        }

        internal static LambdaExpression GetKeySelector<T>(string propertyName)
        {
            var signature = Expression.Parameter(typeof(T), "c");
            return Expression.Lambda(Expression.Property(signature, propertyName), signature);
        }
    }
}