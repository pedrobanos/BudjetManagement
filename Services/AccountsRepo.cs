 using BudjetManagement.Models;
using Dapper;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace BudjetManagement.Services
{
    public interface IAccountsRepo
    {
        Task<IEnumerable<Account>> Search(int userId);
        Task Create(Account account);
        Task<Account> ObtainById(int id, int userId);
        Task Update(CreationAccountViewModel account);
        Task Delete(int id);
    }
    public class AccountsRepo: IAccountsRepo
    {
        private readonly string connectionString;
        public AccountsRepo(IConfiguration configuration) 
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Account account)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                @"INSERT INTO Accounts(Name, TypeAccountId, Description, Balance)
                VALUES(@Name, @TypeAccountId, @Description, @Balance);
                
                SELECT SCOPE_IDENTITY()", account);

            account.Id = id;

        }

        public async Task<IEnumerable<Account>> Search (int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Account>(@"SELECT Accounts.Id, Accounts.Name, Balance, ta.Name AS TypeAccount 
                                                        FROM Accounts
                                                        INNER JOIN TypesAccounts ta
                                                        ON ta.Id = Accounts.TypeAccountId
                                                        WHERE ta.UserId = @UserId
                                                        ORDER BY ta.OrderNumber;", new {userId});
        }

     

        public async Task<Account>ObtainById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Account>(@"SELECT Accounts.Id, Accounts.Name, Balance, Description, ta.Id 
                                                                        FROM Accounts
                                                                        INNER JOIN TypesAccounts ta
                                                                        ON ta.Id = Accounts.TypeAccountId
                                                                        WHERE ta.UserId = @UserId AND Accounts.Id =@Id;", new {id, userId});
        }

        public async Task Update(CreationAccountViewModel account)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Accounts
                                           SET Name = @Name, Balance = @Balance/100, Description = @Description,
                                           TypeAccountId = @TypeAccountId
                                           WHERE Id = @Id", account);
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE Accounts WHERE Id=@Id", new {id});
        }
    }
}


