
Partial Public Class Cufex_Security_Users
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Security"
            myMasterPage.FormName = "Users"
            myMasterPage.section = Cufex_Site.SectionName.Security
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Security_Users

            Dim MyID As Integer = Val(Request.QueryString("user"))
            If MyID <> 0 Then HiddenID.Value = "?user=" & MyID
        End If
    End Sub
End Class