Imports DevExpress.DashboardWeb

Public Class Cufex_Dashboard
    Inherits System.Web.UI.Page

    Private userkey As String = "admin"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        Dim refreshtime As String = CommonMethods.ExternalDashboardRefreshTimeInSeconds
        Me.ASPxTimer1.Interval = Val(refreshtime) * 1000
        Dim dashboardID As Integer = Val(Request.QueryString("Id"))
        If dashboardID <> 0 Then
            Dim dashboardStorage As CustomDashboardStorage = New CustomDashboardStorage()
            Me.ASPxDashboard1.WorkingMode = WorkingMode.ViewerOnly
            Dim dashboardXML = dashboardStorage.LoadDashboard(dashboardID.ToString())
            ASPxDashboard1.OpenDashboard(dashboardXML)
        End If
    End Sub

    Protected Sub ASPxDashboard_ConnectionError(ByVal sender As Object, ByVal e As ConnectionErrorWebEventArgs) Handles ASPxDashboard1.ConnectionError
        Dim exception As Exception = e.Exception
        TextLog.AddToLog(e.Exception, Web.HttpContext.Current.Server.MapPath("~/App_Data/Error.log"))
        ' ...
    End Sub

    Protected Sub ASPxDashboard1_CustomParameters(ByVal sender As Object, ByVal e As DevExpress.DashboardWeb.CustomParametersWebEventArgs)
        Dim refCompanyIdParameter = e.Parameters.FirstOrDefault(Function(p) p.Name Is "username")

        If refCompanyIdParameter IsNot Nothing Then
            refCompanyIdParameter.Value = userkey
        End If
    End Sub

    Protected Sub ASPxDashboard1_ConfigureDataReloadingTimeout(ByVal sender As Object, ByVal e As ConfigureDataReloadingTimeoutWebEventArgs)
        e.DataReloadingTimeout = TimeSpan.FromSeconds(5)
    End Sub

End Class

