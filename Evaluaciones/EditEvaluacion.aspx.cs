using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace Proyecto_FInal_Escuela_Musica.Evaluaciones
{
    public partial class EditEvaluacion : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hdnMensaje.Value = "";

            if (!IsPostBack)
            {
                // Obtener ID de la evaluación desde QueryString
                string idEvaluacion = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(idEvaluacion))
                {
                    hdnIdEvaluacion.Value = idEvaluacion;
                    CargarEstudiantes();
                    CargarCursos();
                    CargarDatosEvaluacion(idEvaluacion);
                }
                else
                {
                    // Si no hay ID, redirigir a la lista
                    Response.Redirect("ListEvaluaciones.aspx");
                }
            }
        }

        private void CargarEstudiantes()
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;
            string query = "SELECT id_estu, CONCAT(nombre_estu, ' ', apellido_estu) AS nombre_completo " +
                           "FROM estudiantes ORDER BY nombre_estu";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlEstudiante.Items.Clear();
                            ddlEstudiante.Items.Add(new ListItem("Seleccione un estudiante", ""));

                            while (reader.Read())
                            {
                                ddlEstudiante.Items.Add(new ListItem(
                                    reader["nombre_completo"].ToString(),
                                    reader["id_estu"].ToString()
                                ));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        hdnMensaje.Value = "ERR|Error al cargar estudiantes: " + ex.Message;
                    }
                }
            }
        }

        private void CargarCursos()
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;
            string query = "SELECT id_cur, nombre_cur FROM cursos ORDER BY nombre_cur";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlCurso.Items.Clear();
                            ddlCurso.Items.Add(new ListItem("Seleccione un curso", ""));

                            while (reader.Read())
                            {
                                ddlCurso.Items.Add(new ListItem(
                                    reader["nombre_cur"].ToString(),
                                    reader["id_cur"].ToString()
                                ));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        hdnMensaje.Value = "ERR|Error al cargar cursos: " + ex.Message;
                    }
                }
            }
        }

        private void CargarDatosEvaluacion(string idEvaluacion)
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;
            string query = @"SELECT id_estu, id_cur, tipo_eva, fecha_eva, puntuacion_eva 
                             FROM evaluaciones 
                             WHERE id_eva = @idEvaluacion";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idEvaluacion", idEvaluacion);

                    try
                    {
                        conn.Open();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Establecer valores en los controles
                                ddlEstudiante.SelectedValue = reader["id_estu"].ToString();
                                ddlCurso.SelectedValue = reader["id_cur"].ToString();
                                ddlTipoEvaluacion.SelectedValue = reader["tipo_eva"].ToString();

                                // Formatear fecha para el input DateTimeLocal
                                DateTime fecha = Convert.ToDateTime(reader["fecha_eva"]);
                                txtFechaEvaluacion.Text = fecha.ToString("yyyy-MM-ddTHH:mm");

                                txtPuntuacion.Text = reader["puntuacion_eva"].ToString();
                            }
                            else
                            {
                                hdnMensaje.Value = "ERR|Evaluación no encontrada";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        hdnMensaje.Value = "ERR|" + ex.Message;
                    }
                }
            }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            // Validaciones en el servidor
            if (ddlEstudiante.SelectedValue == "")
            {
                hdnMensaje.Value = "ERR|Seleccione un estudiante";
                return;
            }

            if (ddlCurso.SelectedValue == "")
            {
                hdnMensaje.Value = "ERR|Seleccione un curso";
                return;
            }

            if (ddlTipoEvaluacion.SelectedValue == "")
            {
                hdnMensaje.Value = "ERR|Seleccione un tipo de evaluación";
                return;
            }

            DateTime fechaEvaluacion;
            if (!DateTime.TryParse(txtFechaEvaluacion.Text, out fechaEvaluacion))
            {
                hdnMensaje.Value = "ERR|Fecha de evaluación inválida";
                return;
            }

            decimal puntuacion;
            if (!decimal.TryParse(txtPuntuacion.Text, out puntuacion))
            {
                hdnMensaje.Value = "ERR|Puntuación debe ser un número";
                return;
            }

            if (puntuacion < 0 || puntuacion > 10)
            {
                hdnMensaje.Value = "ERR|Puntuación debe estar entre 0 y 10";
                return;
            }

            string idEvaluacion = hdnIdEvaluacion.Value;
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = @"UPDATE evaluaciones 
                                   SET 
                                        id_estu = @idEstudiante, 
                                        id_cur = @idCurso, 
                                        tipo_eva = @tipo, 
                                        fecha_eva = @fecha, 
                                        puntuacion_eva = @puntuacion
                                   WHERE id_eva = @idEvaluacion";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@idEstudiante", Convert.ToInt32(ddlEstudiante.SelectedValue));
                    cmd.Parameters.AddWithValue("@idCurso", Convert.ToInt32(ddlCurso.SelectedValue));
                    cmd.Parameters.AddWithValue("@tipo", ddlTipoEvaluacion.SelectedValue);
                    cmd.Parameters.AddWithValue("@fecha", fechaEvaluacion);
                    cmd.Parameters.AddWithValue("@puntuacion", puntuacion);
                    cmd.Parameters.AddWithValue("@idEvaluacion", idEvaluacion);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        hdnMensaje.Value = "REDIRECT|Evaluación actualizada correctamente.|ListEvaluaciones.aspx";
                    }
                    else
                    {
                        hdnMensaje.Value = "ERR|No se pudo actualizar la evaluación. Verifique el ID.";
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