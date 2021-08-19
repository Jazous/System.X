namespace System.Data.Entity
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        IRepository<TEntity> Repository<TEntity>();
        void BeginTransaction(System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.Unspecified);
        bool Commit();
        void Rollback();
    }
}