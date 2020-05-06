Imports System.IO
Imports Newtonsoft.Json

Public Class AutoPostBackDetailsDropDowns
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub AutoPostBack(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim SearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim MyValue As String = HttpContext.Current.Request.Item("MyValue")
        Dim MyFacility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("MyFacility"))
        Dim DropDownFields As String = "UOM:::"

        If SearchTable = "Warehouse_ASN" Or SearchTable = "Warehouse_SO" Or SearchTable = "Warehouse_OrderManagement" Then
            If MyValue <> "" Then
                DropDownFields += CommonMethods.getUOMPerPack(IIf(MyFacility = "", "enterprise", MyFacility), MyValue)
            End If
        End If

        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)
        Using writer As JsonWriter = New JsonTextWriter(sw)
            writer.Formatting = Newtonsoft.Json.Formatting.Indented
            writer.WriteStartObject()

            writer.WritePropertyName("DropDownFields")
            writer.WriteValue(DropDownFields)

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