using Inmobiliaria_Zarate_DoNet.Models;
using MySql.Data.MySqlClient;

namespace Inmobiliaria_Zarate_DoNet.Data
{
    /// <summary>ABM de Contratos + combos.</summary>
    public class ContratoRepository
    {
        private readonly DbConexion _db;
        public ContratoRepository(DbConexion db) => _db = db;

        public List<Contrato> GetAll()
        {
            var lista = new List<Contrato>();
            using var conn = _db.CrearConexion();
            const string sql = @"
SELECT c.id, c.inmueble_id, c.inquilino_id, c.fecha_inicio, c.fecha_fin_original, c.monto_mensual,
       c.fecha_fin_anticipada, c.contrato_origen_id, c.estado, c.creado_por, c.terminado_por, c.creado_en,
       i.direccion AS inm_dir,
       CONCAT(inq.Apellido, ', ', inq.nombre) AS inq_nom
FROM contrato c
JOIN inmueble i    ON i.id = c.inmueble_id
JOIN inquilino inq ON inq.id = c.inquilino_id
ORDER BY c.creado_en DESC;";
            using var cmd = new MySqlCommand(sql, conn);
            using var r = cmd.ExecuteReader();

            int oId = r.GetOrdinal("id");
            int oInm = r.GetOrdinal("inmueble_id");
            int oInq = r.GetOrdinal("inquilino_id");
            int oIni = r.GetOrdinal("fecha_inicio");
            int oFinO = r.GetOrdinal("fecha_fin_original");
            int oMonto = r.GetOrdinal("monto_mensual");
            int oFinA = r.GetOrdinal("fecha_fin_anticipada");
            int oOrig = r.GetOrdinal("contrato_origen_id");
            int oEstado = r.GetOrdinal("estado");
            int oCrePor = r.GetOrdinal("creado_por");
            int oTerPor = r.GetOrdinal("terminado_por");
            int oCre = r.GetOrdinal("creado_en");
            int oDir = r.GetOrdinal("inm_dir");
            int oNom = r.GetOrdinal("inq_nom");

            while (r.Read())
            {
                lista.Add(new Contrato
                {
                    Id = r.GetInt32(oId),
                    InmuebleId = r.GetInt32(oInm),
                    InquilinoId = r.GetInt32(oInq),
                    FechaInicio = r.GetDateTime(oIni),
                    FechaFinOriginal = r.GetDateTime(oFinO),
                    MontoMensual = r.GetDecimal(oMonto),
                    FechaFinAnticipada = r.IsDBNull(oFinA) ? null : r.GetDateTime(oFinA),
                    ContratoOrigenId = r.IsDBNull(oOrig) ? null : r.GetInt32(oOrig),
                    Estado = Enum.Parse<EstadoContrato>(r.GetString(oEstado)),
                    CreadoPor = r.IsDBNull(oCrePor) ? 0 : r.GetInt32(oCrePor),
                    TerminadoPor = r.IsDBNull(oTerPor) ? null : r.GetInt32(oTerPor),
                    CreadoEn = r.GetDateTime(oCre),
                    InmuebleDireccion = r.GetString(oDir),
                    InquilinoNombre = r.GetString(oNom)
                });
            }
            return lista;
        }

        public bool ExisteSolapamiento(int inmuebleId, DateTime inicio, DateTime fin, int? excludeId = null)
        {
            using var conn = _db.CrearConexion();
            var sql = @"
                SELECT COUNT(1)
                  FROM contrato c
                 WHERE c.inmueble_id = @inmuebleId
                   AND (@excludeId IS NULL OR c.id <> @excludeId)
                   AND c.fecha_inicio <= @fin
                   AND COALESCE(c.fecha_fin_anticipada, c.fecha_fin_original) >= @inicio";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@inmuebleId", inmuebleId);
            cmd.Parameters.AddWithValue("@inicio", inicio.Date);
            cmd.Parameters.AddWithValue("@fin", fin.Date);
            cmd.Parameters.AddWithValue("@excludeId", (object?)excludeId ?? DBNull.Value);
            var count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }

        public Contrato? GetById(int id)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
SELECT c.id, c.inmueble_id, c.inquilino_id, c.fecha_inicio, c.fecha_fin_original, c.monto_mensual,
       c.fecha_fin_anticipada, c.contrato_origen_id, c.estado, c.creado_por, c.terminado_por, c.creado_en,
       i.direccion AS inm_dir,
       CONCAT(inq.Apellido, ', ', inq.nombre) AS inq_nom
FROM contrato c
JOIN inmueble i    ON i.id = c.inmueble_id
JOIN inquilino inq ON inq.id = c.inquilino_id
WHERE c.id = @id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            int oId = r.GetOrdinal("id");
            int oInm = r.GetOrdinal("inmueble_id");
            int oInq = r.GetOrdinal("inquilino_id");
            int oIni = r.GetOrdinal("fecha_inicio");
            int oFinO = r.GetOrdinal("fecha_fin_original");
            int oMonto = r.GetOrdinal("monto_mensual");
            int oFinA = r.GetOrdinal("fecha_fin_anticipada");
            int oOrig = r.GetOrdinal("contrato_origen_id");
            int oEstado = r.GetOrdinal("estado");
            int oCrePor = r.GetOrdinal("creado_por");
            int oTerPor = r.GetOrdinal("terminado_por");
            int oCre = r.GetOrdinal("creado_en");
            int oDir = r.GetOrdinal("inm_dir");
            int oNom = r.GetOrdinal("inq_nom");

