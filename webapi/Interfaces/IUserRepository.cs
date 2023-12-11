using webapi.Models;

namespace webapi.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetUsers();
    }
}
