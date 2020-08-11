Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.Script.Serialization
Imports DevExpress.DashboardWeb
Imports NLog
Imports Oracle.ManagedDataAccess.Client

Partial Public Class Cufex_Default
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.section = Cufex_Site.SectionName.Home_Def
            ASPxDashboard1.ColorScheme = If(Request.QueryString("colorSchema"), ASPxDashboard.ColorSchemeLight)

        End If
        SetDashboard()
    End Sub

    Private Sub SetDashboard()

        Try
            Dim refreshtime As String = CStr(Session("refershTime"))

            If Not String.IsNullOrEmpty(refreshtime) Then
                ASPxTimer1.Interval = Integer.Parse(refreshtime) * 1000
                If (Integer.Parse(refreshtime) / 60) < 1 Then
                    RefeshTimeLabel.Text = "Refresh every " + refreshtime + " Sec"

                ElseIf (Integer.Parse(refreshtime) / 60) < 60 And Integer.Parse(refreshtime) / 60 > 1 Then
                    RefeshTimeLabel.Text = "Refresh every " + (Integer.Parse(refreshtime) / 60).ToString() + " Minutes"
                Else
                    RefeshTimeLabel.Text = "Refresh every " + Math.Round(Integer.Parse(refreshtime) / (60 * 60), 0).ToString() + " Hours"
                End If
            Else

            End If

        Catch ex As Exception
            Dim logger As Logger = LogManager.GetCurrentClassLogger()
            logger.[Error](ex, "", "")
        End Try
        If HttpContext.Current.Session("userkey") IsNot Nothing Then
            Dim dashboardStorage As CustomDashboardStorage = New CustomDashboardStorage(HttpContext.Current.Session("userkey").ToString)
            ASPxDashboard1.SetConnectionStringsProvider(New DevExpress.DataAccess.Web.ConfigFileConnectionStringsProvider())
            ASPxDashboard1.SetDashboardStorage(dashboardStorage)
            If CommonMethods.getPermission("Dashboard->Edit (Action)", HttpContext.Current.Session("userkey").ToString) = "0" Then
                ASPxDashboard1.WorkingMode = WorkingMode.ViewerOnly
            Else
                ASPxDashboard1.WorkingMode = WorkingMode.Viewer
            End If
            If CommonMethods.getPermission("Dashboard->Refresh (Action)", HttpContext.Current.Session("userkey").ToString) = "0" Then
                RefreshSettings.Visible = False
            Else
                RefreshSettings.Visible = True
            End If
        End If
    End Sub
    Protected Sub ASPxDashboard1_ConnectionError(sender As Object, e As ConnectionErrorWebEventArgs)
        Dim Exception As Exception = e.Exception
        TextLog.AddToLog(e.Exception, HttpContext.Current.Server.MapPath("~/App_Data/Error.log"))
    End Sub
    Protected Sub ASPxDashboard1_CustomParameters(sender As Object, e As CustomParametersWebEventArgs)
        Dim refCompanyIdParameter As DevExpress.Data.IParameter = e.Parameters.FirstOrDefault(Function(p) p.Name = "username")
        If refCompanyIdParameter IsNot Nothing Then
            refCompanyIdParameter.Value = HttpContext.Current.Session("userkey").ToString
        End If
    End Sub
    Protected Sub ASPxDashboard1_ConfigureDataReloadingTimeout(sender As Object, e As ConfigureDataReloadingTimeoutWebEventArgs)
        e.DataReloadingTimeout = TimeSpan.FromSeconds(500)
    End Sub

    Protected Sub ASPxDashboard1_CustomDataCallback(ByVal sender As Object, ByVal e As DevExpress.Web.CustomDataCallbackEventArgs)
        If e.Parameter.Contains("ExportDashboard") Then
            Using stream As New MemoryStream()
                Dim selectedDashboardID As String = e.Parameter.Split("|"c)(1)
                Dim ToAddress As String = e.Parameter.Split("|"c)(2)
                Dim subject As String = e.Parameter.Split("|"c)(3)
                Dim body As String = e.Parameter.Split("|"c)(4)

                If subject = "undefined" Then
                    subject = "Portal Dashboard Export"
                End If

                If body = "undefined" Then
                    body = ""
                End If

                Dim dateTimeNow As String = Date.Now.ToString("yyyyMMddHHmmss")
                Dim filePath As String = "~/ExportImages/" + selectedDashboardID & "_" & dateTimeNow & ".jpg"
                Try
                    Dim exporter As New ASPxDashboardExporter(ASPxDashboard1)

                    exporter.ExportToImage(selectedDashboardID, stream)

                    SaveFile(stream, filePath)

                    Dim filePath2 = Server.MapPath("~/ExportImages") + "/" & selectedDashboardID & "_" & dateTimeNow & ".jpg"

                    CommonMethods.SendEmail(ToAddress, subject, body, Server.MapPath("~/ExportImages/" & selectedDashboardID & "_" & dateTimeNow & ".jpg"))

                    e.Result = "success"

                Catch ex As Exception
                    Dim s = ex.Message
                    e.Result = ex.Message
                End Try

            End Using

        End If

        If e.Parameter.Contains("ExportItem") Then

            Using stream As New MemoryStream()
                Dim selectedDashboardID As String = e.Parameter.Split("|"c)(1)
                Dim dashboardItem As String = e.Parameter.Split("|"c)(2)
                Dim ToAddress As String = e.Parameter.Split("|"c)(3)
                Dim subject As String = e.Parameter.Split("|"c)(4)
                Dim body As String = e.Parameter.Split("|"c)(5)

                If subject = "undefined" Then
                    subject = "Portal Dashboard Export"
                End If

                If body = "undefined" Then
                    body = ""
                End If

                Dim dateTimeNow As String = Date.Now.ToString("yyyyMMddHHmmss")
                Dim filePath As String = "~/ExportImages/" + selectedDashboardID & "_" & dateTimeNow & ".jpg"
                Try
                    Dim exporter As New ASPxDashboardExporter(ASPxDashboard1)

                    exporter.ExportDashboardItemToImage(selectedDashboardID, dashboardItem, stream)

                    SaveFile(stream, filePath)

                    Dim filePath2 = Server.MapPath("~/ExportImages") + "/" & selectedDashboardID & "_" & dateTimeNow & ".jpg"

                    CommonMethods.SendEmail(ToAddress, subject, body, Server.MapPath("~/ExportImages/" & selectedDashboardID & "_" & dateTimeNow & ".jpg"))

                    e.Result = "success"

                Catch ex As Exception
                    Dim s = ex.Message
                    e.Result = ex.Message
                End Try

            End Using

        End If
        Dim parameters As Dictionary(Of String, String) = New JavaScriptSerializer().Deserialize(Of Dictionary(Of String, String))(e.Parameter)
        If Not parameters.ContainsKey("ExtensionName") Then Return

        If parameters("ExtensionName") = "dxdde-delete-dashboard" AndAlso parameters.ContainsKey("DashboardID") Then
            Dim dashboardProfiledts As String
            Dim dashboard As String

            If (ConfigurationManager.AppSettings("DatabaseType")).ToLower() = "sql" Then
                dashboardProfiledts = "IF (EXISTS(SELECT * FROM dbo.PROFILEDETAILDASHBOARDS  where DASHBOARD= @pname)) BEGIN  DELETE FROM dbo.PROFILEDETAILDASHBOARDS WHERE DASHBOARD=@pname END ELSE BEGIN select '' END"
                dashboard = "IF (EXISTS(SELECT * FROM dbo.Dashboards  where DashboardID=@pname)) BEGIN  DELETE FROM dbo.Dashboards WHERE DashboardID=@pname END ELSE BEGIN select '' END"
            Else
                dashboardProfiledts = "DELETE FROM SYSTEM.PROFILEDETAILDASHBOARDS   WHERE DASHBOARD=:pname and EXISTS (SELECT * FROM SYSTEM.PROFILEDETAILDASHBOARDS  where DASHBOARD=:pname)"
                dashboard = "DELETE FROM SYSTEM.Dashboards   WHERE DashboardID=:pname and EXISTS (SELECT * FROM SYSTEM.Dashboards  where DashboardID=:pname)"
            End If

            Dim records As Boolean = deleteRecord(dashboardProfiledts, dashboard, Integer.Parse(parameters("DashboardID")))
        End If
    End Sub
    Private Sub SaveFile(ByVal stream As MemoryStream, ByVal path As String)
        Dim fileStream = File.Create(Server.MapPath(path))
        stream.WriteTo(fileStream)
        fileStream.Close()
    End Sub

    Private Function deleteRecord(ByVal repotProfiledts As String, ByVal reportPortal As String, ByVal key As Integer) As Boolean
        Try

            If (ConfigurationManager.AppSettings("DatabaseType")).ToLower() = "sql" Then
                Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                conn.Open()
                Dim cmdusers As SqlCommand = New SqlCommand(repotProfiledts, conn)
                cmdusers.Parameters.AddWithValue("@pname", key)
                cmdusers.ExecuteNonQuery()
                Dim cmddts As SqlCommand = New SqlCommand(reportPortal, conn)
                cmddts.Parameters.AddWithValue("@pname", key)
                cmddts.ExecuteNonQuery()
                conn.Close()
            Else
                Dim conn As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                conn.Open()
                Dim cmdusers As OracleCommand = New OracleCommand(repotProfiledts, conn)
                cmdusers.Parameters.Add(New OracleParameter("pname", key))
                cmdusers.ExecuteNonQuery()
                Dim cmddts As OracleCommand = New OracleCommand(reportPortal, conn)
                cmddts.Parameters.Add(New OracleParameter("pname", key))
                cmddts.ExecuteNonQuery()
                conn.Close()
            End If

            Return True
        Catch e1 As Exception
            Dim logger As Logger = LogManager.GetCurrentClassLogger()
            logger.[Error](e1, "", "")
            Return False
        End Try
    End Function

    Protected Sub MyHiddenButton_Click(sender As Object, e As EventArgs)
        Dim timeMs As Integer = HiddenTime.Value
        Session("refershTime") = timeMs.ToString()
        Page.Response.Redirect(Page.Request.Url.ToString(), True)
    End Sub

    Protected Sub btnReload_Click(sender As Object, e As EventArgs)
        Dim i As DateTime = Now


    End Sub
End Class