            return new Contrato
            {
                Id = r.GetInt32(oId),
                InmuebleId = r.GetInt32(oInm),
                InquilinoId = r.GetInt32(oInq),
                FechaInicio = r.GetDateTime(oIni),
                FechaFinOriginal = r.GetDateTime(oFinO),
                MontoMensual = r.GetDecimal(oMonto),
                FechaFinAnticipada = r.IsDBNull(oFinA) ? null : r.GetDateTime(oFinA),
                ContratoOrigenId = r.IsDBNull(oOrig) ? null : r.GetInt32(oOrig),
                Estado = Enum.Parse<EstadoContrato>(r.GetString(oEstado)),
                CreadoPor = r.IsDBNull(oCrePor) ? 0 : r.GetInt32(oCrePor),
                TerminadoPor = r.IsDBNull(oTerPor) ? null : r.GetInt32(oTerPor),
                CreadoEn = r.GetDateTime(oCre),
                InmuebleDireccion = r.GetString(oDir),
                InquilinoNombre = r.GetString(oNom)
            };
        }

        public int Create(Contrato c)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
INSERT INTO contrato
(inmueble_id, inquilino_id, fecha_inicio, fecha_fin_original, monto_mensual, fecha_fin_anticipada, contrato_origen_id, estado, creado_por, terminado_por, creado_en)
VALUES (@inmueble_id, @inquilino_id, @fecha_inicio, @fecha_fin_original, @monto_mensual, @fecha_fin_anticipada, @contrato_origen_id, 'VIGENTE', @creado_por, NULL, NOW());
SELECT LAST_INSERT_ID();";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@inmueble_id", c.InmuebleId);
            cmd.Parameters.AddWithValue("@inquilino_id", c.InquilinoId);
            cmd.Parameters.AddWithValue("@fecha_inicio", c.FechaInicio.Date);
            cmd.Parameters.AddWithValue("@fecha_fin_original", c.FechaFinOriginal.Date);
            cmd.Parameters.AddWithValue("@monto_mensual", c.MontoMensual);
            cmd.Parameters.AddWithValue("@fecha_fin_anticipada", (object?)c.FechaFinAnticipada ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@contrato_origen_id", (object?)c.ContratoOrigenId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@creado_por", c.CreadoPor);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int Update(Contrato c)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
UPDATE contrato
SET inmueble_id = @inmueble_id,
    inquilino_id = @inquilino_id,
    fecha_inicio = @fecha_inicio,
    fecha_fin_original = @fecha_fin_original,
    monto_mensual = @monto_mensual,
    fecha_fin_anticipada = @fecha_fin_anticipada,
    contrato_origen_id = @contrato_origen_id,
    terminado_por = @terminado_por
WHERE id = @id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@inmueble_id", c.InmuebleId);
            cmd.Parameters.AddWithValue("@inquilino_id", c.InquilinoId);
            cmd.Parameters.AddWithValue("@fecha_inicio", c.FechaInicio.Date);
            cmd.Parameters.AddWithValue("@fecha_fin_original", c.FechaFinOriginal.Date);
            cmd.Parameters.AddWithValue("@monto_mensual", c.MontoMensual);
            cmd.Parameters.AddWithValue("@fecha_fin_anticipada", (object?)c.FechaFinAnticipada ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@contrato_origen_id", (object?)c.ContratoOrigenId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@terminado_por", (object?)c.TerminadoPor ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", c.Id);
            return cmd.ExecuteNonQuery();
        }

        public int Terminar(int id, DateTime fechaFinAnticipada, int terminadoPor)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
UPDATE contrato
   SET fecha_fin_anticipada = @ffa,
       estado = 'FINALIZADO',
       terminado_por = @terminado_por
 WHERE id = @id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@ffa", fechaFinAnticipada.Date);
            cmd.Parameters.AddWithValue("@terminado_por", terminadoPor);
            return cmd.ExecuteNonQuery();
        }

        public int Delete(int id)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"DELETE FROM contrato WHERE id = @id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery();
        }

        // Combos
        public List<(int Id, string Texto)> GetInmueblesForSelect()
        {
            var lista = new List<(int, string)>();
            using var conn = _db.CrearConexion();
            const string sql = @"SELECT id, direccion FROM inmueble ORDER BY direccion;";
            using var cmd = new MySqlCommand(sql, conn);
            using var r = cmd.ExecuteReader();
            int oId = r.GetOrdinal("id");
            int oTxt = r.GetOrdinal("direccion");
            while (r.Read()) lista.Add((r.GetInt32(oId), r.GetString(oTxt)));
            return lista;
        }

        public List<(int Id, string Texto)> GetInquilinosForSelect()
        {
            var lista = new List<(int, string)>();
            using var conn = _db.CrearConexion();
            const string sql = @"SELECT id, CONCAT(Apellido, ', ', nombre) AS nom FROM inquilino ORDER BY Apellido, nombre;";
            using var cmd = new MySqlCommand(sql, conn);
            using var r = cmd.ExecuteReader();
            int oId = r.GetOrdinal("id");
            int oTxt = r.GetOrdinal("nom");
            while (r.Read()) lista.Add((r.GetInt32(oId), r.GetString(oTxt)));
            return lista;
        }

         public string? GetUsuarioNombre(int? usuarioId)
        {
            if (usuarioId == null || usuarioId <= 0) return null;
            using var conn = _db.CrearConexion();
            const string sql = @"SELECT CONCAT(apellido, ', ', nombre) AS nom FROM usuario WHERE id=@id LIMIT 1;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", usuarioId);
            var res = cmd.ExecuteScalar();
            return res == null || res == DBNull.Value ? null : Convert.ToString(res);
        }



    }
}
