using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace MiniMarket.Services
{
    public class CorteCajaService
    {
        public double ObtenerTotalVentasHoy()
        {
            using var con = Database.GetConnection();
            con.Open();

            using var cmd = new MySqlCommand(@"
                SELECT IFNULL(SUM(total),0)
                FROM ventas
                WHERE DATE(fecha) = CURDATE();
            ", con);

            return Convert.ToDouble(cmd.ExecuteScalar());
        }

        public void RegistrarCorte(
            double cajaInicial,
            double ingresosExtra,
            double pagos,
            double efectivoFinal
        )
        {
            double totalVentas = ObtenerTotalVentasHoy();
            double totalFinal = cajaInicial + totalVentas + ingresosExtra - pagos;
            double diferencia = efectivoFinal - totalFinal;

            using var con = Database.GetConnection();
            con.Open();

            using var cmd = new MySqlCommand(@"
                INSERT INTO cortes_caja
                (fecha, caja_inicial, total_ventas, ingresos_extra, pagos, efectivo_final, diferencia)
                VALUES
                (NOW(), @ci, @tv, @ie, @p, @ef, @dif);
            ", con);

            cmd.Parameters.AddWithValue("@ci", cajaInicial);
            cmd.Parameters.AddWithValue("@tv", totalVentas);
            cmd.Parameters.AddWithValue("@ie", ingresosExtra);
            cmd.Parameters.AddWithValue("@p", pagos);
            cmd.Parameters.AddWithValue("@ef", efectivoFinal);
            cmd.Parameters.AddWithValue("@dif", diferencia);

            cmd.ExecuteNonQuery();
        }

        public List<(DateTime fecha, double totalVentas)> Historial()
        {
            var list = new List<(DateTime, double)>();

            using var con = Database.GetConnection();
            con.Open();

            using var cmd = new MySqlCommand(@"
                SELECT fecha, total_ventas
                FROM cortes_caja
                ORDER BY fecha DESC
                LIMIT 30;
            ", con);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add((
                    Convert.ToDateTime(rd["fecha"]),
                    Convert.ToDouble(rd["total_ventas"])
                ));
            }

            return list;
        }
    }
}
