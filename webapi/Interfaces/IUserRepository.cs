using webapi.Models.Basic;

namespace webapi.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsers();
        Task<User>? Authenticate(string userName, string password);
        void Register(string username, string password);
        Task<bool> UserAlreadyExists(string username);
    }
}
