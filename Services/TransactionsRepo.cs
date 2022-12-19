using BudjetManagement.Models;
using Dapper;
using System.Data.SqlClient;

namespace BudjetManagement.Services
{
    public interface ITransactionsRepo
    {
        Task Create(Transaction transaction);
    }
    public class TransactionsRepo : ITransactionsRepo
    {
        private readonly string connectionString;

        public TransactionsRepo(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Transaction transaction)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("Transactions_Insert",
                new
                {
                    transaction.userId,
                    transaction.DateTransaction,
                    transaction.Price,
                    transaction.CategoryId,
                    transaction.AccountId,
                    transaction.Comment
                },
                commandType: System.Data.CommandType.StoredProcedure);

            transaction.Id = id;

        }
    }
}
