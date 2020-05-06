Imports DevExpress.XtraReports.Web.ReportDesigner.Services
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.ServiceModel
Imports System.Web

Public Class CustomReportDesignerExceptionHandler
    Inherits ReportDesignerExceptionHandler

    Public Overrides Function GetUnknownExceptionMessage(ByVal ex As Exception) As String
        If TypeOf ex Is FileNotFoundException Then
            Return "File is not found."
        End If

        Return ex.[GetType]().Name & " occurred. See the log file for more details."
    End Function

    Public Overrides Function GetFaultExceptionMessage(ByVal ex As FaultException) As String
        Return "FaultException occurred: " & ex.Message & "."
    End Function
End Class
