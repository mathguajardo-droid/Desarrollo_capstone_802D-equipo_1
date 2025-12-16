using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using MiniMarket.Models;

namespace MiniMarket.Services
{
    public class ReportesService
    {
        
        public List<ReporteVenta> VentasEntre(string desde, string hasta)
        {
            var lista = new List<ReporteVenta>();

            using var conn = Database.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(@"
                SELECT fecha, total, metodo_pago
                FROM ventas
                WHERE DATE(fecha) BETWEEN @d1 AND @d2
                ORDER BY fecha
            ", conn);

            cmd.Parameters.AddWithValue("@d1", desde);
            cmd.Parameters.AddWithValue("@d2", hasta);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new ReporteVenta
                {
                    Fecha = rd.GetDateTime("fecha").ToString("yyyy-MM-dd HH:mm"),
                    Total = rd.GetDouble("total"),
                    MetodoPago = rd.GetString("metodo_pago")
                });
            }

            return lista;
        }

        
        public void GuardarReporte(string nombre, string filtro, string contenido)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(@"
                INSERT INTO reportes_guardados (nombre, filtro, contenido)
                VALUES (@n, @f, @c)
            ", conn);

            cmd.Parameters.AddWithValue("@n", nombre);
            cmd.Parameters.AddWithValue("@f", filtro);
            cmd.Parameters.AddWithValue("@c", contenido);

            cmd.ExecuteNonQuery();
        }

        
        public List<ReporteGuardado> ObtenerGuardados()
        {
            var lista = new List<ReporteGuardado>();

            using var conn = Database.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(@"
                SELECT id, nombre, fecha_guardado, filtro, contenido
                FROM reportes_guardados
                ORDER BY fecha_guardado DESC
            ", conn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new ReporteGuardado
                {
                    Id = rd.GetInt32("id"),
                    Nombre = rd.GetString("nombre"),
                    FechaGuardado = rd.GetDateTime("fecha_guardado"),
                    Filtro = rd.GetString("filtro"),
                    Contenido = rd.GetString("contenido")
                });
            }

            return lista;
        }
    }
}
