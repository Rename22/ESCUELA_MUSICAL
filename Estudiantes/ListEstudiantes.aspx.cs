using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace Proyecto_FInal_Escuela_Musica.Estudiantes
{
    public partial class ListEstudiantes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                hdnMensaje.Value = "";
                Session["FiltroBuscar"] = null;
                Session["FiltroEstado"] = null;
                CargarEstudiantes();
                UpdatePagerLabels();
            }
        }

        private void CargarEstudiantes()
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;
            string query = "SELECT * FROM estudiantes";
            string whereClause = "";

            // Aplicar filtro de búsqueda
            string filtroBuscar = Session["FiltroBuscar"] as string;
            if (!string.IsNullOrEmpty(filtroBuscar))
            {
                whereClause += " (cedula_estu LIKE @filtro OR nombre_estu LIKE @filtro OR apellido_estu LIKE @filtro)";
            }

            // Aplicar filtro de estado (si existiera)
            string filtroEstado = Session["FiltroEstado"] as string;
            if (!string.IsNullOrEmpty(filtroEstado))
            {
                if (!string.IsNullOrEmpty(whereClause)) whereClause += " AND ";
                whereClause += " estado = @estado";
            }

            if (!string.IsNullOrEmpty(whereClause))
            {
                query += " WHERE " + whereClause;
            }

            query += " ORDER BY id_estu DESC";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter(query, conn))
                {
                    if (!string.IsNullOrEmpty(filtroBuscar))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@filtro", $"%{filtroBuscar}%");
                    }


                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvEstudiantes.DataSource = dt;
                    gvEstudiantes.DataBind();

                    // Actualizar etiqueta de total de registros
                    lblTotalRegistros.Text = "Total: " + dt.Rows.Count + " registros";
                    UpdatePagerLabels();

                    // Ejecutar script para actualizar la etiqueta en el cliente
                    ScriptManager.RegisterStartupScript(this, GetType(), "updateTotalRecords",
                        $"updateTotalRecords({dt.Rows.Count});", true);
                }
            }
        }

        protected void gvEstudiantes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEstudiantes.PageIndex = e.NewPageIndex;
            CargarEstudiantes();
        }

        protected void gvEstudiantes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(gvEstudiantes.DataKeys[e.RowIndex].Value);
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = "DELETE FROM estudiantes WHERE id_estu = @id";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    int rows = cmd.ExecuteNonQuery();

                    hdnMensaje.Value = rows > 0
                        ? "OK|Estudiante eliminado correctamente."
                        : "ERR|No se encontró el estudiante.";
                }
                catch (Exception ex)
                {
                    hdnMensaje.Value = "ERR|" + ex.Message;
                }
            }

            CargarEstudiantes();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            Session["FiltroBuscar"] = txtBuscar.Text.Trim();
            gvEstudiantes.PageIndex = 0;
            CargarEstudiantes();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "";
            Session["FiltroBuscar"] = null;
            gvEstudiantes.PageIndex = 0;
            CargarEstudiantes();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvEstudiantes.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            gvEstudiantes.PageIndex = 0;
            CargarEstudiantes();
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            int page;
            if (int.TryParse(txtGoToPage.Text, out page) && page > 0 && page <= gvEstudiantes.PageCount)
            {
                gvEstudiantes.PageIndex = page - 1;
                CargarEstudiantes();
            }
            else
            {
                hdnMensaje.Value = "ERR|Número de página inválido";
                CargarEstudiantes();
            }
        }
        protected void btnEliminarEstudiante_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdnEstudianteAEliminar.Value))
            {
                int id = Convert.ToInt32(hdnEstudianteAEliminar.Value);
                string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();
                        string sql = "DELETE FROM estudiantes WHERE id_estu = @id";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@id", id);
                        int rows = cmd.ExecuteNonQuery();

                        hdnMensaje.Value = rows > 0
                            ? "OK|Estudiante eliminado correctamente."
                            : "ERR|No se encontró el estudiante.";
                    }
                    catch (Exception ex)
                    {
                        hdnMensaje.Value = "ERR|" + ex.Message;
                    }
                }

                CargarEstudiantes();
            }
        }

        private void UpdatePagerLabels()
        {
            lblTotalPages.Text = gvEstudiantes.PageCount.ToString();
            txtGoToPage.Text = (gvEstudiantes.PageIndex + 1).ToString();
        }
    }
}