using Npgsql;
using webapi.Models.Basic;

namespace webapi.Interfaces
{
    public interface IRatingRepository
    {
        Task<List<Rating>> GetRatingsByRecipeIdNoConn(int recipeId);
        Task<int> AddRatingsByRecipeIdNoConn(List<Rating> ratings, int? recipeId, NpgsqlTransaction transaction);
        Task<int> AddRatingsByRecipeId(List<Rating> ratings, int recipeId);
        Task<int> DeleteRatingsByRecipeIdNoConn(int? recipeId, NpgsqlTransaction transaction);
    }
}
