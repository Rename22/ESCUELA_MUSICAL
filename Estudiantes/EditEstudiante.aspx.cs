using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace Proyecto_FInal_Escuela_Musica.Estudiantes
{
    public partial class EditEstudiante : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                hdnMensaje.Value = "";
                hdnRedirect.Value = "";
                string id = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(id))
                {
                    hdnId.Value = id;
                    CargarEstudiante(id);
                }
                else
                {
                    Response.Redirect("ListEstudiantes.aspx");
                }
            }
        }

        private void CargarEstudiante(string id)
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = "SELECT * FROM estudiantes WHERE id_estu = @id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtCedula.Text = reader["cedula_estu"].ToString();
                        txtNombre.Text = reader["nombre_estu"].ToString();
                        txtApellido.Text = reader["apellido_estu"].ToString();
                        txtFechaNacimiento.Text = Convert.ToDateTime(reader["fecha_nacimiento_estu"]).ToString("yyyy-MM-dd");
                        txtTelefono.Text = reader["telefono_estu"].ToString();
                        txtEmail.Text = reader["email_estu"].ToString();
                    }
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
                    string sql = @"UPDATE estudiantes SET
                        cedula_estu = @cedula,
                        nombre_estu = @nombre,
                        apellido_estu = @apellido,
                        fecha_nacimiento_estu = @fecha_nac,
                        telefono_estu = @telefono,
                        email_estu = @email
                        WHERE id_estu = @id";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@cedula", txtCedula.Text.Trim());
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@apellido", txtApellido.Text.Trim());
                    cmd.Parameters.AddWithValue("@fecha_nac", Convert.ToDateTime(txtFechaNacimiento.Text));
                    cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", hdnId.Value);

                    cmd.ExecuteNonQuery();

                    // Configurar mensaje y redirección
                    hdnMensaje.Value = "OK|Estudiante actualizado correctamente.";
                    hdnRedirect.Value = "ListEstudiantes.aspx";
                }
                catch (MySqlException ex)
                {
                    hdnMensaje.Value = "ERR|" + ex.Message;
                }
            }
        }
    }
}