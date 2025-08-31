// PropietarioRepo.cs
using Inmobiliaria_Zarate_DoNet.Models;
using MySql.Data.MySqlClient;

namespace Inmobiliaria_Zarate_DoNet.Data
{
    public class PropietarioRepository
    {
        private readonly DbConexion _db;

        public PropietarioRepository(DbConexion db)
        {
            _db = db;
        }

        // LISTAR
        public List<Propietario> GetAll()
        {
            var lista = new List<Propietario>();

            using (var conn = _db.CrearConexion())
            {
                const string sql = @"SELECT id, dni, apellido, nombre, telefono, email, creado_en
                                     FROM propietario
                                     ORDER BY apellido, nombre";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    int ordId = reader.GetOrdinal("id");
                    int ordDni = reader.GetOrdinal("dni");
                    int ordApellido = reader.GetOrdinal("apellido");
                    int ordNombre = reader.GetOrdinal("nombre");
                    int ordTelefono = reader.GetOrdinal("telefono");
                    int ordEmail = reader.GetOrdinal("email");
                    int ordCreadoEn = reader.GetOrdinal("creado_en");

                    while (reader.Read())
                    {
                        lista.Add(new Propietario
                        {
                            Id = reader.GetInt32(ordId),
                            Dni = reader.GetString(ordDni),
                            Apellido = reader.GetString(ordApellido),
                            Nombre = reader.GetString(ordNombre),
                            Telefono = reader.IsDBNull(ordTelefono) ? null : reader.GetString(ordTelefono),
                            Email = reader.IsDBNull(ordEmail) ? null : reader.GetString(ordEmail),
                            CreadoEn = reader.GetDateTime(ordCreadoEn)
                        });
                    }
                }
            }

            return lista;
        }

        // OBTENER POR ID
        public Propietario? GetById(int id)
        {
            using (var conn = _db.CrearConexion())
            {
                const string sql = @"SELECT id, dni, apellido, nombre, telefono, email, creado_en
                                     FROM propietario
                                     WHERE id = @id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;

                        int ordId = reader.GetOrdinal("id");
                        int ordDni = reader.GetOrdinal("dni");
                        int ordApellido = reader.GetOrdinal("apellido");
                        int ordNombre = reader.GetOrdinal("nombre");
                        int ordTelefono = reader.GetOrdinal("telefono");
                        int ordEmail = reader.GetOrdinal("email");
                        int ordCreadoEn = reader.GetOrdinal("creado_en");

                        return new Propietario
                        {
                            Id = reader.GetInt32(ordId),
                            Dni = reader.GetString(ordDni),
                            Apellido = reader.GetString(ordApellido),
                            Nombre = reader.GetString(ordNombre),
                            Telefono = reader.IsDBNull(ordTelefono) ? null : reader.GetString(ordTelefono),
                            Email = reader.IsDBNull(ordEmail) ? null : reader.GetString(ordEmail),
                            CreadoEn = reader.GetDateTime(ordCreadoEn)
                        };
                    }
                }
            }
        }

        // CREAR
        public int Create(Propietario p)
        {
            using (var conn = _db.CrearConexion())
            {
                const string sql = @"INSERT INTO propietario (dni, apellido, nombre, telefono, email)
                                     VALUES (@dni, @apellido, @nombre, @telefono, @email);
                                     SELECT LAST_INSERT_ID();";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@dni", p.Dni);
                    cmd.Parameters.AddWithValue("@apellido", p.Apellido);
                    cmd.Parameters.AddWithValue("@nombre", p.Nombre);
                    cmd.Parameters.AddWithValue("@telefono", (object?)p.Telefono ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@email", (object?)p.Email ?? DBNull.Value);

                    // Devuelve el nuevo ID
                    var result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        // ACTUALIZAR
        public int Update(Propietario p)
        {
            using (var conn = _db.CrearConexion())
            {
                const string sql = @"UPDATE propietario
                                     SET dni = @dni,
                                         apellido = @apellido,
                                         nombre = @nombre,
                                         telefono = @telefono,
                                         email = @email
                                     WHERE id = @id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@dni", p.Dni);
                    cmd.Parameters.AddWithValue("@apellido", p.Apellido);
                    cmd.Parameters.AddWithValue("@nombre", p.Nombre);
                    cmd.Parameters.AddWithValue("@telefono", (object?)p.Telefono ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@email", (object?)p.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", p.Id);

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // ELIMINAR
        public int Delete(int id)
        {
            using (var conn = _db.CrearConexion())
            {
                const string sql = @"DELETE FROM propietario WHERE id = @id";
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
                string sql = @"SELECT COUNT(1) FROM propietario WHERE dni = @dni";
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
