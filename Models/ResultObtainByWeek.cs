namespace BudjetManagement.Models
{
    public class ResultObtainByWeek
    {
        public int Week { get; set; }
        public decimal Price { get; set; }
        public TypeOperation TypeOperationId { get; set; }

        public decimal Incomings { get; set; }
        public decimal Outgoings { get; set; }
        public DateTime DateTransactionInitial { get; set; }
        public DateTime DateTransactionFinal { get; set; }
    }
}
