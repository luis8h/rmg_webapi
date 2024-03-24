using webapi.Models.Basic;

namespace webapi.Interfaces
{
    public interface IUserRepository
    {
        public Task<User?> GetUser(string username);
        public Task<int> addUser(User user);
        Task<List<User>> GetUsers();
    }
}
