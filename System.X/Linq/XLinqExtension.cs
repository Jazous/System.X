using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq
{
    public static class XLinqExtension
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate)
        {
            if (condition == false)
                return source;
            return Queryable.Where(source, predicate);
        }
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
            var equals = values.Select(value => (Expression)Expression.Equal(keySelector.Body, Expression.Constant(value, typeof(TKey)))).ToArray();
            if (equals.Length == 1)
                return source.Where(Expression.Lambda<Func<TSource, bool>>(equals[0], keySelector.Parameters));

            var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
            return source.Where(Expression.Lambda<Func<TSource, bool>>(body, keySelector.Parameters));
        }
        public static IQueryable<T> ElementsIn<T, TKey>(this IQueryable<T> source, string propertyName, IEnumerable<TKey> values)
        {
            dynamic keySelector = GetKeySelector<T>(propertyName);
            return ElementsIn<T, TKey>(source, keySelector, values);
        }
        public static IQueryable<TSource> Between<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, TKey startValue, TKey endValue)
        {
            var cstart = Expression.GreaterThanOrEqual(keySelector.Body, Expression.Constant(startValue, typeof(TKey)));
            var cend = Expression.LessThan(keySelector.Body, Expression.Constant(endValue, typeof(TKey)));
            return source.Where(Expression.Lambda<Func<TSource, bool>>(Expression.AndAlso(cstart, cend), keySelector.Parameters));
        }
        public static IQueryable<TSource> Between<TSource, TKey>(this IQueryable<TSource> source,string propertyName, TKey startValue, TKey endValue)
        {
            dynamic keySelector = GetKeySelector<TSource>(propertyName);
            return Between<TSource, TKey>(source, keySelector, startValue, endValue);
        }
        public static (List<T>, int) Page<T>(this IOrderedQueryable<T> source, int pageIndex, int pageSize)
        {
            return (new List<T>(source.Skip(pageIndex).Take(pageSize)), source.Count());
        }

        public static IEnumerable<TSource> ElementsIn<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<TKey> values)
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
            if (source is IList<TSource>)
                return Distinct((IList<TSource>)source, keySelector);

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
        public static List<TSource> Distinct<TSource, TKey>(this IList<TSource> source, Func<TSource, TKey> keySelector)
        {
            var result = new List<TSource>();
            var keys = new List<TKey>();
            for (int i = 0; i < source.Count; i++)
            {
                var key = keySelector.Invoke(source[i]);
                if (keys.Contains(key))
                    continue;

                keys.Add(key);
                result.Add(source[i]);
            }
            return result;
        }
        public static bool Contains(this IEnumerable<string> source, string item, bool ignoreCase)
        {
            return Contains(source, item, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }
        public static bool Contains(this IEnumerable<string> source, string item, StringComparison comparisonType)
        {
            if (source is IList<string>)
                return Contains((IList<string>)source, item, comparisonType);

            foreach (var str in source)
                if (string.Equals(str, item, comparisonType))
                    return true;
            return false;
        }
        static bool Contains(this IList<string> source, string item, bool ignoreCase)
        {
            return Contains(source, item, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }
        static bool Contains(this IList<string> source, string item, StringComparison comparisonType)
        {
            for (int i = 0; i < source.Count; i++)
                if (string.Equals(source[i], item, comparisonType))
                    return true;
            return false;
        }

        internal static LambdaExpression GetKeySelector<T>(string propertyName)
        {
            var signature = Expression.Parameter(typeof(T), "c");
            return Expression.Lambda(Expression.Property(signature, propertyName), signature);
        }
        internal static Expression<Func<TSource, dynamic>> GetPropertySelector<TSource>(string propertyName, out Type propertyType)
        {
            var signature = Linq.Expressions.Expression.Parameter(typeof(TSource), "c");
            var body = Linq.Expressions.Expression.Property(signature, propertyName);
            propertyType = body.Type;
            return Linq.Expressions.Expression.Lambda<Func<TSource, dynamic>>(body, signature);
        }
    }
}