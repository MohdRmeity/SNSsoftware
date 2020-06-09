﻿Imports System.IO
Imports Newtonsoft.Json

Public Class DeleteItems
    Implements IHttpHandler
    Implements IRequiresSessionState
    Sub ExecuteQuery(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim tb As SQLExec = New SQLExec
        Dim sql As String = "", tmp As String = ""
        Dim SearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim MyItems As String = HttpContext.Current.Request.Item("MyItems")
        Dim QueryUrlStr As String = HttpContext.Current.Request.Item("QueryUrlStr")
        REM ghina karame - 11/05/2020- get the system flag useRest from webconfig to know if we should replace soapapi with restapi -begin
        Dim useRest As String = ConfigurationManager.AppSettings("UseRestAPI")
        Dim version As String = ConfigurationManager.AppSettings("version")
        REM ghina karame - 11/05/2020- get the system flag useRest from webconfig to know if we should replace soapapi with restapi -end
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
                If i = 3 Then sql += "delete from " & IIf(CommonMethods.dbtype <> "sql", "System.", "") & "PROFILEDETAILREPORTS"
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
            Dim type As String = SearchTable(SearchTable.Length - 1)
            SearchTable = SearchTable.Remove(SearchTable.Length - 1)
            Dim dsItem As DataSet = tb.Cursor("Select StorerKey from " & SearchTable & " where " & primKey & " In (" & MyItems & ")")
            'ghina karame - 11/05/2020- restapicalls- if flag is on and version > 11 then use rest calls instead of soap calls -begin
            If useRest = "1" And version >= "11" Then
                tmp = DeleteRestTradingPartner(dsItem.Tables(0), type)
            Else
                tmp = DeleteConfiguration(dsItem.Tables(0), type)
            End If
            'ghina karame - 11/05/2020- restapicalls- if flag is on and version > 11 then use rest calls instead of soap calls -end
        ElseIf SearchTable = "enterprise.sku" Then
            primKey = "SerialKey"
            Dim dsItem As DataSet = tb.Cursor("Select StorerKey, Sku from " & SearchTable & " where " & primKey & " In (" & MyItems & ")")
            'ghina karame - 11/05/2020- restapicalls- if flag is on and version > 11 then use rest calls instead of soap calls -begin
            If useRest = "1" And version >= "11" Then
                tmp = DeleteRestItem(dsItem.Tables(0))
            Else
                tmp = DeleteItem(dsItem.Tables(0))
            End If
            'ghina karame - 11/05/2020- restapicalls- if flag is on and version > 11 then use rest calls instead of soap calls -end

        ElseIf SearchTable = "SKUCATALOGUE" Then
            primKey = "SerialKey"
            If CommonMethods.dbtype <> "sql" Then SearchTable = "System." & SearchTable
            sql += " delete from " & SearchTable & " where " & primKey & " In (" & MyItems & ") "
            tmp = tb.Execute(sql)
        ElseIf SearchTable = "Warehouse_PO" Then
            'ghina karame - 11/05/2020- restapicalls- if flag is on and version > 11 then use rest calls instead of soap calls -begin
            'If useRest = "1" & version >= "11" Then
            '    tmp = DeleteRestPurchaseOrder(MyItems)
            'Else
            tmp = DeletePurchaseOrder(MyItems)
            'End If
            'ghina karame - 11/05/2020- restapicalls- if flag is on and version > 11 then use rest calls instead of soap calls -end

        ElseIf SearchTable = "Warehouse_ASN" Then
            'ghina karame - 11/05/2020- restapicalls- if flag is on and version > 11 then use rest calls instead of soap calls -begin
            If useRest = "1" And version >= "11" Then
                tmp = DeleteRestASN(MyItems)
            Else
                tmp = DeleteASN(MyItems)
            End If
            'ghina karame - 11/05/2020- restapicalls- if flag is on and version > 11 then use rest calls instead of soap calls -end

        ElseIf SearchTable = "Warehouse_SO" Then
                tmp = DeleteSO(MyItems)
            ElseIf SearchTable = "Warehouse_OrderManagement" Then
                primKey = "SerialKey"
            sql += " delete from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAGDETAIL where "
            sql += " WHSEID In (Select WHSEID from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAG where " & primKey & " In (" & MyItems & "))"
            sql += " And ExternOrderKey In (Select ExternOrderKey from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAG where " & primKey & " In (" & MyItems & "))"
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
    Private Function DeletePurchaseOrder(ByVal MyItems As String) As String
        Dim tmp As String = "", AndFilter As String = "", Sql As String = "", Command As String = "", wname As String = "", warehouselevel As String = ""
        Dim wnameRow As DataRow() = Nothing

        Dim dtw As DataTable = CommonMethods.getFacilitiesPerUser(HttpContext.Current.Session("userkey").ToString)
        Dim warehouses(dtw.Rows.Count - 1) As String
        Dim i As Integer = 0

        For Each row As DataRow In dtw.Rows
            warehouses(i) = row("DB_Name").ToString()
            i = i + 1
        Next

        If i > 0 Then
            Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
            Dim suppliers As String() = CommonMethods.getSupplierPerUser(HttpContext.Current.Session("userkey").ToString)

            If owners IsNot Nothing And suppliers IsNot Nothing Then
                Dim ownersstr As String = String.Join("','", owners)
                ownersstr = "'" & ownersstr & "'"
                If Not UCase(ownersstr).Contains("'ALL'") Then AndFilter += " and STORERKEY IN (" & ownersstr & ")"

                Dim consigneesstr As String = String.Join("','", suppliers)
                consigneesstr = "'" & consigneesstr & "'"
                If Not UCase(consigneesstr).Contains("'ALL'") Then AndFilter += " and SellerName IN (" & consigneesstr & ")"

                Sql += " select * from ("
                For Each s As String In warehouses
                    Dim data As DataTable = New DataTable
                    wnameRow = dtw.Select("DB_NAME='" & s & "'")
                    If wnameRow.Count > 0 Then wname = wnameRow(0)!DB_LOGID

                    If LCase(s.Substring(0, 6)) = "infor_" Then
                        warehouselevel = s.Substring(6, s.Length - 6)
                    Else
                        warehouselevel = s
                    End If
                    warehouselevel = warehouselevel.Split("_")(1)

                    Sql += " select SerialKey, '" & wname & "' as Facility, POKey, ExternPOKey "
                    Sql += " from " & warehouselevel & ".po where 1=1  " & AndFilter
                    Sql += " UNION"
                Next
                If Sql.EndsWith("UNION") Then Sql = Sql.Remove(Sql.Length - 5)
                Sql += ") as ds where 1=1 and SerialKey in (" & MyItems & ")"
            End If
        End If
        Dim ds As DataSet = (New SQLExec).Cursor(Sql)

        For i = 0 To ds.Tables(0).Rows.Count - 1
            With ds.Tables(0).Rows(i)
                Command = "<PurchaseOrderHeader><ExternPOKey>" & !ExternPOKey.ToString & "</ExternPOKey><POKey>" & !POKey.ToString & "</POKey></PurchaseOrderHeader>"
                Dim xml As String = "<Message> <Head> <MessageID>0000000003</MessageID> <MessageType>PurchaseOrder</MessageType> <Action>delete</Action> <Sender> <User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>		<TenantId>INFOR</TenantId>		</Sender>		<Recipient>			<SystemID>" & CommonMethods.getFacilityDBName(!Facility) & "</SystemID>		</Recipient>	</Head>	<Body><PurchaseOrder> " & Command & "</PurchaseOrder></Body></Message>"
                tmp = CommonMethods.DeleteXml(xml, !Facility.ToString, "Po", !POKey.ToString)
            End With
        Next
        Return tmp
    End Function
    ' ghina karame - 01/06/2020 - created a function to delete an purchase order through rest call
    'Private Function DeleteRestPurchaseOrder(ByVal MyItems As String) As String
    '    Dim tmp As String = "", AndFilter As String = "", Sql As String = "", Command As String = "", wname As String = "", warehouselevel As String = ""
    '    Dim wnameRow As DataRow() = Nothing
    '    Dim facility As String = ""

    '    Dim dtw As DataTable = CommonMethods.getFacilitiesPerUser(HttpContext.Current.Session("userkey").ToString)
    '    Dim warehouses(dtw.Rows.Count - 1) As String
    '    Dim i As Integer = 0

    '    For Each row As DataRow In dtw.Rows
    '        warehouses(i) = row("DB_Name").ToString()
    '        i = i + 1
    '    Next

    '    If i > 0 Then
    '        Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
    '        Dim suppliers As String() = CommonMethods.getSupplierPerUser(HttpContext.Current.Session("userkey").ToString)

    '        If owners IsNot Nothing And suppliers IsNot Nothing Then
    '            Dim ownersstr As String = String.Join("','", owners)
    '            ownersstr = "'" & ownersstr & "'"
    '            If Not UCase(ownersstr).Contains("'ALL'") Then AndFilter += " and STORERKEY IN (" & ownersstr & ")"

    '            Dim consigneesstr As String = String.Join("','", suppliers)
    '            consigneesstr = "'" & consigneesstr & "'"
    '            If Not UCase(consigneesstr).Contains("'ALL'") Then AndFilter += " and SellerName IN (" & consigneesstr & ")"

    '            Sql += " select * from ("
    '            For Each s As String In warehouses
    '                Dim data As DataTable = New DataTable
    '                wnameRow = dtw.Select("DB_NAME='" & s & "'")
    '                If wnameRow.Count > 0 Then wname = wnameRow(0)!DB_LOGID

    '                If LCase(s.Substring(0, 6)) = "infor_" Then
    '                    warehouselevel = s.Substring(6, s.Length - 6)
    '                Else
    '                    warehouselevel = s
    '                End If
    '                warehouselevel = warehouselevel.Split("_")(1)

    '                Sql += " select SerialKey, '" & wname & "' as Facility, POKey, ExternPOKey "
    '                Sql += " from " & warehouselevel & ".po where 1=1  " & AndFilter
    '                Sql += " UNION"
    '            Next
    '            If Sql.EndsWith("UNION") Then Sql = Sql.Remove(Sql.Length - 5)
    '            Sql += ") as ds where 1=1 and SerialKey in (" & MyItems & ")"
    '        End If
    '    End If
    '    Dim ds As DataSet = (New SQLExec).Cursor(Sql)

    '    For i = 0 To ds.Tables(0).Rows.Count - 1
    '        With ds.Tables(0).Rows(i)
    '            facility = CommonMethods.getFacilityDBName(!Facility)
    '            Command = "/" & facility & "/advancedshipnotice/" & !ReceiptKey.ToString & ""
    '            tmp = CommonMethods.DeleteRest(Command, "ALL", "Item", UCase(!ReceiptKey.ToString))
    '        End With
    '    Next
    '    Return tmp
    'End Function
    Private Function DeleteASN(ByVal MyItems As String) As String
        Dim tmp As String = "", AndFilter As String = "", Sql As String = "", Command As String = "", wname As String = "", warehouselevel As String = ""
        Dim wnameRow As DataRow() = Nothing

        Dim dtw As DataTable = CommonMethods.getFacilitiesPerUser(HttpContext.Current.Session("userkey").ToString)
        Dim warehouses(dtw.Rows.Count - 1) As String
        Dim i As Integer = 0

        For Each row As DataRow In dtw.Rows
            warehouses(i) = row("DB_Name").ToString()
            i = i + 1
        Next

        If i > 0 Then
            Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)

            If owners IsNot Nothing Then
                Dim ownersstr As String = String.Join("','", owners)
                ownersstr = "'" & ownersstr & "'"
                If Not UCase(ownersstr).Contains("'ALL'") Then AndFilter += " and STORERKEY IN (" & ownersstr & ")"

                Sql += " select * from ("
                For Each s As String In warehouses
                    Dim data As DataTable = New DataTable
                    wnameRow = dtw.Select("DB_NAME='" & s & "'")
                    If wnameRow.Count > 0 Then wname = wnameRow(0)!DB_LOGID

                    If LCase(s.Substring(0, 6)) = "infor_" Then
                        warehouselevel = s.Substring(6, s.Length - 6)
                    Else
                        warehouselevel = s
                    End If
                    warehouselevel = warehouselevel.Split("_")(1)

                    Sql += " select SerialKey, '" & wname & "' as Facility, ExternReceiptKey, ReceiptKey "
                    Sql += " from " & warehouselevel & ".Receipt where 1=1  " & AndFilter
                    Sql += " UNION"
                Next
                If Sql.EndsWith("UNION") Then Sql = Sql.Remove(Sql.Length - 5)
                Sql += ") as ds where 1=1 and SerialKey in (" & MyItems & ")"
            End If
        End If
        Dim ds As DataSet = (New SQLExec).Cursor(Sql)

        For i = 0 To ds.Tables(0).Rows.Count - 1
            With ds.Tables(0).Rows(i)
                Command = "<AdvancedShipNoticeHeader><ExternReceiptKey>" & !ExternReceiptKey.ToString & "</ExternReceiptKey><ReceiptKey>" & !ReceiptKey.ToString & "</ReceiptKey></AdvancedShipNoticeHeader>"
                Dim xml As String = "<Message> <Head> <MessageID>0000000003</MessageID> <MessageType>AdvancedShipNotice</MessageType> <Action>delete</Action> <Sender> 	<User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password>	<SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId></Sender>		<Recipient>			<SystemID>" & CommonMethods.getFacilityDBName(!Facility) & "</SystemID>		</Recipient>	</Head>	<Body><AdvancedShipNotice> " & Command & "</AdvancedShipNotice></Body></Message>"
                tmp = CommonMethods.DeleteXml(xml, !Facility.ToString, "Receipt", !ReceiptKey.ToString)
            End With
        Next
        Return tmp
    End Function
    ' ghina karame - 01/06/2020 - created a function to delete an purchase order through rest call
    Private Function DeleteRestASN(ByVal MyItems As String) As String
        Dim tmp As String = "", AndFilter As String = "", Sql As String = "", Command As String = "", wname As String = "", warehouselevel As String = ""
        Dim wnameRow As DataRow() = Nothing
        Dim facility As String = ""

        Dim dtw As DataTable = CommonMethods.getFacilitiesPerUser(HttpContext.Current.Session("userkey").ToString)
        Dim warehouses(dtw.Rows.Count - 1) As String
        Dim i As Integer = 0

        For Each row As DataRow In dtw.Rows
            warehouses(i) = row("DB_Name").ToString()
            i = i + 1
        Next

        If i > 0 Then
            Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)

            If owners IsNot Nothing Then
                Dim ownersstr As String = String.Join("','", owners)
                ownersstr = "'" & ownersstr & "'"
                If Not UCase(ownersstr).Contains("'ALL'") Then AndFilter += " and STORERKEY IN (" & ownersstr & ")"

                Sql += " select * from ("
                For Each s As String In warehouses
                    Dim data As DataTable = New DataTable
                    wnameRow = dtw.Select("DB_NAME='" & s & "'")
                    If wnameRow.Count > 0 Then wname = wnameRow(0)!DB_LOGID

                    If LCase(s.Substring(0, 6)) = "infor_" Then
                        warehouselevel = s.Substring(6, s.Length - 6)
                    Else
                        warehouselevel = s
                    End If
                    warehouselevel = warehouselevel.Split("_")(1)

                    Sql += " select SerialKey, '" & wname & "' as Facility, ExternReceiptKey, ReceiptKey "
                    Sql += " from " & warehouselevel & ".Receipt where 1=1  " & AndFilter
                    Sql += " UNION"
                Next
                If Sql.EndsWith("UNION") Then Sql = Sql.Remove(Sql.Length - 5)
                Sql += ") as ds where 1=1 and SerialKey in (" & MyItems & ")"
            End If
        End If
        Dim ds As DataSet = (New SQLExec).Cursor(Sql)

        For i = 0 To ds.Tables(0).Rows.Count - 1
            With ds.Tables(0).Rows(i)
                facility = CommonMethods.getFacilityDBName(!Facility)
                Command = "/" & facility & "/advancedshipnotice/" & !ReceiptKey.ToString & ""
                tmp = CommonMethods.DeleteRest(Command, "ALL", "Item", UCase(!ReceiptKey.ToString))
            End With
        Next
        Return tmp
    End Function
    Private Function DeleteSO(ByVal MyItems As String) As String
        Dim tmp As String = "", AndFilter As String = "", Sql As String = "", Command As String = "", wname As String = "", warehouselevel As String = ""
        Dim wnameRow As DataRow() = Nothing

        Dim dtw As DataTable = CommonMethods.getFacilitiesPerUser(HttpContext.Current.Session("userkey").ToString)
        Dim warehouses(dtw.Rows.Count - 1) As String
        Dim i As Integer = 0

        For Each row As DataRow In dtw.Rows
            warehouses(i) = row("DB_Name").ToString()
            i = i + 1
        Next

        If i > 0 Then
            Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
            Dim consignees As String() = CommonMethods.getConsigneePerUser(HttpContext.Current.Session("userkey").ToString)

            If owners IsNot Nothing And consignees IsNot Nothing Then
                Dim ownersstr As String = String.Join("','", owners)
                ownersstr = "'" & ownersstr & "'"
                If Not UCase(ownersstr).Contains("'ALL'") Then AndFilter += " and STORERKEY IN (" & ownersstr & ")"

                Dim consigneesstr As String = String.Join("','", consignees)
                consigneesstr = "'" & consigneesstr & "'"
                If Not UCase(consigneesstr).Contains("'ALL'") Then AndFilter += " and ConsigneeKey IN (" & consigneesstr & ")"

                Sql += " select * from ("
                For Each s As String In warehouses
                    Dim data As DataTable = New DataTable
                    wnameRow = dtw.Select("DB_NAME='" & s & "'")
                    If wnameRow.Count > 0 Then wname = wnameRow(0)!DB_LOGID

                    If LCase(s.Substring(0, 6)) = "infor_" Then
                        warehouselevel = s.Substring(6, s.Length - 6)
                    Else
                        warehouselevel = s
                    End If
                    warehouselevel = warehouselevel.Split("_")(1)

                    Sql += " select SerialKey, '" & wname & "' as Facility, ExternOrderKey, OrderKey "
                    Sql += " from " & warehouselevel & ".orders where 1=1  " & AndFilter
                    Sql += " UNION"
                Next
                If Sql.EndsWith("UNION") Then Sql = Sql.Remove(Sql.Length - 5)
                Sql += ") as ds where 1=1 and SerialKey in (" & MyItems & ")"
            End If
        End If
        Dim ds As DataSet = (New SQLExec).Cursor(Sql)

        For i = 0 To ds.Tables(0).Rows.Count - 1
            With ds.Tables(0).Rows(i)
                Command = "<ShipmentOrderHeader><ExternOrderKey>" & !ExternOrderKey.ToString & "</ExternOrderKey><OrderKey>" & !OrderKey.ToString & "</OrderKey></ShipmentOrderHeader>"
                Dim xml As String = "<Message> <Head> <MessageID>0000000003</MessageID> <MessageType>ShipmentOrder</MessageType> <Action>delete</Action> <Sender> 	<User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password>	<SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId></Sender>		<Recipient>			<SystemID>" & CommonMethods.getFacilityDBName(!Facility) & "</SystemID>		</Recipient>	</Head>	<Body><ShipmentOrder> " & Command & "</ShipmentOrder></Body></Message>"
                tmp = CommonMethods.DeleteXml(xml, !Facility.ToString, "Order", !OrderKey.ToString)
            End With
        Next
        Return tmp
    End Function
    'ghina karame - 19/05/2020 - created a function to delete an item through rest call
    Private Function DeleteRestItem(ByVal objTable As DataTable) As String
        Dim tmp As String = "", Command As String = ""
        Dim enterprise As String = CommonMethods.getEnterpriseDBName()
        For i = 0 To objTable.Rows.Count - 1
            Command = "/" & enterprise & "/items/" & objTable.Rows(i)!Sku.ToString & "/" & objTable.Rows(i)!Storerkey.ToString & ""
            tmp = CommonMethods.DeleteRest(Command, "ALL", "Item", UCase(objTable.Rows(i)!StorerKey.ToString) & "-" & UCase(objTable.Rows(i)!Sku.ToString))
        Next
        Return tmp
    End Function

    'ghina karame - 20/05/2020 - created a function to delete a trading partner through rest call
    Private Function DeleteRestTradingPartner(ByVal objTable As DataTable, ByVal type As String) As String
        Dim tmp As String = "", Command As String = ""
        Dim enterprise As String = CommonMethods.getEnterpriseDBName()
        ' if type is ship to
        If type = 2 Then
            For i = 0 To objTable.Rows.Count - 1
                Command = "/" & enterprise & "/customers/" & objTable.Rows(i)!Storerkey.ToString & ""
                tmp = CommonMethods.DeleteRest(Command, "ALL", "Ship To", UCase(objTable.Rows(i)!StorerKey.ToString))
            Next
        End If

        ' if type is ship from 
        If type = 12 Then
            For i = 0 To objTable.Rows.Count - 1
                Command = "/" & enterprise & "/suppliers/" & objTable.Rows(i)!Storerkey.ToString & ""
                tmp = CommonMethods.DeleteRest(Command, "ALL", "Ship From", UCase(objTable.Rows(i)!StorerKey.ToString))
            Next
        End If

        Return tmp
    End Function
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class