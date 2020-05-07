Imports System.IO
Imports Newtonsoft.Json
Public Class UpdateUserActive
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub Update(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim sql As String = ""
        Dim tb As SQLExec = New SQLExec
        Dim MyID As Integer = HttpContext.Current.Request.Item("MyID")
        Dim Active As Integer = IIf(HttpContext.Current.Request.Item("Active") = "true", 1, 0)
        Dim tmp As String = ""

        sql += " Update " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "PORTALUSERS "
        sql += " Set Active = " & Active & " where ID = " & MyID
        tmp = tb.Execute(sql)

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