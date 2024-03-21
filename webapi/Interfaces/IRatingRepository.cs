using webapi.Models.Basic;

namespace webapi.Interfaces
{
    public interface IRatingRepository
    {
        Task<List<Rating>> GetRatingsByRecipeIdNoConn(int recipeId);
        Task<int> AddRatingsByRecipeId(List<Rating> ratings, int recipeId);
        Task<int> DeleteRatingsByRecipeId(int recipeId);
    }
}
