using Microsoft.AspNetCore.Mvc;
using SorteoAPI.DTOs;
using SorteoAPI.Services;

namespace SorteoAPI.Controllers
{
    /// <summary>
    /// Endpoints de autenticación del administrador.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        /// <summary>
        /// Constructor por inyección de dependencias.
        /// </summary>
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Recibe usuario/contraseña y devuelve token JWT si las credenciales son válidas.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = await _authService.LoginAsync(loginDto);
            
            if (resultado == null)
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
            }

            return Ok(resultado);
        }
    }
}
