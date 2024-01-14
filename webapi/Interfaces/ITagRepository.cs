using Npgsql;
using webapi.Models.Basic;

namespace webapi.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetTags();
        Task<int> DeleteTagsByRecipeIdNoConn(int? recipeId, NpgsqlTransaction transaction);
        Task<int> AddTagsByRecipeIdNoConn(List<Tag> tags, int? recipeId, NpgsqlTransaction transaction);
        Task<List<Tag>> GetTagsByRecipeIdNoConn(int recipeId);
    }
}
