namespace MiniMarket.Models
{
    public class UsuarioRow
    {
        public int Id { get; set; }
        public string Usuario { get; set; } = "";
        public string Password { get; set; } = "";
        public string Rol { get; set; } = "";
    }
}
