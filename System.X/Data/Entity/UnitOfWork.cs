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
        DbContext db;

        public UnitOfWork(DbContext context)
        {
            this.db = context;
        }

        public IDbContextTransaction BeginTransaction()
        {
            return this.db.Database.BeginTransaction();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return db.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            if (db != null)
                db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}