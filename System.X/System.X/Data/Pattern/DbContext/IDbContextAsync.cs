using System.Threading;
using System.Threading.Tasks;

namespace Fn.Pattern
{
    public interface IDbContextAsync : IDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync();
    }
}