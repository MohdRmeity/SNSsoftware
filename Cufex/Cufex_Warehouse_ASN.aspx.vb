
Partial Public Class Cufex_Warehouse_ASN
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "Warehouse"
            myMasterPage.FormName = "ASN Receipt"
            myMasterPage.section = Cufex_Site.SectionName.Warehouse
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Warehouse_ASN

            If HttpContext.Current.Session("userkey") IsNot Nothing Then
                HiddenCanUploadFiles.Value = CommonMethods.getPermission("ASN Receipt->File Upload (Action)", Session("userkey").ToString)
                HiddenCanViewOwnFiles.Value = CommonMethods.getPermission("ASN Receipt->File View Own (Action)", Session("userkey").ToString)
                HiddenCanViewAllFiles.Value = CommonMethods.getPermission("ASN Receipt->File View All (Action)", Session("userkey").ToString)
                HiddenCanRemoveOwnFiles.Value = CommonMethods.getPermission("ASN Receipt->File Remove Own (Action)", Session("userkey").ToString)
                HiddenCanRemoveAllFiles.Value = CommonMethods.getPermission("ASN Receipt->File Remove All (Action)", Session("userkey").ToString)
            End If

            Dim MyWarehouse As String = Request.QueryString("warehouse")
            Dim MyReceipt As String = Request.QueryString("receipt")
            If MyWarehouse <> "" And MyReceipt <> "" Then
                HiddenID.Value = "?warehouse=" & MyWarehouse & "&receipt=" & MyReceipt
                HiddenKeys.Value = CommonMethods.getFacilityDBAlias(MyWarehouse) & "~~~" & MyReceipt
            End If
        End If
    End Sub
End Class