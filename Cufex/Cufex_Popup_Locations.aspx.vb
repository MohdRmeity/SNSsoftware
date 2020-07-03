
Partial Public Class Cufex_Popup_Locations
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.section = Cufex_Site.SectionName.Popup
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Popup_Locations
            QueryUrlStr.Value = Page.Request.Url.Query
        End If
    End Sub
End Class