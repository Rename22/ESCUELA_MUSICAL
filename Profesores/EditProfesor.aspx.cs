using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace Proyecto_FInal_Escuela_Musica.Profesores
{
    public partial class EditProfesor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hdnMensaje.Value = ""; // limpiar mensaje al cargar

            if (!IsPostBack)
            {
                // Cargar las especialidades primero
                CargarEspecialidades();

                if (Request.QueryString["id"] != null)
                {
                    string idProfesor = Request.QueryString["id"];
                    CargarDatosProfesor(idProfesor);
                }
                else
                {
                    // Redirigir si no hay id
                    Response.Redirect("ListProfesores.aspx");
                }
            }
        }

        private void CargarEspecialidades()
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;
            string query = "SELECT id_espec, nombre_espec FROM especialidades ORDER BY nombre_espec";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();

                        ddlEspecialidad.Items.Clear();
                        ddlEspecialidad.Items.Add(new ListItem("Seleccione una especialidad", ""));

                        while (reader.Read())
                        {
                            ddlEspecialidad.Items.Add(new ListItem(
                                reader["nombre_espec"].ToString(),
                                reader["id_espec"].ToString()
                            ));
                        }
                    }
                    catch (Exception ex)
                    {
                        hdnMensaje.Value = "ERR|Error al cargar especialidades: " + ex.Message;
                    }
                }
            }
        }

        private void CargarDatosProfesor(string idProfesor)
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = @"SELECT p.id_prof, p.cedula_prof, p.nombre_prof, p.apellido_prof, 
                                  p.id_espec, p.telefono_prof, p.email_prof, e.nombre_espec
                           FROM profesores p
                           INNER JOIN especialidades e ON p.id_espec = e.id_espec
                           WHERE p.id_prof = @id";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", idProfesor);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            hdnIdProfesor.Value = reader["id_prof"].ToString();
                            txtCedula.Text = reader["cedula_prof"].ToString();
                            txtNombre.Text = reader["nombre_prof"].ToString();
                            txtApellido.Text = reader["apellido_prof"].ToString();
                            ddlEspecialidad.SelectedValue = reader["id_espec"].ToString();
                            txtTelefono.Text = reader["telefono_prof"].ToString();
                            txtEmail.Text = reader["email_prof"].ToString();
                        }
                        else
                        {
                            // Si no encuentra el profesor, redirigir
                            Response.Redirect("ListProfesores.aspx");
                        }
                    }
                }
                catch (Exception ex)
                {
                    hdnMensaje.Value = "ERR|" + ex.Message;
                }
            }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = @"UPDATE profesores 
                                   SET cedula_prof = @cedula, 
                                       nombre_prof = @nombre, 
                                       apellido_prof = @apellido, 
                                       id_espec = @id_espec, 
                                       telefono_prof = @telefono, 
                                       email_prof = @email 
                                   WHERE id_prof = @id";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@cedula", txtCedula.Text.Trim());
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@apellido", txtApellido.Text.Trim());
                    cmd.Parameters.AddWithValue("@id_espec", ddlEspecialidad.SelectedValue);
                    cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", hdnIdProfesor.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Mensaje con redirección
                        hdnMensaje.Value = "REDIRECT|Profesor actualizado correctamente.|ListProfesores.aspx";
                    }
                    else
                    {
                        hdnMensaje.Value = "ERR|No se pudo actualizar el profesor. Por favor, intente de nuevo.";
                    }
                }
                catch (MySqlException ex)
                {
                    hdnMensaje.Value = "ERR|" + ex.Message;
                }
            }
        }
    }
}