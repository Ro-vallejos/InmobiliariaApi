
using System.Text.Json.Serialization;
using inmobiliariaApi.Helpers;


namespace inmobiliariaApi.Dtos
{
    public class PagoDto
    {
        public int idContrato { get; set; }
        [JsonPropertyName("id")]
        public int idPago { get; set; }
        public int nroPago { get; set; }
        [JsonConverter(typeof(JsonDateOnlyConverter))]
        public DateTime? fechaPago { get; set; }
        public string estado { get; set; } = "pendiente";
        public string concepto { get; set; } = "";
        public decimal importe { get; set; }
    }
    public enum EstadoPago
    {
        pendiente,
        recibido,
        anulado
    }
}