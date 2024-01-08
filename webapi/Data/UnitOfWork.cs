using webapi.Interfaces;
using webapi.Data.Repo;
using Npgsql;

namespace webapi.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly NpgsqlConnection _dbConnection;

        public IUserRepository UserRepository => new UserRepository(_dbConnection);
        public IRecipeRepository RecipeRepository => new RecipeRepository(_dbConnection, TagRepository, RatingRepository);
        public ITagRepository TagRepository => new TagRepository(_dbConnection);
        public IRatingRepository RatingRepository => new RatingRepository(_dbConnection);

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
