using Inmobiliaria_Zarate_DoNet.Models;
using MySql.Data.MySqlClient;

namespace Inmobiliaria_Zarate_DoNet.Data
{
    /// <summary>Pagos por contrato + anulación con auditoría.</summary>
    public class PagoRepository
    {
        private readonly DbConexion _db;
        public PagoRepository(DbConexion db) => _db = db;

        public List<Pago> GetByContrato(int contratoId)
        {
            var lista = new List<Pago>();
            using var conn = _db.CrearConexion();
            const string sql = @"
SELECT p.*, CONCAT(i.direccion, ' – ', inq.apellido, ', ', inq.nombre) AS resumen
FROM pago p
JOIN contrato c    ON c.id = p.contrato_id
JOIN inmueble i    ON i.id = c.inmueble_id
JOIN inquilino inq ON inq.id = c.inquilino_id
WHERE p.contrato_id = @contratoId
ORDER BY p.numero;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@contratoId", contratoId);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                lista.Add(MapPago(r));
            }
            return lista;
        }

        public Pago? GetById(int id)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
SELECT p.*, CONCAT(i.direccion, ' – ', inq.apellido, ', ', inq.nombre) AS resumen
FROM pago p
JOIN contrato c    ON c.id = p.contrato_id
JOIN inmueble i    ON i.id = c.inmueble_id
JOIN inquilino inq ON inq.id = c.inquilino_id
WHERE p.id = @id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return MapPago(r);
        }

        public int Create(Pago p)
        {
            using var conn = _db.CrearConexion();
            using var tx = conn.BeginTransaction();

            // Próximo número
            const string sqlNext = @"SELECT COALESCE(MAX(numero),0)+1 FROM pago WHERE contrato_id=@cid FOR UPDATE;";
            int nextNumero;
            using (var cmdNext = new MySqlCommand(sqlNext, conn, tx))
            {
                cmdNext.Parameters.AddWithValue("@cid", p.ContratoId);
                nextNumero = Convert.ToInt32(cmdNext.ExecuteScalar());
            }

            const string sqlIns = @"
INSERT INTO pago (contrato_id, numero, fecha, detalle, importe, anulado, creado_por, creado_en)
VALUES (@cid, @num, @fecha, @det, @imp, 0, @creadoPor, NOW());
SELECT LAST_INSERT_ID();";
            using var cmd = new MySqlCommand(sqlIns, conn, tx);
            cmd.Parameters.AddWithValue("@cid", p.ContratoId);
            cmd.Parameters.AddWithValue("@num", nextNumero);
            cmd.Parameters.AddWithValue("@fecha", p.Fecha);
            cmd.Parameters.AddWithValue("@det", p.Detalle);
            cmd.Parameters.AddWithValue("@imp", p.Importe);
            cmd.Parameters.AddWithValue("@creadoPor", p.CreadoPor);
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            tx.Commit();
            return id;
        }

        public int Anular(int idPago, int anuladoPor)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
UPDATE pago
SET anulado=1, anulado_por=@anuladoPor, anulado_en=NOW()
WHERE id=@id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", idPago);
            cmd.Parameters.AddWithValue("@anuladoPor", anuladoPor);
            return cmd.ExecuteNonQuery();
        }

        public int Delete(int id)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"DELETE FROM pago WHERE id=@id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery();
        }

        private Pago MapPago(MySqlDataReader r)
        {
            return new Pago
            {
                Id = r.GetInt32("id"),
                ContratoId = r.GetInt32("contrato_id"),
                Numero = r.GetInt32("numero"),
                Fecha = r.GetDateTime("fecha"),
                Detalle = r.GetString("detalle"),
                Importe = r.GetDecimal("importe"),
                Anulado = r.GetBoolean("anulado"),
                CreadoPor = r.GetInt32("creado_por"),
                AnuladoPor = r.IsDBNull(r.GetOrdinal("anulado_por")) ? null : r.GetInt32("anulado_por"),
                CreadoEn = r.GetDateTime("creado_en"),
                AnuladoEn = r.IsDBNull(r.GetOrdinal("anulado_en")) ? null : r.GetDateTime("anulado_en"),
                ContratoResumen = r.GetString("resumen")
            };
        }
    }
}
