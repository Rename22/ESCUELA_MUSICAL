using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace Proyecto_FInal_Escuela_Musica.Cursos
{
    public partial class AddCurso : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hdnMensaje.Value = ""; // Limpiar mensaje al cargar

            if (!IsPostBack)
            {
                CargarProfesores();
            }
        }

        private void CargarProfesores()
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;
            string query = "SELECT id_prof, CONCAT(nombre_prof, ' ', apellido_prof) AS nombre_completo FROM profesores ORDER BY nombre_prof";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlProfesor.Items.Clear();
                            ddlProfesor.Items.Add(new ListItem("Seleccione un profesor", ""));

                            while (reader.Read())
                            {
                                ddlProfesor.Items.Add(new ListItem(
                                    reader["nombre_completo"].ToString(),
                                    reader["id_prof"].ToString()
                                ));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        hdnMensaje.Value = "ERR|Error al cargar profesores: " + ex.Message;
                    }
                }
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = @"INSERT INTO cursos 
                        (nombre_cur, descripcion_cur, nivel_cur, duracion_semanas_cur, costo_total_cur, id_prof) 
                        VALUES 
                        (@nombre, @descripcion, @nivel, @duracion, @costo, @idProf)";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text.Trim());
                    cmd.Parameters.AddWithValue("@nivel", ddlNivel.SelectedValue);
                    cmd.Parameters.AddWithValue("@duracion", Convert.ToInt32(txtDuracion.Text.Trim()));
                    cmd.Parameters.AddWithValue("@costo", Convert.ToDecimal(txtCosto.Text.Trim()));

                    // Profesor puede ser nulo
                    if (!string.IsNullOrEmpty(ddlProfesor.SelectedValue))
                    {
                        cmd.Parameters.AddWithValue("@idProf", Convert.ToInt32(ddlProfesor.SelectedValue));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@idProf", DBNull.Value);
                    }

                    cmd.ExecuteNonQuery();

                    // Mensaje con redirección
                    hdnMensaje.Value = "REDIRECT|Curso agregado correctamente.|ListCursos.aspx";

                    // Limpiar campos
                    txtNombre.Text = "";
                    txtDescripcion.Text = "";
                    ddlNivel.SelectedIndex = 0;
                    txtDuracion.Text = "";
                    txtCosto.Text = "";
                    ddlProfesor.SelectedIndex = 0;
                }
                catch (MySqlException ex)
                {
                    hdnMensaje.Value = "ERR|" + ex.Message;
                }
            }
        }
    }
}