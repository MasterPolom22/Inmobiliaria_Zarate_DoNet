using MySql.Data.MySqlClient;
using Inmobiliaria_Zarate_DoNet.Models;

namespace Inmobiliaria_Zarate_DoNet.Data
{
    public class TipoInmuebleRepository
    {
        private readonly DbConexion _db;
        public TipoInmuebleRepository(DbConexion db) => _db = db;

        public List<TipoInmueble> GetAll()
        {
            var lista = new List<TipoInmueble>();
            using var conn = _db.CrearConexion();
            string sql = "SELECT id, nombre, activo, creado_en FROM tipo_inmueble ORDER BY nombre";
            using var cmd = new MySqlCommand(sql, conn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new TipoInmueble
                {
                    Id = rd.GetInt32("id"),
                    Nombre = rd.GetString("nombre"),
                    Activo = rd.GetBoolean("activo"),
                    CreadoEn = rd.GetDateTime("creado_en")
                });
            }
            return lista;
        }

        public TipoInmueble? GetById(int id)
        {
            using var conn = _db.CrearConexion();
            string sql = "SELECT id, nombre, activo, creado_en FROM tipo_inmueble WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var rd = cmd.ExecuteReader();
            if (rd.Read())
            {
                return new TipoInmueble
                {
                    Id = rd.GetInt32("id"),
                    Nombre = rd.GetString("nombre"),
                    Activo = rd.GetBoolean("activo"),
                    CreadoEn = rd.GetDateTime("creado_en")
                };
            }
            return null;
        }

        public int Create(TipoInmueble t)
        {
            using var conn = _db.CrearConexion();
            string sql = @"INSERT INTO tipo_inmueble (nombre, activo) 
                           VALUES (@nombre, @activo);
                           SELECT LAST_INSERT_ID();";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@nombre", t.Nombre);
            cmd.Parameters.AddWithValue("@activo", t.Activo);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int Update(TipoInmueble t)
        {
            using var conn = _db.CrearConexion();
            string sql = @"UPDATE tipo_inmueble 
                           SET nombre=@nombre, activo=@activo 
                           WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", t.Id);
            cmd.Parameters.AddWithValue("@nombre", t.Nombre);
            cmd.Parameters.AddWithValue("@activo", t.Activo);
            return cmd.ExecuteNonQuery();
        }

        public int Delete(int id)
        {
            using var conn = _db.CrearConexion();
            string sql = "DELETE FROM tipo_inmueble WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery();
        }
    }
}
