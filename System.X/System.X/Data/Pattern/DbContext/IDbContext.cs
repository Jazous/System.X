namespace Fn.Pattern
{
    public interface IDbContext : System.IDisposable
    {
        int SaveChanges();
        void SyncObjectState<TEntity>(TEntity entity) where TEntity : class, IEntityState;
        void SyncObjectsStatePostCommit();
    }
}