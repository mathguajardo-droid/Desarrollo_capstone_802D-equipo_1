using System;
using System.Collections.Generic;
using MiniMarket.Models;
using MySql.Data.MySqlClient;

namespace MiniMarket.Services
{
    public class ClienteFiadoService
    {
        public List<ClienteFiado> GetClientes()
        {
            var list = new List<ClienteFiado>();

            using var con = Database.GetConnection();
            con.Open();

            using var cmd = new MySqlCommand(@"
                SELECT id, nombre, telefono, direccion, saldoActual
                FROM clientes_fiados
                ORDER BY nombre;", con);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new ClienteFiado
                {
                    Id = Convert.ToInt32(rd["id"]),
                    Nombre = rd["nombre"].ToString() ?? "",
                    Telefono = rd["telefono"].ToString() ?? "",
                    Direccion = rd["direccion"].ToString() ?? "",
                    SaldoActual = Convert.ToDouble(rd["saldoActual"])
                });
            }

            return list;
        }

        
        public List<ClienteFiado> GetAll() => GetClientes();

        public void AddCliente(ClienteFiado c)
        {
            using var con = Database.GetConnection();
            con.Open();

            using var cmd = new MySqlCommand(@"
                INSERT INTO clientes_fiados(nombre, telefono, direccion, saldoActual)
                VALUES(@n, @t, @d, 0);", con);

            cmd.Parameters.AddWithValue("@n", c.Nombre);
            cmd.Parameters.AddWithValue("@t", c.Telefono);
            cmd.Parameters.AddWithValue("@d", c.Direccion);
            cmd.ExecuteNonQuery();
        }

        public void UpdateCliente(ClienteFiado c)
        {
            using var con = Database.GetConnection();
            con.Open();

            using var cmd = new MySqlCommand(@"
                UPDATE clientes_fiados
                SET nombre=@n, telefono=@t, direccion=@d
                WHERE id=@id;", con);

            cmd.Parameters.AddWithValue("@n", c.Nombre);
            cmd.Parameters.AddWithValue("@t", c.Telefono);
            cmd.Parameters.AddWithValue("@d", c.Direccion);
            cmd.Parameters.AddWithValue("@id", c.Id);
            cmd.ExecuteNonQuery();
        }

        public List<(string fecha, string tipo, double monto, string descripcion, double saldoResultante)>
            GetMovimientos(int clienteId)
        {
            var list = new List<(string, string, double, string, double)>();

            using var con = Database.GetConnection();
            con.Open();

            using var cmd = new MySqlCommand(@"
                SELECT fecha, tipo, monto, descripcion, saldoResultante
                FROM movimientos_cliente_fiado
                WHERE clienteId = @id
                ORDER BY fecha DESC;", con);

            cmd.Parameters.AddWithValue("@id", clienteId);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add((
                    Convert.ToDateTime(rd["fecha"]).ToString("yyyy-MM-dd HH:mm"),
                    rd["tipo"].ToString() ?? "",
                    Convert.ToDouble(rd["monto"]),
                    rd["descripcion"].ToString() ?? "",
                    Convert.ToDouble(rd["saldoResultante"])
                ));
            }

            return list;
        }

        public void RegistrarCargo(int clienteId, double monto, string descripcion)
            => RegistrarMovimiento(clienteId, "CARGO", monto, descripcion);

        public void RegistrarAbono(int clienteId, double monto, string descripcion)
            => RegistrarMovimiento(clienteId, "ABONO", monto, descripcion);

        
        public void RegistrarCargo(int clienteId, string descripcion, double monto)
            => RegistrarCargo(clienteId, monto, descripcion);

        public void RegistrarAbono(int clienteId, string descripcion, double monto)
            => RegistrarAbono(clienteId, monto, descripcion);

        private void RegistrarMovimiento(int clienteId, string tipo, double monto, string descripcion)
        {
            using var con = Database.GetConnection();
            con.Open();

            
            double saldoActual;
            using (var cmdSaldo = new MySqlCommand(
                "SELECT saldoActual FROM clientes_fiados WHERE id=@id;", con))
            {
                cmdSaldo.Parameters.AddWithValue("@id", clienteId);
                saldoActual = Convert.ToDouble(cmdSaldo.ExecuteScalar() ?? 0);
            }

            double nuevoSaldo = (tipo == "CARGO")
                ? saldoActual + monto
                : saldoActual - monto;

            
            using (var cmdMov = new MySqlCommand(@"
                INSERT INTO movimientos_cliente_fiado
                    (clienteId, fecha, tipo, monto, descripcion, saldoResultante)
                VALUES
                    (@id, NOW(), @t, @m, @d, @s);", con))
            {
                cmdMov.Parameters.AddWithValue("@id", clienteId);
                cmdMov.Parameters.AddWithValue("@t", tipo);
                cmdMov.Parameters.AddWithValue("@m", monto);
                cmdMov.Parameters.AddWithValue("@d", descripcion);
                cmdMov.Parameters.AddWithValue("@s", nuevoSaldo);
                cmdMov.ExecuteNonQuery();
            }

            
            using (var cmdUpd = new MySqlCommand(
                "UPDATE clientes_fiados SET saldoActual=@s WHERE id=@id;", con))
            {
                cmdUpd.Parameters.AddWithValue("@s", nuevoSaldo);
                cmdUpd.Parameters.AddWithValue("@id", clienteId);
                cmdUpd.ExecuteNonQuery();
            }
        }

        
        public List<ClienteFiado> GetFiadosVencidos(int diasVencimiento = 30, double saldoMinimo = 1)
        {
            var lista = new List<ClienteFiado>();

            using var con = Database.GetConnection();
            con.Open();

            using var cmd = new MySqlCommand(@"
                SELECT c.id, c.nombre, c.telefono, c.direccion, c.saldoActual
                FROM clientes_fiados c
                WHERE c.saldoActual >= @saldo
                AND c.id IN (
                    SELECT clienteId
                    FROM movimientos_cliente_fiado
                    GROUP BY clienteId
                    HAVING MAX(fecha) <= DATE_SUB(NOW(), INTERVAL @dias DAY)
                )
                ORDER BY c.saldoActual DESC;", con);

            cmd.Parameters.AddWithValue("@dias", diasVencimiento);
            cmd.Parameters.AddWithValue("@saldo", saldoMinimo);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new ClienteFiado
                {
                    Id = Convert.ToInt32(rd["id"]),
                    Nombre = rd["nombre"].ToString() ?? "",
                    Telefono = rd["telefono"].ToString() ?? "",
                    Direccion = rd["direccion"].ToString() ?? "",
                    SaldoActual = Convert.ToDouble(rd["saldoActual"])
                });
            }

            return lista;
        }
    }
}
