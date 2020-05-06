Imports System.Data.SqlClient
Imports System.IO
Imports Microsoft.Reporting.WebForms

Partial Public Class Cufex_Reporting_ViewReports
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "ReportsAndKPIs"
            myMasterPage.FormName = "Reports"
            myMasterPage.section = Cufex_Site.SectionName.Reporting
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Reporting_ViewReports
        End If
    End Sub
End Class