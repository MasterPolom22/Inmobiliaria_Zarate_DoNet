namespace Inmobiliaria_Zarate_DoNet.Models
{
    public enum EstadoContrato { VIGENTE, FINALIZADO, RESCINDIDO }

    public class Contrato
    {
        public int Id { get; set; }
        public int InmuebleId { get; set; }
        public int InquilinoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinOriginal { get; set; }
        public decimal MontoMensual { get; set; }
        public DateTime? FechaFinAnticipada { get; set; }
        public int? ContratoOrigenId { get; set; }
        public EstadoContrato Estado { get; set; } = EstadoContrato.VIGENTE;
        public int CreadoPor { get; set; }   // placeholder (ej: 1)
        public int? TerminadoPor { get; set; }
        public DateTime CreadoEn { get; set; }

        // Lectura/Joined
        public string InmuebleDireccion { get; set; } = "";
        public string InquilinoNombre { get; set; } = "";
    }
}
