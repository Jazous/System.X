using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Data.Entity
{
    public abstract class Repository<T> where T : BaseEntity, new()
    {
        readonly DbContext db;
        protected Repository(DbContext context)
        {
            this.db = context;
        }

        public void Insert(T entity)
        {
            this.db.Set<T>().Add(entity);
        }
        public ValueTask<EntityEntry<T>> InsertAsync(T entity)
        {
            return this.db.Set<T>().AddAsync(entity);
        }
        public void InsertRange(IEnumerable<T> entities)
        {
            this.db.Set<T>().AddRange(entities);
        }
        public Task InsertRangeAsync(IEnumerable<T> entities)
        {
            return this.db.Set<T>().AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            this.db.Set<T>().Update(entity);
        }
        public void UpdateRange(IEnumerable<T> entities)
        {
            this.db.Set<T>().UpdateRange(entities);
        }

        public void SoftDelete(string id)
        {
            T entity = new T();
            entity.Id = id;
            entity.IsDeleted = true;
            this.db.Entry<T>(entity).Property(c => c.IsDeleted).IsModified = true;
            this.db.Set<T>().Update(entity);
        }
        public void SoftDeleteRange(IEnumerable<string> ids)
        {
            List<T> entities = new List<T>();
            foreach (var id in ids)
            {
                T entity = new T();
                entity.Id = id;
                entity.IsDeleted = true;
                this.db.Entry<T>(entity).Property(c => c.IsDeleted).IsModified = true;
                entities.Add(entity);
            }
            this.db.Set<T>().UpdateRange(entities);
        }
        public void Delete(T entity)
        {
            this.db.Set<T>().Remove(entity);
        }
        public void DeleteRange(IEnumerable<T> entities)
        {
            this.db.Set<T>().RemoveRange(entities);
        }

        public T Get(string id)
        {
            return this.db.Set<T>().Find(id);
        }
        public ValueTask<T> GetAsync(string id)
        {
            return this.db.Set<T>().FindAsync(id);
        }

        public virtual (List<T>, PageInfo) Get(QueryFilter filter)
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll()
        {
            return this.db.Set<T>().ToList();
        }
        public List<T> GetAll(Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return this.db.Set<T>().Where(predicate).ToList();
        }

        public Task<List<T>> GetAllAsync()
        {
            return this.db.Set<T>().ToListAsync();
        }
        public Task<List<T>> GetAllAsync(Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return this.db.Set<T>().Where(predicate).ToListAsync();
        }
    }
}