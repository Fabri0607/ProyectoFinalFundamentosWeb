using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging; // Add logging

namespace BackEnd.Helpers
{
    public class CorreoHelper
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CorreoHelper> _logger;

        public CorreoHelper(IConfiguration configuration, ILogger<CorreoHelper> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void EnviarCodigoPorCorreo(string destinatario, string codigo)
        {
            string remitente = _configuration["EmailSettings:SenderEmail"];
            string contrasena = _configuration["EmailSettings:SenderPassword"];

            if (string.IsNullOrEmpty(remitente) || string.IsNullOrEmpty(contrasena))
            {
                _logger.LogError("Configuración de correo no encontrada.");
                throw new InvalidOperationException("Configuración de correo no encontrada.");
            }

            try
            {
                _logger.LogInformation("Enviando correo a {Destinatario} con código {Codigo}", destinatario, codigo);

                MailMessage mensaje = new MailMessage
                {
                    From = new MailAddress(remitente, "Sistema de Inventario y ventas"),
                    Subject = "Tu contraseña temporal",
                    IsBodyHtml = true,
                    Body = $@"
                    <html>
                        <head>
                            <style>
                                .container {{
                                    font-family: Arial, sans-serif;
                                    max-width: 500px;
                                    margin: 0 auto;
                                    padding: 20px;
                                    background-color: #f0f4ff;
                                    border-radius: 12px;
                                    border: 1px solid #dbeafe;
                                }}
                                .header {{
                                    text-align: center;
                                    margin-bottom: 20px;
                                }}
                                .lock-icon {{
                                    font-size: 48px;
                                    color: #2563eb;
                                }}
                                .title {{
                                    font-size: 24px;
                                    font-weight: bold;
                                    color: #1f2937;
                                }}
                                .subtitle {{
                                    font-size: 16px;
                                    color: #4b5563;
                                    margin-bottom: 20px;
                                }}
                                .code {{
                                    display: block;
                                    background-color: #2563eb;
                                    color: white;
                                    padding: 15px;
                                    font-size: 24px;
                                    text-align: center;
                                    border-radius: 8px;
                                    letter-spacing: 3px;
                                    margin-bottom: 20px;
                                    font-weight: bold;
                                }}
                                .footer {{
                                    text-align: center;
                                    font-size: 12px;
                                    color: #9ca3af;
                                }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    <div class='lock-icon'>🔐</div>
                                    <div class='title'>Contraseña temporal</div>
                                    <div class='subtitle'>Usa la siguiente contraseña temporal para continuar con tu sesión</div>
                                </div>
                                <div class='code'>{codigo}</div>
                                <div class='footer'>
                                    Si no solicitaste esta contraseña, puedes ignorar este mensaje.
                                </div>
                            </div>
                        </body>
                    </html>"
                };
                mensaje.To.Add(destinatario);

                using SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(remitente, contrasena),
                    EnableSsl = true
                };

                smtp.Send(mensaje);
                _logger.LogInformation("Correo enviado con éxito a {Destinatario}.", destinatario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar el correo a {Destinatario}.", destinatario);
                throw; // Rethrow for debugging; remove in production if you want to handle silently
            }
        }
    }
}