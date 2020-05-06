
Partial Public Class Cufex_Warehouse_Shipment
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Warehouse"
            myMasterPage.FormName = "Shipment Order"
            myMasterPage.section = Cufex_Site.SectionName.Warehouse
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Warehouse_Shipment

            Dim MyWarehouse As String = Request.QueryString("warehouse")
            Dim MyOrder As String = Request.QueryString("order")
            If MyWarehouse <> "" And MyOrder <> "" Then HiddenID.Value = "?warehouse=" & MyWarehouse & "&order=" & MyOrder
        End If
    End Sub
End Class