using BudjetManagement.Models;
using Dapper;
using System.Data.SqlClient;

namespace BudjetManagement.Services
{
    public interface ICategoriesRepo
    {
        Task Create(Category category);
        Task Delete(int id);
        Task<IEnumerable<Category>> ListItemsCategory(int userId);
        Task<IEnumerable<Category>> ListItemsCategory(int userId, TypeOperation typeOperationId);
        Task<Category> ObtainById(int id, int userId);
        Task Update(Category category);
    }
    public class CategoriesRepo : ICategoriesRepo
    {
        private readonly string connectionString;
        public CategoriesRepo(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Category category)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Categories (Name, TypeOperationId, UserId)
                                                              VALUES(@Name, @TypeOperationId, @UserId);

                                                              SELECT SCOPE_IDENTITY();", category);
            category.Id = id;
        }

        public async Task<IEnumerable<Category>>ListItemsCategory(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Category>(
                @"SELECT * FROM Categories
                WHERE UserId = @UserId", new {userId});

        }

        public async Task<IEnumerable<Category>> ListItemsCategory(int userId, TypeOperation typeOperationId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Category>(
                @"SELECT * 
                FROM Categories
                WHERE UserId = @userId AND TypeOperationId = @typeOperationId", 
                new { userId, typeOperationId });

        }

        public async Task<Category> ObtainById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Category>(@"SELECT *
                                                                        FROM Categories
                                                                        WHERE Id = @Id AND UserId =@UserId;", new { id, userId });
        }

        public async Task Update (Category category)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.ExecuteAsync(@"UPDATE Categories
                                                    SET Name = @Name, TypeOperationId = @TypeOperationId
                                                    WHERE Id = @Id", category);
            category.Id = id;

        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE Categories
                                            WHERE Id=@Id", new { id });
        }
    }
}
