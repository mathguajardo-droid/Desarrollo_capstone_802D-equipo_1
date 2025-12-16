using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MiniMarket.Models;

namespace MiniMarket.Services
{
    public class UserService
    {
        
        public List<UsuarioRow> GetAll()
        {
            var lista = new List<UsuarioRow>();

            using var conn = Database.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, usuario, password, rol
                FROM usuarios
                ORDER BY id;";

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new UsuarioRow
                {
                    Id       = rd.GetInt32("id"),
                    Usuario  = rd.GetString("usuario"),
                    Password = rd.GetString("password"),
                    Rol      = rd.GetString("rol")
                });
            }

            return lista;
        }

        
        public void Add(UsuarioRow u)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO usuarios (usuario, password, rol)
                VALUES (@u, @p, @r);";

            cmd.Parameters.AddWithValue("@u", u.Usuario);
            cmd.Parameters.AddWithValue("@p", u.Password);
            cmd.Parameters.AddWithValue("@r", u.Rol);

            cmd.ExecuteNonQuery();
        }

        
        public void Update(UsuarioRow u)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE usuarios
                SET usuario  = @u,
                    password = @p,
                    rol      = @r
                WHERE id = @id;";

            cmd.Parameters.AddWithValue("@u", u.Usuario);
            cmd.Parameters.AddWithValue("@p", u.Password);
            cmd.Parameters.AddWithValue("@r", u.Rol);
            cmd.Parameters.AddWithValue("@id", u.Id);

            cmd.ExecuteNonQuery();
        }

        
        public void Delete(int id)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM usuarios WHERE id = @id;";
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

        
        public UsuarioRow? Login(string usuario, string password)
        {
            using var conn = Database.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, usuario, password, rol
                FROM usuarios
                WHERE usuario = @u AND password = @p
                LIMIT 1;";

            cmd.Parameters.AddWithValue("@u", usuario);
            cmd.Parameters.AddWithValue("@p", password);

            using var rd = cmd.ExecuteReader();
            if (!rd.Read())
                return null;

            return new UsuarioRow
            {
                Id       = rd.GetInt32("id"),
                Usuario  = rd.GetString("usuario"),
                Password = rd.GetString("password"),
                Rol      = rd.GetString("rol")
            };
        }
    }
}
