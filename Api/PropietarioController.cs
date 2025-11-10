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
using Microsoft.EntityFrameworkCore;

namespace inmobiliariaApi.Controllers
{
    [Route("api/Propietarios")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PropietariosController : ControllerBase
    {
        private readonly IRepositorioPropietario _repo;
        private readonly IConfiguration _config;
        private readonly DataContext _context;
        public PropietariosController(IRepositorioPropietario repo, IConfiguration config, DataContext context)
        {
            _config = config;
            _repo = repo;
            _context = context;
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

               // var p = _repo.ObtenerPorEmail(loginView.Usuario);
               var p = await _context.propietario.AsNoTracking().FirstOrDefaultAsync(p => p.email == loginView.Usuario); 
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
        public async Task<IActionResult> ObtenerPerfil()
        {
            try
            {
                var idToken = int.Parse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

               if (idToken <= 0) 
                    return Unauthorized("Token inválido o expirado");

                //var propietario = _repo.ObtenerPropietarioId(idToken);
               var propietario = await _context.propietario.AsNoTracking().FirstOrDefaultAsync(p => p.id == idToken); 

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
        public async Task <IActionResult> Actualizar([FromBody] Propietario p)
        {
            try
            {
                var idToken = int.Parse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (idToken <= 0)
                    return Unauthorized("Token inválido o expirado.");

               // var actual = _repo.ObtenerPropietarioId(idToken);
               var actual = await _context.propietario.FirstOrDefaultAsync(p => p.id == idToken); 

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

                var emailExiste = await _context.propietario.AnyAsync(x => x.email == p.email && x.id != idToken);

                if (emailExiste)
                    return BadRequest("Ese email ya está en uso por otro propietario.");

              
                await _context.SaveChangesAsync();
               // _repo.ActualizarPropietario(actual);
               

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
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordForm form)
        {
            if (string.IsNullOrWhiteSpace(form.currentPassword) || string.IsNullOrWhiteSpace(form.newPassword))
                return BadRequest("Debe ingresar la contraseña actual y la nueva contraseña.");

            if (form.newPassword.Trim() == form.currentPassword.Trim())
                return BadRequest("La nueva contraseña no puede ser igual a la actual.");

            int id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (id <= 0) return Unauthorized("Token inválido o expirado.");

            //var p = _repo.ObtenerPropietarioId(id);
            var p = await _context.propietario.FirstOrDefaultAsync(p => p.id == id); 

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


            p.clave = Hash(form.newPassword.Trim());
            //_repo.ActualizarClave(id, nuevoHash);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}