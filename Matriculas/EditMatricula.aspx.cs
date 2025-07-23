using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace Proyecto_FInal_Escuela_Musica.Matriculas
{
    public partial class EditMatricula : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hdnMensaje.Value = "";

            if (!IsPostBack)
            {
                string matriculaId = Request.QueryString["id"];
                if (string.IsNullOrEmpty(matriculaId))
                {
                    hdnMensaje.Value = "ERR|ID de matrícula no especificado";
                    return;
                }

                hdnIdMatricula.Value = matriculaId;
                CargarEstudiantes();
                CargarCursos();
                CargarMatricula(matriculaId);
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

        private void CargarMatricula(string matriculaId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;
            string query = @"SELECT m.id_mat, m.id_estu, m.id_cur, 
                            DATE_FORMAT(m.fecha_inicio_mat, '%Y-%m-%d') AS fecha_inicio, 
                            DATE_FORMAT(m.fecha_fin_mat, '%Y-%m-%d') AS fecha_fin
                            FROM matriculas m
                            WHERE m.id_mat = @idMatricula";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idMatricula", matriculaId);

                    try
                    {
                        conn.Open();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ddlEstudiante.SelectedValue = reader["id_estu"].ToString();
                                ddlCurso.SelectedValue = reader["id_cur"].ToString();
                                txtFechaInicio.Text = reader["fecha_inicio"].ToString();
                                txtFechaFin.Text = reader["fecha_fin"].ToString();
                            }
                            else
                            {
                                hdnMensaje.Value = "ERR|Matrícula no encontrada";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        hdnMensaje.Value = "ERR|Error al cargar matrícula: " + ex.Message;
                    }
                }
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            DateTime minDate = new DateTime(2000, 1, 1);
            DateTime fechaInicio;
            DateTime? fechaFin = null;

            if (!DateTime.TryParse(txtFechaInicio.Text, out fechaInicio) || fechaInicio < minDate)
            {
                hdnMensaje.Value = "ERR|Fecha de inicio inválida. Debe ser posterior al 01/01/2000";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFechaFin.Text))
            {
                hdnMensaje.Value = "ERR|La fecha de finalización es obligatoria";
                return;
            }
            else
            {
                DateTime temp;
                if (DateTime.TryParse(txtFechaFin.Text, out temp))
                {
                    if (temp < minDate)
                    {
                        hdnMensaje.Value = "ERR|Fecha final inválida. Debe ser posterior al 01/01/2000";
                        return;
                    }
                    fechaFin = temp;
                }
                else
                {
                    hdnMensaje.Value = "ERR|Formato de fecha final inválido";
                    return;
                }

                if (fechaFin < fechaInicio)
                {
                    hdnMensaje.Value = "ERR|La fecha final debe ser posterior a la fecha de inicio";
                    return;
                }
            }

            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = @"UPDATE matriculas 
                                   SET id_estu = @idEstudiante, 
                                       id_cur = @idCurso, 
                                       fecha_inicio_mat = @fechaInicio, 
                                       fecha_fin_mat = @fechaFin 
                                   WHERE id_mat = @idMatricula";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@idEstudiante", Convert.ToInt32(ddlEstudiante.SelectedValue));
                    cmd.Parameters.AddWithValue("@idCurso", Convert.ToInt32(ddlCurso.SelectedValue));
                    cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@fechaFin", fechaFin.Value);
                    cmd.Parameters.AddWithValue("@idMatricula", hdnIdMatricula.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        hdnMensaje.Value = "REDIRECT|Matrícula actualizada correctamente.|ListMatriculas.aspx";
                    }
                    else
                    {
                        hdnMensaje.Value = "ERR|No se pudo actualizar la matrícula. Registro no encontrado";
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