using inmobiliariaApi.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using inmobiliariaApi.Dtos;

namespace inmobiliariaApi.Repositorios;

public class RepositorioPago : RepositorioBase, IRepositorioPago
{
    public RepositorioPago(IConfiguration configuration) : base(configuration) { }

    public List<PagoDto> ObtenerPagosPorContrato(int contratoId)
    {
        List<PagoDto> pagos = new List<PagoDto>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = @"
            SELECT  *
            FROM pago p
            WHERE p.id_contrato = @idContrato
            AND p.estado = 'recibido'
            ORDER BY p.nro_pago ASC";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@idContrato", contratoId);
                var reader = command.ExecuteReader();

                int idxId = reader.GetOrdinal("id");
                int idxIdContrato = reader.GetOrdinal("id_contrato");
                int idxNroPago = reader.GetOrdinal("nro_pago");
                int idxFechaPago = reader.GetOrdinal("fecha_pago");
                int idxEstado = reader.GetOrdinal("estado");
                int idxConcepto = reader.GetOrdinal("concepto");
                int idxImporte = reader.GetOrdinal("importe");

                while (reader.Read())
                {
                    DateTime? fechaPago = null;
                    var fechaString = reader.IsDBNull(idxFechaPago) ? null : reader.GetValue(idxFechaPago).ToString();
                    if (!string.IsNullOrEmpty(fechaString) && fechaString != "0000-00-00")
                    {
                        fechaPago = DateTime.Parse(fechaString);
                    }

                    pagos.Add(new PagoDto
                    {
                        idPago = reader.GetInt32(idxId),
                        idContrato = reader.GetInt32(idxIdContrato),
                        nroPago = reader.GetInt32(idxNroPago),
                        fechaPago = fechaPago,
                        importe = reader.GetDecimal(idxImporte),
                        estado = reader.GetString(idxEstado) switch
                        {
                            "pendiente" => EstadoPago.pendiente.ToString(),
                            "recibido" => EstadoPago.recibido.ToString(),
                            "anulado" => EstadoPago.anulado.ToString(),
                            _ => throw new ArgumentException("Estado de pago no reconocido")
                        },
                        concepto = reader.GetString(idxConcepto)
                    });
                }
                connection.Close();
            }
        }
        if (pagos.Count == 0)
             return null; 
        return pagos;
    }

}