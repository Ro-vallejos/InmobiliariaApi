using inmobiliariaApi.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace inmobiliariaApi.Repositorios
{
    public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
    {
        public RepositorioPropietario(IConfiguration configuration) : base(configuration) { }

        public List<Propietario> ObtenerPropietarios()
        {
            List<Propietario> propietarios = new List<Propietario>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "SELECT id, nombre, apellido, dni, email, telefono, estado FROM propietario";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Propietario propietario = new Propietario();
                        propietario.id = reader.GetInt32("id");
                        propietario.nombre = reader.GetString("nombre");
                        propietario.apellido = reader.GetString("apellido");
                        propietario.dni = reader.GetString("dni");
                        propietario.email = reader.GetString("email");
                        propietario.telefono = reader.GetString("telefono");
                        propietario.estado = reader.GetInt32("estado");
                        propietarios.Add(propietario);
                    }
                    connection.Close();
                }
                return propietarios;
            }
        }

        public Propietario ObtenerPropietarioId(int id)
        {
            Propietario propietario = new Propietario();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT id, nombre, apellido, dni, email, telefono, estado, clave FROM propietario WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        propietario.id = reader.GetInt32("id");
                        propietario.nombre = reader.GetString("nombre");
                        propietario.apellido = reader.GetString("apellido");
                        propietario.dni = reader.GetString("dni");
                        propietario.email = reader.GetString("email");
                        propietario.telefono = reader.GetString("telefono");
                        propietario.estado = reader.GetInt32("estado");
                        propietario.clave = reader.GetString("clave");
                    }
                    connection.Close();
                }
            }
            return propietario;
        }

        public Propietario ActualizarPropietario(Propietario propietario)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var sql = "UPDATE propietario SET nombre = @nombre, apellido = @apellido, dni = @dni, email = @email, telefono = @telefono WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", propietario.id);
                    command.Parameters.AddWithValue("@nombre", propietario.nombre);
                    command.Parameters.AddWithValue("@apellido", propietario.apellido);
                    command.Parameters.AddWithValue("@dni", propietario.dni);
                    command.Parameters.AddWithValue("@email", propietario.email);
                    command.Parameters.AddWithValue("@telefono", propietario.telefono);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return propietario;
        }

       

        public List<Propietario> ObtenerPropietariosActivos()
        {
            List<Propietario> propietarios = new List<Propietario>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "SELECT id, nombre, apellido, dni, email, telefono, estado FROM propietario WHERE estado = 1";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Propietario propietario = new Propietario();
                        propietario.id = reader.GetInt32("id");
                        propietario.nombre = reader.GetString("nombre");
                        propietario.apellido = reader.GetString("apellido");
                        propietario.dni = reader.GetString("dni");
                        propietario.email = reader.GetString("email");
                        propietario.telefono = reader.GetString("telefono");
                        propietario.estado = reader.GetInt32("estado");
                        propietarios.Add(propietario);
                    }
                    connection.Close();
                }
                return propietarios;
            }
        }

        public Propietario ObtenerPorEmail(string email)
        {
            Propietario propietario = new Propietario();
            var query = "SELECT id, email, dni, telefono, clave, estado, nombre, apellido FROM propietario WHERE email = @email;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@email", email);
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        propietario.id = reader.GetInt32("id");
                        propietario.nombre = reader.GetString("nombre");
                        propietario.apellido = reader.GetString("apellido");
                        propietario.dni = reader.GetString("dni");
                        propietario.email = reader.GetString("email");
                        propietario.telefono = reader.GetString("telefono");
                        propietario.estado = reader.GetInt32("estado");
                        propietario.clave = reader.GetString("clave");
                    }
                    connection.Close();
                }
                return propietario;
            }

        }
        public void ActualizarClave(int id, string nuevoHash)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var sql = "UPDATE propietario SET clave = @clave WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@clave", nuevoHash);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        public void GuardarPassRestore(int id, string hashOtp)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "UPDATE propietario SET pass_restore = @otp WHERE id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@otp", hashOtp);
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            } 
        }
    }
}