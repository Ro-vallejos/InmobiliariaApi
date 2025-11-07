using inmobiliariaApi.Dtos;
using System.Collections.Generic;

namespace inmobiliariaApi.Repositorios
{
    public interface IRepositorioInmueble
    {
        InmuebleDto? ObtenerInmuebleId(int id);
        void AgregarInmueble(InmuebleDto inmuebleNuevo);
        List<InmuebleDto> ObtenerInmueblesPorPropietarioDto(int propietarioId);
        void ActualizarEstado(int id, int activo);
        List<InmuebleDto> ObtenerConContratoVigente(int propietarioId);
    }
}
