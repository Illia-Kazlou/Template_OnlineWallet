using OnlineWallet.Domain.Enums;

namespace OlineWallet.Domain.Extensions
{
    public static class OperationCategoryExtension
    {
        public static string ToLocal(this OperationCategory operationCategory)
        {
            return operationCategory switch
            {
                OperationCategory.Credit => "Credit",
                OperationCategory.Loan => "Loan",
                OperationCategory.MoneyBox => "MoneyBox",
                OperationCategory.Other => "Other",
                _ => "Other",
            };
        }
        
        public static OperationCategory ToOperationCategory(int operationCategory)
        {
            return operationCategory switch
            {
                (int)OperationCategory.Credit => OperationCategory.Credit,
                (int)OperationCategory.Loan => OperationCategory.Loan,
                (int)OperationCategory.MoneyBox => OperationCategory.MoneyBox,
                (int)OperationCategory.Other => OperationCategory.Other,
                _ => OperationCategory.Other,
            };
        }

        public static string ValidateOperationCategory(this OperationCategory operationCategory)
        {
            return operationCategory switch
            {
                OperationCategory.Credit => "category__credit",
                OperationCategory.Loan => "category__loan",
                OperationCategory.MoneyBox => "category__moneybox",
                OperationCategory.Other => "category__other",
                _ => "type__outcome",
            };
        }
    }
}
