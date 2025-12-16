namespace MiniMarket.Models
{
    public class PrediccionResultado
    {
        public int ProductoId { get; set; }
        public string Producto { get; set; } = "";
        public int StockActual { get; set; }
        public int VendidoUltimosDias { get; set; }
        public double PromedioDiario { get; set; }
        public double Proyeccion7Dias { get; set; }
        public double IngresoTotal { get; set; }
    }
}
