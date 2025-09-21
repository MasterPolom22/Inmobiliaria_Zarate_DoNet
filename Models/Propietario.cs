using System;

namespace Inmobiliaria_Zarate_DoNet.Models
{
    public class Propietario
    {
        public int Id { get; set; }
        public string Dni { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string? Telefono { get; set; }
        public string? Email { get; set; }

        // Auditoría
        public DateTime CreadoEn { get; set; }
    }
}
