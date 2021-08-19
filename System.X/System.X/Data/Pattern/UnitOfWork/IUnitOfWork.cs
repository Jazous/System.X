using System;
using System.Data;

namespace Fn.Pattern
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntityState;
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        bool Commit();
        void Rollback();
    }
}