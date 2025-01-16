using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data.SqlClient;
using System;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    public string connectionString = "Data Source=psytest1-aws.c32saqi0apl2.sa-east-1.rds.amazonaws.com;Initial Catalog=PsyTestBD;User ID=adminsql;Password=martin12345;Application Name=PsyTest_App;";
    public int usuarioId { get; private set; }
    public string Usuario { get; private set; }

    public string nombreUltimoTest;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GuardarResultadoEnBaseDeDatos(int testId, int puntaje)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO ResultadoTest (usuarioId, testId, puntaje, fechaModificacion, estado) VALUES (@usuarioId, @testId, @puntaje, @fechaModificacion, @estado)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@usuarioId", usuarioId); // Usar el ID del usuario guardado
                    command.Parameters.AddWithValue("@testId", testId);
                    command.Parameters.AddWithValue("@puntaje", puntaje);
                    command.Parameters.AddWithValue("@fechaModificacion", DateTime.Now);
                    command.Parameters.AddWithValue("@estado", 1); // Estado activo

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Debug.Log("Resultado guardado en la base de datos con éxito.");
                        ObtenerUltimoTestRealizado(connection);
                    }
                    else
                    {
                        Debug.LogError("No se pudo guardar el resultado en la base de datos.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al guardar el resultado en la base de datos: " + ex.Message);
        }
    }

    private void ObtenerUltimoTestRealizado(SqlConnection connection)
    {
        try
        {
            string selectQuery = @"
                SELECT TOP 1 Test.nombre 
                FROM ResultadoTest 
                INNER JOIN Test ON ResultadoTest.testId = Test.id 
                WHERE ResultadoTest.usuarioId = @usuarioId
                ORDER BY ResultadoTest.fechaModificacion DESC";

            using (SqlCommand command = new SqlCommand(selectQuery, connection))
            {
                command.Parameters.AddWithValue("@usuarioId", usuarioId);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    nombreUltimoTest = result.ToString(); // Almacena el nombre en la variable
                    Debug.Log($"Último test realizado: {nombreUltimoTest}");
                }
                else
                {
                    nombreUltimoTest = "No se encontró ningún test reciente.";
                    Debug.Log("No se encontró ningún test reciente.");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al obtener el último test realizado: " + ex.Message);
        }
    }


    public void IniciarSesion(string usuario, string contraseina, Action<Response> callback)
    {
        Debug.Log("Intentando iniciar sesión con usuario: " + usuario + " y contraseina: " + contraseina);

        string query = "SELECT id, usuario FROM usuario WHERE usuario = @usuario AND contraseina = @contraseina";

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@usuario", usuario);
                    command.Parameters.AddWithValue("@contraseina", contraseina);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                usuarioId = reader.GetInt32(0);
                                Usuario = reader["usuario"].ToString();
                                Debug.Log("Sesión iniciada: " + Usuario);
                            }
                            callback(new Response(true, "Sesión iniciada"));
                        }
                        else
                        {
                            Debug.LogError("Error, nombre de usuario o contraseña incorrecta");
                            callback(new Response(false, "Error, nombre de usuario o contraseña incorrecta"));
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error en el servidor: " + ex.Message);
            if (ex.InnerException != null)
            {
                Debug.LogError("Error interno: " + ex.InnerException.Message);
            }
            callback(new Response(false, "Error en el servidor: " + ex.Message));
        }
    }

    public List<CalendarManager.Appointment> ObtenerCitas(DateTime date)
    {
        List<CalendarManager.Appointment> citas = new List<CalendarManager.Appointment>();

        string query = "SELECT * FROM Cita WHERE fecha = @fecha";

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@fecha", date);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CalendarManager.Appointment cita = new CalendarManager.Appointment
                            {
                                Name = reader["situacion"].ToString(),
                                Time = reader["hora"].ToString(),
                                Location = "Miraflores", // Ajustar según tus necesidades
                                ProfileImage = null
                            };
                            citas.Add(cita);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al obtener las citas: " + ex.Message);
        }

        return citas;
    }

    public string ObtenerUrlImagenPerfil(int usuarioId)
    {
        string url = string.Empty;
        string query = "SELECT foto FROM DetalleEstudiante WHERE id = @usuarioId";

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@usuarioId", usuarioId);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        url = result.ToString();
                    }
                    else
                    {
                        Debug.LogWarning("No se encontró la URL de la imagen para el usuario con ID: " + usuarioId);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al obtener la URL de la imagen de perfil: " + ex.Message);
        }

        return url;
    }

    public UserData ObtenerDetallesUsuario(int usuarioId)
    {
        UserData userData = null;

        string query = @"SELECT 
                        nombre, 
                        apellidoPaterno, 
                        apellidoMaterno, 
                        correoInstitucional, 
                        correoPersonal, 
                        numeroDocumento AS dni, 
                        celular, 
                        fechaNacimiento, 
                        direccion
                     FROM DetalleEstudiante
                     WHERE usuarioId = @usuarioId";

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@usuarioId", usuarioId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userData = new UserData
                            {
                                nombre = reader["nombre"].ToString(),
                                apellidoPaterno = reader["apellidoPaterno"].ToString(),
                                apellidoMaterno = reader["apellidoMaterno"].ToString(),
                                correoInstitucional = reader["correoInstitucional"].ToString(),
                                correoPersonal = reader["correoPersonal"].ToString(),
                                dni = reader["dni"].ToString(),
                                celular = reader["celular"].ToString(),
                                fechaNacimiento = reader.GetDateTime(reader.GetOrdinal("fechaNacimiento")),
                                direccion = reader["direccion"].ToString()
                            };
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al obtener los detalles del usuario desde DetalleEstudiante: " + ex.Message);
        }

        return userData;
    }

    public PsychologistData ObtenerDetallesPsicologo(int usuarioId)
    {
        PsychologistData psychologistData = null;

        // Consulta para verificar el rol del usuario
        string queryRol = @"SELECT u.rolId, r.nombre AS rolNombre 
                        FROM Usuario u
                        JOIN Rol r ON u.rolId = r.id
                        WHERE u.id = @usuarioId";

        // Consulta para obtener detalles del psicólogo según el rol
        string queryPsicologo = @"SELECT 
                                dp.nombre, 
                                dp.apellidoPaterno, 
                                dp.apellidoMaterno, 
                                dp.foto 
                              FROM DetallePsicologo dp
                              WHERE dp.id = @usuarioId";

        // Si es estudiante, consulta la relación estudiante-psicólogo
        string queryEstudiantePsicologo = @"SELECT 
                                          dp.nombre, 
                                          dp.apellidoPaterno, 
                                          dp.apellidoMaterno, 
                                          dp.foto 
                                        FROM DetallePsicologo dp
                                        JOIN EstudiantePsicologo ep ON ep.psicologoId = dp.id
                                        WHERE ep.estudianteId = @usuarioId";

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Verificar rol del usuario
                string rolNombre = "";
                using (SqlCommand commandRol = new SqlCommand(queryRol, connection))
                {
                    commandRol.Parameters.AddWithValue("@usuarioId", usuarioId);
                    using (SqlDataReader readerRol = commandRol.ExecuteReader())
                    {
                        if (readerRol.Read())
                        {
                            rolNombre = readerRol["rolNombre"].ToString();
                            Debug.Log($"Rol del usuario: {rolNombre}");
                        }
                        else
                        {
                            Debug.LogWarning("No se encontró el rol del usuario.");
                            return null;
                        }
                    }
                }

                // Obtener detalles del psicólogo según el rol
                string queryToUse = (rolNombre == "Estudiante") ? queryEstudiantePsicologo : queryPsicologo;
                using (SqlCommand command = new SqlCommand(queryToUse, connection))
                {
                    command.Parameters.AddWithValue("@usuarioId", usuarioId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            psychologistData = new PsychologistData
                            {
                                nombre = reader["nombre"].ToString(),
                                apellidoPaterno = reader["apellidoPaterno"].ToString(),
                                apellidoMaterno = reader["apellidoMaterno"].ToString(),
                                foto = reader["foto"].ToString()
                            };
                            Debug.Log("Datos del psicólogo cargados correctamente.");
                        }
                        else
                        {
                            Debug.LogWarning("No se encontraron detalles del psicólogo.");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al obtener los detalles del psicólogo: " + ex.Message);
        }

        return psychologistData;
    }
    public void InsertAppointment(int estudianteId, int psicologoId, DateTime fecha, TimeSpan hora, string situacion, bool estado)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "INSERT INTO Cita (estudianteId, psicologoId, fecha, hora, situacion, fechaRegistro, fechaModificacion, estado) " +
                           "VALUES (@estudianteId, @psicologoId, @fecha, @hora, @situacion, GETDATE(), GETDATE(), @estado)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@estudianteId", estudianteId);
                command.Parameters.AddWithValue("@psicologoId", psicologoId);
                command.Parameters.AddWithValue("@fecha", fecha);
                command.Parameters.AddWithValue("@hora", hora);
                command.Parameters.AddWithValue("@situacion", situacion);
                command.Parameters.AddWithValue("@estado", estado);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
public class Response
{
  public bool done;
  public string message;
  public Response(bool done, string message)
  {
    this.done = done;
    this.message = message;
  }
}