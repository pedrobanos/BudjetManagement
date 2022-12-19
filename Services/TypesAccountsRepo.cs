using BudjetManagement.Models;
using Dapper;
using System.Data.SqlClient;

namespace BudjetManagement.Services
{
    public interface ITypesAccountsRepo
    {
        Task<bool> Exist(string name, int userId);
        Task Create(TypeAccount typeAccount);
        Task<IEnumerable<TypeAccount>> ListItemsAccount(int userId);
        Task Update(TypeAccount typeAccount);
        Task<TypeAccount> ObtainById(int id, int userId);
        Task Delete(int id);
        Task Order(IEnumerable<TypeAccount> typeAccountsOrdered);
    }
    public class TypesAccountsRepo : ITypesAccountsRepo
    {
        private readonly string connectionString;
        public TypesAccountsRepo(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(TypeAccount typeAccount)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>
                                                  ("TypeAccounts_Insert", 
                                                  new {userId = typeAccount.UserId, name = typeAccount.Name},
                                                  commandType: System.Data.CommandType.StoredProcedure);
            typeAccount.Id = id;
        }

        public async Task<bool> Exist(string name, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            var exist = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1
                                                                          FROM TypesAccounts
                                                                          WHERE Name = @Name AND UserId = @UserId;",
                                                                          new {name, userId});
            return exist == 1;
        }

        public async Task<IEnumerable<TypeAccount>> ListItemsAccount(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TypeAccount>(@"SELECT Id, Name, OrderNumber
                                                                FROM TypesAccounts
                                                                WHERE UserId = @UserId
                                                                ORDER BY OrderNumber", new {userId});
        }

        public async Task Update (TypeAccount typeAccount) 
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TypesAccounts
                                                SET Name = @Name
                                                WHERE Id = @Id", typeAccount);
        }
        public async Task<TypeAccount> ObtainById(int id,int userId) 
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TypeAccount>(@"SELECT Id, Name, OrderNumber
                                                                        FROM TypesAccounts
                                                                        WHERE Id = @Id AND UserId = @UserId",
                                                                        new {id, userId});
        }

        public async Task Delete (int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE TypesAccounts
                                             WHERE Id = @Id", new {id});
        }

        public async Task Order(IEnumerable<TypeAccount> typeAccountsOrdered)
        {
            var query = "UPDATE TypesAccounts SET OrderNumber = @OrderNumber WHERE Id = @Id;";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, typeAccountsOrdered);
        }
    }
}
