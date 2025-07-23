using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace Proyecto_FInal_Escuela_Musica.Evaluaciones
{
    public partial class AddEvaluacion : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hdnMensaje.Value = "";

            if (!IsPostBack)
            {
                CargarEstudiantes();
                CargarCursos();
                txtFechaEvaluacion.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
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

        protected void btnGuardar_Click(object sender, EventArgs e)
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

            if (puntuacion < 0 || puntuacion > 100)
            {
                hdnMensaje.Value = "ERR|Puntuación debe estar entre 0 y 100";
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = @"INSERT INTO evaluaciones 
                        (id_estu, id_cur, tipo_eva, fecha_eva, puntuacion_eva) 
                        VALUES 
                        (@idEstudiante, @idCurso, @tipo, @fecha, @puntuacion)";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@idEstudiante", Convert.ToInt32(ddlEstudiante.SelectedValue));
                    cmd.Parameters.AddWithValue("@idCurso", Convert.ToInt32(ddlCurso.SelectedValue));
                    cmd.Parameters.AddWithValue("@tipo", ddlTipoEvaluacion.SelectedValue);
                    cmd.Parameters.AddWithValue("@fecha", fechaEvaluacion);
                    cmd.Parameters.AddWithValue("@puntuacion", puntuacion);

                    cmd.ExecuteNonQuery();

                    hdnMensaje.Value = "REDIRECT|Evaluación registrada correctamente.|ListEvaluaciones.aspx";

                    ddlEstudiante.SelectedIndex = 0;
                    ddlCurso.SelectedIndex = 0;
                    ddlTipoEvaluacion.SelectedIndex = 0;
                    txtFechaEvaluacion.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                    txtPuntuacion.Text = "";
                }
                catch (MySqlException ex)
                {
                    hdnMensaje.Value = "ERR|" + ex.Message;
                }
            }
        }
    }
}