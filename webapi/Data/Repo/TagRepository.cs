using Npgsql;
using webapi.Interfaces;
using webapi.Models.Basic;

namespace webapi.Data.Repo
{
    public class TagRepository : ITagRepository
    {
        private readonly NpgsqlConnection _dbConnection;

        public TagRepository(NpgsqlConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<Tag>> GetTags()
        {
            List<Tag> list = new List<Tag>();
            string query = "SELECT id, name FROM tags";

            await _dbConnection.OpenAsync();
            await using var command = new NpgsqlCommand(query, _dbConnection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Tag ()
                        {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        });
            }

            await _dbConnection.CloseAsync();

            return list;
        }

        public async Task<List<Tag>> GetTagsByRecipeId(int recipeId)
        {
            List<Tag> list = new List<Tag>();
            string query = @"
                select ta.id, ta.name
                from recipe_tags rta
                left join tags ta on rta.tag = ta.id
                where rta.recipe = 60
                ";

            await _dbConnection.OpenAsync();
            await using var command = new NpgsqlCommand(query, _dbConnection);
            await using var reader = await command.ExecuteReaderAsync();

            command.Parameters.AddWithValue("recipe_id", recipeId);

            while (await reader.ReadAsync())
            {
                list.Add(new Tag ()
                        {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        });
            }

            await _dbConnection.CloseAsync();

            return list;
        }
    }
}
