
Imports System.IO
Imports System.Xml
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class DisplayItemsNew
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub DisplayItem(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim tb As SQLExec = New SQLExec
        Dim mySearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim MyID As String = HttpContext.Current.Request.Item("MyID")
        REM ghina karame - 11/05/2020- get the system flag useRest from webconfig to know if we should replace soapapi with restapi -begin
        Dim useRest As String = ConfigurationManager.AppSettings("UseRestAPI")
        Dim version As String = ConfigurationManager.AppSettings("version")
        REM ghina karame - 11/05/2020- get the system flag useRest from webconfig to know if we should replace soapapi with restapi -end
        Dim MyError As String = ""
        Dim SavedFields As String = ""
        Dim ReadOnlyFields As String = ""
        Dim AndFilter As String = ""
        Dim Sql As String = ""

        If MyID.Contains("?") Then
            MyID = CommonMethods.GetMyID(mySearchTable, MyID)
        End If

        Dim primKey As String = "SerialKey"
        If mySearchTable.Contains("enterprise.storer") Then
            AndFilter = " and Type = " & mySearchTable(mySearchTable.Length - 1)
            mySearchTable = mySearchTable.Remove(mySearchTable.Length - 1)
        ElseIf mySearchTable = "enterprise.sku" Then
            primKey = "SerialKey"
        ElseIf mySearchTable = "SKUCATALOGUE" Then
            If CommonMethods.dbtype <> "sql" Then mySearchTable = "System." & mySearchTable
        ElseIf mySearchTable = "Warehouse_PO" Then
            GetPurchaseOrderQuery(Sql, MyID)
        ElseIf mySearchTable = "Warehouse_ASN" Then
            GetASNQuery(Sql, MyID)
        ElseIf mySearchTable = "Warehouse_SO" Then
            GetSOQuery(Sql, MyID)
        ElseIf mySearchTable = "Warehouse_OrderManagement" Then
            primKey = "SerialKey"
            mySearchTable = "ORDERMANAG"
            If CommonMethods.dbtype <> "sql" Then mySearchTable = "System." & mySearchTable
        End If

        If mySearchTable <> "Inventory_Balance" And mySearchTable <> "Warehouse_PO" And mySearchTable <> "Warehouse_ASN" And mySearchTable <> "Warehouse_SO" Then
            Sql += " Select * from " & mySearchTable & " where " & primKey & " = " & MyID & AndFilter
        End If

        Dim ds As DataSet = tb.Cursor(Sql)

        If ds.Tables(0).Rows.Count = 0 Then MyError = "This record does not exist"

        If Val(MyID) > 0 Then
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

                            If Not .IsNull("TariffKey") Then
                                SavedFields += ";;;TariffKey:::" & !TariffKey
                            End If

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
                            'ghina karame - 01/06/2020- restapicalls- if flag is on and version > 11 then use rest calls instead of soap calls -begin
                            Dim Facility As String = ""
                            Dim POKey As String = ""
                            If Not .IsNull("Facility") Then Facility = !Facility
                            If Not .IsNull("POKey") Then POKey = !POKey
                            If useRest = "1" And version >= "11" Then
                                Dim Command As String
                                Command = "/" & CommonMethods.getFacilityDBName(Facility) & "/purchaseorders/" & POKey & ""
                                Dim jsonData = CommonMethods.GetRest(Command)

                                Dim poJObject = JObject.Parse(jsonData)


                                ' if no errors
                                SavedFields += "Facility:::" & Facility
                                ReadOnlyFields += "Facility"

                                SavedFields += ";;;POKey:::" & POKey
                                ReadOnlyFields += "~~~POKey"

                                Dim externpokey As String = poJObject.SelectToken("externpokey").ToString

                                Dim podate As String = poJObject.SelectToken("podate")
                                Dim status As String = poJObject.SelectToken("status")
                                Dim storerkey As String = poJObject.SelectToken("storerkey")
                                Dim buyername As String = poJObject.SelectToken("buyername")
                                Dim buyersreference As String = poJObject.SelectToken("buyersreference")
                                Dim sellername As String = poJObject.SelectToken("sellername")
                                Dim sellerssreference As String = poJObject.SelectToken("sellerssreference")
                                Dim type As String = poJObject.SelectToken("type")
                                Dim effectivedate As String = poJObject.SelectToken("effectivedate")
                                Dim susr1 As String = poJObject.SelectToken("susr1")
                                Dim susr2 As String = poJObject.SelectToken("susr2")
                                Dim susr3 As String = poJObject.SelectToken("susr3")
                                Dim susr4 As String = poJObject.SelectToken("susr4")
                                Dim susr5 As String = poJObject.SelectToken("susr5")




                                If Not String.IsNullOrEmpty(externpokey) Then
                                    SavedFields += ";;;ExternPOKey:::" & poJObject.SelectToken("externpokey").ToString
                                    ReadOnlyFields += "~~~ExternPOKey"
                                End If

                                If Not String.IsNullOrEmpty(podate) Then
                                    Dim DateTime As DateTime
                                    Dim dformat As String = CommonMethods.datef
                                    If Not String.IsNullOrEmpty(dformat) Then
                                        DateTime = DateTime.ParseExact(podate, dformat, Nothing)
                                    Else
                                        If Double.Parse(CommonMethods.version) >= 11 Then
                                            DateTime = DateTime.ParseExact(podate, "MM/dd/yyyy HH:mm:ss", Nothing)
                                        Else
                                            DateTime = DateTime.ParseExact(podate, "dd/MM/yyyy HH:mm:ss", Nothing)
                                        End If
                                    End If
                                    SavedFields += ";;;PODate:::" & DateTime.ToString("MM/dd/yyyy hh:mm:ss tt")
                                    ReadOnlyFields += "~~~PODate"
                                End If


                                If Not String.IsNullOrEmpty(status) Then
                                    Dim code As Integer = Integer.Parse(status)
                                    SavedFields += ";;;Status:::" & CommonMethods.getStatusPo(code)
                                    ReadOnlyFields += "~~~Status"
                                End If

                                If Not String.IsNullOrEmpty(storerkey) Then
                                    SavedFields += ";;;StorerKey:::" & storerkey
                                    ReadOnlyFields += "~~~StorerKey"
                                End If

                                If Not String.IsNullOrEmpty(buyername) Then
                                    SavedFields += ";;;BuyerName:::" & buyername
                                End If

                                If Not String.IsNullOrEmpty(buyersreference) Then
                                    SavedFields += ";;;BuyersReference:::" & buyersreference
                                End If

                                If Not String.IsNullOrEmpty(sellername) Then
                                    SavedFields += ";;;SellerName:::" & sellername
                                End If

                                If Not String.IsNullOrEmpty(sellerssreference) Then
                                    SavedFields += ";;;SellersReference:::" & sellerssreference

                                End If

                                If Not String.IsNullOrEmpty(type) Then
                                    Dim code As Integer = Integer.Parse(type)
                                    SavedFields += ";;;POType:::" & CommonMethods.getTypePo(code)
                                    ReadOnlyFields += "~~~POType"
                                End If

                                If Not String.IsNullOrEmpty(effectivedate) Then
                                    Dim DateTime As DateTime
                                    Dim dformat As String = CommonMethods.datef
                                    If Not String.IsNullOrEmpty(dformat) Then
                                        DateTime = DateTime.ParseExact(effectivedate, dformat, Nothing)
                                    Else
                                        If Double.Parse(CommonMethods.version) >= 11 Then
                                            DateTime = DateTime.ParseExact(effectivedate, "MM/dd/yyyy HH:mm:ss", Nothing)
                                        Else
                                            DateTime = DateTime.ParseExact(effectivedate, "dd/MM/yyyy HH:mm:ss", Nothing)
                                        End If
                                    End If
                                    SavedFields += ";;;EffectiveDate:::" & DateTime.ToString("MM/dd/yyyy")
                                End If

                                If Not String.IsNullOrEmpty(susr1) Then
                                    SavedFields += ";;;SUsr1:::" & susr1
                                End If

                                If Not String.IsNullOrEmpty(susr2) Then
                                    SavedFields += ";;;SUsr2:::" & susr2
                                End If

                                If Not String.IsNullOrEmpty(susr3) Then
                                    SavedFields += ";;;SUsr3:::" & susr3
                                End If

                                If Not String.IsNullOrEmpty(susr4) Then
                                    SavedFields += ";;;SUsr4:::" & susr4
                                End If

                                If Not String.IsNullOrEmpty(susr5) Then
                                    SavedFields += ";;;SUsr5:::" & susr5
                                End If

                            Else

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
                            'ghina karame - 08/06/2020- restapicalls- if flag is on and version > 11 then use rest calls instead of soap calls -begin
                            If useRest = "1" And version >= "11" Then
                                Dim Command As String
                                Command = "/" & CommonMethods.getFacilityDBName(Facility) & "/receipts/" & ReceiptKey & ""
                                Dim jsonData = CommonMethods.GetRest(Command)

                                SavedFields += "Facility:::" & Facility
                                ReadOnlyFields += "Facility"

                                SavedFields += ";;;ReceiptKey:::" & ReceiptKey
                                ReadOnlyFields += "~~~ReceiptKey"

                                Dim asnJObject = JObject.Parse(jsonData)
                                Dim externreceiptkey As String = asnJObject.SelectToken("externreceiptkey")
                                Dim receiptdate As String = asnJObject.SelectToken("receiptdate")
                                Dim status As String = asnJObject.SelectToken("status")
                                Dim type As String = asnJObject.SelectToken("type")
                                Dim storerkey As String = asnJObject.SelectToken("storerkey")
                                Dim pokey As String = asnJObject.SelectToken("pokey")
                                Dim carrierkey As String = asnJObject.SelectToken("carrierkey")
                                Dim warehousereference As String = asnJObject.SelectToken("warehousereference")
                                Dim containerkey As String = asnJObject.SelectToken("containerkey")
                                Dim containertype As String = asnJObject.SelectToken("containertype")
                                Dim origincountry As String = asnJObject.SelectToken("origincountry")
                                Dim transportationmode As String = asnJObject.SelectToken("transportationmode")


                                If Not String.IsNullOrEmpty(externreceiptkey) Then
                                    SavedFields += ";;;ExternReceiptKey:::" & externreceiptkey
                                    ReadOnlyFields += "~~~ExternReceiptKey"
                                End If

                                If Not String.IsNullOrEmpty(receiptdate) Then
                                    Dim DateTime As DateTime
                                    Dim DateStr = receiptdate
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

                                If Not String.IsNullOrEmpty(status) Then
                                    Dim DTReceiptStatus = CommonMethods.getCodeDD(warehouse, "codelkup", "RECSTATUS")
                                    Dim DRReceiptStatus() As DataRow = DTReceiptStatus.Select("CODE='" & status & "'")
                                    If DRReceiptStatus.Length > 0 Then
                                        SavedFields += ";;;Status:::" & DRReceiptStatus(0)!DESCRIPTION
                                    End If
                                    ReadOnlyFields += "~~~Status"
                                End If

                                If Not String.IsNullOrEmpty(type) Then
                                    Dim DTReceiptType = CommonMethods.getCodeDD(warehouse, "codelkup", "RECEIPTYPE")
                                    Dim DRReceiptType() As DataRow = DTReceiptType.Select("CODE='" & type & "'")
                                    If DRReceiptType.Length > 0 Then
                                        SavedFields += ";;;ReceiptType:::" & DRReceiptType(0)!DESCRIPTION
                                    End If
                                    ReadOnlyFields += "~~~ReceiptType"
                                End If

                                If Not String.IsNullOrEmpty(storerkey) Then
                                    SavedFields += ";;;StorerKey:::" & storerkey
                                    ReadOnlyFields += "~~~StorerKey"
                                End If

                                If Not String.IsNullOrEmpty(pokey) Then
                                    SavedFields += ";;;POKey:::" & pokey
                                End If

                                If Not String.IsNullOrEmpty(carrierkey) Then
                                    SavedFields += ";;;CarrierKey:::" & carrierkey
                                End If

                                If Not String.IsNullOrEmpty(warehousereference) Then
                                    SavedFields += ";;;WarehouseReference:::" & warehousereference
                                End If

                                If Not String.IsNullOrEmpty(containerkey) Then
                                    SavedFields += ";;;ContainerKey:::" & containerkey
                                End If

                                If Not String.IsNullOrEmpty(containertype) Then
                                    SavedFields += ";;;ContainerType:::" & containertype
                                End If

                                If Not String.IsNullOrEmpty(origincountry) Then
                                    SavedFields += ";;;OriginCountry:::" & CommonMethods.getISOCountryName(origincountry)
                                End If

                                If Not String.IsNullOrEmpty(transportationmode) Then
                                    SavedFields += ";;;TransportationMode:::" & transportationmode
                                End If

                                ''''''''''''''''''''''''''''''''''''''soapcall''''''''''''''''''''''''''''''''''''''''''''''''''''''''end ghina

                            Else

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
                            End If 'GHINA
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

                            'ghina karame - 08/06/2020- restapicalls- if flag is on and version > 11 then use rest calls instead of soap calls -begin
                            If useRest = "1" And version >= "11" Then
                                Dim Command As String
                                Command = "/" & CommonMethods.getFacilityDBName(Facility) & "/shipments/" & OrderKey & ""
                                Dim jsonData = CommonMethods.GetRest(Command)

                                SavedFields += "Facility:::" & Facility
                                ReadOnlyFields += "Facility"

                                SavedFields += ";;;OrderKey:::" & OrderKey
                                ReadOnlyFields += "~~~OrderKey"

                                Dim soJObject = JObject.Parse(jsonData)
                                Dim externorderkey As String = soJObject.SelectToken("externorderkey")
                                Dim orderdate As String = soJObject.SelectToken("orderdate")
                                Dim requestedshipdate As String = soJObject.SelectToken("requestedshipdate")
                                Dim actualshipdate As String = soJObject.SelectToken("actualshipdate")
                                Dim storerkey As String = soJObject.SelectToken("storerkey")
                                Dim status As String = soJObject.SelectToken("status")
                                Dim type As String = soJObject.SelectToken("type")
                                Dim consigneekey As String = soJObject.SelectToken("consigneekey")
                                Dim susr1 As String = soJObject.SelectToken("susr1")
                                Dim susr2 As String = soJObject.SelectToken("susr2")
                                Dim susr3 As String = soJObject.SelectToken("susr3")
                                Dim susr4 As String = soJObject.SelectToken("susr4")
                                Dim susr5 As String = soJObject.SelectToken("susr5")


                                If Not String.IsNullOrEmpty(externorderkey) Then
                                    SavedFields += ";;;ExternOrderKey:::" & externorderkey
                                    ReadOnlyFields += "~~~ExternOrderKey"
                                End If

                                If Not String.IsNullOrEmpty(orderdate) Then
                                    Dim DateTime As DateTime
                                    Dim DateStr = orderdate
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

                                If Not String.IsNullOrEmpty(requestedshipdate) Then
                                    Dim DateTime As DateTime
                                    Dim DateStr = requestedshipdate
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

                                If Not String.IsNullOrEmpty(actualshipdate) Then
                                    Dim DateTime As DateTime
                                    Dim DateStr = actualshipdate
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


                                If Not String.IsNullOrEmpty(status) Then
                                    Dim DTOrderStatus = CommonMethods.getCodeDD(warehouse, "orderstatussetup", "ORDERSTATUS")
                                    Dim DROrderStatus() As DataRow = DTOrderStatus.Select("CODE='" & status & "'")
                                    If DROrderStatus.Length > 0 Then
                                        SavedFields += ";;;Status:::" & DROrderStatus(0)!DESCRIPTION
                                    End If
                                    ReadOnlyFields += "~~~Status"
                                End If

                                If Not String.IsNullOrEmpty(type) Then
                                    Dim DTOrderType = CommonMethods.getCodeDD(warehouse, "codelkup", "ORDERTYPE")
                                    Dim DROrderType() As DataRow = DTOrderType.Select("CODE='" & type & "'")
                                    If DROrderType.Length > 0 Then
                                        SavedFields += ";;;OrderType:::" & DROrderType(0)!DESCRIPTION
                                    End If
                                    ReadOnlyFields += "~~~OrderType"
                                End If

                                If Not String.IsNullOrEmpty(storerkey) Then
                                    SavedFields += ";;;StorerKey:::" & storerkey
                                    ReadOnlyFields += "~~~StorerKey"
                                End If

                                If Not String.IsNullOrEmpty(consigneekey) Then
                                    SavedFields += ";;;ConsigneeKey:::" & consigneekey
                                End If

                                If Not String.IsNullOrEmpty(susr1) Then
                                    SavedFields += ";;;SUsr1:::" & susr1
                                End If

                                If Not String.IsNullOrEmpty(susr2) Then
                                    SavedFields += ";;;SUsr2:::" & susr2
                                End If

                                If Not String.IsNullOrEmpty(susr3) Then
                                    SavedFields += ";;;SUsr3:::" & susr3
                                End If

                                If Not String.IsNullOrEmpty(susr4) Then
                                    SavedFields += ";;;SUsr4:::" & susr4
                                End If

                                If Not String.IsNullOrEmpty(susr5) Then
                                    SavedFields += ";;;SUsr5:::" & susr5
                                End If

                                '''''''''''''''''''''''''''soap call''''''''''''''''''''''''''''''''''''''''''''''''''' end ghina
                            Else

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
                                        End If 'ghina
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
                End Select
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

    'Private Function GetMyID(ByVal SearchTable As String, ByVal StrID As String) As Integer
    '    Dim MyID As Integer = 0
    '    Dim AndFilter As String = ""
    '    Dim sql As String = ""
    '    Dim tb As SQLExec = New SQLExec
    '    Dim ds As DataSet = Nothing
    '    Select Case SearchTable
    '        Case "PORTALUSERS", "SKUCATALOGUE"
    '            MyID = StrID.Split("=")(1)
    '        Case "USERCONTROL"
    '            sql += "Select " & IIf(CommonMethods.dbtype = "sql", "ID", "SerialKey") & " From " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & SearchTable & " where UserKey = '" & StrID.Split("=")(1) & "'"
    '        Case "enterprise.storer2", "enterprise.storer5"
    '            sql += "Select SerialKey from " & SearchTable.Remove(SearchTable.Length - 1) & " where StorerKey = '" & StrID.Split("=")(1) & "' and Type = " & SearchTable(SearchTable.Length - 1)
    '        Case "enterprise.sku"
    '            sql += "Select SerialKey from " & SearchTable & " where StorerKey = '" & StrID.Split("=")(1).Split("&")(0) & "' and Sku = '" & StrID.Split("=")(2) & "'"
    '        Case "Warehouse_PO", "Warehouse_ASN", "Warehouse_SO", "Warehouse_OrderManagement"
    '            Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
    '            Dim suppliers As String() = CommonMethods.getSupplierPerUser(HttpContext.Current.Session("userkey").ToString)
    '            Dim consignees As String() = CommonMethods.getConsigneePerUser(HttpContext.Current.Session("userkey").ToString)
    '            If owners IsNot Nothing And suppliers IsNot Nothing And consignees IsNot Nothing Then
    '                Dim ownersstr As String = String.Join("','", owners)
    '                ownersstr = "'" & ownersstr & "'"
    '                If Not UCase(ownersstr).Contains("'ALL'") Then AndFilter += " and STORERKEY IN (" & ownersstr & ")"

    '                If SearchTable = "Warehouse_PO" Then
    '                    Dim suppliersstr As String = String.Join("','", suppliers)
    '                    suppliersstr = "'" & suppliersstr & "'"
    '                    If Not UCase(suppliersstr).Contains("'ALL'") Then AndFilter += " and SellerName IN (" & suppliersstr & ")"
    '                End If

    '                If SearchTable = "Warehouse_SO" Or SearchTable = "Warehouse_OrderManagement" Then
    '                    Dim consigneesstr As String = String.Join("','", consignees)
    '                    consigneesstr = "'" & consigneesstr & "'"
    '                    If Not UCase(consigneesstr).Contains("'ALL'") Then AndFilter += " and ConsigneeKey IN (" & consigneesstr & ")"
    '                End If

    '                Dim warehouse As String = StrID.Split("=")(1).Split("&")(0)
    '                Dim warehouselevel As String = warehouse
    '                If SearchTable <> "Warehouse_OrderManagement" Then
    '                    If LCase(warehouse.Substring(0, 6)) = "infor_" Then
    '                        warehouselevel = warehouse.Substring(6, warehouse.Length - 6)
    '                    End If
    '                    warehouselevel = warehouselevel.Split("_")(1)
    '                End If

    '                sql += "Select SerialKey from " & warehouselevel
    '                If SearchTable = "Warehouse_PO" Then
    '                    sql += ".PO where POKey = "
    '                ElseIf SearchTable = "Warehouse_ASN" Then
    '                    sql += ".Receipt where ReceiptKey = "
    '                ElseIf SearchTable = "Warehouse_SO" Then
    '                    sql += ".Orders where OrderKey = "
    '                ElseIf SearchTable = "Warehouse_OrderManagement" Then
    '                    sql = "Select SerialKey from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAG where WHSEID = '" & warehouselevel & "' and ExternOrderKey = "
    '                End If
    '                sql += "'" & StrID.Split("=")(2) & "'" & AndFilter
    '            End If
    '    End Select

    '    If sql <> "" Then
    '        ds = tb.Cursor(sql)
    '        If ds.Tables(0).Rows.Count > 0 Then MyID = ds.Tables(0).Rows(0)(0)
    '    End If

    '    Return Val(MyID)
    'End Function
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class