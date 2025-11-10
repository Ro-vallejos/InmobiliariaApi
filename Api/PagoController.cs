using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using inmobiliariaApi.Repositorios;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace inmobiliariaApi.Controllers
{
    [Route("api/Pagos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PagoController : ControllerBase
    {
      //  private readonly IRepositorioPago _repoPago;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _config;
        private readonly DataContext _context;

        public PagoController( IConfiguration config, IWebHostEnvironment environment, DataContext context)
        {
            _config = config;
            _environment = environment;
            //_repoPago = repoPago;
            _context = context;
        }

        [HttpGet("contrato/{id:int}")]
        public async Task<IActionResult> ObtenerPorContrato(int id)
        {
            var idProp = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (idProp <= 0) return Unauthorized("Token inválido.");

            //var pagos = _repoPago.ObtenerPagosPorContrato(id);
            var pagos = await _context.pago.AsNoTracking().Where(p => p.idContrato == id && p.estado == EstadoPago.recibido).OrderBy(p => p.nroPago).ToListAsync();

            if (pagos == null)
                return NotFound("No se encontraron pagos.");
            
            return Ok(pagos);
        }
    
    }
}