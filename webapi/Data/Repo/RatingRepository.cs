using Dapper;
using Npgsql;
using webapi.Interfaces;
using webapi.Models.Basic;

namespace webapi.Data.Repo
{
    public class RatingRepository : IRatingRepository
    {
        private readonly NpgsqlConnection _dbConnection;

        public RatingRepository(NpgsqlConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<Rating>> GetRatings()
        {
            const string query = @"select ra.id, ra.rating as value, ra.user_id from ratings ra";
            var ratings = await _dbConnection.QueryAsync<Rating>(query);
            return ratings.ToList();
        }

        public async Task<int> AddRatingsByRecipeId(List<Rating> ratings, int recipeId)
        {
            const string query = @"
                INSERT INTO ratings
                (recipe, user_id, rating)
                VALUES (@recipeId, @userId, @ratingValue)
                ";
            var parameters = ratings.Select(rating => new { recipeId, userId = rating.User, ratingValue = rating.Value });
            await _dbConnection.ExecuteAsync(query, parameters);
            return 0;
        }

        public async Task<int> DeleteRatingsByRecipeId(int recipeId)
        {
            const string query = @"delete from ratings where recipe = @recipeId";
            await _dbConnection.ExecuteAsync(query, new { recipeId });
            return 0;
        }
    }
}
