<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Cufex/Cufex_Site.Master" CodeBehind="Cufex_Default.aspx.vb" Inherits="SNSsoftware.Cufex_Default" %>

<%@ Register Assembly="DevExpress.Web.v20.1, Version=20.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Dashboard.v20.1.Web.WebForms, Version=20.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.DashboardWeb" TagPrefix="dx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Cufex_HeadContent" runat="server">
    <script type="text/javascript" src="<%= sAppPath %>JS/Dashboards/DeleteExtension.js"></script>
    <script type="text/javascript" src="<%= sAppPath %>JS/Dashboards/SaveAsExtension.js"></script>
    <script src="../JS/Dashboards/ExportExtension.js"></script>

    <script src="../JS/Dashboards/scripts.js?v=2013"></script>
    <link href="../JS/Dashboards/styles.css?v=2013" rel="stylesheet" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <!-- Defines the "Save As" extension template. -->
    <script type="text/html" id="dx-save-as-form">
        <div>Dashboard Name:</div>
        <div style="margin: 10px 0" data-bind="dxTextBox: { value: newName }"></div>
        <div data-bind="dxButton: { text: 'Save', onClick: saveAs }"></div>
    </script>

    <script>
        function getDashboardControl() {
            return ASPxDashboard.getDashboardControl();
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

        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip();
        });
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

        @media all and (max-width: 786px), only screen and (max-device-width: 852px) {
            .MainDashboardSettings {
                display: none;
            }
        }

        @media all and (max-width: 786px), only screen and (max-device-width: 852px) {
            .exportItemIcon {
                display: none;
            }
        }

        .TimerSettings {
            position: relative;
            background-image: url(../images/Cufex_Images/settings.png);
            background-repeat: no-repeat;
            background-size: contain;
            width: 16px;
            height: 16px;
            cursor: pointer;
            margin-right: 30px
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
            background-image: url(../images/Cufex_Images/return.png);
            background-repeat: no-repeat;
            background-size: contain;
            width: 16px;
            height: 16px;
            border: none;
            margin-right: 10px;
            cursor: pointer;
        }

        .expandButton {
            background-image: url(../images/Cufex_Images/expand.png);
            background-repeat: no-repeat;
            background-size: contain;
            width: 16px;
            height: 16px;
            border: none;
            margin-right: 10px;
            cursor: pointer;
            display: none;
        }

        .collapseButton {
            background-image: url(../images/Cufex_Images/collapse.png);
            background-repeat: no-repeat;
            background-size: contain;
            width: 16px;
            height: 16px;
            border: none;
            margin-right: 10px;
            cursor: pointer;
        }

        .ExpandCollapseStatus {
        }

        .RefreshTimeLabel {
            margin-right: 25px;
            width: 170px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Cufex_MainContent" runat="server">
    <div class="NormalDiv1118Max ZeroPadding">
        <div class="MainDashboardSettings" id="MainDashboardSettingsID">
            <div class="iWantMyChildrenFloatHeight">
                <div class="floatL Width100">
                    <asp:Button ID="Button1" runat="server" Text="Button" Visible="False" OnClick="Button1_Click" />
                    <input type="button" onclick="onExpand();" data-toggle="tooltip" title="Expand" class="expandButton floatL" />

                    <input type="button" onclick="onCollapse();" data-toggle="tooltip" title="Collapse" class="collapseButton floatL" />

                    <asp:Label ID="lblExpandCollapseStatus" runat="server" CssClass="ExpandCollapseStatus floatL" Text="Collapse List"></asp:Label>

                    <div class="floatR">
                        <div class="floatR">
                            <asp:Button ID="btnReload" data-toggle="tooltip" title="Manual Refresh" runat="server" Text="" CssClass="ReloadButton" OnClientClick="javascript:reloaddate();return false;" />
                        </div>

                        <div class="TimerSettings" id="RefreshSettings" runat="server">
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
                            <asp:Label ID="RefeshTimeLabel" runat="server" CssClass="RefreshTimeLabel floatR" Text=""></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <dx:ASPxTimer ID="ASPxTimer1" runat="server" ClientInstanceName="timer">
        <ClientSideEvents Tick="function(s, e) {webDesigner.ReloadData();}" />
    </dx:ASPxTimer>
    <div id="popup"></div>
    <dx:ASPxDashboard ID="ASPxDashboard1" runat="server"
        ClientInstanceName="webDesigner"
        ClientSideEvents-Init="function(s, e) { initializeControls(); }"
        AllowExportDashboardItems="True"
        OnCustomDataCallback="ASPxDashboard1_CustomDataCallback"
        OnCustomParameters="ASPxDashboard1_CustomParameters"
        Height="899px"
        IncludeDashboardIdToUrl="True"
        OnConfigureDataReloadingTimeout="ASPxDashboard1_ConfigureDataReloadingTimeout"
        OnConnectionError="ASPxDashboard1_ConnectionError">
        <ClientSideEvents
            BeforeRender="onBeforeRender" ItemCaptionToolbarUpdated="ItemCaptionToolbarUpdated" DashboardTitleToolbarUpdated="DashboardTitleToolbarUpdated" />
        <BackendOptions Uri="" />
        <DataRequestOptions ItemDataRequestMode="Auto" />
    </dx:ASPxDashboard>

    <asp:Button runat="server" ID="MyHiddenButton" ClientIDMode="Static" Text="" Style="display: none;" OnClick="MyHiddenButton_Click" />
    <input class="HiddenTime" id="HiddenTime" runat="server" type="hidden" value="0" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Cufex_ScriptContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".TimerSettings").click(function () {
                $(".TimerSettingsInner").slideToggle();
            });

            $(".TimeDiv").click(function (e) {
                $(".HiddenTime").val($(this).data("time"));
                //var textRefersh = "Refresh every "+  $(this).data("time")+  " Miuntes";
                //alert(textRefersh);

                //sessionStorage.setItem("textRefersh", textRefersh);
                ////$(".RefreshTimeLabel").text(textRefersh);

                document.getElementById("MyHiddenButton").click();

            });
        });
    </script>
</asp:Content>