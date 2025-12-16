using System;

namespace MiniMarket.Models
{
    public enum TipoMovimientoProveedor
    {
        None = 0,    
        Abono = 1,
        Cargo = 2
    }

    public class MovimientoProveedor
    {
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public TipoMovimientoProveedor Tipo { get; set; }
        public double Monto { get; set; }
        public DateTime Fecha { get; set; }
    }
}
