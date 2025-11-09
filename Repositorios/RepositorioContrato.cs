using inmobiliariaApi.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using inmobiliariaApi.Dtos;

namespace inmobiliariaApi.Repositorios;

public class RepositorioContrato : RepositorioBase, IRepositorioContrato
{
    public RepositorioContrato(IConfiguration configuration) : base(configuration) { }
    public ContratoDto? ObtenerContratoVigentePorInmueble(int idInmueble)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = @"
                SELECT 
                    c.*, 
                    iinq.id AS inq_id, iinq.nombre AS iinq_nombre, iinq.apellido AS iinq_apellido, iinq.dni AS iinq_dni, iinq.email AS iinq_email, iinq.telefono AS iinq_telefono,
                    inm.id AS inm_id, inm.direccion AS inm_direccion, inm.tipo AS inm_tipo, inm.uso AS inm_uso, inm.precio AS inm_precio
                FROM contrato c
                JOIN inquilino iinq ON iinq.id = c.id_inquilino
                JOIN inmueble inm   ON inm.id  = c.id_inmueble
                WHERE c.id_inmueble = @idInmueble
                AND CURDATE() BETWEEN c.fecha_inicio AND c.fecha_fin
                AND c.estado = 1
                LIMIT 1;";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@idInmueble", idInmueble);
                connection.Open();
                var reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                var contrato = new ContratoDto
                {
                    idContrato = reader.GetInt32("id"),
                    idInmueble = reader.GetInt32("id_inmueble"),
                    idInquilino = reader.GetInt32("id_inquilino"),
                    fechaInicio = reader.GetDateTime("fecha_inicio").Date,
                    fechaFin = reader.GetDateTime("fecha_fin").Date,
                    estado = reader.GetInt32("estado"),
                    montoMensual = reader.GetDecimal("monto_mensual"),

                    inquilino = new InquilinoDto
                    {
                        idInquilino = reader.GetInt32("inq_id"),
                        nombre = reader.GetString("iinq_nombre"),
                        apellido = reader.GetString("iinq_apellido"),
                        dni = reader.GetString("iinq_dni"),
                        email = reader.GetString("iinq_email"),
                        telefono = reader.GetString("iinq_telefono")
                    },
                    inmueble = new InmuebleDto
                    {
                        id = reader.GetInt32("inm_id"),
                        direccion = reader.GetString("inm_direccion"),
                        tipo = reader.GetString("inm_tipo"),
                        uso = reader.GetString("inm_uso"),
                        precio = reader.GetDecimal("inm_precio"),
                    },
                };
                return contrato;
            }
        }
    }

}
