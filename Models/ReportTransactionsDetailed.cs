namespace BudjetManagement.Models
{
    public class ReportTransactionsDetailed
    {
        public DateTime DateTransactionInitial { get; set; }
        public DateTime DateTransactionFinal { get; set; }
        public IEnumerable<TransactionsByDate> TransactionsListed { get; set; }
        public decimal BalanceDeposits => TransactionsListed.Sum(el => el.BalanceDeposits);
        public decimal BalanceWithdraws => TransactionsListed.Sum(el => el.BalanceWithdraws);
        public decimal Total => BalanceDeposits - Math.Abs(BalanceWithdraws);
        public class TransactionsByDate
        {
            public DateTime DateTransaction { get; set; }
            public IEnumerable<Transaction> Transactions { get; set; }
            public decimal BalanceDeposits => 
                Transactions.Where(el => el.TypeOperationId == TypeOperation.Incomings)
                .Sum(el => el.Price);
            public decimal BalanceWithdraws =>
                Transactions.Where(el => el.TypeOperationId == TypeOperation.Outgoings)
                .Sum(el => el.Price);
        }

    }
}
