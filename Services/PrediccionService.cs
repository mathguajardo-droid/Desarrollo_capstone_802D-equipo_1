using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MiniMarket.Models;

namespace MiniMarket.Services
{
    public class PrediccionService
    {
        public List<PrediccionResultado> CalcularTopRotacion(int dias)
        {
            if (dias <= 0) dias = 30;
            var resultado = new List<PrediccionResultado>();
            var fechaInicio = DateTime.Now.AddDays(-dias);

            using var conn = Database.GetConnection();
            conn.Open();                                      

            using var cmd = new MySqlCommand(@"
                SELECT
                    p.id                AS ProductoId,
                    p.nombre            AS Producto,
                    p.stock             AS StockActual,
                    IFNULL(SUM(vd.cantidad), 0)      AS VendidoUltimosDias,
                    IFNULL(SUM(vd.subtotal), 0)      AS IngresoTotal
                FROM productos p
                LEFT JOIN venta_detalle vd ON vd.producto_id = p.id
                LEFT JOIN ventas v ON v.id = vd.venta_id
                      AND v.fecha >= @fi
                GROUP BY p.id, p.nombre, p.stock
                HAVING VendidoUltimosDias > 0
                ORDER BY VendidoUltimosDias DESC
                LIMIT 50;
            ", conn);

            cmd.Parameters.AddWithValue("@fi", fechaInicio);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                int vendido = Convert.ToInt32(rd["VendidoUltimosDias"]);
                double ingreso = Convert.ToDouble(rd["IngresoTotal"]);
                double promedio = vendido / (double)dias;
                double proy7 = promedio * 7;

                resultado.Add(new PrediccionResultado
                {
                    ProductoId        = Convert.ToInt32(rd["ProductoId"]),
                    Producto          = rd["Producto"].ToString() ?? "",
                    StockActual       = Convert.ToInt32(rd["StockActual"]),
                    VendidoUltimosDias= vendido,
                    PromedioDiario    = promedio,
                    Proyeccion7Dias   = proy7,
                    IngresoTotal      = ingreso
                });
            }

            return resultado;
        }

        public void GuardarPrediccion(int dias, List<PrediccionResultado> datos)
        {
            using var conn = Database.GetConnection();
            conn.Open();                                      

            using var cmd = new MySqlCommand(@"
                INSERT INTO predicciones_guardadas (fecha_guardado, dias, resumen)
                VALUES (NOW(), @dias, @resumen);
            ", conn);

            cmd.Parameters.AddWithValue("@dias", dias);
            cmd.Parameters.AddWithValue("@resumen", $"Total productos: {datos.Count}");
            cmd.ExecuteNonQuery();
        }
    }
}
