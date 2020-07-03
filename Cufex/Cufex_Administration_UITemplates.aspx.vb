
Partial Public Class Cufex_Administration_UITemplates
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Administration"
            myMasterPage.FormName = "UI Templates"
            myMasterPage.section = Cufex_Site.SectionName.Administration
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Administration_UITemplates

            Dim template As String = Request.QueryString("template")
            If template <> "" Then HiddenID.Value = "?template=" & template
        End If
    End Sub
End Class