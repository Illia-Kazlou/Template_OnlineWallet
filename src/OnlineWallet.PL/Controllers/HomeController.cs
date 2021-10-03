using Microsoft.AspNetCore.Mvc;
using OnlineWallet.BLL.Interfaces;
using OnlineWallet.PL.Models;
using System;
using System.Threading.Tasks;

namespace OnlineWallet.PL.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly IAccountService _accountService;

        public HomeController(IWalletService walletService, IAccountService accountService)
        {
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        public async Task<IActionResult> IndexAsync()
        {
            var userId = await _accountService.GetUserIdByNameAsync(User.Identity.Name);
            var balance = await _walletService.GetBalanceByUserIdAsync(userId);

            var balanceViewModel = new BalanceViewModel()
            {
                Id = balance.Id,
                Amount = balance.Amount,
                Description = balance.Description,
            };

            ViewBag.Balance = balanceViewModel;

            return View();
        }
    }
}

