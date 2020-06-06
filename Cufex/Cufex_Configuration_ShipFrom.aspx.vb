
Partial Public Class Cufex_Configuration_ShipFrom
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Configuration"
            myMasterPage.FormName = "Ship From"
            myMasterPage.section = Cufex_Site.SectionName.Configuration
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Configuration_ShipFrom

            Dim MyID As String = Request.QueryString("sup")
            If MyID <> "" Then HiddenID.Value = "?sup=" & MyID
        End If
    End Sub
End Class