using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace Proyecto_FInal_Escuela_Musica.Profesores
{
    public partial class ListProfesores : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                hdnMensaje.Value = "";
                Session["FiltroBuscar"] = null;
                CargarProfesores();
                UpdatePagerLabels();
            }
        }

        private void CargarProfesores()
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;
            string query = @"
                SELECT 
                    p.id_prof,
                    p.cedula_prof,
                    p.nombre_prof,
                    p.apellido_prof,
                    e.nombre_espec,  -- Nombre de la especialidad
                    p.telefono_prof,
                    p.email_prof
                FROM profesores p
                LEFT JOIN especialidades e ON p.id_espec = e.id_espec";

            string whereClause = "";

            // Aplicar filtro de búsqueda
            string filtroBuscar = Session["FiltroBuscar"] as string;
            if (!string.IsNullOrEmpty(filtroBuscar))
            {
                whereClause += " (cedula_prof LIKE @filtro OR nombre_prof LIKE @filtro OR apellido_prof LIKE @filtro)";
            }

            if (!string.IsNullOrEmpty(whereClause))
            {
                query += " WHERE " + whereClause;
            }

            query += " ORDER BY id_prof DESC";

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

                    gvProfesores.DataSource = dt;
                    gvProfesores.DataBind();

                    // Actualizar etiqueta de total de registros
                    lblTotalRegistros.Text = "Total: " + dt.Rows.Count + " registros";
                    UpdatePagerLabels();

                    // Ejecutar script para actualizar la etiqueta en el cliente
                    ScriptManager.RegisterStartupScript(this, GetType(), "updateTotalRecords",
                        $"updateTotalRecords({dt.Rows.Count});", true);
                }
            }
        }

        protected void gvProfesores_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProfesores.PageIndex = e.NewPageIndex;
            CargarProfesores();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            Session["FiltroBuscar"] = txtBuscar.Text.Trim();
            gvProfesores.PageIndex = 0;
            CargarProfesores();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "";
            Session["FiltroBuscar"] = null;
            gvProfesores.PageIndex = 0;
            CargarProfesores();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvProfesores.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            gvProfesores.PageIndex = 0;
            CargarProfesores();
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            int page;
            if (int.TryParse(txtGoToPage.Text, out page) && page > 0 && page <= gvProfesores.PageCount)
            {
                gvProfesores.PageIndex = page - 1;
                CargarProfesores();
            }
            else
            {
                hdnMensaje.Value = "ERR|Número de página inválido";
                CargarProfesores();
            }
        }

        protected void btnEliminarProfesor_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdnProfesorAEliminar.Value))
            {
                int id = Convert.ToInt32(hdnProfesorAEliminar.Value);
                string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();
                        string sql = "DELETE FROM profesores WHERE id_prof = @id";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@id", id);
                        int rows = cmd.ExecuteNonQuery();

                        hdnMensaje.Value = rows > 0
                            ? "OK|Profesor eliminado correctamente."
                            : "ERR|No se encontró el profesor.";
                    }
                    catch (MySqlException ex)
                    {
                        // Manejar error de FK (profesor con cursos asignados)
                        if (ex.Number == 1451) // MySQL error code for foreign key constraint
                        {
                            hdnMensaje.Value = "ERR|No se puede eliminar el profesor porque tiene cursos asignados";
                        }
                        else
                        {
                            hdnMensaje.Value = "ERR|" + ex.Message;
                        }
                    }
                    catch (Exception ex)
                    {
                        hdnMensaje.Value = "ERR|" + ex.Message;
                    }
                }

                CargarProfesores();
            }
        }

        private void UpdatePagerLabels()
        {
            lblTotalPages.Text = gvProfesores.PageCount.ToString();
            txtGoToPage.Text = (gvProfesores.PageIndex + 1).ToString();
        }
    }
}