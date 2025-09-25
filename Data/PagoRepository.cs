using MySql.Data.MySqlClient;
using Inmobiliaria_Zarate_DoNet.Models;

namespace Inmobiliaria_Zarate_DoNet.Data
{
    public class PagoRepository
    {
        private readonly DbConexion _db;
        public PagoRepository(DbConexion db) => _db = db;

        public List<Pago> GetByContrato(int contratoId)
        {
            var lista = new List<Pago>();
            using var conn = _db.CrearConexion();
            const string sql = @"
                SELECT id, contrato_id, numero, fecha, detalle, importe,
                       anulado, creado_por, anulado_por, creado_en
                  FROM pago
                 WHERE contrato_id = @cid
                 ORDER BY numero";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@cid", contratoId);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(Map(rd)); // <- ahora es null-safe
            }
            return lista;
        }

       public Pago? GetById(int id)
{
    using var conn = _db.CrearConexion();
    const string sql = @"
        SELECT p.id, p.contrato_id, p.numero, p.fecha, p.detalle, p.importe,
               p.anulado, p.creado_por, p.anulado_por, p.creado_en,
               CONCAT(uc.nombre, ' ', uc.apellido) AS creado_por_nombre,
               CONCAT(ua.nombre, ' ', ua.apellido) AS anulado_por_nombre
          FROM pago p
          LEFT JOIN usuario uc ON uc.id = p.creado_por
          LEFT JOIN usuario ua ON ua.id = p.anulado_por
         WHERE p.id=@id";
    using var cmd = new MySqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("@id", id);
    using var rd = cmd.ExecuteReader();
    if (!rd.Read()) return null;

    return new Pago {
        Id = rd.GetInt32("id"),
        ContratoId = rd.GetInt32("contrato_id"),
        Numero = rd.GetInt32("numero"),
        Fecha = rd.GetDateTime("fecha"),
        Detalle = rd.IsDBNull(rd.GetOrdinal("detalle")) ? "" : rd.GetString("detalle"),
        Importe = rd.GetDecimal("importe"),
        Anulado = rd.GetBoolean("anulado"),
        CreadoPorId = rd.GetInt32("creado_por"),
        AnuladoPorId = rd.IsDBNull(rd.GetOrdinal("anulado_por")) ? (int?)null : rd.GetInt32("anulado_por"),
        CreadoEn = rd.GetDateTime("creado_en"),
        CreadoPorNombre = rd.IsDBNull(rd.GetOrdinal("creado_por_nombre")) ? null : rd.GetString("creado_por_nombre"),
        AnuladoPorNombre = rd.IsDBNull(rd.GetOrdinal("anulado_por_nombre")) ? null : rd.GetString("anulado_por_nombre"),
    };
}

        private int NextNumero(int contratoId, MySqlConnection conn, MySqlTransaction tx)
        {
            const string sql = "SELECT COALESCE(MAX(numero),0)+1 FROM pago WHERE contrato_id=@cid";
            using var cmd = new MySqlCommand(sql, conn, tx);
            cmd.Parameters.AddWithValue("@cid", contratoId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int Create(Pago p, int creadoPor)
        {
            using var conn = _db.CrearConexion();
            using var tx = conn.BeginTransaction();

            var numero = NextNumero(p.ContratoId, conn, tx);

            const string sql = @"
                INSERT INTO pago(contrato_id, numero, fecha, detalle, importe, anulado, creado_por)
                VALUES (@contrato_id, @numero, @fecha, @detalle, @importe, 0, @creado_por);
                SELECT LAST_INSERT_ID();";
            using var cmd = new MySqlCommand(sql, conn, tx);
            cmd.Parameters.AddWithValue("@contrato_id", p.ContratoId);
            cmd.Parameters.AddWithValue("@numero", numero);
            cmd.Parameters.AddWithValue("@fecha", p.Fecha.Date);
            cmd.Parameters.AddWithValue("@detalle", p.Detalle ?? "");
            cmd.Parameters.AddWithValue("@importe", p.Importe);
            cmd.Parameters.AddWithValue("@creado_por", creadoPor);
            var id = Convert.ToInt32(cmd.ExecuteScalar());

            tx.Commit();
            return id;
        }

        public int UpdateDetalle(int id, string detalle)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
                UPDATE pago
                   SET detalle=@detalle
                 WHERE id=@id AND anulado=0";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@detalle", detalle ?? "");
            return cmd.ExecuteNonQuery();
        }

        public int Anular(int id, int anuladoPor)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
                UPDATE pago
                   SET anulado=1, anulado_por=@ap, anulado_en=NOW()
                 WHERE id=@id AND anulado=0";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@ap", anuladoPor);
            return cmd.ExecuteNonQuery();
        }

        private Pago Map(MySqlDataReader rd) => new Pago
        {
            Id = rd.GetInt32("id"),
            ContratoId = rd.GetInt32("contrato_id"),
            Numero = rd.GetInt32("numero"),
            Fecha = rd.GetDateTime("fecha"),
            Detalle = rd.IsDBNull(rd.GetOrdinal("detalle")) ? "" : rd.GetString("detalle"),
            Importe = rd.GetDecimal("importe"),
            Anulado = rd.GetBoolean("anulado"),
            CreadoPorId = rd.GetInt32("creado_por"),
            AnuladoPorId = rd.IsDBNull(rd.GetOrdinal("anulado_por")) ? (int?)null : rd.GetInt32("anulado_por"),
            CreadoEn = rd.GetDateTime("creado_en"),
        };
    }
}
