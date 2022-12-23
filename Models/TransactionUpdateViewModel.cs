namespace BudjetManagement.Models
{
    public class TransactionUpdateViewModel: TransactionCreateViewModel
    {
        public int AccountPrevId { get; set; }
        public decimal PricePrev { get; set; }
    }
}
