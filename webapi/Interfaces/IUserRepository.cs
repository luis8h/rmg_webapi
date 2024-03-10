using webapi.Models.Basic;

namespace webapi.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsers();
        Task<User> Authenticate(string userName, string password);
    }
}
