using Npgsql;
using webapi.Models.Basic;

namespace webapi.Interfaces
{
    public interface IRatingRepository
    {
        Task<List<Rating>> GetRatingsByRecipeId(int recipeId);
        Task<int> AddRatingsByRecipeId(List<Rating> ratings, int? recipeId, NpgsqlTransaction transaction);
        Task<int> DeleteRatingsByRecipeId(int? recipeId, NpgsqlTransaction transaction);
    }
}
