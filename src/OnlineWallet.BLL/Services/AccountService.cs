using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineWallet.BLL.Interfaces;
using OnlineWallet.DAL.Interfaces;
using OnlineWallet.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineWallet.BLL.Managers
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userRepository;
        private readonly IRepository<Profile> _profileRepository;
        private readonly IRepository<Balance> _balanceRepository;

        public AccountService(UserManager<User> userRepository,
                              IRepository<Profile> profileRepository,
                              IRepository<Balance> balanceRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
        }

        public async Task<IdentityResult> RegisterAsync(string userName, string email, string password)
        {
            var user = new User()
            {
                Email = email,
                UserName = userName,
            };

            var result = await _userRepository.CreateAsync(user, password);

            await CreateDefaultProfileAsync(userName, email);
            await CreateDefaultBalanceAsync(userName);

            return result;
        }

        private async Task CreateDefaultBalanceAsync(string userName)
        {
            var userId = await GetUserIdByNameAsync(userName);

            var userBalance = new Balance()
            {
                UserId = userId,
                Amount = 0,
                Description = "BYN"
            };

            await _balanceRepository.CreateAsync(userBalance);
        }

        private async Task CreateDefaultProfileAsync(string userName, string email)
        {
            var userId = await GetUserIdByNameAsync(userName);

            var userProfile = new Profile()
            {
                Avatar = null,
                FirstName = userName,
                BirthDate = DateTime.MinValue,
                CreationDate = DateTime.Now,
                Email = email,
                UserId = userId,
                LastName = "null",
                PhoneNumber = null,
            };

            await _profileRepository.CreateAsync(userProfile);
        }
        
        public async Task EditProfileAsync(Profile newProfile)
        {
            if (await GetUserByIdAsync(newProfile.Id) == null)
            {
                var existingProfile = await _profileRepository.GetAsync(newProfile.Id);

                static bool CheckToUpdate(Profile existingProfile, Profile newProfile)
                {
                    bool updated = false;

                    if (existingProfile.FirstName != newProfile.FirstName)
                    {
                        existingProfile.FirstName = newProfile.FirstName;
                        updated = true;
                    }

                    if (existingProfile.LastName != newProfile.LastName)
                    {
                        existingProfile.LastName = newProfile.LastName;
                        updated = true;
                    }

                    if (existingProfile.BirthDate != newProfile.BirthDate)
                    {
                        existingProfile.BirthDate = newProfile.BirthDate;
                        updated = true;
                    }

                    if (existingProfile.PhoneNumber != newProfile.PhoneNumber)
                    {
                        existingProfile.PhoneNumber = newProfile.PhoneNumber;
                        updated = true;
                    }

                    if (existingProfile.Email != newProfile.Email)
                    {
                        existingProfile.Email = newProfile.Email;
                        updated = true;
                    }

                    if (existingProfile.Avatar != newProfile.Avatar)
                    {
                        existingProfile.Avatar = newProfile.Avatar;
                        updated = true;
                    }

                    return updated;
                }

                var result = CheckToUpdate(existingProfile, newProfile);

                if (result)
                {
                    await _profileRepository.UpdateAsync(newProfile);
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _userRepository.FindByIdAsync(id);
        }

        public async Task<string> GetUserIdByNameAsync(string userName)
        {
            var user = await _userRepository.Users.FirstAsync(u => u.UserName == userName);

            return user.Id;
        }

        public async Task<IEnumerable<Profile>> GetProfileAsync(string userId)
        {
            var listProfile = await _profileRepository
                .GetAll()
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .ToListAsync();

            var newProfile = new List<Profile>();

            foreach (var profile in listProfile)
            {
                newProfile.Add(new Profile
                {
                    Id = profile.Id,
                    UserId = profile.UserId,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    BirthDate = profile.BirthDate,
                    CreationDate =profile.CreationDate,
                    PhoneNumber = profile.PhoneNumber,
                    Email = profile.Email,
                    Avatar = profile.Avatar,
                });
            }

            return newProfile;
        }
        
        public async Task DeleteUserAsync(string id)
        {
            var user = await GetUserByIdAsync(id);

            if (user != null)
            {
                await _userRepository.DeleteAsync(user);
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
