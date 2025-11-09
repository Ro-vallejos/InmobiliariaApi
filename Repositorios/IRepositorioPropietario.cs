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
        void ActualizarClave(int id, string nuevoHash);
        void GuardarPassRestore(int id, string hashOtp);
    }
}