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

        public async Task<List<Rating>> GetRatingsByRecipeId(int recipeId)
        {
            List<Rating> list = new List<Rating>();
            string query = @"
                select ra.id, ra.rating, ra.user_id
                from ratings ra
                where ra.recipe = 60
                ";

            await _dbConnection.OpenAsync();
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

            await _dbConnection.CloseAsync();

            return list;
        }
    }
}
