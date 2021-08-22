namespace System.Data.Entity
{
    public interface IService<T> where T : class, new()
    {
        void Insert(T entity);
        void InsertRange(System.Collections.Generic.IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(System.Collections.Generic.IEnumerable<T> entities);
        void Delete(T entity);
        void DeleteRange(System.Collections.Generic.IEnumerable<T> entities);
        T Find(string keys);
        T Find(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        System.Linq.IQueryable<T> FindAll(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        (Collections.Generic.List<T>, PageInfo) FindAll(System.Linq.Expressions.Expression<Func<T, bool>> predicate, int pageIndex, int pageSize);
    }
}