using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.UI;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace Proyecto_FInal_Escuela_Musica
{
    public partial class _Default : Page
    {
        // Títulos de los gráficos
        public string[] TituloGraficos = new string[]
        {
            "Estudiantes por Curso (Top 15)",
            "Promedio Puntuaciones por Curso",
            "Promedio Edad por Curso",
            "Cursos por Especialidad",
            "Ingresos por Curso (Top 20)",
            "Estudiantes por Especialidad",
            "Matriculados por Mes",
            "Ingresos por Especialidad",
            "Cursos por Nivel",
            "Duración Promedio por Nivel"
        };

        // Variables JSON para cada indicador
        public string estudiantes_por_curso, promedio_puntuaciones_curso, promedio_edad_curso,
                      cursos_por_especialidad, ingresos_por_curso, estudiantes_por_especialidad,
                      matriculados_por_mes, ingresos_por_especialidad, cursos_por_nivel,
                      duracion_por_nivel;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                estudiantes_por_curso = GetIndicadorJson("estudiantes_por_curso");
                promedio_puntuaciones_curso = GetIndicadorJson("promedio_puntuaciones_curso");
                promedio_edad_curso = GetIndicadorJson("promedio_edad_curso");
                cursos_por_especialidad = GetIndicadorJson("cursos_por_especialidad");
                ingresos_por_curso = GetIndicadorJson("ingresos_por_curso");
                estudiantes_por_especialidad = GetIndicadorJson("estudiantes_por_especialidad");
                matriculados_por_mes = GetIndicadorJson("matriculados_por_mes");
                ingresos_por_especialidad = GetIndicadorJson("ingresos_por_especialidad");
                cursos_por_nivel = GetIndicadorJson("cursos_por_nivel");
                duracion_por_nivel = GetIndicadorJson("duracion_por_nivel");
            }
        }

        private string GetIndicadorJson(string indicador)
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConexionBdd"].ConnectionString;
            string query = "";

            switch (indicador)
            {
                case "estudiantes_por_curso":
                    query = @"SELECT c.nombre_cur, COUNT(m.id_estu) AS numero_estudiantes
                              FROM cursos c
                              LEFT JOIN matriculas m ON c.id_cur = m.id_cur
                              GROUP BY c.id_cur 
                              ORDER BY numero_estudiantes DESC
                              LIMIT 5";
                    break;

                case "promedio_puntuaciones_curso":
                    query = @"SELECT c.nombre_cur, COALESCE(AVG(e.puntuacion_eva), 0) AS promedio_puntuacion
                              FROM cursos c
                              LEFT JOIN evaluaciones e ON c.id_cur = e.id_cur
                              GROUP BY c.id_cur
                              ORDER BY promedio_puntuacion DESC
                              LIMIT 5";
                    break;

                case "promedio_edad_curso":
                    query = @"SELECT c.nombre_cur, AVG(TIMESTAMPDIFF(YEAR, e.fecha_nacimiento_estu, CURDATE())) AS promedio_edad
                              FROM cursos c
                              JOIN matriculas m ON c.id_cur = m.id_cur
                              JOIN estudiantes e ON m.id_estu = e.id_estu
                              GROUP BY c.id_cur
                              ORDER BY promedio_edad DESC
                              LIMIT 5";
                    break;

                case "cursos_por_especialidad":
                    query = @"SELECT es.nombre_espec, COUNT(c.id_cur) AS numero_cursos
                              FROM especialidades es
                              LEFT JOIN profesores p ON es.id_espec = p.id_espec
                              LEFT JOIN cursos c ON p.id_prof = c.id_prof
                              GROUP BY es.id_espec";
                    break;

                case "ingresos_por_curso":
                    query = @"SELECT c.nombre_cur, SUM(c.costo_total_cur) AS total_ingresos
                              FROM cursos c
                              LEFT JOIN matriculas m ON c.id_cur = m.id_cur
                              GROUP BY c.id_cur
                              ORDER BY total_ingresos DESC
                              LIMIT 5";
                    break;

                case "estudiantes_por_especialidad":
                    query = @"SELECT es.nombre_espec, COUNT(m.id_estu) AS numero_estudiantes
                              FROM especialidades es
                              LEFT JOIN profesores p ON es.id_espec = p.id_espec
                              LEFT JOIN cursos c ON p.id_prof = c.id_prof
                              LEFT JOIN matriculas m ON c.id_cur = m.id_cur
                              GROUP BY es.id_espec";
                    break;

                case "matriculados_por_mes":
                    query = @"SELECT MONTHNAME(fecha_matricula_mat) AS mes, COUNT(id_estu) AS numero_estudiantes
                              FROM matriculas
                              GROUP BY MONTH(fecha_matricula_mat), mes
                              ORDER BY MONTH(fecha_matricula_mat)";
                    break;

                case "ingresos_por_especialidad":
                    query = @"SELECT es.nombre_espec, COALESCE(SUM(c.costo_total_cur), 0) AS total_ingresos
                              FROM especialidades es
                              LEFT JOIN profesores p ON es.id_espec = p.id_espec
                              LEFT JOIN cursos c ON p.id_prof = c.id_prof
                              GROUP BY es.id_espec";
                    break;

                case "cursos_por_nivel":
                    query = @"SELECT nivel_cur, COUNT(id_cur) AS numero_cursos
                              FROM cursos
                              GROUP BY nivel_cur";
                    break;

                case "duracion_por_nivel":
                    query = @"SELECT nivel_cur, AVG(duracion_semanas_cur) AS promedio_duracion
                              FROM cursos
                              GROUP BY nivel_cur";
                    break;

                default:
                    return "{\"labels\":[],\"values\":[]}";
            }

            return EjecutarConsultaJson(connStr, query);
        }

        private string EjecutarConsultaJson(string connStr, string query)
        {
            var labels = new List<string>();
            var values = new List<decimal>();

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            labels.Add(reader[0].ToString());
                            values.Add(reader[1] != DBNull.Value ? Convert.ToDecimal(reader[1]) : 0);
                        }
                    }
                }
            }

            // Redondear valores decimales
            for (int i = 0; i < values.Count; i++)
            {
                values[i] = Math.Round(values[i], 2);
            }

            var resultado = new
            {
                labels = labels,
                values = values
            };

            return JsonConvert.SerializeObject(resultado);
        }
    }
}