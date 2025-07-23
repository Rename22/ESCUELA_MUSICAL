using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace Proyecto_FInal_Escuela_Musica.Especialidades
{
    public partial class ListEspecialidades : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                hdnMensaje.Value = "";
                Session["FiltroBuscar"] = null;
                CargarEspecialidades();
                UpdatePagerLabels();
            }
        }

        private void CargarEspecialidades()
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;
            string query = @"SELECT id_espec, nombre_espec, fecha_creacion_espec
                             FROM especialidades";

            string whereClause = "";

            // Aplicar filtro de búsqueda
            string filtroBuscar = Session["FiltroBuscar"] as string;
            if (!string.IsNullOrEmpty(filtroBuscar))
            {
                whereClause += " WHERE nombre_espec LIKE @filtro";
            }

            if (!string.IsNullOrEmpty(whereClause))
            {
                query += " " + whereClause;
            }

            query += " ORDER BY id_espec ASC";

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

                    gvEspecialidades.DataSource = dt;
                    gvEspecialidades.DataBind();

                    // Actualizar etiqueta de total de registros
                    lblTotalRegistros.Text = "Total: " + dt.Rows.Count + " registros";
                    UpdatePagerLabels();

                    // Ejecutar script para actualizar la etiqueta en el cliente
                    ScriptManager.RegisterStartupScript(this, GetType(), "updateTotalRecords",
                        $"updateTotalRecords({dt.Rows.Count});", true);
                }
            }
        }

        protected void gvEspecialidades_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEspecialidades.PageIndex = e.NewPageIndex;
            CargarEspecialidades();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            Session["FiltroBuscar"] = txtBuscar.Text.Trim();
            gvEspecialidades.PageIndex = 0;
            CargarEspecialidades();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "";
            Session["FiltroBuscar"] = null;
            gvEspecialidades.PageIndex = 0;
            CargarEspecialidades();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvEspecialidades.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            gvEspecialidades.PageIndex = 0;
            CargarEspecialidades();
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            int page;
            if (int.TryParse(txtGoToPage.Text, out page) && page > 0 && page <= gvEspecialidades.PageCount)
            {
                gvEspecialidades.PageIndex = page - 1;
                CargarEspecialidades();
            }
            else
            {
                hdnMensaje.Value = "ERR|Número de página inválido";
                CargarEspecialidades();
            }
        }

        protected void btnEliminarEspecialidad_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdnEspecialidadAEliminar.Value))
            {
                int id = Convert.ToInt32(hdnEspecialidadAEliminar.Value);
                string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();
                        string sql = "DELETE FROM especialidades WHERE id_espec = @id";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@id", id);
                        int rows = cmd.ExecuteNonQuery();

                        hdnMensaje.Value = rows > 0
                            ? "OK|Especialidad eliminada correctamente."
                            : "ERR|No se encontró la especialidad.";
                    }
                    catch (Exception ex)
                    {
                        hdnMensaje.Value = "ERR|" + ex.Message;
                    }
                }

                CargarEspecialidades();
            }
        }

        private void UpdatePagerLabels()
        {
            lblTotalPages.Text = gvEspecialidades.PageCount.ToString();
            txtGoToPage.Text = (gvEspecialidades.PageIndex + 1).ToString();
        }
    }
}