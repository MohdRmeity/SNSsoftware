<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Cufex/Cufex_Site.Master" CodeBehind="Cufex_Default.aspx.vb" Inherits="SNSsoftware.Cufex_Default" %>

<%@ Register Assembly="DevExpress.Web.v20.1, Version=20.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Dashboard.v20.1.Web.WebForms, Version=20.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.DashboardWeb" TagPrefix="dx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Cufex_HeadContent" runat="server">
    <script type="text/javascript" src="<%= sAppPath %>JS/Dashboards/DeleteExtension.js"></script>
    <script type="text/javascript" src="<%= sAppPath %>JS/Dashboards/SaveAsExtension.js"></script>

    <!-- Defines the "Save As" extension template. -->
    <script type="text/html" id="dx-save-as-form">
        <div>Dashboard Name:</div>
        <div style="margin: 10px 0" data-bind="dxTextBox: { value: newName }"></div>
        <div data-bind="dxButton: { text: 'Save', onClick: saveAs }"></div>
    </script>

    <script type="text/javascript">
        //This will setup the toggle for viewer mode and edit mode.
        function onBeforeRender(sender) {
            var control = sender.GetDashboardControl();
            setInterval(function () { }, 3000);
            control.registerExtension(new DevExpress.Dashboard.DashboardPanelExtension(control, { dashboardThumbnail: "~/App_Data/Dashboards/DashboardThumbnail/{0}.png" }));

            control.requestDashboardList()
                .done(function (e) {
                    e.forEach(function (dashboardInfo) {
                        console.log(dashboardInfo);
                    });
                });

            var control = sender.GetDashboardControl();
            control.registerExtension(new SaveAsDashboardExtension(control));
            control.registerExtension(new DeleteDashboardExtension(sender));
        }
    </script>

    <script type="text/javascript">
        var ass = 0;
        function reloaddate() {
            webDesigner.ReloadData();
        }
    </script>

    <script>
        /* When the user clicks on the button,
        toggle between hiding and showing the dropdown content */
        function myFunction() {
            document.getElementById("myDropdown").classList.toggle("show");
        }

        // Close the dropdown if the user clicks outside of it
        window.onclick = function (event) {
            if (!event.target.matches('.dropbtn')) {
                var dropdowns = document.getElementsByClassName("dropdown-content");
                var i;
                for (i = 0; i < dropdowns.length; i++) {
                    var openDropdown = dropdowns[i];
                    if (openDropdown.classList.contains('show')) {
                        openDropdown.classList.remove('show');
                    }
                }
            }
        }
    </script>

    <script type="text/javascript">
        function setSessionValue(s) {
            document.getElementById("Hidden1").value = s;
            location.reload();
        }
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Cufex_MainContent" runat="server">
    <div class="NormalDiv1118Max ZeroPadding">
        <dx:ASPxTimer ID="ASPxTimer1" runat="server" ClientInstanceName="timer">
            <ClientSideEvents Tick="function(s, e) {webDesigner.ReloadData();}" />
        </dx:ASPxTimer>
        <dx:ASPxDashboard ID="ASPxDashboard1" runat="server" ClientInstanceName="webDesigner" AllowExportDashboardItems="True" OnCustomDataCallback="ASPxDashboard1_CustomDataCallback"
            OnCustomParameters="ASPxDashboard1_CustomParameters" Height="1000px"
            OnConfigureDataReloadingTimeout="ASPxDashboard1_ConfigureDataReloadingTimeout"
            OnConnectionError="ASPxDashboard1_ConnectionError">
            <ClientSideEvents BeforeRender="onBeforeRender" />
            <BackendOptions Uri="" />
            <DataRequestOptions ItemDataRequestMode="Auto" />
        </dx:ASPxDashboard>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Cufex_ScriptContent" runat="server">
</asp:Content>

