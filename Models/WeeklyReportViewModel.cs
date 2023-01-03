namespace BudjetManagement.Models
{
    public class WeeklyReportViewModel
    {
        public decimal  Incomings => TransactionsByWeek.Sum(x => x.Incomings);
        public decimal Outgoings => TransactionsByWeek.Sum(x => x.Outgoings);
        public decimal Total => Incomings - Outgoings;
        public DateTime ReferenceDate { get; set; }
        public IEnumerable<ResultObtainByWeek> TransactionsByWeek{ get; set; }
    }
}
