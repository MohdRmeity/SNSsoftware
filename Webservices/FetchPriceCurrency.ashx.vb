Imports System.IO
Imports Newtonsoft.Json

Public Class FetchPriceCurrency
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub FetchData(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim SearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim MyOwner As String = HttpContext.Current.Request.Item("MyOwner")
        Dim MyConsignee As String = HttpContext.Current.Request.Item("MyConsignee")
        Dim MyItems As String = HttpContext.Current.Request.Item("MyItems")
        Dim MyItemsArr = MyItems.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)
        Dim Sql As String = ""
        Dim DS As DataSet = Nothing
        Dim dr As DataRow() = Nothing
        Dim Fields1 As String = "Price:::"
        Dim Fields2 As String = ";;;Currency:::"

        If SearchTable = "Warehouse_OrderManagement" Then
            Sql += "Select * from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "SKUCATALOGUE"
            DS = (New SQLExec).Cursor(Sql)

            If DS.Tables(0).Rows.Count > 0 Then
                For i = 0 To MyItemsArr.Length - 1
                    Fields1 += IIf(i > 0, "~~~", "")
                    Fields2 += IIf(i > 0, "~~~", "")
                    dr = DS.Tables(0).Select("STORERKEY= '" & MyOwner & "' AND CONSIGNEEKEY= '" & MyConsignee & "' AND SKU = " & MyItemsArr(i))
                    If dr.Length > 0 Then
                        Fields1 += dr(0)!Price.ToString
                        Fields2 += dr(0)!Currency.ToString
                    End If
                Next
            End If
        End If

        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)
        Using writer As JsonWriter = New JsonTextWriter(sw)
            writer.Formatting = Newtonsoft.Json.Formatting.Indented
            writer.WriteStartObject()

            writer.WritePropertyName("Fields")
            writer.WriteValue(Fields1 & Fields2)

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