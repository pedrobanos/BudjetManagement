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
        private readonly IReportsService reportsService;

        public TransactionsController(IUserService userService,
            IAccountsRepo accountsRepo, ICategoriesRepo categoriesRepo,
            ITransactionsRepo transactionsRepo,IMapper mapper, IReportsService reportsService)
        {
            this.userService = userService;
            this.accountsRepo = accountsRepo;
            this.categoriesRepo = categoriesRepo;
            this.transactionsRepo = transactionsRepo;
            this.mapper = mapper;
            this.reportsService = reportsService;
        }

        public async Task<IActionResult> Index(int month, int year)
        {
            var userId = userService.ObtainUserId();

            var model = await reportsService.ObtainReportTransactionsDetailed(userId, month, year, ViewBag);

            return View(model);
        }

        public async Task<IActionResult> Weekly(int month, int year)
        {
            var userId = userService.ObtainUserId();
            IEnumerable<ResultObtainByWeek> transactionsByWeek =
                    await reportsService.ObtainReportWeekly(userId, month, year, ViewBag);

            var sorted = transactionsByWeek.GroupBy(x => x.Week).Select(x =>
            new ResultObtainByWeek()
            {
                Week = x.Key,
                Incomings = x.Where(x => x.TypeOperationId == TypeOperation.Incomings)
                .Select(x => x.Price).FirstOrDefault(),
                Outgoings = x.Where(x => x.TypeOperationId == TypeOperation.Outgoings)
                .Select(x => x.Price).FirstOrDefault()
            }).ToList();

            if (year == 0 || month == 0)
            {
                var today = DateTime.Today;
                year = today.Year;
                month = today.Month;
            }

            var referenceDate = new DateTime(year, month, 1);
            var daysOfMonth = Enumerable.Range(1, referenceDate.AddMonths(1).AddDays(-1).Day);

            var daysDivided = daysOfMonth.Chunk(7).ToList();
            
            for(int i = 0; i < daysDivided.Count; i++)
            {
                var week = i + 1;
                var dateTransactionInitial = new DateTime(year, month, daysDivided[i].First());
                var dateTransactionFinal = new DateTime(year, month, daysDivided[i].Last());
                var weekSorted = sorted.FirstOrDefault(x => x.Week == week);

                if (weekSorted is null)
                {
                    sorted.Add(new ResultObtainByWeek()
                    {
                        Week = week,
                        DateTransactionInitial = dateTransactionInitial,
                        DateTransactionFinal = dateTransactionFinal
                    });
                }
                else
                {
                    weekSorted.DateTransactionInitial = dateTransactionInitial;
                    weekSorted.DateTransactionFinal = dateTransactionFinal;
                }
            }

            sorted = sorted.OrderByDescending(el => el.Week).ToList();

            var model = new WeeklyReportViewModel();
            model.TransactionsByWeek = sorted;
            model.ReferenceDate = referenceDate;

            return View(model);
        }

        public IActionResult Monthly()
        {
            return View();
        }

        public IActionResult ExcelReport()
        {
            return View();
        }

        public IActionResult Calendary()
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
        public async Task<IActionResult> Delete(int id, string urlReturn = null)
        {
            var userId = userService.ObtainUserId();

            var transaction = await transactionsRepo.ObtainById(id, userId);

            if (transaction is null)
            {
                return RedirectToAction("NotFoundPage", "Index");
            }

            await transactionsRepo.Delete(id);

            if (string.IsNullOrEmpty(urlReturn))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(urlReturn);
            }
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
