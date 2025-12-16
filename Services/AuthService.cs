using MySql.Data.MySqlClient;

namespace MiniMarket.Services
{
    public class AuthService
    {
        public (bool ok, string rol) Login(string usuario, string clave)
        {
            using var conn = Database.GetConnection();
            using var cmd = new MySqlCommand(
                "SELECT rol FROM usuarios WHERE usuario=@u AND clave=@c LIMIT 1",
                conn
            );

            cmd.Parameters.AddWithValue("@u", usuario);
            cmd.Parameters.AddWithValue("@c", clave);

            var result = cmd.ExecuteScalar();

            if (result == null)
                return (false, "");

            return (true, result.ToString());
        }
    }
}
