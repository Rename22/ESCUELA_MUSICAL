<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="ListEspecialidades.aspx.cs" Inherits="Proyecto_FInal_Escuela_Musica.Especialidades.ListEspecialidades" %>

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
        
        .table-container {
            overflow-x: auto;
        }
        
        .custom-table {
            width: 100%;
            border-collapse: separate;
            border-spacing: 0;
            border-radius: 0.5rem;
            overflow: hidden;
        }
        
        .custom-table thead {
            background-color: var(--primary-color);
            color: white;
        }
        
        .custom-table th {
            padding: 1rem;
            font-weight: 600;
            text-align: left;
            border: none;
        }
        
        .custom-table td {
            padding: 1rem;
            border-top: 1px solid #e3e6f0;
            vertical-align: middle;
        }
        
        .custom-table tbody tr {
            transition: all 0.15s ease;
        }
        
        .custom-table tbody tr:hover {
            background-color: rgba(78, 115, 223, 0.05);
        }
        
        .btn-primary {
            background-color: var(--primary-color);
            border-color: var(--primary-color);
        }
        
        .btn-success {
            background-color: var(--success-color);
            border-color: var(--success-color);
        }
        
        .btn-warning {
            background-color: var(--warning-color);
            border-color: var(--warning-color);
        }
        
        .btn-danger {
            background-color: var(--danger-color);
            border-color: var(--danger-color);
        }
        
        .btn-sm {
            padding: 0.35rem 0.75rem;
            font-size: 0.875rem;
            border-radius: 0.35rem;
        }
        
        .search-container {
            background-color: #fff;
            padding: 1.5rem;
            border-radius: 0.5rem;
            box-shadow: var(--shadow);
            margin-bottom: 1.5rem;
        }
        
        .pagination-container {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 1rem;
            background-color: #f8f9fc;
            border-top: 1px solid #e3e6f0;
        }
        
        .page-size-selector {
            display: flex;
            align-items: center;
        }
        
        .page-size-selector label {
            margin-right: 0.5rem;
            margin-bottom: 0;
            color: var(--dark-color);
        }
        
        .actions-column {
            min-width: 140px;
        }
        
        .empty-state {
            text-align: center;
            padding: 3rem;
            color: var(--secondary-color);
        }
        
        .empty-state i {
            font-size: 4rem;
            margin-bottom: 1rem;
            color: #dddfeb;
        }
        
        .form-control:focus {
            border-color: var(--primary-color);
            box-shadow: 0 0 0 0.2rem rgba(78, 115, 223, 0.25);
        }
        
        .header-actions {
            display: flex;
            align-items: center;
            gap: 1rem;
        }
        
        .btn-icon {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 0.5rem;
        }
        
        .btn-icon i {
            font-size: 0.9rem;
        }
        
        .action-buttons {
            display: flex;
            gap: 0.5rem;
        }
        
        .pager-controls {
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .pager-controls input {
            max-width: 60px;
            text-align: center;
        }
        
        .form-control-sm {
            height: calc(1.8125rem + 2px);
            padding: 0.25rem 0.5rem;
            font-size: 0.875rem;
            line-height: 1.5;
            border-radius: 0.2rem;
        }
        
        @media (max-width: 768px) {
            .header-actions {
                flex-direction: column;
                align-items: flex-start;
                gap: 0.5rem;
                margin-top: 1rem;
                width: 100%;
            }
            
            .card-header {
                flex-direction: column;
                align-items: flex-start;
            }
            
            .search-container .col-md-8, 
            .search-container .col-md-4 {
                width: 100%;
                margin-bottom: 0.5rem;
            }
            
            .search-container .d-flex {
                flex-direction: column;
                gap: 0.5rem;
            }
            
            .card-footer {
                flex-direction: column;
                gap: 1rem;
            }
            
            .pager-controls {
                width: 100%;
                justify-content: center;
            }
        }
    </style>

    <div class="container mt-4">
        <div class="card">
            <div class="card-header">
                <h3 class="card-title">Listado de Especialidades</h3>
                <div class="header-actions">
                    <a href="AddEspecialidad.aspx" class="btn btn-success btn-icon">
                        <i class="fas fa-plus"></i> Nueva Especialidad
                    </a>
                    <div class="d-flex align-items-center">
                        <span class="mr-2">Mostrar</span>
                        <asp:DropDownList ID="ddlPageSize" runat="server" CssClass="form-control form-control-sm" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" Width="70px">
                            <asp:ListItem Text="10" Value="10" />
                            <asp:ListItem Text="20" Value="20" Selected="True" />
                            <asp:ListItem Text="50" Value="50" />
                            <asp:ListItem Text="100" Value="100" />
                        </asp:DropDownList>
                        <span class="ml-2">registros</span>
                    </div>
                </div>
            </div>
            
            <div class="card-body">
                <asp:HiddenField ID="hdnMensaje" runat="server" />

                <!-- Filtro de búsqueda -->
                <div class="search-container">
                    <div class="row">
                        <div class="col-md-8">
                            <div class="input-group">
                                <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control" placeholder="Buscar por nombre..." />
                                <div class="input-group-append">
                                    <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-primary" OnClick="btnBuscar_Click" />
                                    <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-outline-secondary" OnClick="btnLimpiar_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- GridView con paginación -->
                <div class="table-container">
                    <asp:GridView ID="gvEspecialidades" runat="server" AutoGenerateColumns="False" 
                        CssClass="custom-table" AllowPaging="True" PageSize="20"
                        OnPageIndexChanging="gvEspecialidades_PageIndexChanging" 
                        DataKeyNames="id_espec" PagerStyle-CssClass="pagination" 
                        HeaderStyle-CssClass="thead-dark" ShowHeaderWhenEmpty="true">
                        <Columns>
                            <asp:BoundField DataField="id_espec" HeaderText="ID" Visible="true" />
                            <asp:BoundField DataField="nombre_espec" HeaderText="Nombre" />
                            <asp:BoundField DataField="fecha_creacion_espec" HeaderText="Fecha Creación" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:TemplateField HeaderText="Acciones" HeaderStyle-CssClass="actions-column" ItemStyle-CssClass="actions-column">
                                <ItemTemplate>
                                    <div class="action-buttons">
                                        <asp:HyperLink runat="server" NavigateUrl='<%# "EditEspecialidad.aspx?id=" + Eval("id_espec") %>' 
                                            CssClass="btn btn-sm btn-warning" ToolTip="Editar">
                                            <i class="fas fa-edit"></i>
                                        </asp:HyperLink>
                                        <button type="button" class="btn btn-sm btn-danger" 
                                            title="Eliminar"
                                            onclick="confirmarEliminacionEspecialidad(<%# Eval("id_espec") %>, '<%# Eval("nombre_espec") %>')">
                                            <i class="fas fa-trash-alt"></i>
                                        </button>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerSettings Mode="NumericFirstLast" Position="TopAndBottom" />
                        <PagerStyle CssClass="pagination-container" HorizontalAlign="Right" />
                        <EmptyDataTemplate>
                            <div class="empty-state">
                                <i class="fas fa-guitar"></i>
                                <h4>No se encontraron especialidades</h4>
                                <p class="mb-0">No hay especialidades registradas en el sistema</p>
                                <a href="AddEspecialidad.aspx" class="btn btn-primary mt-3">
                                    <i class="fas fa-plus"></i> Nueva Especialidad
                                </a>
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
            
            <div class="card-footer d-flex flex-column flex-md-row justify-content-between align-items-center">
                <div class="text-muted mb-2 mb-md-0">
                    <asp:Label ID="lblTotalRegistros" runat="server" Text="Total: 0 registros"></asp:Label>
                </div>
                <div class="pager-controls">
                    <span class="mr-2">Página</span>
                    <asp:TextBox ID="txtGoToPage" runat="server" CssClass="form-control form-control-sm" Width="50px" Text="1" />
                    <span class="mx-2">de</span>
                    <asp:Label ID="lblTotalPages" runat="server" Text="1" CssClass="font-weight-bold" />
                    <asp:Button ID="btnGo" runat="server" Text="Ir" CssClass="btn btn-sm btn-primary ml-2" OnClick="btnGo_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- Font Awesome para íconos -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" />

    <!-- iziToast -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/izitoast/dist/css/iziToast.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/izitoast/dist/js/iziToast.min.js"></script>

    <!-- Formulario oculto para eliminar especialidades -->
    <asp:Button ID="btnEliminarEspecialidad" runat="server" OnClick="btnEliminarEspecialidad_Click" style="display: none;" />
    <asp:HiddenField ID="hdnEspecialidadAEliminar" runat="server" />

    <script>
        // Mostrar mensajes con iziToast
        var mensaje = document.getElementById('<%= hdnMensaje.ClientID %>').value;
        if (mensaje !== "") {
            var tipo = mensaje.startsWith("OK|") ? "success" : "error";
            var texto = mensaje.replace("OK|", "").replace("ERR|", "");

            iziToast[tipo]({
                title: tipo === "success" ? "Éxito" : "Error",
                message: texto,
                position: "topRight",
                timeout: 5000,
                transitionIn: 'fadeIn',
                transitionOut: 'fadeOut'
            });
        }

        // Actualizar etiqueta de total de registros
        function updateTotalRecords(total) {
            document.getElementById('<%= lblTotalRegistros.ClientID %>').innerText = "Total: " + total + " registros";
        }

        // Función para confirmar eliminación con iziToast
        function confirmarEliminacionEspecialidad(idEspecialidad, nombre) {
            iziToast.question({
                timeout: false,
                close: false,
                overlay: true,
                displayMode: 'once',
                title: '¿Estás seguro?',
                message: `Esta acción eliminará permanentemente la especialidad:<br><strong>${nombre}</strong>`,
                position: 'center',
                buttons: [
                    ['<button><b>Sí, eliminar</b></button>', function (instance, toast) {
                        instance.hide({}, toast);
                        
                        // Guardar el ID de la especialidad a eliminar en el campo oculto
                        document.getElementById('<%= hdnEspecialidadAEliminar.ClientID %>').value = idEspecialidad;
                        
                        // Disparar el evento de eliminación
                        document.getElementById('<%= btnEliminarEspecialidad.ClientID %>').click();
                    }, true],
                    ['<button>Cancelar</button>', function (instance, toast) {
                        instance.hide({}, toast);
                    }]
                ],
                onClosing: function (instance, toast, closedBy) {
                    console.info('Closed | closedBy: ' + closedBy);
                }
            });
        }
    </script>
</asp:Content>