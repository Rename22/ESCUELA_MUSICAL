using System;
using System.Data;
using System.Configuration;
using System.Web.UI;
using MySql.Data.MySqlClient;

namespace Proyecto_FInal_Escuela_Musica.Especialidades
{
    public partial class AddEspecialidad : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hdnMensaje.Value = ""; // Limpiar mensaje al cargar
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string descripcion = txtDescripcion.Text.Trim();

            // Validación básica en servidor
            if (nombre.Length < 3)
            {
                hdnMensaje.Value = "ERR|El nombre debe tener al menos 3 caracteres";
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // Verificar si ya existe
                    string checkSql = "SELECT COUNT(*) FROM especialidades WHERE nombre_espec = @nombre";
                    MySqlCommand checkCmd = new MySqlCommand(checkSql, conn);
                    checkCmd.Parameters.AddWithValue("@nombre", nombre);

                    int existe = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (existe > 0)
                    {
                        hdnMensaje.Value = "WARN|Ya existe una especialidad con ese nombre";
                        return;
                    }

                    // Insertar nueva especialidad
                    string insertSql = @"INSERT INTO especialidades 
                        (nombre_espec, descripcion_espec) 
                        VALUES 
                        (@nombre, @descripcion)";

                    MySqlCommand insertCmd = new MySqlCommand(insertSql, conn);
                    insertCmd.Parameters.AddWithValue("@nombre", nombre);
                    insertCmd.Parameters.AddWithValue("@descripcion",
                        string.IsNullOrEmpty(descripcion) ? DBNull.Value : (object)descripcion);

                    insertCmd.ExecuteNonQuery();

                    // Mensaje con redirección
                    hdnMensaje.Value = "REDIRECT|Especialidad agregada correctamente.|ListEspecialidades.aspx";

                    // Limpiar campos
                    txtNombre.Text = "";
                    txtDescripcion.Text = "";
                }
                catch (MySqlException ex)
                {
                    hdnMensaje.Value = "ERR|Error en base de datos: " + ex.Message;
                }
                catch (Exception ex)
                {
                    hdnMensaje.Value = "ERR|Error inesperado: " + ex.Message;
                }
            }
        }
    }
}