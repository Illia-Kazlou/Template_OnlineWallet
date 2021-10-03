using Microsoft.EntityFrameworkCore;
using OnlineWallet.DAL.Context;
using OnlineWallet.DAL.Interfaces;
using OnlineWallet.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineWallet.DAL.Repository
{
    public class SqlOperationRepository : IRepository<Operation>
    {
        private OnlineWalletContext db;

        public SqlOperationRepository(OnlineWalletContext context)
        {
            this.db = context;
        }

        public async Task CreateAsync(Operation operation)
        {
            await db.Operations.AddAsync(operation);
            await db.SaveChangesAsync();
        }

        public async Task<Operation> GetAsync(string id)
        {
            return await db.Operations.FindAsync(id);
        }

        public IQueryable<Operation> GetAll()
        {
            return db.Operations.AsNoTracking();
        }

        public Task UpdateAsync(Operation operation)
        {
            db.Entry(operation).State = EntityState.Modified;
            return db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            Operation operation = await db.Set<Operation>().FindAsync(id);
            if (operation != null)
            {
                db.Operations.Remove(operation);
            }
            await db.SaveChangesAsync();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
