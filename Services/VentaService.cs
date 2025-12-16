using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace MiniMarket.Services
{
    public class VentaService
    {
        
        public int RegistrarVenta(
            double total,
            string metodoPago,
            List<(int productoId, double cantidad, double precio)> items,
            int? clienteFiadoId = null)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            using var tx = conn.BeginTransaction();

            try
            {
                
                using var cmdVenta = conn.CreateCommand();
                cmdVenta.Transaction = tx;

                cmdVenta.CommandText = @"
                    INSERT INTO ventas (fecha, total, metodo_pago, cliente_fiado_id)
                    VALUES (NOW(), @t, @m, @c);
                    SELECT LAST_INSERT_ID();";

                cmdVenta.Parameters.AddWithValue("@t", total);
                cmdVenta.Parameters.AddWithValue("@m", metodoPago);
                cmdVenta.Parameters.AddWithValue("@c", (object?)clienteFiadoId ?? DBNull.Value);

                int ventaId = Convert.ToInt32(cmdVenta.ExecuteScalar());

                
                foreach (var item in items)
                {
                    using var cmdDetalle = conn.CreateCommand();
                    cmdDetalle.Transaction = tx;

                    cmdDetalle.CommandText = @"
                        INSERT INTO venta_detalle
                        (venta_id, producto_id, cantidad, precio_unitario, subtotal)
                        VALUES (@v, @p, @c, @pr, @s);";

                    cmdDetalle.Parameters.AddWithValue("@v", ventaId);
                    cmdDetalle.Parameters.AddWithValue("@p", item.productoId);
                    cmdDetalle.Parameters.AddWithValue("@c", item.cantidad);
                    cmdDetalle.Parameters.AddWithValue("@pr", item.precio);
                    cmdDetalle.Parameters.AddWithValue("@s", item.cantidad * item.precio);

                    cmdDetalle.ExecuteNonQuery();
                }

                
                if (string.Equals(metodoPago, "FIADO", StringComparison.OrdinalIgnoreCase)
                    && clienteFiadoId.HasValue)
                {
                    using var cmdFiado = conn.CreateCommand();
                    cmdFiado.Transaction = tx;

                    cmdFiado.CommandText = @"
                        UPDATE clientes_fiados
                        SET saldoActual = saldoActual + @t
                        WHERE id = @id;";

                    cmdFiado.Parameters.AddWithValue("@t", total);
                    cmdFiado.Parameters.AddWithValue("@id", clienteFiadoId.Value);
                    cmdFiado.ExecuteNonQuery();

                    
                    using var cmdMov = conn.CreateCommand();
                    cmdMov.Transaction = tx;

                    cmdMov.CommandText = @"
                        INSERT INTO movimientos_cliente_fiado
                            (clienteId, fecha, tipo, monto, descripcion, saldoResultante)
                        SELECT
                            @id, NOW(), 'CARGO', @t, CONCAT('Venta #', @ventaId),
                            saldoActual
                        FROM clientes_fiados
                        WHERE id=@id;";

                    cmdMov.Parameters.AddWithValue("@id", clienteFiadoId.Value);
                    cmdMov.Parameters.AddWithValue("@t", total);
                    cmdMov.Parameters.AddWithValue("@ventaId", ventaId);
                    cmdMov.ExecuteNonQuery();
                }

                tx.Commit();
                return ventaId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        
        public double ObtenerTotalVentasHoy()
        {
            using var conn = Database.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT IFNULL(SUM(total),0)
                FROM ventas
                WHERE DATE(fecha) = CURDATE();";

            return Convert.ToDouble(cmd.ExecuteScalar());
        }

       
        public double ObtenerTotalVentasHoyPorMetodo(string metodoPago)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT IFNULL(SUM(total),0)
                FROM ventas
                WHERE DATE(fecha) = CURDATE()
                  AND metodo_pago = @m;";

            cmd.Parameters.AddWithValue("@m", metodoPago);
            return Convert.ToDouble(cmd.ExecuteScalar());
        }
    }
}
