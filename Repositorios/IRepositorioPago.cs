using inmobiliariaApi.Models;
using System.Collections.Generic;

namespace inmobiliariaApi.Repositorios;

public interface IRepositorioPago
{
    List<Pago> ObtenerPagosPorContrato(int contratoId);
    
}