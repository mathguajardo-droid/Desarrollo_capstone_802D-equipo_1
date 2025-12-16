using System;
using System.Collections.Generic;
using MiniMarket.Models;
using MySql.Data.MySqlClient;

namespace MiniMarket.Services
{
    public class ProveedorService
    {
        public List<Proveedor> GetProveedores()
        {
            var list = new List<Proveedor>();

            using var con = Database.GetConnection();
            con.Open();

            using var cmd = new MySqlCommand(@"SELECT id, nombre, telefono, contacto, saldoActual FROM proveedores ORDER BY nombre;", con);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new Proveedor
                {
                    Id = Convert.ToInt32(rd["id"]),
                    Nombre = rd["nombre"].ToString() ?? "",
                    Telefono = rd["telefono"].ToString() ?? "",
                    Contacto = rd["contacto"].ToString() ?? "",
                    SaldoActual = Convert.ToDouble(rd["saldoActual"])
                });
            }

            return list;
        }

        public List<(string fecha, string tipo, double monto, string descripcion, double saldoResultante)> GetMovimientos(int proveedorId)
        {
            var list = new List<(string, string, double, string, double)>();

            using var con = Database.GetConnection();
            con.Open();

            using var cmd = new MySqlCommand(@"
                SELECT fecha, tipo, monto, descripcion, saldoResultante
                FROM movimientos_proveedor
                WHERE proveedorId=@id
                ORDER BY fecha DESC;", con);

            cmd.Parameters.AddWithValue("@id", proveedorId);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add((
                    rd["fecha"].ToString() ?? "",
                    rd["tipo"].ToString() ?? "",
                    Convert.ToDouble(rd["monto"]),
                    rd["descripcion"].ToString() ?? "",
                    Convert.ToDouble(rd["saldoResultante"])
                ));
            }

            return list;
        }

        public void AddProveedor(Proveedor p)
        {
            using var con = Database.GetConnection();
            con.Open();

            using var cmd = new MySqlCommand(@"
                INSERT INTO proveedores(nombre, telefono, contacto, saldoActual)
                VALUES(@n,@t,@c,0);", con);

            cmd.Parameters.AddWithValue("@n", p.Nombre);
            cmd.Parameters.AddWithValue("@t", p.Telefono);
            cmd.Parameters.AddWithValue("@c", p.Contacto);
            cmd.ExecuteNonQuery();
        }

        public void UpdateProveedor(Proveedor p)
        {
            using var con = Database.GetConnection();
            con.Open();

            using var cmd = new MySqlCommand(@"
                UPDATE proveedores
                SET nombre=@n, telefono=@t, contacto=@c
                WHERE id=@id;", con);

            cmd.Parameters.AddWithValue("@n", p.Nombre);
            cmd.Parameters.AddWithValue("@t", p.Telefono);
            cmd.Parameters.AddWithValue("@c", p.Contacto);
            cmd.Parameters.AddWithValue("@id", p.Id);
            cmd.ExecuteNonQuery();
        }

        public void RegistrarCargo(int proveedorId, double monto, string descripcion)
            => RegistrarMovimiento(proveedorId, "CARGO", monto, descripcion);

        
        public void RegistrarAbono(int proveedorId, double monto, string descripcion)
            => RegistrarMovimiento(proveedorId, "ABONO", monto, descripcion);

        private void RegistrarMovimiento(int proveedorId, string tipo, double monto, string descripcion)
        {
            using var con = Database.GetConnection();
            con.Open();

            using var tx = con.BeginTransaction();

            try
            {
                double saldoActual;
                using (var cmdSaldo = con.CreateCommand())
                {
                    cmdSaldo.Transaction = tx;
                    cmdSaldo.CommandText = "SELECT saldoActual FROM proveedores WHERE id=@id";
                    cmdSaldo.Parameters.AddWithValue("@id", proveedorId);
                    saldoActual = Convert.ToDouble(cmdSaldo.ExecuteScalar());
                }

                double nuevoSaldo = (tipo == "CARGO") ? saldoActual + monto : saldoActual - monto;

                using (var cmdMov = con.CreateCommand())
                {
                    cmdMov.Transaction = tx;
                    cmdMov.CommandText = @"
                        INSERT INTO movimientos_proveedor
                        (proveedorId, fecha, tipo, monto, descripcion, saldoResultante)
                        VALUES (@id, NOW(), @t, @m, @d, @s);";
                    cmdMov.Parameters.AddWithValue("@id", proveedorId);
                    cmdMov.Parameters.AddWithValue("@t", tipo);
                    cmdMov.Parameters.AddWithValue("@m", monto);
                    cmdMov.Parameters.AddWithValue("@d", descripcion);
                    cmdMov.Parameters.AddWithValue("@s", nuevoSaldo);
                    cmdMov.ExecuteNonQuery();
                }

                using (var cmdUpd = con.CreateCommand())
                {
                    cmdUpd.Transaction = tx;
                    cmdUpd.CommandText = "UPDATE proveedores SET saldoActual=@s WHERE id=@id";
                    cmdUpd.Parameters.AddWithValue("@s", nuevoSaldo);
                    cmdUpd.Parameters.AddWithValue("@id", proveedorId);
                    cmdUpd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        
        public double TotalPagosProveedoresHoy()
        {
            using var con = Database.GetConnection();
            con.Open();

            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
                SELECT COALESCE(SUM(monto),0)
                FROM movimientos_proveedor
                WHERE tipo='ABONO' AND DATE(fecha)=CURDATE();";

            return Convert.ToDouble(cmd.ExecuteScalar());
        }
    }
}
