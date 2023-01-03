namespace BudjetManagement.Models
{
    public class ParamObtainTransactionsByUser
    {
        public int UserId { get; set; }
        public DateTime DateTransactionInitial { get; set; }
        public DateTime DateTransactionFinal { get; set; }

    }
}
