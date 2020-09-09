Imports System.IO
Imports Newtonsoft.Json

Public Class DeleteItems
    Implements IHttpHandler
    Implements IRequiresSessionState
    Sub ExecuteQuery(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim tb As SQLExec = New SQLExec
        Dim sql As String = "", tmp As String = ""
        Dim SearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim MyItems As String = HttpContext.Current.Request.Item("MyItems")
        Dim MyKeys As String = HttpContext.Current.Request.Item("MyKeys")
        Dim QueryUrlStr As String = HttpContext.Current.Request.Item("QueryUrlStr")

        If MyItems.Contains("?") Then
            MyItems = CommonMethods.GetMyID(SearchTable, MyItems)
        End If

        Dim primKey As String = "ID"
        If SearchTable = "PORTALUSERS" Then
            If CommonMethods.dbtype <> "sql" Then
                SearchTable = "System." & SearchTable
                primKey = "SerialKey"
            End If
            For i = 1 To 5
                sql += " delete from " & IIf(CommonMethods.dbtype <> "sql", "System.", "")
                If i = 1 Then sql += "USERPROFILE"
                If i = 2 Then sql += "USERCONTROLFACILITY"
                If i = 3 Then sql += "USERCONTROL"
                If i = 4 Then sql += "USERWIDGETS"
                If i = 5 Then sql += "PORTALUSERSCONFIGURATION"
                sql += " Where USERKEY in (Select USERKEY From " & SearchTable & " where " & primKey & " in (" & MyItems & ")) "
            Next
            primKey = "ID"
            sql += " delete from " & SearchTable & " where " & primKey & " in (" & MyItems & ") "
            tmp = tb.Execute(sql)
        ElseIf SearchTable = "USERCONTROL" Then
            If CommonMethods.dbtype <> "sql" Then
                SearchTable = "System." & SearchTable
                primKey = "SerialKey"
            End If
            sql += " delete from " & IIf(CommonMethods.dbtype <> "sql", "System.", "") & "USERCONTROLFACILITY"
            sql += " Where USERKEY in (Select USERKEY From " & SearchTable & " where " & primKey & " in (" & MyItems & ")) "
            sql += " delete from " & SearchTable & " where " & primKey & " in (" & MyItems & ") "
            tmp = tb.Execute(sql)
        ElseIf SearchTable = "USERPROFILE" Then
            If CommonMethods.dbtype <> "sql" Then
                SearchTable = "System." & SearchTable
                primKey = "SerialKey"
            End If
            sql += " delete from " & SearchTable & " where " & primKey & " in (" & MyItems & ") "
            tmp = tb.Execute(sql)
        ElseIf SearchTable = "PROFILES" Then
            If CommonMethods.dbtype <> "sql" Then
                SearchTable = "System." & SearchTable
                primKey = "SerialKey"
            End If
            For i = 1 To 4
                If i = 1 Then sql += "delete from " & IIf(CommonMethods.dbtype <> "sql", "System.", "") & "USERPROFILE"
                If i = 2 Then sql += "delete from " & IIf(CommonMethods.dbtype <> "sql", "System.", "") & "PROFILEDETAIL"
                If i = 3 Then sql += "delete from " & IIf(CommonMethods.dbtype <> "sql", "System.", "") & "REPORTSPROFILEDETAIL"
                If i = 4 Then
                    If CommonMethods.dbtype = "sql" Then
                        sql += "delete from " & IIf(CommonMethods.dbtype <> "sql", "System.", "") & "PROFILEDETAILDASHBOARDS"
                    Else
                        Exit For
                    End If
                End If
                sql += " Where PROFILENAME In (Select PROFILENAME From " & SearchTable & " where " & primKey & " In (" & MyItems & ")) "
            Next
            sql += " delete from " & SearchTable & " where " & primKey & " In (" & MyItems & ") "
            tmp = tb.Execute(sql)
        ElseIf SearchTable = "REPORTSPORTAL" Then
            primKey = "ReportID"
            If CommonMethods.dbtype <> "sql" Then SearchTable = "System." & SearchTable
            sql += " delete from REPORTSPROFILEDETAIL where report In (" & MyItems & ") "
            sql += " delete from " & SearchTable & " where " & primKey & " In (" & MyItems & ") "
            tmp = tb.Execute(sql)
        ElseIf SearchTable = "PROFILEDETAIL" Then
            sql += " delete from " & SearchTable & "DASHBOARDS where " & primKey & " In (" & MyItems & ") and PROFILENAME ='" & QueryUrlStr & "'"
            tmp = tb.Execute(sql)
        ElseIf SearchTable.Contains("enterprise.storer") Then
            primKey = "SerialKey"
            Dim type As String = IIf(SearchTable = "enterprise.storer2", "2", "12")
            SearchTable = "enterprise.storer"
            Dim dsItem As DataSet = tb.Cursor("Select StorerKey from " & SearchTable & " where " & primKey & " In (" & MyItems & ")")
            tmp = DeleteConfiguration(dsItem.Tables(0), type)
        ElseIf SearchTable = "enterprise.sku" Then
            primKey = "SerialKey"
            Dim dsItem As DataSet = tb.Cursor("Select StorerKey, Sku from " & SearchTable & " where " & primKey & " In (" & MyItems & ")")
            tmp = DeleteItem(dsItem.Tables(0))
        ElseIf SearchTable = "SKUCATALOGUE" Or SearchTable = "UITEMPLATES" Then
            primKey = "SerialKey"
            If CommonMethods.dbtype <> "sql" Then SearchTable = "System." & SearchTable
            sql += " delete from " & SearchTable & " where " & primKey & " In (" & MyItems & ") "
            tmp = tb.Execute(sql)
        ElseIf SearchTable = "Warehouse_PO" Then
            tmp = DeletePurchaseOrder(MyKeys)
        ElseIf SearchTable = "Warehouse_ASN" Then
            tmp = DeleteASN(MyKeys)
        ElseIf SearchTable = "Warehouse_SO" Then
            tmp = DeleteSO(MyKeys)
        ElseIf SearchTable = "Warehouse_OrderManagement" Then
            primKey = "SerialKey"
            sql += " delete from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAGDETAIL where "
            sql += " WHSEID In (Select WHSEID from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAG where " & primKey & " In (" & MyItems & "))"
            sql += " And ExternOrderKey In (Select ExternOrderKey from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAG where " & primKey & " In (" & MyItems & "))"
            sql += " delete from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAG_FILES"
            sql += " Where OrderManagKey In (Select OrderManagKey from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAG where " & primKey & " In (" & MyItems & "))"
            sql += " delete from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAG where " & primKey & " In (" & MyItems & ") "
            tmp = tb.Execute(sql)
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
    Private Function DeleteConfiguration(ByVal objTable As DataTable, ByVal type As String) As String
        Dim tmp As String = "", Command As String = ""
        Dim enterprise As String = CommonMethods.getEnterpriseDBName()
        For i = 0 To objTable.Rows.Count - 1
            Command = "<StorerHeader><StorerKey>" & objTable.Rows(i)!StorerKey.ToString & "</StorerKey><Type>" & type & "</Type></StorerHeader>"
            Dim Xml As String = "<Message> <Head> <MessageID>0000000003</MessageID> <MessageType>storer</MessageType> <Action>delete</Action> <Sender> <User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>		<TenantId>INFOR</TenantId>		</Sender>		<Recipient>			<SystemID>" & enterprise & "</SystemID>		</Recipient>	</Head>	<Body><Storer> " & Command & "</Storer></Body></Message>"
            tmp = CommonMethods.DeleteXml(Xml, "ALL", IIf(type = "2", "Ship To", "Supplier"), UCase(objTable.Rows(i)!StorerKey.ToString))
        Next
        Return tmp
    End Function
    Private Function DeleteItem(ByVal objTable As DataTable) As String
        Dim tmp As String = "", Command As String = ""
        Dim enterprise As String = CommonMethods.getEnterpriseDBName()
        For i = 0 To objTable.Rows.Count - 1
            Command = "<Item><StorerKey>" & objTable.Rows(i)!StorerKey.ToString & "</StorerKey><Sku>" & objTable.Rows(i)!Sku.ToString & "</Sku></Item>"
            Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>ItemMaster</MessageType><Action>delete</Action><Sender><User>" & CommonMethods.username & "</User><Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID><TenantId>INFOR</TenantId></Sender><Recipient><SystemID>" & enterprise & "</SystemID></Recipient></Head><Body><ItemMaster> " & Command & "</ItemMaster></Body></Message>"
            tmp = CommonMethods.DeleteXml(Xml, "ALL", "Item", UCase(objTable.Rows(i)!StorerKey.ToString) & "-" & UCase(objTable.Rows(i)!Sku.ToString))
        Next
        Return tmp
    End Function
    Private Function DeletePurchaseOrder(ByVal MyKeys As String) As String
        Dim tmp As String = "", Command As String = "", Sql As String = ""
        Dim MyKeysArr As String() = MyKeys.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)

        For i = 0 To MyKeysArr.Count - 1
            Dim MyKeyArrArr As String() = MyKeysArr(i).Split(New String() {"~~~"}, StringSplitOptions.RemoveEmptyEntries)
            Dim Facility As String = MyKeyArrArr(0)
            Dim POKey As String = MyKeyArrArr(1)
            Dim ExternPOKey As String = MyKeyArrArr(2)

            Command = "<PurchaseOrderHeader><ExternPOKey>" & ExternPOKey & "</ExternPOKey><POKey>" & POKey & "</POKey></PurchaseOrderHeader>"
            Dim xml As String = "<Message> <Head> <MessageID>0000000003</MessageID> <MessageType>PurchaseOrder</MessageType> <Action>delete</Action> <Sender> <User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>		<TenantId>INFOR</TenantId>		</Sender>		<Recipient>			<SystemID>" & CommonMethods.getFacilityDBName(Facility) & "</SystemID>		</Recipient>	</Head>	<Body><PurchaseOrder> " & Command & "</PurchaseOrder></Body></Message>"
            tmp = CommonMethods.DeleteXml(xml, Facility, "Po", POKey)

            If tmp = "" Then
                Dim Warehouse As String = CommonMethods.getFacilityDBName(Facility)
                If LCase(Warehouse.Substring(0, 6)) = "infor_" Then Warehouse = Warehouse.Substring(6, Warehouse.Length - 6)
                If LCase(Warehouse).Contains("_") Then Warehouse = Warehouse.Split("_")(1)
                Sql = "Delete from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "PO_FILES where WHSEID = '" & Warehouse & "' and POKEY='" & POKey & "'"
                tmp = (New SQLExec).Execute(Sql)
            End If
            tmp = ""
        Next

        Return tmp
    End Function
    Private Function DeleteASN(ByVal MyKeys As String) As String
        Dim tmp As String = "", Command As String = "", Sql As String = ""
        Dim MyKeysArr As String() = MyKeys.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)

        For i = 0 To MyKeysArr.Count - 1
            Dim MyKeyArrArr As String() = MyKeysArr(i).Split(New String() {"~~~"}, StringSplitOptions.RemoveEmptyEntries)
            Dim Facility As String = MyKeyArrArr(0)
            Dim ReceiptKey As String = MyKeyArrArr(1)
            Dim ExternReceiptKey As String = MyKeyArrArr(2)

            Command = "<AdvancedShipNoticeHeader><ExternReceiptKey>" & ExternReceiptKey & "</ExternReceiptKey><ReceiptKey>" & ReceiptKey & "</ReceiptKey></AdvancedShipNoticeHeader>"
            Dim xml As String = "<Message> <Head> <MessageID>0000000003</MessageID> <MessageType>AdvancedShipNotice</MessageType> <Action>delete</Action> <Sender> 	<User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password>	<SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId></Sender>		<Recipient>			<SystemID>" & CommonMethods.getFacilityDBName(Facility) & "</SystemID>		</Recipient>	</Head>	<Body><AdvancedShipNotice> " & Command & "</AdvancedShipNotice></Body></Message>"
            tmp = CommonMethods.DeleteXml(xml, Facility, "Receipt", ReceiptKey)

            If tmp = "" Then
                Dim Warehouse As String = CommonMethods.getFacilityDBName(Facility)
                If LCase(Warehouse.Substring(0, 6)) = "infor_" Then Warehouse = Warehouse.Substring(6, Warehouse.Length - 6)
                If LCase(Warehouse).Contains("_") Then Warehouse = Warehouse.Split("_")(1)
                Sql = "Delete from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "RECEIPT_FILES where WHSEID = '" & Warehouse & "' and ReceiptKey='" & ReceiptKey & "'"
                tmp = (New SQLExec).Execute(Sql)
            End If
            tmp = ""
        Next

        Return tmp
    End Function
    Private Function DeleteSO(ByVal MyKeys As String) As String
        Dim tmp As String = "", Command As String = "", Sql As String = ""
        Dim MyKeysArr As String() = MyKeys.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)

        For i = 0 To MyKeysArr.Count - 1
            Dim MyKeyArrArr As String() = MyKeysArr(i).Split(New String() {"~~~"}, StringSplitOptions.RemoveEmptyEntries)
            Dim Facility As String = MyKeyArrArr(0)
            Dim OrderKey As String = MyKeyArrArr(1)
            Dim ExternOrderKey As String = MyKeyArrArr(2)

            Command = "<ShipmentOrderHeader><ExternOrderKey>" & ExternOrderKey & "</ExternOrderKey><OrderKey>" & OrderKey & "</OrderKey></ShipmentOrderHeader>"
            Dim xml As String = "<Message> <Head> <MessageID>0000000003</MessageID> <MessageType>ShipmentOrder</MessageType> <Action>delete</Action> <Sender> 	<User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password>	<SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId></Sender>		<Recipient>			<SystemID>" & CommonMethods.getFacilityDBName(Facility) & "</SystemID>		</Recipient>	</Head>	<Body><ShipmentOrder> " & Command & "</ShipmentOrder></Body></Message>"
            tmp = CommonMethods.DeleteXml(xml, Facility, "Order", OrderKey)

            If tmp = "" Then
                Dim Warehouse As String = CommonMethods.getFacilityDBName(Facility)
                If LCase(Warehouse.Substring(0, 6)) = "infor_" Then Warehouse = Warehouse.Substring(6, Warehouse.Length - 6)
                If LCase(Warehouse).Contains("_") Then Warehouse = Warehouse.Split("_")(1)
                Sql = "Delete from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERS_FILES where WHSEID = '" & Warehouse & "' and OrderKey='" & OrderKey & "'"
                tmp = (New SQLExec).Execute(Sql)
            End If
            tmp = ""
        Next

        Return tmp
    End Function
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class