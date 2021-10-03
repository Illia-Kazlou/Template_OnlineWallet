using OnlineWallet.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineWallet.BLL.Interfaces
{
    public interface IWalletService
    {
        Task<bool> AddOperationAsync(Operation newOperation);

        Task EditLastOperationAsync(Operation operation);

        Task<IEnumerable<Operation>> GetAllOperationAsync(string userId);

        Task<Balance> GetBalanceByUserIdAsync(string userId);
    }
}
