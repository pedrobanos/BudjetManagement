using System.ComponentModel.DataAnnotations;

namespace BudjetManagement.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="The field {0} is required")]
        [StringLength(maximumLength:50, ErrorMessage ="Cant be higher than {1} char")]
        public string Name { get; set; }
        [Display(Name="Type Operation")]
        public TypeOperation TypeOperationId { get; set; }
        public int UserId { get; set; }
    }
}
