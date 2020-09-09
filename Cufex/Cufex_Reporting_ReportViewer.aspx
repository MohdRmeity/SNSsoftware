<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Cufex/Cufex_Site.Master" CodeBehind="Cufex_Reporting_ReportViewer.aspx.vb" Inherits="SNSsoftware.Cufex_Reporting_ReportViewer" %>

<%@ Register Assembly="DevExpress.XtraReports.v20.1.Web.WebForms, Version=20.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.XtraReports.Web" TagPrefix="dx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Cufex_MainContent" runat="server">
    <dx:ASPxWebDocumentViewer ID="ASPxWebDocumentViewer" runat="server"></dx:ASPxWebDocumentViewer>
</asp:Content>