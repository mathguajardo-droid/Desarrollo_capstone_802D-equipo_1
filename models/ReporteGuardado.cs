using System;

namespace MiniMarket.Models
{
    public class ReporteGuardado
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public DateTime FechaGuardado { get; set; }
        public string Filtro { get; set; } = "";
        public string Contenido { get; set; } = "";
    }
}
