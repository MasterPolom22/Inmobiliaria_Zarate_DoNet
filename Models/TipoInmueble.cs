namespace Inmobiliaria_Zarate_DoNet.Models
{
    public class TipoInmueble
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public bool Activo { get; set; }
        public DateTime CreadoEn { get; set; }
    }
}
