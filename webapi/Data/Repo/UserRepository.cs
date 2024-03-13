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

        public async Task<User>? Authenticate(string userName, string passwordText)
        {
            string query = "select id, password_hashed, password_key, username from users where username = @username";
            await _dbConnection.OpenAsync();
            await using var command = new NpgsqlCommand(query, _dbConnection);
            command.Parameters.AddWithValue("username", userName);
            await using var reader = await command.ExecuteReaderAsync();

            User? authuser = null;

            while (await reader.ReadAsync())
            {
                authuser = new User
                {
                    Id = reader["id"] == System.DBNull.Value ? -1 : Convert.ToInt32(reader["id"]),
                    Username = reader["username"] == System.DBNull.Value ? null: reader["username"].ToString(),
                    PasswordHashed = reader["password_hashed"] == System.DBNull.Value ? null: (byte[])reader["password_hashed"],
                    PasswordKey = reader["password_key"] == System.DBNull.Value ? null: (byte[])reader["password_key"]
                };
            }

            await _dbConnection.CloseAsync();

            if (authuser == null || authuser.PasswordKey  == null || authuser.PasswordHashed == null)
                return null;

            if (!MatchPasswordHash(passwordText, authuser.PasswordHashed, authuser.PasswordKey))
                return null;

            return authuser;
        }

        private bool MatchPasswordHash(string passwordText, byte[] passwordHashed, byte[] passwordKey)
        {
            using (var hmac = new HMACSHA256(passwordKey))
            {
                var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passwordText));

                for (int i = 0; i < passwordHash.Length; i++)
                {
                    if (passwordHash[i] != passwordHashed[i])
                        return false;
                }

                return true;
            }
        }

        public async Task<List<User>> GetUsers()
        {
            List<User> list = new List<User>();
            string query = "SELECT id, firstname, lastname, username, email FROM users";

            await _dbConnection.OpenAsync();
            await using var command = new NpgsqlCommand(query, _dbConnection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new User
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Firstname = reader["firstname"].ToString(),
                    Lastname = reader["lastname"].ToString(),
                    Username = reader["username"].ToString(),
                    Email = reader["email"].ToString()
                });
            }

            await _dbConnection.CloseAsync();

            return list;
        }

        public void Register(string username, string password)
        {
            byte[] passwordHash, passwordKey;
            using (var hmac = new HMACSHA256())
            {
                passwordKey = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

            User user = new User();
            user.Username = username;
            user.PasswordHashed = passwordHash;
            user.PasswordKey = passwordKey;

            Console.WriteLine("creating user ...");

            addUser(user);
        }

        private async void addUser(User user)
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

