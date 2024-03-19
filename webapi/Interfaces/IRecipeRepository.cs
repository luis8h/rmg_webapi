using webapi.Models.Basic;
using webapi.Models.Extended;

namespace webapi.Interfaces
{
    public interface IRecipeRepository
    {
        Task<List<Recipe>> GetRecipesWithTagsAndRatings();
        Task<int> AddRecipe(Recipe recipe);
        Task<int> PutRecipe(Recipe recipe);
        Task<Recipe> GetRecipeById(int recipeId);
        Task<Recipe> GetRecipeByIdV2(int recipeId);
    }
}
