using System;

namespace Inmobiliaria_Zarate_DoNet.Models
{
    public class Inquilino
    {
        public int Id { get; set; }
        public string Dni { get; set; } = "";
        // En la BD está "Apellido" con A mayúscula; en C# usamos "Apellido" también.
        public string Apellido { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public DateTime CreadoEn { get; set; }
    }
}
