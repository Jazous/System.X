using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity
{
    public class UnitOfWork : IDisposable
    {
        protected internal DbContext DbContext { get; }

        public UnitOfWork(DbContext context)
        {
            this.DbContext = context;
        }

        public IDbContextTransaction BeginTransaction()
        {
            return this.DbContext.Database.BeginTransaction();
        }

        public int SaveChanges()
        {
            return this.DbContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return this.DbContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            if (this.DbContext != null)
                this.DbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}