using Microsoft.AspNetCore.Mvc;
using SorteoAPI.Data;
using SorteoAPI.DTOs;
using SorteoAPI.Models;
using SorteoAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace SorteoAPI.Controllers
{
    /// <summary>
    /// Endpoints para inscripciones y gestión por parte del administrador.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UsuarioService _usuarioService;
        private readonly EmailService _emailService;
        private readonly ILogger<UsuariosController> _logger;

        /// <summary>
        /// Constructor por inyección de dependencias.
        /// </summary>
        public UsuariosController(ApplicationDbContext context, UsuarioService usuarioService, EmailService emailService, ILogger<UsuariosController> logger)
        {
            _context = context;
            _usuarioService = usuarioService;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Inscripción pública. Recibe datos del formulario y un archivo (imagen/pdf) por multipart/form-data.
        /// </summary>
        [HttpPost("inscripcion")]
        [AllowAnonymous]
        public async Task<IActionResult> Inscripcion([FromForm] UsuarioInscripcionDTO inscripcionDto, IFormFile documento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar edad
            if (!await _usuarioService.ValidarEdadAsync(inscripcionDto.FechaNacimiento))
            {
                return BadRequest(new { message = "Debe ser mayor de edad para participar" });
            }

            // Validar que no exista un usuario con el mismo documento
            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.TipoDocumento == inscripcionDto.TipoDocumento && 
                                         u.NumeroDocumento == inscripcionDto.NumeroDocumento);
            
            if (usuarioExistente != null)
            {
                return BadRequest(new { message = "Ya existe una inscripción con este documento" });
            }

            // Validar que no exista un usuario con el mismo correo
            var correoExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CorreoElectronico == inscripcionDto.CorreoElectronico);
            
            if (correoExistente != null)
            {
                return BadRequest(new { message = "Ya existe una inscripción con este correo electrónico" });
            }

            try
            {
                // Guardar documento
                var nombreArchivo = await _usuarioService.GuardarDocumentoAsync(documento);

                // Crear usuario
                var usuario = new Usuario
                {
                    TipoDocumento = inscripcionDto.TipoDocumento,
                    NumeroDocumento = inscripcionDto.NumeroDocumento,
                    NombresApellidos = inscripcionDto.NombresApellidos,
                    FechaNacimiento = inscripcionDto.FechaNacimiento,
                    Direccion = inscripcionDto.Direccion,
                    Telefono = inscripcionDto.Telefono,
                    CorreoElectronico = inscripcionDto.CorreoElectronico,
                    RutaDocumento = nombreArchivo,
                    FechaSolicitud = DateTime.Now,
                    Estado = "Pendiente"
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Inscripción realizada exitosamente", id = usuario.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error al procesar la inscripción: {ex.Message}" });
            }
        }

        /// <summary>
        /// Lista de inscripciones para el panel del administrador.
        /// </summary>
        [HttpGet("lista")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ObtenerLista()
        {
            var usuarios = await _usuarioService.ObtenerListaUsuariosAsync();
            return Ok(usuarios);
        }

        /// <summary>
        /// Detalle de una inscripción específica.
        /// </summary>
        [HttpGet("detalle/{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ObtenerDetalle(int id)
        {
            var usuario = await _usuarioService.ObtenerUsuarioDetalleAsync(id);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(usuario);
        }

        /// <summary>
        /// Cambia el estado de una inscripción a Aceptada/Rechazada e incluye comentario opcional.
        /// </summary>
        [HttpPut("cambiar-estado")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CambiarEstado([FromBody] CambioEstadoDTO cambioEstadoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (cambioEstadoDto.Estado != "Aceptada" && cambioEstadoDto.Estado != "Rechazada")
            {
                return BadRequest(new { message = "El estado debe ser 'Aceptada' o 'Rechazada'" });
            }

            var resultado = await _usuarioService.CambiarEstadoUsuarioAsync(cambioEstadoDto);
            if (!resultado)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(new { message = $"Estado cambiado a {cambioEstadoDto.Estado} exitosamente" });
        }

        /// <summary>
        /// Devuelve el archivo adjunto (imagen/pdf) asociado a una inscripción.
        /// </summary>
        [HttpGet("documento/{nombreArchivo}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult ObtenerDocumento(string nombreArchivo)
        {
            var rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", nombreArchivo);
            
            if (!System.IO.File.Exists(rutaArchivo))
            {
                return NotFound(new { message = "Documento no encontrado" });
            }

            var extension = Path.GetExtension(nombreArchivo).ToLowerInvariant();
            string contentType = extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            var bytes = System.IO.File.ReadAllBytes(rutaArchivo);
            return File(bytes, contentType, nombreArchivo);
        }

        /// <summary>
        /// Endpoint de prueba para SMTP. Envía un correo a la dirección indicada.
        /// </summary>
        [HttpPost("test-email")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> TestEmail([FromQuery] string to)
        {
            if (string.IsNullOrWhiteSpace(to)) return BadRequest(new { message = "Parámetro 'to' requerido" });
            try
            {
                await _emailService.EnviarAsync(to, "Prueba SMTP", "Este es un correo de prueba desde SorteoAPI");
                return Ok(new { message = $"Correo de prueba enviado a {to}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo envío de prueba SMTP a {to}", to);
                return StatusCode(500, new { message = "Error enviando correo", detail = ex.Message });
            }
        }
    }
}
