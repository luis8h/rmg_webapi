using Npgsql;
using webapi.Interfaces;
using webapi.Models.Basic;

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
            string query = @"
                SELECT re.id as id, re.name as name, re.description as description, avg(ra.rating) as avg_rating
                FROM recipes re
                left join ratings ra on ra.recipe = re.id
                group by re.id
            ";

            await _dbConnection.OpenAsync();
            await using var command = new NpgsqlCommand(query, _dbConnection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Recipe ()
                        {
                        Id = Convert.ToInt32(reader["id"]),
                        Description = reader["description"].ToString() ?? "",
                        Name = reader["Name"].ToString(),
                        // AvgRating = Convert.ToSingle(reader["avg_rating"]),
                        // AvgRating = reader["avg_rating"] != DBNull.Value ? Convert.ToSingle(reader["avg_rating"]) : -1.0f,
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
            await _dbConnection.OpenAsync();

            await using var transaction = await _dbConnection.BeginTransactionAsync();

            try
            {
                // inserting into recipes
                string query = @"
                    INSERT INTO recipes
                    (name, description, preptime, cooktime, worktime, difficulty, created_by)
                    VALUES (@name, @description, @preptime, @cooktime, @worktime, @difficulty, @created_by)
                    returning id
                ";

                await using var command = new NpgsqlCommand(query, _dbConnection);
                command.Transaction = transaction;

                command.Parameters.AddWithValue("name", CheckNull(recipe.Name));
                command.Parameters.AddWithValue("description", CheckNull(recipe.Description));
                command.Parameters.AddWithValue("preptime", CheckNull(recipe.Preptime));
                command.Parameters.AddWithValue("cooktime", CheckNull(recipe.Cooktime));
                command.Parameters.AddWithValue("worktime", CheckNull(recipe.Worktime));
                command.Parameters.AddWithValue("difficulty", CheckNull(recipe.Difficulty));
                command.Parameters.AddWithValue("created_by", 1);

                int recipeId = (int) await command.ExecuteScalarAsync();


                // inserting into recipe_tags
                query = @"
                    INSERT INTO recipe_tags
                    (tag, recipe)
                    VALUES (@tag, @recipe)
                    ";

                command.CommandText = query;
                command.Parameters.Clear();

                NpgsqlParameter tagParam = command.Parameters.AddWithValue("tag", 0);
                NpgsqlParameter recipeParam = command.Parameters.AddWithValue("recipe", recipeId);

                foreach (Tag tag in recipe.Tags)
                {
                    tagParam.Value = tag.Id!;
                    await command.ExecuteNonQueryAsync();
                }

                // inserting into ratings
                query = @"
                    INSERT INTO ratings
                    (recipe, user_id, rating)
                    VALUES (@recipe, @user_id, @rating)
                    ";

                command.CommandText = query;
                command.Parameters.Clear();

                NpgsqlParameter ratingRecipeParam = command.Parameters.AddWithValue("recipe", recipeId);
                NpgsqlParameter ratingUserParam = command.Parameters.AddWithValue("user_id", 0);
                NpgsqlParameter ratingValueParam = command.Parameters.AddWithValue("rating", 0);

                foreach (Rating rating in recipe.Ratings)
                {
                    ratingValueParam.Value = rating.Value;
                    ratingUserParam.Value = rating.User;
                    await command.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                await _dbConnection.CloseAsync();
                return 0;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await _dbConnection.CloseAsync();
                Console.WriteLine(ex);
                return 1;
            }

        }
    }
}

