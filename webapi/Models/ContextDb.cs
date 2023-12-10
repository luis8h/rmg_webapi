using Npgsql;

namespace webapi.Models
{
    public class ContextDb
    {
        string connString = "Server=localhost; Port=5435; Database=rmg_db; User Id=postgres; Password=test;";

        public List<User> GetUsers()
        {
            List<User> list = new List<User>();
            string query = "SELECT * FROM users";

            using (NpgsqlConnection con = new NpgsqlConnection(connString))
            {
                con.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new User
                            {
                                Id = Convert.ToInt32(reader[0]),
                                Firstname = Convert.ToString(reader[1]),
                                Lastname = Convert.ToString(reader[2])
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}


