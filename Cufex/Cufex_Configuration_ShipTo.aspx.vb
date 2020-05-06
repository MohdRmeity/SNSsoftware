
Partial Public Class Cufex_Configuration_ShipTo
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Configuration"
            myMasterPage.FormName = "Ship To"
            myMasterPage.section = Cufex_Site.SectionName.Configuration
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Configuration_ShipTo

            Dim MyID As String = Request.QueryString("cust")
            If MyID <> "" Then HiddenID.Value = "?cust=" & MyID
        End If
    End Sub
End Class