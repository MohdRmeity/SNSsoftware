Imports Newtonsoft.Json
Imports System.Data.SqlClient
Imports Oracle.ManagedDataAccess.Client
Imports NLog
Imports System.IO
Imports System.Globalization
Imports System.Xml

Public Class SaveItemsNew
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub SaveItem(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim mySearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim MyID As String = HttpContext.Current.Request.Item("MyID")
        Dim MyDataTable As DataTable = Nothing

        If MyID.Contains("?") Then
            MyID = CommonMethods.GetMyID(mySearchTable, MyID)
        End If

        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)
        Using writer As JsonWriter = New JsonTextWriter(sw)
            writer.Formatting = Newtonsoft.Json.Formatting.Indented
            writer.WriteStartObject()

            If mySearchTable.Contains("enterprise.storer") Then
                Dim type As String = IIf(mySearchTable = "enterprise.storer2", "2", "12")
                MyDataTable = SaveConfiguration(MyID, type)
            ElseIf mySearchTable = "enterprise.sku" Then
                MyDataTable = SaveItem(MyID)
            ElseIf mySearchTable = "SKUCATALOGUE" Then
                MyDataTable = SaveItemCatalogue(MyID)
            ElseIf mySearchTable = "Warehouse_PO" Then
                MyDataTable = SavePurchaseOrder(MyID)
            ElseIf mySearchTable = "Warehouse_ASN" Then
                MyDataTable = SaveASN(MyID)
            ElseIf mySearchTable = "Warehouse_SO" Then
                MyDataTable = SaveSO(MyID)
            ElseIf mySearchTable = "Warehouse_OrderManagement" Then
                MyDataTable = SaveOrderManagement(MyID)
            End If

            writer.WritePropertyName("tmp")
            writer.WriteValue(MyDataTable.Rows(0)!tmp)

            writer.WritePropertyName("key")
            If mySearchTable.Contains("enterprise") Or mySearchTable = "SKUCATALOGUE" Then
                writer.WriteValue("")
            Else
                writer.WriteValue(MyDataTable.Rows(0)!key)
            End If

            writer.WritePropertyName("Facility")
            writer.WriteValue(CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Field_Facility")))

            writer.WritePropertyName("serialkey")
            writer.WriteValue(MyDataTable.Rows(0)!serialkey)

            writer.WritePropertyName("queryurl")
            writer.WriteValue(MyDataTable.Rows(0)!queryurl)

            writer.WriteEndObject()
        End Using
        context.Response.Write(sw)
        context.Response.End()

    End Sub
    Private Function SaveConfiguration(ByVal MyID As Integer, ByVal type As String) As DataTable
        Dim MyTable As New DataTable
        MyTable.Columns.Add("tmp", GetType(String))
        MyTable.Columns.Add("serialkey", GetType(String))
        MyTable.Columns.Add("queryurl", GetType(String))

        Dim tmp As String = "", Command As String = "", serialkey As String = "", queryurl As String = ""
        Dim IsValid As Boolean = True
        Dim EditOperation As Boolean = MyID <> 0
        Dim r As Regex = New Regex("[~`!#$%^&*()+=|\{}':;,<>/?[\]""]")
        Dim storer As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
        , company As String = HttpContext.Current.Request.Item("Field_Company") _
        , descr As String = HttpContext.Current.Request.Item("Field_Description") _
        , country As String = HttpContext.Current.Request.Item("Field_Country") _
        , city As String = HttpContext.Current.Request.Item("Field_City") _
        , state As String = HttpContext.Current.Request.Item("Field_State") _
        , zip As String = HttpContext.Current.Request.Item("Field_Zip") _
        , Address1 As String = HttpContext.Current.Request.Item("Field_Address1") _
        , Address2 As String = HttpContext.Current.Request.Item("Field_Address2") _
        , Address3 As String = HttpContext.Current.Request.Item("Field_Address3") _
        , Address4 As String = HttpContext.Current.Request.Item("Field_Address4") _
        , Address5 As String = HttpContext.Current.Request.Item("Field_Address5") _
        , Address6 As String = HttpContext.Current.Request.Item("Field_Address6") _
        , Contact1 As String = HttpContext.Current.Request.Item("Field_Contact1") _
        , Contact2 As String = HttpContext.Current.Request.Item("Field_Contact2") _
        , Email1 As String = HttpContext.Current.Request.Item("Field_Email1") _
        , Email2 As String = HttpContext.Current.Request.Item("Field_Email2") _
        , Phone1 As String = HttpContext.Current.Request.Item("Field_Phone1") _
        , Phone2 As String = HttpContext.Current.Request.Item("Field_Phone2") _
        , UDf1 As String = HttpContext.Current.Request.Item("Field_SUsr1") _
        , UDf2 As String = HttpContext.Current.Request.Item("Field_SUsr2") _
        , UDf3 As String = HttpContext.Current.Request.Item("Field_SUsr3") _
        , UDf4 As String = HttpContext.Current.Request.Item("Field_SUsr4") _
        , UDf5 As String = HttpContext.Current.Request.Item("Field_SUsr5") _
        , isoCode As String = ""

        If Not String.IsNullOrEmpty(storer) And Trim(storer) <> "" Then
            If r.IsMatch(storer) Then
                IsValid = False
                tmp += IIf(type = "2", "Ship To", "Supplier") & " cannot have any special characters <br/>"
            Else
                Command += "<StorerKey>" & storer & "</StorerKey><Type>" & type & "</Type>"
            End If
        Else
            IsValid = False
            tmp += IIf(type = "2", "Ship To", "Supplier") & " cannot be Empty <br/>"
        End If

        If Not String.IsNullOrEmpty(company) Then
            If r.IsMatch(company) Then
                IsValid = False
                tmp += "Company cannot have any special characters <br/>"
            Else
                Command += "<Company>" & company & "</Company>"
            End If
        End If

        If Not String.IsNullOrEmpty(descr) Then
            If r.IsMatch(descr) Then
                IsValid = False
                tmp += "Description cannot have any special characters <br/>"
            Else
                Command += "<Description>" & descr & "</Description>"
            End If
        End If

        If Not String.IsNullOrEmpty(country) Then
            Command += "<Country>" & country & "</Country>"
            isoCode = CommonMethods.getISOCountryCode(country)
            If Not String.IsNullOrEmpty(isoCode) Then
                Command += "<ISOCntryCode>" & isoCode & "</ISOCntryCode>"
            End If
        End If

        If Not String.IsNullOrEmpty(city) Then
            If r.IsMatch(city) Then
                IsValid = False
                tmp += "City cannot have any special characters <br/>"
            Else
                Command += "<City>" & city & "</City>"
            End If
        End If

        If Not String.IsNullOrEmpty(state) Then
            If r.IsMatch(state) Then
                IsValid = False
                tmp += "State cannot have any special characters <br/>"
            Else
                Command += "<State>" & state & "</State>"
            End If
        End If

        If Not String.IsNullOrEmpty(zip) Then
            If r.IsMatch(zip) Then
                IsValid = False
                tmp += "Zip Code cannot have any special characters <br/>"
            Else
                Command += "<Zip>" & zip & "</Zip>"
            End If
        End If

        If Not String.IsNullOrEmpty(Address1) Then
            If r.IsMatch(Address1) Then
                IsValid = False
                tmp += "Address1 cannot have any special characters <br/>"
            Else
                Command += "<Address1>" & Address1 & "</Address1>"
            End If
        End If

        If Not String.IsNullOrEmpty(Address2) Then
            If r.IsMatch(Address2) Then
                IsValid = False
                tmp += "Address2 cannot have any special characters <br/>"
            Else
                Command += "<Address2>" & Address2 & "</Address2>"
            End If
        End If

        If Not String.IsNullOrEmpty(Address3) Then
            If r.IsMatch(Address3) Then
                IsValid = False
                tmp += "Address3 cannot have any special characters <br/>"
            Else
                Command += "<Address3>" & Address3 & "</Address3>"
            End If
        End If

        If Not String.IsNullOrEmpty(Address4) Then
            If r.IsMatch(Address4) Then
                IsValid = False
                tmp += "Address4 cannot have any special characters <br/>"
            Else
                Command += "<Address4>" & Address4 & "</Address4>"
            End If
        End If

        If Not String.IsNullOrEmpty(Address5) Then
            If r.IsMatch(Address5) Then
                IsValid = False
                tmp += "Address5 cannot have any special characters <br/>"
            Else
                Command += "<Address5>" & Address5 & "</Address5>"
            End If
        End If

        If Not String.IsNullOrEmpty(Address6) Then
            If r.IsMatch(Address6) Then
                IsValid = False
                tmp += "Address6 cannot have any special characters <br/>"
            Else
                Command += "<Address6>" & Address6 & "</Address6>"
            End If
        End If

        If Not String.IsNullOrEmpty(Contact1) Then
            If r.IsMatch(Contact1) Then
                IsValid = False
                tmp += "Contact1 cannot have any special characters <br/>"
            Else
                Command += "<Contact1>" & Contact1 & "</Contact1>"
            End If
        End If

        If Not String.IsNullOrEmpty(Contact2) Then
            If r.IsMatch(Contact2) Then
                IsValid = False
                tmp += "Contact2 cannot have any special characters <br/>"
            Else
                Command += "<Contact2>" & Contact2 & "</Contact2>"
            End If
        End If

        If Not String.IsNullOrEmpty(Phone1) Then
            If r.IsMatch(Phone1) Then
                IsValid = False
                tmp += "Phone1 cannot have any special characters <br/>"
            Else
                Command += "<Phone1>" & Phone1 & "</Phone1>"
            End If
        End If

        If Not String.IsNullOrEmpty(Phone2) Then
            If r.IsMatch(Phone2) Then
                IsValid = False
                tmp += "Phone2 cannot have any special characters <br/>"
            Else
                Command += "<Phone2>" & Phone2 & "</Phone2>"
            End If
        End If

        If Not String.IsNullOrEmpty(Email1) Then
            If r.IsMatch(Email1) Then
                IsValid = False
                tmp += "Email1 cannot have any special characters <br/>"
            Else
                Command += "<Email1>" & Email1 & "</Email1>"
            End If
        End If

        If Not String.IsNullOrEmpty(Email2) Then
            If r.IsMatch(Email2) Then
                IsValid = False
                tmp += "Email2 cannot have any special characters <br/>"
            Else
                Command += "<Email2>" & Email2 & "</Email2>"
            End If
        End If

        If Not String.IsNullOrEmpty(UDf1) Then
            If r.IsMatch(UDf1) Then
                IsValid = False
                tmp += "UDF1 cannot have any special characters <br/>"
            Else
                Command += "<SUsr1>" & UDf1 & "</SUsr1>"
            End If
        End If

        If Not String.IsNullOrEmpty(UDf2) Then
            If r.IsMatch(UDf2) Then
                IsValid = False
                tmp += "UDF2 cannot have any special characters <br/>"
            Else
                Command += "<SUsr2>" & UDf2 & "</SUsr2>"
            End If
        End If

        If Not String.IsNullOrEmpty(UDf3) Then
            If r.IsMatch(UDf3) Then
                IsValid = False
                tmp += "UDF3 cannot have any special characters <br/>"
            Else
                Command += "<SUsr3>" & UDf3 & "</SUsr3>"
            End If
        End If

        If Not String.IsNullOrEmpty(UDf4) Then
            If r.IsMatch(UDf4) Then
                IsValid = False
                tmp += "UDF4 cannot have any special characters <br/>"
            Else
                Command += "<SUsr4>" & UDf4 & "</SUsr4>"
            End If
        End If

        If Not String.IsNullOrEmpty(UDf5) Then
            If r.IsMatch(UDf5) Then
                IsValid = False
                tmp += "UDF5 cannot have any special characters <br/>"
            Else
                Command += "<SUsr5>" & UDf5 & "</SUsr5>"
            End If
        End If

        If IsValid Then
            Dim exist As Integer = 0
            If Not EditOperation Then exist = CommonMethods.checkConfigurationExist(storer, type)
            If exist = 0 Then
                Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>storer</MessageType><Action>store</Action><Sender><User>" & CommonMethods.username & "</User><Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID><TenantId>INFOR</TenantId></Sender><Recipient><SystemID>" & CommonMethods.getEnterpriseDBName() & "</SystemID></Recipient></Head><Body><Storer><StorerHeader>" & Command & "</StorerHeader></Storer></Body></Message>"
                tmp = CommonMethods.SaveXml(Xml, IIf(type = "2", "Ship To", "Supplier"), storer)
            Else
                tmp = "Error: " & IIf(type = "2", "Ship To", "Supplier") & " already exists!"
            End If
        End If

        If tmp = "" Then
            If Not EditOperation Then
                Dim sql = "Select top 1 SerialKey from enterprise.storer where Type='" & type & "' order by SerialKey DESC "
                Dim ds As DataSet = (New SQLExec).Cursor(sql)
                If ds.Tables(0).Rows.Count > 0 Then serialkey = ds.Tables(0).Rows(0)!SerialKey
            Else
                serialkey = MyID
            End If
        End If

        queryurl = "?" & IIf(type = "2", "cust", "sup") & "=" & storer
        MyTable.Rows.Add(tmp, serialkey, queryurl)
        Return MyTable
    End Function
    Private Function SaveItem(ByVal MyID As Integer) As DataTable
        Dim MyTable As New DataTable
        MyTable.Columns.Add("tmp", GetType(String))
        MyTable.Columns.Add("serialkey", GetType(String))
        MyTable.Columns.Add("queryurl", GetType(String))

        Dim tmp As String = "", Command As String = "", serialkey As String = "", queryurl As String = ""
        Dim IsValid As Boolean = True
        Dim EditOperation As Boolean = MyID <> 0
        Dim storer As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
        , Sku As String = UCase(HttpContext.Current.Request.Item("Field_Sku")) _
        , PackKey As String = HttpContext.Current.Request.Item("Field_PackKey") _
        , Descr As String = HttpContext.Current.Request.Item("Field_Descr") _
        , TariffKey As String = HttpContext.Current.Request.Item("Field_TariffKey") _
        , StdCube As String = HttpContext.Current.Request.Item("Field_StdCube") _
        , StdNetWgt As String = HttpContext.Current.Request.Item("Field_StdNetWgt") _
        , StdGrossWgt As String = HttpContext.Current.Request.Item("Field_StdGrossWgt") _
        , SkuGroup As String = HttpContext.Current.Request.Item("Field_SkuGroup")

        If Not String.IsNullOrEmpty(storer) And Trim(storer) <> "" Then
            Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
            If owners.Any(Function(x) x = storer) Or owners.Any(Function(x) x = "ALL") Then
                Command += "<StorerKey>" & storer & "</StorerKey>"
            Else
                IsValid = False
                tmp += "This owner is not authorized <br/>"
            End If
        Else
            IsValid = False
            tmp += "Owner cannot be Empty <br/>"
        End If

        If Not String.IsNullOrEmpty(Sku) Then
            Command += "<Sku>" & Sku & "</Sku>"
        Else
            IsValid = False
            tmp += "Item cannot be Empty <br/>"
        End If

        If Not String.IsNullOrEmpty(Descr) Then Command += "<Descr>" & Descr & "</Descr>"
        If Not String.IsNullOrEmpty(PackKey) Then Command += "<PackKey>" & PackKey & "</PackKey>"
        If Not String.IsNullOrEmpty(TariffKey) Then Command += "<TariffKey>" & TariffKey & "</TariffKey>"
        If Not String.IsNullOrEmpty(StdCube) Then Command += "<StdCube>" & StdCube & "</StdCube>"
        If Not String.IsNullOrEmpty(StdNetWgt) Then Command += "<StdNetWgt>" & StdNetWgt & "</StdNetWgt>"
        If Not String.IsNullOrEmpty(StdGrossWgt) Then Command += "<StdGrossWgt>" & StdGrossWgt & "</StdGrossWgt>"
        If Not String.IsNullOrEmpty(SkuGroup) Then Command += "<SkuGroup>" & SkuGroup & "</SkuGroup>"

        If IsValid Then
            Dim exist As Integer = 0
            If Not EditOperation Then exist = CommonMethods.checkItemExist(storer, Sku)
            If exist = 0 Then
                Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>ItemMaster</MessageType><Action>store</Action><Sender><User>" & CommonMethods.username & "</User><Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID><TenantId>INFOR</TenantId></Sender><Recipient><SystemID>" & CommonMethods.getEnterpriseDBName() & "</SystemID></Recipient></Head><Body><ItemMaster><Item>" & Command & "</Item></ItemMaster></Body></Message>"
                tmp = CommonMethods.SaveXml(Xml, "Item", storer & "-" & Sku)
            Else
                tmp = "Error: Item already exists!"
            End If
        End If

        If tmp = "" Then
            If Not EditOperation Then
                Dim sql = "Select top 1 SerialKey from enterprise.sku order by SerialKey DESC "
                Dim ds As DataSet = (New SQLExec).Cursor(sql)
                If ds.Tables(0).Rows.Count > 0 Then serialkey = ds.Tables(0).Rows(0)!SerialKey
            Else
                serialkey = MyID
            End If
        End If
        queryurl = "?storer=" & storer & "&sku=" & Sku
        MyTable.Rows.Add(tmp, serialkey, queryurl)
        Return MyTable
    End Function
    Private Function SaveItemCatalogue(ByVal MyID As Integer) As DataTable
        Dim MyTable As New DataTable
        MyTable.Columns.Add("tmp", GetType(String))
        MyTable.Columns.Add("serialkey", GetType(String))
        MyTable.Columns.Add("queryurl", GetType(String))

        Dim tmp As String = "", serialkey As String = "", queryurl As String = ""
        Dim IsValid As Boolean = True
        Dim EditOperation As Boolean = MyID <> 0
        Dim storer As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
        , consignee As String = UCase(HttpContext.Current.Request.Item("Field_ConsigneeKey")) _
        , sku As String = UCase(HttpContext.Current.Request.Item("Field_Sku")) _
        , price As String = HttpContext.Current.Request.Item("Field_Price") _
        , currency As String = HttpContext.Current.Request.Item("Field_Currency") _
        , udf1 As String = HttpContext.Current.Request.Item("Field_SUsr1") _
        , udf2 As String = HttpContext.Current.Request.Item("Field_SUsr2") _
        , udf3 As String = HttpContext.Current.Request.Item("Field_SUsr3") _
        , udf4 As String = HttpContext.Current.Request.Item("Field_SUsr4") _
        , udf5 As String = HttpContext.Current.Request.Item("Field_SUsr5")

        If String.IsNullOrEmpty(storer) Then
            IsValid = False
            tmp += "Owner must be defined <br/>"
        End If

        If String.IsNullOrEmpty(consignee) Then
            IsValid = False
            tmp += "Consignee must be defined <br/>"
        End If

        If String.IsNullOrEmpty(sku) Then
            IsValid = False
            tmp += "Item must be defined <br/>"
        End If

        If String.IsNullOrEmpty(price) Then
            IsValid = False
            tmp += "Price must be defined <br/>"
        End If

        If String.IsNullOrEmpty(currency) Then
            IsValid = False
            tmp += "Currency be defined <br/>"
        End If

        If IsValid Then
            If EditOperation Then
                Try
                    If CommonMethods.dbtype = "sql" Then
                        Dim updatequery As String = "set dateformat dmy update dbo.SKUCATALOGUE set PRICE =@price , SUsr1=@susr1 ,SUsr2= @susr2 , SUsr3= @susr3, SUsr4= @susr4 , SUsr5= @susr4  where SERIALKEY = @serialkey "
                        Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                        conn.Open()
                        Dim cmd As SqlCommand = New SqlCommand(updatequery, conn)
                        cmd.Parameters.AddWithValue("@price", price)
                        cmd.Parameters.AddWithValue("@susr1", udf1)
                        cmd.Parameters.AddWithValue("@susr2", udf2)
                        cmd.Parameters.AddWithValue("@susr3", udf3)
                        cmd.Parameters.AddWithValue("@susr4", udf4)
                        cmd.Parameters.AddWithValue("@susr5", udf5)
                        cmd.Parameters.AddWithValue("@serialkey", MyID)
                        cmd.ExecuteNonQuery()
                        conn.Close()
                    Else
                        Dim updatequery As String = "set dateformat dmy update SYSTEM.SKUCATALOGUE set PRICE =:price , SUsr1=:susr1 ,SUsr2= :susr2 , SUsr3= :susr3, SUsr4= :susr4 , SUsr5= :susr4  where SERIALKEY = :serialkey "
                        Dim conn As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                        conn.Open()
                        Dim cmd As OracleCommand = New OracleCommand(updatequery, conn)
                        cmd.Parameters.Add(New OracleParameter("price", price))
                        cmd.Parameters.Add(New OracleParameter("susr1", udf1))
                        cmd.Parameters.Add(New OracleParameter("susr2", udf2))
                        cmd.Parameters.Add(New OracleParameter("susr3", udf3))
                        cmd.Parameters.Add(New OracleParameter("susr4", udf4))
                        cmd.Parameters.Add(New OracleParameter("susr5", udf5))
                        cmd.Parameters.Add(New OracleParameter("serialkey", MyID))
                        cmd.ExecuteNonQuery()
                        conn.Close()
                    End If
                Catch e1 As Exception
                    tmp += "Error: " & e1.Message & vbTab + e1.GetType.ToString & "<br/>"
                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                    logger.Error(e1, "", "")
                End Try
            Else
                Dim exist As Integer = CommonMethods.checkItemExist(storer, consignee, sku, currency)
                If exist = 0 Then
                    Try
                        If CommonMethods.dbtype = "sql" Then
                            Dim insert As String = "set dateformat dmy insert into dbo.SKUCATALOGUE  (StorerKey, ConsigneeKey, Sku, Price, Currency, SUsr1, SUsr2, SUsr3 , SUsr4,SUsr5) values (@storer, @consignee, @sku, @price, @currency, @susr1, @susr2, @susr3, @susr4, @susr5);"
                            Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                            conn.Open()

                            Dim cmd As SqlCommand = New SqlCommand(insert, conn)
                            cmd.Parameters.AddWithValue("@storer", storer)
                            cmd.Parameters.AddWithValue("@consignee", consignee)
                            cmd.Parameters.AddWithValue("@sku", sku)
                            cmd.Parameters.AddWithValue("@price", price)
                            cmd.Parameters.AddWithValue("@currency", currency)
                            cmd.Parameters.AddWithValue("@susr1", udf1)
                            cmd.Parameters.AddWithValue("@susr2", udf2)
                            cmd.Parameters.AddWithValue("@susr3", udf3)
                            cmd.Parameters.AddWithValue("@susr4", udf4)
                            cmd.Parameters.AddWithValue("@susr5", udf5)
                            cmd.ExecuteNonQuery()

                            conn.Close()
                        Else
                            Dim insert As String = "set dateformat dmy insert into SYSTEM.SKUCATALOGUE (StorerKey, ConsigneeKey, Sku, Price, Currency, SUsr1, SUsr2, SUsr3 , SUsr4,SUsr5) values (:storer, :consignee, :sku, :price, :currency, :susr1, :susr2, :susr3, :susr4, :susr5);"
                            Dim conn As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                            conn.Open()

                            Dim cmd As OracleCommand = New OracleCommand(insert, conn)
                            cmd.Parameters.Add(New OracleParameter("storer", storer))
                            cmd.Parameters.Add(New OracleParameter("consignee", consignee))
                            cmd.Parameters.Add(New OracleParameter("sku", sku))
                            cmd.Parameters.Add(New OracleParameter("price", price))
                            cmd.Parameters.Add(New OracleParameter("currency", currency))
                            cmd.Parameters.Add(New OracleParameter("susr1", udf1))
                            cmd.Parameters.Add(New OracleParameter("susr2", udf2))
                            cmd.Parameters.Add(New OracleParameter("susr3", udf3))
                            cmd.Parameters.Add(New OracleParameter("susr4", udf4))
                            cmd.Parameters.Add(New OracleParameter("susr5", udf5))
                            cmd.ExecuteNonQuery()

                            conn.Close()
                        End If
                    Catch e2 As Exception
                        tmp += "Error: " & e2.Message & vbTab + e2.GetType.ToString & "<br/>"
                        Dim logger As Logger = LogManager.GetCurrentClassLogger()
                        logger.Error(e2, "", "")
                    End Try
                Else
                    tmp += "Error: this item was already defined for this Currency <br/>"
                End If
            End If
        End If
        If tmp = "" Then
            If Not EditOperation Then
                Dim sql = "Select top 1 SerialKey from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "SKUCATALOGUE order by SerialKey DESC "
                Dim ds As DataSet = (New SQLExec).Cursor(sql)
                If ds.Tables(0).Rows.Count > 0 Then serialkey = ds.Tables(0).Rows(0)!SerialKey
            Else
                serialkey = MyID
            End If
        End If
        queryurl = "?item=" & serialkey
        MyTable.Rows.Add(tmp, serialkey, queryurl)
        Return MyTable
    End Function
    Private Function SavePurchaseOrder(ByVal MyID As Integer) As DataTable
        Dim MyTable As New DataTable
        MyTable.Columns.Add("tmp", GetType(String))
        MyTable.Columns.Add("key", GetType(String))
        MyTable.Columns.Add("serialkey", GetType(String))
        MyTable.Columns.Add("queryurl", GetType(String))

        Dim tmp As String = "", Command As String = "", key As String = "", serialkey As String = "", queryurl As String = ""
        Dim IsValid As Boolean = True
        Dim EditOperation As Boolean = MyID <> 0
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Field_Facility")) _
        , Owner As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
        , Buyer As String = HttpContext.Current.Request.Item("Field_BuyerName") _
        , Buyerref As String = HttpContext.Current.Request.Item("Field_BuyersReference") _
        , Sellerref As String = HttpContext.Current.Request.Item("Field_SellersReference") _
        , supplier As String = UCase(HttpContext.Current.Request.Item("Field_SellerName")) _
        , pokey As String = HttpContext.Current.Request.Item("Field_POKey") _
        , externpo As String = HttpContext.Current.Request.Item("Field_ExternPOKey") _
        , podate As String = HttpContext.Current.Request.Item("Field_PODate") _
        , effectDate As String = HttpContext.Current.Request.Item("Field_EffectiveDate") _
        , type As String = HttpContext.Current.Request.Item("Field_POType") _
        , UDF1 As String = HttpContext.Current.Request.Item("Field_SUsr1") _
        , UDF2 As String = HttpContext.Current.Request.Item("Field_SUsr2") _
        , UDF3 As String = HttpContext.Current.Request.Item("Field_SUsr3") _
        , UDF4 As String = HttpContext.Current.Request.Item("Field_SUsr4") _
        , UDF5 As String = HttpContext.Current.Request.Item("Field_SUsr5") _
        , exterline As String = HttpContext.Current.Request.Item("DetailsField_ExternLineNo") _
        , Sku As String = UCase(HttpContext.Current.Request.Item("DetailsField_Sku")) _
        , QtyOrdered As String = HttpContext.Current.Request.Item("DetailsField_QtyOrdered")

        If String.IsNullOrEmpty(Facility) Then
            IsValid = False
            tmp += "Facility must be defined <br/>"
        End If

        If Not EditOperation Then
            If String.IsNullOrEmpty(externpo) And Not String.IsNullOrEmpty(Facility) Then
                externpo = CommonMethods.getExternKey(Facility, "EXTERNPO")
            End If
            Command += "<ExternPOKey>" & externpo & "</ExternPOKey><Status>0</Status>"
        Else
            Command += "<ExternPOKey>" & externpo & "</ExternPOKey><POKey>" & pokey & "</POKey>"
        End If

        If Not String.IsNullOrEmpty(type) Then
            Dim DTPOType = CommonMethods.getCodeDD(Facility, "codelkup", "POTYPE")
            Dim DRPOType() As DataRow = DTPOType.Select("DESCRIPTION='" & type & "'")
            If DRPOType.Length > 0 Then
                Command += "<POType>" & DRPOType(0)!CODE & "</POType>"
            Else
                IsValid = False
                tmp += "Type must be defined <br/>"
            End If
        Else
            IsValid = False
            tmp += "Type must be defined <br/>"
        End If

        If Not String.IsNullOrEmpty(Owner) And Trim(Owner) <> "" Then
            Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
            If owners.Any(Function(x) x = Owner) Or owners.Any(Function(x) x = "ALL") Then
                Command += "<StorerKey>" & Owner & "</StorerKey>"
            Else
                IsValid = False
                tmp += "This owner is not authorized <br/>"
            End If
        Else
            IsValid = False
            tmp += "Owner must be defined <br/>"
        End If

        If Not String.IsNullOrEmpty(Buyer) Then Command += "<BuyerName>" & Buyer & "</BuyerName>"
        If Not String.IsNullOrEmpty(Buyerref) Then Command += "<BuyersReference>" & Buyerref & "</BuyersReference>"

        If Not String.IsNullOrEmpty(supplier) And Trim(supplier) <> "" Then
            Dim suppliers As String() = CommonMethods.getSupplierPerUser(HttpContext.Current.Session("userkey").ToString)
            If suppliers.Any(Function(x) x = supplier) Or suppliers.Any(Function(x) x = "ALL") Then
                Command += "<SellerName>" & supplier & "</SellerName>"
            Else
                IsValid = False
                tmp += "This supplier is not authorized <br/>"
            End If
        End If

        If Not String.IsNullOrEmpty(Sellerref) Then Command += "<SellersReference>" & Sellerref & "</SellersReference>"

        If Not String.IsNullOrEmpty(effectDate) Then
            Dim datetime As DateTime
            If DateTime.TryParseExact(effectDate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                Dim datetime2 As DateTime = DateTime.ParseExact(effectDate, "MM/dd/yyyy", Nothing)
                If Not String.IsNullOrEmpty(CommonMethods.dformat) Then
                    Command += "<EffectiveDate>" & datetime2.ToString(CommonMethods.dformat & " 14:00:00") & " </EffectiveDate>"
                ElseIf Double.Parse(CommonMethods.version) >= 11 Then
                    Command += "<EffectiveDate>" & datetime2.ToString("MM/dd/yyyy 14:00:00") & " </EffectiveDate>"
                Else
                    Command += "<EffectiveDate>" & datetime2.ToString("dd/MM/yyyy 14:00:00") & " </EffectiveDate>"
                End If
            Else
                IsValid = False
                tmp += "This Effective Date doesn't match the required date format <br/>"
            End If
        End If

        If Not String.IsNullOrEmpty(UDF1) Then Command += "<SUsr1>" & UDF1 & "</SUsr1>"
        If Not String.IsNullOrEmpty(UDF2) Then Command += "<SUsr2>" & UDF2 & "</SUsr2>"
        If Not String.IsNullOrEmpty(UDF3) Then Command += "<SUsr3>" & UDF3 & "</SUsr3>"
        If Not String.IsNullOrEmpty(UDF4) Then Command += "<SUsr4>" & UDF4 & "</SUsr4>"
        If Not String.IsNullOrEmpty(UDF5) Then Command += "<SUsr5>" & UDF5 & "</SUsr5>"

        If IsValid Then
            If Not EditOperation Then
                Dim exist As Integer = CommonMethods.checkPOExist(Facility, externpo)
                If exist = 0 Then
                    If String.IsNullOrEmpty(exterline) And String.IsNullOrEmpty(Sku) And String.IsNullOrEmpty(QtyOrdered) Then
                        tmp = "Detail line cannot be empty <br/>"
                        MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                        Return MyTable
                    End If

                    Command += "<PurchaseOrderDetail>"

                    If String.IsNullOrEmpty(exterline) Then exterline = "1"
                    exterline = exterline.ToString.PadLeft(5, "0")
                    Command += "<ExternLineNo>" & exterline & "</ExternLineNo>"

                    If Not String.IsNullOrEmpty(Sku) Then
                        Command += "<StorerKey>" & Owner & "</StorerKey><Sku>" & Sku & "</Sku>"
                    Else
                        tmp = "Item cannot be empty <br/>"
                        MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                        Return MyTable
                    End If

                    If Not String.IsNullOrEmpty(QtyOrdered) Then
                        Command += "<QtyOrdered>" & QtyOrdered & "</QtyOrdered>"
                    Else
                        tmp = "Qty Ordered cannot be empty <br/>"
                        MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                        Return MyTable
                    End If

                    Command += "</PurchaseOrderDetail>"
                Else
                    tmp = "Error: Extern PO Key already exists!"
                    MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                    Return MyTable
                End If
            End If
            Try
                Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>PurchaseOrder</MessageType><Action>store</Action><Sender><User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId>	</Sender><Recipient><SystemID>" & Facility & "</SystemID></Recipient></Head><Body><PurchaseOrder><PurchaseOrderHeader>" & Command & "</PurchaseOrderHeader></PurchaseOrder></Body></Message>"

                Dim soapResult As String = CommonMethods.sendwebRequest(Xml)

                If String.IsNullOrEmpty(soapResult) Then
                    tmp = "Error: Unable to connect to webservice, kindly check the logs"
                Else
                    Dim dsresult As DataSet = New DataSet
                    Dim doc As XmlDocument = New XmlDocument
                    doc.LoadXml(soapResult)
                    Dim xmlFile As XmlReader = XmlReader.Create(New StringReader(soapResult), New XmlReaderSettings)
                    If LCase(soapResult).Contains("error") Then
                        Dim nodeList As XmlNodeList
                        If soapResult.Contains("ERROR") Then
                            nodeList = doc.GetElementsByTagName("Error")
                        Else
                            nodeList = doc.GetElementsByTagName("string")
                        End If
                        Dim message As String = ""
                        For Each node As XmlNode In nodeList
                            message = node.InnerText
                        Next
                        message = Regex.Replace(message, "&.*?;", "")
                        tmp = "Error: " & message & "<br/>"
                        Dim logger As Logger = LogManager.GetCurrentClassLogger()
                        logger.Error(message, "", "")
                    Else
                        tmp = CommonMethods.incremenetKey(Facility, "EXTERNPO")
                        If tmp = "" Then
                            Dim po As String = "", lineno As String = "", logmessage As String = ""

                            Dim nodeList As XmlNodeList = doc.GetElementsByTagName("POKey")
                            For Each node As XmlNode In nodeList
                                po = node.InnerText
                                key = po
                                queryurl = "?warehouse=" & Facility & "&po=" & po
                                logmessage = CommonMethods.logger(Facility, "Po", po, HttpContext.Current.Session("userkey").ToString)
                            Next


                            nodeList = doc.GetElementsByTagName("SerialKey")
                            For Each node As XmlNode In nodeList
                                serialkey = node.InnerText
                                Exit For
                            Next

                            If Not EditOperation Then
                                lineno = "00001"
                                logmessage += CommonMethods.logger(Facility, "PoDetail", po & "-" & lineno, HttpContext.Current.Session("userkey").ToString)
                            End If

                            If Not String.IsNullOrEmpty(logmessage) Then
                                tmp = "Logging Error: " + logmessage + "<br />"
                                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                                logger.Error(logmessage, "", "")
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                tmp = "Error: " & ex.Message & vbTab + ex.GetType.ToString & "<br/>"
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(ex, "", "")
            End Try
        End If

        MyTable.Rows.Add(tmp, key, serialkey, queryurl)

        Return MyTable
    End Function
    Private Function SaveASN(ByVal MyID As Integer) As DataTable
        Dim MyTable As New DataTable
        MyTable.Columns.Add("tmp", GetType(String))
        MyTable.Columns.Add("key", GetType(String))
        MyTable.Columns.Add("serialkey", GetType(String))
        MyTable.Columns.Add("queryurl", GetType(String))

        Dim tmp As String = "", Command As String = "", key As String = "", serialkey As String = "", queryurl As String = ""
        Dim IsValid As Boolean = True
        Dim EditOperation As Boolean = MyID <> 0
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Field_Facility")) _
        , externreceipt As String = HttpContext.Current.Request.Item("Field_ExternReceiptKey") _
        , receiptkey As String = HttpContext.Current.Request.Item("Field_ReceiptKey") _
        , Owner As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
        , CarrierKey As String = HttpContext.Current.Request.Item("Field_CarrierKey") _
        , WarehouseReference As String = HttpContext.Current.Request.Item("Field_WarehouseReference") _
        , pokey As String = HttpContext.Current.Request.Item("Field_POKey") _
        , type As String = HttpContext.Current.Request.Item("Field_ReceiptType") _
        , receiptdate As String = HttpContext.Current.Request.Item("Field_ReceiptDate") _
        , ContainerKey As String = HttpContext.Current.Request.Item("Field_ContainerKey") _
        , ContainerType As String = HttpContext.Current.Request.Item("Field_ContainerType") _
        , OriginCountry As String = HttpContext.Current.Request.Item("Field_OriginCountry") _
        , TransportationMode As String = HttpContext.Current.Request.Item("Field_TransportationMode") _
        , exterline As String = HttpContext.Current.Request.Item("DetailsField_ExternLineNo") _
        , Sku As String = UCase(HttpContext.Current.Request.Item("DetailsField_Sku")) _
        , QtyExpected As String = HttpContext.Current.Request.Item("DetailsField_QtyExpected") _
        , QtyReceived As String = HttpContext.Current.Request.Item("DetailsField_QtyReceived") _
        , PackKey As String = HttpContext.Current.Request.Item("DetailsField_PackKey") _
        , UOM As String = HttpContext.Current.Request.Item("DetailsField_UOM") _
        , POKeyDtl As String = HttpContext.Current.Request.Item("DetailsField_POKey") _
        , ToId As String = HttpContext.Current.Request.Item("DetailsField_ToId") _
        , ToLoc As String = HttpContext.Current.Request.Item("DetailsField_ToLoc") _
        , ConditionCode As String = HttpContext.Current.Request.Item("DetailsField_ConditionCode") _
        , TariffKey As String = HttpContext.Current.Request.Item("DetailsField_TariffKey") _
        , Lottable01 As String = HttpContext.Current.Request.Item("DetailsField_Lottable01") _
        , Lottable02 As String = HttpContext.Current.Request.Item("DetailsField_Lottable02") _
        , Lottable03 As String = HttpContext.Current.Request.Item("DetailsField_Lottable03") _
        , Lottable04 As String = HttpContext.Current.Request.Item("DetailsField_Lottable04") _
        , Lottable05 As String = HttpContext.Current.Request.Item("DetailsField_Lottable05") _
        , Lottable06 As String = HttpContext.Current.Request.Item("DetailsField_Lottable06") _
        , Lottable07 As String = HttpContext.Current.Request.Item("DetailsField_Lottable07") _
        , Lottable08 As String = HttpContext.Current.Request.Item("DetailsField_Lottable08") _
        , Lottable09 As String = HttpContext.Current.Request.Item("DetailsField_Lottable09") _
        , Lottable10 As String = HttpContext.Current.Request.Item("DetailsField_Lottable10") _
        , Lottable11 As String = HttpContext.Current.Request.Item("DetailsField_Lottable11") _
        , Lottable12 As String = HttpContext.Current.Request.Item("DetailsField_Lottable12")

        If String.IsNullOrEmpty(Facility) Then
            IsValid = False
            tmp += "Facility must be defined <br/>"
        End If

        If Not EditOperation Then
            If String.IsNullOrEmpty(externreceipt) And Not String.IsNullOrEmpty(Facility) Then
                externreceipt = CommonMethods.getExternKey(Facility, "EXTERNASN")
            End If
            Command += "<ExternReceiptKey>" & externreceipt & "</ExternReceiptKey><Status>0</Status>"
        Else
            Command += "<ExternReceiptKey>" & externreceipt & "</ExternReceiptKey><ReceiptKey>" & receiptkey & "</ReceiptKey>"
        End If

        If Not String.IsNullOrEmpty(type) Then
            Dim DTReceiptType = CommonMethods.getCodeDD(Facility, "codelkup", "RECEIPTYPE")
            Dim DRReceiptType() As DataRow = DTReceiptType.Select("DESCRIPTION='" & type & "'")
            If DRReceiptType.Length > 0 Then
                Command += "<ReceiptType>" & DRReceiptType(0)!CODE & "</ReceiptType>"
            Else
                IsValid = False
                tmp += "Type must be defined <br/>"
            End If
        Else
            IsValid = False
            tmp += "Type must be defined <br/>"
        End If

        If Not String.IsNullOrEmpty(Owner) And Trim(Owner) <> "" Then
            Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
            If owners.Any(Function(x) x = Owner) Or owners.Any(Function(x) x = "ALL") Then
                Command += "<StorerKey>" & Owner & "</StorerKey>"
            Else
                IsValid = False
                tmp += "This owner is not authorized <br/>"
            End If
        Else
            IsValid = False
            tmp += "Owner must be defined <br/>"
        End If

        If Not String.IsNullOrEmpty(pokey) Then Command += "<POKey>" & pokey & "</POKey>"
        If Not String.IsNullOrEmpty(CarrierKey) Then Command += "<CarrierKey>" & CarrierKey & "</CarrierKey>"
        If Not String.IsNullOrEmpty(WarehouseReference) Then Command += "<WarehouseReference>" & WarehouseReference & "</WarehouseReference>"
        If Not String.IsNullOrEmpty(ContainerKey) Then Command += "<ContainerKey>" & ContainerKey & "</ContainerKey>"
        If Not String.IsNullOrEmpty(ContainerType) Then Command += "<ContainerType>" & ContainerType & "</ContainerType>"
        If Not String.IsNullOrEmpty(OriginCountry) Then Command += "<OriginCountry>" & CommonMethods.getISOCountryCode(OriginCountry) & "</OriginCountry>"
        If Not String.IsNullOrEmpty(TransportationMode) Then Command += "<TransportationMode>" & TransportationMode & "</TransportationMode>"

        If IsValid Then
            If Not EditOperation Then
                Dim exist As Integer = CommonMethods.checkASNExist(Facility, externreceipt)
                If exist = 0 Then
                    If String.IsNullOrEmpty(exterline) And String.IsNullOrEmpty(Sku) And String.IsNullOrEmpty(QtyExpected) Then
                        tmp = "Detail line cannot be empty <br/>"
                        MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                        Return MyTable
                    End If

                    Command += "<AdvancedShipNoticeDetail>"

                    If String.IsNullOrEmpty(exterline) Then exterline = "1"
                    exterline = exterline.ToString.PadLeft(5, "0")
                    Command += "<ExternLineNo>" & exterline & "</ExternLineNo>"

                    If Not String.IsNullOrEmpty(Sku) Then
                        Command += "<StorerKey>" & Owner & "</StorerKey><Sku>" & Sku & "</Sku>"
                    Else
                        tmp = "Item cannot be empty <br/>"
                        MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                        Return MyTable
                    End If

                    If Not String.IsNullOrEmpty(QtyExpected) Then
                        Command += "<QtyExpected>" & QtyExpected & "</QtyExpected>"
                    Else
                        tmp = "Qty Expected cannot be empty <br/>"
                        MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                        Return MyTable
                    End If

                    If Not String.IsNullOrEmpty(QtyReceived) Then Command += "<QtyReceived>" & QtyReceived & "</QtyReceived>"
                    If Not String.IsNullOrEmpty(PackKey) Then Command += "<PackKey>" & PackKey & "</PackKey>"
                    If Not String.IsNullOrEmpty(UOM) Then Command += "<UOM>" & UOM & "</UOM>"
                    If Not String.IsNullOrEmpty(POKeyDtl) Then Command += "<POKey>" & POKeyDtl & "</POKey>"
                    If Not String.IsNullOrEmpty(ToId) Then Command += "<ToId>" & ToId & "</ToId>"
                    If Not String.IsNullOrEmpty(ToLoc) Then Command += "<ToLoc>" & ToLoc & "</ToLoc>"
                    If Not String.IsNullOrEmpty(ConditionCode) Then Command += "<ConditionCode>" & ConditionCode & "</ConditionCode>"
                    If Not String.IsNullOrEmpty(TariffKey) Then Command += "<TariffKey>" & TariffKey & "</TariffKey>"
                    If Not String.IsNullOrEmpty(Lottable01) Then Command += "<Lottable01>" & Lottable01 & "</Lottable01>"
                    If Not String.IsNullOrEmpty(Lottable02) Then Command += "<Lottable02>" & Lottable02 & "</Lottable02>"
                    If Not String.IsNullOrEmpty(Lottable03) Then Command += "<Lottable03>" & Lottable03 & "</Lottable03>"

                    Try
                        If Not String.IsNullOrEmpty(Lottable04) Then
                            Dim datetime As DateTime
                            If DateTime.TryParseExact(Lottable04, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                                Dim datetime2 As DateTime = DateTime.ParseExact(Lottable04, "MM/dd/yyyy", Nothing)
                                If Not String.IsNullOrEmpty(CommonMethods.dformat) Then
                                    Command += "<Lottable04>" & datetime2.ToString(CommonMethods.dformat & " 14:00:00") & " </Lottable04>"
                                ElseIf Double.Parse(CommonMethods.version) >= 11 Then
                                    Command += "<Lottable04>" & datetime2.ToString("MM/dd/yyyy 14:00:00") & " </Lottable04>"
                                Else
                                    Command += "<Lottable04>" & datetime2.ToString("dd/MM/yyyy 14:00:00") & " </Lottable04>"
                                End If
                            Else
                                tmp = "Lottable04 doesn't match the required date format <br/>"
                                MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                                Return MyTable
                            End If
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable05) Then
                            Dim datetime As DateTime
                            If DateTime.TryParseExact(Lottable05, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                                Dim datetime2 As DateTime = DateTime.ParseExact(Lottable05, "MM/dd/yyyy", Nothing)
                                If Not String.IsNullOrEmpty(CommonMethods.dformat) Then
                                    Command += "<Lottable05>" & datetime2.ToString(CommonMethods.dformat & " 14:00:00") & " </Lottable05>"
                                ElseIf Double.Parse(CommonMethods.version) >= 11 Then
                                    Command += "<Lottable05>" & datetime2.ToString("MM/dd/yyyy 14:00:00") & " </Lottable05>"
                                Else
                                    Command += "<Lottable05>" & datetime2.ToString("dd/MM/yyyy 14:00:00") & " </Lottable05>"
                                End If
                            Else
                                tmp = "Lottable05 doesn't match the required date format one line <br/>"
                                MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                                Return MyTable
                            End If
                        End If
                    Catch ex As Exception
                    End Try

                    If Not String.IsNullOrEmpty(Lottable06) Then Command += "<Lottable06>" & Lottable06 & "</Lottable06>"
                    If Not String.IsNullOrEmpty(Lottable07) Then Command += "<Lottable07>" & Lottable07 & "</Lottable07>"
                    If Not String.IsNullOrEmpty(Lottable08) Then Command += "<Lottable08>" & Lottable08 & "</Lottable08>"
                    If Not String.IsNullOrEmpty(Lottable09) Then Command += "<Lottable09>" & Lottable09 & "</Lottable09>"
                    If Not String.IsNullOrEmpty(Lottable10) Then Command += "<Lottable10>" & Lottable10 & "</Lottable10>"

                    Try
                        If Not String.IsNullOrEmpty(Lottable11) Then
                            Dim datetime As DateTime
                            If DateTime.TryParseExact(Lottable11, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                                Dim datetime2 As DateTime = DateTime.ParseExact(Lottable11, "MM/dd/yyyy", Nothing)
                                If Not String.IsNullOrEmpty(CommonMethods.dformat) Then
                                    Command += "<Lottable11>" & datetime2.ToString(CommonMethods.dformat & " 14:00:00") & " </Lottable11>"
                                ElseIf Double.Parse(CommonMethods.version) >= 11 Then
                                    Command += "<Lottable11>" & datetime2.ToString("MM/dd/yyyy 14:00:00") & " </Lottable11>"
                                Else
                                    Command += "<Lottable11>" & datetime2.ToString("dd/MM/yyyy 14:00:00") & " </Lottable11>"
                                End If
                            Else
                                tmp = "Lottable11 doesn't match the required date format one line <br/>"
                                MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                                Return MyTable
                            End If
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable12) Then
                            Dim datetime As DateTime
                            If DateTime.TryParseExact(Lottable12, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                                Dim datetime2 As DateTime = DateTime.ParseExact(Lottable12, "MM/dd/yyyy", Nothing)
                                If Not String.IsNullOrEmpty(CommonMethods.dformat) Then
                                    Command += "<Lottable12>" & datetime2.ToString(CommonMethods.dformat & " 14:00:00") & " </Lottable12>"
                                ElseIf Double.Parse(CommonMethods.version) >= 11 Then
                                    Command += "<Lottable12>" & datetime2.ToString("MM/dd/yyyy 14:00:00") & " </Lottable12>"
                                Else
                                    Command += "<Lottable12>" & datetime2.ToString("dd/MM/yyyy 14:00:00") & " </Lottable12>"
                                End If
                            Else
                                tmp = "Lottable12 doesn't match the required date format one line <br/>"
                                MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                                Return MyTable
                            End If
                        End If
                    Catch ex As Exception
                    End Try

                    Command += "</AdvancedShipNoticeDetail>"
                Else
                    tmp = "Error: Extern Receipt Key already exists!"
                    MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                    Return MyTable
                End If
            End If
            Try
                Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>AdvancedShipNotice</MessageType><Action>store</Action><Sender><User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId>	</Sender><Recipient><SystemID>" & Facility & "</SystemID></Recipient></Head><Body><AdvancedShipNotice><AdvancedShipNoticeHeader>" & Command & "</AdvancedShipNoticeHeader></AdvancedShipNotice></Body></Message>"
                Dim soapResult As String = CommonMethods.sendwebRequest(Xml)

                If String.IsNullOrEmpty(soapResult) Then
                    tmp = "Error: Unable to connect to webservice, kindly check the logs"
                Else
                    Dim dsresult As DataSet = New DataSet
                    Dim doc As XmlDocument = New XmlDocument
                    doc.LoadXml(soapResult)
                    Dim xmlFile As XmlReader = XmlReader.Create(New StringReader(soapResult), New XmlReaderSettings)
                    If LCase(soapResult).Contains("error") Then
                        Dim nodeList As XmlNodeList
                        If soapResult.Contains("ERROR") Then
                            nodeList = doc.GetElementsByTagName("Error")
                        Else
                            nodeList = doc.GetElementsByTagName("string")
                        End If
                        Dim message As String = ""
                        For Each node As XmlNode In nodeList
                            message = node.InnerText
                        Next
                        message = Regex.Replace(message, "&.*?;", "")
                        tmp = "Error: " & message & "<br/>"
                        Dim logger As Logger = LogManager.GetCurrentClassLogger()
                        logger.Error(message, "", "")
                    Else
                        tmp = CommonMethods.incremenetKey(Facility, "EXTERNASN")
                        If tmp = "" Then
                            Dim receipt As String = "", logmessage As String = ""

                            Dim nodeList As XmlNodeList = doc.GetElementsByTagName("ReceiptKey")
                            For Each node As XmlNode In nodeList
                                receipt = node.InnerText
                                key = receipt
                                queryurl = "?warehouse=" & Facility & "&receipt=" & receipt
                                logmessage = CommonMethods.logger(Facility, "Receipt", receipt, HttpContext.Current.Session("userkey").ToString)
                            Next

                            nodeList = doc.GetElementsByTagName("SerialKey")
                            For Each node As XmlNode In nodeList
                                serialkey = node.InnerText
                                Exit For
                            Next

                            If Not EditOperation Then
                                Dim nodeList2 As XmlNodeList = doc.GetElementsByTagName("ReceiptLineNumber")
                                Dim lineno As String = ""
                                For Each node As XmlNode In nodeList2
                                    lineno = node.InnerText
                                    logmessage += CommonMethods.logger(Facility, "ReceiptDetail", receipt & "-" & lineno, HttpContext.Current.Session("userkey").ToString)
                                Next
                            End If

                            If Not String.IsNullOrEmpty(logmessage) Then
                                tmp = "Logging Error: " + logmessage + "<br />"
                                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                                logger.Error(logmessage, "", "")
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                tmp = "Error: " & ex.Message & vbTab + ex.GetType.ToString & "<br/>"
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(ex, "", "")
            End Try
        End If

        MyTable.Rows.Add(tmp, key, serialkey, queryurl)

        Return MyTable
    End Function
    Private Function SaveSO(ByVal MyID As Integer) As DataTable
        Dim MyTable As New DataTable
        MyTable.Columns.Add("tmp", GetType(String))
        MyTable.Columns.Add("key", GetType(String))
        MyTable.Columns.Add("serialkey", GetType(String))
        MyTable.Columns.Add("queryurl", GetType(String))

        Dim tmp As String = "", Command As String = "", key As String = "", serialkey As String = "", queryurl As String = ""
        Dim IsValid As Boolean = True
        Dim EditOperation As Boolean = MyID <> 0
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Field_Facility")) _
        , externorder As String = HttpContext.Current.Request.Item("Field_ExternOrderKey") _
        , orderkey As String = HttpContext.Current.Request.Item("Field_OrderKey") _
        , Owner As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
        , ConsigneeKey As String = UCase(HttpContext.Current.Request.Item("Field_ConsigneeKey")) _
        , type As String = HttpContext.Current.Request.Item("Field_OrderType") _
        , orderdate As String = HttpContext.Current.Request.Item("Field_OrderDate") _
        , requestedshipdate As String = HttpContext.Current.Request.Item("Field_RequestedShipDate") _
        , actualshipdate As String = HttpContext.Current.Request.Item("Field_ActualShipDate") _
        , UDF1 As String = HttpContext.Current.Request.Item("Field_SUsr1") _
        , UDF2 As String = HttpContext.Current.Request.Item("Field_SUsr2") _
        , UDF3 As String = HttpContext.Current.Request.Item("Field_SUsr3") _
        , UDF4 As String = HttpContext.Current.Request.Item("Field_SUsr4") _
        , UDF5 As String = HttpContext.Current.Request.Item("Field_SUsr5") _
        , exterline As String = HttpContext.Current.Request.Item("DetailsField_ExternLineNo") _
        , Sku As String = UCase(HttpContext.Current.Request.Item("DetailsField_Sku")) _
        , OpenQty As String = HttpContext.Current.Request.Item("DetailsField_OpenQty") _
        , PackKey As String = HttpContext.Current.Request.Item("DetailsField_PackKey") _
        , UOM As String = HttpContext.Current.Request.Item("DetailsField_UOM") _
        , UDF1Dtl As String = HttpContext.Current.Request.Item("DetailsField_SUsr1") _
        , UDF2Dtl As String = HttpContext.Current.Request.Item("DetailsField_SUsr2") _
        , UDF3Dtl As String = HttpContext.Current.Request.Item("DetailsField_SUsr3") _
        , UDF4Dtl As String = HttpContext.Current.Request.Item("DetailsField_SUsr4") _
        , UDF5Dtl As String = HttpContext.Current.Request.Item("DetailsField_SUsr5") _
        , Lottable01 As String = HttpContext.Current.Request.Item("DetailsField_Lottable01") _
        , Lottable02 As String = HttpContext.Current.Request.Item("DetailsField_Lottable02") _
        , Lottable03 As String = HttpContext.Current.Request.Item("DetailsField_Lottable03") _
        , Lottable04 As String = HttpContext.Current.Request.Item("DetailsField_Lottable04") _
        , Lottable05 As String = HttpContext.Current.Request.Item("DetailsField_Lottable05") _
        , Lottable06 As String = HttpContext.Current.Request.Item("DetailsField_Lottable06") _
        , Lottable07 As String = HttpContext.Current.Request.Item("DetailsField_Lottable07") _
        , Lottable08 As String = HttpContext.Current.Request.Item("DetailsField_Lottable08") _
        , Lottable09 As String = HttpContext.Current.Request.Item("DetailsField_Lottable09") _
        , Lottable10 As String = HttpContext.Current.Request.Item("DetailsField_Lottable10")

        If String.IsNullOrEmpty(Facility) Then
            IsValid = False
            tmp += "Facility must be defined <br/>"
        End If

        If Not EditOperation Then
            If String.IsNullOrEmpty(externorder) And Not String.IsNullOrEmpty(Facility) Then
                externorder = CommonMethods.getExternKey(Facility, "EXTERNSO")
            End If
            Command += "<ExternOrderKey>" & externorder & "</ExternOrderKey>"
        Else
            Command += "<ExternOrderKey>" & externorder & "</ExternOrderKey><OrderKey>" & orderkey & "</OrderKey>"
        End If

        If Not String.IsNullOrEmpty(type) Then
            Dim DTOrderType = CommonMethods.getCodeDD(Facility, "codelkup", "ORDERTYPE")
            Dim DROrderType() As DataRow = DTOrderType.Select("DESCRIPTION='" & type & "'")
            If DROrderType.Length > 0 Then
                Command += "<OrderType>" & DROrderType(0)!CODE & "</OrderType>"
            Else
                IsValid = False
                tmp += "Type must be defined <br/>"
            End If
        Else
            IsValid = False
            tmp += "Type must be defined <br/>"
        End If

        If Not String.IsNullOrEmpty(Owner) And Trim(Owner) <> "" Then
            Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
            If owners.Any(Function(x) x = Owner) Or owners.Any(Function(x) x = "ALL") Then
                Command += "<StorerKey>" & Owner & "</StorerKey>"
            Else
                IsValid = False
                tmp += "This owner is not authorized <br/>"
            End If
        Else
            IsValid = False
            tmp += "Owner must be defined <br/>"
        End If

        If Not String.IsNullOrEmpty(ConsigneeKey) And Trim(ConsigneeKey) <> "" Then
            Dim consignees As String() = CommonMethods.getConsigneePerUser(HttpContext.Current.Session("userkey").ToString)
            If consignees.Any(Function(x) x = ConsigneeKey) Or consignees.Any(Function(x) x = "ALL") Then
                Command += "<ConsigneeKey>" & ConsigneeKey & "</ConsigneeKey>"
            Else
                IsValid = False
                tmp += "This consignee is not authorized <br/>"
            End If
        End If

        If Not String.IsNullOrEmpty(requestedshipdate) Then
            Dim datetime As DateTime
            If DateTime.TryParseExact(requestedshipdate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                Dim datetime2 As DateTime = DateTime.ParseExact(requestedshipdate, "MM/dd/yyyy", Nothing)
                If Not String.IsNullOrEmpty(CommonMethods.dformat) Then
                    Command += "<RequestedShipDate>" & datetime2.ToString(CommonMethods.dformat & " 14:00:00") & " </RequestedShipDate>"
                Else
                    Command += "<RequestedShipDate>" & datetime2.ToString("dd/MM/yyyy 14:00:00") & " </RequestedShipDate>"
                End If
            Else
                IsValid = False
                tmp += "Requested ship date doesn't match the required date format <br/>"
            End If
        End If

        If EditOperation Then
            If Not String.IsNullOrEmpty(actualshipdate) Then
                Command += "<ActualShipDate>" & actualshipdate & " </ActualShipDate>"
            End If
        End If

        If Not String.IsNullOrEmpty(UDF1) Then Command += "<SUsr1>" & UDF1 & "</SUsr1>"
        If Not String.IsNullOrEmpty(UDF2) Then Command += "<SUsr2>" & UDF2 & "</SUsr2>"
        If Not String.IsNullOrEmpty(UDF3) Then Command += "<SUsr3>" & UDF3 & "</SUsr3>"
        If Not String.IsNullOrEmpty(UDF4) Then Command += "<SUsr4>" & UDF4 & "</SUsr4>"
        If Not String.IsNullOrEmpty(UDF5) Then Command += "<SUsr5>" & UDF5 & "</SUsr5>"

        If IsValid Then
            If Not EditOperation Then
                Dim exist As Integer = CommonMethods.checkSOExist(Facility, externorder)
                If exist = 0 Then
                    If String.IsNullOrEmpty(exterline) And String.IsNullOrEmpty(Sku) And String.IsNullOrEmpty(OpenQty) Then
                        tmp = "Detail line cannot be empty <br/>"
                        MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                        Return MyTable
                    End If

                    Command += "<ShipmentOrderDetail>"

                    If String.IsNullOrEmpty(exterline) Then exterline = "1"
                    exterline = exterline.ToString.PadLeft(5, "0")
                    Command += "<ExternLineNo>" & exterline & "</ExternLineNo>"

                    If Not String.IsNullOrEmpty(Sku) Then
                        Command += "<StorerKey>" & Owner & "</StorerKey><Sku>" & Sku & "</Sku>"
                    Else
                        tmp = "Item cannot be empty <br/>"
                        MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                        Return MyTable
                    End If

                    If Not String.IsNullOrEmpty(OpenQty) Then
                        Command += "<OpenQty>" & OpenQty & "</OpenQty>"
                    Else
                        tmp = "Open Qty cannot be empty <br/>"
                        MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                        Return MyTable
                    End If

                    If Not String.IsNullOrEmpty(PackKey) Then Command += "<PackKey>" & PackKey & "</PackKey>"
                    If Not String.IsNullOrEmpty(UOM) Then Command += "<UOM>" & UOM & "</UOM>"
                    If Not String.IsNullOrEmpty(UDF1Dtl) Then Command += "<SUsr1>" & UDF1Dtl & "</SUsr1>"
                    If Not String.IsNullOrEmpty(UDF2Dtl) Then Command += "<SUsr2>" & UDF2Dtl & "</SUsr2>"
                    If Not String.IsNullOrEmpty(UDF3Dtl) Then Command += "<SUsr3>" & UDF3Dtl & "</SUsr3>"
                    If Not String.IsNullOrEmpty(UDF4Dtl) Then Command += "<SUsr4>" & UDF4Dtl & "</SUsr4>"
                    If Not String.IsNullOrEmpty(UDF5Dtl) Then Command += "<SUsr4>" & UDF5Dtl & "</SUsr4>"
                    If Not String.IsNullOrEmpty(Lottable01) Then Command += "<Lottable01>" & Lottable01 & "</Lottable01>"
                    If Not String.IsNullOrEmpty(Lottable02) Then Command += "<Lottable02>" & Lottable02 & "</Lottable02>"
                    If Not String.IsNullOrEmpty(Lottable03) Then Command += "<Lottable03>" & Lottable03 & "</Lottable03>"

                    Try
                        If Not String.IsNullOrEmpty(Lottable04) Then
                            Dim datetime As DateTime
                            If DateTime.TryParseExact(Lottable04, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                                Dim datetime2 As DateTime = DateTime.ParseExact(Lottable04, "MM/dd/yyyy", Nothing)
                                If Not String.IsNullOrEmpty(CommonMethods.dformat) Then
                                    Command += "<Lottable04>" & datetime2.ToString(CommonMethods.dformat & " 14:00:00") & " </Lottable04>"
                                ElseIf Double.Parse(CommonMethods.version) >= 11 Then
                                    Command += "<Lottable04>" & datetime2.ToString("MM/dd/yyyy 14:00:00") & " </Lottable04>"
                                Else
                                    Command += "<Lottable04>" & datetime2.ToString("dd/MM/yyyy 14:00:00") & " </Lottable04>"
                                End If
                            Else
                                tmp = "Lottable04 doesn't match the required date format <br/>"
                                MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                                Return MyTable
                            End If
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable05) Then
                            Dim datetime As DateTime
                            If DateTime.TryParseExact(Lottable05, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                                Dim datetime2 As DateTime = DateTime.ParseExact(Lottable05, "MM/dd/yyyy", Nothing)
                                If Not String.IsNullOrEmpty(CommonMethods.dformat) Then
                                    Command += "<Lottable05>" & datetime2.ToString(CommonMethods.dformat & " 14:00:00") & " </Lottable05>"
                                ElseIf Double.Parse(CommonMethods.version) >= 11 Then
                                    Command += "<Lottable05>" & datetime2.ToString("MM/dd/yyyy 14:00:00") & " </Lottable05>"
                                Else
                                    Command += "<Lottable05>" & datetime2.ToString("dd/MM/yyyy 14:00:00") & " </Lottable05>"
                                End If
                            Else
                                tmp = "Lottable05 doesn't match the required date format one line <br/>"
                                MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                                Return MyTable
                            End If
                        End If
                    Catch ex As Exception
                    End Try

                    If Not String.IsNullOrEmpty(Lottable06) Then Command += "<Lottable06>" & Lottable06 & "</Lottable06>"
                    If Not String.IsNullOrEmpty(Lottable07) Then Command += "<Lottable07>" & Lottable07 & "</Lottable07>"
                    If Not String.IsNullOrEmpty(Lottable08) Then Command += "<Lottable08>" & Lottable08 & "</Lottable08>"
                    If Not String.IsNullOrEmpty(Lottable09) Then Command += "<Lottable09>" & Lottable09 & "</Lottable09>"
                    If Not String.IsNullOrEmpty(Lottable10) Then Command += "<Lottable10>" & Lottable10 & "</Lottable10>"

                    Command += "</ShipmentOrderDetail>"

                Else
                    tmp = "Error: Extern Order Key already exists!"
                    MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                    Return MyTable
                End If
            End If

            Try
                Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>ShipmentOrder</MessageType><Action>store</Action><Sender><User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId>	</Sender><Recipient><SystemID>" & Facility & "</SystemID></Recipient></Head><Body><ShipmentOrder><ShipmentOrderHeader>" & Command & "</ShipmentOrderHeader></ShipmentOrder></Body></Message>"
                Dim soapResult As String = CommonMethods.sendwebRequest(Xml)

                If String.IsNullOrEmpty(soapResult) Then
                    tmp = "Error: Unable to connect to webservice, kindly check the logs"
                Else
                    Dim dsresult As DataSet = New DataSet
                    Dim doc As XmlDocument = New XmlDocument
                    doc.LoadXml(soapResult)
                    Dim xmlFile As XmlReader = XmlReader.Create(New StringReader(soapResult), New XmlReaderSettings)
                    If LCase(soapResult).Contains("error") Then
                        Dim nodeList As XmlNodeList
                        If soapResult.Contains("ERROR") Then
                            nodeList = doc.GetElementsByTagName("Error")
                        Else
                            nodeList = doc.GetElementsByTagName("string")
                        End If
                        Dim message As String = ""
                        For Each node As XmlNode In nodeList
                            message = node.InnerText
                        Next
                        message = Regex.Replace(message, "&.*?;", "")
                        tmp = "Error: " & message & "<br/>"
                        Dim logger As Logger = LogManager.GetCurrentClassLogger()
                        logger.Error(message, "", "")
                    Else
                        tmp = CommonMethods.incremenetKey(Facility, "EXTERNSO")
                        If tmp = "" Then
                            Dim order As String = "", logmessage As String = ""

                            Dim nodeList As XmlNodeList = doc.GetElementsByTagName("OrderKey")
                            For Each node As XmlNode In nodeList
                                order = node.InnerText
                                key = order
                                queryurl = "?warehouse=" & Facility & "&order=" & order
                                logmessage = CommonMethods.logger(Facility, "Order", order, HttpContext.Current.Session("userkey").ToString)
                            Next

                            nodeList = doc.GetElementsByTagName("SerialKey")
                            For Each node As XmlNode In nodeList
                                serialkey = node.InnerText
                                Exit For
                            Next

                            If Not EditOperation Then
                                Dim nodeList2 As XmlNodeList = doc.GetElementsByTagName("OrderLineNumber")
                                Dim lineno As String = ""
                                For Each node As XmlNode In nodeList2
                                    lineno = node.InnerText
                                    logmessage += CommonMethods.logger(Facility, "OrderDetail", order & "-" & lineno, HttpContext.Current.Session("userkey").ToString)
                                Next
                            End If

                            If Not String.IsNullOrEmpty(logmessage) Then
                                tmp = "Logging Error: " + logmessage + "<br />"
                                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                                logger.Error(logmessage, "", "")
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                tmp = "Error: " & ex.Message & vbTab + ex.GetType.ToString & "<br/>"
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(ex, "", "")
            End Try
        End If

        MyTable.Rows.Add(tmp, key, serialkey, queryurl)

        Return MyTable
    End Function
    Private Function SaveOrderManagement(ByVal MyID As Integer) As DataTable
        Dim MyTable As New DataTable
        MyTable.Columns.Add("tmp", GetType(String))
        MyTable.Columns.Add("key", GetType(String))
        MyTable.Columns.Add("serialkey", GetType(String))
        MyTable.Columns.Add("queryurl", GetType(String))

        Dim tmp As String = "", Command As String = "", CommandDetails As String = "", columns As String = "", key As String = "", serialkey As String = "", queryurl As String = ""
        Dim IsValid As Boolean = True
        Dim EditOperation As Boolean = MyID <> 0
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Field_Facility")) _
        , externorder As String = HttpContext.Current.Request.Item("Field_ExternOrderKey") _
        , orderkey As String = HttpContext.Current.Request.Item("Field_OrderManagKey") _
        , Owner As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
        , ConsigneeKey As String = UCase(HttpContext.Current.Request.Item("Field_ConsigneeKey")) _
        , type As String = HttpContext.Current.Request.Item("Field_Type") _
        , orderdate As String = HttpContext.Current.Request.Item("Field_OrderDate") _
        , requestedshipdate As String = HttpContext.Current.Request.Item("Field_RequestedShipDate") _
        , UDF1 As String = HttpContext.Current.Request.Item("Field_SUsr1") _
        , UDF2 As String = HttpContext.Current.Request.Item("Field_SUsr2") _
        , UDF3 As String = HttpContext.Current.Request.Item("Field_SUsr3") _
        , UDF4 As String = HttpContext.Current.Request.Item("Field_SUsr4") _
        , UDF5 As String = HttpContext.Current.Request.Item("Field_SUsr5") _
        , exterline As String = HttpContext.Current.Request.Item("DetailsField_ExternLineNo") _
        , Sku As String = UCase(HttpContext.Current.Request.Item("DetailsField_Sku")) _
        , OpenQty As String = HttpContext.Current.Request.Item("DetailsField_OpenQty") _
        , PackKey As String = HttpContext.Current.Request.Item("DetailsField_PackKey") _
        , UOM As String = HttpContext.Current.Request.Item("DetailsField_UOM") _
        , UDF1Dtl As String = HttpContext.Current.Request.Item("DetailsField_SUsr1") _
        , UDF2Dtl As String = HttpContext.Current.Request.Item("DetailsField_SUsr2") _
        , UDF3Dtl As String = HttpContext.Current.Request.Item("DetailsField_SUsr3") _
        , UDF4Dtl As String = HttpContext.Current.Request.Item("DetailsField_SUsr4") _
        , Lottable01 As String = HttpContext.Current.Request.Item("DetailsField_Lottable01") _
        , Lottable02 As String = HttpContext.Current.Request.Item("DetailsField_Lottable02") _
        , Lottable03 As String = HttpContext.Current.Request.Item("DetailsField_Lottable03") _
        , Lottable04 As String = HttpContext.Current.Request.Item("DetailsField_Lottable04") _
        , Lottable05 As String = HttpContext.Current.Request.Item("DetailsField_Lottable05") _
        , Lottable06 As String = HttpContext.Current.Request.Item("DetailsField_Lottable06") _
        , Lottable07 As String = HttpContext.Current.Request.Item("DetailsField_Lottable07") _
        , Lottable08 As String = HttpContext.Current.Request.Item("DetailsField_Lottable08") _
        , Lottable09 As String = HttpContext.Current.Request.Item("DetailsField_Lottable09") _
        , Lottable10 As String = HttpContext.Current.Request.Item("DetailsField_Lottable10") _
        , Price As String = HttpContext.Current.Request.Item("DetailsField_Price") _
        , Currency As String = HttpContext.Current.Request.Item("DetailsField_Currency")


        Dim cmd As SqlCommand = New SqlCommand, cmdDetails As SqlCommand = New SqlCommand
        Dim cmdOracle As OracleCommand = New OracleCommand, cmdOracleDetails As OracleCommand = New OracleCommand

        If Not EditOperation Then
            If Not String.IsNullOrEmpty(Facility) Then
                columns += "WHSEID, SUsr1, SUsr2, SUsr3, SUsr4, SUsr5 "
                If CommonMethods.dbtype = "sql" Then
                    Command += " @facility, @SUsr1, @SUsr2, @SUsr3, @SUsr4, @SUsr5 "
                    cmd.Parameters.AddWithValue("@facility", Facility)
                    cmd.Parameters.AddWithValue("@SUsr1", UDF1)
                    cmd.Parameters.AddWithValue("@SUsr2", UDF2)
                    cmd.Parameters.AddWithValue("@SUsr3", UDF3)
                    cmd.Parameters.AddWithValue("@SUsr4", UDF4)
                    cmd.Parameters.AddWithValue("@SUsr5", UDF5)
                Else
                    Command += " :facility, :SUsr1, :SUsr2, :SUsr3, :SUsr4, :SUsr5 "
                    cmdOracle.Parameters.Add(New OracleParameter("facility", Facility))
                    cmdOracle.Parameters.Add(New OracleParameter("SUsr1", UDF1))
                    cmdOracle.Parameters.Add(New OracleParameter("SUsr2", UDF2))
                    cmdOracle.Parameters.Add(New OracleParameter("SUsr3", UDF3))
                    cmdOracle.Parameters.Add(New OracleParameter("SUsr4", UDF4))
                    cmdOracle.Parameters.Add(New OracleParameter("SUsr5", UDF5))
                End If
            Else
                IsValid = False
                tmp += "Facility must be defined <br/>"
            End If

            If Not String.IsNullOrEmpty(externorder) Then
                columns += ", EXTERNORDERKEY"
                If CommonMethods.dbtype = "sql" Then
                    Command += ", @externokey "
                    cmd.Parameters.AddWithValue("@externokey", externorder)
                Else
                    Command += ", :externokey "
                    cmdOracle.Parameters.Add(New OracleParameter("externokey", externorder))
                End If
            ElseIf IsValid Then
                externorder = CommonMethods.getExternKey(Facility, "EXTERNSO")
                columns += ", EXTERNORDERKEY"
                Command += ",'" & externorder & "'"
            End If

            If Not String.IsNullOrEmpty(orderdate) Then
                Dim datetime As DateTime
                If DateTime.TryParseExact(orderdate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                    Dim datetime2 As DateTime = DateTime.ParseExact(orderdate, "MM/dd/yyyy", Nothing)
                    columns += ", ORDERDATE"
                    orderdate = datetime2.ToString("yyyy-MM-dd HH:mm:ss")
                    If CommonMethods.dbtype = "sql" Then
                        Command += ", @orderdate "
                        cmd.Parameters.AddWithValue("@orderdate", orderdate)
                    Else
                        Command += ",TO_DATE( :orderdate,'yyyy/mm/dd hh24:mi:ss')  "
                        cmdOracle.Parameters.Add(New OracleParameter("orderdate", orderdate))
                    End If
                Else
                    IsValid = False
                    tmp += "Order date doesn't match the required date format <br/>"
                End If
            End If

            If Not String.IsNullOrEmpty(Owner) Then
                Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
                If owners.Any(Function(x) x = Owner) Or owners.Any(Function(x) x = "ALL") Then
                    columns += ", STORERKEY"
                    If CommonMethods.dbtype = "sql" Then
                        Command += ", @Owner "
                        cmd.Parameters.AddWithValue("@Owner", Owner)
                    Else
                        Command += ", :Owner "
                        cmdOracle.Parameters.Add(New OracleParameter("Owner", Owner))
                    End If
                Else
                    IsValid = False
                    tmp += "This owner is not authorized <br/>"
                End If
            Else
                IsValid = False
                tmp += "Owner must be defined <br/>"
            End If

            If Not String.IsNullOrEmpty(type) Then
                Dim DTOrderType = CommonMethods.getCodeDD(Facility, "codelkup", "ORDERTYPE")
                Dim DROrderType() As DataRow = DTOrderType.Select("DESCRIPTION='" & type & "'")
                If DROrderType.Length > 0 Then
                    columns += ", TYPE"
                    If CommonMethods.dbtype = "sql" Then
                        Command += ", @type "
                        cmd.Parameters.AddWithValue("@type", DROrderType(0)!CODE)
                    Else
                        Command += ", :type "
                        cmdOracle.Parameters.Add(New OracleParameter("type", DROrderType(0)!CODE))
                    End If
                Else
                    IsValid = False
                    tmp += "Type must be defined <br/>"
                End If
            Else
                IsValid = False
                tmp += "Type must be defined <br/>"
            End If

            If Not String.IsNullOrEmpty(ConsigneeKey) Then
                Dim consignees As String() = CommonMethods.getConsigneePerUser(HttpContext.Current.Session("userkey").ToString)
                If consignees.Any(Function(x) x = ConsigneeKey) Or consignees.Any(Function(x) x = "ALL") Then
                    columns += ", ConsigneeKey"
                    If CommonMethods.dbtype = "sql" Then
                        Command += ", @consignee "
                        cmd.Parameters.AddWithValue("@consignee", ConsigneeKey)
                    Else
                        Command += ", :consignee "
                        cmdOracle.Parameters.Add(New OracleParameter("consignee", ConsigneeKey))
                    End If
                Else
                    IsValid = False
                    tmp += "This consignee is not authorized <br/>"
                End If
            End If

            If Not String.IsNullOrEmpty(requestedshipdate) Then
                Dim datetime As DateTime
                If DateTime.TryParseExact(requestedshipdate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                    Dim datetime2 As DateTime = DateTime.ParseExact(requestedshipdate, "MM/dd/yyyy", Nothing)
                    columns += ", RequestedShipDate"
                    requestedshipdate = datetime2.ToString("yyyy-MM-dd HH:mm:ss")
                    If CommonMethods.dbtype = "sql" Then
                        Command += ", @Reqdate "
                        cmd.Parameters.AddWithValue("@Reqdate", requestedshipdate)
                    Else
                        Command += ",TO_DATE( :Reqdate,'yyyy/mm/dd hh24:mi:ss')  "
                        cmdOracle.Parameters.Add(New OracleParameter("Reqdate", requestedshipdate))
                    End If
                Else
                    IsValid = False
                    tmp += "Requested ship date doesn't match the required date format <br/>"
                End If
            End If
        Else
            If CommonMethods.dbtype = "sql" Then
                Command += "  SUsr1 = @SUsr1, SUsr2 = @SUsr2, SUsr3 = @SUsr3, SUsr4 = @SUsr4, SUsr5 = @SUsr5 "
                cmd.Parameters.AddWithValue("@SerialKey", MyID)
                cmd.Parameters.AddWithValue("@SUsr1", UDF1)
                cmd.Parameters.AddWithValue("@SUsr2", UDF2)
                cmd.Parameters.AddWithValue("@SUsr3", UDF3)
                cmd.Parameters.AddWithValue("@SUsr4", UDF4)
                cmd.Parameters.AddWithValue("@SUsr5", UDF5)
            Else
                Command += "  SUsr1 = :SUsr1, SUsr2 = :SUsr2, SUsr3 = :SUsr3, SUsr4 = :SUsr4, SUsr5 = :SUsr5 "
                cmdOracle.Parameters.Add(New OracleParameter(":SerialKey", UDF1))
                cmdOracle.Parameters.Add(New OracleParameter("SUsr1", UDF1))
                cmdOracle.Parameters.Add(New OracleParameter("SUsr2", UDF2))
                cmdOracle.Parameters.Add(New OracleParameter("SUsr3", UDF3))
                cmdOracle.Parameters.Add(New OracleParameter("SUsr4", UDF4))
                cmdOracle.Parameters.Add(New OracleParameter("SUsr5", UDF5))
            End If

            If Not String.IsNullOrEmpty(ConsigneeKey) Then
                Dim consignees As String() = CommonMethods.getConsigneePerUser(HttpContext.Current.Session("userkey").ToString)
                If consignees.Any(Function(x) x = ConsigneeKey) Or consignees.Any(Function(x) x = "ALL") Then
                    If CommonMethods.dbtype = "sql" Then
                        Command += ", ConsigneeKey = @consignee "
                        cmd.Parameters.AddWithValue("@consignee", ConsigneeKey)
                    Else
                        Command += ", ConsigneeKey = :consignee "
                        cmdOracle.Parameters.Add(New OracleParameter("consignee", ConsigneeKey))
                    End If
                Else
                    IsValid = False
                    tmp += "This consignee is not authorized <br/>"
                End If
            End If

            If Not String.IsNullOrEmpty(requestedshipdate) Then
                Dim datetime As DateTime
                If DateTime.TryParseExact(requestedshipdate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                    Dim datetime2 As DateTime = DateTime.ParseExact(requestedshipdate, "MM/dd/yyyy", Nothing)
                    requestedshipdate = datetime2.ToString("yyyy-MM-dd HH:mm:ss")
                    If CommonMethods.dbtype = "sql" Then
                        Command += ", RequestedShipDate = @Reqdate "
                        cmd.Parameters.AddWithValue("@Reqdate", requestedshipdate)
                    Else
                        Command += ", RequestedShipDate = TO_DATE( :Reqdate,'yyyy/mm/dd hh24:mi:ss')  "
                        cmdOracle.Parameters.Add(New OracleParameter("Reqdate", requestedshipdate))
                    End If
                Else
                    IsValid = False
                    tmp += "Requested ship date doesn't match the required date format <br/>"
                End If
            End If
        End If

        If IsValid Then
            If Not EditOperation Then
                Dim exist As Integer = CommonMethods.checkOrderManagementExist(Facility, externorder)
                If exist = 0 Then
                    If String.IsNullOrEmpty(exterline) And String.IsNullOrEmpty(Sku) And String.IsNullOrEmpty(OpenQty) Then
                        tmp = "Detail line cannot be empty <br/>"
                        MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                        Return MyTable
                    End If

                    CommandDetails += " insert into " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & " ORDERMANAGDETAIL "
                    CommandDetails += " (WHSEID, EXTERNORDERKEY, ORDERMANAGKEY, orderkey, OrderLineNumber, ORDERMANAGLINENUMBER, "
                    CommandDetails += "  StorerKey, ExternLineNo, Sku, OpenQty, PackKey, UOM, SUsr1, SUsr2, SUsr3, SUsr4, "
                    CommandDetails += " Lottable01, Lottable02, Lottable03, Lottable04, Lottable05, Lottable06, "
                    CommandDetails += " Lottable07, Lottable08, Lottable09, Lottable10, UNITPRICE, SUsr5) values "

                    Dim linenumber As String = "1"
                    Dim linenb As String = linenumber.ToString.PadLeft(5, "0")

                    If CommonMethods.dbtype = "sql" Then
                        CommandDetails += "(@warehouse ,@externorderkey"
                        If Not EditOperation Then
                            CommandDetails += ",(select ORDERMANAGKEY from  ORDERMANAG where WHSEID=@warehouse and EXTERNORDERKEY=@externorderkey),(select ORDERMANAGKEY from ORDERMANAG where WHSEID=@warehouse and EXTERNORDERKEY=@externorderkey)"
                        Else
                            CommandDetails += ",@ordermanagkey,@orderkey"
                            cmdDetails.Parameters.AddWithValue("@ordermanagkey", orderkey)
                            cmdDetails.Parameters.AddWithValue("@orderkey", orderkey)
                        End If
                        CommandDetails += ",@linenb,@linenb,@owner"
                        cmdDetails.Parameters.AddWithValue("@warehouse", Facility)
                        cmdDetails.Parameters.AddWithValue("@externorderkey", externorder)
                        cmdDetails.Parameters.AddWithValue("@linenb", linenb)
                        cmdDetails.Parameters.AddWithValue("@owner", Owner)
                    Else
                        CommandDetails += "(:warehouse,:externorderkey"
                        If Not EditOperation Then
                            CommandDetails += ",(select ORDERMANAGKEY from  ORDERMANAG where WHSEID=:warehouse and EXTERNORDERKEY=:externorderkey),(select ORDERMANAGKEY from ORDERMANAG where WHSEID=:warehouse and EXTERNORDERKEY=:externorderkey)"
                        Else
                            CommandDetails += ",:ordermanagkey,:orderkey"
                            cmdDetails.Parameters.AddWithValue("ordermanagkey", orderkey)
                            cmdDetails.Parameters.AddWithValue("orderkey", orderkey)
                        End If
                        CommandDetails += ",:linenb, :linenb,:owner"
                        cmdOracleDetails.Parameters.Add(New OracleParameter("warehouse", Facility))
                        cmdOracleDetails.Parameters.Add(New OracleParameter("externorderkey", externorder))
                        cmdOracleDetails.Parameters.Add(New OracleParameter("linenb", linenb))
                        cmdOracleDetails.Parameters.Add(New OracleParameter("owner", Owner))
                    End If

                    Try
                        If Not String.IsNullOrEmpty(exterline) Then
                            exterline = exterline.ToString.PadLeft(5, "0")
                        Else
                            exterline = linenumber.ToString.PadLeft(5, "0")
                        End If
                    Catch ex As Exception
                        exterline = linenumber.ToString.PadLeft(5, "0")
                    End Try
                    If CommonMethods.dbtype = "sql" Then
                        CommandDetails += " , @exterline"
                        cmdDetails.Parameters.AddWithValue("@exterline", exterline)
                    Else
                        CommandDetails += " , :exterline"
                        cmdOracleDetails.Parameters.Add(New OracleParameter("exterline", exterline))
                    End If

                    Try
                        If Not String.IsNullOrEmpty(Sku) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @Sku"
                                cmdDetails.Parameters.AddWithValue("@Sku", Sku)
                            Else
                                CommandDetails += " , :Sku"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("Sku", Sku))
                            End If
                        Else
                            IsValid = False
                            tmp += "Item cannot be empty <br/>"
                        End If
                    Catch ex As Exception
                        IsValid = False
                        tmp += "Item cannot be empty <br/>"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(OpenQty) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @OpenQty"
                                cmdDetails.Parameters.AddWithValue("@OpenQty", OpenQty)
                            Else
                                CommandDetails += " , :OpenQty"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("OpenQty", OpenQty))
                            End If
                        Else
                            IsValid = False
                            tmp += "Open Qty cannot be empty <br/>"
                        End If
                    Catch ex As Exception
                        IsValid = False
                        tmp += "Open Qty cannot be empty <br/>"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(PackKey) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @pack"
                                cmdDetails.Parameters.AddWithValue("@pack", PackKey)
                            Else
                                CommandDetails += " , :pack"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("pack", PackKey))
                            End If
                        Else
                            IsValid = False
                            tmp += "Pack cannot be empty <br/>"
                        End If
                    Catch ex As Exception
                        IsValid = False
                        tmp += "Pack cannot be empty <br/>"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(UOM) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @uom"
                                cmdDetails.Parameters.AddWithValue("@uom", UOM)
                            Else
                                CommandDetails += " , :uom"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("uom", UOM))
                            End If
                        Else
                            CommandDetails += " ,'EA'"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,'EA'"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(UDF1Dtl) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @SUsr1"
                                cmdDetails.Parameters.AddWithValue("@SUsr1", UDF1Dtl)
                            Else
                                CommandDetails += " , :SUsr1"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("SUsr1", UDF1Dtl))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(UDF2Dtl) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @SUsr2"
                                cmdDetails.Parameters.AddWithValue("@SUsr2", UDF2Dtl)
                            Else
                                CommandDetails += " , :SUsr2"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("SUsr2", UDF2Dtl))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(UDF3Dtl) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @SUsr3"
                                cmdDetails.Parameters.AddWithValue("@SUsr3", UDF3Dtl)
                            Else
                                CommandDetails += " , :SUsr3"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("SUsr3", UDF3Dtl))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(UDF4Dtl) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @SUsr4"
                                cmdDetails.Parameters.AddWithValue("@SUsr4", UDF4Dtl)
                            Else
                                CommandDetails += " , :SUsr4"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("SUsr4", UDF4Dtl))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable01) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @lot1"
                                cmdDetails.Parameters.AddWithValue("@lot1", Lottable01)
                            Else
                                CommandDetails += " , :lot1"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("lot1", Lottable01))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable02) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @lot2"
                                cmdDetails.Parameters.AddWithValue("@lot2", Lottable02)
                            Else
                                CommandDetails += " , :lot2"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("lot2", Lottable02))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable03) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @lot3"
                                cmdDetails.Parameters.AddWithValue("@lot3", Lottable03)
                            Else
                                CommandDetails += " , :lot3"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("lot3", Lottable03))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable04) Then
                            Dim datetime As DateTime
                            If DateTime.TryParseExact(Lottable04, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                                Dim datetime2 As DateTime = DateTime.ParseExact(Lottable04, "MM/dd/yyyy", Nothing)
                                If CommonMethods.dbtype = "sql" Then
                                    CommandDetails += " , @lot4"
                                Else
                                    CommandDetails += " , :lot4"
                                End If
                                If Double.Parse(CommonMethods.version) >= 11 Then
                                    If CommonMethods.dbtype = "sql" Then
                                        cmdDetails.Parameters.AddWithValue("@lot4", datetime2.ToString("MM/dd/yyyy 14:00:00"))
                                    Else
                                        cmdOracleDetails.Parameters.Add(New OracleParameter("lot4", datetime2.ToString("MM/dd/yyyy 14:00:00")))
                                    End If
                                Else
                                    If CommonMethods.dbtype = "sql" Then
                                        cmdDetails.Parameters.AddWithValue("@lot4", datetime2.ToString("dd/MM/yyyy 14:00:00"))
                                    Else
                                        cmdOracleDetails.Parameters.Add(New OracleParameter("lot4", datetime2.ToString("dd/MM/yyyy 14:00:00")))
                                    End If
                                End If
                            Else
                                IsValid = False
                                tmp += "Lottable04 doesn't match the required date format <br/>"
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable05) Then
                            Dim datetime As DateTime
                            If DateTime.TryParseExact(Lottable05, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                                Dim datetime2 As DateTime = DateTime.ParseExact(Lottable05, "MM/dd/yyyy", Nothing)
                                If CommonMethods.dbtype = "sql" Then
                                    CommandDetails += " , @lot5"
                                Else
                                    CommandDetails += " , :lot5"
                                End If
                                If Double.Parse(CommonMethods.version) >= 11 Then
                                    If CommonMethods.dbtype = "sql" Then
                                        cmdDetails.Parameters.AddWithValue("@lot5", datetime2.ToString("MM/dd/yyyy 14:00:00"))
                                    Else
                                        cmdOracleDetails.Parameters.Add(New OracleParameter("lot5", datetime2.ToString("MM/dd/yyyy 14:00:00")))
                                    End If
                                Else
                                    If CommonMethods.dbtype = "sql" Then
                                        cmdDetails.Parameters.AddWithValue("@lot5", datetime2.ToString("dd/MM/yyyy 14:00:00"))
                                    Else
                                        cmdOracleDetails.Parameters.Add(New OracleParameter("lot5", datetime2.ToString("dd/MM/yyyy 14:00:00")))
                                    End If
                                End If
                            Else
                                IsValid = False
                                tmp += "Lottable05 doesn't match the required date format <br/>"
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable06) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @lot6"
                                cmdDetails.Parameters.AddWithValue("@lot6", Lottable06)
                            Else
                                CommandDetails += " , :lot6"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("lot6", Lottable06))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable07) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @lot7"
                                cmdDetails.Parameters.AddWithValue("@lot7", Lottable07)
                            Else
                                CommandDetails += " , :lot7"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("lot7", Lottable07))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable08) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @lot8"
                                cmdDetails.Parameters.AddWithValue("@lot8", Lottable08)
                            Else
                                CommandDetails += " , :lot8"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("lot8", Lottable08))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable09) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @lot9"
                                cmdDetails.Parameters.AddWithValue("@lot9", Lottable09)
                            Else
                                CommandDetails += " , :lot9"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("lot9", Lottable09))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable10) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @lot10"
                                cmdDetails.Parameters.AddWithValue("@lot10", Lottable10)
                            Else
                                CommandDetails += " , :lot10"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("lot10", Lottable10))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Price) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @unitprice"
                                cmdDetails.Parameters.AddWithValue("@unitprice", Double.Parse(Price))
                            Else
                                CommandDetails += " , :unitprice"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("unitprice", Double.Parse(Price)))
                            End If
                        Else
                            CommandDetails += " , 0"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,0"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Currency) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @currency)"
                                cmdDetails.Parameters.AddWithValue("@currency", Currency)
                            Else
                                CommandDetails += " , :currency)"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("currency", Currency))
                            End If
                        Else
                            CommandDetails += " ,'')"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,'')"
                    End Try

                Else
                    tmp = "Error: Extern Order Key already exists!"
                    MyTable.Rows.Add(tmp, key, serialkey, queryurl)
                    Return MyTable
                End If
            End If
        End If

        If IsValid Then
            If CommonMethods.dbtype = "sql" Then
                Try
                    Dim query As String = ""
                    If EditOperation Then
                        query += " update dbo.ORDERMANAG set " & Command & " where SerialKey = @SerialKey "
                    Else
                        query += " insert into dbo.ORDERMANAG (ORDERMANAGKEY, ORDERKEY, ORDERMANAGSTATUS," & columns & " )values((select  (RIGHT('0000000000' + CAST(ISNULL(max(ORDERMANAGKEY)+1,1) AS varchar(10)) , 10) ) from dbo.ORDERMANAG), (select  (RIGHT('0000000000' + CAST(ISNULL(max(ORDERMANAGKEY)+1,1) AS varchar(10)) , 10) ) from dbo.ORDERMANAG),'NOT CREATED in SCE', " & Command & ");"
                    End If

                    Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                    conn.Open()
                    cmd.CommandText = query
                    cmd.Connection = conn
                    cmd.ExecuteNonQuery()
                    conn.Close()
                    If Not EditOperation Then
                        CommonMethods.incremenetKey(Facility, "EXTERNSO")
                        conn = New SqlConnection(CommonMethods.dbconx)
                        conn.Open()
                        cmdDetails.CommandText = CommandDetails
                        cmdDetails.Connection = conn
                        cmdDetails.ExecuteNonQuery()
                        conn.Close()
                    End If
                Catch ex As Exception
                    IsValid = False
                    tmp += "Error: " & ex.Message & vbTab + ex.GetType.ToString & "<br/>"
                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                    logger.Error(ex, "", "")
                End Try
            Else
                Try
                    Dim query As String = ""
                    If EditOperation Then
                        query += " update dbo.ORDERMANAG set " & Command & " where SerialKey = :SerialKey "
                    Else
                        query += " insert into SYSTEM.ORDERMANAG (ORDERMANAGKEY, ORDERKEY, ORDERMANAGSTATUS," & columns & " )values((select  (SUBSTR(CONCAT('000000000' , CAST(NVL(max(ORDERMANAGKEY)+1,1) AS nvarchar2(10))) , -10) ) from SYSTEM.ORDERMANAG), (select  (SUBSTR(CONCAT('000000000' , CAST(NVL(max(ORDERMANAGKEY)+1,1) AS nvarchar2(10))) , -10) ) from SYSTEM.ORDERMANAG),'NOT CREATED in SCE', " & Command & ")"
                    End If
                    Dim conn As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                    conn.Open()
                    cmdOracle.CommandText = query
                    cmdOracle.Connection = conn
                    cmdOracle.ExecuteNonQuery()
                    conn.Close()
                    If Not EditOperation Then
                        CommonMethods.incremenetKey(Facility, "EXTERNSO")
                        conn = New OracleConnection(CommonMethods.dbconx)
                        conn.Open()
                        cmdOracleDetails.CommandText = CommandDetails
                        cmdOracleDetails.Connection = conn
                        cmdOracleDetails.ExecuteNonQuery()
                        conn.Close()
                    End If
                Catch ex As Exception
                    IsValid = False
                    tmp += "Error: " & ex.Message & vbTab + ex.GetType.ToString & "<br/>"
                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                    logger.Error(ex, "", "")
                End Try
            End If
            If Not IsValid And Not EditOperation Then
                Dim SqlDelete As String = ""
                SqlDelete += " delete from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ordermanagdetail where WHSEID= '" & Facility & "'  and EXTERNORDERKEY = '" & externorder & "' "
                SqlDelete += " delete from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ordermanag where WHSEID= '" & Facility & "'  and EXTERNORDERKEY = '" & externorder & "' "
                Dim tmp2 As String = (New SQLExec).Execute(SqlDelete)
                If tmp2 <> "" Then tmp += tmp2
            End If
        End If

        If Not EditOperation Then
            Dim sql As String = ""
            If CommonMethods.dbtype = "sql" Then
                sql += "select RIGHT('0000000000' + CAST(ISNULL(max(ORDERMANAGKEY),1) AS varchar(10)) , 10) as ORDERMANAGKEY from dbo.ORDERMANAG"
            Else
                sql += "select SUBSTR(CONCAT('000000000' , CAST(NVL(max(ORDERMANAGKEY),1) AS nvarchar2(10))) , -10) as ORDERMANAGKEY from SYSTEM.ORDERMANAG"
            End If
            sql += " select top 1 SerialKey from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAG order by SerialKey DESC "
            Dim ds As DataSet = (New SQLExec).Cursor(sql)
            If ds.Tables(0).Rows.Count > 0 Then
                key = ds.Tables(0).Rows(0)!ORDERMANAGKEY
                queryurl = "?ordermanagkey=" & key
            End If
            If ds.Tables(1).Rows.Count > 0 Then
                serialkey = ds.Tables(1).Rows(0)!SerialKey
            End If
        Else
            key = orderkey
            queryurl = "?ordermanagkey=" & orderkey
            serialkey = MyID
        End If

        MyTable.Rows.Add(tmp, key, serialkey, queryurl)
        Return MyTable
    End Function
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class