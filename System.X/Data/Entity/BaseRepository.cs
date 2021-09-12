using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Data.Entity
{
    public abstract class BaseRepository<TEntity> where TEntity : BaseEntity, new()
    {
        protected internal UnitOfWork UnitOfWork { get; }
        readonly DbContext db;
        protected internal BaseRepository(UnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
            this.db = unitOfWork.DbContext;
        }

        public void Insert(TEntity entity)
        {
            this.db.Set<TEntity>().Add(entity);
        }
        public ValueTask<EntityEntry<TEntity>> InsertAsync(TEntity entity)
        {
            return this.db.Set<TEntity>().AddAsync(entity);
        }
        public void InsertRange(IEnumerable<TEntity> entities)
        {
            this.db.Set<TEntity>().AddRange(entities);
        }
        public Task InsertRangeAsync(IEnumerable<TEntity> entities)
        {
            return this.db.Set<TEntity>().AddRangeAsync(entities);
        }

        public void Update(TEntity entity)
        {
            this.db.Entry(entity).Property(c => c.CreateTime).IsModified = false;
            this.db.Set<TEntity>().Update(entity);
        }
        public void Update(TEntity entity, IEnumerable<Linq.Expressions.Expression<Func<TEntity, dynamic>>> includes)
        {
            this.db.Entry(entity).State = EntityState.Unchanged;
            foreach (var prop in includes)
                this.db.Entry(entity).Property(prop).IsModified = true;
            this.db.Set<TEntity>().Update(entity);
        }
        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                this.db.Entry(entity).Property(c => c.CreateTime).IsModified = false;
            this.db.Set<TEntity>().UpdateRange(entities);
        }
        public void UpdateRange(IEnumerable<TEntity> entities, IEnumerable<Linq.Expressions.Expression<Func<TEntity, dynamic>>> includes)
        {
            foreach (var entity in entities)
            {
                this.db.Entry(entity).State = EntityState.Unchanged;
                foreach (var prop in includes)
                    this.db.Entry(entity).Property(prop).IsModified = true;
            }
            this.db.Set<TEntity>().UpdateRange(entities);
        }

        public void SoftDelete(TEntity entity)
        {
            this.db.Entry(entity).State = EntityState.Unchanged;
            entity.IsDeleted = true;
            this.db.Entry(entity).Property(c => c.IsDeleted).IsModified = true;
            this.db.Set<TEntity>().Update(entity);
        }
        public void SoftDeleteRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                this.db.Entry(entity).State = EntityState.Unchanged;
                entity.IsDeleted = true;
                this.db.Entry(entity).Property(c => c.IsDeleted).IsModified = true;
            }
            this.db.Set<TEntity>().UpdateRange(entities);
        }
        public void Delete(TEntity entity)
        {
            this.db.Set<TEntity>().Remove(entity);
        }
        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            this.db.Set<TEntity>().RemoveRange(entities);
        }

        public TEntity GetById(params object[] ids)
        {
            return this.db.Set<TEntity>().Find(ids);
        }
        public ValueTask<TEntity> GetByIdAsync(params object[] ids)
        {
            return this.db.Set<TEntity>().FindAsync(ids);
        }
        public List<TEntity> GetByKeys<TKey>(Linq.Expressions.Expression<Func<TEntity, TKey>> keySelector, IEnumerable<TKey> values)
        {
            return this.db.Set<TEntity>().AsNoTracking().ElementsIn(keySelector, values).ToList();
        }
        public List<TResult> GetByKeys<TKey, TResult>(Linq.Expressions.Expression<Func<TEntity, TKey>> keySelector, IEnumerable<TKey> values, Func<TEntity, TResult> selector)
        {
            return this.db.Set<TEntity>().AsNoTracking().ElementsIn(keySelector, values).Select(selector).ToList();
        }
        public Task<List<TEntity>> GetByKeysAsync<TKey>(Linq.Expressions.Expression<Func<TEntity, TKey>> keySelector, IEnumerable<TKey> values)
        {
            return this.db.Set<TEntity>().AsNoTracking().ElementsIn(keySelector, values).ToListAsync();
        }

        protected virtual IOrderedQueryable<TEntity> BuildQuery(IEnumerable<QueryItem> queryItems)
        {
            var query = db.Set<TEntity>().AsNoTracking().Where(c => !c.IsDeleted);

            foreach (var item in queryItems)
            {
                if (item.Name.StartsWith("#"))
                    continue;

                var predicate = X.Linq.ExpressionHelper.Instance.LambdaBuild<TEntity>(item.Name, item.Mode, item.Values);
                if (predicate != null)
                    query = query.Where(predicate);
            }
            var sorts = queryItems.Where(c => string.Equals(c.Name, "#SortPropertyName", StringComparison.OrdinalIgnoreCase)).ToArray();
            if (sorts.Length == 0)
                return query.OrderByDescending(c => c.CreateTime);

            var sort = sorts[0];
            var desc = Fn.ToBoolean(sort.Values[0]);
            IOrderedQueryable<TEntity> result = desc ? query.OrderByDescending(sort.Name) : query.OrderBy(sort.Name);
            for (int i = 1; i < sorts.Length; i++)
            {
                sort = sorts[i];
                desc = Fn.ToBoolean(sort.Values[i]);
                result = desc ? result.ThenByDescending(sort.Name) : result.ThenBy(sort.Name);
            }
            return result;
        }
        public List<TEntity> GetAll()
        {
            return this.db.Set<TEntity>().ToList();
        }
        public List<TResult> GetAll<TResult>(Func<TEntity, TResult> selector)
        {
            return this.db.Set<TEntity>().AsNoTracking().Select(selector).ToList();
        }
        public List<TEntity> GetAll(Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return this.db.Set<TEntity>().Where(predicate).ToList();
        }
        public List<TResult> GetAll<TResult>(Linq.Expressions.Expression<Func<TEntity, bool>> predicate, Func<TEntity, TResult> selector)
        {
            return this.db.Set<TEntity>().AsNoTracking().Where(predicate).Select(selector).ToList();
        }
        public List<TEntity> GetAll(IEnumerable<QueryItem> queryItems)
        {
            return BuildQuery(queryItems).ToList();
        }
        public List<TResult> GetAll<TResult>(IEnumerable<QueryItem> queryItems, Func<TEntity, TResult> selector)
        {
            return BuildQuery(queryItems).Select(selector).ToList();
        }
        public (List<TEntity>, int) GetAll(IEnumerable<QueryItem> queryItems, int pageIndex, int pageSize)
        {
            var query = BuildQuery(queryItems);
            return (query.Skip(pageIndex * pageSize).Take(pageSize).ToList(), query.Count());
        }
        public (List<TResult>, int) GetAll<TResult>(IEnumerable<QueryItem> queryItems, int pageIndex, int pageSize, Func<TEntity, TResult> selector)
        {
            var query = BuildQuery(queryItems);
            return (query.Skip(pageIndex * pageSize).Take(pageSize).Select(selector).ToList(), query.Count());
        }


        public Task<List<TEntity>> GetAllAsync()
        {
            return this.db.Set<TEntity>().ToListAsync();
        }
        public Task<List<TEntity>> GetAllAsync(Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return this.db.Set<TEntity>().Where(predicate).ToListAsync();
        }
        public Task<List<TEntity>> GetAllAsync(IEnumerable<QueryItem> queryItems)
        {
            return BuildQuery(queryItems).ToListAsync();
        }
        public async Task<(List<TEntity>, int)> GetAllAsync(IEnumerable<QueryItem> queryItems, int pageIndex, int pageSize)
        {
            var query = BuildQuery(queryItems);
            return (await query.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(), await query.CountAsync());
        }


        public bool Exists(Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return this.db.Set<TEntity>().AsNoTracking().Any(predicate);
        }
        public Task<bool> ExistsAsync(Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return this.db.Set<TEntity>().AsNoTracking().AnyAsync(predicate);
        }
        public TEntity FirstOrDefault(Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return this.db.Set<TEntity>().AsNoTracking().FirstOrDefault(predicate);
        }
        public TEntity FirstOrDefault<TKey>(Linq.Expressions.Expression<Func<TEntity, bool>> predicate, Linq.Expressions.Expression<Func<TEntity, TKey>> keySelector, bool ascending)
        {
            return ascending
                ? this.db.Set<TEntity>().AsNoTracking().OrderBy(keySelector).FirstOrDefault(predicate)
                : this.db.Set<TEntity>().AsNoTracking().OrderByDescending(keySelector).FirstOrDefault(predicate);
        }
        public Task<TEntity> FirstOrDefaultAsync(Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return this.db.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(predicate);
        }
        public Task<TEntity> FirstOrDefaultAsync<TKey>(Linq.Expressions.Expression<Func<TEntity, bool>> predicate, Linq.Expressions.Expression<Func<TEntity, TKey>> keySelector, bool ascending)
        {
            return ascending
                ? this.db.Set<TEntity>().AsNoTracking().OrderBy(keySelector).FirstOrDefaultAsync(predicate)
                : this.db.Set<TEntity>().AsNoTracking().OrderByDescending(keySelector).FirstOrDefaultAsync(predicate);
        }
        public int GetCount(Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return this.db.Set<TEntity>().AsNoTracking().Count(predicate);
        }
        public Task<int> GetCountAsync(Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return this.db.Set<TEntity>().AsNoTracking().CountAsync(predicate);
        }
        public decimal GetSum(Linq.Expressions.Expression<Func<TEntity, decimal>> predicate)
        {
            return this.db.Set<TEntity>().AsNoTracking().Sum(predicate);
        }
        public Task<decimal> GetSumAsync(Linq.Expressions.Expression<Func<TEntity, decimal>> predicate)
        {
            return this.db.Set<TEntity>().AsNoTracking().SumAsync(predicate);
        }
    }
}