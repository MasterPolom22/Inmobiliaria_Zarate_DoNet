using Inmobiliaria_Zarate_DoNet.Models;
using MySql.Data.MySqlClient;

namespace Inmobiliaria_Zarate_DoNet.Data
{
    public class InmuebleRepository
    {
        private readonly DbConexion _db;
        public InmuebleRepository(DbConexion db) => _db = db;

        // ======== LISTAR ========
        public List<Inmueble> GetAll()
        {
            var lista = new List<Inmueble>();
            using var conn = _db.CrearConexion();

            const string sql = @"
SELECT  i.id, i.propietario_id, i.tipo_id, i.uso, i.direccion, i.ambientes,
        i.latitud, i.longitud, i.precio_base, i.disponible, i.suspendido, i.creado_en,
        CONCAT(p.apellido, ', ', p.nombre) AS propietario,
        t.nombre AS tipo
FROM inmueble i
JOIN propietario p ON p.id = i.propietario_id
JOIN tipo_inmueble t ON t.id = i.tipo_id
ORDER BY i.creado_en DESC;";

            using var cmd = new MySqlCommand(sql, conn);
            using var r = cmd.ExecuteReader();

            int oId = r.GetOrdinal("id");
            int oProp = r.GetOrdinal("propietario_id");
            int oTipo = r.GetOrdinal("tipo_id");
            int oUso = r.GetOrdinal("uso");
            int oDir = r.GetOrdinal("direccion");
            int oAmb = r.GetOrdinal("ambientes");
            int oLat = r.GetOrdinal("latitud");
            int oLon = r.GetOrdinal("longitud");
            int oPrecio = r.GetOrdinal("precio_base");
            int oDisp = r.GetOrdinal("disponible");
            int oSusp = r.GetOrdinal("suspendido");
            int oCreado = r.GetOrdinal("creado_en");
            int oPropNom = r.GetOrdinal("propietario");
            int oTipoNom = r.GetOrdinal("tipo");

            while (r.Read())
            {
                lista.Add(new Inmueble
                {
                    Id = r.GetInt32(oId),
                    PropietarioId = r.GetInt32(oProp),
                    TipoId = r.GetInt32(oTipo),
                    Uso = r.GetString(oUso) == "COMERCIAL" ? UsoInmueble.COMERCIAL : UsoInmueble.RESIDENCIAL,
                    Direccion = r.GetString(oDir),
                    Ambientes = r.GetInt32(oAmb),
                    Latitud = r.IsDBNull(oLat) ? null : r.GetDecimal(oLat),
                    Longitud = r.IsDBNull(oLon) ? null : r.GetDecimal(oLon),
                    PrecioBase = r.GetDecimal(oPrecio),
                    Disponible = r.GetBoolean(oDisp),
                    Suspendido = r.GetBoolean(oSusp),
                    CreadoEn = r.GetDateTime(oCreado),
                    PropietarioNombreCompleto = r.GetString(oPropNom),
                    TipoNombre = r.GetString(oTipoNom)
                });
            }
            return lista;
        }

        // ======== DETALLE ========
        public Inmueble? GetById(int id)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
SELECT  i.id, i.propietario_id, i.tipo_id, i.uso, i.direccion, i.ambientes,
        i.latitud, i.longitud, i.precio_base, i.disponible, i.suspendido, i.creado_en,
        CONCAT(p.apellido, ', ', p.nombre) AS propietario,
        t.nombre AS tipo
FROM inmueble i
JOIN propietario p ON p.id = i.propietario_id
JOIN tipo_inmueble t ON t.id = i.tipo_id
WHERE i.id = @id;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            int oId = r.GetOrdinal("id");
            int oProp = r.GetOrdinal("propietario_id");
            int oTipo = r.GetOrdinal("tipo_id");
            int oUso = r.GetOrdinal("uso");
            int oDir = r.GetOrdinal("direccion");
            int oAmb = r.GetOrdinal("ambientes");
            int oLat = r.GetOrdinal("latitud");
            int oLon = r.GetOrdinal("longitud");
            int oPrecio = r.GetOrdinal("precio_base");
            int oDisp = r.GetOrdinal("disponible");
            int oSusp = r.GetOrdinal("suspendido");
            int oCreado = r.GetOrdinal("creado_en");
            int oPropNom = r.GetOrdinal("propietario");
            int oTipoNom = r.GetOrdinal("tipo");

