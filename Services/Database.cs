using MySql.Data.MySqlClient;

namespace MiniMarket.Services
{
    public static class Database
    {
        public static string ConnectionString =
            "server=localhost;database=minimarket;user=root;password=MDN12345;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public static void Initialize()
        {
            using var conn = GetConnection();
            conn.Open();
        }
    }
}
