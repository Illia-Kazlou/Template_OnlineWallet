using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineWallet.DAL.Interfaces
{
    public interface IRepository<T> : IDisposable
        where T : class
    {
        Task CreateAsync(T entity);

        Task<T> GetAsync(string id);

        IQueryable<T> GetAll();

        Task UpdateAsync(T entity);

        Task DeleteAsync(string id);
    }
}
