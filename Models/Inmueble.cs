namespace Inmobiliaria_Zarate_DoNet.Models
{
    public enum UsoInmueble { RESIDENCIAL = 0, COMERCIAL = 1 }

    
    public enum EstadoInmueble { DISPONIBLE = 0, SUSPENDIDO = 1 }

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

        
        public EstadoInmueble Estado
        {
            get => Suspendido ? EstadoInmueble.SUSPENDIDO : EstadoInmueble.DISPONIBLE;
            set
            {
                Disponible = (value == EstadoInmueble.DISPONIBLE);
                Suspendido = (value == EstadoInmueble.SUSPENDIDO);
            }
        }

        public DateTime CreadoEn { get; set; }
        public string PropietarioNombreCompleto { get; set; } = "";
        public string TipoNombre { get; set; } = "";
    }
}
