using System;

namespace Inmobiliaria_Zarate_DoNet.Models
{
    public class Pago
    {
        public int Id { get; set; }
        public int ContratoId { get; set; }
        public int Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Detalle { get; set; } = "";
        public decimal Importe { get; set; }

        public bool Anulado { get; set; }
        public int CreadoPorId { get; set; }
        public int? AnuladoPorId { get; set; }
        public DateTime CreadoEn { get; set; }

      
        public string? CreadoPorNombre { get; set; }
        public string? AnuladoPorNombre { get; set; }
    }
}
