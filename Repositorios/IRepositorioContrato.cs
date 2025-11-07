using inmobiliariaApi.Dtos;
using inmobiliariaApi.Models;
using System.Collections.Generic;

namespace inmobiliariaApi.Repositorios
{
    public interface IRepositorioContrato
    {
        InquilinoDto? ObtenerInquilinoDeContrato(int contratoId, int idPropietario);
        ContratoDto? ObtenerContratoVigentePorInmueble(int idInmueble);

    }
}
