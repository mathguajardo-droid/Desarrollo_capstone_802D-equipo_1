using System;

namespace MiniMarket.Models
{
    public enum TipoMovimientoCliente
    {
        None = 0,    
        Abono = 1,
        Cargo = 2
    }

    public class MovimientoClienteFiado
    {
        public int Id { get; set; }
        public int ClienteFiadoId { get; set; }
        public TipoMovimientoCliente Tipo { get; set; }
        public double Monto { get; set; }
        public DateTime Fecha { get; set; }
    }
}
