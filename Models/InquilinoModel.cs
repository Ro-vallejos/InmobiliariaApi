
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


    public class Inquilino
    {
        [JsonPropertyName("idContrato")]
        [Key]
        public int idInquilino { get; set; }
        public string? nombre { get; set; }
        public string? apellido { get; set; }
        public string? dni { get; set; }
        public string? email { get; set; }
        public string? telefono { get; set; }

    }
