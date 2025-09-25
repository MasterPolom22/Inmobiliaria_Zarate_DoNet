using Inmobiliaria_Zarate_DoNet.Models;
using MySql.Data.MySqlClient;

namespace Inmobiliaria_Zarate_DoNet.Data
{
    /// <summary>ABM de Propietarios.</summary>
    public class PropietarioRepository
    {
        private readonly DbConexion _db;
        public PropietarioRepository(DbConexion db) => _db = db;

        public List<Propietario> GetAll()
        {
            var lista = new List<Propietario>();
            using var conn = _db.CrearConexion();
            const string sql = @"
SELECT id, dni, apellido, nombre, telefono, email, creado_en
FROM propietario
ORDER BY apellido, nombre;";
            using var cmd = new MySqlCommand(sql, conn);
            using var r = cmd.ExecuteReader();

            int oId = r.GetOrdinal("id");
            int oDni = r.GetOrdinal("dni");
            int oApe = r.GetOrdinal("apellido");
            int oNom = r.GetOrdinal("nombre");
            int oTel = r.GetOrdinal("telefono");
            int oMail = r.GetOrdinal("email");
            int oCre = r.GetOrdinal("creado_en");

            while (r.Read())
            {
                lista.Add(new Propietario
                {
                    Id = r.GetInt32(oId),
                    Dni = r.GetString(oDni),
                    Apellido = r.GetString(oApe),
                    Nombre = r.GetString(oNom),
                    Telefono = r.IsDBNull(oTel) ? null : r.GetString(oTel),
                    Email = r.IsDBNull(oMail) ? null : r.GetString(oMail),
                    CreadoEn = r.GetDateTime(oCre)
                });
            }
            return lista;
        }

        public Propietario? GetById(int id)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
SELECT id, dni, apellido, nombre, telefono, email, creado_en
FROM propietario
WHERE id = @id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            int oId = r.GetOrdinal("id");
            int oDni = r.GetOrdinal("dni");
            int oApe = r.GetOrdinal("apellido");
            int oNom = r.GetOrdinal("nombre");
            int oTel = r.GetOrdinal("telefono");
            int oMail = r.GetOrdinal("email");
            int oCre = r.GetOrdinal("creado_en");

            return new Propietario
            {
                Id = r.GetInt32(oId),
                Dni = r.GetString(oDni),
                Apellido = r.GetString(oApe),
                Nombre = r.GetString(oNom),
                Telefono = r.IsDBNull(oTel) ? null : r.GetString(oTel),
                Email = r.IsDBNull(oMail) ? null : r.GetString(oMail),
                CreadoEn = r.GetDateTime(oCre)
            };
        }

        public int Create(Propietario p)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
INSERT INTO propietario (dni, apellido, nombre, telefono, email)
VALUES (@dni, @apellido, @nombre, @telefono, @email);
SELECT LAST_INSERT_ID();";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@dni", p.Dni);
            cmd.Parameters.AddWithValue("@apellido", p.Apellido);
            cmd.Parameters.AddWithValue("@nombre", p.Nombre);
            cmd.Parameters.AddWithValue("@telefono", (object?)p.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object?)p.Email ?? DBNull.Value);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int Update(Propietario p)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
UPDATE propietario
SET dni = @dni,
    apellido = @apellido,
    nombre = @nombre,
    telefono = @telefono,
    email = @email
WHERE id = @id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@dni", p.Dni);
            cmd.Parameters.AddWithValue("@apellido", p.Apellido);
            cmd.Parameters.AddWithValue("@nombre", p.Nombre);
            cmd.Parameters.AddWithValue("@telefono", (object?)p.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object?)p.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", p.Id);
            return cmd.ExecuteNonQuery();
        }

        public int Delete(int id)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"DELETE FROM propietario WHERE id = @id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery();
        }

        
        public bool ExistsDni(string dni, int? excludeId = null)
        {
            using var conn = _db.CrearConexion();
            var sql = @"SELECT COUNT(1) FROM propietario WHERE dni = @dni";
            if (excludeId.HasValue) sql += " AND id <> @x";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@dni", dni);
            if (excludeId.HasValue) cmd.Parameters.AddWithValue("@x", excludeId.Value);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
    }
}
