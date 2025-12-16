using System;

namespace MiniMarket.Models
{
    public class Movimiento
    {
        public string Fecha { get; set; } = "";
        public string Tipo { get; set; } = "";       
        public string Descripcion { get; set; } = "";
        public double Monto { get; set; }
        public double SaldoResultante { get; set; }
    }
}
