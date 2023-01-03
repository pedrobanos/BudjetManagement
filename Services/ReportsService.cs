using BudjetManagement.Models;
using System.Security.Principal;

namespace BudjetManagement.Services
{
    public interface IReportsService
    {
        Task<ReportTransactionsDetailed> ObtainReportTransactionsDetailed(int userId, int month, int year, dynamic ViewBag);
        Task<ReportTransactionsDetailed> ObtainReportTransactionsDetailedByAccount(int userId, int accountId, int month, int year, dynamic ViewBag);
        Task<IEnumerable<ResultObtainByWeek>> ObtainReportWeekly(int userId, int month, int year, dynamic ViewBag);
    }
    public class ReportsService: IReportsService
    {
        private readonly ITransactionsRepo transactionsRepo;
        private readonly HttpContext httpContext;

        public ReportsService(ITransactionsRepo transactionsRepo,IHttpContextAccessor httpContextAccessor)
        {
            this.transactionsRepo = transactionsRepo;
            this.httpContext = httpContextAccessor.HttpContext;
        }

        public async Task <IEnumerable<ResultObtainByWeek>>ObtainReportWeekly(int userId, int month, int year, dynamic ViewBag)
        {
            (DateTime dateTransactionInitial, DateTime dateTransactionFinal) = GenerateDateInitialToFinal(month, year);
            var param = new ParamObtainTransactionsByUser()
            {
                UserId = userId,
                DateTransactionInitial = dateTransactionInitial,
                DateTransactionFinal = dateTransactionFinal,
            };

            AsignValuesViewBag(ViewBag, dateTransactionInitial);
            var model = await transactionsRepo.ObtainByWeek(param);
            return model;

        }

        public async Task<ReportTransactionsDetailed>
            ObtainReportTransactionsDetailed(int userId, int month, int year, dynamic ViewBag)
        {
            (DateTime dateTransactionInitial, DateTime dateTransactionFinal) = GenerateDateInitialToFinal(month, year);

            var param = new ParamObtainTransactionsByUser()
            {
                UserId = userId,
                DateTransactionInitial = dateTransactionInitial,
                DateTransactionFinal = dateTransactionFinal,
            };

            var transactions = await transactionsRepo.ObtainByUserId(param);
            var model = GenerateReportTransactionsDetailed(dateTransactionInitial, dateTransactionFinal, transactions);
            AsignValuesViewBag(ViewBag, dateTransactionInitial);

            return model;

        }
        public async Task<ReportTransactionsDetailed> ObtainReportTransactionsDetailedByAccount(
            int userId, int accountId, int month, int year, dynamic ViewBag)
        {
            (DateTime dateTransactionInitial, DateTime dateTransactionFinal) = GenerateDateInitialToFinal(month, year);
            var obtainTransactionsByAccount = new ObtainTransactionsByAccount()
            {
                AccountId = accountId,
                UserId = userId,
                DateTransactionInitial = dateTransactionInitial,
                DateTransactionFinal = dateTransactionFinal
            };
            var transactions = await transactionsRepo
                .ObtainByAccoundId(obtainTransactionsByAccount);
            var model = GenerateReportTransactionsDetailed(dateTransactionInitial, dateTransactionFinal, transactions);

            AsignValuesViewBag(ViewBag, dateTransactionInitial);

            return model;

        }

        private void AsignValuesViewBag(dynamic ViewBag, DateTime dateTransactionInitial)
        {
            ViewBag.prevMonth = dateTransactionInitial.AddMonths(-1).Month;
            ViewBag.prevYear = dateTransactionInitial.AddMonths(-1).Year;
            ViewBag.nextMonth = dateTransactionInitial.AddMonths(1).Month;
            ViewBag.nextYear = dateTransactionInitial.AddMonths(1).Year;
            ViewBag.urlReturn = httpContext.Request.Path + httpContext.Request.QueryString;
        }

        private static ReportTransactionsDetailed GenerateReportTransactionsDetailed(DateTime dateTransactionInitial, DateTime dateTransactionFinal, IEnumerable<Transaction> transactions)
        {
            var model = new ReportTransactionsDetailed();


            var transactionsByDate = transactions.OrderByDescending(el => el.DateTransaction)
                           .GroupBy(el => el.DateTransaction)
                           .Select(group => new ReportTransactionsDetailed.TransactionsByDate()
                           {
                               DateTransaction = group.Key,
                               Transactions = group.AsEnumerable()
                           });

            model.TransactionsListed = transactionsByDate;
            model.DateTransactionInitial = dateTransactionInitial;
            model.DateTransactionFinal = dateTransactionFinal;
            return model;
        }

        private (DateTime dateTransactionInitial, DateTime dateTransactionFinal) GenerateDateInitialToFinal(int month, int year)
        {
            DateTime dateTransactionInitial;
            DateTime dateTransactionFinal;

            if (month <= 0 || month > 12 || year <= 1900)
            {
                var today = DateTime.Today;
                dateTransactionInitial = new DateTime(today.Year, today.Month, 1);
            }
            else
            {
                dateTransactionInitial = new DateTime(year, month, 1);
            }

            dateTransactionFinal = dateTransactionInitial.AddMonths(1).AddDays(-1);

            return (dateTransactionInitial, dateTransactionFinal);
        }
    }
}
