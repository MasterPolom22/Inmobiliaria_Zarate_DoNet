using Inmobiliaria_Zarate_DoNet.Models;
using MySql.Data.MySqlClient;

namespace Inmobiliaria_Zarate_DoNet.Data
{
    /// <summary>ABM de Usuarios + login.</summary>
    public class UsuarioRepository
    {
        private readonly DbConexion _db;
        public UsuarioRepository(DbConexion db) => _db = db;

        public List<Usuario> GetAll()
        {
            var lista = new List<Usuario>();
            using var conn = _db.CrearConexion();
            const string sql = @"
SELECT id, nombre, apellido, email, password_hash, rol, activo, creado_en
FROM usuario
ORDER BY apellido, nombre;";
            using var cmd = new MySqlCommand(sql, conn);
            using var rd = cmd.ExecuteReader();

            int oId = rd.GetOrdinal("id");
            int oNom = rd.GetOrdinal("nombre");
            int oApe = rd.GetOrdinal("apellido");
            int oMail = rd.GetOrdinal("email");
            int oHash = rd.GetOrdinal("password_hash");
            int oRol = rd.GetOrdinal("rol");
            int oAct = rd.GetOrdinal("activo");
            int oCre = rd.GetOrdinal("creado_en");

            while (rd.Read())
            {
                lista.Add(new Usuario
                {
                    Id = rd.GetInt32(oId),
                    Nombre = rd.GetString(oNom),
                    Apellido = rd.GetString(oApe),
                    Email = rd.GetString(oMail),
                    PasswordHash = rd.GetString(oHash),
                    Rol = Enum.Parse<Rol>(rd.GetString(oRol)),
                    Activo = rd.GetBoolean(oAct),
                    CreadoEn = rd.GetDateTime(oCre)
                });
            }
            return lista;
        }

        public Usuario? GetById(int id)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
SELECT id, nombre, apellido, email, password_hash, rol, activo, creado_en
FROM usuario
WHERE id=@id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            int oId = rd.GetOrdinal("id");
            int oNom = rd.GetOrdinal("nombre");
            int oApe = rd.GetOrdinal("apellido");
            int oMail = rd.GetOrdinal("email");
            int oHash = rd.GetOrdinal("password_hash");
            int oRol = rd.GetOrdinal("rol");
            int oAct = rd.GetOrdinal("activo");
            int oCre = rd.GetOrdinal("creado_en");

            return new Usuario
            {
                Id = rd.GetInt32(oId),
                Nombre = rd.GetString(oNom),
                Apellido = rd.GetString(oApe),
                Email = rd.GetString(oMail),
                PasswordHash = rd.GetString(oHash),
                Rol = Enum.Parse<Rol>(rd.GetString(oRol)),
                Activo = rd.GetBoolean(oAct),
                CreadoEn = rd.GetDateTime(oCre)
            };
        }

        public int Create(Usuario u, string passwordPlano)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(passwordPlano, workFactor: 12);
            using var conn = _db.CrearConexion();
            const string sql = @"
INSERT INTO usuario (nombre, apellido, email, password_hash, rol, activo)
VALUES (@n, @a, @e, @p, @r, @act);
SELECT LAST_INSERT_ID();";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@n", u.Nombre);
            cmd.Parameters.AddWithValue("@a", u.Apellido);
            cmd.Parameters.AddWithValue("@e", u.Email.ToLower());
            cmd.Parameters.AddWithValue("@p", hash);
            cmd.Parameters.AddWithValue("@r", u.Rol.ToString()); // 'ADMIN'|'EMPLEADO'
            cmd.Parameters.AddWithValue("@act", u.Activo);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int Update(Usuario u)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
UPDATE usuario
SET nombre=@n, apellido=@a, email=@e, rol=@r, activo=@act
WHERE id=@id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@n", u.Nombre);
            cmd.Parameters.AddWithValue("@a", u.Apellido);
            cmd.Parameters.AddWithValue("@e", u.Email.ToLower());
            cmd.Parameters.AddWithValue("@r", u.Rol.ToString());
            cmd.Parameters.AddWithValue("@act", u.Activo);
            cmd.Parameters.AddWithValue("@id", u.Id);
            return cmd.ExecuteNonQuery();
        }

        public int UpdatePassword(int id, string passwordPlano)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(passwordPlano, workFactor: 12);
            using var conn = _db.CrearConexion();
            const string sql = @"UPDATE usuario SET password_hash=@p WHERE id=@id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@p", hash);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery();
        }

        public int Delete(int id)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"DELETE FROM usuario WHERE id=@id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>Email único (usar en Create/Edit).</summary>
        public bool ExistsEmail(string email, int? excludeId = null)
        {
            using var conn = _db.CrearConexion();
            var sql = @"SELECT COUNT(1) FROM usuario WHERE email=@e";
            if (excludeId.HasValue) sql += " AND id<>@x";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@e", email.ToLower());
            if (excludeId.HasValue) cmd.Parameters.AddWithValue("@x", excludeId.Value);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        /// <summary>Login (búsqueda por email).</summary>
        public Usuario? GetByEmail(string email)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
SELECT id, nombre, apellido, email, password_hash, rol, activo, creado_en
FROM usuario WHERE email=@e LIMIT 1;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@e", email.ToLower().Trim());
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new Usuario
            {
                Id = rd.GetInt32("id"),
                Nombre = rd.GetString("nombre"),
                Apellido = rd.GetString("apellido"),
                Email = rd.GetString("email"),
                PasswordHash = rd.GetString("password_hash"),
                Rol = Enum.Parse<Rol>(rd.GetString("rol")),
                Activo = rd.GetBoolean("activo"),
                CreadoEn = rd.GetDateTime("creado_en")
            };
        }
    }
}
