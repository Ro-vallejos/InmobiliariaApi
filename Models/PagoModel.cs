
namespace inmobiliariaApi.Dtos
{
    public class PagoDto
    {
        public int idContrato { get; set; }
        public int idPago { get; set; }
        public int nroPago { get; set; }
        public DateTime? fechaPago { get; set; }
        public string estado { get; set; } = "pendiente";
        public string concepto { get; set; } = "";
    }
     public enum EstadoPago
    {
        pendiente,
        recibido,
        anulado
    }
}