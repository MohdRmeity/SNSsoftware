Imports System.IO
Imports DevExpress.DashboardCommon

Public Class TextLog
    Public Shared Sub AddToLog(ByVal exception As Exception, ByVal path As String)
        Dim sb As StringBuilder = New StringBuilder
        sb.AppendLine(DateTime.Now.ToLocalTime().ToString("F"))
        sb.AppendLine("Source File: " + System.Web.HttpContext.Current.Request.RawUrl)
        GetExceptionInfo(exception, sb)
        sb.AppendLine("------------------------------------------------------------" & Environment.NewLine)
        File.AppendAllText(path, sb.ToString())
    End Sub

    Public Shared Sub GetExceptionInfo(ByVal exception As Exception, ByVal sb As StringBuilder)
        sb.AppendLine(exception.GetType().ToString)
        sb.AppendLine(exception.Message)
        sb.AppendLine("Stack Trace: ")
        sb.AppendLine(exception.StackTrace)

        If TypeOf exception Is DashboardDataLoadingException Then
            For Each dataLoadingError In CType(exception, DashboardDataLoadingException).Errors
                sb.AppendLine("InnerException: ")
                GetExceptionInfo(dataLoadingError.InnerException, sb)
            Next
        End If

        If exception.InnerException IsNot Nothing Then
            sb.AppendLine("InnerException: ")
            GetExceptionInfo(exception.InnerException, sb)
        End If
    End Sub
End Class