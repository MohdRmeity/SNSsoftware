
Partial Public Class Cufex_Warehouse_OrderManagement
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Warehouse"
            myMasterPage.FormName = "Order Management"
            myMasterPage.section = Cufex_Site.SectionName.Warehouse
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Warehouse_OrderManagement

            Dim MyOrderManagKey As String = Request.QueryString("ordermanagkey")
            If MyOrderManagKey <> "" Then HiddenID.Value = "?ordermanagkey=" & MyOrderManagKey
        End If
    End Sub
End Class