using Inmobiliaria_Zarate_DoNet.Models;
using MySql.Data.MySqlClient;

namespace Inmobiliaria_Zarate_DoNet.Data
{
    public class PagoRepository
    {
        private readonly DbConexion _db;
        public PagoRepository(DbConexion db) => _db = db;

        // LISTAR por contrato
        public List<Pago> GetByContrato(int contratoId)
        {
            var lista = new List<Pago>();
            using var conn = _db.CrearConexion();

            const string sql = @"
SELECT p.id, p.contrato_id, p.numero, p.fecha, p.detalle, p.importe, p.anulado, p.creado_por, p.anulado_por, p.creado_en,
       CONCAT(i.direccion, ' – ', inq.Apellido, ', ', inq.nombre) AS resumen
FROM pago p
JOIN contrato c    ON c.id = p.contrato_id
JOIN inmueble i    ON i.id = c.inmueble_id
JOIN inquilino inq ON inq.id = c.inquilino_id
WHERE p.contrato_id = @contratoId
ORDER BY p.numero;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@contratoId", contratoId);
            using var r = cmd.ExecuteReader();

            int oId = r.GetOrdinal("id");
            int oC  = r.GetOrdinal("contrato_id");
            int oN  = r.GetOrdinal("numero");
            int oF  = r.GetOrdinal("fecha");
            int oDet= r.GetOrdinal("detalle");
            int oImp= r.GetOrdinal("importe");
            int oAnu= r.GetOrdinal("anulado");
            int oCrePor = r.GetOrdinal("creado_por");
            int oAnuPor = r.GetOrdinal("anulado_por");
            int oCre = r.GetOrdinal("creado_en");
            int oRes = r.GetOrdinal("resumen");

            while (r.Read())
            {
                lista.Add(new Pago
                {
                    Id = r.GetInt32(oId),
                    ContratoId = r.GetInt32(oC),
                    Numero = r.GetInt32(oN),
                    Fecha = r.GetDateTime(oF),
                    Detalle = r.GetString(oDet),
                    Importe = r.GetDecimal(oImp),
                    Anulado = r.GetBoolean(oAnu),
                    CreadoPor = r.GetInt32(oCrePor),
                    AnuladoPor = r.IsDBNull(oAnuPor) ? null : r.GetInt32(oAnuPor),
                    CreadoEn = r.GetDateTime(oCre),
                    ContratoResumen = r.GetString(oRes)
                });
            }
            return lista;
        }

        // OBTENER
        public Pago? GetById(int id)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
SELECT p.id, p.contrato_id, p.numero, p.fecha, p.detalle, p.importe, p.anulado, p.creado_por, p.anulado_por, p.creado_en,
       CONCAT(i.direccion, ' – ', inq.Apellido, ', ', inq.nombre) AS resumen
FROM pago p
JOIN contrato c    ON c.id = p.contrato_id
JOIN inmueble i    ON i.id = c.inmueble_id
JOIN inquilino inq ON inq.id = c.inquilino_id
WHERE p.id = @id;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            int oId = r.GetOrdinal("id");
            int oC  = r.GetOrdinal("contrato_id");
            int oN  = r.GetOrdinal("numero");
            int oF  = r.GetOrdinal("fecha");
            int oDet= r.GetOrdinal("detalle");
            int oImp= r.GetOrdinal("importe");
            int oAnu= r.GetOrdinal("anulado");
            int oCrePor = r.GetOrdinal("creado_por");
            int oAnuPor = r.GetOrdinal("anulado_por");
            int oCre = r.GetOrdinal("creado_en");
            int oRes = r.GetOrdinal("resumen");

            return new Pago
            {
                Id = r.GetInt32(oId),
                ContratoId = r.GetInt32(oC),
                Numero = r.GetInt32(oN),
                Fecha = r.GetDateTime(oF),
                Detalle = r.GetString(oDet),
                Importe = r.GetDecimal(oImp),
                Anulado = r.GetBoolean(oAnu),
                CreadoPor = r.GetInt32(oCrePor),
                AnuladoPor = r.IsDBNull(oAnuPor) ? null : r.GetInt32(oAnuPor),
                CreadoEn = r.GetDateTime(oCre),
                ContratoResumen = r.GetString(oRes)
            };
        }

        // CREAR (numero lo autogenera trigger)
       public int Create(Pago p)
{
    using var conn = _db.CrearConexion();
    using var tx = conn.BeginTransaction(); // importante

    // 1) Calcular el próximo número del contrato con bloqueo de lectura-escritura
    int nextNumero;
    const string sqlNext = @"
        SELECT COALESCE(MAX(numero), 0) + 1
        FROM pago
        WHERE contrato_id = @contratoId
        FOR UPDATE;"; // bloquea filas candidatas hasta COMMIT
    using (var cmdNext = new MySqlCommand(sqlNext, conn, tx))
    {
        cmdNext.Parameters.AddWithValue("@contratoId", p.ContratoId);
        nextNumero = Convert.ToInt32(cmdNext.ExecuteScalar());
    }

    // 2) Insertar el pago con ese número
    const string sqlIns = @"
        INSERT INTO pago (contrato_id, numero, fecha, detalle, importe, anulado, creado_por, anulado_por)
        VALUES (@contrato_id, @numero, @fecha, @detalle, @importe, 0, @creado_por, NULL);
        SELECT LAST_INSERT_ID();";
    using (var cmd = new MySqlCommand(sqlIns, conn, tx))
    {
        cmd.Parameters.AddWithValue("@contrato_id", p.ContratoId);
        cmd.Parameters.AddWithValue("@numero", nextNumero);
        cmd.Parameters.AddWithValue("@fecha", p.Fecha);
        cmd.Parameters.AddWithValue("@detalle", p.Detalle);
        cmd.Parameters.AddWithValue("@importe", p.Importe);
        cmd.Parameters.AddWithValue("@creado_por", p.CreadoPor);

        var id = Convert.ToInt32(cmd.ExecuteScalar());
        tx.Commit();
        return id;
    }
}


        // ANULAR
        public int Anular(int idPago, int anuladoPor)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"
UPDATE pago
SET anulado = 1,
    anulado_por = @anulado_por
WHERE id = @id;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@anulado_por", anuladoPor);
            cmd.Parameters.AddWithValue("@id", idPago);

            return cmd.ExecuteNonQuery();
        }

        // (Opcional) ELIMINAR
        public int Delete(int id)
        {
            using var conn = _db.CrearConexion();
            const string sql = @"DELETE FROM pago WHERE id = @id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery();
        }
    }
}
