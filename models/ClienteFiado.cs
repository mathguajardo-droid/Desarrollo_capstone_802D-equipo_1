using System;

namespace MiniMarket.Models
{
    public class ClienteFiado
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public double SaldoActual { get; set; }
    }
}
