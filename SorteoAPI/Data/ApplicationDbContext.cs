using Microsoft.EntityFrameworkCore;
using SorteoAPI.Models;

namespace SorteoAPI.Data
{
    /// <summary>
    /// DbContext de Entity Framework Core.
    /// Define tablas, restricciones e inserta el administrador por defecto.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Administrador> Administradores { get; set; }

        /// <summary>
        /// Configura el modelo: restricciones, longitudes, índices e inserción de datos semilla.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TipoDocumento).IsRequired().HasMaxLength(50);
                entity.Property(e => e.NumeroDocumento).IsRequired().HasMaxLength(20);
                entity.Property(e => e.NombresApellidos).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Direccion).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Telefono).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CorreoElectronico).IsRequired().HasMaxLength(100);
                entity.Property(e => e.RutaDocumento).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(20);
                entity.Property(e => e.ComentarioAdmin).HasMaxLength(1000);
                
                // Índice único para documento
                entity.HasIndex(e => new { e.TipoDocumento, e.NumeroDocumento }).IsUnique();
                
                // Índice para correo electrónico
                entity.HasIndex(e => e.CorreoElectronico).IsUnique();
            });

            // Configuración para Administrador
            modelBuilder.Entity<Administrador>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Usuario).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                
                // Índice único para usuario
                entity.HasIndex(e => e.Usuario).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Datos semilla para administrador por defecto
            modelBuilder.Entity<Administrador>().HasData(
                new Administrador
                {
                    Id = 1,
                    Usuario = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Nombre = "Administrador",
                    Email = "admin@sorteo.com",
                    FechaCreacion = DateTime.Now,
                    Activo = true
                }
            );
        }
    }
}
