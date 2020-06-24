Imports System.Web.Routing
Imports Canonicalize

Public Class URLRewriter
    Shared Sub RegisterRoutes(ByVal routes As RouteCollection)
        Try

            Dim tb As SQLExec = New SQLExec
            routes.Clear()

#Region "cufex"
            routes.MapPageRoute("SNSsoftware-Home", "", "~/Cufex/Cufex.aspx")
            routes.MapPageRoute("SNSsoftware-CMS", "cufex", "~/Cufex/Cufex.aspx")
            routes.MapPageRoute("SNSsoftware-Cufex-Home", "cufex/Home", "~/Cufex/Cufex_Default.aspx")

            routes.MapPageRoute("SNSsoftware-Cufex-Forget_Password", "cufex/Forget_Password", "~/Cufex/Cufex_ForgotPassword.aspx")
            routes.MapPageRoute("SNSsoftware-Cufex-Reset_Password", "cufex/Reset_Password", "~/Cufex/Cufex_ResetPasswordByCode.aspx")

            routes.MapPageRoute("SNSsoftware-Cufex-Security_ChangePassword", "cufex/Security/ChangePassword", "~/Cufex/Cufex_Security_ChangePassword.aspx")
            routes.MapPageRoute("SNSsoftware-Cufex-Security_Users", "cufex/Security/ProfileUsers", "~/Cufex/Cufex_Security_Users.aspx")
            routes.MapPageRoute("SNSsoftware-Cufex-Security_UsersControl", "cufex/Security/UsersControl", "~/Cufex/Cufex_Security_UsersControl.aspx")
            routes.MapPageRoute("SNSsoftware-Cufex-Security_UserProfile", "cufex/Security/UserProfile", "~/Cufex/Cufex_Security_UserProfile.aspx")
            routes.MapPageRoute("SNSsoftware-Cufex-Security_Profiles", "cufex/Security/Profile", "~/Cufex/Cufex_Security_Profiles.aspx")
            routes.MapPageRoute("SNSsoftware-Cufex-Security_ProfilesDetails", "cufex/Security/ProfileDetails", "~/Cufex/Cufex_Security_ProfilesDetails.aspx")

            routes.MapPageRoute("SNSsoftware-Cufex-Configuration_ShipTo", "cufex/Configuration/ShipTo", "~/Cufex/Cufex_Configuration_ShipTo.aspx")
            routes.MapPageRoute("SNSsoftware-Cufex-Configuration_ShipFrom", "cufex/Configuration/ShipFrom", "~/Cufex/Cufex_Configuration_ShipFrom.aspx")
            routes.MapPageRoute("SNSsoftware-Cufex-Configuration_Items", "cufex/Configuration/Items", "~/Cufex/Cufex_Configuration_Items.aspx")
            routes.MapPageRoute("SNSsoftware-Cufex-Configuration_ItemCatalogue", "cufex/Configuration/ItemCatalogue", "~/Cufex/Cufex_Configuration_ItemCatalogue.aspx")

            routes.MapPageRoute("SNSsoftware-Cufex-Warehouse_PO", "cufex/Warehouse/PO", "~/Cufex/Cufex_Warehouse_PO.aspx")
            routes.MapPageRoute("SNSsoftware-Cufex-Warehouse_ASN", "cufex/Warehouse/ASN", "~/Cufex/Cufex_Warehouse_ASN.aspx")
            routes.MapPageRoute("SNSsoftware-Cufex-Warehouse_Shipment", "cufex/Warehouse/Shipment", "~/Cufex/Cufex_Warehouse_Shipment.aspx")
            routes.MapPageRoute("SNSsoftware-Cufex-Warehouse_OrderManagement", "cufex/Warehouse/OrderManagement", "~/Cufex/Cufex_Warehouse_OrderManagement.aspx")

            routes.MapPageRoute("SNSsoftware-Cufex-Inventory_Balance", "cufex/Inventory/Balance", "~/Cufex/Cufex_Inventory_Balance.aspx")

            routes.MapPageRoute("SNSsoftware-Cufex-Reporting_ViewReports", "cufex/Reporting/ViewReports", "~/Cufex/Cufex_Reporting_ViewReports.aspx")
            routes.MapPageRoute("SNSsoftware-Cufex-Reporting_ReportDesigner", "cufex/Reporting/ReportDesigner", "~/Cufex/Cufex_Reporting_ReportDesigner.aspx")
            routes.MapPageRoute("SNSsoftware-Cufex-Reporting_ReportViewer", "cufex/Reporting/ReportViewer", "~/Cufex/Cufex_Reporting_ReportViewer.aspx")

            routes.MapPageRoute("SNSsoftware-Cufex-Popup_Items", "cufex/Search/Items", "~/Cufex/Cufex_Popup_Items.aspx")
            routes.MapPageRoute("SNSsoftware-Cufex-Popup_Packs", "cufex/Search/Packs", "~/Cufex/Cufex_Popup_Packs.aspx")
            routes.MapPageRoute("SNSsoftware-Cufex-Popup_Locations", "cufex/Search/Locations", "~/Cufex/Cufex_Popup_Locations.aspx")
#End Region

            routes.Canonicalize().Www().Lowercase().NoTrailingSlash()
        Catch ex As Exception
            Dim MyString As String = ex.Message
        End Try
    End Sub
End Class
