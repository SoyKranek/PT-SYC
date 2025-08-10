using Microsoft.EntityFrameworkCore;
using SorteoAPI.Data;
using SorteoAPI.DTOs;
using SorteoAPI.Models;

namespace SorteoAPI.Services
{
    /// <summary>
    /// Lógica de negocio para las inscripciones de usuarios.
    /// Gestiona listado, detalle, cambios de estado, validación de edad y manejo de documentos.
    /// </summary>
    public class UsuarioService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly EmailService _emailService;

        /// <summary>
        /// Constructor por inyección de dependencias.
        /// </summary>
        public UsuarioService(ApplicationDbContext context, IWebHostEnvironment environment, EmailService emailService)
        {
            _context = context;
            _environment = environment;
            _emailService = emailService;
        }

        /// <summary>
        /// Devuelve la lista para el panel del administrador, ordenada por fecha de solicitud descendente.
        /// </summary>
        public async Task<List<UsuarioListaDTO>> ObtenerListaUsuariosAsync()
        {
            return await _context.Usuarios
                .OrderByDescending(u => u.FechaSolicitud)
                .Select(u => new UsuarioListaDTO
                {
                    Id = u.Id,
                    NombresApellidos = u.NombresApellidos,
                    FechaSolicitud = u.FechaSolicitud,
                    Estado = u.Estado
                })
                .ToListAsync();
        }

        /// <summary>
        /// Devuelve el detalle completo de una inscripción por Id.
        /// </summary>
        public async Task<UsuarioDetalleDTO?> ObtenerUsuarioDetalleAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return null;

            return new UsuarioDetalleDTO
            {
                Id = usuario.Id,
                TipoDocumento = usuario.TipoDocumento,
                NumeroDocumento = usuario.NumeroDocumento,
                NombresApellidos = usuario.NombresApellidos,
                FechaNacimiento = usuario.FechaNacimiento,
                Direccion = usuario.Direccion,
                Telefono = usuario.Telefono,
                CorreoElectronico = usuario.CorreoElectronico,
                RutaDocumento = usuario.RutaDocumento,
                FechaSolicitud = usuario.FechaSolicitud,
                Estado = usuario.Estado,
                ComentarioAdmin = usuario.ComentarioAdmin,
                FechaRevision = usuario.FechaRevision
            };
        }

        /// <summary>
        /// Cambia el estado de la inscripción y envía notificación por correo si está configurado el SMTP.
        /// </summary>
        public async Task<bool> CambiarEstadoUsuarioAsync(CambioEstadoDTO cambioEstadoDto)
        {
            var usuario = await _context.Usuarios.FindAsync(cambioEstadoDto.UsuarioId);
            if (usuario == null) return false;

            usuario.Estado = cambioEstadoDto.Estado;
            usuario.ComentarioAdmin = cambioEstadoDto.ComentarioAdmin;
            usuario.FechaRevision = DateTime.Now;

            await _context.SaveChangesAsync();

            // Enviar correo (si está configurado)
            await _emailService.EnviarCambioEstadoAsync(usuario);

            return true;
        }

        /// <summary>
        /// Valida y almacena el archivo en wwwroot/uploads con un nombre único.
        /// </summary>
        public async Task<string> GuardarDocumentoAsync(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                throw new ArgumentException("Archivo no válido");

            // Validar tipo de archivo
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            
            if (!extensionesPermitidas.Contains(extension))
                throw new ArgumentException("Tipo de archivo no permitido. Solo se permiten JPG, PNG y PDF");

            // Crear directorio si no existe
            var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            // Generar nombre único para el archivo
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var rutaCompleta = Path.Combine(uploadsDir, nombreArchivo);

            // Guardar archivo
            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            return nombreArchivo;
        }

        /// <summary>
        /// Verifica si la persona tiene 18 años o más.
        /// </summary>
        public async Task<bool> ValidarEdadAsync(DateTime fechaNacimiento)
        {
            var edad = DateTime.Today.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > DateTime.Today.AddYears(-edad)) edad--;
            return edad >= 18;
        }
    }
}
