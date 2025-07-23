using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace Proyecto_FInal_Escuela_Musica.Profesores
{
    public partial class AddProfesor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hdnMensaje.Value = ""; // limpiar mensaje al cargar

            if (!IsPostBack)
            {
                CargarEspecialidades();
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

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = @"INSERT INTO profesores 
                        (cedula_prof, nombre_prof, apellido_prof, id_espec, telefono_prof, email_prof) 
                        VALUES 
                        (@cedula, @nombre, @apellido, @id_espec, @telefono, @email)";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@cedula", txtCedula.Text.Trim());
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@apellido", txtApellido.Text.Trim());
                    cmd.Parameters.AddWithValue("@id_espec", ddlEspecialidad.SelectedValue);
                    cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());

                    cmd.ExecuteNonQuery();

                    // Mensaje con redirección
                    hdnMensaje.Value = "REDIRECT|Profesor agregado correctamente.|ListProfesores.aspx";

                    // Limpiar campos
                    txtCedula.Text = "";
                    txtNombre.Text = "";
                    txtApellido.Text = "";
                    ddlEspecialidad.SelectedIndex = 0;
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