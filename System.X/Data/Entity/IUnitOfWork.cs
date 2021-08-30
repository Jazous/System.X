using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Repository<T> Repositories<T>() where T : BaseEntity, new();
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        bool Commit();
        void Rollback();
    }
}