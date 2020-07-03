
Partial Public Class Cufex_ExportLogs
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Administration"
            myMasterPage.FormName = "Export Logs"
            myMasterPage.section = Cufex_Site.SectionName.Administration
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Administration_ExportLogs
        End If
    End Sub
End Class