using System.ComponentModel.DataAnnotations;

namespace SorteoAPI.Models
{
    /// <summary>
    /// Representa a un usuario administrador del sistema.
    /// </summary>
    public class Administrador
    {
        public int Id { get; set; }
        
        [Required]
        public string Usuario { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public string Nombre { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        public bool Activo { get; set; } = true;
    }
}
