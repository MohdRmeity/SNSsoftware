
Imports System.IO
Imports System.Xml
Imports Newtonsoft.Json

Public Class DisplayItems
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub DisplayItem(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim tb As SQLExec = New SQLExec
        Dim mySearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim MyID As String = HttpContext.Current.Request.Item("MyID")
        Dim MyError As String = ""
        Dim SavedFields As String = ""
        Dim ReadOnlyFields As String = ""
        Dim SavedDetailsFields As String = ""
        Dim ReadOnlyDetailsFields As String = ""
        Dim InputDetailsFields As String = ""
        Dim AndFilter As String = ""
        Dim Sql As String = ""
        Dim DetailsCount As Integer = 0


        If MyID.Contains("?") Then
            MyID = GetMyID(mySearchTable, MyID)
        End If

        Dim primKey As String = "ID"
        If mySearchTable = "PORTALUSERS" Then
            If CommonMethods.dbtype <> "sql" Then mySearchTable = "System." & mySearchTable
        ElseIf mySearchTable = "USERCONTROL" Then
            If CommonMethods.dbtype <> "sql" Then
                mySearchTable = "System." & mySearchTable
                primKey = "SerialKey"
            End If
        ElseIf mySearchTable.Contains("enterprise.storer") Then
            primKey = "SerialKey"
            Dim type As String = mySearchTable(mySearchTable.Length - 1)
            AndFilter = " and Type=" & type
            mySearchTable = mySearchTable.Remove(mySearchTable.Length - 1)
        ElseIf mySearchTable = "enterprise.sku" Then
            primKey = "SerialKey"
        ElseIf mySearchTable = "SKUCATALOGUE" Then
            primKey = "SerialKey"
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
        ElseIf mySearchTable = "Inventory_Balance" Then
            Dim InvBalInfo As String() = HttpContext.Current.Request.Item("MyID").ToString.Split(New String() {"~~~"}, StringSplitOptions.RemoveEmptyEntries)
            GetInventoryBalanceQuery(Sql, InvBalInfo)
        End If

        If mySearchTable <> "Inventory_Balance" And mySearchTable <> "Warehouse_PO" And mySearchTable <> "Warehouse_ASN" And mySearchTable <> "Warehouse_SO" Then
            Sql += " Select * from " & mySearchTable & " where " & primKey & " = " & MyID & AndFilter
        End If

        If mySearchTable.Contains("USERCONTROL") Then
            Sql += " Select UserKey, (select DB_ALIAS from wmsadmin.pl_db where isActive='1' and db_enterprise='0' and db_name = USERCONTROLFACILITY.FACILITY) as Facility "
            Sql += " from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "USERCONTROLFACILITY"
        ElseIf mySearchTable.Contains("ORDERMANAG") Then
            Sql += " Select * from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAGDETAIL"
        End If


        Dim ds As DataSet = tb.Cursor(Sql)

        If ds.Tables(0).Rows.Count = 0 Then MyError = "This record does not exist"

        If Val(MyID) > 0 Then
            If ds.Tables(0).Rows.Count > 0 Then
                Select Case mySearchTable
                    Case "PORTALUSERS", "SYSTEM.PORTALUSERS"
                        With ds.Tables(0).Rows(0)
                            If Not .IsNull("UserKey") Then
                                SavedFields += "UserKey:::" & !UserKey
                                ReadOnlyFields += "UserKey"
                            End If

                            If Not .IsNull("FirstName") Then
                                SavedFields += ";;;FirstName:::" & !FirstName
                                ReadOnlyFields += "~~~FirstName"
                            End If

                            If Not .IsNull("LastName") Then
                                SavedFields += ";;;LastName:::" & !LastName
                                ReadOnlyFields += "~~~LastName"
                            End If

                            If Not .IsNull("Email") Then
                                SavedFields += ";;;Email:::" & !Email
                            End If

                            If Not .IsNull("Active") Then
                                SavedFields += ";;;Active:::" & !Active
                            End If

                            If Not .IsNull("Password") Then
                                SavedFields += ";;;Password:::" & !Password
                                SavedFields += ";;;ConfirmPassword:::" & !Password
                            End If
                        End With
                    Case "USERCONTROL", "SYSTEM.USERCONTROL"
                        With ds.Tables(0).Rows(0)
                            If Not .IsNull("UserKey") Then
                                SavedFields += "UserKey:::" & !UserKey
                                ReadOnlyFields += "UserKey"

                                Dim MyRecords As String = ""
                                Dim dr As DataRow() = ds.Tables(1).Select("UserKey = '" & !UserKey & "'")
                                For j = 0 To dr.Count - 1
                                    If j = 0 Then
                                        MyRecords += "" & dr(j)!Facility
                                    Else
                                        MyRecords += "," & dr(j)!Facility
                                    End If
                                Next
                                SavedFields += ";;;Facility:::" & MyRecords
                            End If

                            If Not .IsNull("StorerKey") Then
                                SavedFields += ";;;StorerKey:::" & !StorerKey
                                ReadOnlyFields += "~~~FirstName"
                            End If

                            If Not .IsNull("ConsigneeKey") Then
                                SavedFields += ";;;ConsigneeKey:::" & !ConsigneeKey
                            End If

                            If Not .IsNull("SupplierKey") Then
                                SavedFields += ";;;SupplierKey:::" & !SupplierKey
                            End If
                        End With
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
                    Case "Inventory_Balance"
                        With ds.Tables(0).Rows(0)
                            If Not .IsNull("Facility") Then
                                SavedFields += "Facility:::" & !Facility
                                ReadOnlyFields += "Facility"
                            End If

                            If Not .IsNull("StorerKey") Then
                                SavedFields += ";;;StorerKey:::" & !StorerKey
                                ReadOnlyFields += "~~~StorerKey"
                            End If

                            If Not .IsNull("Sku") Then
                                SavedFields += ";;;Sku:::" & !Sku
                                ReadOnlyFields += "~~~Sku"
                            End If

                            If ds.Tables.Count > 1 Then
                                Dim TimeZone As Integer = 0
                                If Integer.TryParse(HttpContext.Current.Session("timezone").ToString, TimeZone) Then
                                    TimeZone = Integer.Parse(HttpContext.Current.Session("timezone").ToString)
                                End If

                                SavedDetailsFields += "Lot:::"
                                InputDetailsFields += "Lot:::"
                                ReadOnlyDetailsFields += "Lot"
                                For i = 0 To ds.Tables(1).Rows.Count - 1
                                    With ds.Tables(1).Rows(i)
                                        If Not .IsNull("Lot") Then
                                            SavedDetailsFields += IIf(i > 0, "~~~", "") & !Lot
                                        End If
                                        InputDetailsFields += IIf(i > 0, "~~~", "") & "Lot"
                                    End With
                                Next

                                SavedDetailsFields += ";;;Qty:::"
                                InputDetailsFields += ";;;Qty:::"
                                ReadOnlyDetailsFields += "~~~Qty"
                                For i = 0 To ds.Tables(1).Rows.Count - 1
                                    With ds.Tables(1).Rows(i)
                                        If Not .IsNull("Qty") Then
                                            SavedDetailsFields += IIf(i > 0, "~~~", "") & !Qty
                                        End If
                                        InputDetailsFields += IIf(i > 0, "~~~", "") & "Qty"
                                    End With
                                Next

                                SavedDetailsFields += ";;;Lottable01:::"
                                InputDetailsFields += ";;;Lottable01:::"
                                ReadOnlyDetailsFields += "~~~Lottable01"
                                For i = 0 To ds.Tables(1).Rows.Count - 1
                                    With ds.Tables(1).Rows(i)
                                        If Not .IsNull("Lottable01") Then
                                            SavedDetailsFields += IIf(i > 0, "~~~", "") & !Lottable01
                                        End If
                                        If Not .IsNull("Lottable01LABEL") Then
                                            InputDetailsFields += IIf(i > 0, "~~~", "") & IIf(!Lottable01LABEL.ToString = "", "Lottable01", !Lottable01LABEL)
                                        End If
                                    End With
                                Next

                                SavedDetailsFields += ";;;Lottable02:::"
                                InputDetailsFields += ";;;Lottable02:::"
                                ReadOnlyDetailsFields += "~~~Lottable02"
                                For i = 0 To ds.Tables(1).Rows.Count - 1
                                    With ds.Tables(1).Rows(i)
                                        If Not .IsNull("Lottable02") Then
                                            SavedDetailsFields += IIf(i > 0, "~~~", "") & !Lottable02
                                        End If
                                        If Not .IsNull("Lottable01LABEL") Then
                                            InputDetailsFields += IIf(i > 0, "~~~", "") & IIf(!Lottable02LABEL.ToString = "", "Lottable02", !Lottable02LABEL)
                                        End If
                                    End With
                                Next

                                SavedDetailsFields += ";;;Lottable03:::"
                                InputDetailsFields += ";;;Lottable03:::"
                                ReadOnlyDetailsFields += "~~~Lottable03"
                                For i = 0 To ds.Tables(1).Rows.Count - 1
                                    With ds.Tables(1).Rows(i)
                                        If Not .IsNull("Lottable03") Then
                                            SavedDetailsFields += IIf(i > 0, "~~~", "") & !Lottable03
                                        End If
                                        If Not .IsNull("Lottable03LABEL") Then
                                            InputDetailsFields += IIf(i > 0, "~~~", "") & IIf(!Lottable03LABEL.ToString = "", "Lottable03", !Lottable03LABEL)
                                        End If
                                    End With
                                Next

                                SavedDetailsFields += ";;;Lottable04:::"
                                InputDetailsFields += ";;;Lottable04:::"
                                ReadOnlyDetailsFields += "~~~Lottable04"
                                For i = 0 To ds.Tables(1).Rows.Count - 1
                                    With ds.Tables(1).Rows(i)
                                        If Not .IsNull("Lottable04") Then
                                            If i > 0 Then SavedDetailsFields += "~~~"
                                            SavedDetailsFields += CommonMethods.ConvertLottableToDate(!Lottable04.ToString, TimeZone)
                                        End If
                                        If Not .IsNull("Lottable04LABEL") Then
                                            InputDetailsFields += IIf(i > 0, "~~~", "") & IIf(!Lottable04LABEL.ToString = "", "Lottable04", !Lottable04LABEL)
                                        End If
                                    End With
                                Next

                                SavedDetailsFields += ";;;Lottable05:::"
                                InputDetailsFields += ";;;Lottable05:::"
                                ReadOnlyDetailsFields += "~~~Lottable05"
                                For i = 0 To ds.Tables(1).Rows.Count - 1
                                    With ds.Tables(1).Rows(i)
                                        If Not .IsNull("Lottable05") Then
                                            If i > 0 Then SavedDetailsFields += "~~~"
                                            SavedDetailsFields += CommonMethods.ConvertLottableToDate(!Lottable05.ToString, TimeZone)
                                        End If
                                        If Not .IsNull("Lottable05LABEL") Then
                                            InputDetailsFields += IIf(i > 0, "~~~", "") & IIf(!Lottable05LABEL.ToString = "", "Lottable05", !Lottable05LABEL)
                                        End If
                                    End With
                                Next

                                SavedDetailsFields += ";;;Lottable06:::"
                                InputDetailsFields += ";;;Lottable06:::"
                                ReadOnlyDetailsFields += "~~~Lottable06"
                                For i = 0 To ds.Tables(1).Rows.Count - 1
                                    With ds.Tables(1).Rows(i)
                                        If Not .IsNull("Lottable06") And Not .IsNull("Lottable06LABEL") Then
                                            SavedDetailsFields += IIf(i > 0, "~~~", "") & !Lottable06
                                        End If
                                        If Not .IsNull("Lottable06LABEL") Then
                                            InputDetailsFields += IIf(i > 0, "~~~", "") & IIf(!Lottable06LABEL.ToString = "", "Lottable06", !Lottable06LABEL)
                                        End If
                                    End With
                                Next

                                SavedDetailsFields += ";;;Lottable07:::"
                                InputDetailsFields += ";;;Lottable07:::"
                                ReadOnlyDetailsFields += "~~~Lottable07"
                                For i = 0 To ds.Tables(1).Rows.Count - 1
                                    With ds.Tables(1).Rows(i)
                                        If Not .IsNull("Lottable07") Then
                                            SavedDetailsFields += IIf(i > 0, "~~~", "") & !Lottable07
                                        End If
                                        If Not .IsNull("Lottable07LABEL") Then
                                            InputDetailsFields += IIf(i > 0, "~~~", "") & IIf(!Lottable07LABEL.ToString = "", "Lottable07", !Lottable07LABEL)
                                        End If
                                    End With
                                Next

                                SavedDetailsFields += ";;;Lottable08:::"
                                InputDetailsFields += ";;;Lottable08:::"
                                ReadOnlyDetailsFields += "~~~Lottable08"
                                For i = 0 To ds.Tables(1).Rows.Count - 1
                                    With ds.Tables(1).Rows(i)
                                        If Not .IsNull("Lottable08") Then
                                            SavedDetailsFields += IIf(i > 0, "~~~", "") & !Lottable08
                                        End If
                                        If Not .IsNull("Lottable08LABEL") Then
                                            InputDetailsFields += IIf(i > 0, "~~~", "") & IIf(!Lottable08LABEL.ToString = "", "Lottable08", !Lottable08LABEL)
                                        End If
                                    End With
                                Next

                                SavedDetailsFields += ";;;Lottable09:::"
                                InputDetailsFields += ";;;Lottable09:::"
                                ReadOnlyDetailsFields += "~~~Lottable09"
                                For i = 0 To ds.Tables(1).Rows.Count - 1
                                    With ds.Tables(1).Rows(i)
                                        If Not .IsNull("Lottable09") Then
                                            SavedDetailsFields += IIf(i > 0, "~~~", "") & !Lottable09
                                        End If
                                        If Not .IsNull("Lottable09LABEL") Then
                                            InputDetailsFields += IIf(i > 0, "~~~", "") & IIf(!Lottable09LABEL.ToString = "", "Lottable09", !Lottable09LABEL)
                                        End If
                                    End With
                                Next

                                SavedDetailsFields += ";;;Lottable10:::"
                                InputDetailsFields += ";;;Lottable10:::"
                                ReadOnlyDetailsFields += "~~~Lottable10"
                                For i = 0 To ds.Tables(1).Rows.Count - 1
                                    With ds.Tables(1).Rows(i)
                                        If Not .IsNull("Lottable10") Then
                                            SavedDetailsFields += IIf(i > 0, "~~~", "") & !Lottable10
                                        End If
                                        If Not .IsNull("Lottable10LABEL") Then
                                            InputDetailsFields += IIf(i > 0, "~~~", "") & IIf(!Lottable10LABEL.ToString = "", "Lottable10", !Lottable10LABEL)
                                        End If
                                    End With
                                Next

                                SavedDetailsFields += ";;;Lottable11:::"
                                InputDetailsFields += ";;;Lottable11:::"
                                ReadOnlyDetailsFields += "~~~Lottable11"
                                For i = 0 To ds.Tables(1).Rows.Count - 1
                                    With ds.Tables(1).Rows(i)
                                        If Not .IsNull("Lottable11") Then
                                            If i > 0 Then SavedDetailsFields += "~~~"
                                            SavedDetailsFields += CommonMethods.ConvertLottableToDate(!Lottable11.ToString, TimeZone)
                                        End If
                                        If Not .IsNull("Lottable11LABEL") Then
                                            InputDetailsFields += IIf(i > 0, "~~~", "") & IIf(!Lottable11LABEL.ToString = "", "Lottable11", !Lottable11LABEL)
                                        End If
                                    End With
                                Next

                                SavedDetailsFields += ";;;Lottable12:::"
                                InputDetailsFields += ";;;Lottable12:::"
                                ReadOnlyDetailsFields += "~~~Lottable12"
                                For i = 0 To ds.Tables(1).Rows.Count - 1
                                    With ds.Tables(1).Rows(i)
                                        If Not .IsNull("Lottable12") Then
                                            If i > 0 Then SavedDetailsFields += "~~~"
                                            SavedDetailsFields += CommonMethods.ConvertLottableToDate(!Lottable12.ToString, TimeZone)
                                        End If
                                        If Not .IsNull("Lottable12LABEL") Then
                                            InputDetailsFields += IIf(i > 0, "~~~", "") & IIf(!Lottable12LABEL.ToString = "", "Lottable12", !Lottable12LABEL)
                                        End If
                                    End With
                                Next
                                DetailsCount = ds.Tables(1).Rows.Count
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

                                    Dim measureunit As Double = 1
                                    Dim externln As String = ""
                                    Dim Sku As String = ""
                                    Dim QtyOrdered As String = ""
                                    Dim QtyReceived As String = ""
                                    Dim resultsDts As XmlNodeList = MyDoc.SelectNodes("//*[local-name()='PurchaseOrderDetail']")
                                    DetailsCount = resultsDts.Count
                                    Dim i As Integer = -1
                                    If DetailsCount > 0 Then
                                        For Each nodeDT As XmlNode In resultsDts
                                            i = i + 1
                                            measureunit = CommonMethods.getUomMeasure(CommonMethods.getFacilityDBName(Facility), nodeDT("PackKey").InnerText.ToString(), nodeDT("UOM").InnerText.ToString())
                                            If Not nodeDT("ExternLineNo").IsEmpty Then
                                                externln += IIf(i <> 0, "~~~", "") & nodeDT("ExternLineNo").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Sku").IsEmpty Then
                                                Sku += IIf(i <> 0, "~~~", "") & nodeDT("Sku").InnerText.ToString()
                                            End If
                                            If Not nodeDT("QtyOrdered").IsEmpty Then
                                                QtyOrdered += IIf(i <> 0, "~~~", "") & (Double.Parse(nodeDT("QtyOrdered").InnerText.ToString()) / measureunit).ToString
                                            End If
                                            If Not nodeDT("QtyReceived").IsEmpty Then
                                                QtyReceived += IIf(i <> 0, "~~~", "") & (Double.Parse(nodeDT("QtyReceived").InnerText.ToString()) / measureunit).ToString
                                            End If
                                        Next
                                        SavedDetailsFields += "ExternLineNo:::" & externln
                                        SavedDetailsFields += ";;;Sku:::" & Sku
                                        SavedDetailsFields += ";;;QtyOrdered:::" & QtyOrdered
                                        SavedDetailsFields += ";;;QtyReceived:::" & QtyReceived
                                        ReadOnlyDetailsFields = "ExternLineNo~~~Sku~~~QtyOrdered~~~QtyReceived"
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

                                    Dim measureunit As Double = 1
                                    Dim externln As String = "", Sku As String = "", QtyExpected As String = "", QtyReceived As String = "",
                                        PackKey As String = "", UOM As String = "", POKeyDtl As String = "", ToId As String = "",
                                        ToLoc As String = "", ConditionCode As String = "", TariffKey As String = "", Lottable01 As String = "",
                                        Lottable02 As String = "", Lottable03 As String = "", Lottable04 As String = "", Lottable05 As String = "",
                                        Lottable06 As String = "", Lottable07 As String = "", Lottable08 As String = "", Lottable09 As String = "",
                                        Lottable10 As String = "", Lottable11 As String = "", Lottable12 As String = ""

                                    Dim resultsDts As XmlNodeList = MyDoc.SelectNodes("//*[local-name()='AdvancedShipNoticeDetail']")
                                    DetailsCount = resultsDts.Count
                                    Dim i As Integer = -1
                                    If DetailsCount > 0 Then
                                        For Each nodeDT As XmlNode In resultsDts
                                            i = i + 1
                                            measureunit = CommonMethods.getUomMeasure(warehouse, nodeDT("PackKey").InnerText.ToString(), nodeDT("UOM").InnerText.ToString())
                                            If Not nodeDT("ExternLineNo").IsEmpty Then
                                                externln += IIf(i <> 0, "~~~", "") & nodeDT("ExternLineNo").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Sku").IsEmpty Then
                                                Sku += IIf(i <> 0, "~~~", "") & nodeDT("Sku").InnerText.ToString()
                                            End If
                                            If Not nodeDT("QtyExpected").IsEmpty Then
                                                QtyExpected += IIf(i <> 0, "~~~", "") & (Double.Parse(nodeDT("QtyExpected").InnerText.ToString()) / measureunit).ToString
                                            End If
                                            If Not nodeDT("QtyReceived").IsEmpty Then
                                                QtyReceived += IIf(i <> 0, "~~~", "") & (Double.Parse(nodeDT("QtyReceived").InnerText.ToString()) / measureunit).ToString
                                            End If
                                            If Not nodeDT("PackKey").IsEmpty Then
                                                PackKey += IIf(i <> 0, "~~~", "") & nodeDT("PackKey").InnerText.ToString()
                                            End If
                                            If Not nodeDT("UOM").IsEmpty Then
                                                UOM += IIf(i <> 0, "~~~", "") & nodeDT("UOM").InnerText.ToString()
                                            End If
                                            If Not nodeDT("POKey").IsEmpty Then
                                                POKeyDtl += IIf(i <> 0, "~~~", "") & nodeDT("POKey").InnerText.ToString()
                                            End If
                                            If Not nodeDT("ToId").IsEmpty Then
                                                ToId += IIf(i <> 0, "~~~", "") & nodeDT("ToId").InnerText.ToString()
                                            End If
                                            If Not nodeDT("ToLoc").IsEmpty Then
                                                ToLoc += IIf(i <> 0, "~~~", "") & nodeDT("ToLoc").InnerText.ToString()
                                            End If
                                            If Not nodeDT("ConditionCode").IsEmpty Then
                                                ConditionCode += IIf(i <> 0, "~~~", "") & nodeDT("ConditionCode").InnerText.ToString()
                                            End If
                                            If Not nodeDT("TariffKey").IsEmpty Then
                                                TariffKey += IIf(i <> 0, "~~~", "") & nodeDT("TariffKey").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Lottable01").IsEmpty Then
                                                Lottable01 += IIf(i <> 0, "~~~", "") & nodeDT("Lottable01").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Lottable02").IsEmpty Then
                                                Lottable02 += IIf(i <> 0, "~~~", "") & nodeDT("Lottable02").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Lottable03").IsEmpty Then
                                                Lottable03 += IIf(i <> 0, "~~~", "") & nodeDT("Lottable03").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Lottable04").IsEmpty Then
                                                Dim DateTime As DateTime
                                                Dim DateStr = nodeDT("Lottable04").InnerText.ToString()
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
                                                Lottable04 += IIf(i <> 0, "~~~", "") & DateTime.ToString("MM/dd/yyyy")
                                            End If
                                            If Not nodeDT("Lottable05").IsEmpty Then
                                                Dim DateTime As DateTime
                                                Dim DateStr = nodeDT("Lottable05").InnerText.ToString()
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
                                                Lottable05 += IIf(i <> 0, "~~~", "") & DateTime.ToString("MM/dd/yyyy")
                                            End If
                                            If Not nodeDT("Lottable06").IsEmpty Then
                                                Lottable06 += IIf(i <> 0, "~~~", "") & nodeDT("Lottable06").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Lottable07").IsEmpty Then
                                                Lottable07 += IIf(i <> 0, "~~~", "") & nodeDT("Lottable07").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Lottable08").IsEmpty Then
                                                Lottable08 += IIf(i <> 0, "~~~", "") & nodeDT("Lottable08").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Lottable09").IsEmpty Then
                                                Lottable09 += IIf(i <> 0, "~~~", "") & nodeDT("Lottable09").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Lottable10").IsEmpty Then
                                                Lottable10 += IIf(i <> 0, "~~~", "") & nodeDT("Lottable10").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Lottable11").IsEmpty Then
                                                Dim DateTime As DateTime
                                                Dim DateStr = nodeDT("Lottable11").InnerText.ToString()
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
                                                Lottable11 += IIf(i <> 0, "~~~", "") & DateTime.ToString("MM/dd/yyyy")
                                            End If
                                            If Not nodeDT("Lottable12").IsEmpty Then
                                                Dim DateTime As DateTime
                                                Dim DateStr = nodeDT("Lottable12").InnerText.ToString()
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
                                                Lottable12 += IIf(i <> 0, "~~~", "") & DateTime.ToString("MM/dd/yyyy")
                                            End If
                                        Next
                                        SavedDetailsFields += "ExternLineNo:::" & externln
                                        SavedDetailsFields += ";;;Sku:::" & Sku
                                        SavedDetailsFields += ";;;QtyExpected:::" & QtyExpected
                                        SavedDetailsFields += ";;;QtyReceived:::" & QtyReceived
                                        SavedDetailsFields += ";;;PackKey:::" & PackKey
                                        SavedDetailsFields += ";;;UOM:::" & UOM
                                        SavedDetailsFields += ";;;POKey:::" & POKeyDtl
                                        SavedDetailsFields += ";;;ToId:::" & ToId
                                        SavedDetailsFields += ";;;ToLoc:::" & ToLoc
                                        SavedDetailsFields += ";;;ConditionCode:::" & ConditionCode
                                        SavedDetailsFields += ";;;TariffKey:::" & TariffKey
                                        SavedDetailsFields += ";;;Lottable01:::" & Lottable01
                                        SavedDetailsFields += ";;;Lottable02:::" & Lottable02
                                        SavedDetailsFields += ";;;Lottable03:::" & Lottable03
                                        SavedDetailsFields += ";;;Lottable04:::" & Lottable04
                                        SavedDetailsFields += ";;;Lottable05:::" & Lottable05
                                        SavedDetailsFields += ";;;Lottable06:::" & Lottable06
                                        SavedDetailsFields += ";;;Lottable07:::" & Lottable07
                                        SavedDetailsFields += ";;;Lottable08:::" & Lottable08
                                        SavedDetailsFields += ";;;Lottable09:::" & Lottable09
                                        SavedDetailsFields += ";;;Lottable10:::" & Lottable10
                                        SavedDetailsFields += ";;;Lottable11:::" & Lottable11
                                        SavedDetailsFields += ";;;Lottable12:::" & Lottable12

                                        ReadOnlyDetailsFields += "ExternLineNo~~~Sku~~~QtyOrdered~~~QtyReceived~~~PackKey"
                                        ReadOnlyDetailsFields += "~~~UOM~~~POKey~~~ToId~~~ToLoc~~~ConditionCode~~~TariffKey"
                                        ReadOnlyDetailsFields += "~~~Lottable01~~~Lottable02~~~Lottable03~~~Lottable04"
                                        ReadOnlyDetailsFields += "~~~Lottable05~~~Lottable06~~~Lottable07~~~Lottable08"
                                        ReadOnlyDetailsFields += "~~~Lottable09~~~Lottable10~~~Lottable11~~~Lottable12"
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

                                    Dim measureunit As Double = 1
                                    Dim externln As String = "", Sku As String = "", OpenQty As String = "",
                                    PackKey As String = "", UOM As String = "", UDF1Dtl As String = "", UDF2Dtl As String = "",
                                        UDF3Dtl As String = "", UDF4Dtl As String = "", UDF5Dtl As String = "", Lottable01 As String = "",
                                        Lottable02 As String = "", Lottable03 As String = "", Lottable04 As String = "", Lottable05 As String = "",
                                        Lottable06 As String = "", Lottable07 As String = "", Lottable08 As String = "", Lottable09 As String = "",
                                        Lottable10 As String = ""

                                    Dim resultsDts As XmlNodeList = MyDoc.SelectNodes("//*[local-name()='ShipmentOrderDetail']")
                                    DetailsCount = resultsDts.Count
                                    Dim i As Integer = -1
                                    If DetailsCount > 0 Then
                                        For Each nodeDT As XmlNode In resultsDts
                                            i = i + 1
                                            measureunit = CommonMethods.getUomMeasure(warehouse, nodeDT("PackKey").InnerText.ToString(), nodeDT("UOM").InnerText.ToString())
                                            If Not nodeDT("ExternLineNo").IsEmpty Then
                                                externln += IIf(i <> 0, "~~~", "") & nodeDT("ExternLineNo").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Sku").IsEmpty Then
                                                Sku += IIf(i <> 0, "~~~", "") & nodeDT("Sku").InnerText.ToString()
                                            End If
                                            If Not nodeDT("OpenQty").IsEmpty Then
                                                OpenQty += IIf(i <> 0, "~~~", "") & (Double.Parse(nodeDT("OpenQty").InnerText.ToString()) / measureunit).ToString
                                            End If
                                            If Not nodeDT("PackKey").IsEmpty Then
                                                PackKey += IIf(i <> 0, "~~~", "") & nodeDT("PackKey").InnerText.ToString()
                                            End If
                                            If Not nodeDT("UOM").IsEmpty Then
                                                UOM += IIf(i <> 0, "~~~", "") & nodeDT("UOM").InnerText.ToString()
                                            End If
                                            If Not nodeDT("SUsr1").IsEmpty Then
                                                UDF1Dtl += IIf(i <> 0, "~~~", "") & nodeDT("SUsr1").InnerText.ToString()
                                            End If
                                            If Not nodeDT("SUsr2").IsEmpty Then
                                                UDF2Dtl += IIf(i <> 0, "~~~", "") & nodeDT("SUsr2").InnerText.ToString()
                                            End If
                                            If Not nodeDT("SUsr3").IsEmpty Then
                                                UDF3Dtl += IIf(i <> 0, "~~~", "") & nodeDT("SUsr3").InnerText.ToString()
                                            End If
                                            If Not nodeDT("SUsr4").IsEmpty Then
                                                UDF4Dtl += IIf(i <> 0, "~~~", "") & nodeDT("SUsr4").InnerText.ToString()
                                            End If
                                            If Not nodeDT("SUsr5").IsEmpty Then
                                                UDF5Dtl += IIf(i <> 0, "~~~", "") & nodeDT("SUsr5").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Lottable01").IsEmpty Then
                                                Lottable01 += IIf(i <> 0, "~~~", "") & nodeDT("Lottable01").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Lottable02").IsEmpty Then
                                                Lottable02 += IIf(i <> 0, "~~~", "") & nodeDT("Lottable02").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Lottable03").IsEmpty Then
                                                Lottable03 += IIf(i <> 0, "~~~", "") & nodeDT("Lottable03").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Lottable04").IsEmpty Then
                                                Dim DateTime As DateTime
                                                Dim DateStr = nodeDT("Lottable04").InnerText.ToString()
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
                                                Lottable04 += IIf(i <> 0, "~~~", "") & DateTime.ToString("MM/dd/yyyy")
                                            End If
                                            If Not nodeDT("Lottable05").IsEmpty Then
                                                Dim DateTime As DateTime
                                                Dim DateStr = nodeDT("Lottable05").InnerText.ToString()
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
                                                Lottable05 += IIf(i <> 0, "~~~", "") & DateTime.ToString("MM/dd/yyyy")
                                            End If
                                            If Not nodeDT("Lottable06").IsEmpty Then
                                                Lottable06 += IIf(i <> 0, "~~~", "") & nodeDT("Lottable06").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Lottable07").IsEmpty Then
                                                Lottable07 += IIf(i <> 0, "~~~", "") & nodeDT("Lottable07").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Lottable08").IsEmpty Then
                                                Lottable08 += IIf(i <> 0, "~~~", "") & nodeDT("Lottable08").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Lottable09").IsEmpty Then
                                                Lottable09 += IIf(i <> 0, "~~~", "") & nodeDT("Lottable09").InnerText.ToString()
                                            End If
                                            If Not nodeDT("Lottable10").IsEmpty Then
                                                Lottable10 += IIf(i <> 0, "~~~", "") & nodeDT("Lottable10").InnerText.ToString()
                                            End If
                                        Next
                                        SavedDetailsFields += "ExternLineNo:::" & externln
                                        SavedDetailsFields += ";;;Sku:::" & Sku
                                        SavedDetailsFields += ";;;OpenQty:::" & OpenQty
                                        SavedDetailsFields += ";;;PackKey:::" & PackKey
                                        SavedDetailsFields += ";;;UOM:::" & UOM
                                        SavedDetailsFields += ";;;SUsr1:::" & UDF1Dtl
                                        SavedDetailsFields += ";;;SUsr2:::" & UDF2Dtl
                                        SavedDetailsFields += ";;;SUsr3:::" & UDF3Dtl
                                        SavedDetailsFields += ";;;SUsr4:::" & UDF4Dtl
                                        SavedDetailsFields += ";;;SUsr5:::" & UDF5Dtl
                                        SavedDetailsFields += ";;;Lottable01:::" & Lottable01
                                        SavedDetailsFields += ";;;Lottable02:::" & Lottable02
                                        SavedDetailsFields += ";;;Lottable03:::" & Lottable03
                                        SavedDetailsFields += ";;;Lottable04:::" & Lottable04
                                        SavedDetailsFields += ";;;Lottable05:::" & Lottable05
                                        SavedDetailsFields += ";;;Lottable06:::" & Lottable06
                                        SavedDetailsFields += ";;;Lottable07:::" & Lottable07
                                        SavedDetailsFields += ";;;Lottable08:::" & Lottable08
                                        SavedDetailsFields += ";;;Lottable09:::" & Lottable09
                                        SavedDetailsFields += ";;;Lottable10:::" & Lottable10

                                        ReadOnlyDetailsFields += "ExternLineNo~~~Sku~~~OpenQty~~~PackKey~~~UOM"
                                        ReadOnlyDetailsFields += "~~~SUsr1~~~SUsr2~~~SUsr3~~~SUsr4~~~SUsr5"
                                        ReadOnlyDetailsFields += "~~~Lottable01~~~Lottable02~~~Lottable03~~~Lottable04"
                                        ReadOnlyDetailsFields += "~~~Lottable05~~~Lottable06~~~Lottable07~~~Lottable08"
                                        ReadOnlyDetailsFields += "~~~Lottable09~~~Lottable10"
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

                            Dim externln As String = "", Sku As String = "", OpenQty As String = "",
                                PackKey As String = "", UOM As String = "", Price As String = "",
                                Currency As String = "", UDF1Dtl As String = "", UDF2Dtl As String = "",
                                UDF3Dtl As String = "", UDF4Dtl As String = "", Lottable01 As String = "",
                                Lottable02 As String = "", Lottable03 As String = "", Lottable04 As String = "", Lottable05 As String = "",
                                Lottable06 As String = "", Lottable07 As String = "", Lottable08 As String = "", Lottable09 As String = "",
                                Lottable10 As String = ""
                            Dim dr As DataRow() = ds.Tables(1).Select("WHSEID = '" & !WHSEID & "' and EXTERNORDERKEY ='" & !ExternOrderKey & "'")
                            DetailsCount = dr.Count
                            For j = 0 To DetailsCount - 1
                                With dr(j)
                                    If Not .IsNull("ExternLineNo") Then
                                        externln += IIf(j > 0, "~~~", "") & !ExternLineNo
                                    End If

                                    If Not .IsNull("Sku") Then
                                        Sku += IIf(j > 0, "~~~", "") & !Sku
                                    End If

                                    If Not .IsNull("OpenQty") Then
                                        OpenQty += IIf(j > 0, "~~~", "") & Val(!OpenQty)
                                    End If

                                    If Not .IsNull("PackKey") Then
                                        PackKey += IIf(j > 0, "~~~", "") & !PackKey
                                    End If

                                    If Not .IsNull("UOM") Then
                                        UOM += IIf(j > 0, "~~~", "") & !UOM
                                    End If

                                    If Not .IsNull("UnitPrice") Then
                                        Price += IIf(j > 0, "~~~", "") & !UnitPrice.ToString
                                    End If

                                    If Not .IsNull("SUsr5") Then
                                        Currency += IIf(j > 0, "~~~", "") & !SUsr5
                                    End If

                                    If Not .IsNull("SUsr1") Then
                                        UDF1Dtl += IIf(j > 0, "~~~", "") & !SUsr1
                                    End If

                                    If Not .IsNull("SUsr2") Then
                                        UDF2Dtl += IIf(j > 0, "~~~", "") & !SUsr2
                                    End If

                                    If Not .IsNull("SUsr3") Then
                                        UDF3Dtl += IIf(j > 0, "~~~", "") & !SUsr3
                                    End If

                                    If Not .IsNull("SUsr4") Then
                                        UDF4Dtl += IIf(j > 0, "~~~", "") & !SUsr4
                                    End If

                                    If Not .IsNull("Lottable01") Then
                                        Lottable01 += IIf(j > 0, "~~~", "") & !Lottable01
                                    End If

                                    If Not .IsNull("Lottable02") Then
                                        Lottable02 += IIf(j > 0, "~~~", "") & !Lottable02
                                    End If

                                    If Not .IsNull("Lottable03") Then
                                        Lottable03 += IIf(j > 0, "~~~", "") & !Lottable03
                                    End If

                                    If Not .IsNull("Lottable04") Then
                                        Dim DateStr = !Lottable04.ToString()
                                        If DateStr <> "" Then
                                            Dim DateTime As DateTime
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
                                            Lottable04 += IIf(j > 0, "~~~", "") & DateTime.ToString("MM/dd/yyyy")
                                        End If
                                    End If

                                    If Not .IsNull("Lottable05") Then
                                        Dim DateStr = !Lottable05.ToString()
                                        If DateStr <> "" Then
                                            Dim DateTime As DateTime
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
                                            Lottable05 += IIf(j > 0, "~~~", "") & DateTime.ToString("MM/dd/yyyy")
                                        End If
                                    End If

                                    If Not .IsNull("Lottable06") Then
                                        Lottable06 += IIf(j > 0, "~~~", "") & !Lottable06
                                    End If

                                    If Not .IsNull("Lottable07") Then
                                        Lottable07 += IIf(j > 0, "~~~", "") & !Lottable07
                                    End If

                                    If Not .IsNull("Lottable08") Then
                                        Lottable08 += IIf(j > 0, "~~~", "") & !Lottable08
                                    End If

                                    If Not .IsNull("Lottable09") Then
                                        Lottable09 += IIf(j > 0, "~~~", "") & !Lottable09
                                    End If

                                    If Not .IsNull("Lottable10") Then
                                        Lottable10 += IIf(j > 0, "~~~", "") & !Lottable10
                                    End If
                                End With
                            Next
                            SavedDetailsFields += "ExternLineNo:::" & externln
                            SavedDetailsFields += ";;;Sku:::" & Sku
                            SavedDetailsFields += ";;;OpenQty:::" & OpenQty
                            SavedDetailsFields += ";;;PackKey:::" & PackKey
                            SavedDetailsFields += ";;;UOM:::" & UOM
                            SavedDetailsFields += ";;;Price:::" & Price
                            SavedDetailsFields += ";;;Currency:::" & Currency
                            SavedDetailsFields += ";;;SUsr1:::" & UDF1Dtl
                            SavedDetailsFields += ";;;SUsr2:::" & UDF2Dtl
                            SavedDetailsFields += ";;;SUsr3:::" & UDF3Dtl
                            SavedDetailsFields += ";;;SUsr4:::" & UDF4Dtl
                            SavedDetailsFields += ";;;Lottable01:::" & Lottable01
                            SavedDetailsFields += ";;;Lottable02:::" & Lottable02
                            SavedDetailsFields += ";;;Lottable03:::" & Lottable03
                            SavedDetailsFields += ";;;Lottable04:::" & Lottable04
                            SavedDetailsFields += ";;;Lottable05:::" & Lottable05
                            SavedDetailsFields += ";;;Lottable06:::" & Lottable06
                            SavedDetailsFields += ";;;Lottable07:::" & Lottable07
                            SavedDetailsFields += ";;;Lottable08:::" & Lottable08
                            SavedDetailsFields += ";;;Lottable09:::" & Lottable09
                            SavedDetailsFields += ";;;Lottable10:::" & Lottable10

                            ReadOnlyDetailsFields += "ExternLineNo~~~Sku~~~OpenQty~~~PackKey~~~UOM"
                            ReadOnlyDetailsFields += "~~~Price~~~Currency~~~SUsr1~~~SUsr2~~~SUsr3~~~SUsr4"
                            ReadOnlyDetailsFields += "~~~Lottable01~~~Lottable02~~~Lottable03~~~Lottable04"
                            ReadOnlyDetailsFields += "~~~Lottable05~~~Lottable06~~~Lottable07~~~Lottable08"
                            ReadOnlyDetailsFields += "~~~Lottable09~~~Lottable10"
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

            writer.WritePropertyName("DetailsCount")
            writer.WriteValue(DetailsCount)

            writer.WritePropertyName("SavedDetailsFields")
            writer.WriteValue(SavedDetailsFields)

            writer.WritePropertyName("ReadOnlyDetailsFields")
            writer.WriteValue(ReadOnlyDetailsFields)

            writer.WritePropertyName("InputDetailsFields")
            writer.WriteValue(InputDetailsFields)

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

    Private Function GetMyID(ByVal SearchTable As String, ByVal StrID As String) As Integer
        Dim MyID As Integer = 0
        Dim AndFilter As String = ""
        Dim sql As String = ""
        Dim tb As SQLExec = New SQLExec
        Dim ds As DataSet = Nothing
        Select Case SearchTable
            Case "PORTALUSERS", "SKUCATALOGUE"
                MyID = StrID.Split("=")(1)
            Case "USERCONTROL"
                sql += "Select " & IIf(CommonMethods.dbtype = "sql", "ID", "SerialKey") & " From " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & SearchTable & " where UserKey = '" & StrID.Split("=")(1) & "'"
            Case "enterprise.storer2", "enterprise.storer5"
                sql += "Select SerialKey from " & SearchTable.Remove(SearchTable.Length - 1) & " where StorerKey = '" & StrID.Split("=")(1) & "' and Type = " & SearchTable(SearchTable.Length - 1)
            Case "enterprise.sku"
                sql += "Select SerialKey from " & SearchTable & " where StorerKey = '" & StrID.Split("=")(1).Split("&")(0) & "' and Sku = '" & StrID.Split("=")(2) & "'"
            Case "Warehouse_PO", "Warehouse_ASN", "Warehouse_SO", "Warehouse_OrderManagement"
                Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
                Dim suppliers As String() = CommonMethods.getSupplierPerUser(HttpContext.Current.Session("userkey").ToString)
                Dim consignees As String() = CommonMethods.getConsigneePerUser(HttpContext.Current.Session("userkey").ToString)
                If owners IsNot Nothing And suppliers IsNot Nothing And consignees IsNot Nothing Then
                    Dim ownersstr As String = String.Join("','", owners)
                    ownersstr = "'" & ownersstr & "'"
                    If Not UCase(ownersstr).Contains("'ALL'") Then AndFilter += " and STORERKEY IN (" & ownersstr & ")"

                    If SearchTable = "Warehouse_PO" Then
                        Dim suppliersstr As String = String.Join("','", suppliers)
                        suppliersstr = "'" & suppliersstr & "'"
                        If Not UCase(suppliersstr).Contains("'ALL'") Then AndFilter += " and SellerName IN (" & suppliersstr & ")"
                    End If

                    If SearchTable = "Warehouse_SO" Or SearchTable = "Warehouse_OrderManagement" Then
                        Dim consigneesstr As String = String.Join("','", consignees)
                        consigneesstr = "'" & consigneesstr & "'"
                        If Not UCase(consigneesstr).Contains("'ALL'") Then AndFilter += " and ConsigneeKey IN (" & consigneesstr & ")"
                    End If

                    Dim warehouse As String = StrID.Split("=")(1).Split("&")(0)
                    Dim warehouselevel As String = warehouse
                    If SearchTable <> "Warehouse_OrderManagement" Then
                        If LCase(warehouse.Substring(0, 6)) = "infor_" Then
                            warehouselevel = warehouse.Substring(6, warehouse.Length - 6)
                        End If
                        warehouselevel = warehouselevel.Split("_")(1)
                    End If

                    sql += "Select SerialKey from " & warehouselevel
                    If SearchTable = "Warehouse_PO" Then
                        sql += ".PO where POKey = "
                    ElseIf SearchTable = "Warehouse_ASN" Then
                        sql += ".Receipt where ReceiptKey = "
                    ElseIf SearchTable = "Warehouse_SO" Then
                        sql += ".Orders where OrderKey = "
                    ElseIf SearchTable = "Warehouse_OrderManagement" Then
                        sql = "Select SerialKey from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAG where WHSEID = '" & warehouselevel & "' and ExternOrderKey = "
                    End If
                    sql += "'" & StrID.Split("=")(2) & "'" & AndFilter
                End If
        End Select

        If sql <> "" Then
            ds = tb.Cursor(sql)
            If ds.Tables(0).Rows.Count > 0 Then MyID = ds.Tables(0).Rows(0)(0)
        End If

        Return Val(MyID)
    End Function
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class