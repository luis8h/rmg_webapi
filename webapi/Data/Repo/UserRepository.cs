using System.Reflection;
using Dapper;
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
            const string query = "select * from users";
            var userList = await _dbConnection.QueryAsync<User>(query);
            return userList.ToList();
        }

        public async void addUser(User user)
        {
            const string query = @"
                insert into users
                (username, password_hashed, password_key, password, email)
                values (@username, @passwordHashed, @passwordKey, 'deb', 'test@test.de')
                ";

            await _dbConnection.ExecuteAsync(query, user);
        }

        public async Task<bool> UserAlreadyExists(string username)
        {

            string query = @"
                select count(*) as count from users where username = @username
                ";

            await _dbConnection.OpenAsync();
            await using var command = new NpgsqlCommand(query, _dbConnection);

            command.Parameters.AddWithValue("username", username);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // TODO: add another if for checking if the count is > 1 and if so, a warning message gets logged
                if (Convert.ToInt32(reader["count"]) > 0)
                {
                    await _dbConnection.CloseAsync();
                    return true;
                }
            }

            await _dbConnection.CloseAsync();
            return false;

        }

    }
}

