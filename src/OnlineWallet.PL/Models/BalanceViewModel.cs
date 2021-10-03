using System.ComponentModel.DataAnnotations;

namespace OnlineWallet.PL.Models
{
    public class BalanceViewModel
    {
        public string Id { get; set; }

        [Required]
        public double? Amount { get; set; }

        public string Description { get; set; }
    }
}
