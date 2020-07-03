
Imports System.Data
Imports System.IO
Imports System.Text
Imports System.Web

Public Class clsCSV
    Public Shared Function ToCSV(ByVal dtInput As DataTable) As String
        If dtInput.Rows.Count > 0 Then
            Dim csvHandle As New CSVHelper
            Dim strData As String = csvHandle.Export(dtInput, True)
            Return strData
        End If
        Return String.Empty
    End Function

    Public Shared Function ConvertCSVToDataTable(ByVal ImportFile As HttpPostedFile) As DataTable
        Return (New CSVHelper).Import(ImportFile)
    End Function
End Class
