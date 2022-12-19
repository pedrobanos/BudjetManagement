namespace BudjetManagement.Models
{
    public class IndexAccountsViewModel
    {
        public string TypeAccount { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public decimal Balance => Accounts.Sum(e => e.Balance);
    }
}
