using BudjetManagement.Models;
using Dapper;
using System.Data.SqlClient;

namespace BudjetManagement.Services
{
    public interface ITransactionsRepo
    {
        Task Create(Transaction transaction);
        Task Delete(int id);
        Task<IEnumerable<Transaction>> ObtainByAccoundId(ObtainTransactionsByAccount model);
        Task<Transaction> ObtainById(int id, int userId);
        Task Update(Transaction transaction, decimal pricePrev, int accountPrev);
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

        public async Task<IEnumerable<Transaction>>ObtainByAccoundId(
            ObtainTransactionsByAccount model)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaction>(
                                    @"SELECT t.Id, t.Price, t.DateTransaction, c.Name as Category, 
                                    acc.Name as Account, c.TypeOperationId
                                    FROM Transactions t
                                    INNER JOIN Categories c
                                    ON c.Id = t.CategoryId
                                    INNER JOIN Accounts acc
                                    ON acc.Id = t.AccountId
                                    WHERE t.AccountId = @AccountId AND t.UserId = @UserId
                                    AND DateTransaction BETWEEN @DateTransactionInitial AND @DateTransactionFinal", model);
        }

        public async Task Update (Transaction transaction, decimal pricePrev, int accountPrevId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transactions_Update",
                new
                {
                    transaction.Id,
                    transaction.DateTransaction,
                    transaction.Price,
                    transaction.CategoryId,
                    transaction.AccountId,
                    transaction.Comment,
                    pricePrev, 
                    accountPrevId
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaction> ObtainById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaction>(
                                                    @"SELECT Transactions.*, cat.TypeOperationId
                                                    FROM Transactions
                                                    INNER JOIN Categories cat
                                                    ON cat.Id = Transactions.CategoryId
                                                    WHERE Transactions.Id = @Id AND Transactions.UserId = @UserId",
                                                new { id, userId });
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transactions_delete",
                new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}

