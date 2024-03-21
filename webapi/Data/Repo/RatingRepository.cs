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

        public async Task<List<Rating>> GetRatingsByRecipeIdNoConn(int recipeId)
        {
            List<Rating> list = new List<Rating>();
            string query = @"
                select ra.id, ra.rating, ra.user_id
                from ratings ra
                where ra.recipe = 60
                ";

            await using var command = new NpgsqlCommand(query, _dbConnection);
            await using var reader = await command.ExecuteReaderAsync();

            command.Parameters.AddWithValue("recipe_id", recipeId);

            while (await reader.ReadAsync())
            {
                list.Add(new Rating ()
                        {
                        Id = Convert.ToInt32(reader["id"]),
                        User = Convert.ToInt32(reader["user_id"]),
                        Value = Convert.ToInt32(reader["rating"])
                        });
            }

            return list;
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
