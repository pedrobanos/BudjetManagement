using AutoMapper;
using BudjetManagement.Models;
using BudjetManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace BudjetManagement.Controllers
{
    public class AccountsController: Controller
    {
        private readonly ITypesAccountsRepo typesAccountsRepo;
        private readonly IUserService userService;
        private readonly IAccountsRepo accountsRepo;
        private readonly IMapper mapper;
        private readonly ITransactionsRepo transactionsRepo;
        private readonly IReportsService reportsService;

        public AccountsController(ITypesAccountsRepo typesAccountsRepo, 
            IUserService userService, IAccountsRepo accountsRepo,IMapper mapper, ITransactionsRepo transactionsRepo,
            IReportsService reportsService)
        {
            this.typesAccountsRepo = typesAccountsRepo;
            this.userService = userService;
            this.accountsRepo = accountsRepo;
            this.mapper = mapper;
            this.transactionsRepo = transactionsRepo;
            this.reportsService = reportsService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = userService.ObtainUserId();
            var accountsWithTypesAccounts = await accountsRepo.Search(userId);

            var modelo = accountsWithTypesAccounts
                .GroupBy(e => e.TypeAccount)
                .Select(group => new IndexAccountsViewModel
                {
                    TypeAccount = group.Key,
                    Accounts = group.AsEnumerable()

                }).ToList();

            return View(modelo);
                                        
        }
        public async Task<IActionResult> Detailed(int id, int month, int year)
        {
            var userId = userService.ObtainUserId();
            var account = await accountsRepo.ObtainById(id, userId);

            if(account is null)
            {
                return RedirectToAction("NotFoundPage", "Index");
            }

            ViewBag.Account = account.Name;
            var model = await reportsService.ObtainReportTransactionsDetailedByAccount(userId, id, month, year, ViewBag);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = userService.ObtainUserId();
            var model = new CreationAccountViewModel();
            model.TypesAccounts = await ObtaintypesAccounts(userId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create (CreationAccountViewModel account)
        {
            var userId = userService.ObtainUserId();
            var typeAccount = await typesAccountsRepo.ObtainById(account.TypeAccountId, userId);

            if (typeAccount is null)
            {
                return RedirectToAction("NotFound","Index");
            }

            if (!ModelState.IsValid)
            {
                account.TypesAccounts = await ObtaintypesAccounts(userId);
                return View(account);
            }

     
            await accountsRepo.Create(account);
            return RedirectToAction("Index");


        }

        public async Task<IActionResult> Update(int id)
        {
            var userId = userService.ObtainUserId();
            var account = await accountsRepo.ObtainById(id, userId);

            if (account is null)
            {
                return RedirectToAction("NotFoundPage","Index");
            }

            var model = mapper.Map<CreationAccountViewModel>(account);  
            model.TypesAccounts= await ObtaintypesAccounts(userId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult>Update(CreationAccountViewModel accountUpdate)
        {
            var userId = userService.ObtainUserId();
            var account = await accountsRepo.ObtainById(accountUpdate.Id, userId);
            if (account is null)
            {
                return RedirectToAction("NotFoundPage", "Index");
            }
            var typeAccount = await typesAccountsRepo.ObtainById(accountUpdate.TypeAccountId, userId);
            
            if (typeAccount is null)
            {
                return RedirectToAction("NotFoundPage", "Index");
            }

            await accountsRepo.Update(accountUpdate);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete (int id)
        {
            var userId = userService.ObtainUserId();
            var account = await accountsRepo.ObtainById(id, userId);
            
            if(account is null)
            {
                return RedirectToAction("NotFoundPage", "Index");
            }
            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var userId = userService.ObtainUserId();
            var account = await accountsRepo.ObtainById(id, userId);

            if (account is null)
            {
                return RedirectToAction("NotFoundPage", "Index");
            }
            await accountsRepo.Delete(id);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtaintypesAccounts(int userId)
        {
            var typesAccounts = await typesAccountsRepo.ListItemsAccount(userId);
            return typesAccounts.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }

        public IActionResult NotFoundPage()
        {
            return View();
        }

    }
}
