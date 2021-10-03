using OnlineWallet.Domain.Enums;

namespace OlineWallet.Domain.Extensions
{
    public static class OperationTypeExtension
    {
        public static string ToLocal(this OperationType operationType)
        {
            return operationType switch
            {
                OperationType.Income => "Income",
                OperationType.Outcome => "Outcome",
                _ => "Outcome",
            };
        }
        
        public static OperationType ToOperationType(int operationType)
        {
            return operationType switch
            {
                (int)OperationType.Income => OperationType.Income,
                (int)OperationType.Outcome => OperationType.Outcome,
                _ => OperationType.Outcome,
            };
        }

        public static string ValidateOperationType(this OperationType operationType)
        {
            return operationType switch
            {
                OperationType.Income => "type__income",
                OperationType.Outcome => "type__outcome",
                _ => "type__outcome",
            };
        }
    }
}
