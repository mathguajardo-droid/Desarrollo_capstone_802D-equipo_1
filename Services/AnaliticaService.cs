using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace MiniMarket.Services
{
    public class AnaliticaService
    {
        public class ProductoVentaStats
        {
            public int ProductoId { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public double Precio { get; set; }
            public int Stock { get; set; }
            public int CantidadVendida { get; set; }
            public double IngresoTotal { get; set; }
            public double PromedioDiario { get; set; }
            public double Proyeccion7Dias { get; set; }
        }

        
        public List<ProductoVentaStats> GetVentasPorProducto(int dias)
        {
            if (dias <= 0) dias = 30;
            var resultado = new List<ProductoVentaStats>();

            using var conn = Database.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(@"
                SELECT 
                    p.id        AS producto_id,
                    p.nombre    AS nombre,
                    p.precio    AS precio,
                    p.stock     AS stock,
                    IFNULL(SUM(d.cantidad), 0)      AS total_cantidad,
                    IFNULL(SUM(d.subtotal), 0)      AS total_ingreso
                FROM productos p
                LEFT JOIN venta_detalle d ON d.producto_id = p.id
                LEFT JOIN ventas v        ON v.id = d.venta_id
                    AND v.fecha >= DATE_SUB(CURDATE(), INTERVAL @dias DAY)
                GROUP BY p.id, p.nombre, p.precio, p.stock
                HAVING total_cantidad > 0
                ORDER BY total_cantidad DESC;
            ", conn);

            cmd.Parameters.AddWithValue("@dias", dias);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                int cant = Convert.ToInt32(rd["total_cantidad"]);
                double ingreso = Convert.ToDouble(rd["total_ingreso"]);
                double promedioDiario = cant / (double)dias;
                double proyeccion7 = promedioDiario * 7;

                resultado.Add(new ProductoVentaStats
                {
                    ProductoId      = Convert.ToInt32(rd["producto_id"]),
                    Nombre          = rd["nombre"]?.ToString() ?? string.Empty,
                    Precio          = Convert.ToDouble(rd["precio"]),
                    Stock           = Convert.ToInt32(rd["stock"]),
                    CantidadVendida = cant,
                    IngresoTotal    = ingreso,
                    PromedioDiario  = promedioDiario,
                    Proyeccion7Dias = proyeccion7
                });
            }

            return resultado;
        }
    }
}
