using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MiniMarket.Models;

namespace MiniMarket.Services
{
    public class ReportesGuardadosService
    {
        public List<ReporteGuardado> Listar()
        {
            var lista = new List<ReporteGuardado>();

            using var conn = Database.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(@"
                SELECT id, nombre, fecha_guardado, filtro, contenido
                FROM reportes_guardados
                ORDER BY fecha_guardado DESC;
            ", conn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new ReporteGuardado
                {
                    Id = Convert.ToInt32(rd["id"]),
                    Nombre = rd["nombre"].ToString() ?? "",
                    FechaGuardado = Convert.ToDateTime(rd["fecha_guardado"]),
                    Filtro = rd["filtro"].ToString() ?? "",
                    Contenido = rd["contenido"].ToString() ?? ""
                });
            }

            return lista;
        }
    }
}
