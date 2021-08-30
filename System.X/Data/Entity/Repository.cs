using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Data.Entity
{
    public abstract class Repository<TSource> where TSource : BaseEntity, new()
    {
        readonly DbContext db;
        protected Repository(DbContext context)
        {
            this.db = context;
        }

        public void Insert(TSource entity)
        {
            this.db.Set<TSource>().Add(entity);
        }
        public ValueTask<EntityEntry<TSource>> InsertAsync(TSource entity)
        {
            return this.db.Set<TSource>().AddAsync(entity);
        }
        public void InsertRange(IEnumerable<TSource> entities)
        {
            this.db.Set<TSource>().AddRange(entities);
        }
        public Task InsertRangeAsync(IEnumerable<TSource> entities)
        {
            return this.db.Set<TSource>().AddRangeAsync(entities);
        }

        public void Update(TSource entity)
        {
            this.db.Entry(entity).State = EntityState.Modified;
            this.db.Entry(entity).Property(c => c.CreateTime).IsModified = false;
            this.db.Set<TSource>().Update(entity);
        }
        public void Update(TSource entity, IEnumerable<Linq.Expressions.Expression<Func<TSource, dynamic>>> includes)
        {
            this.db.Entry(entity).State = EntityState.Unchanged;
            foreach (var prop in includes)
                this.db.Entry(entity).Property(prop).IsModified = true;
            this.db.Set<TSource>().Update(entity);
        }
        public void UpdateRange(IEnumerable<TSource> entities)
        {
            foreach (var entity in entities)
                this.db.Entry(entity).Property(c => c.CreateTime).IsModified = false;
            this.db.Set<TSource>().UpdateRange(entities);
        }
        public void UpdateRange(IEnumerable<TSource> entities, IEnumerable<Linq.Expressions.Expression<Func<TSource, dynamic>>> includes)
        {
            foreach (var entity in entities)
                Update(entity, includes);
        }

        public void SoftDelete(TSource entity)
        {
            this.db.Entry(entity).State = EntityState.Unchanged;
            entity.IsDeleted = true;
            this.db.Entry(entity).Property(c => c.IsDeleted).IsModified = true;
            this.db.Set<TSource>().Update(entity);
        }
        public void SoftDeleteRange(IEnumerable<TSource> entities)
        {
            foreach (var entity in entities)
            {
                this.db.Entry(entity).State = EntityState.Unchanged;
                entity.IsDeleted = true;
                this.db.Entry(entity).Property(c => c.IsDeleted).IsModified = true;
            }
            this.db.Set<TSource>().UpdateRange(entities);
        }
        public void Delete(TSource entity)
        {
            this.db.Set<TSource>().Remove(entity);
        }
        public void DeleteRange(IEnumerable<TSource> entities)
        {
            this.db.Set<TSource>().RemoveRange(entities);
        }

        public TSource Get(params object[] ids)
        {
            return this.db.Set<TSource>().Find(ids);
        }
        public ValueTask<TSource> GetAsync(params object[] ids)
        {
            return this.db.Set<TSource>().FindAsync(ids);
        }

        protected virtual IQueryable<TSource> BuildQuery(IEnumerable<QueryItem> queryItems)
        {
            var query = db.Set<TSource>().AsNoTracking().Where(c => !c.IsDeleted);
            foreach (var item in queryItems)
            {
                var tbs = item.Name.Split('.');
                switch (item.Mode)
                {
                    case QueryMode.Between:
                        break;
                    case QueryMode.Contains:
                        break;

                }
            }
            return query;
        }

        public (List<TSource>, int) GetAll(IEnumerable<QueryItem> queryItems, int pageIndex, int pageSize)
        {
            var query = BuildQuery(queryItems);
            return (query.Skip(pageIndex * pageSize).Take(pageSize).ToList(), query.Count());
        }
        public async Task<(List<TSource>, int)> GetAllAsync(IEnumerable<QueryItem> queryItems, int pageIndex, int pageSize)
        {
            var query = BuildQuery(queryItems);
            return (await query.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(), await query.CountAsync());
        }
        public List<TSource> GetAll(IEnumerable<QueryItem> queryItems)
        {
            return BuildQuery(queryItems).ToList();
        }
        public Task<List<TSource>> GetAllAsync(IEnumerable<QueryItem> queryItems)
        {
            return BuildQuery(queryItems).ToListAsync();
        }

        public List<TSource> GetAll()
        {
            return this.db.Set<TSource>().ToList();
        }
        public List<TSource> GetAll(Linq.Expressions.Expression<Func<TSource, bool>> predicate)
        {
            return this.db.Set<TSource>().Where(predicate).ToList();
        }

        public Task<List<TSource>> GetAllAsync()
        {
            return this.db.Set<TSource>().ToListAsync();
        }
        public Task<List<TSource>> GetAllAsync(Linq.Expressions.Expression<Func<TSource, bool>> predicate)
        {
            return this.db.Set<TSource>().Where(predicate).ToListAsync();
        }
    }
}