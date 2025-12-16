using System;

namespace MiniMarket.Models
{
    public class Prediccion
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int DiasAnalizados { get; set; }
        public int ProductoId { get; set; }
        public double VendidoUltimosDias { get; set; }
        public double PromedioDiario { get; set; }
        public double Proyeccion7Dias { get; set; }
        public double IngresoTotal { get; set; }
    }
}
