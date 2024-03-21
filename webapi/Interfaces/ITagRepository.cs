using webapi.Models.Basic;

namespace webapi.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetTags();
        Task<int> DeleteTagsByRecipeId(int recipeId);
        Task<int> AddTagsByRecipeId(List<Tag> tags, int recipeId);
    }
}
