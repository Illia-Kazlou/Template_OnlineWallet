using OnlineWallet.Domain.Enums;
using OnlineWallet.Domain.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineWallet.Domain
{
    public class Operation : IHasDbIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity), Key()]
        public string Id { get; set; }

        public string Name { get; set; }
        public double Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }

        public OperationType OperationTypes { get; set; }
        public OperationCategory OperationCategories { get; set; }
        public double? Percent { get; set; }
        public bool Planned { get; set; }
        public double? Residue { get; set; }
        public string Description { get; set; }

        public string BalanceId { get; set; }
        public Balance Balance { get; set; }
    }
}