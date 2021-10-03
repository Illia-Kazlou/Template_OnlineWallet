using Microsoft.EntityFrameworkCore;
using OnlineWallet.DAL.Context;
using OnlineWallet.DAL.Interfaces;
using OnlineWallet.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineWallet.DAL.Repository
{
    public class SqlBalanceRepository : IRepository<Balance>
    {
        private readonly OnlineWalletContext _db;

        public SqlBalanceRepository(OnlineWalletContext context)
        {
            _db = context;
        }

        public async Task CreateAsync(Balance balance)
        {
            await _db.Balances.AddAsync(balance);
            await _db.SaveChangesAsync();
        }

        public async Task<Balance> GetAsync(string id)
        {
            return await _db.Balances.FindAsync(id);
        }

        public IQueryable<Balance> GetAll()
        {
            return _db.Balances.AsNoTracking();
        }

        public Task UpdateAsync(Balance balance)
        {
            _db.Entry(balance).State = EntityState.Modified;
            return _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            Balance balance = await _db.Set<Balance>().FindAsync(id);
            if (balance != null)
            {
                _db.Balances.Remove(balance);
            }
            await _db.SaveChangesAsync();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
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
