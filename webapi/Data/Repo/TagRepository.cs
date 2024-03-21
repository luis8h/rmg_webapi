using Dapper;
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
            const string query = "SELECT id, name FROM tags";
            var tagList = await _dbConnection.QueryAsync<Tag>(query);
            return tagList.ToList();
        }

        public async Task<List<Tag>> GetTagsByRecipeIdNoConn(int recipeId)
        {
            List<Tag> list = new List<Tag>();
            string query = @"
                select ta.id, ta.name
                from recipe_tags rta
                left join tags ta on rta.tag = ta.id
                where rta.recipe = 60
                ";

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

            return list;
        }

        public async Task<int> DeleteTagsByRecipeIdNoConn(int? recipeId, NpgsqlTransaction transaction)
        {
            string query = @"
                delete from recipe_tags
                where recipe = @recipe
                ";

            await using var command = new NpgsqlCommand(query, _dbConnection);
            command.Transaction = transaction;

            NpgsqlParameter recipeIdParam = command.Parameters.AddWithValue("recipe", recipeId!);

            await command.ExecuteScalarAsync();

            return 0;
        }

        public async Task<int> AddTagsByRecipeIdNoConn(List<Tag> tags, int? recipeId, NpgsqlTransaction transaction)
        {
            string query = @"
                INSERT INTO recipe_tags
                (tag, recipe)
                VALUES (@tag, @recipe)
                ";

            await using var command = new NpgsqlCommand(query, _dbConnection);
            command.CommandText = query;
            command.Transaction = transaction;

            NpgsqlParameter tagParam = command.Parameters.AddWithValue("tag", 0);
            NpgsqlParameter recipeParam = command.Parameters.AddWithValue("recipe", recipeId!);

            foreach (Tag tag in tags)
            {
                tagParam.Value = tag.Id!;
                await command.ExecuteNonQueryAsync();
            }

            return 0;
        }

        public async Task<int> AddTagsByRecipeId(List<Tag> tags, int recipeId)
        {
            const string query = @"
                INSERT INTO recipe_tags
                (tag, recipe)
                VALUES (@tagId, @recipeId)
                ";

            var parameters = tags.Select(tag => new { tagId = tag.Id, recipeId });
            await _dbConnection.ExecuteAsync(query, parameters);
            return 0;
        }
    }
}
