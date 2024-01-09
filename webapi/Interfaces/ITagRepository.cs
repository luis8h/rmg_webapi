using Npgsql;
using webapi.Models.Basic;

namespace webapi.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetTags();
        Task<int> DeleteTagsByRecipeId(int? recipeId, NpgsqlTransaction transaction);
        Task<int> AddTagsByRecipeId(List<Tag> tags, int? recipeId, NpgsqlTransaction transaction);
        Task<List<Tag>> GetTagsByRecipeId(int recipeId);
    }
}
