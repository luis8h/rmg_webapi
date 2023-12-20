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
                list.Add(new Recipe (reader["name"].ToString() ?? "")
                        {
                        Id = Convert.ToInt32(reader["id"]),
                        Description = reader["description"].ToString(),
                        });
            }

            await _dbConnection.CloseAsync();

            return list;
        }

        private object CheckNull(int? value)
        {
            return value.HasValue ? (object)value.Value : DBNull.Value;
        }

        private object CheckNull(string? value)
        {
            return value ?? (object) DBNull.Value;
        }

        public async Task<int> AddRecipe(Recipe recipe)
        {
            string queryInsertRecipe = @"
                INSERT INTO recipes
                    (name, description, preptime, cooktime, worktime, difficulty, created_by)
                    VALUES (@name, @description, @preptime, @cooktime, @worktime, @difficulty, @created_by)
            ";


            await _dbConnection.OpenAsync();

            await using var command = new NpgsqlCommand(queryInsertRecipe, _dbConnection);

            command.Parameters.AddWithValue("name", recipe.Name);
            command.Parameters.AddWithValue("description", CheckNull(recipe.Description));
            command.Parameters.AddWithValue("preptime", CheckNull(recipe.Preptime));
            command.Parameters.AddWithValue("cooktime",  CheckNull(recipe.Cooktime));
            command.Parameters.AddWithValue("worktime", CheckNull(recipe.Worktime));
            command.Parameters.AddWithValue("difficulty", CheckNull(recipe.Difficulty));
            command.Parameters.AddWithValue("created_by", 1);

            await command.ExecuteScalarAsync();

            string queryInsertRecipeTags = @"
                INSERT INTO recipe_tags
                    (tag, recipe)
                    VALUES (@tag, @recipe)
            ";

            foreach (Tag tag in recipe.Tags)
            {
                await using var command2 = new NpgsqlCommand(queryInsertRecipeTags, _dbConnection);

                command2.Parameters.AddWithValue("tag", tag.Id!);
                command2.Parameters.AddWithValue("recipe", 2);

                await command2.ExecuteScalarAsync();
            }

            await _dbConnection.CloseAsync();

            return 1;
        }
    }
}

