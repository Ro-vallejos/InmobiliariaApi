using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using inmobiliariaApi.Models;
using inmobiliariaApi.Repositorios;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MimeKit;
using MailKit.Net.Smtp;

namespace inmobiliariaApi.Controllers
{
    [Route("api/Propietarios")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PropietariosController : ControllerBase
    {
        private readonly IRepositorioPropietario _repo;
        private readonly IConfiguration _config;

        public PropietariosController(IRepositorioPropietario repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginView loginView)
        {
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: loginView.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(_config["Salt"] ?? ""),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));

                var p = _repo.ObtenerPorEmail(loginView.Usuario);
                if (p == null || p.clave != hashed)
                {
                    return BadRequest("Nombre de usuario o clave incorrecta");
                }

                var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(_config["TokenAuthentication:SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, p.id.ToString()),
                    new Claim("FullName", p.nombre + " " + p.apellido),
                    new Claim(ClaimTypes.Role, "Propietario")
                };

                var token = new JwtSecurityToken(
                    issuer: _config["TokenAuthentication:Issuer"],
                    audience: _config["TokenAuthentication:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: creds
                );

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult ObtenerPerfil()
        {
            try
            {
                var idToken = int.Parse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

               if (idToken <= 0) 
                    return Unauthorized("Token inválido o expirado");

                var propietario = _repo.ObtenerPropietarioId(idToken);

                if (propietario == null)
                    return NotFound("Propietario no encontrado");

                propietario.clave = null;
                return Ok(propietario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("actualizar")]
        public IActionResult Actualizar([FromBody] Propietario p)
        {
            try
            {
                var idToken = int.Parse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (idToken <= 0)
                    return Unauthorized("Token inválido o expirado.");

                var actual = _repo.ObtenerPropietarioId(idToken);
                if (actual == null)
                    return NotFound("Propietario no encontrado.");

                if (!string.IsNullOrWhiteSpace(p.nombre))
                    actual.nombre = p.nombre.Trim().ToUpper();

                if (!string.IsNullOrWhiteSpace(p.apellido))
                    actual.apellido = p.apellido.Trim().ToUpper();

                if (!string.IsNullOrWhiteSpace(p.telefono))
                    actual.telefono = p.telefono.Trim();

                if (!string.IsNullOrWhiteSpace(p.dni))
                    actual.dni = p.dni.Trim();

                if (!string.IsNullOrWhiteSpace(p.email))
                    actual.email = p.email.Trim().ToLower();


                _repo.ActualizarPropietario(actual);
                actual.clave = null;

                return Ok(actual);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public record ChangePasswordForm(string currentPassword, string newPassword);
        [HttpPut("changePassword")]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult ChangePassword([FromForm] ChangePasswordForm form)
        {
            if (string.IsNullOrWhiteSpace(form.currentPassword) || string.IsNullOrWhiteSpace(form.newPassword))
                return BadRequest("Debe ingresar la contraseña actual y la nueva contraseña.");

            if (form.newPassword.Trim() == form.currentPassword.Trim())
                return BadRequest("La nueva contraseña no puede ser igual a la actual.");

            int id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (id <= 0) return Unauthorized("Token inválido o expirado.");

            var p = _repo.ObtenerPropietarioId(id);
            if (p is null) return NotFound("Propietario no encontrado.");

            var salt = _config["Salt"] ?? "";
            string Hash(string plain) => Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: plain,
                salt: Encoding.ASCII.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8
            ));

            var actualHash = Hash(form.currentPassword.Trim());
            if (!string.Equals(p.clave, actualHash, StringComparison.Ordinal))
                return Unauthorized("La contraseña actual es incorrecta.");

            var nuevoHash = Hash(form.newPassword.Trim());
            _repo.ActualizarClave(id, nuevoHash);

            return NoContent();
        }
        [HttpPost("email")]
        [AllowAnonymous]
        public IActionResult EnviarEmailRecuperacion([FromForm] string email)
        {
            try
            {
                var propietario = _repo.ObtenerPorEmail(email);
                if (propietario == null)
                    return NotFound("No existe propietario con ese email.");

                var otp = new Random().Next(100000, 999999).ToString();
                var salt = _config["Salt"] ?? "";

                string hashOtp = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: otp,
                    salt: Encoding.ASCII.GetBytes(salt),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8
                ));

                _repo.GuardarPassRestore(propietario.id, hashOtp);

                var msg = new MimeKit.MimeMessage();
                msg.To.Add(new MailboxAddress($"{propietario.nombre}", propietario.email));
                msg.From.Add(new MailboxAddress("Soporte Inmobiliaria", _config["SMTPUser"]));
                msg.Subject = "Restablecer contraseña";
                msg.Body = new TextPart("html")
                {
                    Text = $"Hola {propietario.nombre}, tu código de recuperación es: {otp}"
                };

                using (var smtp = new SmtpClient())
                {
                    smtp.Connect("smtp.gmail.com", 465, true);
                    smtp.Authenticate(_config["SMTPUser"], _config["SMTPPass"]);
                    smtp.Send(msg);
                    smtp.Disconnect(true);
                }

                return Ok("Se envió un correo con instrucciones para restablecer la contraseña.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}