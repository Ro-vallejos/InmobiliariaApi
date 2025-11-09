using inmobiliariaApi.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using inmobiliariaApi.Dtos;

namespace inmobiliariaApi.Repositorios
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {

        public RepositorioInmueble(IConfiguration configuration) : base(configuration) { }
        public InmuebleDto? ObtenerInmuebleId(int id)
        {
            InmuebleDto? inmueble = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT id, id_propietario, direccion, uso, tipo, ambientes, longitud, latitud, precio, estado, imagen FROM inmueble WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        string estadoString = reader.GetString("estado");
                        Estado estadoEnum = (Estado)Enum.Parse(typeof(Estado), estadoString, true);
                       int estadoInt = (int)estadoEnum;

                        inmueble = new InmuebleDto
                        {
                            id = reader.GetInt32("id"),
                            idPropietario = reader.GetInt32("id_propietario"),
                            direccion = reader.GetString("direccion"),
                            uso = reader.GetString("uso"),
                            tipo = reader.GetString("tipo"),
                            ambientes = reader.GetInt32("ambientes"),
                            precio = reader.GetDecimal("precio"),
                            estado = estadoInt,
                            imagen = reader.GetString("imagen")
                        };
                    }
                    connection.Close();
                }
            }
            return inmueble;
        }

        public void AgregarInmueble(InmuebleDto inmuebleNuevo)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    var sql = "INSERT INTO inmueble (id_propietario, direccion, uso, tipo, ambientes, precio, imagen, estado) VALUES (@id_propietario, @direccion, @uso, @tipo, @ambientes, @precio, @imagen, 2)";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        connection.Open();
                        command.Parameters.AddWithValue("@id_propietario", inmuebleNuevo.idPropietario);
                        command.Parameters.AddWithValue("@direccion", inmuebleNuevo.direccion);
                        command.Parameters.AddWithValue("@uso", inmuebleNuevo.uso.ToString());
                        command.Parameters.AddWithValue("@tipo", inmuebleNuevo.tipo);
                        command.Parameters.AddWithValue("@ambientes", inmuebleNuevo.ambientes);
                        command.Parameters.AddWithValue("@precio", inmuebleNuevo.precio);
                        command.Parameters.AddWithValue("@imagen", inmuebleNuevo.imagen);
                        command.ExecuteNonQuery();
                        connection.Close();
                        Console.WriteLine("Inmueble agregado correctamente.");
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error SQL: {ex.Message}");
            }
        }
        public void ActualizarEstado(int id, int estado)
        {
            string sql = "UPDATE inmueble SET estado=@estado WHERE id=@id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@estado", estado);
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }

        }
        public List<InmuebleDto> ObtenerInmueblesPorPropietarioDto(int propietarioId)
        {
            List<InmuebleDto> inmuebles = new List<InmuebleDto>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "SELECT id, id_propietario, direccion, uso, tipo, ambientes, longitud,latitud,precio,estado,imagen, tipo FROM inmueble WHERE id_propietario = @propietarioId";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@propietarioId", propietarioId);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string estadoString = reader.GetString("estado");
                        Estado estadoEnum = (Estado)Enum.Parse(typeof(Estado), estadoString, true);
                        int estadoInt = (int)estadoEnum;
                        inmuebles.Add(new InmuebleDto
                        {
                            id = reader.GetInt32("id"),
                            idPropietario = reader.GetInt32("id_propietario"),
                            direccion = reader.GetString("direccion"),
                            uso = reader.GetString("uso"),
                            ambientes = reader.GetInt32("ambientes"),
                            precio = reader.GetDecimal("precio"),
                            estado = estadoInt,
                            imagen = reader.GetString("imagen"),
                            tipo = reader.GetString("tipo")
                        });
                    }
                    connection.Close();
                }
            }
             return inmuebles.Count == 0 ? null : inmuebles;
        }
        
        public List<InmuebleDto> ObtenerConContratoVigente(int propietarioId)
        {
             List<InmuebleDto> inmuebles = new List<InmuebleDto>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = @"SELECT DISTINCT i.*
                FROM inmueble i
                JOIN contrato c ON c.id_inmueble = i.id
                WHERE c.estado = 1 AND i.id_propietario = @propietarioId;
                ";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@propietarioId", propietarioId);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string estadoString = reader.GetString("estado");
                        Estado estadoEnum = (Estado)Enum.Parse(typeof(Estado), estadoString, true);
                        int estadoInt = (int)estadoEnum;
                        inmuebles.Add(new InmuebleDto
                        {
                            id = reader.GetInt32("id"),
                            idPropietario = reader.GetInt32("id_propietario"),
                            direccion = reader.GetString("direccion"),
                            uso = reader.GetString("uso"),
                            ambientes = reader.GetInt32("ambientes"),
                            precio = reader.GetDecimal("precio"),
                            estado = estadoInt,
                            imagen = reader.GetString("imagen"),
                            tipo = reader.GetString("tipo"),
                        });
                    }
                    connection.Close();
                }
            }
             return inmuebles.Count == 0 ? null : inmuebles;
        }


    }

}

