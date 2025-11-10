using inmobiliariaApi.Models;
using System.Collections.Generic;

namespace inmobiliariaApi.Repositorios
{
    public interface IRepositorioContrato
    {
        Contrato? ObtenerContratoVigentePorInmueble(int idInmueble);

    }
}
