using inmobiliariaApi.Models;

namespace inmobiliariaApi.Repositorios
{
    public interface IRepositorioPropietario
    {
        List<Propietario> ObtenerPropietarios();
         List<Propietario> ObtenerPropietariosActivos();
        Propietario ObtenerPropietarioId(int id);
        Propietario ObtenerPorEmail(string email);
        Propietario ActualizarPropietario(Propietario propietario);
        bool EliminarPropietario(int id);
        void ActivarPropietario(int id);

        void AgregarPropietario(Propietario propietario);
        bool ExisteDni(string dni, int? idExcluido = null);
        bool ExisteEmail(string email, int? idExcluido = null);
        void ActualizarClave(int id, string nuevoHash);
    }
}