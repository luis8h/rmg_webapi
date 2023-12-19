using webapi.Models;

namespace webapi.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetTags();
    }
}
