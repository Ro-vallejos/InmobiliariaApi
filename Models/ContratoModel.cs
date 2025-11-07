
namespace inmobiliariaApi.Dtos
{
    public class ContratoDto
    {
         public int idContrato { get; set; }
        public int idInmueble { get; set; }
        public int idInquilino { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
        public int estado { get; set; }
        public decimal montoMensual { get; set; }
        public InmuebleDto inmueble { get; set; }
        public InquilinoDto inquilino { get; set; }
    }
}