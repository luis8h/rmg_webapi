using webapi.Models.Basic;
using webapi.Models.Extended;

namespace webapi.Interfaces
{
    public interface IRecipeRepository
    {
        Task<List<Recipe>> GetRecipes();
        Task<List<DetailRecipe>> GetRecipesDetail();
        Task<int> AddRecipe(Recipe recipe);
        Task<int> PutRecipe(Recipe recipe);
        Task<Recipe> GetRecipeById(int recipeId);
        Task<Recipe> GetRecipeByIdV2(int recipeId);
    }
}
