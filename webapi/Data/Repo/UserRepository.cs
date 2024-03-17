using System.Security.Authentication;
using System.Security.Cryptography;
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
            await _dbConnection.OpenAsync();

            string query = "select id, password_hashed, password_key, username from users where username = @username";
            await using var command = new NpgsqlCommand(query, _dbConnection);

            command.Parameters.AddWithValue("username", username);

            await using var reader = await command.ExecuteReaderAsync();

            User? user = null;

            while (await reader.ReadAsync())
            {
                user = new User
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Username = reader["username"].ToString(),
                    PasswordHashed = (byte[])reader["password_hashed"],
                    PasswordKey = (byte[])reader["password_key"],
                };
            }

            await _dbConnection.CloseAsync();

            return user;
        }


        public async Task<List<User>> GetUsers()
        {
            await _dbConnection.OpenAsync();
            await using var command = new NpgsqlCommand("SELECT id, firstname, lastname, username FROM users", _dbConnection);
            await using var reader = await command.ExecuteReaderAsync();

            List<User> list = new List<User>();

            while (await reader.ReadAsync())
            {
                list.Add(new User
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Firstname = reader["firstname"]?.ToString(),
                    Lastname = reader["lastname"]?.ToString(),
                    Username = reader["username"]?.ToString(),
                });
            }

            await _dbConnection.CloseAsync();

            return list;
        }


        public async void addUser(User user)
        {
            string query = @"
                insert into users
                (username, password_hashed, password_key, password, email)
                values (@username, @password_hashed, @password_key, 'deb', 'test@test.de')
                ";

            await _dbConnection.OpenAsync();
            await using var command = new NpgsqlCommand(query, _dbConnection);

            command.Parameters.AddWithValue("username", user.Username!);
            command.Parameters.AddWithValue("password_hashed", user.PasswordHashed!);
            command.Parameters.AddWithValue("password_key", user.PasswordKey!);

            await command.ExecuteScalarAsync();

            await _dbConnection.CloseAsync();
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

