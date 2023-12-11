using webapi.Interfaces;
using webapi.Data.Repo;
using Npgsql;

namespace webapi.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly NpgsqlConnection _dbConnection;

        public IUserRepository UserRepository => new UserRepository(_dbConnection);

        public UnitOfWork(NpgsqlConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public NpgsqlConnection GetDbConnection()
        {
            return _dbConnection;
        }

        public void Dispose()
        {
            _dbConnection.Dispose();
        }
    }
}
