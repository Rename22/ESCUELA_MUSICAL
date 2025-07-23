using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace Proyecto_FInal_Escuela_Musica.Evaluaciones
{
    public partial class ListEvaluaciones : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                hdnMensaje.Value = "";
                Session["FiltroBuscar"] = null;
                CargarEvaluaciones();
                UpdatePagerLabels();
            }
        }

        private void CargarEvaluaciones()
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;
            string query = @"SELECT e.id_eva, 
                                    est.nombre_estu, 
                                    est.apellido_estu, 
                                    c.nombre_cur, 
                                    e.tipo_eva, 
                                    e.fecha_eva, 
                                    e.puntuacion_eva
                             FROM evaluaciones e
                             INNER JOIN estudiantes est ON e.id_estu = est.id_estu
                             INNER JOIN cursos c ON e.id_cur = c.id_cur";

            string whereClause = "";

            // Aplicar filtro de búsqueda
            string filtroBuscar = Session["FiltroBuscar"] as string;
            if (!string.IsNullOrEmpty(filtroBuscar))
            {
                whereClause += " WHERE (est.nombre_estu LIKE @filtro OR est.apellido_estu LIKE @filtro OR c.nombre_cur LIKE @filtro OR e.tipo_eva LIKE @filtro)";
            }

            if (!string.IsNullOrEmpty(whereClause))
            {
                query += " " + whereClause;
            }

            query += " ORDER BY e.fecha_eva DESC";

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

                    gvEvaluaciones.DataSource = dt;
                    gvEvaluaciones.DataBind();

                    // Actualizar etiqueta de total de registros
                    lblTotalRegistros.Text = "Total: " + dt.Rows.Count + " registros";
                    UpdatePagerLabels();

                    // Ejecutar script para actualizar la etiqueta en el cliente
                    ScriptManager.RegisterStartupScript(this, GetType(), "updateTotalRecords",
                        $"updateTotalRecords({dt.Rows.Count});", true);
                }
            }
        }

        protected void gvEvaluaciones_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEvaluaciones.PageIndex = e.NewPageIndex;
            CargarEvaluaciones();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            Session["FiltroBuscar"] = txtBuscar.Text.Trim();
            gvEvaluaciones.PageIndex = 0;
            CargarEvaluaciones();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "";
            Session["FiltroBuscar"] = null;
            gvEvaluaciones.PageIndex = 0;
            CargarEvaluaciones();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvEvaluaciones.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            gvEvaluaciones.PageIndex = 0;
            CargarEvaluaciones();
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            int page;
            if (int.TryParse(txtGoToPage.Text, out page) && page > 0 && page <= gvEvaluaciones.PageCount)
            {
                gvEvaluaciones.PageIndex = page - 1;
                CargarEvaluaciones();
            }
            else
            {
                hdnMensaje.Value = "ERR|Número de página inválido";
                CargarEvaluaciones();
            }
        }

        protected void btnEliminarEvaluacion_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdnEvaluacionAEliminar.Value))
            {
                int id = Convert.ToInt32(hdnEvaluacionAEliminar.Value);
                string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();
                        string sql = "DELETE FROM evaluaciones WHERE id_eva = @id";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@id", id);
                        int rows = cmd.ExecuteNonQuery();

                        hdnMensaje.Value = rows > 0
                            ? "OK|Evaluación eliminada correctamente."
                            : "ERR|No se encontró la evaluación.";
                    }
                    catch (Exception ex)
                    {
                        hdnMensaje.Value = "ERR|" + ex.Message;
                    }
                }

                CargarEvaluaciones();
            }
        }

        private void UpdatePagerLabels()
        {
            lblTotalPages.Text = gvEvaluaciones.PageCount.ToString();
            txtGoToPage.Text = (gvEvaluaciones.PageIndex + 1).ToString();
        }
    }
}