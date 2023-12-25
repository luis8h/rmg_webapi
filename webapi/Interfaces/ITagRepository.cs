using webapi.Models.Basic;

namespace webapi.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetTags();
    }
}
