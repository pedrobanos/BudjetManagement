using AutoMapper;
using BudjetManagement.Models;
using BudjetManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.Common;
using System.Reflection;
using System.Transactions;
using Transaction = BudjetManagement.Models.Transaction;

namespace BudjetManagement.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly IUserService userService;
        private readonly IAccountsRepo accountsRepo;
        private readonly ICategoriesRepo categoriesRepo;
        private readonly ITransactionsRepo transactionsRepo;
        private readonly IMapper mapper;

        public TransactionsController(IUserService userService,
            IAccountsRepo accountsRepo, ICategoriesRepo categoriesRepo,
            ITransactionsRepo transactionsRepo,IMapper mapper)
        {
            this.userService = userService;
            this.accountsRepo = accountsRepo;
            this.categoriesRepo = categoriesRepo;
            this.transactionsRepo = transactionsRepo;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var userId = userService.ObtainUserId();
            var model = new TransactionCreateViewModel();
            model.Accounts = await ObtainAccounts(userId);
            model.Categories = await ObtainCategories(userId, model.TypeOperationId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransactionCreateViewModel model)
        {
            var userId = userService.ObtainUserId();

            if (!ModelState.IsValid)
            {
                model.Accounts = await ObtainAccounts(userId);
                model.Categories = await ObtainCategories(userId, model.TypeOperationId);
                return View(model);
            }
            var account = await accountsRepo.ObtainById(model.AccountId, userId);

            if (account is null)
            {
                return RedirectToAction("NotFoundPage", "Index");
            }

            var category = await categoriesRepo.ObtainById(model.CategoryId, userId);

            if (category is null)
            {
                return RedirectToAction("NotFoundPage", "Index");
            }

            model.userId = userId;
            if (model.TypeOperationId == TypeOperation.Outgoings)
            {
                model.Price *= -1;
            }

            await transactionsRepo.Create(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id, string urlReturn = null)
        {
            var userId = userService.ObtainUserId();
            var transaction = await transactionsRepo.ObtainById(id, userId);

            if(transaction is null)
            {
                return RedirectToAction("NotFoundPage", "Index");
            }

            var model = mapper.Map<TransactionUpdateViewModel>(transaction);

            model.PricePrev= model.Price;

            if(model.TypeOperationId == TypeOperation.Outgoings)
            {
                model.PricePrev *= -1;
            }

            model.AccountPrevId = transaction.AccountId;
            model.Categories = await ObtainCategories(userId, transaction.TypeOperationId);
            model.Accounts = await ObtainAccounts(userId);
            model.UrlReturn= urlReturn;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update (TransactionUpdateViewModel model)
        {
            var userId = userService.ObtainUserId();

            if (!ModelState.IsValid)
            {
                model.Accounts = await ObtainAccounts(userId);
                model.Categories = await ObtainCategories(userId, model.TypeOperationId);
                return View(model);
            }

            var account = await accountsRepo.ObtainById(model.AccountId, userId);

            if(account is null)
            {
                return RedirectToAction("NotFoundPage", "Index");
            }

            var category = await categoriesRepo.ObtainById(model.CategoryId, userId);

            if (category is null)
            {
                return RedirectToAction("NotFoundPage", "Index");
            }

           var transaction = mapper.Map<Transaction> (model);

            if (model.TypeOperationId == TypeOperation.Outgoings)
            {
                model.Price *= -1;
            }

            await transactionsRepo.Update(transaction, 
                model.PricePrev, 
                model.AccountPrevId);
            if (string.IsNullOrEmpty(model.UrlReturn))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(model.UrlReturn);
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = userService.ObtainUserId();

            var transaction = await transactionsRepo.ObtainById(id, userId);

            if (transaction is null)
            {
                return RedirectToAction("NotFoundPage", "Index");
            }

            await transactionsRepo.Delete(id);

            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtainAccounts(int userId)
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
