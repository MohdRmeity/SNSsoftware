Partial Public Class Cufex_Logout
    Inherits MultiLingualPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
        Session("BLoggedout") = ""
        Session("BUserCode") = 0
        myMasterPage.BUserCode = 0
        Session("BUserID") = ""
        'Session("BUserGroup") = 0
        myMasterPage.BUserGroup = 0
        Session("BFullName") = ""
        'Session("BCanClone") = 0
        'Session("BUserGroupName") = ""
        Session("fromDefault") = 0
        'Session("BCounter") = 0
        Session("BLog") = 0
        Session("LogMeOut") = 1

        Response.Redirect(Page.GetRouteUrl("SNSsoftware-CMS", Nothing))
    End Sub

End Class