using webapi.Models.Basic;

namespace webapi.Interfaces
{
    public interface IRecipeRepository
    {
        Task<List<Recipe>> GetRecipes();
        Task<int> AddRecipe(Recipe recipe);
    }
}
