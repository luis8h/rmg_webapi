using webapi.Models.Basic;
using webapi.Models.Extended;

namespace webapi.Interfaces
{
    public interface IRecipeRepository
    {
        Task<List<DetailRecipe>> GetRecipesDetail();
        Task<int> AddRecipe(Recipe recipe);
        Task<int> PutRecipe(Recipe recipe);
        Task<DetailRecipe> GetRecipeById(int recipeId);
    }
}
