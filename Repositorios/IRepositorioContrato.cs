using inmobiliariaApi.Dtos;
using inmobiliariaApi.Models;
using System.Collections.Generic;

namespace inmobiliariaApi.Repositorios
{
    public interface IRepositorioContrato
    {
        ContratoDto? ObtenerContratoVigentePorInmueble(int idInmueble);

    }
}
