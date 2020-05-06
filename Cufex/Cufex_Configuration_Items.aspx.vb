
Partial Public Class Cufex_Configuration_Items
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Configuration"
            myMasterPage.FormName = "Items"
            myMasterPage.section = Cufex_Site.SectionName.Configuration
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Configuration_Items

            Dim MyStorer As String = Request.QueryString("storer")
            Dim MySku As String = Request.QueryString("sku")
            If MyStorer <> "" And MySku <> "" Then HiddenID.Value = "?storer=" & MyStorer & "&sku=" & MySku
        End If
    End Sub
End Class