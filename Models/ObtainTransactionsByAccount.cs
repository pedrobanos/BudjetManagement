namespace BudjetManagement.Models
{
    public class ObtainTransactionsByAccount
    {
        public int UserId { get; set; }
        public int AccountId { get; set; }
        public DateTime DateTransactionInitial { get; set; }
        public DateTime DateTransactionFinal { get; set; }

    }
}
