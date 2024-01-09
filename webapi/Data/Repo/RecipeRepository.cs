using System.Text.Json;
using System.Transactions;
using Newtonsoft.Json;
using Npgsql;
using webapi.Interfaces;
using webapi.Models.Basic;
using webapi.Models.Extended;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace webapi.Data.Repo
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly NpgsqlConnection _dbConnection;

        private readonly ITagRepository _tagRepository;
        private readonly IRatingRepository _ratingRepository;

        public RecipeRepository(NpgsqlConnection dbConnection, ITagRepository tagRepository, IRatingRepository ratingRepository)
        {
            _dbConnection = dbConnection;
            _tagRepository = tagRepository;
            _ratingRepository = ratingRepository;
        }

        public async Task<Recipe> GetRecipeById(int recipeId)
        {
            string query = @"
                SELECT
                re.id as id,
                re.name as name,
                re.description as description
                    FROM recipes re
                    left join ratings ra on ra.recipe = re.id
                    left join recipe_tags rta on rta.recipe = re.id
                    left join tags ta on ta.id = rta.tag
                where re.id = @recipe_id
                group by re.id
                    ";

            await _dbConnection.OpenAsync();
            await using var command = new NpgsqlCommand(query, _dbConnection);
            command.Parameters.AddWithValue("recipe_id", recipeId);
            await using var reader = await command.ExecuteReaderAsync();

            Recipe recipe = new Recipe() { Id = -1 };

            while (await reader.ReadAsync())
            {
                recipe = new Recipe()
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Description = reader["Description"].ToString(),
                    Name = reader["Name"].ToString(),
                };
            }

            await _dbConnection.CloseAsync();

            recipe.Tags = await _tagRepository.GetTagsByRecipeId(recipeId);
            recipe.Ratings = await _ratingRepository.GetRatingsByRecipeId(recipeId);

            return recipe;
        }

        public async Task<List<DetailRecipe>> GetRecipes()
        {
            List<DetailRecipe> list = new List<DetailRecipe>();
            string query = @"
                SELECT
                re.id as id,
                re.name as name,
                re.description as description,
                avg(ra.rating) as avg_rating,
                array_agg(row_to_json(ra)) as ratings,
                array_agg(row_to_json(ta)) as tags
                    FROM recipes re
                    left join ratings ra on ra.recipe = re.id
                    left join recipe_tags rta on rta.recipe = re.id
                    left join tags ta on ta.id = rta.tag
                    group by re.id
                    ";

            await _dbConnection.OpenAsync();
            await using var command = new NpgsqlCommand(query, _dbConnection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // var ratingsArray = reader["ratings"] as string[];
                // var tagsArray = reader["tags"] as string[];

                list.Add(new DetailRecipe()
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Description = reader["Description"].ToString(),
                    Name = reader["Name"].ToString(),
                    AvgRating = reader["avg_rating"] != DBNull.Value ? Convert.ToSingle(reader["avg_rating"]) : null,
                    // Ratings = ratingsArray != null ? GetRatingsOfJson(ratingsArray) : new List<Rating>(),
                    // Tags = tagsArray != null ? GetTagsOfJson(tagsArray) : new List<Tag>()
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
            return value ?? (object)DBNull.Value;
        }

        public async Task<int> PutRecipe(Recipe recipe)
        {
            await _dbConnection.OpenAsync();

            await using var transaction = await _dbConnection.BeginTransactionAsync();

            try
            {
                await UpdateRecipe(recipe, transaction);

                await _tagRepository.DeleteTagsByRecipeId(recipe.Id, transaction);
                await _tagRepository.AddTagsByRecipeId(recipe.Tags, recipe.Id, transaction);

                await _ratingRepository.DeleteRatingsByRecipeId(recipe.Id, transaction);
                await _ratingRepository.AddRatingsByRecipeId(recipe.Ratings, recipe.Id, transaction);

                await transaction.CommitAsync();
                await _dbConnection.CloseAsync();
                return recipe.Id ?? -1;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await _dbConnection.CloseAsync();
                Console.WriteLine(ex);
                return -1;
            }

        }

        public async Task<int> UpdateRecipe(Recipe recipe, NpgsqlTransaction transaction)
        {
                string query = @"
                    UPDATE recipes
                    SET name = @name,
                        description = @description,
                        preptime = @preptime,
                        cooktime = @cooktime,
                        worktime = @worktime,
                        difficulty = @difficulty,
                        created_by = @created_by
                    WHERE id = @recipe_id
                ";

                await using var command = new NpgsqlCommand(query, _dbConnection);
                command.Transaction = transaction;

                command.Parameters.AddWithValue("recipe_id", CheckNull(recipe.Id));
                command.Parameters.AddWithValue("name", CheckNull(recipe.Name));
                command.Parameters.AddWithValue("description", CheckNull(recipe.Description));
                command.Parameters.AddWithValue("preptime", CheckNull(recipe.Preptime));
                command.Parameters.AddWithValue("cooktime", CheckNull(recipe.Cooktime));
                command.Parameters.AddWithValue("worktime", CheckNull(recipe.Worktime));
                command.Parameters.AddWithValue("difficulty", CheckNull(recipe.Difficulty));
                command.Parameters.AddWithValue("created_by", 1);

                await command.ExecuteScalarAsync();

                return 0;
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

                int recipeId = (await command.ExecuteScalarAsync() as int?) ?? -1;
                if (recipeId == -1) return -1;

                // inserting into recipe_tags
                await _tagRepository.AddTagsByRecipeId(recipe.Tags, recipeId, transaction);

                // inserting into ratings
                await _ratingRepository.AddRatingsByRecipeId(recipe.Ratings, recipeId, transaction);
                // query = @"
                //     INSERT INTO ratings
                //     (recipe, user_id, rating)
                //     VALUES (@recipe, @user_id, @rating)
                //     ";
                //
                // command.CommandText = query;
                // command.Parameters.Clear();
                //
                // NpgsqlParameter ratingRecipeParam = command.Parameters.AddWithValue("recipe", recipeId);
                // NpgsqlParameter ratingUserParam = command.Parameters.AddWithValue("user_id", 0);
                // NpgsqlParameter ratingValueParam = command.Parameters.AddWithValue("rating", 0);
                //
                // foreach (Rating rating in recipe.Ratings)
                // {
                //     ratingValueParam.Value = rating.Value;
                //     ratingUserParam.Value = rating.User;
                //     await command.ExecuteNonQueryAsync();
                // }
                //
                // await transaction.CommitAsync();
                // await _dbConnection.CloseAsync();
                return recipeId;
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

