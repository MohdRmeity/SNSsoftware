Imports System.IO
Imports Newtonsoft.Json

Public Class ActionButtons
    Implements IHttpHandler
    Implements IRequiresSessionState
    Sub ExecuteAction(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim tmp As String = ""
        Dim SearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim MyItems As String = HttpContext.Current.Request.Item("MyItems")
        Dim ActionID As Integer = Val(HttpContext.Current.Request.Item("ActionID"))

        If SearchTable = "PORTALUSERS" Then
            tmp = ResetUserConfiguration(MyItems)
        ElseIf SearchTable = "Warehouse_SO" Then
            tmp = AllocateSO(MyItems)
        ElseIf SearchTable = "Warehouse_OrderManagement" Then
            If ActionID = 1 Then
                tmp = ReleaseToSCEOrder(MyItems)
            ElseIf ActionID = 2 Then
                tmp = ReleaseAndAllocateOrder(MyItems)
            End If
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
    Private Function ResetUserConfiguration(ByVal MyItems As String) As String
        Dim sql As String = " Delete From PORTALUSERSCONFIGURATION where UserKey in "
        sql += " (select UserKey from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "PORTALUSERS "
        sql += " where ID in (" & MyItems & ")) "
        Return (New SQLExec).Execute(sql)
    End Function
    Private Function AllocateSO(ByVal MyItems As String) As String
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
                Dim xml As String = "<Message> <Head> <MessageID>0000000003</MessageID> <MessageType>ShipmentOrder</MessageType> <Action>allocate</Action> <Sender> 	<User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password>	<SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId></Sender>		<Recipient>			<SystemID>" & CommonMethods.getFacilityDBName(!Facility) & "</SystemID>		</Recipient>	</Head>	<Body><ShipmentOrder> " & Command & "</ShipmentOrder></Body></Message>"
                tmp = CommonMethods.ActionXml(xml)
            End With
        Next
        Return tmp
    End Function
    Private Function ReleaseToSCEOrder(ByVal MyItems As String) As String
        Dim tmp As String = "", ordermanagkey As String = "", warehouse As String = "",
        externorderkey As String = "", wmsokey As String = "", Owner As String = ""
        Dim sql As String = "select * from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAG where SerialKey in (" & MyItems & ")"
        sql += " select * from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAGDETAIL "
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        Dim HeaderTable As DataTable = ds.Tables(0)
        Dim DetailsTable As DataTable = ds.Tables(1)
        For i = 0 To HeaderTable.Rows.Count - 1
            With HeaderTable.Rows(i)
                warehouse = !WHSEID.ToString()
                ordermanagkey = !ORDERMANAGKEY.ToString()
                If !ORDERMANAGSTATUS.ToString = "NOT CREATED in SCE" Then
                    tmp = ReleaseToSCE(ordermanagkey, warehouse, HeaderTable, DetailsTable, False)
                Else
                    Dim exist As Integer = CommonMethods.checkNewLines(ordermanagkey, warehouse)
                    If exist > 0 Then
                        externorderkey = !EXTERNORDERKEY.ToString()
                        wmsokey = !NOTES2.ToString()
                        Owner = !StorerKey.ToString()
                        Dim state As Boolean = CommonMethods.CheckOrderState(warehouse, ordermanagkey)
                        If state Then
                            tmp = AddLines(ordermanagkey, externorderkey, wmsokey, Owner, warehouse, DetailsTable)
                            If tmp = "" Then
                                tmp = OrderStatusUpdate(ordermanagkey, warehouse, "Created in SCE", wmsokey)
                            End If
                        Else
                            tmp = "Order has been processed in SCE and cannot be edited <br/>"
                        End If
                    Else
                        tmp = "Error: Order is already Created in SCE! <br/>"
                    End If
                End If
            End With
        Next
        Return tmp
    End Function
    Private Function ReleaseAndAllocateOrder(ByVal MyItems As String) As String
        Dim tmp As String = "", ordermanagkey As String = "", warehouse As String = ""
        Dim sql As String = "select * from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAG where SerialKey in (" & MyItems & ")"
        sql += " select * from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAGDETAIL "
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        Dim HeaderTable As DataTable = ds.Tables(0)
        Dim DetailsTable As DataTable = ds.Tables(1)
        For i = 0 To HeaderTable.Rows.Count - 1
            With HeaderTable.Rows(i)
                warehouse = !WHSEID.ToString()
                ordermanagkey = !ORDERMANAGKEY.ToString()
                If !ORDERMANAGSTATUS.ToString = "NOT CREATED in SCE" Then
                    tmp = ReleaseToSCE(ordermanagkey, warehouse, HeaderTable, DetailsTable, True)
                Else
                    tmp = "Error: Order is already Created in SCE! <br/>"
                End If
            End With
        Next
        Return tmp
    End Function
    Private Function ReleaseToSCE(ByVal ordermanagkey As String, ByVal warehouse As String, ByVal HeadeTable As DataTable, ByVal DetailsTable As DataTable, ByVal alloc As Boolean) As String
        Dim tmp As String = "", OrderXml As String = "", DetailsXml As String = "",
        externokey As String = "", reqdate As String = "", Owner As String = ""
        Dim HeaderRow As DataRow() = HeadeTable.Select("OrderManagKey = '" & ordermanagkey & "' and WHSEID = '" & warehouse & "'")
        Dim DetailsRow As DataRow() = DetailsTable.Select("OrderManagKey = '" & ordermanagkey & "' and WHSEID = '" & warehouse & "'")

        For i = 0 To DetailsRow.Count - 1
            With DetailsRow(i)
                DetailsXml += "<ShipmentOrderDetail><ExternLineNo>" & !ExternLineNo.ToString() & "</ExternLineNo>" &
                "<StorerKey>" & !STORERKEY.ToString() & "</StorerKey><Sku>" & !SKU.ToString() & "</Sku>" &
                "<OpenQty>" & !OpenQty.ToString() & "</OpenQty>" & "<UnitPrice>" & !unitprice.ToString() & "</UnitPrice>" &
                "<PackKey>" & !PackKey.ToString() & "</PackKey>" & "<UOM>" & !UOM.ToString() & "</UOM>" & "<SUsr1>" & !SUSR1.ToString() & "</SUsr1>" &
                 "<SUsr2>" & !SUSR2.ToString() & "</SUsr2>" & "<SUsr3>" & !SUSR3.ToString() & "</SUsr3>" &
                 "<SUsr4>" & !SUSR4.ToString() & "</SUsr4>" & "<SUsr5>" & !SUSR5.ToString() & "</SUsr5>" &
                 "<Lottable01>" & !Lottable01.ToString() & "</Lottable01>" & "<Lottable02>" & !Lottable02.ToString() & "</Lottable02>" &
                 "<Lottable03>" & !Lottable03.ToString() & "</Lottable03>" & "<Lottable04>" & !Lottable04.ToString() & "</Lottable04>" &
                 "<Lottable05>" & !Lottable05.ToString() & "</Lottable05>" & "<Lottable06>" & !Lottable06.ToString() & "</Lottable06>" &
                 "<Lottable07>" & !Lottable07.ToString() & "</Lottable07>" & "<Lottable08>" & !Lottable08.ToString() & "</Lottable08>" &
                 "<Lottable09>" & !Lottable09.ToString() & "</Lottable09>" & "<Lottable10>" & !Lottable10.ToString() & "</Lottable10>" & "</ShipmentOrderDetail>"
            End With
        Next

        For i = 0 To HeaderRow.Count - 1
            With HeaderRow(i)
                externokey = !EXTERNORDERKEY.ToString()
                Owner = !STORERKEY.ToString()
                If Not String.IsNullOrEmpty(!RequestedShipDate.ToString()) Then
                    Dim DateTime As DateTime = DateTime.ParseExact(!RequestedShipDate.ToString(), "M/d/yyyy h:mm:ss tt", Nothing)
                    reqdate = DateTime.ToString("dd/MM/yyyy HH:mm:ss")
                End If
                OrderXml += "<ShipmentOrder><ShipmentOrderHeader><ExternOrderKey>" & externokey & "</ExternOrderKey>" &
                "<StorerKey>" & Owner & "</StorerKey><Type>" & !Type.ToString() & "</Type>" &
                "<ConsigneeKey>" & !ConsigneeKey.ToString() & "</ConsigneeKey>" & "<RequestedShipDate>" & reqdate & "</RequestedShipDate>" &
                "<SUsr1>" & !SUSR1.ToString() & "</SUsr1>" & "<SUsr2>" & !SUSR2.ToString() & "</SUsr2>" &
                "<SUsr3>" & !SUSR3.ToString() & "</SUsr3>" & "<SUsr4>" & !SUSR4.ToString() & "</SUsr4>" &
                "<SUsr5>" & !SUSR5.ToString() & "</SUsr5>"
                OrderXml += DetailsXml & "</ShipmentOrderHeader></ShipmentOrder>"
            End With
        Next

        tmp = StoreOrderApi(ordermanagkey, warehouse, externokey, Owner, OrderXml, alloc)

        Return tmp
    End Function
    Private Function StoreOrderApi(ByVal ordermanagkey As String, ByVal warehouse As String, ByVal externokey As String, ByVal Owner As String, ByVal OrderXml As String, ByVal allocate As Boolean) As String
        Dim tmp As String = ""
        Dim exist As Integer = CommonMethods.checkSOExist(warehouse, ordermanagkey)
        If exist = 0 Then
            Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>ShipmentOrder</MessageType><Action>store</Action><Sender><User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId>	</Sender><Recipient><SystemID>" & warehouse & "</SystemID></Recipient></Head><Body>" & OrderXml & "</Body></Message>"
            tmp = CommonMethods.ActionXml(Xml)
            If tmp = "" Then
                Dim okey As String = CommonMethods.getwmsOkeyFromWMS(externokey, warehouse)
                If allocate Then
                    Dim order As String = "<ShipmentOrderHeader><ExternOrderKey>" + externokey + "</ExternOrderKey><StorerKey>" & Owner & "</StorerKey></ShipmentOrderHeader>"
                    Xml = "<Message> <Head> <MessageID>0000000003</MessageID> <MessageType>ShipmentOrder</MessageType> <Action>allocate</Action> <Sender> 	<User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>			<TenantId>INFOR</TenantId>	</Sender>		<Recipient>			<SystemID>" & warehouse & "</SystemID>		</Recipient>	</Head>	<Body><ShipmentOrder> " & order & "</ShipmentOrder></Body></Message>"
                    tmp = CommonMethods.ActionXml(Xml)
                    If tmp = "" Then
                        tmp = OrderStatusUpdate(ordermanagkey, warehouse, "CREATED & ALLOCATED", okey)
                    Else
                        tmp = "Order Created in SCE but not allocated due to " & tmp & "<br/>"
                        tmp += OrderStatusUpdate(ordermanagkey, warehouse, "Created in SCE", okey)
                    End If
                Else
                    tmp = OrderStatusUpdate(ordermanagkey, warehouse, "Created in SCE", okey)
                End If
            End If
        Else
            tmp = "Extern OrderKey already exists in SCE! <br/>"
        End If
        Return tmp
    End Function
    Private Function OrderStatusUpdate(ByVal ordermanagkey As String, ByVal warehouse As String, ByVal status As String, ByVal okey As String) As String
        Dim sql As String = " Update " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ordermanag "
        sql += " set ORDERMANAGSTATUS= '" & status & " , notes2= '" & okey & "'"
        sql += " where  ORDERMANAGKEY= '" & ordermanagkey & " and WHSEID= '" & warehouse & "'"
        sql += " Update " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ordermanagdetail "
        sql += " set Status = '04' "
        sql += " where  ORDERMANAGKEY= '" & ordermanagkey & " and WHSEID= '" & warehouse & "'"
        Return (New SQLExec).Execute(sql)
    End Function
    Private Function AddLines(ByVal ordermanagkey As String, ByVal externokey As String, ByVal wmsokey As String, ByVal Owner As String, ByVal warehouse As String, ByVal DetailsTable As DataTable) As String
        Dim tmp As String = "", DetailsXml As String = ""
        Dim DetailsRow As DataRow() = DetailsTable.Select("OrderManagKey = '" & ordermanagkey & "' and WHSEID = '" & warehouse & "' and Status = '02'")

        DetailsXml = "<ShipmentOrder><ShipmentOrderHeader><ExternOrderKey>" & externokey & "</ExternOrderKey><OrderKey>" & wmsokey & "</OrderKey><StorerKey>" & Owner & "</StorerKey>"
        For i = 0 To DetailsRow.Count - 1
            With DetailsRow(i)
                DetailsXml += "<ShipmentOrderDetail><ExternLineNo>" & !ExternLineNo.ToString() & "</ExternLineNo>" &
                "<StorerKey>" & !STORERKEY.ToString() & "</StorerKey><Sku>" & !SKU.ToString() & "</Sku>" &
                "<OpenQty>" & !OpenQty.ToString() & "</OpenQty>" & "<UnitPrice>" & !unitprice.ToString() & "</UnitPrice>" &
                "<PackKey>" & !PackKey.ToString() & "</PackKey>" & "<UOM>" & !UOM.ToString() & "</UOM>" & "<SUsr1>" & !SUSR1.ToString() & "</SUsr1>" &
                 "<SUsr2>" & !SUSR2.ToString() & "</SUsr2>" & "<SUsr3>" & !SUSR3.ToString() & "</SUsr3>" &
                 "<SUsr4>" & !SUSR4.ToString() & "</SUsr4>" & "<SUsr5>" & !SUSR5.ToString() & "</SUsr5>" &
                 "<Lottable01>" & !Lottable01.ToString() & "</Lottable01>" & "<Lottable02>" & !Lottable02.ToString() & "</Lottable02>" &
                 "<Lottable03>" & !Lottable03.ToString() & "</Lottable03>" & "<Lottable04>" & !Lottable04.ToString() & "</Lottable04>" &
                 "<Lottable05>" & !Lottable05.ToString() & "</Lottable05>" & "<Lottable06>" & !Lottable06.ToString() & "</Lottable06>" &
                 "<Lottable07>" & !Lottable07.ToString() & "</Lottable07>" & "<Lottable08>" & !Lottable08.ToString() & "</Lottable08>" &
                 "<Lottable09>" & !Lottable09.ToString() & "</Lottable09>" & "<Lottable10>" & !Lottable10.ToString() & "</Lottable10>" & "</ShipmentOrderDetail>"
            End With
        Next
        DetailsXml += "</ShipmentOrderHeader></ShipmentOrder>"

        Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>ShipmentOrder</MessageType><Action>store</Action><Sender><User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId>	</Sender><Recipient><SystemID>" & warehouse & "</SystemID></Recipient></Head><Body>" & DetailsXml & "</Body></Message>"
        tmp = CommonMethods.ActionXml(Xml)

        Return tmp
    End Function

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class