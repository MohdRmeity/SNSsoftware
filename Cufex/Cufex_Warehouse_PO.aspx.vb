
Partial Public Class Cufex_Warehouse_PO
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Warehouse"
            myMasterPage.FormName = "Purchase Order"
            myMasterPage.section = Cufex_Site.SectionName.Warehouse
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Warehouse_PO

            Dim MyWarehouse As String = Request.QueryString("warehouse")
            Dim MyPO As String = Request.QueryString("po")
            If MyWarehouse <> "" And MyPO <> "" Then HiddenID.Value = "?warehouse=" & MyWarehouse & "&po=" & MyPO
        End If
    End Sub
End Class