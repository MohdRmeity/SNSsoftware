
Partial Public Class Cufex_Security_UsersControl
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Security"
            myMasterPage.FormName = "User Control"
            myMasterPage.section = Cufex_Site.SectionName.Security
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Security_UsersControl

            Dim MyID As String = Request.QueryString("user")
            If MyID <> "" Then HiddenID.Value = "?user=" & MyID
        End If
    End Sub
End Class