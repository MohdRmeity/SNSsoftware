<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Cufex/Cufex_Site.Master" CodeBehind="Cufex_Default.aspx.vb" Inherits="SNSsoftware.Cufex_Default" %>

<%@ Register Assembly="DevExpress.Web.v19.2, Version=19.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Dashboard.v19.2.Web.WebForms, Version=19.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.DashboardWeb" TagPrefix="dx" %>

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

    <style>
        .MainDashboardSettings {
            padding: 10px 20px;
        }

        .TimerSettings {
            position: relative;
            background-image: url(../../images/Cufex_Images/settings.png);
            background-repeat: no-repeat;
            background-size: contain;
            width: 16px;
            height: 16px;
            cursor: pointer;
        }

        .TimerSettingsInner {
            z-index: 1;
            position: absolute;
            top: 26px;
            left: -35px;
            width: 75px;
            background-color: #FFF;
            display: none;
        }

        .TimeDiv {
            padding: 5px 10px;
            cursor: pointer;
        }

            .TimeDiv:first-child {
                padding-top: 10px;
            }

            .TimeDiv:last-child {
                padding-bottom: 10px;
            }

            .TimeDiv:hover {
                background-color: #3673ff;
                color: #FFF;
            }

        .ReloadButton {
            background-image: url(../../images/Cufex_Images/return.png);
            background-repeat: no-repeat;
            background-size: contain;
            width: 16px;
            height: 16px;
            border: none;
            margin-right: 10px;
              cursor: pointer;
        }
    </style>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Cufex_MainContent" runat="server">
    <div class="NormalDiv1118Max ZeroPadding">
        <div class="MainDashboardSettings">
            <div class="iWantMyChildrenFloatHeight">
                <div class="floatL Width100">
                    <div class="floatR">
                        <div class="TimerSettings">
                            <div class="TimerSettingsInner">
                                <div class="TimeDiv AnimateMe" data-time="30">30 Sec</div>
                                <div class="TimeDiv AnimateMe" data-time="60">1 Min</div>
                                <div class="TimeDiv AnimateMe" data-time="300">5 Min</div>
                                <div class="TimeDiv AnimateMe" data-time="600">10 Min</div>
                                <div class="TimeDiv AnimateMe" data-time="900">15 Min</div>
                                <div class="TimeDiv AnimateMe" data-time="1800">30 Min</div>
                                <div class="TimeDiv AnimateMe" data-time="2700">45 Min</div>
                                <div class="TimeDiv AnimateMe" data-time="3600">1 H</div>
                            </div>
                        </div>
                    </div>
                    <div class="floatR">
                        <asp:Button ID="btnReload" runat="server" Text="" CssClass="ReloadButton"   OnClientClick="javascript:reloaddate();return false;"  />
                    </div>
                </div>
            </div>
        </div>
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
        <asp:Button runat="server" ID="MyHiddenButton" ClientIDMode="Static" Text="" Style="display: none;" OnClick="MyHiddenButton_Click" />
        <input class="HiddenTime" id="HiddenTime" runat="server" type="hidden" value="0" />
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Cufex_ScriptContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".TimerSettings").click(function () {
                $(".TimerSettingsInner").slideToggle();
            });

            $(".TimeDiv").click(function (e) {
                $(".HiddenTime").val($(this).data("time"));
                document.getElementById("MyHiddenButton").click();
            });
        });
    </script>
</asp:Content>

