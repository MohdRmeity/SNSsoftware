
Public Class MultiLingualControl
    Inherits UserControl
    Public sAppPath As String
    Public AppVersion As String

    Protected Overrides Sub OnInit(ByVal e As EventArgs)
        On Error Resume Next
        AppVersion = clsGlobals.AppVersion
        sAppPath = clsGlobals.sAppPath
        MyBase.OnInit(e)
    End Sub
    Public Shared Sub FillCombobox(ByRef combobox As DropDownList, ByVal dt As DataTable, ByVal DataTextField As String, ByVal DataValueField As String, Optional ByVal DefaultString As String = "None")
        Security.FillCombobox(combobox, dt, DataTextField, DataValueField, DefaultString)
    End Sub
    Public Shared Sub FillHtmlSelect(ByRef combobox As HtmlSelect, ByVal dt As DataTable, ByVal DataTextField As String, ByVal DataValueField As String, Optional ByVal DefaultString As String = "None")
        Security.FillHtmlSelect(combobox, dt, DataTextField, DataValueField, DefaultString)
    End Sub
    Public Shared Function GetLebanonTime() As Date
        Return clsGlobals.GetLebanonTime()
    End Function
    Public Shared Function fixMyString(ByVal strWithHtml As String, Optional ByVal iLength As Integer = 0) As String
        Return clsGlobals.fixMyString(strWithHtml, iLength)
    End Function
    Public Shared Function StripHTML(ByVal source As String) As String
        Return clsGlobals.StripHTML(source)
    End Function
    Public Shared Function FilterStr(ByVal RequestVariable As String) As String
        Return clsGlobals.FilterStr(RequestVariable)
    End Function
End Class
