
Partial Public Class Cufex_Security_ChangePassword
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Security"
            myMasterPage.FormName = "Change Password"
            myMasterPage.section = Cufex_Site.SectionName.Security
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Security_ChangePassword
        End If
    End Sub
End Class