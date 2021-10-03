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
    public class SqlProfileRepository : IRepository<Profile>
    {
        private readonly OnlineWalletContext _db;

        public SqlProfileRepository(OnlineWalletContext context)
        {
            _db = context;
        }

        public async Task CreateAsync(Profile profile)
        {
            await _db.Profiles.AddAsync(profile);
            await _db.SaveChangesAsync();
        }

        public async Task<Profile> GetAsync(string id)
        {
            return await _db.Profiles.FindAsync(id);
        }

        public IQueryable<Profile> GetAll()
        {
            return _db.Profiles.AsNoTracking();
        }

        public Task UpdateAsync(Profile profile)
        {
            _db.Entry(profile).State = EntityState.Modified;
            return _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            Profile profile = await _db.Profiles.FindAsync(id);
            if (profile != null)
            {
                _db.Profiles.Remove(profile);
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
