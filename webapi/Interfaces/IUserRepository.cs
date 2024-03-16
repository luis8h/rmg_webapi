using webapi.Models.Basic;

namespace webapi.Interfaces
{
    public interface IUserRepository
    {
        public Task<User?> GetUser(string username);
        public void addUser(User user);
        Task<List<User>> GetUsers();
        Task<bool> UserAlreadyExists(string username);
    }
}
