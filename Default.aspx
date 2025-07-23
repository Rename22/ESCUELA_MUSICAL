<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Proyecto_FInal_Escuela_Musica._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Chart.js -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

    <style>
        body {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }

        .dashboard-container {
            background: rgba(255, 255, 255, 0.95);
            backdrop-filter: blur(10px);
            border-radius: 20px;
            margin: 20px;
            padding: 30px;
            box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
        }

        .charts-container {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
            gap: 30px;
            padding: 30px 0;
        }

        .card {
            background: linear-gradient(145deg, #ffffff, #f8f9fa);
            border: none;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 
                0 10px 30px rgba(0, 0, 0, 0.1),
                inset 0 1px 0 rgba(255, 255, 255, 0.6);
            text-align: center;
            height: 420px;
            position: relative;
            overflow: hidden;
            transition: transform 0.3s ease, box-shadow 0.3s ease;
        }

        .card:hover {
            transform: translateY(-5px);
            box-shadow: 
                0 20px 40px rgba(0, 0, 0, 0.15),
                inset 0 1px 0 rgba(255, 255, 255, 0.6);
        }

        /* Bordes superiores únicos para cada gráfico */
        .card-1::before { background: linear-gradient(90deg, #FF6B6B, #FF8E53); }
        .card-2::before { background: linear-gradient(90deg, #4ECDC4, #44A08D); }
        .card-3::before { background: linear-gradient(90deg, #45B7D1, #96C93D); }
        .card-4::before { background: linear-gradient(90deg, #F093FB, #F5576C); }
        .card-5::before { background: linear-gradient(90deg, #4FACFE, #00F2FE); }
        .card-6::before { background: linear-gradient(90deg, #43E97B, #38F9D7); }
        .card-7::before { background: linear-gradient(90deg, #FA71CD, #C471F5); }
        .card-8::before { background: linear-gradient(90deg, #FD79A8, #FDCB6E); }
        .card-9::before { background: linear-gradient(90deg, #6C5CE7, #A29BFE); }
        .card-10::before { background: linear-gradient(90deg, #00B894, #00CEB4); }

        .card::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            height: 4px;
            border-radius: 15px 15px 0 0;
        }

        canvas {
            width: 100% !important;
            height: 320px !important;
            margin-top: 15px;
        }

        .chart-title {
            font-weight: 600;
            font-size: 1.2rem;
            color: #2c3e50;
            margin-bottom: 10px;
            text-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
        }
        
        h2 {
            text-align: center;
            margin: 0 0 30px 0;
            color: #2c3e50;
            font-size: 2.5rem;
            font-weight: 700;
            text-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }

        /* Responsive design */
        @media (max-width: 768px) {
            .charts-container {
                grid-template-columns: 1fr;
                gap: 20px;
                padding: 20px 0;
            }
            
            .card {
                height: 380px;
                padding: 20px;
            }
            
            canvas {
                height: 280px !important;
            }
        }
    </style>

    <div class="dashboard-container">
        <h2>Dashboard </h2>

        <div class="charts-container">
            <% for (int i = 0; i < 10; i++) { %>
                <div class="card card-<%= i + 1 %>">
                    <div class="chart-title"><%= TituloGraficos[i] %></div>
                    <canvas id='chart<%= i + 1 %>'></canvas>
                </div>
            <% } %>
        </div>
    </div>

    <script>
        const indicadores = {
            estudiantes_por_curso: <%= estudiantes_por_curso %>,
            promedio_puntuaciones_curso: <%= promedio_puntuaciones_curso %>,
            promedio_edad_curso: <%= promedio_edad_curso %>,
            cursos_por_especialidad: <%= cursos_por_especialidad %>,
            ingresos_por_curso: <%= ingresos_por_curso %>,
            estudiantes_por_especialidad: <%= estudiantes_por_especialidad %>,
            matriculados_por_mes: <%= matriculados_por_mes %>,
            ingresos_por_especialidad: <%= ingresos_por_especialidad %>,
            cursos_por_nivel: <%= cursos_por_nivel %>,
            duracion_por_nivel: <%= duracion_por_nivel %>
        };

        const titles = [
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
        ];

        // 10 tipos de gráficos diferentes
        const tiposGrafico = [
            "bar",           // 1. Barras verticales clásicas
            "horizontalBar", // 2. Barras horizontales
            "line",          // 3. Línea con puntos
            "pie",           // 4. Circular (pie)
            "doughnut",      // 5. Dona
            "polarArea",     // 6. Área polar
            "radar",         // 7. Radar/araña
            "scatter",       // 8. Dispersión
            "bubble",        // 9. Burbujas
            "bar"            // 10. Barras apiladas
        ];

        // Paletas de colores únicas para cada gráfico
        const colorPalettes = [
            // Gráfico 1 - Barras verticales (Naranja-Rojo)
            {
                background: ['rgba(255, 107, 107, 0.8)', 'rgba(255, 142, 83, 0.8)', 'rgba(255, 159, 67, 0.8)', 'rgba(255, 183, 77, 0.8)', 'rgba(255, 206, 84, 0.8)'],
                border: ['rgba(255, 107, 107, 1)', 'rgba(255, 142, 83, 1)', 'rgba(255, 159, 67, 1)', 'rgba(255, 183, 77, 1)', 'rgba(255, 206, 84, 1)']
            },
            // Gráfico 2 - Barras horizontales (Verde-Azul)
            {
                background: ['rgba(78, 205, 196, 0.8)', 'rgba(68, 160, 141, 0.8)', 'rgba(85, 239, 196, 0.8)', 'rgba(0, 184, 148, 0.8)', 'rgba(0, 206, 201, 0.8)'],
                border: ['rgba(78, 205, 196, 1)', 'rgba(68, 160, 141, 1)', 'rgba(85, 239, 196, 1)', 'rgba(0, 184, 148, 1)', 'rgba(0, 206, 201, 1)']
            },
            // Gráfico 3 - Línea (Azul cielo)
            {
                background: 'rgba(69, 183, 209, 0.2)',
                border: 'rgba(69, 183, 209, 1)',
                point: 'rgba(69, 183, 209, 1)'
            },
            // Gráfico 4 - Pie (Rosa-Morado)
            {
                background: ['rgba(240, 147, 251, 0.9)', 'rgba(245, 87, 108, 0.9)', 'rgba(250, 113, 205, 0.9)', 'rgba(196, 113, 245, 0.9)', 'rgba(168, 85, 247, 0.9)'],
                border: '#ffffff'
            },
            // Gráfico 5 - Doughnut (Azul vibrante)
            {
                background: ['rgba(79, 172, 254, 0.9)', 'rgba(0, 242, 254, 0.9)', 'rgba(116, 185, 255, 0.9)', 'rgba(162, 155, 254, 0.9)', 'rgba(67, 56, 202, 0.9)'],
                border: '#ffffff'
            },
            // Gráfico 6 - Polar Area (Verde esmeralda)
            {
                background: ['rgba(67, 233, 123, 0.7)', 'rgba(56, 249, 215, 0.7)', 'rgba(0, 184, 148, 0.7)', 'rgba(0, 206, 180, 0.7)', 'rgba(85, 239, 196, 0.7)'],
                border: ['rgba(67, 233, 123, 1)', 'rgba(56, 249, 215, 1)', 'rgba(0, 184, 148, 1)', 'rgba(0, 206, 180, 1)', 'rgba(85, 239, 196, 1)']
            },
            // Gráfico 7 - Radar (Morado-Rosa)
            {
                background: 'rgba(250, 113, 205, 0.2)',
                border: 'rgba(250, 113, 205, 1)',
                point: 'rgba(250, 113, 205, 1)'
            },
            // Gráfico 8 - Scatter (Amarillo-Naranja)
            {
                background: 'rgba(253, 203, 110, 0.8)',
                border: 'rgba(253, 203, 110, 1)',
                point: 'rgba(253, 203, 110, 1)'
            },
            // Gráfico 9 - Bubble (Morado)
            {
                background: 'rgba(108, 92, 231, 0.6)',
                border: 'rgba(108, 92, 231, 1)'
            },
            // Gráfico 10 - Barras apiladas (Verde menta)
            {
                background: ['rgba(0, 184, 148, 0.8)', 'rgba(0, 206, 180, 0.8)', 'rgba(85, 239, 196, 0.8)', 'rgba(129, 236, 236, 0.8)', 'rgba(162, 255, 178, 0.8)'],
                border: ['rgba(0, 184, 148, 1)', 'rgba(0, 206, 180, 1)', 'rgba(85, 239, 196, 1)', 'rgba(129, 236, 236, 1)', 'rgba(162, 255, 178, 1)']
            }
        ];

        Object.keys(indicadores).forEach((key, i) => {
            const ctx = document.getElementById("chart" + (i + 1)).getContext("2d");
            const data = indicadores[key];
            const chartType = tiposGrafico[i];
            const colors = colorPalettes[i];

            let chartData = {
                labels: data.labels,
                datasets: []
            };

            let chartOptions = {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'top',
                        labels: {
                            font: { size: 11, family: 'Segoe UI' },
                            color: '#2c3e50',
                            padding: 15,
                            usePointStyle: true
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(44, 62, 80, 0.9)',
                        titleColor: '#ffffff',
                        bodyColor: '#ffffff',
                        cornerRadius: 8,
                        padding: 12
                    }
                }
            };

            // Configuración específica para cada tipo de gráfico
            switch (chartType) {
                case 'bar':
                    if (i === 9) { // Gráfico 10 - Barras apiladas
                        chartData.datasets = [
                            {
                                label: 'Serie 1',
                                data: data.values.map(v => v * 0.6),
                                backgroundColor: colors.background[0],
                                borderColor: colors.border[0],
                                borderWidth: 2
                            },
                            {
                                label: 'Serie 2',
                                data: data.values.map(v => v * 0.4),
                                backgroundColor: colors.background[1],
                                borderColor: colors.border[1],
                                borderWidth: 2
                            }
                        ];
                        chartOptions.scales = {
                            x: { stacked: true, grid: { display: false } },
                            y: { stacked: true, beginAtZero: true, grid: { color: 'rgba(0, 0, 0, 0.1)' } }
                        };
                    } else { // Gráfico 1 - Barras normales
                        chartData.datasets = [{
                            label: titles[i],
                            data: data.values,
                            backgroundColor: colors.background,
                            borderColor: colors.border,
                            borderWidth: 2,
                            borderRadius: 8,
                            borderSkipped: false
                        }];
                        chartOptions.scales = {
                            y: { beginAtZero: true, grid: { color: 'rgba(0, 0, 0, 0.1)' } },
                            x: { grid: { display: false } }
                        };
                    }
                    break;

                case 'horizontalBar':
                    chartData.datasets = [{
                        label: titles[i],
                        data: data.values,
                        backgroundColor: colors.background,
                        borderColor: colors.border,
                        borderWidth: 2
                    }];
                    chartOptions.indexAxis = 'y';
                    chartOptions.scales = {
                        x: { beginAtZero: true, grid: { color: 'rgba(0, 0, 0, 0.1)' } },
                        y: { grid: { display: false } }
                    };
                    break;

                case 'line':
                    chartData.datasets = [{
                        label: titles[i],
                        data: data.values,
                        backgroundColor: colors.background,
                        borderColor: colors.border,
                        borderWidth: 3,
                        fill: true,
                        tension: 0.4,
                        pointBackgroundColor: colors.point,
                        pointBorderColor: '#ffffff',
                        pointBorderWidth: 2,
                        pointRadius: 6,
                        pointHoverRadius: 8
                    }];
                    chartOptions.scales = {
                        y: { beginAtZero: true, grid: { color: 'rgba(69, 183, 209, 0.1)' } },
                        x: { grid: { color: 'rgba(69, 183, 209, 0.1)' } }
                    };
                    break;

                case 'pie':
                case 'doughnut':
                    chartData.datasets = [{
                        data: data.values,
                        backgroundColor: colors.background,
                        borderColor: colors.border,
                        borderWidth: 2
                    }];
                    chartOptions.plugins.legend.position = 'bottom';
                    if (chartType === 'doughnut') {
                        chartOptions.cutout = '60%';
                    }
                    break;

                case 'polarArea':
                    chartData.datasets = [{
                        data: data.values,
                        backgroundColor: colors.background,
                        borderColor: colors.border,
                        borderWidth: 2
                    }];
                    chartOptions.plugins.legend.position = 'bottom';
                    chartOptions.scales = {
                        r: {
                            beginAtZero: true,
                            grid: { color: 'rgba(0, 0, 0, 0.1)' },
                            pointLabels: { color: '#2c3e50' }
                        }
                    };
                    break;

                case 'radar':
                    chartData.datasets = [{
                        label: titles[i],
                        data: data.values,
                        backgroundColor: colors.background,
                        borderColor: colors.border,
                        borderWidth: 3,
                        pointBackgroundColor: colors.point,
                        pointBorderColor: '#ffffff',
                        pointBorderWidth: 2,
                        pointRadius: 5
                    }];
                    chartOptions.scales = {
                        r: {
                            beginAtZero: true,
                            grid: { color: 'rgba(250, 113, 205, 0.2)' },
                            angleLines: { color: 'rgba(250, 113, 205, 0.2)' },
                            pointLabels: { color: '#2c3e50', font: { size: 10 } }
                        }
                    };
                    break;

                case 'scatter':
                    chartData.datasets = [{
                        label: titles[i],
                        data: data.values.map((value, index) => ({
                            x: index + 1,
                            y: value
                        })),
                        backgroundColor: colors.background,
                        borderColor: colors.border,
                        borderWidth: 2,
                        pointRadius: 8,
                        pointHoverRadius: 10
                    }];
                    chartOptions.scales = {
                        x: { type: 'linear', position: 'bottom', grid: { color: 'rgba(0, 0, 0, 0.1)' } },
                        y: { beginAtZero: true, grid: { color: 'rgba(0, 0, 0, 0.1)' } }
                    };
                    break;

                case 'bubble':
                    chartData.datasets = [{
                        label: titles[i],
                        data: data.values.map((value, index) => ({
                            x: index + 1,
                            y: value,
                            r: Math.max(5, value / 10) // Radio basado en el valor
                        })),
                        backgroundColor: colors.background,
                        borderColor: colors.border,
                        borderWidth: 2
                    }];
                    chartOptions.scales = {
                        x: { type: 'linear', position: 'bottom', grid: { color: 'rgba(0, 0, 0, 0.1)' } },
                        y: { beginAtZero: true, grid: { color: 'rgba(0, 0, 0, 0.1)' } }
                    };
                    break;
            }

            // Crear el gráfico
            new Chart(ctx, {
                type: chartType === 'horizontalBar' ? 'bar' : chartType,
                data: chartData,
                options: chartOptions
            });
        });
    </script>
</asp:Content>