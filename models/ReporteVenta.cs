using System;

namespace MiniMarket.Models
{
    public class ReporteVenta
    {
        public int Id { get; set; }
        public string Fecha { get; set; } = "";
        public string MetodoPago { get; set; } = "";
        public double Total { get; set; }
    }
}
