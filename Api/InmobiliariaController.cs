using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using inmobiliariaApi.Repositorios;
using System.Security.Claims;
using inmobiliariaApi.Models;
using inmobiliariaApi.Dtos;


namespace inmobiliariaApi.Controllers
{
    [Route("api/Inmuebles")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class InmobiliariaController : ControllerBase
    {
        private readonly IRepositorioInmueble _repoInmueble;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _config;

        public InmobiliariaController(IRepositorioInmueble repoInmueble, IConfiguration config, IWebHostEnvironment environment)
        {
            _repoInmueble = repoInmueble;
            _config = config;
            _environment = environment;
        }



        [HttpGet]
        public IActionResult obtenerInmuebles()
        {
            try
            {
                int idPropietario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (idPropietario <= 0)
                    return Unauthorized("Token inválido o expirado");
                var lista = _repoInmueble.ObtenerInmueblesPorPropietarioDto(idPropietario);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("cargar")]
        [Consumes("Multipart/form-data")]
        public IActionResult Agregar([FromForm] IFormFile imagen, [FromForm] string inmueble)
        {
            try
            {
                int idPropietario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (idPropietario <= 0)
                    return Unauthorized("Token inválido o expirado");

                if (imagen == null || string.IsNullOrEmpty(inmueble))
                    return BadRequest("Debe enviar la imagen y los datos del inmueble.");

                var inmuebleNuevo = System.Text.Json.JsonSerializer.Deserialize<InmuebleDto>(inmueble);
                if (inmuebleNuevo == null)
                    return BadRequest("Datos del inmueble inválidos.");
                string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
                string rutaCarpeta = Path.Combine(_environment.WebRootPath, "Uploads", "Inmuebles");

                if (!Directory.Exists(rutaCarpeta))
                    Directory.CreateDirectory(rutaCarpeta);
                string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    imagen.CopyTo(stream);
                }

                inmuebleNuevo.imagen = Path.Combine("/Uploads/Inmuebles/", nombreArchivo).Replace("\\", "/");
                inmuebleNuevo.idPropietario = idPropietario;

                _repoInmueble.AgregarInmueble(inmuebleNuevo);
                return Ok(inmuebleNuevo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("actualizar")]
        public IActionResult Actualizar([FromBody] InmuebleDto inmueble)
        {
            try
            {
                int idPropietario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (idPropietario <= 0) return Unauthorized("Token inválido o expirado.");

                if (inmueble == null || inmueble.id <= 0)
                    return BadRequest("Faltan datos del inmueble.");


                var actual = _repoInmueble.ObtenerInmuebleId(inmueble.id);
                if (actual == null)
                    return NotFound("Inmueble no encontrado.");

                if (inmueble.disponible)
                    _repoInmueble.ActualizarEstado(inmueble.id, 1);
                else
                    _repoInmueble.ActualizarEstado(inmueble.id, 2);

                return Ok(actual);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetContratoVigente")]
        public ActionResult<IEnumerable<InmuebleDto>> GetContratoVigente()
        {
            try
            {
                int idPropietario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (idPropietario <= 0)
                    return Unauthorized("Token inválido o expirado");

                var lista = _repoInmueble.ObtenerConContratoVigente(idPropietario);
                if(lista == null)
                    return NotFound("No hay inmuebles con contrato vigente.");
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        

    }
}