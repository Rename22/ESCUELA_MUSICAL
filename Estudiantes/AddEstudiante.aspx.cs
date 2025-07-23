using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace Proyecto_FInal_Escuela_Musica.Estudiantes
{
    public partial class AddEstudiante : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hdnMensaje.Value = ""; // limpiar mensaje al cargar
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = @"INSERT INTO estudiantes 
                        (cedula_estu, nombre_estu, apellido_estu, fecha_nacimiento_estu, telefono_estu, email_estu) 
                        VALUES 
                        (@cedula, @nombre, @apellido, @fecha_nac, @telefono, @email)";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@cedula", txtCedula.Text.Trim());
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@apellido", txtApellido.Text.Trim());
                    cmd.Parameters.AddWithValue("@fecha_nac", Convert.ToDateTime(txtFechaNacimiento.Text));
                    cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());

                    cmd.ExecuteNonQuery();

                    // Mensaje con redirección
                    hdnMensaje.Value = "REDIRECT|Estudiante agregado correctamente.|ListEstudiantes.aspx";

                    // Limpiar campos
                    txtCedula.Text = "";
                    txtNombre.Text = "";
                    txtApellido.Text = "";
                    txtFechaNacimiento.Text = "";
                    txtTelefono.Text = "";
                    txtEmail.Text = "";
                }
                catch (MySqlException ex)
                {
                    hdnMensaje.Value = "ERR|" + ex.Message;
                }
            }
        }
    }
}