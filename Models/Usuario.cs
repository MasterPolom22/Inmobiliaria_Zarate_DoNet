using System;

namespace Inmobiliaria_Zarate_DoNet.Models
{
    public enum Rol { ADMIN, EMPLEADO }

    public class Usuario
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string Email { get; set; } = "";

        // Hash de contraseña (BCrypt)
        public string PasswordHash { get; set; } = "";

        public Rol Rol { get; set; } = Rol.EMPLEADO;
        public bool Activo { get; set; } = true;

        // Auditoría
        public DateTime CreadoEn { get; set; }
    }
}
