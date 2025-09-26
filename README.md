Inmobiliaria Zarate – Proyecto .NET

 Descripción
Sistema de gestión inmobiliaria desarrollado en ASP.NET  MVC con ADO.NET + MySQL.  
Incluye ABMs completos, control de roles (ADMIN/EMPLEADO), auditoría de contratos y pagos, y módulo de perfil de usuario con avatar y cambio de contraseña.

 Integrante
- Zarate Lucas(unico integrante, pregunte para hacer equipo pero no consegui)

Funcionalidades principales
  ABM de:
  - Propietarios
  - Inquilinos
  - Inmuebles
  - Tipos de Inmuebles
  - Contratos
  - Pagos
  - Usuarios (solo administradores)
- Roles:
  - ADMIN : gestiona todo el sistema y usuarios.
  - EMPLEADO: acceso restringido, no puede eliminar ni gestionar usuarios.
- Perfil de usuario:
  - Editar datos personales.
  - Subir/eliminar avatar(imagen).
  - Cambiar contraseña.
  -si se desactiva un usuario no puede ingresar al sistema, no se borra solo se restringe la entrada, si se habilita puede volver a ingresar(esto lo hace el admin).

- Listados especiales de Inmuebles(modulo incompleto):
  - Listar los inmuebles que estén disponibles (estado disponible, no por fechas)-Completo!.
   -Listar los inmuebles que no estén ocupados en algún contrato entre dos fechas dadas (inicio y fin)(NO ENTIENDO LA LOGICA-INCOMPLETO).

- Contratos(este modulo esta incompleto):
  - Validación de superposición de fechas(no terminado).
  - Auditoría: quién creó y quién terminó el contrato solo el admin lo ve.

- Pagos:
  - Auditoría. Registrar qué usuario creó un contrato y, en caso que corresponda, quien lo terminó. Similar para pagos, quién lo creó y, en caso que corresponda, quien lo  anuló. Esta información de auditoría sólo es visible para administradores y en una vista de detalles de la entidad correspondiente.
- UI: vistas  con Bootstrap, mensajes  y alertas de confirmación.
 
-Base de datos: `inmobiliariadb`


 Usuarios de prueba

Administrador
Email: admin@gmail.com
Contraseña: 123

Administrador1
Email: admin1@gmail.com
Contraseña: 123

Empleado
Email: empleado1@gmail.com
Contraseña: 123
