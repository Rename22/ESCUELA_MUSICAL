using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace Proyecto_FInal_Escuela_Musica.Cursos
{
    public partial class ListCursos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                hdnMensaje.Value = "";
                Session["FiltroBuscar"] = null;
                CargarCursos();
                UpdatePagerLabels();
            }
        }

        private void CargarCursos()
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;
            string query = @"SELECT c.id_cur, c.nombre_cur, c.descripcion_cur, c.nivel_cur, 
                                    c.duracion_semanas_cur, c.costo_total_cur, 
                                    CONCAT(p.nombre_prof, ' ', p.apellido_prof) AS nombre_prof 
                             FROM cursos c
                             LEFT JOIN profesores p ON c.id_prof = p.id_prof"; // Modificado con CONCAT para nombre y apellido

            string whereClause = "";

            // Aplicar filtro de búsqueda
            string filtroBuscar = Session["FiltroBuscar"] as string;
            if (!string.IsNullOrEmpty(filtroBuscar))
            {
                whereClause += " WHERE (c.nombre_cur LIKE @filtro OR c.descripcion_cur LIKE @filtro OR c.nivel_cur LIKE @filtro)";
            }

            if (!string.IsNullOrEmpty(whereClause))
            {
                query += " " + whereClause;
            }

            query += " ORDER BY c.id_cur DESC";

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

                    gvCursos.DataSource = dt;
                    gvCursos.DataBind();

                    // Actualizar etiqueta de total de registros
                    lblTotalRegistros.Text = "Total: " + dt.Rows.Count + " registros";
                    UpdatePagerLabels();

                    // Ejecutar script para actualizar la etiqueta en el cliente
                    ScriptManager.RegisterStartupScript(this, GetType(), "updateTotalRecords",
                        $"updateTotalRecords({dt.Rows.Count});", true);
                }
            }
        }

        protected void gvCursos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCursos.PageIndex = e.NewPageIndex;
            CargarCursos();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            Session["FiltroBuscar"] = txtBuscar.Text.Trim();
            gvCursos.PageIndex = 0;
            CargarCursos();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "";
            Session["FiltroBuscar"] = null;
            gvCursos.PageIndex = 0;
            CargarCursos();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvCursos.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            gvCursos.PageIndex = 0;
            CargarCursos();
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            int page;
            if (int.TryParse(txtGoToPage.Text, out page) && page > 0 && page <= gvCursos.PageCount)
            {
                gvCursos.PageIndex = page - 1;
                CargarCursos();
            }
            else
            {
                hdnMensaje.Value = "ERR|Número de página inválido";
                CargarCursos();
            }
        }

        protected void btnEliminarCurso_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdnCursoAEliminar.Value))
            {
                int id = Convert.ToInt32(hdnCursoAEliminar.Value);
                string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();
                        string sql = "DELETE FROM cursos WHERE id_cur = @id";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@id", id);
                        int rows = cmd.ExecuteNonQuery();

                        hdnMensaje.Value = rows > 0
                            ? "OK|Curso eliminado correctamente."
                            : "ERR|No se encontró el curso.";
                    }
                    catch (Exception ex)
                    {
                        hdnMensaje.Value = "ERR|" + ex.Message;
                    }
                }

                CargarCursos();
            }
        }

        private void UpdatePagerLabels()
        {
            lblTotalPages.Text = gvCursos.PageCount.ToString();
            txtGoToPage.Text = (gvCursos.PageIndex + 1).ToString();
        }
    }
}
