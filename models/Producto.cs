using System;

namespace MiniMarket.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string? Codigo { get; set; }
        public double Precio { get; set; }
        public int Stock { get; set; }
        public string Categoria { get; set; } = "General";
        public string? CodigoBarra { get; set; }

        public bool EsGranel { get; set; }
        public double StockGramos { get; set; }
        public double PrecioPorKilo { get; set; }
    }
}
