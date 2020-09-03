
Imports System.IO
Imports System.Xml
Imports Newtonsoft.Json

Public Class DisplayItemsNew
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub DisplayItem(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim tb As SQLExec = New SQLExec
        Dim mySearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim MyID As String = HttpContext.Current.Request.Item("MyID")
        Dim MyError As String = ""
        Dim SavedFields As String = ""
        Dim ReadOnlyFields As String = ""
        Dim AndFilter As String = ""
        Dim Sql As String = ""

        Dim CarrierEventsNbr As Integer = 0
        Dim CarrierRecords As String = ""

        If MyID.Contains("?") Then
            MyID = CommonMethods.GetMyID(mySearchTable, MyID)
        End If

        Dim primKey As String = "SerialKey"
        If mySearchTable.Contains("enterprise.storer") Then
            Dim type As String = IIf(mySearchTable = "enterprise.storer2", "2", "12")
            AndFilter = " and Type = '" & type & "'"
            mySearchTable = "enterprise.storer"
        ElseIf mySearchTable = "enterprise.sku" Then
            primKey = "SerialKey"
        ElseIf mySearchTable = "SKUCATALOGUE" Then
            If CommonMethods.dbtype <> "sql" Then mySearchTable = "System." & mySearchTable
        ElseIf mySearchTable = "Warehouse_PO" Then
            GetPurchaseOrderQuery(Sql, MyID)
            Sql += "select * from PO_FILES"
        ElseIf mySearchTable = "Warehouse_ASN" Then
            GetASNQuery(Sql, MyID)
            Sql += "select * from RECEIPT_FILES"
        ElseIf mySearchTable = "Warehouse_SO" Then
            GetSOQuery(Sql, MyID)
            Sql += "select * from ORDERS_FILES"
        ElseIf mySearchTable = "Warehouse_OrderManagement" Then
            primKey = "SerialKey"
            mySearchTable = "ORDERMANAG"
            If CommonMethods.dbtype <> "sql" Then mySearchTable = "System." & mySearchTable
        ElseIf mySearchTable = "Warehouse_OrderTracking" Then
            GetOrderTrackingQuery(Sql, MyID)
        End If

        If mySearchTable <> "Inventory_Balance" And mySearchTable <> "Warehouse_PO" And mySearchTable <> "Warehouse_ASN" And mySearchTable <> "Warehouse_SO" And mySearchTable <> "Warehouse_OrderTracking" Then
            Sql += " Select * from " & mySearchTable & " where " & primKey & " = " & MyID & AndFilter
        End If

        If mySearchTable.Contains("ORDERMANAG") Then
            Sql += "select * from ORDERMANAG_FILES"
        End If

        Dim ds As DataSet = tb.Cursor(Sql)

        If ds.Tables(0).Rows.Count = 0 Then MyError = "This record does not exist"

        'If Val(MyID) > 0 Then
        If ds.Tables(0).Rows.Count > 0 Then
            Select Case mySearchTable
                Case "enterprise.storer"
                    With ds.Tables(0).Rows(0)
                        If Not .IsNull("StorerKey") Then
                            SavedFields += "StorerKey:::" & !StorerKey
                            ReadOnlyFields += "StorerKey"
                        End If

                        If Not .IsNull("Company") Then
                            SavedFields += ";;;Company:::" & !Company
                        End If

                        If Not .IsNull("Description") Then
                            SavedFields += ";;;Description:::" & !Description
                        End If

                        If Not .IsNull("Country") Then
                            SavedFields += ";;;Country:::" & !Country
                        End If

                        If Not .IsNull("City") Then
                            SavedFields += ";;;City:::" & !City
                        End If

                        If Not .IsNull("State") Then
                            SavedFields += ";;;State:::" & !State
                        End If

                        If Not .IsNull("Zip") Then
                            SavedFields += ";;;Zip:::" & !Zip
                        End If

                        If Not .IsNull("Address1") Then
                            SavedFields += ";;;Address1:::" & !Address1
                        End If

                        If Not .IsNull("Address2") Then
                            SavedFields += ";;;Address2:::" & !Address2
                        End If

                        If Not .IsNull("Address3") Then
                            SavedFields += ";;;Address3:::" & !Address3
                        End If

                        If Not .IsNull("Address4") Then
                            SavedFields += ";;;Address4:::" & !Address4
                        End If

                        If Not .IsNull("Address5") Then
                            SavedFields += ";;;Address5:::" & !Address5
                        End If

                        If Not .IsNull("Address6") Then
                            SavedFields += ";;;Address6:::" & !Address6
                        End If

                        If Not .IsNull("Contact1") Then
                            SavedFields += ";;;Contact1:::" & !Contact1
                        End If

                        If Not .IsNull("Contact2") Then
                            SavedFields += ";;;Contact2:::" & !Contact2
                        End If

                        If Not .IsNull("Email1") Then
                            SavedFields += ";;;Email1:::" & !Email1
                        End If

                        If Not .IsNull("Email2") Then
                            SavedFields += ";;;Email2:::" & !Email2
                        End If

                        If Not .IsNull("Phone1") Then
                            SavedFields += ";;;Phone1:::" & !Phone1
                        End If

                        If Not .IsNull("Phone2") Then
                            SavedFields += ";;;Phone2:::" & !Phone2
                        End If

                        If Not .IsNull("SUsr1") Then
                            SavedFields += ";;;SUsr1:::" & !SUsr1
                        End If

                        If Not .IsNull("SUsr2") Then
                            SavedFields += ";;;SUsr2:::" & !SUsr2
                        End If

                        If Not .IsNull("SUsr3") Then
                            SavedFields += ";;;SUsr3:::" & !SUsr3
                        End If

                        If Not .IsNull("SUsr4") Then
                            SavedFields += ";;;SUsr4:::" & !SUsr4
                        End If

                        If Not .IsNull("SUsr5") Then
                            SavedFields += ";;;SUsr5:::" & !SUsr5
                        End If
                    End With
                Case "enterprise.sku"
                    With ds.Tables(0).Rows(0)
                        If Not .IsNull("StorerKey") Then
                            SavedFields += "StorerKey:::" & !StorerKey
                            ReadOnlyFields += "StorerKey"
                        End If

                        If Not .IsNull("Sku") Then
                            SavedFields += ";;;Sku:::" & !Sku
                            ReadOnlyFields += "~~~Sku"
                        End If

                        If Not .IsNull("Descr") Then
                            SavedFields += ";;;Descr:::" & !Descr
                        End If

                        If Not .IsNull("PackKey") Then
                            SavedFields += ";;;PackKey:::" & !PackKey
                        End If

                        'Mohamad Rmeity - Removing Tariff Key from items screen
                        'If Not .IsNull("TariffKey") Then
                        '    SavedFields += ";;;TariffKey:::" & !TariffKey
                        'End If

                        If Not .IsNull("StdCube") Then
                            SavedFields += ";;;StdCube:::" & !StdCube
                        End If

                        If Not .IsNull("StdNetWgt") Then
                            SavedFields += ";;;StdNetWgt:::" & !StdNetWgt
                        End If

                        If Not .IsNull("StdGrossWgt") Then
                            SavedFields += ";;;StdGrossWgt:::" & !StdGrossWgt
                        End If

                        If Not .IsNull("SkuGroup") Then
                            SavedFields += ";;;SkuGroup:::" & !SkuGroup
                        End If
                    End With
                Case "SKUCATALOGUE", "SYSTEM.SKUCATALOGUE"
                    With ds.Tables(0).Rows(0)
                        If Not .IsNull("StorerKey") Then
                            SavedFields += "StorerKey:::" & !StorerKey
                            ReadOnlyFields += "StorerKey"
                        End If

                        If Not .IsNull("ConsigneeKey") Then
                            SavedFields += ";;;ConsigneeKey:::" & !ConsigneeKey
                            ReadOnlyFields += "~~~ConsigneeKey"
                        End If

                        If Not .IsNull("Sku") Then
                            SavedFields += ";;;Sku:::" & !Sku
                            ReadOnlyFields += "~~~Sku"
                        End If

                        If Not .IsNull("Price") Then
                            SavedFields += ";;;Price:::" & !Price
                        End If

                        If Not .IsNull("Currency") Then
                            SavedFields += ";;;Currency:::" & !Currency
                            ReadOnlyFields += "~~~Currency"
                        End If

                        If Not .IsNull("SUsr1") Then
                            SavedFields += ";;;SUsr1:::" & !SUsr1
                        End If

                        If Not .IsNull("SUsr2") Then
                            SavedFields += ";;;SUsr2:::" & !SUsr2
                        End If

                        If Not .IsNull("SUsr3") Then
                            SavedFields += ";;;SUsr3:::" & !SUsr3
                        End If

                        If Not .IsNull("SUsr4") Then
                            SavedFields += ";;;SUsr4:::" & !SUsr4
                        End If

                        If Not .IsNull("SUsr5") Then
                            SavedFields += ";;;SUsr5:::" & !SUsr5
                        End If
                    End With
                Case "Warehouse_PO"
                    With ds.Tables(0).Rows(0)
                        Dim Facility As String = ""
                        Dim POKey As String = ""
                        If Not .IsNull("Facility") Then Facility = !Facility
                        If Not .IsNull("POKey") Then POKey = !POKey

                        Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>PurchaseOrder</MessageType><Action>list</Action><Sender><User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId>	</Sender><Recipient><SystemID>" & CommonMethods.getFacilityDBName(Facility) & "</SystemID></Recipient></Head><Body><PurchaseOrder>			<PurchaseOrderHeader>  <POKey>" & POKey & "</POKey> </PurchaseOrderHeader>		</PurchaseOrder></Body></Message>"
                        Dim ViewTable As DataTable = CommonMethods.ViewXml(Xml)
                        MyError = ViewTable.Rows(0)!error
                        Dim MyDoc As XmlDocument = ViewTable.Rows(0)!doc
                        If MyError = "" Then
                            SavedFields += "Facility:::" & Facility
                            ReadOnlyFields += "Facility"

                            SavedFields += ";;;POKey:::" & POKey
                            ReadOnlyFields += "~~~POKey"

                            Dim nodeReader As XmlNodeReader = New XmlNodeReader(MyDoc)
                            Dim results As XmlNodeList = MyDoc.SelectNodes("//*[local-name()='PurchaseOrderHeader']")
                            For Each node As XmlNode In results
                                If Not node("ExternPOKey").IsEmpty Then
                                    SavedFields += ";;;ExternPOKey:::" & node("ExternPOKey").InnerText.ToString()
                                    ReadOnlyFields += "~~~ExternPOKey"
                                End If

                                If Not node("PODate").IsEmpty Then
                                    Dim DateTime As DateTime
                                    Dim DateStr = node("PODate").InnerText.ToString()
                                    Dim dformat As String = CommonMethods.datef
                                    If Not String.IsNullOrEmpty(dformat) Then
                                        DateTime = DateTime.ParseExact(DateStr, dformat, Nothing)
                                    Else
                                        If Double.Parse(CommonMethods.version) >= 11 Then
                                            DateTime = DateTime.ParseExact(DateStr, "MM/dd/yyyy HH:mm:ss", Nothing)
                                        Else
                                            DateTime = DateTime.ParseExact(DateStr, "dd/MM/yyyy HH:mm:ss", Nothing)
                                        End If
                                    End If
                                    SavedFields += ";;;PODate:::" & DateTime.ToString("MM/dd/yyyy hh:mm:ss tt")
                                    ReadOnlyFields += "~~~PODate"
                                End If

                                If Not node("Status").IsEmpty Then
                                    Dim code As Integer = Integer.Parse(node("Status").InnerText.ToString())
                                    SavedFields += ";;;Status:::" & CommonMethods.getStatusPo(code)
                                    ReadOnlyFields += "~~~Status"
                                End If

                                If Not node("StorerKey").IsEmpty Then
                                    SavedFields += ";;;StorerKey:::" & node("StorerKey").InnerText.ToString()
                                    ReadOnlyFields += "~~~StorerKey"
                                End If

                                If Not node("BuyerName").IsEmpty Then
                                    SavedFields += ";;;BuyerName:::" & node("BuyerName").InnerText.ToString()
                                End If

                                If Not node("BuyersReference").IsEmpty Then
                                    SavedFields += ";;;BuyersReference:::" & node("BuyersReference").InnerText.ToString()
                                End If

                                If Not node("SellerName").IsEmpty Then
                                    SavedFields += ";;;SellerName:::" & node("SellerName").InnerText.ToString()
                                End If

                                If Not node("SellersReference").IsEmpty Then
                                    SavedFields += ";;;SellersReference:::" & node("SellersReference").InnerText.ToString()
                                End If

                                If Not node("POType").IsEmpty Then
                                    Dim code As Integer = Integer.Parse(node("POType").InnerText.ToString())
                                    SavedFields += ";;;POType:::" & CommonMethods.getTypePo(code)
                                    ReadOnlyFields += "~~~POType"
                                End If

                                If Not node("EffectiveDate").IsEmpty Then
                                    Dim DateTime As DateTime
                                    Dim DateStr = node("EffectiveDate").InnerText.ToString()
                                    Dim dformat As String = CommonMethods.datef
                                    If Not String.IsNullOrEmpty(dformat) Then
                                        DateTime = DateTime.ParseExact(DateStr, dformat, Nothing)
                                    Else
                                        If Double.Parse(CommonMethods.version) >= 11 Then
                                            DateTime = DateTime.ParseExact(DateStr, "MM/dd/yyyy HH:mm:ss", Nothing)
                                        Else
                                            DateTime = DateTime.ParseExact(DateStr, "dd/MM/yyyy HH:mm:ss", Nothing)
                                        End If
                                    End If
                                    SavedFields += ";;;EffectiveDate:::" & DateTime.ToString("MM/dd/yyyy")
                                End If

                                If Not node("SUsr1").IsEmpty Then
                                    SavedFields += ";;;SUsr1:::" & node("SUsr1").InnerText.ToString()
                                End If

                                If Not node("SUsr2").IsEmpty Then
                                    SavedFields += ";;;SUsr2:::" & node("SUsr2").InnerText.ToString()
                                End If

                                If Not node("SUsr3").IsEmpty Then
                                    SavedFields += ";;;SUsr3:::" & node("SUsr3").InnerText.ToString()
                                End If

                                If Not node("SUsr4").IsEmpty Then
                                    SavedFields += ";;;SUsr4:::" & node("SUsr4").InnerText.ToString()
                                End If

                                If Not node("SUsr5").IsEmpty Then
                                    SavedFields += ";;;SUsr5:::" & node("SUsr5").InnerText.ToString()
                                End If
                            Next
                        End If
                    End With
                Case "Warehouse_ASN"
                    With ds.Tables(0).Rows(0)
                        Dim Facility As String = ""
                        Dim ReceiptKey As String = ""
                        Dim warehouse As String = ""
                        If Not .IsNull("Facility") Then
                            Facility = !Facility
                            warehouse = CommonMethods.getFacilityDBName(Facility)
                        End If
                        If Not .IsNull("ReceiptKey") Then ReceiptKey = !ReceiptKey

                        Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>AdvancedShipNotice</MessageType><Action>list</Action><Sender>	<User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId>	</Sender><Recipient><SystemID>" & warehouse & "</SystemID></Recipient></Head><Body><AdvancedShipNotice><AdvancedShipNoticeHeader>  <ReceiptKey>" & ReceiptKey & "</ReceiptKey> </AdvancedShipNoticeHeader>		</AdvancedShipNotice></Body></Message>"
                        Dim ViewTable As DataTable = CommonMethods.ViewXml(Xml)
                        MyError = ViewTable.Rows(0)!error
                        Dim MyDoc As XmlDocument = ViewTable.Rows(0)!doc
                        If MyError = "" Then
                            SavedFields += "Facility:::" & Facility
                            ReadOnlyFields += "Facility"

                            SavedFields += ";;;ReceiptKey:::" & ReceiptKey
                            ReadOnlyFields += "~~~ReceiptKey"

                            Dim nodeReader As XmlNodeReader = New XmlNodeReader(MyDoc)
                            Dim results As XmlNodeList = MyDoc.SelectNodes("//*[local-name()='AdvancedShipNoticeHeader']")
                            For Each node As XmlNode In results
                                If Not node("ExternReceiptKey").IsEmpty Then
                                    SavedFields += ";;;ExternReceiptKey:::" & node("ExternReceiptKey").InnerText.ToString()
                                    ReadOnlyFields += "~~~ExternReceiptKey"
                                End If

                                If Not node("ReceiptDate").IsEmpty Then
                                    Dim DateTime As DateTime
                                    Dim DateStr = node("ReceiptDate").InnerText.ToString()
                                    Dim dformat As String = CommonMethods.datef
                                    If Not String.IsNullOrEmpty(dformat) Then
                                        DateTime = DateTime.ParseExact(DateStr, dformat, Nothing)
                                    Else
                                        If Double.Parse(CommonMethods.version) >= 11 Then
                                            DateTime = DateTime.ParseExact(DateStr, "MM/dd/yyyy HH:mm:ss", Nothing)
                                        Else
                                            DateTime = DateTime.ParseExact(DateStr, "dd/MM/yyyy HH:mm:ss", Nothing)
                                        End If
                                    End If
                                    SavedFields += ";;;ReceiptDate:::" & DateTime.ToString("MM/dd/yyyy hh:mm:ss tt")
                                    ReadOnlyFields += "~~~ReceiptDate"
                                End If

                                If Not node("Status").IsEmpty Then
                                    Dim DTReceiptStatus = CommonMethods.getCodeDD(warehouse, "codelkup", "RECSTATUS")
                                    Dim DRReceiptStatus() As DataRow = DTReceiptStatus.Select("CODE='" & node("Status").InnerText.ToString() & "'")
                                    If DRReceiptStatus.Length > 0 Then
                                        SavedFields += ";;;Status:::" & DRReceiptStatus(0)!DESCRIPTION
                                    End If
                                    ReadOnlyFields += "~~~Status"
                                End If

                                If Not node("Type").IsEmpty Then
                                    Dim DTReceiptType = CommonMethods.getCodeDD(warehouse, "codelkup", "RECEIPTYPE")
                                    Dim DRReceiptType() As DataRow = DTReceiptType.Select("CODE='" & node("Type").InnerText.ToString() & "'")
                                    If DRReceiptType.Length > 0 Then
                                        SavedFields += ";;;ReceiptType:::" & DRReceiptType(0)!DESCRIPTION
                                    End If
                                    ReadOnlyFields += "~~~ReceiptType"
                                End If

                                If Not node("StorerKey").IsEmpty Then
                                    SavedFields += ";;;StorerKey:::" & node("StorerKey").InnerText.ToString()
                                    ReadOnlyFields += "~~~StorerKey"
                                End If

                                If Not node("POKey").IsEmpty Then
                                    SavedFields += ";;;POKey:::" & node("POKey").InnerText.ToString()
                                End If

                                If Not node("CarrierKey").IsEmpty Then
                                    SavedFields += ";;;CarrierKey:::" & node("CarrierKey").InnerText.ToString()
                                End If

                                If Not node("WarehouseReference").IsEmpty Then
                                    SavedFields += ";;;WarehouseReference:::" & node("WarehouseReference").InnerText.ToString()
                                End If

                                If Not node("ContainerKey").IsEmpty Then
                                    SavedFields += ";;;ContainerKey:::" & node("ContainerKey").InnerText.ToString()
                                End If

                                If Not node("ContainerType").IsEmpty Then
                                    SavedFields += ";;;ContainerType:::" & node("ContainerType").InnerText.ToString()
                                End If

                                If Not node("OriginCountry").IsEmpty Then
                                    SavedFields += ";;;OriginCountry:::" & CommonMethods.getISOCountryName(node("OriginCountry").InnerText.ToString())
                                End If

                                If Not node("TransportationMode").IsEmpty Then
                                    SavedFields += ";;;TransportationMode:::" & node("TransportationMode").InnerText.ToString()
                                End If
                            Next
                        End If
                    End With
                Case "Warehouse_SO"
                    With ds.Tables(0).Rows(0)
                        Dim Facility As String = ""
                        Dim OrderKey As String = ""
                        Dim warehouse As String = ""
                        If Not .IsNull("Facility") Then
                            Facility = !Facility
                            warehouse = CommonMethods.getFacilityDBName(Facility)
                        End If
                        If Not .IsNull("OrderKey") Then OrderKey = !OrderKey

                        Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>ShipmentOrder</MessageType><Action>list</Action><Sender>	<User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId>	</Sender><Recipient><SystemID>" & warehouse & "</SystemID></Recipient></Head><Body><ShipmentOrder><ShipmentOrderHeader>  <OrderKey>" & OrderKey & "</OrderKey> </ShipmentOrderHeader>		</ShipmentOrder></Body></Message>"
                        Dim ViewTable As DataTable = CommonMethods.ViewXml(Xml)
                        MyError = ViewTable.Rows(0)!error
                        Dim MyDoc As XmlDocument = ViewTable.Rows(0)!doc
                        If MyError = "" Then
                            SavedFields += "Facility:::" & Facility
                            ReadOnlyFields += "Facility"

                            SavedFields += ";;;OrderKey:::" & OrderKey
                            ReadOnlyFields += "~~~OrderKey"

                            Dim nodeReader As XmlNodeReader = New XmlNodeReader(MyDoc)
                            Dim results As XmlNodeList = MyDoc.SelectNodes("//*[local-name()='ShipmentOrderHeader']")
                            For Each node As XmlNode In results
                                If Not node("ExternOrderKey").IsEmpty Then
                                    SavedFields += ";;;ExternOrderKey:::" & node("ExternOrderKey").InnerText.ToString()
                                    ReadOnlyFields += "~~~ExternOrderKey"
                                End If

                                If Not node("OrderDate").IsEmpty Then
                                    Dim DateTime As DateTime
                                    Dim DateStr = node("OrderDate").InnerText.ToString()
                                    'If DateStr.Contains(" ") Then DateStr = DateStr.Substring(0, DateStr.IndexOf(" "))
                                    Dim dformat As String = CommonMethods.datef
                                    If Not String.IsNullOrEmpty(dformat) Then
                                        DateTime = DateTime.ParseExact(DateStr, dformat, Nothing)
                                    Else
                                        If Double.Parse(CommonMethods.version) >= 11 Then
                                            DateTime = DateTime.ParseExact(DateStr, "MM/dd/yyyy HH:mm:ss", Nothing)
                                        Else
                                            DateTime = DateTime.ParseExact(DateStr, "dd/MM/yyyy HH:mm:ss", Nothing)
                                        End If
                                    End If
                                    SavedFields += ";;;OrderDate:::" & DateTime.ToString("MM/dd/yyyy hh:mm:ss tt")
                                    ReadOnlyFields += "~~~OrderDate"
                                End If

                                If Not node("RequestedShipDate").IsEmpty Then
                                    Dim DateTime As DateTime
                                    Dim DateStr = node("RequestedShipDate").InnerText.ToString()
                                    'If DateStr.Contains(" ") Then DateStr = DateStr.Substring(0, DateStr.IndexOf(" "))
                                    Dim dformat As String = CommonMethods.datef
                                    If Not String.IsNullOrEmpty(dformat) Then
                                        DateTime = DateTime.ParseExact(DateStr, dformat, Nothing)
                                    Else
                                        If Double.Parse(CommonMethods.version) >= 11 Then
                                            DateTime = DateTime.ParseExact(DateStr, "MM/dd/yyyy HH:mm:ss", Nothing)
                                        Else
                                            DateTime = DateTime.ParseExact(DateStr, "dd/MM/yyyy HH:mm:ss", Nothing)
                                        End If
                                    End If
                                    SavedFields += ";;;RequestedShipDate:::" & DateTime.ToString("MM/dd/yyyy")
                                End If

                                If Not node("ActualShipDate").IsEmpty Then
                                    Dim DateTime As DateTime
                                    Dim DateStr = node("ActualShipDate").InnerText.ToString()
                                    'If DateStr.Contains(" ") Then DateStr = DateStr.Substring(0, DateStr.IndexOf(" "))
                                    Dim dformat As String = CommonMethods.datef
                                    If Not String.IsNullOrEmpty(dformat) Then
                                        DateTime = DateTime.ParseExact(DateStr, dformat, Nothing)
                                    Else
                                        If Double.Parse(CommonMethods.version) >= 11 Then
                                            DateTime = DateTime.ParseExact(DateStr, "MM/dd/yyyy HH:mm:ss", Nothing)
                                        Else
                                            DateTime = DateTime.ParseExact(DateStr, "dd/MM/yyyy HH:mm:ss", Nothing)
                                        End If
                                    End If
                                    SavedFields += ";;;ActualShipDate:::" & DateTime.ToString("MM/dd/yyyy hh:mm:ss tt")
                                End If
                                ReadOnlyFields += "~~~ActualShipDate"


                                If Not node("Status").IsEmpty Then
                                    Dim DTOrderStatus = CommonMethods.getCodeDD(warehouse, "orderstatussetup", "ORDERSTATUS")
                                    Dim DROrderStatus() As DataRow = DTOrderStatus.Select("CODE='" & node("Status").InnerText.ToString() & "'")
                                    If DROrderStatus.Length > 0 Then
                                        SavedFields += ";;;Status:::" & DROrderStatus(0)!DESCRIPTION
                                    End If
                                    ReadOnlyFields += "~~~Status"
                                End If

                                If Not node("Type").IsEmpty Then
                                    Dim DTOrderType = CommonMethods.getCodeDD(warehouse, "codelkup", "ORDERTYPE")
                                    Dim DROrderType() As DataRow = DTOrderType.Select("CODE='" & node("Type").InnerText.ToString() & "'")
                                    If DROrderType.Length > 0 Then
                                        SavedFields += ";;;OrderType:::" & DROrderType(0)!DESCRIPTION
                                    End If
                                    ReadOnlyFields += "~~~OrderType"
                                End If

                                If Not node("StorerKey").IsEmpty Then
                                    SavedFields += ";;;StorerKey:::" & node("StorerKey").InnerText.ToString()
                                    ReadOnlyFields += "~~~StorerKey"
                                End If

                                If Not node("ConsigneeKey").IsEmpty Then
                                    SavedFields += ";;;ConsigneeKey:::" & node("ConsigneeKey").InnerText.ToString()
                                End If

                                If Not node("SUsr1").IsEmpty Then
                                    SavedFields += ";;;SUsr1:::" & node("SUsr1").InnerText.ToString()
                                End If

                                If Not node("SUsr2").IsEmpty Then
                                    SavedFields += ";;;SUsr2:::" & node("SUsr2").InnerText.ToString()
                                End If

                                If Not node("SUsr3").IsEmpty Then
                                    SavedFields += ";;;SUsr3:::" & node("SUsr3").InnerText.ToString()
                                End If

                                If Not node("SUsr4").IsEmpty Then
                                    SavedFields += ";;;SUsr4:::" & node("SUsr4").InnerText.ToString()
                                End If

                                If Not node("SUsr5").IsEmpty Then
                                    SavedFields += ";;;SUsr5:::" & node("SUsr5").InnerText.ToString()
                                End If
                            Next
                        End If
                    End With
                Case "ORDERMANAG", "SYSTEM.ORDERMANAG"
                    With ds.Tables(0).Rows(0)
                        If Not .IsNull("WHSEID") Then
                            SavedFields += "Facility:::" & CommonMethods.getFacilityDBAlias(!WHSEID)
                            ReadOnlyFields += "Facility"
                        End If

                        If Not .IsNull("ExternOrderKey") Then
                            SavedFields += ";;;ExternOrderKey:::" & !ExternOrderKey
                            ReadOnlyFields += "~~~ExternOrderKey"
                        End If

                        If Not .IsNull("OrderManagKey") Then
                            SavedFields += ";;;OrderManagKey:::" & !OrderManagKey
                            ReadOnlyFields += "~~~OrderManagKey"
                        End If

                        If Not .IsNull("OrderDate") Then
                            SavedFields += ";;;OrderDate:::" & !OrderDate
                            ReadOnlyFields += "~~~OrderDate"
                        End If

                        If Not .IsNull("Type") Then
                            Dim DTOrderType = CommonMethods.getCodeDD(!WHSEID, "codelkup", "ORDERTYPE")
                            Dim DROrderType() As DataRow = DTOrderType.Select("CODE='" & !Type & "'")
                            If DROrderType.Length > 0 Then
                                SavedFields += ";;;Type:::" & DROrderType(0)!DESCRIPTION
                            End If
                            ReadOnlyFields += "~~~Type"
                        End If

                        If Not .IsNull("OrderManagStatus") Then
                            SavedFields += ";;;OrderManagStatus:::" & !OrderManagStatus
                            ReadOnlyFields += "~~~OrderManagStatus"
                        End If

                        If Not .IsNull("StorerKey") Then
                            SavedFields += ";;;StorerKey:::" & !StorerKey
                            ReadOnlyFields += "~~~StorerKey"
                        End If

                        If Not .IsNull("ConsigneeKey") Then
                            SavedFields += ";;;ConsigneeKey:::" & !ConsigneeKey
                        End If

                        If Not .IsNull("RequestedShipDate") Then
                            SavedFields += ";;;RequestedShipDate:::" & Format(!RequestedShipDate, "MM/dd/yyyy")
                            ReadOnlyFields += "~~~RequestedShipDate"
                        End If

                        If Not .IsNull("SUsr1") Then
                            SavedFields += ";;;SUsr1:::" & !SUsr1
                        End If

                        If Not .IsNull("SUsr2") Then
                            SavedFields += ";;;SUsr2:::" & !SUsr2
                        End If

                        If Not .IsNull("SUsr3") Then
                            SavedFields += ";;;SUsr3:::" & !SUsr3
                        End If

                        If Not .IsNull("SUsr4") Then
                            SavedFields += ";;;SUsr4:::" & !SUsr4
                        End If

                        If Not .IsNull("SUsr5") Then
                            SavedFields += ";;;SUsr5:::" & !SUsr5
                        End If

                        If Not .IsNull("notes2") Then
                            SavedFields += ";;;WMSOrderKey:::" & !notes2
                        End If
                        ReadOnlyFields += "~~~WMSOrderKey"
                    End With
                Case "Warehouse_OrderTracking"
                    With ds.Tables(0).Rows(0)
                        If Not .IsNull("ExternOrderKey") Then
                            SavedFields += "ExternOrderKey:::" & !ExternOrderKey
                        End If

                        If Not .IsNull("StorerKey") Then
                            SavedFields += ";;;StorerKey:::" & !StorerKey
                        End If

                        If Not .IsNull("Facility") Then
                            SavedFields += ";;;Facility:::" & !Facility
                        End If

                        If Not .IsNull("CustomOrderDate") Then
                            SavedFields += ";;;CustomOrderDate:::" & Format(!CustomOrderDate, "MM/dd/yyyy hh:mm:ss tt")
                        End If

                        If Not .IsNull("CustReqDate") Then
                            SavedFields += ";;;CustReqDate:::" & Format(!CustReqDate, "MM/dd/yyyy hh:mm:ss tt")
                        End If

                        If Not .IsNull("CustActShipDate") Then
                            SavedFields += ";;;CustActShipDate:::" & Format(!CustActShipDate, "MM/dd/yyyy hh:mm:ss tt")
                        End If

                        If Not .IsNull("OrderType") Then
                            SavedFields += ";;;OrderType:::" & !OrderType
                        End If

                        If Not .IsNull("PortalDescription") Then
                            SavedFields += ";;;PortalDescription:::" & !PortalDescription
                        End If

                        If Not .IsNull("Order_Transport_Status") Then
                            SavedFields += ";;;Order_Transport_Status:::" & !Order_Transport_Status
                        End If

                        If Not .IsNull("VASStatus") Then
                            SavedFields += ";;;VASStatus:::" & !VASStatus
                        End If

                        If Not .IsNull("CarrierName") Then
                            SavedFields += ";;;CarrierName:::" & !CarrierName
                        End If

                        If Not .IsNull("C_Company") Then
                            SavedFields += ";;;C_Company:::" & !C_Company
                        End If

                        If Not .IsNull("C_City") Then
                            SavedFields += ";;;C_City:::" & !C_City
                        End If

                        If Not .IsNull("C_State") Then
                            SavedFields += ";;;C_State:::" & !C_State
                        End If

                        If Not .IsNull("C_Zip") Then
                            SavedFields += ";;;C_Zip:::" & !C_Zip
                        End If

                        If Not .IsNull("C_Address1") Then
                            SavedFields += ";;;C_Address1:::" & !C_Address1
                        End If

                        If Not .IsNull("C_Address2") Then
                            SavedFields += ";;;C_Address2:::" & !C_Address2
                        End If

                        If Not .IsNull("C_Address3") Then
                            SavedFields += ";;;C_Address3:::" & !C_Address3
                        End If

                        If Not .IsNull("C_Address4") Then
                            SavedFields += ";;;C_Address4:::" & !C_Address4
                        End If

                        If Not .IsNull("C_Address5") Then
                            SavedFields += ";;;C_Address5:::" & !C_Address5
                        End If

                        If Not .IsNull("BuyerPO") Then
                            SavedFields += ";;;BuyerPO:::" & !BuyerPO
                        End If

                        If Not .IsNull("SUsr1") Then
                            SavedFields += ";;;SUsr1:::" & !SUsr1
                        End If

                        If Not .IsNull("SUsr2") Then
                            SavedFields += ";;;SUsr2:::" & !SUsr2
                        End If

                        If Not .IsNull("SUsr3") Then
                            SavedFields += ";;;SUsr3:::" & !SUsr3
                        End If

                        If Not .IsNull("SUsr4") Then
                            SavedFields += ";;;SUsr4:::" & !SUsr4
                        End If

                        If Not .IsNull("SUsr5") Then
                            SavedFields += ";;;SUsr5:::" & !SUsr5
                        End If

                        If Not .IsNull("VASStartDate") Then
                            SavedFields += ";;;VASStartDate:::" & Format(!VASStartDate, "MM/dd/yyyy hh:mm:ss tt")
                        End If

                        If Not .IsNull("VASEndDate") Then
                            SavedFields += ";;;VASEndDate:::" & Format(!VASEndDate, "MM/dd/yyyy hh:mm:ss tt")
                        End If

                        If Not .IsNull("NbrOfSplit") Then
                            SavedFields += ";;;NbrOfSplit:::" & !NbrOfSplit
                        End If

                        If Not .IsNull("POD") Then
                            SavedFields += ";;;POD:::" & Format(!POD, "MM/dd/yyyy hh:mm:ss tt")
                        End If

                        ReadOnlyFields += "ExternOrderKey~~~StorerKey~~~Facility~~~CustomOrderDate~~~CustReqDate~~~CustActShipDate~~~"
                        ReadOnlyFields += "OrderType~~~PortalDescription~~~Order_Transport_Status~~~VASStatus~~~CarrierName~~~"
                        ReadOnlyFields += "C_Company~~~C_City~~~C_State~~~C_Zip~~~C_Address1~~~C_Address2~~~C_Address3~~~C_Address4~~~"
                        ReadOnlyFields += "C_Address5~~~BuyerPO~~~SUsr1~~~SUsr2~~~SUsr3~~~SUsr4~~~SUsr5~~~"
                        ReadOnlyFields += "VASStartDate~~~VASEndDate~~~NbrOfSplit~~~POD"
                    End With

                    CarrierEventsNbr = ds.Tables(1).Rows.Count * 5

                    For i = 0 To CarrierEventsNbr - 1
                        With ds.Tables(1).Rows(0)
                            CarrierRecords += "<div class='CarrierDiv' data-externalorderkey='" & !ExternalOrderKey & "' data-storerkey='" & !StorerKey & "' data-eventdescription='" & !EventDescription & "' data-consignmentdate = '" & Format(!ConsignmentDate, "MMMM dd, yyyy - hh:mm tt") & "' data-eventdate = '" & Format(!EventDate, "MMMM dd, yyyy - hh:mm tt") & "' data-cube='" & Val(!Cube) & "' data-items='" & !Items & "' data-weight='" & Val(!Weight) & "' data-category='" & !Category & "' data-consignmentid='" & !ConsignmentID & "'>"
                            CarrierRecords += IIf(i = 0, "<div class='CarrierIndicator'></div>", "")
                            CarrierRecords += "   <div class='CarrierContainer'>"
                            CarrierRecords += "      <div class='EventStatusIcon' style='background-image:url(" & clsGlobals.sAppPath & "images/Cufex_Images/EventStatus" & !Category & ".svg)'></div>"
                            CarrierRecords += "      <div class='CarrierConsignment'>" & !ConsignmentID & "</div>"
                            CarrierRecords += "      <div class='CarrierName'>" & !CarrierName & "</div>"
                            CarrierRecords += "   </div>"
                            CarrierRecords += "</div>"
                        End With
                    Next
            End Select
        End If
        'End If

        Dim SavedFiles As String = ""
        If mySearchTable = "Warehouse_PO" Or mySearchTable = "Warehouse_ASN" Or mySearchTable = "Warehouse_SO" Or mySearchTable.Contains("ORDERMANAG") Then
            Dim Warehouse As String = ""
            Dim KeyName As String
            Dim KeyValue As String = ""

            If mySearchTable = "Warehouse_PO" Then
                KeyName = "POKEY"
            ElseIf mySearchTable = "Warehouse_ASN" Then
                KeyName = "RECEIPTKEY"
            ElseIf mySearchTable = "Warehouse_SO" Then
                KeyName = "ORDERKEY"
            Else
                KeyName = "ORDERMANAGKEY"
            End If

            If ds.Tables(0).Rows.Count > 0 Then
                With ds.Tables(0).Rows(0)
                    If Not mySearchTable.Contains("ORDERMANAG") Then
                        If Not .IsNull("Facility") Then
                            Warehouse = CommonMethods.getFacilityDBName(!Facility)
                        End If
                    Else
                        If Not .IsNull("WHSEID") Then
                            Warehouse = !WHSEID
                        End If
                    End If
                    If Warehouse <> "" Then
                        If LCase(Warehouse.Substring(0, 6)) = "infor_" Then Warehouse = Warehouse.Substring(6, Warehouse.Length - 6)
                        If LCase(Warehouse).Contains("_") Then Warehouse = Warehouse.Split("_")(1)
                    End If
                    If Not .IsNull(KeyName) Then KeyValue = ds.Tables(0).Rows(0)(KeyName)
                End With
                Dim dr As DataRow() = ds.Tables(1).Select("WHSEID = '" & Warehouse & "' and " & KeyName & " ='" & KeyValue & "'")
                For i = 0 To dr.Count - 1
                    SavedFiles += IIf(i <> 0, ";;;", "") & dr(i)!FileName & ":::" & dr(i)!FileSize & ":::" & IIf(dr(i)!AddWho = HttpContext.Current.Session("userkey"), "0", "1")
                Next
            End If
        End If

        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)
        Using writer As JsonWriter = New JsonTextWriter(sw)
            writer.Formatting = Newtonsoft.Json.Formatting.Indented
            writer.WriteStartObject()

            writer.WritePropertyName("Error")
            writer.WriteValue(MyError)

            writer.WritePropertyName("SavedFields")
            writer.WriteValue(SavedFields)

            writer.WritePropertyName("ReadOnlyFields")
            writer.WriteValue(ReadOnlyFields)

            writer.WritePropertyName("SavedFiles")
            writer.WriteValue(SavedFiles)

            writer.WritePropertyName("CarrierEventsNbr")
            writer.WriteValue(CarrierEventsNbr)

            writer.WritePropertyName("CarrierRecords")
            writer.WriteValue(CarrierRecords)

            writer.WriteEndObject()
        End Using
        context.Response.Write(sw)
        context.Response.End()

    End Sub
    Private Sub GetPurchaseOrderQuery(ByRef Sql As String, ByVal MyID As Integer)
        Dim wname As String = "", warehouselevel As String = "", AndFilter As String = ""
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

                    Sql += " select SerialKey, '" & wname & "' as Facility, POKey "
                    Sql += " from " & warehouselevel & ".po where 1=1  " & AndFilter
                    Sql += " UNION"
                Next
                If Sql.EndsWith("UNION") Then Sql = Sql.Remove(Sql.Length - 5)
                Sql += ") as ds where 1=1 and SerialKey = " & MyID
            End If
        End If
    End Sub
    Private Sub GetASNQuery(ByRef Sql As String, ByVal MyID As Integer)
        Dim wname As String = "", warehouselevel As String = "", AndFilter As String = ""
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

                    Sql += " select SerialKey, '" & wname & "' as Facility, ReceiptKey "
                    Sql += " from " & warehouselevel & ".Receipt where 1=1  " & AndFilter
                    Sql += " UNION"
                Next
                If Sql.EndsWith("UNION") Then Sql = Sql.Remove(Sql.Length - 5)
                Sql += ") as ds where 1=1 and SerialKey = " & MyID
            End If
        End If
    End Sub
    Private Sub GetSOQuery(ByRef Sql As String, ByVal MyID As Integer)
        Dim wname As String = "", warehouselevel As String = "", AndFilter As String = ""
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

                    Sql += " select SerialKey, '" & wname & "' as Facility, OrderKey "
                    Sql += " from " & warehouselevel & ".Orders where 1=1  " & AndFilter
                    Sql += " UNION"
                Next
                If Sql.EndsWith("UNION") Then Sql = Sql.Remove(Sql.Length - 5)
                Sql += ") as ds where 1=1 and SerialKey = " & MyID
            End If
        End If
    End Sub
    Private Sub GetOrderTrackingQuery(ByRef Sql As String, ByVal MyID As String)
        Dim AndFilter As String = ""
        Dim TimeZone As Integer = 0

        If Integer.TryParse(HttpContext.Current.Session("timezone").ToString, TimeZone) Then
            TimeZone = Integer.Parse(HttpContext.Current.Session("timezone").ToString)
        End If

        Dim MyIDArr As String() = MyID.Split(New String() {"~~~"}, StringSplitOptions.RemoveEmptyEntries)

        If MyIDArr.Length > 0 Then AndFilter += " And OT.ExternOrderKey = '" & MyIDArr(0) & "'"
        If MyIDArr.Length > 1 Then AndFilter += " And OT.StorerKey = '" & MyIDArr(1) & "'"

        '0 Header
        Sql += " SELECT OT.EXTERNORDERKEY, (CASE WHEN COUNT(OT.WHSEID) > 1 then 'MULTI' else (select db_alias from wmsadmin.pl_db where db_logid = MAX(OT.WHSEID)) END) AS FACILITY,  (select DESCRIPTION from enterprise.codelkup where code=OT.Type AND listname = 'ORDERTYPE' ) AS ORDERTYPE,   MIN(OT.STATUS) AS STATUS,   (SELECT PORTALDESCRIPTION from dbo.PORTALORDERSTATUSSETUP WITH (NOLOCK) WHERE CODE =CAST(MIN(OT.STATUS) as INT)) AS PORTALDESCRIPTION, DATEADD(hh , " & TimeZone & ", MIN(OT.ORDERDATE)) AS CustomOrderDate,   DATEADD(hh , " & TimeZone & " , MIN(OT.REQUESTEDSHIPDATE)) AS CustReqDate,   DATEADD (hh , " & TimeZone & " , MIN(OT.ACTUALSHIPDATE)) AS CustActShipDate,   OT.STORERKEY, (CASE WHEN OT2.CCC > 1 then 'MULTI' WHEN OT2.MAXCC IS NULL THEN 'No Carrier Assigned' else OT2.MAXCC END) AS CarrierCode,  (CASE WHEN OT2.CCN > 1 then 'MULTI' WHEN OT2.MAXCN IS NULL THEN 'No Carrier Assigned' else OT2.MAXCN END) AS CarrierName,   OT.BUYERPO,   MAX(OT.CONSIGNEEKEY) CONSIGNEEKEY,   OT.C_COMPANY,   OT.C_CITY,   OT.C_STATE,  OT.C_ZIP,   OT.C_COUNTRY,  OT.C_ADDRESS1,   OT.C_ADDRESS2,   OT.C_ADDRESS3,   OT.C_ADDRESS4,   OT.C_ADDRESS5,   MIN(OT.SUSR1) SUSR1,  MIN(OT.SUSR2) SUSR2,   MIN(OT.SUSR3) SUSR3,   MIN(OT.SUSR4) SUSR4,   MIN(OT.SUSR5) SUSR5,   VS.VASSTARTDATE,  VS.VASENDDATE,  (COUNT(DISTINCT(OT.WHSEID))) AS NBROFSPLIT,  CASE WHEN CET.ORDER_TRANSPORT_STATUS IS NULL THEN 'Not Started' ELSE CET.ORDER_TRANSPORT_STATUS END ORDER_TRANSPORT_STATUS,  (CASE WHEN (VS.VASENDDATE IS NOT NULL AND VS.VASSTARTDATE IS NOT NULL) THEN 'VAS Out' WHEN  VS.VASSTARTDATE IS NOT NULL THEN 'VAS In' ELSE 'Not Started' END) AS VASSTATUS,  MIN(OT.ADDDATE) as ADDDATE, CAT.POD FROM ENTERPRISE.ALL_ORDERS OT  LEFT JOIN dbo.VAS VS WITH (NOLOCK)  ON VS.EXTERNALORDERKEY = OT.EXTERNORDERKEY   AND VS.STORERKEY = OT.STORERKEY  LEFT JOIN (SELECT R3.STORERKEY, R3.EXTERNALORDERKEY, R3.CARRIERNAME, R3.CONSOLIDATIONSEQ, CASE WHEN MIN(CARRIEREVENTSTATUSCODES.EVENTDESCRIPTION) IS NULL THEN 'Not Started' ELSE MIN(CARRIEREVENTSTATUSCODES.EVENTDESCRIPTION) END ORDER_TRANSPORT_STATUS FROM   CARRIEREVENTSTATUSCODES,  ( SELECT R2.STORERKEY, R2.EXTERNALORDERKEY, R2.CARRIERNAME, MIN(R2.CONSOLIDATIONSEQ) CONSOLIDATIONSEQ FROM ( SELECT R1.STORERKEY, R1.EXTERNALORDERKEY, R1.CARRIERNAME, R1.CONSIGNMENTID, CASE WHEN MIN(R1.CONSOLIDATIONSEQ) IS NULL THEN '000' ELSE MIN(R1.CONSOLIDATIONSEQ) END CONSOLIDATIONSEQ FROM ( SELECT CE.STORERKEY, CE.EXTERNALORDERKEY, CE.CARRIERNAME, CE.CONSIGNMENTID, MAX(CESC.CONSOLIDATIONSEQ) CONSOLIDATIONSEQ FROM   CARRIEREVENTS CE LEFT JOIN CARRIEREVENTSTATUSCODES CESC ON  CE.EVENTSTATUSCODE = CESC.EVENTCODE  AND          CESC.CONSOLIDATE = 'Y' GROUP  BY CE.STORERKEY, CE.EXTERNALORDERKEY, CE.CARRIERNAME, CE.CONSIGNMENTID, CE.ARTICLEID ) R1 GROUP BY R1.STORERKEY, R1.EXTERNALORDERKEY, R1.CARRIERNAME, R1.CONSIGNMENTID ) R2 GROUP BY R2.STORERKEY, R2.EXTERNALORDERKEY, R2.CARRIERNAME ) R3 WHERE  CARRIEREVENTSTATUSCODES.CONSOLIDATIONSEQ = R3.CONSOLIDATIONSEQ GROUP  BY R3.STORERKEY, R3.EXTERNALORDERKEY, R3.CARRIERNAME, R3.CONSOLIDATIONSEQ) CET   ON CET.EXTERNALORDERKEY = OT.EXTERNORDERKEY   AND CET.STORERKEY = OT.STORERKEY LEFT JOIN (SELECT EXTERNORDERKEY, STORERKEY, COUNT(DISTINCT(CarrierCode)) CCC , MAX(CarrierCode) MAXCC, MAX(CarrierName) MAXCN, COUNT(DISTINCT(CarrierName)) CCN FROM ENTERPRISE.ALL_ORDERS  WITH (NOLOCK) GROUP BY EXTERNORDERKEY, STORERKEY) OT2  ON OT2.EXTERNORDERKEY = OT.EXTERNORDERKEY  AND OT2.STORERKEY = OT.STORERKEY LEFT JOIN (SELECT EXTERNALORDERKEY, STORERKEY, MIN(POD) AS POD  FROM dbo.CARRIEREVENTS WITH (NOLOCK) GROUP BY EXTERNALORDERKEY, STORERKEY) CAT ON CAT.EXTERNALORDERKEY = OT.EXTERNORDERKEY AND CAT.STORERKEY = OT.STORERKEY WHERE 1=1 " & AndFilter & " GROUP BY OT.EXTERNORDERKEY, OT.TYPE, OT.STORERKEY, OT.BUYERPO,  OT.C_COMPANY, OT.C_CITY, OT.C_STATE, OT.C_ZIP, OT.C_COUNTRY,  OT.C_ADDRESS1, OT.C_ADDRESS2,  OT.C_ADDRESS3, OT.C_ADDRESS4, OT.C_ADDRESS5,  VS.VASSTARTDATE, VS.VASENDDATE, CET.ORDER_TRANSPORT_STATUS, OT2.CCC, OT2.MAXCC,  OT2.MAXCN, OT2.CCN, CAT.POD "
        '1 Carrier Events
        Sql += " SELECT R3.*, CTSC.EVENTDESCRIPTION, CTSC.CATEGORY FROM ( SELECT CT1.EXTERNALORDERKEY, CT1.STORERKEY, CT1.CONSIGNMENTID, CT1.CONSIGNMENTDATE, CT1.CARRIERNAME, MAX(R2.CUBE) CUBE, MAX(R2.WEIGHT) WEIGHT, COUNT(CT1.ARTICLEID) ITEMS, MIN(R2.POD) POD, MAX(CT1.EVENTDATE) EVENTDATE,  CASE WHEN MIN(EVENTSTATUSCODE) IS NULL THEN '000' WHEN MIN(EVENTSTATUSCODE) = '' THEN '000' ELSE MIN(EVENTSTATUSCODE) END EVENTSTATUSCODE FROM (   SELECT R1.EXTERNALORDERKEY, R1.STORERKEY, R1.CARRIERNAME, R1.CONSIGNMENTID, R1.ARTICLEID, MAX(R1.WEIGHT) WEIGHT, MAX(R1.CUBE) CUBE, MIN(R1.POD) POD, MAX(R1.EVENTDATE) EVENTDATE FROM  (    SELECT CE.STORERKEY, CE.EXTERNALORDERKEY, CE.CARRIERNAME, CE.CONSIGNMENTID, MIN(CE.CONSIGNMENTDATE) CONSIGNMENTDATE,  MAX(CE.EVENTDATE) EVENTDATE, MIN(CE.POD) POD, MAX(CESC.CONSOLIDATIONSEQ) CONSOLIDATIONSEQ,  CE.ARTICLEID, MAX(CE.CUBE) CUBE, MAX(CE.WEIGHT) WEIGHT FROM   CARRIEREVENTS CE  LEFT JOIN CARRIEREVENTSTATUSCODES CESC ON  CE.EVENTSTATUSCODE = CESC.EVENTCODE   AND   CESC.CONSOLIDATE = 'Y'   WHERE 1 = 1 " & AndFilter.Replace("OT.", "CE.").Replace("ExternOrderKey", "ExternalOrderKey") & " GROUP  BY CE.STORERKEY, CE.EXTERNALORDERKEY, CE.CARRIERNAME, CE.CONSIGNMENTID, CE.ARTICLEID, CE.EVENTSTATUSCODE, CE.WEIGHT) R1  GROUP BY R1.STORERKEY, R1.EXTERNALORDERKEY, R1.CARRIERNAME, R1.CONSIGNMENTID, R1.ARTICLEID, R1.CONSIGNMENTDATE  ) R2 LEFT JOIN dbo.CARRIEREVENTS CT1 On R2.EXTERNALORDERKEY = CT1.EXTERNALORDERKEY And R2.STORERKEY = CT1.STORERKEY And R2.ARTICLEID = CT1.ARTICLEID And R2.EVENTDATE = CT1.EVENTDATE GROUP BY CT1.EXTERNALORDERKEY, CT1.STORERKEY, CT1.CONSIGNMENTID, CT1.CONSIGNMENTDATE, CT1.CARRIERNAME ) R3   LEFT JOIN dbo.CARRIEREVENTSTATUSCODES CTSC On R3.EVENTSTATUSCODE = CTSC.EVENTCODE And   CTSC.CONSOLIDATE = 'Y' "
    End Sub
    Private Sub GetInventoryBalanceQuery(ByRef Sql As String, ByVal InvBalInfo As String())
        Dim MyID As Integer = Val(InvBalInfo(0))
        Dim AndFilter As String = ""
        Dim MyStatus As String = InvBalInfo(1)
        Dim MyStorerKey As String = InvBalInfo(2)
        Dim MySku As String = InvBalInfo(3)
        Dim MyWarehouse As String = CommonMethods.getFacilityDBName(InvBalInfo(4))
        If LCase(MyWarehouse.Substring(0, 6)) = "infor_" Then MyWarehouse = MyWarehouse.Substring(6, MyWarehouse.Length - 6)
        If LCase(MyWarehouse).Contains("_") Then MyWarehouse = MyWarehouse.Split("_")(1)

        Dim ShowDetails As Boolean = MyStatus <> "" And MyStatus <> "" And MySku <> "" And MyWarehouse <> ""

        Dim wname As String = "", warehouselevel As String = ""
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

                    Sql += " SELECT SerialKey, '" & wname & "' as Facility, StorerKey, Sku, Status "
                    Sql += " FROM " & warehouselevel & ".LOTxLOCxID "
                    Sql += " Where (StorerKey >= '0') "
                    Sql += " AND(StorerKey <= 'ZZZZZZZZZZ') AND(Sku >= '0') AND(Sku <= 'ZZZZZZZZZZ') "
                    Sql += " AND(Lot >= '0') AND(Lot <= 'ZZZZZZZZZZ') AND(Loc >= '0') AND(Loc <= 'ZZZZZZZZZZ') "
                    Sql += " AND(Id >= ' ') AND(Id <= 'ZZZZZZZZZZZZZZZZZZ') " & AndFilter
                    Sql += " UNION"
                Next
                If Sql.EndsWith("UNION") Then Sql = Sql.Remove(Sql.Length - 5)
                Sql += ") as ds where 1=1 and SerialKey = " & MyID
                If ShowDetails Then
                    Sql += " And StorerKey = '" & MyStorerKey & "' and Sku='" & MySku & "' and Status = '" & MyStatus & "'"

                    If LCase(MyStatus) = "ok" Then
                        Sql += " SELECT L.LOT, (L.qty-L.qtyallocated-L.qtypicked-L.qtyonhold- L.QTYPREALLOCATED) as QTY, LA.Lottable01, LA.Lottable02, LA.Lottable03, LA.Lottable04,  LA.Lottable05, LA.Lottable06, LA.Lottable07, LA.Lottable08, LA.Lottable09, LA.Lottable10, LA.Lottable11, LA.Lottable12 , S.Lottable01LABEL, S.Lottable02LABEL, S.Lottable03LABEL, S.Lottable04LABEL, S.Lottable05LABEL, S.Lottable06LABEL, S.Lottable07LABEL, S.Lottable08LABEL, S.Lottable09LABEL, S.Lottable10LABEL, S.Lottable11LABEL, S.Lottable12LABEL FROM " & MyWarehouse & ".LOT L," & MyWarehouse & ".LotAttribute LA,  " & MyWarehouse & ".SKU S WHERE  L.STORERKEY = '" & MyStorerKey & "'   AND L.SKU = '" & MySku & "' and L.LOT = LA.LOT AND L.SKU = LA.SKU AND L.STORERKEY = LA.STORERKEY AND S.SKU = L.SKU AND S.STORERKEY = L.STORERKEY AND (L.qty-L.qtyallocated-L.qtypicked-L.qtyonhold- L.QTYPREALLOCATED)>0 "
                    Else
                        Sql += " SELECT L.LOT, L.QTYONHOLD as QTY, LA.Lottable01, LA.Lottable02, LA.Lottable03, LA.Lottable04,  LA.Lottable05, LA.Lottable06, LA.Lottable07, LA.Lottable08, LA.Lottable09, LA.Lottable10, LA.Lottable11, LA.Lottable12 , S.Lottable01LABEL, S.Lottable02LABEL, S.Lottable03LABEL, S.Lottable04LABEL,   S.Lottable05LABEL, S.Lottable06LABEL, S.Lottable07LABEL, S.Lottable08LABEL, S.Lottable09LABEL, S.Lottable10LABEL, S.Lottable11LABEL, S.Lottable12LABEL FROM " & MyWarehouse & ".LOT L," & MyWarehouse & ".LotAttribute LA,  " & MyWarehouse & ".SKU S WHERE  L.STORERKEY = '" & MyStorerKey & "'   AND L.SKU = '" & MySku & "'   and L.LOT = LA.LOT AND L.SKU = LA.SKU AND L.STORERKEY = LA.STORERKEY AND S.SKU = L.SKU AND S.STORERKEY = L.STORERKEY AND L.QTYONHOLD>0"
                    End If
                End If
            End If
        End If
    End Sub
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class