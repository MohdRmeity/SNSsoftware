Imports System.Web.Routing
Imports DevExpress.XtraReports.Web.ReportDesigner.Native.Services

Public Class Global_asax
    Inherits HttpApplication

    Sub Application_Start(ByVal sendeApplication_Errorr As Object, ByVal e As EventArgs)
        ' Fires when the application is started 

        'Dim dic As Dictionary = New Dictionary
        'dic.LoadVariables()
        URLRewriter.RegisterRoutes(RouteTable.Routes)

        DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(New CustomReportStorageWebExtension())

        'DevExpress.XtraReports.Web.ReportDesigner.DefaultReportDesignerContainer.Register(Of IReportDesignerExceptionHandler, CustomReportDesignerExceptionHandler)()



        DevExpress.XtraReports.Web.ReportDesigner.DefaultReportDesignerContainer.RegisterDataSourceWizardConnectionStringsProvider(Of DataSourceWizardConnectionStringsProvider)()


    End Sub
    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)

    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)

    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires upon attempting to authenticate the use
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        Dim ex As Exception = Server.GetLastError()

        If InStr(ex.Message, "does not exist") > 0 Then
            Application("TheException") = ex
            Application("ErrorRawURL") = Request.RawUrl
            Server.ClearError()
        Else
            Application("TheException") = ex
            Application("ErrorRawURL") = Request.RawUrl

            If ex IsNot Nothing Then
                Dim lasterror As New StringBuilder()

                If ex.Message IsNot Nothing Then
                    lasterror.AppendLine("Message:")
                    lasterror.AppendLine(ex.Message)
                    lasterror.AppendLine()
                End If

                If ex.InnerException IsNot Nothing Then
                    lasterror.AppendLine("InnerException:")
                    lasterror.AppendLine(ex.InnerException.ToString())
                    lasterror.AppendLine()
                End If

                If ex.Source IsNot Nothing Then
                    lasterror.AppendLine("Source:")
                    lasterror.AppendLine(ex.Source)
                    lasterror.AppendLine()
                End If

                If ex.StackTrace IsNot Nothing Then
                    lasterror.AppendLine("StackTrace:")
                    lasterror.AppendLine(ex.StackTrace)
                    lasterror.AppendLine()
                End If
            End If
            Server.ClearError()
        End If
    End Sub
    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session ends
    End Sub
    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application ends
    End Sub
End Class