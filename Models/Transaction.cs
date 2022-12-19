using System.ComponentModel.DataAnnotations;

namespace BudjetManagement.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int userId { get; set; }
        [Display(Name = "Date of Transaction")]
        [DataType(DataType.Date)]
        public DateTime DateTransaction { get; set; } = DateTime.Today;
        public decimal Price { get; set; }
        [StringLength(maximumLength:1000, ErrorMessage ="Max {1} char")]
        public string Comment { get; set; }
        [Range(1,maximum:int.MaxValue,ErrorMessage ="Must select a valid category")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Must select a valid account")]
        [Display(Name = "Account")]
        public int AccountId { get; set; } 



    }
}
