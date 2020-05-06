Imports System.Data.SqlClient
Imports System.Web.Services
Imports Oracle.ManagedDataAccess.Client
Imports System.Web.Script.Services
Imports System.IO
Imports DevExpress.XtraReports.UI

Partial Public Class Cufex_Reporting_ReportViewer
    Inherits MultiLingualPage


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "ReportsAndKPIs"
            myMasterPage.FormName = "Report Viwer"
            myMasterPage.section = Cufex_Site.SectionName.Reporting
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Reporting_ViewReports
            SetReportViewer(Request.QueryString("reportid"))
        End If
    End Sub

    Private Sub SetReportViewer(ByVal ReportId As String)
        Dim customReportStorageWeb As CustomReportStorageWebExtension = New CustomReportStorageWebExtension()
        Dim tb As DataTable = customReportStorageWeb.GetReportByID(ReportId)

        Dim layoutdata As Byte() = customReportStorageWeb.GetReportByID(ReportId).AsEnumerable().[Select](Function(x) x.Field(Of Byte())("LayoutData")).FirstOrDefault()
        Dim memoryStream As MemoryStream = New MemoryStream(layoutdata)
        Dim report As XtraReport = New XtraReport()
        report.LoadLayout(memoryStream)
        If (report.Parameters("username") IsNot Nothing) Then
            report.Parameters("username").Value = UserInfo.LoginUser
        End If
        ASPxWebDocumentViewer.OpenReport(report)
    End Sub

End Class