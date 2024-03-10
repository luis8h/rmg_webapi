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

        public async Task<User> Authenticate(string userName, string password)
        {
            string query = "select id, username from users where username = @username and password = @password";
            await _dbConnection.OpenAsync();
            await using var command = new NpgsqlCommand(query, dbConection);
            command.Parameters.AddWithValue("username", userName);
            await using var reader = await command.ExecuteReaderAsync();

            User authuser;

            while (await reader.ReadAsync())
            {
                authuser = new User
                {
                    Id = reader["id"] == System.DBNull.Value ? -1 : Convert.ToInt32(reader["id"]),
                    UserName = reader["username"].ToString()
                };
            }

            await _dbConnection.CloseAsync();
            return authuser;
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

    }
}

