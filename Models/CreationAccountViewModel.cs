using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudjetManagement.Models
{
    public class CreationAccountViewModel : Account
    {
        public IEnumerable<SelectListItem> TypesAccounts { get; set; }
    }
}
