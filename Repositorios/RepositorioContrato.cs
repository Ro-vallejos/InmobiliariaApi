using inmobiliariaApi.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using inmobiliariaApi.Dtos;

namespace inmobiliariaApi.Repositorios;

public class RepositorioContrato : RepositorioBase, IRepositorioContrato
{
    public RepositorioContrato(IConfiguration configuration) : base(configuration) { }
    public InquilinoDto? ObtenerInquilinoDeContrato(int contratoId, int idPropietario)
    {
        InquilinoDto inquilino = new InquilinoDto();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = @"
                SELECT q.*
                FROM contrato c
                JOIN inmueble i  ON i.id = c.id_inmueble
                JOIN inquilino q ON q.id = c.id_inquilino
                WHERE c.id = @contratoId
                AND i.id_propietario = @idPropietario
                AND c.estado=1";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@contratoId", contratoId);
                command.Parameters.AddWithValue("@idPropietario", idPropietario);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    inquilino.idInquilino = reader.GetInt32("id");
                    inquilino.nombre = reader.GetString("nombre");
                    inquilino.apellido = reader.GetString("apellido");
                    inquilino.dni = reader.GetString("dni");
                    inquilino.email = reader.GetString("email");
                    inquilino.telefono = reader.GetString("telefono");
                    connection.Close();
                    return inquilino;
                }
                connection.Close();
            }
        }
            return inquilino;
    }
    public ContratoDto? ObtenerContratoVigentePorInmueble(int idInmueble)
    {
        ContratoDto contrato = new ContratoDto();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = @"
            SELECT  c.id, c.id_inmueble, c.fecha_inicio, c.fecha_fin, c.estado,
                    c.monto_mensual,
                    i.id AS inq_id, i.nombre, i.apellido, i.dni, i.email, i.telefono
            FROM contrato c
            JOIN inquilino i ON i.id = c.id_inquilino
            WHERE c.id_inmueble = @idInmueble
              AND CURDATE() BETWEEN c.fecha_inicio AND c.fecha_fin
            ORDER BY c.fecha_inicio DESC
            LIMIT 1;";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@idInmueble", idInmueble);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    contrato.idContrato = reader.GetInt32("id");
                    contrato.idInmueble = reader.GetInt32("id_inmueble");
                    contrato.fechaInicio = reader.GetDateTime("fecha_inicio");
                    contrato.fechaFin = reader.GetDateTime("fecha_fin");
                    contrato.estado = reader.GetInt32("estado");
                    contrato.montoMensual = reader.GetDecimal("monto_mensual");

                    contrato.inquilino = new InquilinoDto
                    {
                        idInquilino = reader.GetInt32("inq_id"),
                        nombre = reader.GetString("nombre"),
                        apellido = reader.GetString("apellido"),
                        dni = reader.GetString("dni"),
                        email = reader.GetString("email"),
                        telefono = reader.GetString("telefono")
                    };
                    connection.Close();
                }
            }
        }
        return contrato;
    }

}
