using Npgsql;
using webapi.Interfaces;
using webapi.Models;

namespace webapi.Data.Repo
{
    public class UserRepository : IUserRepository
    {
        private readonly NpgsqlConnection _dbConnection;

        public UserRepository(NpgsqlConnection dbConnection)
        {
            _dbConnection = dbConnection;
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

