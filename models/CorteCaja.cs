using System;

namespace MiniMarket.Models
{
    public class CorteCaja
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }

        public double CajaInicial { get; set; }
        public double IngresosExtra { get; set; }
        public double Pagos { get; set; }
        public double TotalVentas { get; set; }

        public double EfectivoFinal { get; set; }
        public double Diferencia { get; set; }
    }
}
