Imports System.IO
Imports Newtonsoft.Json
Public Class UpdateProfileDetail
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub Update(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim sql As String = ""
        Dim tb As SQLExec = New SQLExec
        Dim QueryUrlStr As String = HttpContext.Current.Request.Item("QueryUrlStr")
        Dim TabName As String = HttpContext.Current.Request.Item("TabName")
        Dim ActionName As String = clsGlobals.StripHTML(HttpContext.Current.Request.Item("ActionName"))
        Dim Edit As Integer = IIf(HttpContext.Current.Request.Item("Edit") = "true", 1, 0)
        Dim MyReadOnly As String = IIf(HttpContext.Current.Request.Item("ReadOnly") = "true", 1, 0)
        Dim SearchTable As String = ""
        Dim tmp As String = ""
        Dim SetStr As String = " Set "
        Dim AndFilter As String = " and ProfileName = '" & QueryUrlStr & "' and "

        If TabName = "Actions" Then
            SearchTable = IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "PROFILEDETAIL"
            SetStr += "Edit = '" & Edit & "', ReadOnly = '" & MyReadOnly & "'"
            AndFilter += " ScreenButtonName "
        ElseIf TabName = "Reports" Then
            SearchTable = IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "PROFILEDETAILREPORTS"
            SetStr += "Edit = '" & Edit & "'"
            AndFilter += " Report_Name "
        ElseIf TabName = "Dashboards" Then
            SearchTable = "PROFILEDETAILDASHBOARDS"
            SetStr += "Edit = '" & Edit & "'"
            AndFilter += " Dashboard_Name "
        End If
        AndFilter += " = '" & ActionName & "'"

        sql += "Update " & SearchTable & SetStr & " where 1=1 " & AndFilter
        tmp = tb.Execute(sql)

        If tmp = "" Then
            Try
                Dim FormName As String = ActionName.Substring(0, ActionName.IndexOf("-"))
                If LCase(ActionName).Contains("screen") Then
                    Dim FormParentName As String = ActionName.Substring(0, ActionName.IndexOf("-"))
                    FormName = ActionName.Substring(ActionName.IndexOf(">") + 1).Split("(")(0).TrimEnd()
                    HttpContext.Current.Session("CanView_" & FormParentName & "_" & FormName) = Nothing
                ElseIf LCase(ActionName).Contains("new") Then
                    HttpContext.Current.Session("CanAddNew_" & FormName) = Nothing
                ElseIf LCase(ActionName).Contains("save") Then
                    HttpContext.Current.Session("CanSave_" & FormName) = Nothing
                ElseIf LCase(ActionName).Contains("delete") Then
                    HttpContext.Current.Session("CanDelete_" & FormName) = Nothing
                ElseIf LCase(ActionName).Contains("search") Then
                    HttpContext.Current.Session("CanSearch_" & FormName) = Nothing
                End If
            Catch ex As Exception
                tmp = ex.Message
            End Try
        End If


        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)
        Using writer As JsonWriter = New JsonTextWriter(sw)
            writer.Formatting = Newtonsoft.Json.Formatting.Indented
            writer.WriteStartObject()

            writer.WritePropertyName("Error")
            writer.WriteValue(tmp)

            writer.WriteEndObject()
        End Using
        context.Response.Write(sw)
        context.Response.End()
    End Sub
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class