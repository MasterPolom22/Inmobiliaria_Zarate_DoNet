using MySql.Data.MySqlClient;

namespace Inmobiliaria_Zarate_DoNet.Data
{
    public class DbConexion
{
    private readonly string _connectionString = string.Empty;

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
