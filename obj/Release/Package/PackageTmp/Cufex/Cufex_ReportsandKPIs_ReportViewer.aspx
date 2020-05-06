<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Cufex/Cufex_Site.Master" CodeBehind="Cufex_ReportsandKPIs_ReportViewer.aspx.vb" Inherits="SNSsoftware.Cufex_ReportsandKPIs_ReportViewer" %>

<%@ Register Assembly="DevExpress.XtraReports.v19.2.Web.WebForms, Version=19.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.XtraReports.Web" TagPrefix="dx" %>

 
<asp:Content ID="Content3" ContentPlaceHolderID="Cufex_MainContent" runat="server">
 
    <dx:ASPxWebDocumentViewer ID="ASPxWebDocumentViewer1" runat="server"></dx:ASPxWebDocumentViewer>
      
      
        <div class="FormSettings">
            <input type="hidden" id="NumberOfRecordsInPage" value="10" />
            <input type="hidden" id="SortBy" value="report asc" />
        </div> 
</asp:Content>
