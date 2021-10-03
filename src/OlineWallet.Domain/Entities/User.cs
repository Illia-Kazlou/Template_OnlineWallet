using Microsoft.AspNetCore.Identity;

namespace OnlineWallet.Domain
{
    public class User : IdentityUser
    {
        public Balance Balance { get; set; }

        public Profile Profile { get; set; }
    }
}