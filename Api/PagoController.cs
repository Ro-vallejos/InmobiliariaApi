using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using inmobiliariaApi.Repositorios;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using inmobiliariaApi.Dtos;

namespace inmobiliariaApi.Controllers
{
    [Route("api/Pagos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PagoController : ControllerBase
    {
        private readonly IRepositorioContrato _repoContrato;
        private readonly IRepositorioPago _repoPago;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _config;

        public PagoController(IRepositorioContrato repoContrato, IConfiguration config, IWebHostEnvironment environment, IRepositorioPago repoPago)
        {
            _repoContrato = repoContrato;
            _config = config;
            _environment = environment;
            _repoPago = repoPago;
        }

        [HttpGet("contrato/{id:int}")]
        public IActionResult ObtenerPorContrato(int id)
        {
            var idProp = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (idProp <= 0) return Unauthorized("Token inválido.");

            var pagos = _repoPago.ObtenerPagosPorContrato(id).ToList();
            
            return Ok(pagos);
        }
    
    }
}