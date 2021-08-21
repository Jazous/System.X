namespace System.Linq
{
    public static class XLinqExtension
    {
        /// <summary>
        /// 根据属性按升序对序列的元素排序。
        /// </summary>
        /// <typeparam name="TSource">source 中的元素的类型。</typeparam>
        /// <param name="source">一个要排序的值序列。</param>
        /// <param name="propertyName">用于排序的属性。</param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string propertyName)
        {
            dynamic keySelector = GetKeySelector<TSource>(propertyName);
            return Queryable.OrderBy(source, keySelector);
        }
        /// <summary>
        /// 根据属性按降序对序列的元素排序。
        /// </summary>
        /// <typeparam name="TSource">source 中的元素的类型。</typeparam>
        /// <param name="source">一个要排序的值序列。</param>
        /// <param name="propertyName">用于排序的属性。</param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> OrderByDescending<TSource>(this IQueryable<TSource> source, string propertyName)
        {
            dynamic keySelector = GetKeySelector<TSource>(propertyName);
            return Queryable.OrderByDescending(source, keySelector);
        }
        /// <summary>
        /// 根据某个属性按升序对序列中的元素执行后续排序。
        /// </summary>
        /// <typeparam name="TSource">source 中的元素的类型。</typeparam>
        /// <param name="source">一个 System.Linq.IOrderedQueryable`1，包含要排序的元素。</param>
        /// <param name="propertyName">用于排序的属性。</param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> ThenBy<TSource>(this IOrderedQueryable<TSource> source, string propertyName)
        {
            dynamic keySelector = GetKeySelector<TSource>(propertyName);
            return Queryable.ThenBy(source, keySelector);
        }
        /// <summary>
        /// 根据某个属性按降序对序列中的元素执行后续排序。
        /// </summary>
        /// <typeparam name="TSource">source 中的元素的类型。</typeparam>
        /// <param name="source">一个 System.Linq.IOrderedQueryable`1，包含要排序的元素。</param>
        /// <param name="propertyName">用于排序的属性。</param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> ThenByDescending<TSource>(this IOrderedQueryable<TSource> source, string propertyName)
        {
            dynamic keySelector = GetKeySelector<TSource>(propertyName);
            return Queryable.ThenByDescending(source, keySelector);
        }
        /// <summary>
        /// 返回某个键的值在指定值范围内序列。
        /// </summary>
        /// <typeparam name="TSource">source 中的元素的类型。</typeparam>
        /// <typeparam name="TKey">由 keySelector 表示的函数返回的键类型。</typeparam>
        /// <typeparam name="TValue">由 keySelector 表示的函数返回的键的值的类型。</typeparam>
        /// <param name="source">source 中的元素的类型。</param>
        /// <param name="keySelector">用于从元素中提取键的函数。</param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IQueryable<TSource> ElementsIn<TSource, TKey, TValue>(this IQueryable<TSource> source, Expressions.Expression<Func<TSource, TKey>> keySelector, Collections.Generic.IEnumerable<TValue> values)
        {
            var equals = values.Select(value => (Expressions.Expression)Expressions.Expression.Equal(keySelector.Body, Expressions.Expression.Constant(Convert.ChangeType(value, keySelector.ReturnType), keySelector.ReturnType)));
            var body = equals.Aggregate<Expressions.Expression>((accumulate, equal) => Expressions.Expression.Or(accumulate, equal));
            return source.Where(Expressions.Expression.Lambda<Func<TSource, bool>>(body, keySelector.Parameters));
        }

        public static Collections.Generic.PagedList<T> ToPagedList<T>(this IOrderedEnumerable<T> source, int pageIndex, int pageSize)
        {
            var result = new System.Collections.Generic.PagedList<T>(source.Skip(pageIndex).Take(pageSize));
            result.TotalCount = source.Count();
            return result;
        }
        public static Collections.Generic.PagedList<T> ToPagedList<T>(this IOrderedQueryable<T> source, int pageIndex, int pageSize)
        {
            var result = new System.Collections.Generic.PagedList<T>(source.Skip(pageIndex).Take(pageSize));
            result.TotalCount = source.Count();
            return result;
        }

        internal static Expressions.LambdaExpression GetKeySelector<T>(string propertyName)
        {
            var signature = Expressions.Expression.Parameter(typeof(T), "c");
            return Expressions.Expression.Lambda(Expressions.Expression.Property(signature, propertyName), signature);
        }
    }
}