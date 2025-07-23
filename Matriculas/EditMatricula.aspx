<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditMatricula.aspx.cs" Inherits="Proyecto_FInal_Escuela_Musica.Matriculas.EditMatricula" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        :root {
            --primary-color: #4e73df;
            --secondary-color: #858796;
            --success-color: #1cc88a;
            --warning-color: #f6c23e;
            --danger-color: #e74a3b;
            --light-color: #f8f9fc;
            --dark-color: #5a5c69;
            --shadow: 0 0.15rem 1.75rem 0 rgba(58, 59, 69, 0.15);
        }
        
        .card {
            border-radius: 0.5rem;
            box-shadow: var(--shadow);
            border: none;
            margin-bottom: 2rem;
            background-color: #fff;
        }
        
        .card-header {
            background-color: #f8f9fc;
            border-bottom: 1px solid #e3e6f0;
            padding: 1.25rem 1.5rem;
            border-radius: 0.5rem 0.5rem 0 0 !important;
            display: flex;
            flex-wrap: wrap;
            justify-content: space-between;
            align-items: center;
        }
        
        .card-title {
            font-weight: 600;
            color: var(--dark-color);
            margin-bottom: 0;
            font-size: 1.5rem;
        }
        
        .btn-primary {
            background-color: var(--primary-color);
            border-color: var(--primary-color);
            transition: all 0.2s ease;
        }
        
        .btn-primary:hover {
            background-color: #2e59d9;
            border-color: #2e59d9;
            transform: translateY(-2px);
        }
        
        .form-control:focus {
            border-color: var(--primary-color);
            box-shadow: 0 0 0 0.2rem rgba(78, 115, 223, 0.25);
        }
        
        .form-label {
            font-weight: 600;
            color: var(--dark-color);
            margin-bottom: 0.5rem;
        }
        
        .form-section {
            padding: 1.5rem;
            border-bottom: 1px solid #e3e6f0;
        }
        
        .form-section:last-child {
            border-bottom: none;
        }
        
        .form-actions {
            padding: 1.5rem;
            background-color: #f8f9fc;
            border-top: 1px solid #e3e6f0;
            border-radius: 0 0 0.5rem 0.5rem;
            display: flex;
            justify-content: flex-end;
        }
        
        .card-body {
            padding: 0;
        }
        
        .error-message {
            display: block;
            margin-top: 0.25rem;
            font-size: 0.875rem;
            color: var(--danger-color);
        }
        
        .input-group-text {
            background-color: #eaecf4;
            color: var(--dark-color);
            border: 1px solid #d1d3e2;
        }
        
        .form-control.is-invalid {
            border-color: var(--danger-color);
            padding-right: calc(1.5em + 0.75rem);
            background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' fill='none' stroke='%23e74a3b' viewBox='0 0 12 12'%3e%3ccircle cx='6' cy='6' r='4.5'/%3e%3cpath stroke-linejoin='round' d='M5.8 3.6h.4L6 6.5z'/%3e%3ccircle cx='6' cy='8.2' r='.6' fill='%23e74a3b' stroke='none'/%3e%3c/svg%3e");
            background-repeat: no-repeat;
            background-position: right calc(0.375em + 0.1875rem) center;
            background-size: calc(0.75em + 0.375rem) calc(0.75em + 0.375rem);
        }
        
        .form-control.is-invalid:focus {
            box-shadow: 0 0 0 0.2rem rgba(231, 74, 59, 0.25);
        }
        
        @media (max-width: 768px) {
            .form-actions {
                flex-direction: column;
                gap: 0.5rem;
            }
            
            .btn {
                width: 100%;
            }
        }
    </style>

    <div class="container mt-4">
        <div class="card">
            <div class="card-header">
                <h3 class="card-title">Editar Matrícula</h3>
            </div>
            
            <div class="card-body">
                <asp:HiddenField ID="hdnMensaje" runat="server" />
                <asp:HiddenField ID="hdnIdMatricula" runat="server" />
                
                <!-- Sección de estudiante -->
                <div class="form-section">
                    <h5 class="mb-4 text-primary"><i class="fas fa-user-graduate me-2"></i>Datos del Estudiante</h5>
                    
                    <div class="row mb-3">
                        <div class="col-md-12 mb-3">
                            <label for="ddlEstudiante" class="form-label">Estudiante</label>
                            <div class="input-group">
                                <span class="input-group-text"><i class="fas fa-user"></i></span>
                                <asp:DropDownList ID="ddlEstudiante" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="Seleccione un estudiante" Value="" Selected="True" />
                                </asp:DropDownList>
                            </div>
                            <small id="errorEstudiante" class="error-message d-none"></small>
                        </div>
                    </div>
                </div>
                
                <!-- Sección de curso -->
                <div class="form-section">
                    <h5 class="mb-4 text-primary"><i class="fas fa-book me-2"></i>Datos del Curso</h5>
                    
                    <div class="row mb-3">
                        <div class="col-md-12 mb-3">
                            <label for="ddlCurso" class="form-label">Curso</label>
                            <div class="input-group">
                                <span class="input-group-text"><i class="fas fa-book"></i></span>
                                <asp:DropDownList ID="ddlCurso" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="Seleccione un curso" Value="" Selected="True" />
                                </asp:DropDownList>
                            </div>
                            <small id="errorCurso" class="error-message d-none"></small>
                        </div>
                    </div>
                </div>
                
                <!-- Sección de fechas -->
                <div class="form-section">
                    <h5 class="mb-4 text-primary"><i class="fas fa-calendar-alt me-2"></i>Fechas de Matrícula</h5>
                    
                    <div class="row mb-3">
                        <div class="col-md-6 mb-3">
                            <label for="txtFechaInicio" class="form-label">Fecha de Inicio</label>
                            <div class="input-group">
                                <span class="input-group-text"><i class="fas fa-calendar"></i></span>
                                <asp:TextBox ID="txtFechaInicio" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>
                            <small id="errorFechaInicio" class="error-message d-none"></small>
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="txtFechaFin" class="form-label">Fecha de Finalización</label>
                            <div class="input-group">
                                <span class="input-group-text"><i class="fas fa-calendar"></i></span>
                                <asp:TextBox ID="txtFechaFin" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>
                            <small id="errorFechaFin" class="error-message d-none"></small>
                        </div>
                    </div>
                </div>
                
                <div class="form-actions">
                    <asp:Button ID="btnGuardar" runat="server" Text="Actualizar" 
                        OnClick="btnGuardar_Click" CssClass="btn btn-primary btn-lg px-4 me-2" />
                    <a href="ListMatriculas.aspx" class="btn btn-secondary btn-lg px-4">Cancelar</a>
                </div>

            </div>
        </div>
    </div>

    <!-- Font Awesome para íconos -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" />

    <!-- iziToast -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/izitoast/dist/css/iziToast.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/izitoast/dist/js/iziToast.min.js"></script>

    <!-- jQuery -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

    <!-- Script de validación -->
    <script>
        function mostrarError(idInput, idError, mensaje) {
            $(idInput).addClass("is-invalid");
            $(idError).removeClass("d-none").text(mensaje);
        }

        function limpiarError(idInput, idError) {
            $(idInput).removeClass("is-invalid");
            $(idError).addClass("d-none").text("");
        }

        function validarSelect(idSelect, idError, mensaje) {
            const valor = $(idSelect).val();
            if (!valor) {
                mostrarError(idSelect, idError, mensaje);
                return false;
            } else {
                limpiarError(idSelect, idError);
                return true;
            }
        }

        function validarFecha(idInput, idError, minDate, maxDate, mensaje) {
            const fechaStr = $(idInput).val();
            if (!fechaStr) {
                mostrarError(idInput, idError, mensaje);
                return false;
            }

            const fecha = new Date(fechaStr);
            const min = new Date(minDate);
            const max = new Date(maxDate || '9999-12-31');

            if (fecha < min || fecha > max) {
                mostrarError(idInput, idError, mensaje);
                return false;
            } else {
                limpiarError(idInput, idError);
                return true;
            }
        }

        function validarFechaFin(idFechaInicio, idFechaFin, idError) {
            const fechaInicioStr = $(idFechaInicio).val();
            const fechaFinStr = $(idFechaFin).val();

            if (!fechaFinStr) {
                mostrarError(idFechaFin, idError, "La fecha final es obligatoria");
                return false;
            }

            const fechaInicio = new Date(fechaInicioStr);
            const fechaFin = new Date(fechaFinStr);

            if (fechaFin < fechaInicio) {
                mostrarError(idFechaFin, idError, "La fecha final debe ser posterior a la fecha de inicio");
                return false;
            } else {
                limpiarError(idFechaFin, idError);
                return true;
            }
        }

        $(document).ready(function () {
            $("#<%= btnGuardar.ClientID %>").click(function (e) {
                let valido = true;

                valido &= validarSelect("#<%= ddlEstudiante.ClientID %>", "#errorEstudiante", "Seleccione un estudiante");
                valido &= validarSelect("#<%= ddlCurso.ClientID %>", "#errorCurso", "Seleccione un curso");
                valido &= validarFecha("#<%= txtFechaInicio.ClientID %>", "#errorFechaInicio", "2000-01-01", null, "Fecha inicio debe ser posterior al 01/01/2000");
                valido &= validarFechaFin("#<%= txtFechaInicio.ClientID %>", "#<%= txtFechaFin.ClientID %>", "#errorFechaFin");

                if (!valido) {
                    e.preventDefault();
                }
            });

            $("#<%= ddlEstudiante.ClientID %>").on("change", function () {
                validarSelect(this, "#errorEstudiante", "Seleccione un estudiante");
            });

            $("#<%= ddlCurso.ClientID %>").on("change", function () {
                validarSelect(this, "#errorCurso", "Seleccione un curso");
            });

            $("#<%= txtFechaInicio.ClientID %>").on("change blur", function () {
                validarFecha(this, "#errorFechaInicio", "2000-01-01", null, "Fecha inicio debe ser posterior al 01/01/2000");
                validarFechaFin("#<%= txtFechaInicio.ClientID %>", "#<%= txtFechaFin.ClientID %>", "#errorFechaFin");
            });

            $("#<%= txtFechaFin.ClientID %>").on("change blur", function () {
                validarFecha(this, "#errorFechaFin", "2000-01-01", null, "Fecha fin debe ser posterior al 01/01/2000");
                validarFechaFin("#<%= txtFechaInicio.ClientID %>", this, "#errorFechaFin");
            });

            var mensaje = document.getElementById('<%= hdnMensaje.ClientID %>').value;
            if (mensaje !== "") {
                if (mensaje.startsWith("REDIRECT|")) {
                    var partes = mensaje.split('|');
                    if (partes.length >= 3) {
                        var texto = partes[1];
                        var redireccion = partes[2];

                        iziToast.success({
                            title: 'Éxito',
                            message: texto,
                            position: 'topRight',
                            timeout: 3000,
                            onClosed: function () {
                                window.location.href = redireccion;
                            }
                        });
                    }
                }
                else if (mensaje.startsWith("ERR|")) {
                    var textoError = mensaje.substring(4);
                    iziToast.error({
                        title: 'Error',
                        message: textoError,
                        position: 'topRight',
                        timeout: 4000
                    });
                }
            }
        });
    </script>
</asp:Content>