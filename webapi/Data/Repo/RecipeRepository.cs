using Npgsql;
using webapi.Interfaces;
using webapi.Models;

namespace webapi.Data.Repo
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly NpgsqlConnection _dbConnection;

        public RecipeRepository(NpgsqlConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<Recipe>> GetRecipes()
        {
            List<Recipe> list = new List<Recipe>();
            string query = "SELECT id, name, description FROM recipes";

            await _dbConnection.OpenAsync();
            await using var command = new NpgsqlCommand(query, _dbConnection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Recipe
                        {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Description = reader["description"].ToString(),
                        });
            }

            await _dbConnection.CloseAsync();

            return list;
        }

        public async Task<int> AddRecipe(Recipe recipe)
        {
            string query = "INSERT INTO recipes (name, created_by) VALUES (@name, @created_by)";

            await _dbConnection.OpenAsync();

            await using var command = new NpgsqlCommand(query, _dbConnection);

            command.Parameters.AddWithValue("name", recipe.Name);
            command.Parameters.AddWithValue("created_by", 1);

            await command.ExecuteScalarAsync();

            await _dbConnection.CloseAsync();

            return 1;
        }
    }
}

