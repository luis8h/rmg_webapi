using webapi.Models;

namespace webapi.Interfaces
{
    public interface IRecipeRepository
    {
        Task<List<Recipe>> GetRecipes();
    }
}
