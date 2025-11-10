using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using inmobiliariaApi.Repositorios;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace inmobiliariaApi.Controllers
{
    [Route("api/Contratos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class ContratoController : ControllerBase
    {
        private readonly DataContext context; 
        private readonly IConfiguration _config;
        //private readonly IRepositorioContrato _repoContrato;
        private readonly IRepositorioInmueble _repoInmueble;
        private readonly IWebHostEnvironment _environment;

        public ContratoController( IConfiguration config, IWebHostEnvironment environment, IRepositorioInmueble repoInmueble, DataContext context)
        {
           // _repoContrato = repoContrato;
            _config = config;
            _environment = environment;
            _repoInmueble = repoInmueble;
            this.context = context;
        }

        [HttpGet("inmueble/{id:int}")]
        public async Task<IActionResult> ObtenerPorInmueble(int id)
        {
            var idProp = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (idProp <= 0) return Unauthorized("Token inválido o expirado.");
            var today = DateTime.Today;
            // var inmueble = _repoInmueble.ObtenerInmuebleId(id);
            var inmueble = await context.inmueble.AsNoTracking().FirstOrDefaultAsync(i => i.id == id && i.idPropietario == idProp);
            if (inmueble == null) return NotFound("Inmueble no encontrado.");
            // var dto = _repoContrato.ObtenerContratoVigentePorInmueble(id);
            var contrato = await context.contrato
                 .AsNoTracking()
                 .Include(c => c.inquilino)
                 .Include(c => c.inmueble)
                 .Where(c =>
                     c.idInmueble == id &&
                     c.inmueble.idPropietario == idProp &&
                     c.estado == 1 &&
                     c.fechaInicio <= today &&
                     today <= c.fechaFin)
                 .OrderByDescending(c => c.fechaInicio)
                 .FirstOrDefaultAsync();
                
            if (contrato == null)
                return NotFound("El inmueble no tiene contrato vigente.");
            
            return Ok(contrato);
        }   
    }
}