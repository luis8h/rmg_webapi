using webapi.Models.Basic;

namespace webapi.Interfaces
{
    public interface IRatingRepository
    {
        Task<List<Rating>> GetRatingsByRecipeId(int recipeId);
    }
}
