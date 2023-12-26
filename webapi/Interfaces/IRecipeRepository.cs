using webapi.Models.Basic;
using webapi.Models.Extended;

namespace webapi.Interfaces
{
    public interface IRecipeRepository
    {
        Task<List<DetailRecipe>> GetRecipes();
        Task<int> AddRecipe(Recipe recipe);
    }
}
