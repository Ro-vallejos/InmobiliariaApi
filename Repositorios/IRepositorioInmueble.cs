using System.Collections.Generic;

namespace inmobiliariaApi.Repositorios
{
    public interface IRepositorioInmueble
    {
        Inmueble? ObtenerInmuebleId(int id);
        void AgregarInmueble(Inmueble inmuebleNuevo);
        List<Inmueble> ObtenerInmueblesPorPropietarioDto(int propietarioId);
        void ActualizarEstado(int id, int activo);
        List<Inmueble> ObtenerConContratoVigente(int propietarioId);
    }
}
