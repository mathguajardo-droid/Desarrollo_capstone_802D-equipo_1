using System;

namespace MiniMarket.Models
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        public string? Contacto { get; set; }
        public double SaldoActual { get; set; }
    }
}
