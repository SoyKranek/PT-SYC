using System.ComponentModel.DataAnnotations;

namespace SorteoAPI.DTOs
{
    public class UsuarioInscripcionDTO
    {
        [Required(ErrorMessage = "El tipo de documento es obligatorio")]
        public string TipoDocumento { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El número de documento es obligatorio")]
        public string NumeroDocumento { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Los nombres y apellidos son obligatorios")]
        public string NombresApellidos { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
        
        [Required(ErrorMessage = "La dirección es obligatoria")]
        public string Direccion { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        public string Telefono { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
        public string CorreoElectronico { get; set; } = string.Empty;
    }

    public class UsuarioListaDTO
    {
        public int Id { get; set; }
        public string NombresApellidos { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class UsuarioDetalleDTO
    {
        public int Id { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public string NumeroDocumento { get; set; } = string.Empty;
        public string NombresApellidos { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string RutaDocumento { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? ComentarioAdmin { get; set; }
        public DateTime? FechaRevision { get; set; }
    }

    public class CambioEstadoDTO
    {
        [Required]
        public int UsuarioId { get; set; }
        
        [Required]
        public string Estado { get; set; } = string.Empty; // "Aceptada" o "Rechazada"
        
        public string? ComentarioAdmin { get; set; }
    }
}
