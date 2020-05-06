Imports System.IO
Imports Newtonsoft.Json

Public Class DeleteItemsDetails
    Implements IHttpHandler
    Implements IRequiresSessionState
    Sub ExecuteQuery(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim tb As SQLExec = New SQLExec
        Dim tmp As String = ""
        Dim SearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim MyItems As String = HttpContext.Current.Request.Item("MyItems")
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Facility"))
        Dim SQL As String = ""

        If SearchTable <> "Warehouse_OrderManagement" Then
            SQL = " set dateformat dmy select * "

            Dim warehouselevel As String = ""
            If LCase(Facility.Substring(0, 6)) = "infor_" Then
                warehouselevel = Facility.Substring(6, Facility.Length - 6)
            Else
                warehouselevel = Facility
            End If
            warehouselevel = warehouselevel.Split("_")(1)

            If SearchTable = "Warehouse_PO" Then
                SQL += ", (select top 1 ExternPOKey from " & warehouselevel & ".PO where POKey = " & warehouselevel & ".PODETAIL.POKey) as MyExternPOKey "
            End If

            SQL += " from " & warehouselevel & "."
            If SearchTable = "Warehouse_PO" Then
                SQL += "PODETAIL"
            ElseIf SearchTable = "Warehouse_ASN" Then
                SQL += "RECEIPTDETAIL"
            ElseIf SearchTable = "Warehouse_SO" Then
                SQL += "ORDERDETAIL"
            End If
            SQL += " where SerialKey in (" & MyItems & ") "

            Dim DS As DataSet = tb.Cursor(SQL)
            Dim OBJTable As DataTable = DS.Tables(0)

            If Not OBJTable Is Nothing Then
                If SearchTable = "Warehouse_PO" Then
                    tmp = DeletePurchaseOrderDetails(OBJTable, Facility)
                ElseIf SearchTable = "Warehouse_ASN" Then
                    tmp = DeleteASNDetails(OBJTable, Facility)
                ElseIf SearchTable = "Warehouse_SO" Then
                    tmp = DeleteSODetails(OBJTable, Facility)
                End If
            End If
        Else
            SQL += "Delete From " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & ".ORDERMANAGDETAIL"
            SQL += " where SerialKey in (" & MyItems & ") "
            tmp = tb.Execute(SQL)
        End If

        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)
        Using writer As JsonWriter = New JsonTextWriter(sw)
            writer.Formatting = Newtonsoft.Json.Formatting.Indented
            writer.WriteStartObject()

            writer.WritePropertyName("Result")
            writer.WriteValue(tmp)

            writer.WriteEndObject()
        End Using
        context.Response.Write(sw)
        context.Response.End()
    End Sub
    Private Function DeletePurchaseOrderDetails(ByVal OBJTable As DataTable, ByVal Facility As String) As String
        Dim tmp As String = "", Command As String = ""
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                Command = "<PurchaseOrderHeader><POKey>" & !POKey.ToString & "</POKey><ExternPOKey>" & !MyExternPOKey.ToString & "</ExternPOKey><PurchaseOrderDetail><ExternLineNo>" & !ExternLineNo.ToString & "</ExternLineNo></PurchaseOrderDetail></PurchaseOrderHeader>"
                Dim xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>PurchaseOrder</MessageType><Action>delete</Action><Sender><User>" & CommonMethods.username & "</User><Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID><TenantId>INFOR</TenantId></Sender><Recipient><SystemID>" & Facility & "</SystemID></Recipient></Head><Body><PurchaseOrder> " & Command & "</PurchaseOrder></Body></Message>"
                tmp = CommonMethods.DeleteXml(xml, Facility, "PoDetail", !POKey.ToString & "-" & !POLineNumber.ToString)
            End With
        Next
        Return tmp
    End Function
    Private Function DeleteASNDetails(ByVal OBJTable As DataTable, ByVal Facility As String) As String
        Dim tmp As String = "", Command As String = ""
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                Command = "<AdvancedShipNoticeHeader><ReceiptKey>" & !ReceiptKey.ToString & "</ReceiptKey><ExternReceiptKey>" & !ExternReceiptKey.ToString & "</ExternReceiptKey><AdvancedShipNoticeDetail><ReceiptLineNumber>" & !ReceiptLineNumber.ToString & "</ReceiptLineNumber><ExternLineNo>" & !ExternLineNo.ToString & "</ExternLineNo></AdvancedShipNoticeDetail></AdvancedShipNoticeHeader>"
                Dim xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>AdvancedShipNotice</MessageType><Action>delete</Action><Sender><User>" & CommonMethods.username & "</User><Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID><TenantId>INFOR</TenantId></Sender><Recipient><SystemID>" & Facility & "</SystemID></Recipient></Head><Body><AdvancedShipNotice> " & Command & "</AdvancedShipNotice></Body></Message>"
                tmp = CommonMethods.DeleteXml(xml, Facility, "ReceiptDetail", !ReceiptKey.ToString & "-" & !ReceiptLineNumber.ToString)
            End With
        Next
        Return tmp
    End Function
    Private Function DeleteSODetails(ByVal OBJTable As DataTable, ByVal Facility As String) As String
        Dim tmp As String = "", Command As String = ""
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                Command = "<ShipmentOrderHeader><OrderKey>" & !OrderKey.ToString & "</OrderKey><ExternOrderKey>" & !ExternOrderKey.ToString & "</ExternOrderKey><ShipmentOrderDetail><OrderLineNumber>" & !OrderLineNumber.ToString & "</OrderLineNumber><ExternLineNo>" & !ExternLineNo.ToString & "</ExternLineNo></ShipmentOrderDetail></ShipmentOrderHeader>"
                Dim xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>ShipmentOrder</MessageType><Action>delete</Action><Sender><User>" & CommonMethods.username & "</User><Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID><TenantId>INFOR</TenantId></Sender><Recipient><SystemID>" & Facility & "</SystemID></Recipient></Head><Body><ShipmentOrder> " & Command & "</ShipmentOrder></Body></Message>"
                tmp = CommonMethods.DeleteXml(xml, Facility, "OrderDetail", !OrderKey.ToString & "-" & !OrderLineNumber.ToString)
            End With
        Next
        Return tmp
    End Function

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class