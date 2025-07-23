using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace Proyecto_FInal_Escuela_Musica.Matriculas
{
    public partial class ListMatriculas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                hdnMensaje.Value = "";
                Session["FiltroBuscar"] = null;
                CargarMatriculas();
                UpdatePagerLabels();
            }
        }

        private void CargarMatriculas()
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;
            string query = @"SELECT m.id_mat, 
                                    e.nombre_estu, 
                                    e.apellido_estu, 
                                    c.nombre_cur, 
                                    m.fecha_matricula_mat, 
                                    m.fecha_inicio_mat, 
                                    m.fecha_fin_mat 
                             FROM matriculas m
                             INNER JOIN estudiantes e ON m.id_estu = e.id_estu
                             INNER JOIN cursos c ON m.id_cur = c.id_cur";

            string whereClause = "";

            // Aplicar filtro de búsqueda
            string filtroBuscar = Session["FiltroBuscar"] as string;
            if (!string.IsNullOrEmpty(filtroBuscar))
            {
                whereClause += " WHERE (e.nombre_estu LIKE @filtro OR e.apellido_estu LIKE @filtro OR c.nombre_cur LIKE @filtro)";
            }

            if (!string.IsNullOrEmpty(whereClause))
            {
                query += " " + whereClause;
            }

            query += " ORDER BY m.fecha_matricula_mat DESC";

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

                    gvMatriculas.DataSource = dt;
                    gvMatriculas.DataBind();

                    // Actualizar etiqueta de total de registros
                    lblTotalRegistros.Text = "Total: " + dt.Rows.Count + " registros";
                    UpdatePagerLabels();

                    // Ejecutar script para actualizar la etiqueta en el cliente
                    ScriptManager.RegisterStartupScript(this, GetType(), "updateTotalRecords",
                        $"updateTotalRecords({dt.Rows.Count});", true);
                }
            }
        }

        protected void gvMatriculas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMatriculas.PageIndex = e.NewPageIndex;
            CargarMatriculas();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            Session["FiltroBuscar"] = txtBuscar.Text.Trim();
            gvMatriculas.PageIndex = 0;
            CargarMatriculas();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "";
            Session["FiltroBuscar"] = null;
            gvMatriculas.PageIndex = 0;
            CargarMatriculas();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvMatriculas.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            gvMatriculas.PageIndex = 0;
            CargarMatriculas();
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            int page;
            if (int.TryParse(txtGoToPage.Text, out page) && page > 0 && page <= gvMatriculas.PageCount)
            {
                gvMatriculas.PageIndex = page - 1;
                CargarMatriculas();
            }
            else
            {
                hdnMensaje.Value = "ERR|Número de página inválido";
                CargarMatriculas();
            }
        }

        protected void btnEliminarMatricula_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdnMatriculaAEliminar.Value))
            {
                int id = Convert.ToInt32(hdnMatriculaAEliminar.Value);
                string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();
                        string sql = "DELETE FROM matriculas WHERE id_mat = @id";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@id", id);
                        int rows = cmd.ExecuteNonQuery();

                        hdnMensaje.Value = rows > 0
                            ? "OK|Matrícula eliminada correctamente."
                            : "ERR|No se encontró la matrícula.";
                    }
                    catch (Exception ex)
                    {
                        hdnMensaje.Value = "ERR|" + ex.Message;
                    }
                }

                CargarMatriculas();
            }
        }

        private void UpdatePagerLabels()
        {
            lblTotalPages.Text = gvMatriculas.PageCount.ToString();
            txtGoToPage.Text = (gvMatriculas.PageIndex + 1).ToString();
        }
    }
}