using System.ComponentModel.DataAnnotations;

namespace inmobiliariaApi.Dtos
{

    public class InmuebleDto
    {
        [Required]
        public int id { get; set; }
        [Required]
        public bool disponible { get; set; }
        public string? direccion { get; set; }

        public int idPropietario { get; set; }
        public string? tipo { get; set; }

        public int estado { get; set; }
        public string? uso { get; set; }

        public int ambientes { get; set; }
        public decimal superficie { get; set; }
        public decimal latitud { get; set; }
        public decimal longitud { get; set; }
        public decimal precio { get; set; }
        public string? imagen { get; set; }
        public ContratoDto contrato { get; set; }       


    }
    public enum Estado
        {
            Disponible= 1, 
            Suspendido= 2,
            Alquilado= 3
        }
}