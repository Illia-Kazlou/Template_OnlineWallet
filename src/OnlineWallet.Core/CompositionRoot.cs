using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineWallet.BLL.Interfaces;
using OnlineWallet.BLL.Managers;
using OnlineWallet.DAL.Context;
using OnlineWallet.DAL.Interfaces;
using OnlineWallet.DAL.Repository;
using OnlineWallet.Domain;

namespace OnlineWallet.Core
{
    public class CompositionRoot
    {
        public static void InjectDependencies(IServiceCollection services)
        {

            services.AddScoped<OnlineWalletContext>();

            services.AddScoped<IRepository<User>, SqlUserRepository>();
            services.AddScoped<IRepository<Profile>, SqlProfileRepository>();
            services.AddScoped<IRepository<Balance>, SqlBalanceRepository>();
            services.AddScoped<IRepository<Operation>, SqlOperationRepository>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IWalletService, OperationService>();

        }
    }
}
