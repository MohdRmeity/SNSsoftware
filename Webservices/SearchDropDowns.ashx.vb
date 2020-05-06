Imports System.IO
Imports Newtonsoft.Json

Public Class SearchDropDowns
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub Search(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim SearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim MyFacility As String = HttpContext.Current.Request.Item("MyFacility")
        Dim MyOwner As String = HttpContext.Current.Request.Item("MyOwner")
        Dim tb As SQLExec = New SQLExec
        Dim ds As DataSet = Nothing
        Dim tmp As String = ""
        Dim sql As String = ""
        Dim DropDownFields As String = ""
        Dim AndFilter As String = ""
        Dim IsValid As Boolean = Not String.IsNullOrEmpty(MyFacility) And Not String.IsNullOrEmpty(MyOwner)

        If SearchTable = "Warehouse_PO" Or SearchTable = "Warehouse_ASN" Or SearchTable = "Warehouse_SO" Or SearchTable = "Warehouse_OrderManagement" Then
            If IsValid Then
                Dim warehouselevel As String = CommonMethods.getFacilityDBName(MyFacility)
                If String.IsNullOrEmpty(warehouselevel) Then
                    warehouselevel = "enterprise"
                ElseIf warehouselevel.Substring(0, 6).ToLower() = "infor_" Then
                    warehouselevel = warehouselevel.Substring(6, (warehouselevel.Length - 6))
                    warehouselevel = warehouselevel.Split("_")(1)
                Else
                    warehouselevel = warehouselevel.Split("_")(1)
                End If

                Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)

                If owners IsNot Nothing Then
                    Dim ownersstr As String = String.Join("','", owners)
                    ownersstr = "'" & ownersstr & "'"
                    If Not UCase(ownersstr).Contains("'ALL'") Then AndFilter += " and STORERKEY IN (" & ownersstr & ")"

                    sql = " select top 100 Sku from " & warehouselevel & ".SKU where StorerKey= '" & MyOwner & "' " & AndFilter
                    ds = tb.Cursor(sql)

                    DropDownFields += "Sku:::"
                    For i = 0 To ds.Tables(0).Rows.Count - 1
                        With ds.Tables(0).Rows(i)
                            DropDownFields += IIf(i <> 0, ",", "") & !Sku
                        End With
                    Next
                End If
            End If
        End If

        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)
        Using writer As JsonWriter = New JsonTextWriter(sw)
            writer.Formatting = Formatting.Indented
            writer.WriteStartObject()

            writer.WritePropertyName("Error")
            writer.WriteValue("")

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