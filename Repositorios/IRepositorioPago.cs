using inmobiliariaApi.Models;
using System.Collections.Generic;
using inmobiliariaApi.Dtos;

namespace inmobiliariaApi.Repositorios;

public interface IRepositorioPago
{
    List<PagoDto> ObtenerPagosPorContrato(int contratoId);
    
}