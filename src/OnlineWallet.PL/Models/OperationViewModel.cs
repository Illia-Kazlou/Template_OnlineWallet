using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineWallet.PL.Models
{
    public class OperationViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Amount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }

        public bool Planned { get; set; }

        [Required]
        public string OperationType { get; set; }

        [Required]
        public string OperationCategory { get; set; }

        public double? Percent { get; set; }

        public double? Residue { get; set; }

        public string Description { get; set; }
    }
}
