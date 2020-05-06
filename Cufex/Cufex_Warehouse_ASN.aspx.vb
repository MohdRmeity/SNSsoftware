
Partial Public Class Cufex_Warehouse_ASN
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Warehouse"
            myMasterPage.FormName = "ASN Receipt"
            myMasterPage.section = Cufex_Site.SectionName.Warehouse
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Warehouse_ASN

            Dim MyWarehouse As String = Request.QueryString("warehouse")
            Dim MyReceipt As String = Request.QueryString("receipt")
            If MyWarehouse <> "" And MyReceipt <> "" Then HiddenID.Value = "?warehouse=" & MyWarehouse & "&receipt=" & MyReceipt
        End If
    End Sub
End Class