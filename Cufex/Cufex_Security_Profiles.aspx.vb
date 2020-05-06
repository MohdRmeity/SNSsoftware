
Partial Public Class Cufex_Security_Profiles
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Security"
            myMasterPage.FormName = "Profiles"
            myMasterPage.section = Cufex_Site.SectionName.Security
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Security_Profiles
        End If
    End Sub
End Class