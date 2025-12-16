using System;

namespace MiniMarket.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public double Total { get; set; }
        public string MetodoPago { get; set; } = "efectivo";
    }
}
