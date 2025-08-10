using System.Net;
using System.Net.Mail;
using SorteoAPI.Models;

namespace SorteoAPI.Services
{
    /// <summary>
    /// Servicio simple para envío de correos por SMTP.
    /// Usa configuración de appsettings (Smtp:Host, Port, User, Pass, From, Ssl).
    /// Si no hay configuración, registra y omite el envío sin fallar la operación.
    /// </summary>
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        /// <summary>
        /// Constructor por inyección de dependencias.
        /// </summary>
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Envía un correo al usuario informando el cambio de estado de su inscripción.
        /// </summary>
        public async Task EnviarCambioEstadoAsync(Usuario usuario)
        {
            var host = _configuration["Smtp:Host"];
            var portStr = _configuration["Smtp:Port"];
            var user = _configuration["Smtp:User"];
            var pass = _configuration["Smtp:Pass"];
            var from = _configuration["Smtp:From"] ?? user;
            var enableSsl = bool.TryParse(_configuration["Smtp:Ssl"], out var ssl) ? ssl : true;

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
            {
                _logger.LogInformation("SMTP no configurado. Se omite el envío de correo.");
                return;
            }

            var port = 587;
            if (int.TryParse(portStr, out var parsed)) port = parsed;

            var subject = $"Estado de tu inscripción: {usuario.Estado}";
            var body = $@"Hola {usuario.NombresApellidos},\n\n" +
                       $"Tu solicitud de inscripción ha sido '{usuario.Estado}'.\n" +
                       (string.IsNullOrWhiteSpace(usuario.ComentarioAdmin) ? "" : $"Comentario del administrador: {usuario.ComentarioAdmin}\n") +
                       "\nGracias por participar.";

            using var message = new MailMessage(from!, usuario.CorreoElectronico)
            {
                Subject = subject,
                Body = body
            };

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = string.IsNullOrEmpty(user) ? CredentialCache.DefaultNetworkCredentials : new NetworkCredential(user, pass)
            };

            try
            {
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando correo de cambio de estado");
            }
        }

        /// <summary>
        /// Envía un correo genérico a un destinatario específico. Útil para pruebas.
        /// </summary>
        public async Task EnviarAsync(string destinatario, string asunto, string cuerpo)
        {
            var host = _configuration["Smtp:Host"];
            var portStr = _configuration["Smtp:Port"];
            var user = _configuration["Smtp:User"];
            var pass = _configuration["Smtp:Pass"];
            var from = _configuration["Smtp:From"] ?? user;
            var enableSsl = bool.TryParse(_configuration["Smtp:Ssl"], out var ssl) ? ssl : true;

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
            {
                _logger.LogInformation("SMTP no configurado. Se omite el envío de correo (EnviarAsync).");
                return;
            }

            var port = 587;
            if (int.TryParse(portStr, out var parsed)) port = parsed;

            using var message = new MailMessage(from!, destinatario)
            {
                Subject = string.IsNullOrWhiteSpace(asunto) ? "Prueba de SMTP" : asunto,
                Body = string.IsNullOrWhiteSpace(cuerpo) ? "Correo de prueba desde SorteoAPI" : cuerpo
            };

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = string.IsNullOrEmpty(user) ? CredentialCache.DefaultNetworkCredentials : new NetworkCredential(user, pass)
            };

            try
            {
                await client.SendMailAsync(message);
                _logger.LogInformation("Correo de prueba enviado a {dest}", destinatario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando correo de prueba a {dest}", destinatario);
                throw;
            }
        }
    }
}
