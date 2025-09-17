namespace Inmobiliaria_Zarate_DoNet.Models
{
    public class Pago
    {
        public int Id { get; set; }
        public int ContratoId { get; set; }
        public int Numero { get; set; }          // ahora lo asignamos nosotros
        public DateTime Fecha { get; set; }
        public string Detalle { get; set; } = ""; // NOT NULL
        public decimal Importe { get; set; }      // CHECK (>=0) en BD
        public bool Anulado { get; set; }
        public int CreadoPor { get; set; }        // NOT NULL
        public int? AnuladoPor { get; set; }
        public DateTime CreadoEn { get; set; }

        // Solo lectura (JOIN)
        public string ContratoResumen { get; set; } = "";
    }
}
