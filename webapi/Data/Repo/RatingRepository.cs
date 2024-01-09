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

        public async Task<int> DeleteRatingsByRecipeId(int? recipeId, NpgsqlTransaction transaction)
        {
            string query = @"
                delete from ratings
                where recipe = @recipe
                ";

            await using var command = new NpgsqlCommand(query, _dbConnection);
            command.Transaction = transaction;

            NpgsqlParameter recipeIdParam = command.Parameters.AddWithValue("recipe", recipeId!);

            await command.ExecuteScalarAsync();

            return 0;
        }


        public async Task<int> AddRatingsByRecipeId(List<Rating> ratings, int? recipeId, NpgsqlTransaction transaction)
        {
            string query = @"
                INSERT INTO ratings
                (recipe, user_id, rating)
                VALUES (@recipe, @user_id, @rating)
                ";

            await using var command = new NpgsqlCommand(query, _dbConnection);

            command.CommandText = query;
            command.Transaction = transaction;

            NpgsqlParameter ratingRecipeParam = command.Parameters.AddWithValue("recipe", recipeId!);
            NpgsqlParameter ratingUserParam = command.Parameters.AddWithValue("user_id", 0);
            NpgsqlParameter ratingValueParam = command.Parameters.AddWithValue("rating", 0);

            foreach (Rating rating in ratings)
            {
                ratingValueParam.Value = rating.Value;
                ratingUserParam.Value = rating.User;
                await command.ExecuteNonQueryAsync();
            }

            return 0;
        }

    }
}
