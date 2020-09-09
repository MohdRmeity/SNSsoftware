
Partial Public Class Cufex_Warehouse_PO
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Warehouse"
            myMasterPage.FormName = "Purchase Order"
            myMasterPage.section = Cufex_Site.SectionName.Warehouse
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Warehouse_PO

            If HttpContext.Current.Session("userkey") IsNot Nothing Then
                HiddenCanUploadFiles.Value = CommonMethods.getPermission("Purchase Order->File Upload (Action)", Session("userkey").ToString)
                HiddenCanViewOwnFiles.Value = CommonMethods.getPermission("Purchase Order->File View Own (Action)", Session("userkey").ToString)
                HiddenCanViewAllFiles.Value = CommonMethods.getPermission("Purchase Order->File View All (Action)", Session("userkey").ToString)
                HiddenCanRemoveOwnFiles.Value = CommonMethods.getPermission("Purchase Order->File Remove Own (Action)", Session("userkey").ToString)
                HiddenCanRemoveAllFiles.Value = CommonMethods.getPermission("Purchase Order->File Remove All (Action)", Session("userkey").ToString)
            End If

            Dim MyWarehouse As String = Request.QueryString("warehouse")
            Dim MyPO As String = Request.QueryString("po")
            If MyWarehouse <> "" And MyPO <> "" Then
                HiddenID.Value = "?warehouse=" & MyWarehouse & "&po=" & MyPO
                HiddenKeys.Value = CommonMethods.getFacilityDBAlias(MyWarehouse) & "~~~" & MyPO
            End If
        End If
    End Sub
End Class