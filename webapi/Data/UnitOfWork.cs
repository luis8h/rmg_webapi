using webapi.Interfaces;
using webapi.Data.Repo;

namespace webapi.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        public IUserRepository UserRepository => new UserRepository();
    }
}
