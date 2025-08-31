using Inmobiliaria_Zarate_DoNet.Models;
using MySql.Data.MySqlClient;

namespace Inmobiliaria_Zarate_DoNet.Data
{
    public class InquilinoRepository
    {
        private readonly DbConexion _db;

        public InquilinoRepository(DbConexion db)
        {
            _db = db;
        }

        // LISTAR
        public List<Inquilino> GetAll()
        {
            var lista = new List<Inquilino>();

            using (var conn = _db.CrearConexion())
            {
                const string sql = @"SELECT id, dni, Apellido AS apellido, nombre, telefono, email, creado_en
                                     FROM inquilino
                                     ORDER BY apellido, nombre";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    int ordId        = reader.GetOrdinal("id");
                    int ordDni       = reader.GetOrdinal("dni");
                    int ordApellido  = reader.GetOrdinal("apellido");
                    int ordNombre    = reader.GetOrdinal("nombre");
                    int ordTelefono  = reader.GetOrdinal("telefono");
                    int ordEmail     = reader.GetOrdinal("email");
                    int ordCreadoEn  = reader.GetOrdinal("creado_en");

                    while (reader.Read())
                    {
                        lista.Add(new Inquilino
                        {
                            Id       = reader.GetInt32(ordId),
                            Dni      = reader.GetString(ordDni),
                            Apellido = reader.GetString(ordApellido),
                            Nombre   = reader.GetString(ordNombre),
                            Telefono = reader.IsDBNull(ordTelefono) ? null : reader.GetString(ordTelefono),
                            Email    = reader.IsDBNull(ordEmail) ? null : reader.GetString(ordEmail),
                            CreadoEn = reader.GetDateTime(ordCreadoEn)
                        });
                    }
                }
            }

            return lista;
        }

        // OBTENER POR ID
        public Inquilino? GetById(int id)
        {
            using (var conn = _db.CrearConexion())
            {
                const string sql = @"SELECT id, dni, Apellido AS apellido, nombre, telefono, email, creado_en
                                     FROM inquilino
                                     WHERE id = @id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;

                        int ordId        = reader.GetOrdinal("id");
                        int ordDni       = reader.GetOrdinal("dni");
                        int ordApellido  = reader.GetOrdinal("apellido");
                        int ordNombre    = reader.GetOrdinal("nombre");
                        int ordTelefono  = reader.GetOrdinal("telefono");
                        int ordEmail     = reader.GetOrdinal("email");
                        int ordCreadoEn  = reader.GetOrdinal("creado_en");

                        return new Inquilino
                        {
                            Id       = reader.GetInt32(ordId),
                            Dni      = reader.GetString(ordDni),
                            Apellido = reader.GetString(ordApellido),
                            Nombre   = reader.GetString(ordNombre),
                            Telefono = reader.IsDBNull(ordTelefono) ? null : reader.GetString(ordTelefono),
                            Email    = reader.IsDBNull(ordEmail) ? null : reader.GetString(ordEmail),
                            CreadoEn = reader.GetDateTime(ordCreadoEn)
                        };
                    }
                }
            }
        }

        // CREAR
        public int Create(Inquilino i)
        {
            using (var conn = _db.CrearConexion())
            {
                const string sql = @"INSERT INTO inquilino (dni, Apellido, nombre, telefono, email)
                                     VALUES (@dni, @apellido, @nombre, @telefono, @email);
                                     SELECT LAST_INSERT_ID();";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@dni", i.Dni);
                    cmd.Parameters.AddWithValue("@apellido", i.Apellido);
                    cmd.Parameters.AddWithValue("@nombre", i.Nombre);
                    cmd.Parameters.AddWithValue("@telefono", (object?)i.Telefono ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@email", (object?)i.Email ?? DBNull.Value);

                    var result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        // ACTUALIZAR
        public int Update(Inquilino i)
        {
            using (var conn = _db.CrearConexion())
            {
                const string sql = @"UPDATE inquilino
                                     SET dni = @dni,
                                         Apellido = @apellido,
                                         nombre = @nombre,
                                         telefono = @telefono,
                                         email = @email
                                     WHERE id = @id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@dni", i.Dni);
                    cmd.Parameters.AddWithValue("@apellido", i.Apellido);
                    cmd.Parameters.AddWithValue("@nombre", i.Nombre);
                    cmd.Parameters.AddWithValue("@telefono", (object?)i.Telefono ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@email", (object?)i.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", i.Id);

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // ELIMINAR
        public int Delete(int id)
        {
            using (var conn = _db.CrearConexion())
            {
                const string sql = @"DELETE FROM inquilino WHERE id = @id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // VALIDAR DNI ÚNICO
        public bool ExistsDni(string dni, int? excludeId = null)
        {
            using (var conn = _db.CrearConexion())
            {
                string sql = @"SELECT COUNT(1) FROM inquilino WHERE dni = @dni";
                if (excludeId.HasValue)
                    sql += " AND id <> @excludeId";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@dni", dni);
                    if (excludeId.HasValue)
                        cmd.Parameters.AddWithValue("@excludeId", excludeId.Value);

                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
    }
}
