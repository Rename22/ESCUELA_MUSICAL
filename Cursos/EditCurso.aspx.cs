using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace Proyecto_FInal_Escuela_Musica.Cursos
{
    public partial class EditCurso : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hdnMensaje.Value = ""; // Limpiar mensaje al cargar

            if (!IsPostBack)
            {
                // CORRECCIÓN: Usar "id" en lugar de "id_cur"
                if (Request.QueryString["id"] != null)
                {
                    hdnIdCurso.Value = Request.QueryString["id"];
                    CargarProfesores();
                    CargarDatosCurso();
                }
                else
                {
                    Response.Redirect("ListCursos.aspx");
                }
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
                        ddlProfesor.Items.Clear();
                        ddlProfesor.Items.Add(new ListItem("Seleccione un profesor", ""));

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
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

        private void CargarDatosCurso()
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;
            string query = "SELECT nombre_cur, descripcion_cur, nivel_cur, duracion_semanas_cur, costo_total_cur, id_prof FROM cursos WHERE id_cur = @idCurso";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@idCurso", hdnIdCurso.Value);
                        conn.Open();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtNombre.Text = reader["nombre_cur"].ToString();
                                txtDescripcion.Text = reader["descripcion_cur"].ToString();
                                ddlNivel.SelectedValue = reader["nivel_cur"].ToString();
                                txtDuracion.Text = reader["duracion_semanas_cur"].ToString();
                                txtCosto.Text = reader["costo_total_cur"].ToString();

                                if (!reader.IsDBNull(reader.GetOrdinal("id_prof")))
                                {
                                    ddlProfesor.SelectedValue = reader["id_prof"].ToString();
                                }
                            }
                            else
                            {
                                hdnMensaje.Value = "ERR|Curso no encontrado";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        hdnMensaje.Value = "ERR|Error al cargar curso: " + ex.Message;
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
                    string sql = @"UPDATE cursos SET
                        nombre_cur = @nombre,
                        descripcion_cur = @descripcion,
                        nivel_cur = @nivel,
                        duracion_semanas_cur = @duracion,
                        costo_total_cur = @costo,
                        id_prof = @idProf
                        WHERE id_cur = @idCurso";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text.Trim());
                    cmd.Parameters.AddWithValue("@nivel", ddlNivel.SelectedValue);
                    cmd.Parameters.AddWithValue("@duracion", Convert.ToInt32(txtDuracion.Text.Trim()));
                    cmd.Parameters.AddWithValue("@costo", Convert.ToDecimal(txtCosto.Text.Trim()));
                    cmd.Parameters.AddWithValue("@idCurso", hdnIdCurso.Value);

                    // Profesor puede ser nulo
                    if (!string.IsNullOrEmpty(ddlProfesor.SelectedValue))
                    {
                        cmd.Parameters.AddWithValue("@idProf", Convert.ToInt32(ddlProfesor.SelectedValue));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@idProf", DBNull.Value);
                    }

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        hdnMensaje.Value = "REDIRECT|Curso actualizado correctamente.|ListCursos.aspx";
                    }
                    else
                    {
                        hdnMensaje.Value = "ERR|No se pudo actualizar el curso. Verifique los datos.";
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