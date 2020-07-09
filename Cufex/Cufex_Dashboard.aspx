<%@ Page Language="vb" AutoEventWireup="true" CodeBehind="Cufex_Dashboard.aspx.vb" MasterPageFile="~/Cufex/Cufex_DashboardSite.Master" Inherits="SNSsoftware.Cufex_Dashboard" %>

<%@ Register Assembly="DevExpress.Dashboard.v20.1.Web.WebForms, Version=20.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.DashboardWeb" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v20.1, Version=20.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">


    <style>
        .dx-layout-item-wrapper .dx-layout-item-container {
            margin: 0;
        }



        element.style {
            clear: both;
            padding: 0px;
            margin: 0px;
            height: 10px;
            width: 10px;
        }

        .dx-dashboard-widget-container {
            position: relative;
            width: 100%;
            height: 100%;
            min-height: 14px;
            min-width: 10px;
            overflow: hidden;
        }

        .dx-dashboard-item-container {
            box-shadow: 0 1px 0 rgba(0,0,0,0);
            background-color: #fff;
            border-radius: 0px;
            box-sizing: border-box;
            pointer-events: visiblePainted;
            pointer-events: auto;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            flex-wrap: nowrap;
        }

        .dx-dashboard-viewer-content {
            position: absolute;
            top: 0;
            left: 0px;
            right: 0px;
            top: 0px;
            bottom: 0px;
        }

        .dx-dashboard-control {
            background-color: white;
        }
    </style>

</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>

        <dx:ASPxTimer ID="ASPxTimer1" runat="server" ClientInstanceName="timer">
            <ClientSideEvents Tick="function(s, e) {
	                webDesigner.ReloadData();
                        }" />
        </dx:ASPxTimer>
        <div style="position: absolute; left:0; right:0; top:0; bottom:0;">
      
            <dx:ASPxDashboard ID="ASPxDashboard1" runat="server"
                ClientInstanceName="webDesigner"
                OnCustomParameters="ASPxDashboard1_CustomParameters"
                OnConfigureDataReloadingTimeout="ASPxDashboard1_ConfigureDataReloadingTimeout"
                Width="100%"
                AllowExportDashboardItems="False" WorkingMode="ViewerOnly" AllowExportDashboard="False" AllowMaximizeItems="False" Height="100%">
                <DataRequestOptions ItemDataRequestMode="Auto" />
            </dx:ASPxDashboard>
        </div>
    </div>

</asp:Content>
