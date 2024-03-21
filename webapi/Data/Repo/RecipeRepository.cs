using Dapper;
using Dapper.FluentMap;
using Dapper.FluentMap.Mapping;
using Microsoft.VisualBasic;
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

        public async Task<List<DetailRecipe>> GetRecipesDetail()
        {
            const string query = @"
                select
                    re.id as recipe_id,
                    re.name as recipe_name,
                    re.description as description,
                    re.preptime as preptime,
                    re.cooktime as cooktime,
                    re.worktime as worktime,
                    re.difficulty as difficulty,
                    AVG(ra.rating) OVER (PARTITION BY re.id) as avg_rating,
                    ta.id as tag_id,
                    ta.name as tag_name,
                    ra.user_id as user_id,
                    ra.rating as rating,
                    ra.id as rating_id
                from recipes re
                left join recipe_tags rta on rta.recipe = re.id
                left join tags ta on ta.id = rta.tag
                left join ratings ra on ra.recipe = re.id
                ";

            var recipes = await _dbConnection.QueryAsync<DetailRecipe, Tag, Rating, DetailRecipe>(query, (recipe, tag, rating) => {
                        recipe.Ratings.Add(rating);
                        recipe.Tags.Add(tag);
                        return recipe;
                    }, splitOn: "tag_id,user_id");

            var result = recipes.GroupBy(r => r.Id).Select(g =>
            {
                var groupedRecipe = g.First();

                var tags = g.SelectMany(r => r.Tags).Distinct();
                var tagResult = tags?.GroupBy(t => t?.Id).Select(gt =>
                {
                    return gt.First();
                });

                var ratings = g.SelectMany(r => r.Ratings).Distinct();
                var ratingResult = ratings?.GroupBy(ra => ra?.Id).Select(gr =>
                {
                    return gr.First();
                });

                groupedRecipe.Tags = tagResult?.ToList() ?? new List<Tag>();
                groupedRecipe.Ratings = ratingResult?.ToList() ?? new List<Rating>();

                return groupedRecipe;
            });

            return result.ToList();
        }

        public async Task<List<Recipe>> GetRecipes()
        {
            const string query = @"
                select
                    re.id as recipe_id,
                    re.name as recipe_name,
                    re.description as description,
                    re.preptime as preptime,
                    re.cooktime as cooktime,
                    re.worktime as worktime,
                    re.difficulty as difficulty,
                    ta.id as tag_id,
                    ta.name as tag_name,
                    ra.user_id as user_id,
                    ra.rating as rating,
                    ra.id as rating_id
                from recipes re
                left join recipe_tags rta on rta.recipe = re.id
                left join tags ta on ta.id = rta.tag
                left join ratings ra on ra.recipe = re.id
                ";

            var recipes = await _dbConnection.QueryAsync<Recipe, Tag, Rating, Recipe>(query, (recipe, tag, rating) => {
                        recipe.Ratings.Add(rating);
                        recipe.Tags.Add(tag);
                        return recipe;
                    }, splitOn: "tag_id,user_id");

            var result = recipes.GroupBy(r => r.Id).Select(g =>
            {
                var groupedRecipe = g.First();

                var tags = g.SelectMany(r => r.Tags).Distinct();
                var tagResult = tags?.GroupBy(t => t?.Id).Select(gt =>
                {
                    return gt.First();
                });

                var ratings = g.SelectMany(r => r.Ratings).Distinct();
                var ratingResult = ratings?.GroupBy(ra => ra?.Id).Select(gr =>
                {
                    return gr.First();
                });

                groupedRecipe.Tags = tagResult?.ToList() ?? new List<Tag>();
                groupedRecipe.Ratings = ratingResult?.ToList() ?? new List<Rating>();

                return groupedRecipe;
            });

            return result.ToList();
        }

        public async Task<DetailRecipe> GetRecipeById(int recipeId)
        {
            const string query = @"
                select
                    re.id as recipe_id,
                    re.name as recipe_name,
                    re.description as description,
                    re.preptime as preptime,
                    re.cooktime as cooktime,
                    re.worktime as worktime,
                    re.difficulty as difficulty,
                    AVG(ra.rating) OVER (PARTITION BY re.id) as avg_rating,
                    ta.id as tag_id,
                    ta.name as tag_name,
                    ra.user_id as user_id,
                    ra.rating as rating,
                    ra.id as rating_id
                from recipes re
                left join recipe_tags rta on rta.recipe = re.id
                left join tags ta on ta.id = rta.tag
                left join ratings ra on ra.recipe = re.id
                where re.id = @RecipeId
                ";

            var dbResult = await _dbConnection.QueryAsync<DetailRecipe, Tag, Rating, DetailRecipe>(
                    query,
                    (recipe, tag, rating) => {
                        recipe.Ratings.Add(rating);
                        recipe.Tags.Add(tag);
                        return recipe;
                    },
                    new { RecipeId = recipeId },
                    splitOn: "tag_id,user_id"
                    );

            var recipe = dbResult.GroupBy(r => r.Id).Select(g =>
            {
                var groupedRecipe = g.First();

                var tags = g.SelectMany(r => r.Tags).Distinct();
                var tagResult = tags?.GroupBy(t => t?.Id).Select(gt =>
                {
                    return gt.First();
                });

                var ratings = g.SelectMany(r => r.Ratings).Distinct();
                var ratingResult = ratings?.GroupBy(ra => ra?.Id).Select(gr =>
                {
                    return gr.First();
                });

                groupedRecipe.Tags = tagResult?.ToList() ?? new List<Tag>();
                groupedRecipe.Ratings = ratingResult?.ToList() ?? new List<Rating>();

                return groupedRecipe;
            }).First();

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

