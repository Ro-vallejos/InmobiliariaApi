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
    [Route("api/Contratos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class ContratoController : ControllerBase
    {
        private readonly IRepositorioContrato _repoContrato;
        private readonly IRepositorioInmueble _repoInmueble;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _config;

        public ContratoController(IRepositorioContrato repoContrato, IConfiguration config, IWebHostEnvironment environment, IRepositorioInmueble repoInmueble)
        {
            _repoContrato = repoContrato;
            _config = config;
            _environment = environment;
            _repoInmueble = repoInmueble;
        }

        [HttpGet("{contratoId:int}/Inquilino")]
        public IActionResult ObtenerInquilino(int contratoId)
        {
            try
            {
                int idPropietario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (idPropietario <= 0)
                    return Unauthorized("Token inválido o expirado.");

                var inquilino = _repoContrato.ObtenerInquilinoDeContrato(contratoId, idPropietario);
                if (inquilino == null)
                    return NotFound("No se encontró un inquilino para este contrato.");

                return Ok(inquilino);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("inmueble/{id:int}")]
        public IActionResult ObtenerPorInmueble(int id)
        {
            var idProp = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (idProp <= 0) return Unauthorized("Token inválido o expirado.");

            var inmueble = _repoInmueble.ObtenerInmuebleId(id);
            if (inmueble == null || inmueble.idPropietario != idProp)
                return Forbid("No podés ver contratos de inmuebles que no son tuyos.");

            var dto = _repoContrato.ObtenerContratoVigentePorInmueble(id);
            if (dto == null) return NotFound("El inmueble no tiene contrato vigente.");
            return Ok(dto);
        }   
    }
}