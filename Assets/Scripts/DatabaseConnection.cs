using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data.SqlClient;
using System;

public class DatabaseConnection : MonoBehaviour
{
    public string connectionString = "Data Source=localhost; Initial Catalog=PsyTestBD; User ID=sa; Password=1234;";

    void Start()
    {
        // Aquí puedes reemplazar "usuario" y "contraseña" con los valores reales introducidos por el usuario
        IniciarSesion("nombre_de_usuario", "contrasena");
    }

    void IniciarSesion(string usuario, string contraseina)
    {
        Debug.Log("Intentando iniciar sesión con usuario: " + usuario + " y contraseina: " + contraseina);

        string query = "SELECT usuario FROM usuario WHERE usuario = @usuario AND contraseina = @contraseina";

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
                                Debug.Log("Sesión iniciada: " + reader["usuario"].ToString());
                            }
                        }
                        else
                        {
                            Debug.LogError("Error, nombre de usuario o contraseña incorrecta");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error en el servidor: " + ex.Message);
        }
    }
}