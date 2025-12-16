using System;

namespace MiniMarket.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string UsuarioNombre { get; set; } = "";
        public string Clave { get; set; } = "";
        public string Rol { get; set; } = "trabajador";
    }
}
