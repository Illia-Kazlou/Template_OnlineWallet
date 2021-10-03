using Microsoft.AspNetCore.Mvc;
using OnlineWallet.BLL.Interfaces;
using OnlineWallet.PL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineWallet.PL.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IAccountService _accountService;

        public ProfileController(IAccountService accountService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService)); ;
        }

        public async Task<IActionResult> Index()
        {
            var userId = await _accountService.GetUserIdByNameAsync(User.Identity.Name);
            var profiles = await _accountService.GetProfileAsync(userId);

            var profileViewModel = new List<ProfileViewModel>() ;

            foreach (var profile in profiles)
            {
                profileViewModel.Add(new ProfileViewModel
                {
                    Id = profile.Id,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    BirthDate = profile.BirthDate,
                    CreationDate = profile.CreationDate,
                    PhoneNumber = profile.PhoneNumber,
                    Email = profile.Email,
                    Avatar = profile.Avatar,
                });
            }

            return View(profileViewModel);
        }
    }
}
