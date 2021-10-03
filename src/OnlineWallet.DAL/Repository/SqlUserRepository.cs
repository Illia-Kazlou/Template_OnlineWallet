using Microsoft.EntityFrameworkCore;
using OnlineWallet.DAL.Context;
using OnlineWallet.DAL.Interfaces;
using OnlineWallet.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineWallet.DAL.Repository
{
    public class SqlUserRepository : IRepository<User>
    {
        private OnlineWalletContext db;

        public SqlUserRepository(OnlineWalletContext context)
        {
            this.db = context;
        }

        public async Task CreateAsync(User user)
        {
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
        }

        public async Task<User> GetAsync(string id)
        {
            return await db.Users.FindAsync(id);
        }

        public IQueryable<User> GetAll()
        {
            return  db.Users.AsNoTracking();
        }

        public Task UpdateAsync(User user)
        {
            db.Entry(user).State = EntityState.Modified;
            return db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            User user = await db.Users.FindAsync(id);
            if (user != null)
            {
                db.Users.Remove(user);
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
