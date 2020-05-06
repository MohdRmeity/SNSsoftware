
Partial Public Class Cufex_Warehouse_OrderManagement
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Warehouse"
            myMasterPage.FormName = "Order Management"
            myMasterPage.section = Cufex_Site.SectionName.Warehouse
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Warehouse_OrderManagement

            Dim MyWarehouse As String = Request.QueryString("warehouse")
            Dim MyExternKey As String = Request.QueryString("externkey")
            If MyWarehouse <> "" And MyExternKey <> "" Then HiddenID.Value = "?warehouse=" & MyWarehouse & "&externkey=" & MyExternKey
        End If
    End Sub
End Class