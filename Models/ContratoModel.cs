
using System.Text.Json.Serialization;
using inmobiliariaApi.Helpers;
using System.ComponentModel.DataAnnotations;

    public class Contrato
    {   
        [Key]
        [JsonPropertyName("id")]
        public int idContrato { get; set; }
        public int idInmueble { get; set; }
        public int idInquilino { get; set; }
        [JsonConverter(typeof(JsonDateOnlyConverter))]
        public DateTime fechaInicio { get; set; }
        [JsonConverter(typeof(JsonDateOnlyConverter))]
        public DateTime fechaFin { get; set; }
        public int estado { get; set; }
        public decimal montoMensual { get; set; }
        public Inmueble inmueble { get; set; }
        public Inquilino inquilino { get; set; }
    }
