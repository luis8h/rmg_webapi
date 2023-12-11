using webapi.Models;

namespace webapi.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsers();
    }
}
