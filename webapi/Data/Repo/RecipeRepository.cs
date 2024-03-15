using Npgsql;
using webapi.Interfaces;
using webapi.Models.Basic;
using webapi.Models.Extended;

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

        private int? nullOrInt(Object value)
        {
            return value == System.DBNull.Value ? null : Convert.ToInt32(value);
        }

        public async Task<List<DetailRecipe>> GetRecipesWithTagsAndRatings()
        {
            string query = @"
                    SELECT
                    re.id as recipe_id,
                    re.name as recipe_name,
                    re.description as recipe_description,
                    re.preptime as preptime,
                    re.cooktime as cooktime,
                    re.worktime as worktime,
                    re.difficulty as difficulty,
                    ta.id as tag_id,
                    ta.name as tag_name,
                    ra.user_id as rating_user_id,
                    ra.rating as rating_value,
                    ra.id as rating_id,
                    AVG(ra.rating) OVER (PARTITION BY re.id) as average_rating
                    FROM
                    recipes re
                    LEFT JOIN
                    ratings ra ON ra.recipe = re.id
                    LEFT JOIN
                    recipe_tags rta ON rta.recipe = re.id
                    LEFT JOIN
                    tags ta ON ta.id = rta.tag
                ";

            var recipes = new Dictionary<int, DetailRecipe>();

            await _dbConnection.OpenAsync();
            await using var command = new NpgsqlCommand(query, _dbConnection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var recipeId = reader["recipe_id"] == System.DBNull.Value ? -1 : Convert.ToInt32(reader["recipe_id"]);
                if (!recipes.TryGetValue(recipeId, out var recipe))
                {
                    recipe = new DetailRecipe
                    {
                        Id = recipeId,
                        Description = reader["recipe_description"].ToString(),
                        Name = reader["recipe_name"].ToString(),
                        AvgRating = reader["average_rating"] == System.DBNull.Value ? null : Convert.ToSingle(reader["average_rating"]),
                        Preptime = nullOrInt(reader["preptime"]),
                        Cooktime = nullOrInt(reader["cooktime"]),
                        Worktime = nullOrInt(reader["worktime"]),
                        Difficulty = nullOrInt(reader["difficulty"]),
                        Tags = new List<Tag>(),
                        Ratings = new List<Rating>()
                    };
                    recipes.Add(recipeId, recipe);
                }

                var tagId = reader["tag_id"] == System.DBNull.Value ? -1 : Convert.ToInt32(reader["tag_id"]);
                if (tagId > 0 && !recipe.Tags.Any(t => t.Id == tagId))
                {
                    recipe.Tags.Add(new Tag
                            {
                            Id = tagId,
                            Name = reader["tag_name"].ToString(),
                            });
                }

                var ratingId = reader["rating_id"] == System.DBNull.Value ? -1 : Convert.ToInt32(reader["rating_id"]);
                if (ratingId > 0 && !recipe.Ratings.Any(r => r.Id == ratingId))
                {
                    recipe.Ratings.Add(new Rating
                            {
                            Id = reader["rating_id"] == System.DBNull.Value ? -1 : Convert.ToInt32(reader["rating_id"]),
                            User = reader["rating_user_id"] == System.DBNull.Value ? -1 : Convert.ToInt32(reader["rating_user_id"]),
                            Value = Convert.ToInt32(reader["rating_value"]),
                            });
                }
            }

            await _dbConnection.CloseAsync();

            return recipes.Values.ToList();
        }

        public async Task<Recipe> GetRecipeById(int recipeId)
        {
            string query = @"
                    SELECT
                    re.id as recipe_id,
                    re.name as recipe_name,
                    re.description as recipe_description,
                    re.preptime as preptime,
                    re.cooktime as cooktime,
                    re.worktime as worktime,
                    re.difficulty as difficulty,
                    ta.id as tag_id,
                    ta.name as tag_name,
                    ra.user_id as rating_user_id,
                    ra.rating as rating_value,
                    AVG(ra.rating) OVER (PARTITION BY re.id) as average_rating,
                    ra.id as rating_id
                    FROM
                    recipes re
                    LEFT JOIN
                    ratings ra ON ra.recipe = re.id
                    LEFT JOIN
                    recipe_tags rta ON rta.recipe = re.id
                    LEFT JOIN
                    tags ta ON ta.id = rta.tag
                    where re.id = @recipe_id
                    ";

            await _dbConnection.OpenAsync();
            await using var command = new NpgsqlCommand(query, _dbConnection);
            command.Parameters.AddWithValue("recipe_id", recipeId);
            await using var reader = await command.ExecuteReaderAsync();

            DetailRecipe recipe = new DetailRecipe() { Id = -1 };

            while (await reader.ReadAsync())
            {
                if (recipe.Id == -1)
                {
                    recipe = new DetailRecipe
                    {
                        Id = reader["recipe_id"] == System.DBNull.Value ? -1 : Convert.ToInt32(reader["recipe_id"]),
                        Description = reader["recipe_description"].ToString(),
                        Name = reader["recipe_name"].ToString(),
                        AvgRating = reader["average_rating"] == System.DBNull.Value ? null : Convert.ToSingle(reader["average_rating"]),
                        Preptime = nullOrInt(reader["preptime"]),
                        Cooktime = nullOrInt(reader["cooktime"]),
                        Worktime = nullOrInt(reader["worktime"]),
                        Difficulty = nullOrInt(reader["difficulty"]),
                        Tags = new List<Tag>(),
                        Ratings = new List<Rating>()
                    };
                }

                var tagId = reader["tag_id"] == System.DBNull.Value ? -1 : Convert.ToInt32(reader["tag_id"]);
                if (tagId > 0 && !recipe.Tags.Any(t => t.Id == tagId))
                {
                    recipe.Tags.Add(new Tag
                            {
                            Id = tagId,
                            Name = reader["tag_name"].ToString(),
                            });
                }

                var ratingId = reader["rating_id"] == System.DBNull.Value ? -1 : Convert.ToInt32(reader["rating_id"]);
                if (ratingId > 0 && !recipe.Ratings.Any(r => r.Id == ratingId))
                {
                    recipe.Ratings.Add(new Rating
                            {
                            Id = reader["rating_id"] == System.DBNull.Value ? -1 : Convert.ToInt32(reader["rating_id"]),
                            User = reader["rating_user_id"] == System.DBNull.Value ? -1 : Convert.ToInt32(reader["rating_user_id"]),
                            Value = Convert.ToInt32(reader["rating_value"]),
                            });
                }
            }

            await _dbConnection.CloseAsync();
            return recipe;
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

                await _tagRepository.DeleteTagsByRecipeIdNoConn(recipe.Id, transaction);
                await _tagRepository.AddTagsByRecipeIdNoConn(recipe.Tags, recipe.Id, transaction);

                await _ratingRepository.DeleteRatingsByRecipeIdNoConn(recipe.Id, transaction);
                await _ratingRepository.AddRatingsByRecipeIdNoConn(recipe.Ratings, recipe.Id, transaction);

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
                command.Parameters.AddWithValue("created_by", 3);

                int recipeId = (await command.ExecuteScalarAsync() as int?) ?? -1;
                if (recipeId == -1) return -1;

                // inserting into recipe_tags
                await _tagRepository.AddTagsByRecipeIdNoConn(recipe.Tags, recipeId, transaction);

                // inserting into ratings
                await _ratingRepository.AddRatingsByRecipeIdNoConn(recipe.Ratings, recipeId, transaction);

                await transaction.CommitAsync();
                await _dbConnection.CloseAsync();
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

