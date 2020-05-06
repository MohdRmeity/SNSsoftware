
Partial Public Class Cufex_Inventory_Balance
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Inventory"
            myMasterPage.FormName = "Inventory Balance"
            myMasterPage.section = Cufex_Site.SectionName.Inventory
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Inventory_Balance
        End If
    End Sub
End Class