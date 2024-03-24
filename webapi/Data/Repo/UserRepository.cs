using System.Reflection;
using Dapper;
using Dapper.FluentMap;
using Dapper.FluentMap.Mapping;
using Npgsql;
using webapi.Interfaces;
using webapi.Models.Basic;

namespace webapi.Data.Repo
{
    public class UserRepository : IUserRepository
    {
        private readonly NpgsqlConnection _dbConnection;

        public UserRepository(NpgsqlConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<User?> GetUser(string username)
        {
            const string query = @"
                select *
                from users
                where username = @Username
                limit 1
                ";

            var user = await _dbConnection.QuerySingleOrDefaultAsync<User>(
                    query,
                    new { Username = username });

            return user;
        }

        public async Task<List<User>> GetUsers()
        {
            const string query = "select id as Id, firstname as Firstname, lastname, username, email, signup_date, password_hashed, password_key from users";
            var userList = await _dbConnection.QueryAsync<User>(query);
            return userList.ToList();
        }

        public async Task<int> addUser(User user)
        {
            const string query = @"
                insert into users
                (username, password_hashed, password_key, password, email)
                values (@Username, @PasswordHashed, @PasswordKey, @Password, @Email)
                ";

            return await _dbConnection.ExecuteAsync(query, user);
        }
    }
}

