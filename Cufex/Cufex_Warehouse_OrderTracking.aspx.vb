
Partial Public Class Cufex_Warehouse_OrderTracking
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Warehouse"
            myMasterPage.FormName = "Order Tracking"
            myMasterPage.section = Cufex_Site.SectionName.Warehouse
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Warehouse_OrderTracking

            Dim MyKey As String = Request.QueryString("externorderkey")
            Dim MyOwner As String = Request.QueryString("storerkey")
            If MyKey <> "" And MyOwner <> "" Then HiddenID.Value = MyKey & "~~~" & MyOwner



        End If
    End Sub
End Class