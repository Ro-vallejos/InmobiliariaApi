using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


    public class Inmueble
    {
        [Required]
        [Key]
        [JsonPropertyName("id")]
        public int id { get; set; }
        
        //public bool disponible { get; set; }
        public string? direccion { get; set; }

        public int idPropietario { get; set; }
        public string? tipo { get; set; }

        public int estado { get; set; }
        public string? uso { get; set; }

        public int ambientes { get; set; }
        public decimal superficie { get; set; }
        public decimal precio { get; set; }
        public string? imagen { get; set; }


    }
    public enum Estado
        {
            Disponible= 1, 
            Suspendido= 2,
            Alquilado= 3
        }
