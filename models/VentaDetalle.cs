namespace MiniMarket.Models
{
    public class VentaDetalle
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public int ProductoId { get; set; }
        public double Cantidad { get; set; }
        public double Subtotal { get; set; }
    }
}
