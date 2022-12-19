using BudjetManagement.Models;
using BudjetManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudjetManagement.Controllers
{
    public class TransactionsController: Controller
    {
        private readonly IUserService userService;
        private readonly IAccountsRepo accountsRepo;
        private readonly ICategoriesRepo categoriesRepo;

        public TransactionsController(IUserService userService,
            IAccountsRepo accountsRepo, ICategoriesRepo categoriesRepo)
        {
            this.userService = userService;
            this.accountsRepo = accountsRepo;
            this.categoriesRepo = categoriesRepo;
        }

        public async Task<IActionResult> Create()
        {
            var userId = userService.ObtainUserId();
            var model = new TransactionCreateViewModel();
            model.Accounts = await ObtainAccounts(userId);
            return View(model);
        }

        private async Task<IEnumerable<SelectListItem>>ObtainAccounts(int userId)
        {
            var accounts = await accountsRepo.Search(userId);
            return accounts.Select(el => new SelectListItem(el.Name, el.Id.ToString()));
            
        }

        private async Task<IEnumerable<SelectListItem>> ObtainCategories(int userId, TypeOperation typeOperation)
        {
            var categories = await categoriesRepo.ListItemsCategory(userId, typeOperation);
            return categories.Select(el => new SelectListItem(el.Name, el.Id.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> ObtainCategories([FromBody] TypeOperation typeOperation)
        {
            var userId = userService.ObtainUserId();
            var categories = await ObtainCategories(userId, typeOperation);
            return Ok(categories);
        }
    }
}
