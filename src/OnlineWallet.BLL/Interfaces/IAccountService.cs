using Microsoft.AspNetCore.Identity;
using OnlineWallet.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineWallet.BLL.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(string firstName, string email, string password);

        Task<IEnumerable<Profile>> GetProfileAsync(string userId);

        Task DeleteUserAsync(string id);

        Task EditProfileAsync(Profile profile);

        Task<string> GetUserIdByNameAsync(string userName);
    }
}
