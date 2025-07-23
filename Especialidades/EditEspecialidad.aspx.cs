using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using MySql.Data.MySqlClient;

namespace Proyecto_FInal_Escuela_Musica.Especialidades
{
    public partial class EditEspecialidad : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hdnMensaje.Value = ""; // Limpiar mensaje al cargar

            if (!IsPostBack)
            {
                // Obtener el id de la especialidad de la URL
                string idEspecialidad = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(idEspecialidad))
                {
                    CargarEspecialidad(idEspecialidad);
                }
                else
                {
                    // Redirigir si no hay id
                    Response.Redirect("ListEspecialidades.aspx");
                }
            }
        }

        private void CargarEspecialidad(string id)
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;
            string query = "SELECT id_espec, nombre_espec, descripcion_espec FROM especialidades WHERE id_espec = @id";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    try
                    {
                        conn.Open();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hdnIdEspecialidad.Value = reader["id_espec"].ToString();
                                txtNombre.Text = reader["nombre_espec"].ToString();
                                txtDescripcion.Text = reader["descripcion_espec"].ToString();
                            }
                            else
                            {
                                // No se encontró la especialidad
                                hdnMensaje.Value = "ERR|Especialidad no encontrada";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        hdnMensaje.Value = "ERR|Error al cargar: " + ex.Message;
                    }
                }
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string descripcion = txtDescripcion.Text.Trim();
            string idEspecialidad = hdnIdEspecialidad.Value;

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

                    // Verificar si ya existe otro registro con el mismo nombre (excluyendo el actual)
                    string checkSql = "SELECT COUNT(*) FROM especialidades WHERE nombre_espec = @nombre AND id_espec != @id";
                    MySqlCommand checkCmd = new MySqlCommand(checkSql, conn);
                    checkCmd.Parameters.AddWithValue("@nombre", nombre);
                    checkCmd.Parameters.AddWithValue("@id", idEspecialidad);

                    int existe = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (existe > 0)
                    {
                        hdnMensaje.Value = "WARN|Ya existe otra especialidad con ese nombre";
                        return;
                    }

                    // Actualizar la especialidad
                    string updateSql = @"UPDATE especialidades 
                        SET nombre_espec = @nombre, 
                            descripcion_espec = @descripcion
                        WHERE id_espec = @id";

                    MySqlCommand updateCmd = new MySqlCommand(updateSql, conn);
                    updateCmd.Parameters.AddWithValue("@nombre", nombre);
                    updateCmd.Parameters.AddWithValue("@descripcion",
                        string.IsNullOrEmpty(descripcion) ? DBNull.Value : (object)descripcion);
                    updateCmd.Parameters.AddWithValue("@id", idEspecialidad);

                    int rowsAffected = updateCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Mensaje con redirección
                        hdnMensaje.Value = "REDIRECT|Especialidad actualizada correctamente.|ListEspecialidades.aspx";
                    }
                    else
                    {
                        hdnMensaje.Value = "ERR|No se pudo actualizar la especialidad. Verifique los datos.";
                    }
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