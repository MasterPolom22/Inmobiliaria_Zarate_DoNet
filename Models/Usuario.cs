using System;

namespace Inmobiliaria_Zarate_DoNet.Models
{
    public enum Rol { ADMIN, EMPLEADO } // coincide con enum de la BD

    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = ""; // nunca guardes el password plano
        public Rol Rol { get; set; } = Rol.EMPLEADO;
        public bool Activo { get; set; } = true;
        public DateTime CreadoEn { get; set; }
    }
}
