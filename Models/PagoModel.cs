
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using inmobiliariaApi.Helpers;

    public class Pago
    {
        [Key]
        [JsonPropertyName("id")]
        public int idPago { get; set; }
        public int idContrato { get; set; }
        public int nroPago { get; set; }
        [JsonConverter(typeof(JsonDateOnlyConverter))]
        public DateTime? fechaPago { get; set; }
       // public string estado { get; set; } = "pendiente";
       public EstadoPago estado { get; set; }
        public string concepto { get; set; } = "";
        public decimal importe { get; set; }
    }
    public enum EstadoPago
    {
        pendiente,
        recibido,
        anulado
    }
