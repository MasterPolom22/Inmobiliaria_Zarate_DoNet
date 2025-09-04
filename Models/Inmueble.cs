namespace Inmobiliaria_Zarate_DoNet.Models
{
    public enum UsoInmueble { RESIDENCIAL = 0, COMERCIAL = 1 }

    public class Inmueble
    {
        public int Id { get; set; }
        public int PropietarioId { get; set; }
        public int TipoId { get; set; }
        public UsoInmueble Uso { get; set; }
        public string Direccion { get; set; } = "";
        public int Ambientes { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public decimal PrecioBase { get; set; }
        public bool Disponible { get; set; }
        public bool Suspendido { get; set; }
        public DateTime CreadoEn { get; set; }

        // Campos “de lectura” para mostrar en la lista (JOINs)
        public string PropietarioNombreCompleto { get; set; } = "";
        public string TipoNombre { get; set; } = "";
    }
}
