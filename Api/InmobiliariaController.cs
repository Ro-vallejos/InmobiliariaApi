using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using inmobiliariaApi.Repositorios;
using System.Security.Claims;
using inmobiliariaApi.Models;
using Microsoft.EntityFrameworkCore;


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
        private readonly DataContext context;

        public InmobiliariaController(IRepositorioInmueble repoInmueble, IConfiguration config, IWebHostEnvironment environment, DataContext context)
        {
            _repoInmueble = repoInmueble;
            _config = config;
            _environment = environment;
            this.context = context;
        }



        [HttpGet]
        public async Task< IActionResult> obtenerInmuebles()
        {
            try
            {
                int idPropietario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (idPropietario <= 0)
                    return Unauthorized("Token inválido o expirado");
               // var lista = _repoInmueble.ObtenerInmueblesPorPropietarioDto(idPropietario);
            var lista = await context.inmueble.AsNoTracking().Where(i => i.idPropietario == idPropietario).ToListAsync();

                if (!lista.Any())
                {
                    return NotFound("No se encontraron inmuebles.");
                }
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("cargar")]
        [Consumes("Multipart/form-data")]
        public async Task<IActionResult> Agregar([FromForm] IFormFile imagen, [FromForm] string inmueble)
        {
            try
            {
                int idPropietario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (idPropietario <= 0)
                    return Unauthorized("Token inválido o expirado");

                if (imagen == null || string.IsNullOrEmpty(inmueble))
                    return BadRequest("Debe enviar la imagen y los datos del inmueble.");

                var inmuebleNuevo = System.Text.Json.JsonSerializer.Deserialize<Inmueble>(inmueble);
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

                //_repoInmueble.AgregarInmueble(inmuebleNuevo);
                 context.inmueble.Add(inmuebleNuevo);
                await context.SaveChangesAsync();
                return Ok(inmuebleNuevo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("actualizar")]
        public async Task<IActionResult> Actualizar([FromBody] Inmueble inmueble)
        {
            try
            {
                int idPropietario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (idPropietario <= 0) return Unauthorized("Token inválido o expirado.");

                if (inmueble == null || inmueble.id <= 0)
                    return BadRequest("Faltan datos del inmueble.");

                var actual = await context.inmueble.FirstOrDefaultAsync(i => i.id == inmueble.id);

                if (actual == null)
                    return NotFound("Inmueble no encontrado.");
                // _repoInmueble.ActualizarEstado(inmueble.id, inmueble.estado);
                actual.estado = inmueble.estado;
                await context.SaveChangesAsync();

                return Ok(actual);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetContratoVigente")]
        public async Task<ActionResult<List<Inmueble>>> GetContratoVigente()
        {
            try
            {
                int idPropietario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (idPropietario <= 0)
                    return Unauthorized("Token inválido o expirado");

                //var lista = _repoInmueble.ObtenerConContratoVigente(idPropietario);
               var lista = await context.inmueble.AsNoTracking().Join(context.contrato,
                i => i.id,
                c => c.idInmueble,
                (i, c) => new { i, c })
                    .Where(x => x.c.estado == 1 && x.i.idPropietario == idPropietario)
                    .Select(x => x.i)
                    .Distinct()
                    .ToListAsync();
                if(!lista.Any())
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