using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudjetManagement.Models
{
    public class CreationCategoryViewModel: Category
    {
        public IEnumerable<SelectListItem> TypeOperation { get; set; }
    }
}
