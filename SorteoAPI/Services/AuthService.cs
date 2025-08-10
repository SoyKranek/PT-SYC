using Microsoft.IdentityModel.Tokens;
using SorteoAPI.Data;
using SorteoAPI.DTOs;
using SorteoAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SorteoAPI.Services
{
    /// <summary>
    /// Servicio de autenticación del administrador.
    /// Encargado de validar credenciales y generar el token JWT con los claims necesarios.
    /// </summary>
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor por inyección de dependencias.
        /// </summary>
        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Valida las credenciales del administrador y devuelve la información de sesión con el token.
        /// </summary>
        public async Task<LoginResponseDTO?> LoginAsync(LoginDTO loginDto)
        {
            var admin = await _context.Administradores
                .FirstOrDefaultAsync(a => a.Usuario == loginDto.Usuario && a.Activo);

            if (admin == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, admin.PasswordHash))
            {
                return null;
            }

            var token = GenerateJwtToken(admin);
            var expiracion = DateTime.UtcNow.AddHours(8); // Token válido por 8 horas

            return new LoginResponseDTO
            {
                Token = token,
                Usuario = admin.Usuario,
                Nombre = admin.Nombre,
                Expiracion = expiracion
            };
        }

        /// <summary>
        /// Genera y firma el token JWT con claims básicos: Id, Usuario, Rol y Nombre.
        /// </summary>
        private string GenerateJwtToken(Administrador admin)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "DefaultKey123456789"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Name, admin.Usuario),
                new Claim(ClaimTypes.Role, "Administrador"),
                new Claim("Nombre", admin.Nombre)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
