namespace System.Data.Entity
{
    public interface IRepository<TEntity>
    {
        void Add(TEntity entity);
        void AddRange(System.Collections.Generic.IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void UpdateRange(System.Collections.Generic.IEnumerable<TEntity> entities);
        void Delete(TEntity entity);
        void DeleteRange(System.Collections.Generic.IEnumerable<TEntity> entities);
        TEntity Find(params object[] keys);
        TEntity Find(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate);
        System.Linq.IQueryable<TEntity> FindAll(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate);
        System.Collections.Generic.PagedList<TEntity> FindAll(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate, int pageIndex, int pageSize);
        bool Contains(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate);
        System.Linq.IQueryable<TEntity> Query(string sql, params object[] parameters);
    }
}