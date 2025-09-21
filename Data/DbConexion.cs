using MySql.Data.MySqlClient;

namespace Inmobiliaria_Zarate_DoNet.Data
{
    /// <summary>Abre conexiones MySQL.</summary>
    public class DbConexion
    {
        private readonly string _connectionString;

        public DbConexion(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("Cadena de conexión no encontrada");
        }

        public MySqlConnection CrearConexion()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
