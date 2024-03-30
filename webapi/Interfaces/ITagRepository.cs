using webapi.Models.Basic;
using webapi.Models.Dtos;

namespace webapi.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetTags();
        Task<int> DeleteTagsByRecipeId(int recipeId);
        Task<int> AddTagsByRecipeId(List<Tag> tags, int recipeId);
        Task<int> AddTag(AddTagDto tagDto);
        Task<int> DeleteTag(int id);
    }
}
