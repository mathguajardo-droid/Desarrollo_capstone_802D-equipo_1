using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MiniMarket.Models;

namespace MiniMarket.Services
{
    public class ProductService
    {
        public List<Producto> ObtenerTodos()
{
    var lista = new List<Producto>();

    using var conn = Database.GetConnection();
    conn.Open();  

    string sql = @"
        SELECT 
            id,
            nombre,
            codigo AS codigo_barra,
            categoria,
            precio,
            stock,
            es_granel,
            precio_por_kilo,
            stock_gramos
        FROM productos";

    using var cmd = new MySqlCommand(sql, conn);
    using var rd = cmd.ExecuteReader();

    while (rd.Read())
    {
        var p = new Producto
        {
            Id = rd.GetInt32("id"),
            Nombre = rd.GetString("nombre"),
            CodigoBarra = rd.IsDBNull(rd.GetOrdinal("codigo_barra"))
                ? null
                : rd.GetString("codigo_barra"),
            Categoria = rd.IsDBNull(rd.GetOrdinal("categoria"))
                ? "General"
                : rd.GetString("categoria"),
            Precio = rd.IsDBNull(rd.GetOrdinal("precio"))
                ? 0
                : rd.GetDouble("precio"),
            Stock = rd.IsDBNull(rd.GetOrdinal("stock"))
                ? 0
                : rd.GetInt32("stock"),
            EsGranel = !rd.IsDBNull(rd.GetOrdinal("es_granel"))
                && rd.GetBoolean("es_granel"),
            PrecioPorKilo = rd.IsDBNull(rd.GetOrdinal("precio_por_kilo"))
                ? 0
                : rd.GetDouble("precio_por_kilo"),
            StockGramos = rd.IsDBNull(rd.GetOrdinal("stock_gramos"))
                ? 0
                : rd.GetDouble("stock_gramos")
        };

        lista.Add(p);
    }

    return lista;
}


        public void ActualizarStockUnidades(int productoId, int cantidadVendida)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            string sql = @"UPDATE productos 
                           SET stock = stock - @cant
                           WHERE id = @id";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@cant", cantidadVendida);
            cmd.Parameters.AddWithValue("@id", productoId);
            cmd.ExecuteNonQuery();
        }

        public void ActualizarStockGramos(int productoId, double gramosVendidos)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            string sql = @"UPDATE productos
                           SET stock_gramos = stock_gramos - @g
                           WHERE id = @id";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@g", gramosVendidos);
            cmd.Parameters.AddWithValue("@id", productoId);
            cmd.ExecuteNonQuery();
        }
    }
}