            return new Inmueble
            {
                Id = r.GetInt32(oId),
                PropietarioId = r.GetInt32(oProp),
                TipoId = r.GetInt32(oTipo),
                Uso = r.GetString(oUso) == "COMERCIAL" ? UsoInmueble.COMERCIAL : UsoInmueble.RESIDENCIAL,
                Direccion = r.GetString(oDir),
                Ambientes = r.GetInt32(oAmb),
                Latitud = r.IsDBNull(oLat) ? null : r.GetDecimal(oLat),
                Longitud = r.IsDBNull(oLon) ? null : r.GetDecimal(oLon),
                PrecioBase = r.GetDecimal(oPrecio),
                Disponible = r.GetBoolean(oDisp),
                Suspendido = r.GetBoolean(oSusp),
                CreadoEn = r.GetDateTime(oCreado),
                PropietarioNombreCompleto = r.GetString(oPropNom),
                TipoNombre = r.GetString(oTipoNom)
            };
        }

        // ======== CREAR ========
        public int Create(Inmueble x)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
INSERT INTO inmueble
(propietario_id, tipo_id, uso, direccion, ambientes, latitud, longitud, precio_base, disponible, suspendido)
VALUES (@propietario_id, @tipo_id, @uso, @direccion, @ambientes, @latitud, @longitud, @precio_base, @disponible, @suspendido);
SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@propietario_id", x.PropietarioId);
            cmd.Parameters.AddWithValue("@tipo_id", x.TipoId);
            cmd.Parameters.AddWithValue("@uso", x.Uso == UsoInmueble.COMERCIAL ? "COMERCIAL" : "RESIDENCIAL");
            cmd.Parameters.AddWithValue("@direccion", x.Direccion);
            cmd.Parameters.AddWithValue("@ambientes", x.Ambientes);
            cmd.Parameters.AddWithValue("@latitud", (object?)x.Latitud ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@longitud", (object?)x.Longitud ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@precio_base", x.PrecioBase);
            cmd.Parameters.AddWithValue("@disponible", x.Disponible);
            cmd.Parameters.AddWithValue("@suspendido", x.Suspendido);

            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        // ======== ACTUALIZAR ========
        public int Update(Inmueble x)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
UPDATE inmueble
SET propietario_id = @propietario_id,
    tipo_id        = @tipo_id,
    uso            = @uso,
    direccion      = @direccion,
    ambientes      = @ambientes,
    latitud        = @latitud,
    longitud       = @longitud,
    precio_base    = @precio_base,
    disponible     = @disponible,
    suspendido     = @suspendido
WHERE id = @id;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@propietario_id", x.PropietarioId);
            cmd.Parameters.AddWithValue("@tipo_id", x.TipoId);
            cmd.Parameters.AddWithValue("@uso", x.Uso == UsoInmueble.COMERCIAL ? "COMERCIAL" : "RESIDENCIAL");
            cmd.Parameters.AddWithValue("@direccion", x.Direccion);
            cmd.Parameters.AddWithValue("@ambientes", x.Ambientes);
            cmd.Parameters.AddWithValue("@latitud", (object?)x.Latitud ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@longitud", (object?)x.Longitud ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@precio_base", x.PrecioBase);
            cmd.Parameters.AddWithValue("@disponible", x.Disponible);
            cmd.Parameters.AddWithValue("@suspendido", x.Suspendido);
            cmd.Parameters.AddWithValue("@id", x.Id);

            return cmd.ExecuteNonQuery();
        }

        // ======== ELIMINAR ========
        public int Delete(int id)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"DELETE FROM inmueble WHERE id = @id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery();
        }

        // ======== LISTAS PARA COMBOS ========

        public List<(int Id, string NombreCompleto)> GetPropietariosForSelect()
        {
            var lista = new List<(int, string)>();
            using var conn = _db.CrearConexion();
            const string sql = @"SELECT id, CONCAT(apellido, ', ', nombre) AS nom FROM propietario ORDER BY apellido, nombre;";
            using var cmd = new MySqlCommand(sql, conn);
            using var r = cmd.ExecuteReader();
            int oId = r.GetOrdinal("id");
            int oNom = r.GetOrdinal("nom");
            while (r.Read()) lista.Add((r.GetInt32(oId), r.GetString(oNom)));
            return lista;
        }

        public List<(int Id, string Nombre)> GetTiposForSelect()
        {
            var lista = new List<(int, string)>();
            using var conn = _db.CrearConexion();
            const string sql = @"SELECT id, nombre FROM tipo_inmueble WHERE activo = 1 ORDER BY nombre;";
            using var cmd = new MySqlCommand(sql, conn);
            using var r = cmd.ExecuteReader();
            int oId = r.GetOrdinal("id");
            int oNom = r.GetOrdinal("nombre");
            while (r.Read()) lista.Add((r.GetInt32(oId), r.GetString(oNom)));
            return lista;
        }
    }
}
