
Partial Public Class Cufex_Configuration_ItemCatalogue
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Configuration"
            myMasterPage.FormName = "Item Catalogue"
            myMasterPage.section = Cufex_Site.SectionName.Configuration
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Configuration_ItemCatalogue

            Dim MyID As Integer = Val(Request.QueryString("item"))
            If MyID <> 0 Then HiddenID.Value = "?sku=" & MyID
        End If
    End Sub
End Class