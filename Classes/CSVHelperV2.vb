Imports System.IO
Imports System.Text

Public Class CSVHelper
    Public Function Export(ByVal ObjTable As DataTable, ByVal exportcolumnheadings As Boolean) As String
        Dim sb As New StringBuilder

        If exportcolumnheadings Then
            For Each col As DataColumn In ObjTable.Columns
                'header = header & Chr(34) & col.ColumnName & Chr(34) & ","
                sb.Append(Chr(34))
                sb.Append(col.ColumnName)
                sb.Append(Chr(34))
                sb.Append(",")
            Next
            'header = header.Substring(0, header.Length - 1)
            sb.Replace(",", "", sb.Length - 1, 1)
            sb.Append(vbCrLf)
        End If
        For Each row As DataRow In ObjTable.Rows
            Dim arr() As Object = row.ItemArray()
            For i As Integer = 0 To arr.Length - 1
                If arr(i).ToString().IndexOf(",") > 0 Then
                    'record = record & Chr(34) & arr(i).ToString() & Chr(34) & ","
                    sb.Append(Chr(34))
                    sb.Append(arr(i).ToString)
                    sb.Append(Chr(34))
                    sb.Append(",")
                Else
                    'record = record & arr(i).ToString() & ","
                    sb.Append(arr(i).ToString)
                    sb.Append(",")
                End If
            Next
            sb.Replace(",", "", sb.Length - 1, 1)
            sb.Append(vbCrLf)
            'body = body & record.Substring(0, record.Length - 1) & vbCrLf
            'record = ""
        Next
        Return sb.ToString()
        'If exportcolumnheadings Then
        '    'Return header & vbCrLf & body
        'Else
        '    'Return body
        'End If
    End Function
    Public Function Import(ByVal ImportFile As HttpPostedFile) As DataTable
        If ImportFile Is Nothing Or ImportFile.ContentLength <= 0 Then Return Nothing

        Dim dt As DataTable = New DataTable
        Dim firstLine As Boolean = True
        Using sr As New StreamReader(ImportFile.InputStream)
            While Not sr.EndOfStream
                If firstLine Then
                    firstLine = False
                    Dim cols = sr.ReadLine.Split(",")
                    For Each col In cols
                        dt.Columns.Add(New DataColumn(col, GetType(String)))
                    Next
                Else
                    Dim data() As String = sr.ReadLine.Split(",")
                    dt.Rows.Add(data.ToArray)
                End If
            End While
        End Using

        Return dt
    End Function
End Class
