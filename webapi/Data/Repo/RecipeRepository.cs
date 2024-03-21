using System.Transactions;
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

        public async Task<int> PutRecipe(Recipe recipe)
        {
            if (recipe.Id == null)
                throw new KeyNotFoundException(); // TODO: replace with custom exception

            int recipeId = recipe.Id ?? default(int);

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await UpdateRecipe(recipe);

                await _tagRepository.DeleteTagsByRecipeId(recipeId);
                await _tagRepository.AddTagsByRecipeId(recipe.Tags, recipeId);

                await _ratingRepository.DeleteRatingsByRecipeId(recipeId);
                await _ratingRepository.AddRatingsByRecipeId(recipe.Ratings, recipeId);

                scope.Complete();
            }

            return 0;
        }

        public async Task<int> UpdateRecipe(Recipe recipe)
        {
            string query = @"
                update recipes
                set name = @name,
                    description = @description,
                    preptime = @preptime,
                    cooktime = @cooktime,
                    worktime = @worktime,
                    difficulty = @difficulty
                where id = @Id
                ";
                await _dbConnection.QueryAsync(query, recipe);
                return 0;
        }

        public async Task<int> AddRecipe(Recipe recipe)
        {
            const string query = @"
                INSERT INTO recipes
                (name, description, preptime, cooktime, worktime, difficulty, created_by)
                VALUES (@name, @description, @preptime, @cooktime, @worktime, @difficulty, @createdBy)
                returning id
            ";

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var newId = await _dbConnection.QuerySingleAsync<int>(query, recipe);

                await _tagRepository.AddTagsByRecipeId(recipe.Tags, newId);
                await _ratingRepository.AddRatingsByRecipeId(recipe.Ratings, newId);

                scope.Complete();

                return newId;
            }

        }
    }
}

