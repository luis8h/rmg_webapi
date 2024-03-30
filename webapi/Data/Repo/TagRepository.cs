using Dapper;
using Npgsql;
using webapi.Interfaces;
using webapi.Models.Basic;
using webapi.Models.Dtos;

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

        public async Task<int> DeleteTagsByRecipeId(int recipeId)
        {
            const string query = @"delete from recipe_tags where recipe = @recipeId";
            await _dbConnection.ExecuteAsync(query, new { recipeId });
            return 0;
        }

        public async Task<int> AddTag(AddTagDto tagDto)
        {
            const string query = @"insert into tags (name) values (@Name) returning id;";
            var tagId = await _dbConnection.QuerySingleAsync<int>(query, tagDto);
            return tagId;
        }

        public async Task<int> DeleteTag(int id)
        {
            // TODO: different deleting concept (set as invisible, if later a tag with the same name is created pick up old id)
            const string query = @"delete from tags where id = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(query, new { Id = id });
            return affectedRows;
        }
    }
}
