using BudjetManagement.Validations;
using System.ComponentModel.DataAnnotations;

namespace BudjetManagement.Models
{
    public class Account
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 50)]
        [FirstUpperCaseLetter]
        public string Name { get; set; }
        [Display(Name = "Type Account")]
        public int TypeAccountId { get; set; }
        public decimal Balance { get; set; }
        [StringLength(maximumLength: 1000)]
        public string Description { get; set; }
        public string TypeAccount { get; set; }
    }

}
