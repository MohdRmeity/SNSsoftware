<%@ Page Language="vb"  AutoEventWireup="true" CodeBehind="Dashboard.aspx.vb" MasterPageFile="~/Cufex/Site1.Master"  Inherits="SNSsoftware.Dashboard" %>

<%@ Register Assembly="DevExpress.Dashboard.v20.1.Web.WebForms, Version=20.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.DashboardWeb" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v20.1, Version=20.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

 
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div>

            <dx:ASPxTimer ID="ASPxTimer1" runat="server" ClientInstanceName="timer">
                <clientsideevents tick="function(s, e) {
	                webDesigner.ReloadData();
                        }" />
            </dx:ASPxTimer>

            <dx:ASPxDashboard ID="ASPxDashboard1" runat="server"
                ClientInstanceName="webDesigner"
                OnCustomParameters="ASPxDashboard1_CustomParameters"
                OnConfigureDataReloadingTimeout="ASPxDashboard1_ConfigureDataReloadingTimeout"
                AllowExportDashboardItems="True" WorkingMode="ViewerOnly">
                <DataRequestOptions ItemDataRequestMode="Auto" />
            </dx:ASPxDashboard>
                    </div>
 
</asp:Content>