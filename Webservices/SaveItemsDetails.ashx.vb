Imports Newtonsoft.Json
Imports System.Data.SqlClient
Imports Oracle.ManagedDataAccess.Client
Imports NLog
Imports System.IO
Imports System.Globalization
Imports System.Xml

Public Class SaveItemsDetails
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub SaveItem(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim mySearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim MyID As Integer = Val(HttpContext.Current.Request.Item("MyID"))
        Dim tmp As String = "", Status As String = ""
        REM ghina karame - 11/05/2020- get the system flag useRest from webconfig to know if we should replace soapapi with restapi -begin
        Dim useRest As String = ConfigurationManager.AppSettings("UseRestAPI")
        Dim version As String = ConfigurationManager.AppSettings("version")
        REM ghina karame - 11/05/2020- get the system flag useRest from webconfig to know if we should replace soapapi with restapi -end

        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)
        Using writer As JsonWriter = New JsonTextWriter(sw)
            writer.Formatting = Newtonsoft.Json.Formatting.Indented
            writer.WriteStartObject()

            writer.WritePropertyName("tmp")
            If mySearchTable = "Warehouse_PO" Then
                'ghina karame - 01/06/2020- restapicalls- if flag is on and version > 11 then use rest calls instead of soap calls -begin
                If useRest = "1" & version >= "11" Then
                    tmp = SaveRestPurchaseOrderDetail(MyID)
                Else
                    tmp = SavePurchaseOrderDetail(MyID)
                End If
                'ghina karame - 01/06/2020- restapicalls- if flag is on and version > 11 then use rest calls instead of soap calls -end

            ElseIf mySearchTable = "Warehouse_ASN" Then
                tmp = SaveASNDetail(MyID)
            ElseIf mySearchTable = "Warehouse_SO" Then
                tmp = SaveSODetail(MyID)
            ElseIf mySearchTable = "Warehouse_OrderManagement" Then
                tmp = SaveOrderManagementDetail(MyID)
            End If
            writer.WriteValue(tmp)

            writer.WritePropertyName("Status")
            If tmp = "" Then
                If mySearchTable = "Warehouse_PO" Then
                    Status = GetPurchaseOrderStatus()
                ElseIf mySearchTable = "Warehouse_ASN" Then
                    Status = GetASNStatus()
                ElseIf mySearchTable = "Warehouse_SO" Then
                    Status = GetSOStatus()
                End If
            End If
            writer.WriteValue(Status)

            writer.WriteEndObject()
        End Using
        context.Response.Write(sw)
        context.Response.End()

    End Sub
    Private Function SavePurchaseOrderDetail(ByVal MyID As Integer) As String
        Dim tmp As String = "", Command As String = "", Saved As String = "", line As String = ""
        Dim EditOperation As Boolean = MyID <> 0
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Field_Facility")) _
        , Owner As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
        , pokey As String = HttpContext.Current.Request.Item("Field_POKey") _
        , externpo As String = HttpContext.Current.Request.Item("Field_ExternPOKey") _
        , exterline As String = HttpContext.Current.Request.Item("DetailsField_ExternLineNo") _
        , Sku As String = UCase(HttpContext.Current.Request.Item("DetailsField_Sku")) _
        , QtyOrdered As String = HttpContext.Current.Request.Item("DetailsField_QtyOrdered")

        If Not EditOperation Then
            Command += "<PurchaseOrder>" & pokey & "</PurchaseOrder>"
        Else
            Command += "<POKey>" & pokey & "</POKey>"
        End If
        Command += "<ExternPOKey>" & externpo & "</ExternPOKey>"
        Command += "<StorerKey>" & Owner & "</StorerKey>"
        Command += "<PurchaseOrderDetail> "

        If EditOperation Then
            Dim warehouse As String = Facility
            If LCase(warehouse.Substring(0, 6)) = "infor_" Then warehouse = warehouse.Substring(6, Facility.Length - 6)
            If LCase(warehouse).Contains("_") Then warehouse = warehouse.Split("_")(1)
            Dim sql As String = "select POLineNumber from " & warehouse & ".PODETAIL where POKey= '" & pokey & "' and ExternLineNo = '" & exterline & "'"
            Dim ds As DataSet = (New SQLExec).Cursor(sql)
            If ds.Tables(0).Rows.Count > 0 Then
                line = ds.Tables(0).Rows(0)!POLineNumber
                Command += "<POLineNumber>" & line & "</POLineNumber>"
            Else
                tmp = "Error: cannot get purchase line number <br/>"
                Return tmp
            End If
        End If

        If Not EditOperation Then
            If Not String.IsNullOrEmpty(Sku) Then
                Command += "<StorerKey>" & Owner & "</StorerKey><Sku>" & Sku & "</Sku>"
            Else
                tmp = "Item cannot be empty <br/>"
                Return tmp
            End If
        Else
            Command += "<StorerKey>" & Owner & "</StorerKey><Sku>" & Sku & "</Sku>"
        End If

        If Not String.IsNullOrEmpty(QtyOrdered) Then
            Command += "<QtyOrdered>" & QtyOrdered & "</QtyOrdered>"
        Else
            tmp = "Qty Ordered cannot be empty <br/>"
            Return tmp
        End If

        If Not EditOperation Then
            If Not String.IsNullOrEmpty(exterline) Then
                exterline = exterline.ToString.PadLeft(5, "0")
                Command += "<ExternLineNo>" & exterline & "</ExternLineNo>"
            Else
                exterline = CommonMethods.getExternline(Facility, pokey, "podetail", "POKey")
                Command += "<ExternLineNo>" & exterline & "</ExternLineNo>"
            End If
        Else
            Command += "<ExternLineNo>" & exterline & "</ExternLineNo>"
        End If

        Command += "</PurchaseOrderDetail>"

        Dim exist As Integer = 0

        If Not EditOperation Then exist = CommonMethods.checkPOLineExist(Facility, pokey, exterline)

        If exist = 0 Then
            Try
                Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>PurchaseOrder</MessageType><Action>store</Action><Sender><User>" & CommonMethods.username & "</User><Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId>	</Sender><Recipient><SystemID>" & Facility & "</SystemID></Recipient></Head><Body><PurchaseOrder><PurchaseOrderHeader>" & Command & "</PurchaseOrderHeader></PurchaseOrder></Body></Message>"
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
                        Dim logmessage As String = ""
                        If Not EditOperation Then
                            Dim results As XmlNodeList = doc.SelectNodes("//*[local-name()='PurchaseOrderDetail']")
                            Dim lineno As String = ""
                            For Each node As XmlNode In results
                                If node("ExternLineNo").InnerText.ToString = exterline Then
                                    lineno = node("POLineNumber").InnerText.ToString
                                    logmessage = CommonMethods.logger(Facility, "PoDetail", pokey + "-" + lineno, HttpContext.Current.Session("userkey").ToString)
                                End If
                            Next
                        Else
                            logmessage = CommonMethods.logger(Facility, "PoDetail", pokey + "-" + line, HttpContext.Current.Session("userkey").ToString)
                        End If
                        If Not String.IsNullOrEmpty(logmessage) Then
                            tmp = "Logging Error: " + logmessage + "<br />"
                            Dim logger As Logger = LogManager.GetCurrentClassLogger()
                            logger.Error(logmessage, "", "")
                        End If
                    End If
                End If
            Catch ex As Exception
                tmp = "Error: " & ex.Message & vbTab + ex.GetType.ToString & "<br/>"
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(ex, "", "")
            End Try
        Else
            tmp = "Extern line# already exists! <br/>"
        End If

        Return tmp
    End Function
    'ghina karame - RestApis - 03-06-2020 - method to save the podetail lines using rest calls -
    'mainly here the objective is to prepare the json data to send to the post request
    Private Function SaveRestPurchaseOrderDetail(ByVal MyID As Integer) As String
        Dim tmp As String = "", Command As String = "", Saved As String = "", line As String = ""
        Dim EditOperation As Boolean = MyID <> 0
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Field_Facility")) _
        , Owner As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
        , pokey As String = HttpContext.Current.Request.Item("Field_POKey") _
        , externpo As String = HttpContext.Current.Request.Item("Field_ExternPOKey") _
        , exterline As String = HttpContext.Current.Request.Item("DetailsField_ExternLineNo") _
        , Sku As String = UCase(HttpContext.Current.Request.Item("DetailsField_Sku")) _
        , QtyOrdered As String = HttpContext.Current.Request.Item("DetailsField_QtyOrdered")

        If Not EditOperation Then
            'Command += "<PurchaseOrder>" & pokey & "</PurchaseOrder>"
            Command += "{""externpokey"":""" + externpo + """"
        Else
            'Command += "<POKey>" & pokey & "</POKey>"
            Command += "{""pokey"":""" + pokey + """"
            Command += ",""externpokey"":""" + externpo + """"
        End If
        'Command += "<ExternPOKey>" & externpo & "</ExternPOKey>"
        'Command += "<StorerKey>" & Owner & "</StorerKey>"
        'Command += "<PurchaseOrderDetail> "

        Command += ",""storerkey"":""" + Owner + """"
        Command += ",""podetails"" : [ {"


        If EditOperation Then
            Dim warehouse As String = Facility
            If LCase(warehouse.Substring(0, 6)) = "infor_" Then warehouse = warehouse.Substring(6, Facility.Length - 6)
            If LCase(warehouse).Contains("_") Then warehouse = warehouse.Split("_")(1)
            Dim sql As String = "select POLineNumber from " & warehouse & ".PODETAIL where POKey= '" & pokey & "' and ExternLineNo = '" & exterline & "'"
            Dim ds As DataSet = (New SQLExec).Cursor(sql)
            If ds.Tables(0).Rows.Count > 0 Then
                line = ds.Tables(0).Rows(0)!POLineNumber
                'Command += "<POLineNumber>" & line & "</POLineNumber>"
                Command += """polinenumber"":""" + line + """"
            Else
                tmp = "Error: cannot get purchase line number <br/>"
                Return tmp
            End If
        End If

        If Not EditOperation Then
            If Not String.IsNullOrEmpty(Sku) Then
                'Command += "<StorerKey>" & Owner & "</StorerKey><Sku>" & Sku & "</Sku>"
                Command += """storerkey"":""" + Owner + """"
                Command += ",""sku"":""" + Sku + """"
            Else
                tmp = "Item cannot be empty <br/>"
                Return tmp
            End If
        Else
            'Command += "<StorerKey>" & Owner & "</StorerKey><Sku>" & Sku & "</Sku>"
            Command += """storerkey"":""" + Owner + """"
            Command += ",""sku"":""" + Sku + """"
        End If

        If Not String.IsNullOrEmpty(QtyOrdered) Then
            'Command += "<QtyOrdered>" & QtyOrdered & "</QtyOrdered>"
            Command += ",""qtyordered"":""" + QtyOrdered + """"

        Else
            tmp = "Qty Ordered cannot be empty <br/>"
            Return tmp
        End If

        If Not EditOperation Then
            If Not String.IsNullOrEmpty(exterline) Then
                exterline = exterline.ToString.PadLeft(5, "0")
                'Command += "<ExternLineNo>" & exterline & "</ExternLineNo>"
                Command += ",""externlineno"":""" + exterline + """"

            Else
                exterline = CommonMethods.getExternline(Facility, pokey, "podetail", "POKey")
                'Command += "<ExternLineNo>" & exterline & "</ExternLineNo>"
                Command += ",""externlineno"":""" + exterline + """"
            End If
        Else
            'Command += "<ExternLineNo>" & exterline & "</ExternLineNo>"
            Command += ",""externlineno"":""" + exterline + """"
        End If

        'Command += "</PurchaseOrderDetail>"
        Command += "}]}"

        Dim exist As Integer = 0

        If Not EditOperation Then exist = CommonMethods.checkPOLineExist(Facility, pokey, exterline)

        If exist = 0 Then
            Try
                'Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>PurchaseOrder</MessageType><Action>store</Action><Sender><User>" & CommonMethods.username & "</User><Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId>	</Sender><Recipient><SystemID>" & Facility & "</SystemID></Recipient></Head><Body><PurchaseOrder><PurchaseOrderHeader>" & Command & "</PurchaseOrderHeader></PurchaseOrder></Body></Message>"
                'Dim soapResult As String = CommonMethods.sendwebRequest(Xml)
                Dim uri As String = "/" & Facility & "/purchaseorders "
                tmp = CommonMethods.SaveRest(uri, Command)
                'If String.IsNullOrEmpty(soapResult) Then
                '    tmp = "Error: Unable to connect to webservice, kindly check the logs"
                'Else
                '    Dim dsresult As DataSet = New DataSet
                '    Dim doc As XmlDocument = New XmlDocument
                '    doc.LoadXml(soapResult)
                '    Dim xmlFile As XmlReader = XmlReader.Create(New StringReader(soapResult), New XmlReaderSettings)
                '    If LCase(soapResult).Contains("error") Then
                '        Dim nodeList As XmlNodeList
                '        If soapResult.Contains("ERROR") Then
                '            nodeList = doc.GetElementsByTagName("Error")
                '        Else
                '            nodeList = doc.GetElementsByTagName("string")
                '        End If
                '        Dim message As String = ""
                '        For Each node As XmlNode In nodeList
                '            message = node.InnerText
                '        Next
                '        message = Regex.Replace(message, "&.*?;", "")
                '        tmp = "Error: " & message & "<br/>"
                '        Dim logger As Logger = LogManager.GetCurrentClassLogger()
                '        logger.Error(message, "", "")
                '    Else
                '        Dim logmessage As String = ""
                '        If Not EditOperation Then
                '            Dim results As XmlNodeList = doc.SelectNodes("//*[local-name()='PurchaseOrderDetail']")
                '            Dim lineno As String = ""
                '            For Each node As XmlNode In results
                '                If node("ExternLineNo").InnerText.ToString = exterline Then
                '                    lineno = node("POLineNumber").InnerText.ToString
                '                    logmessage = CommonMethods.logger(Facility, "PoDetail", pokey + "-" + lineno, HttpContext.Current.Session("userkey").ToString)
                '                End If
                '            Next
                '        Else
                '            logmessage = CommonMethods.logger(Facility, "PoDetail", pokey + "-" + line, HttpContext.Current.Session("userkey").ToString)
                '        End If
                '        If Not String.IsNullOrEmpty(logmessage) Then
                '            tmp = "Logging Error: " + logmessage + "<br />"
                '            Dim logger As Logger = LogManager.GetCurrentClassLogger()
                '            logger.Error(logmessage, "", "")
                '        End If
                '    End If
                'End If
            Catch ex As Exception
                tmp = "Error: " & ex.Message & vbTab + ex.GetType.ToString & "<br/>"
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(ex, "", "")
            End Try
        Else
            tmp = "Extern line# already exists! <br/>"
        End If

        Return tmp
    End Function
    Private Function SaveASNDetail(ByVal MyID As Integer) As String
        Dim tmp As String = "", Command As String = "", Saved As String = "", line As String = ""
        Dim EditOperation As Boolean = MyID <> 0
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Field_Facility")) _
        , externreceipt As String = HttpContext.Current.Request.Item("Field_ExternReceiptKey") _
        , receiptkey As String = HttpContext.Current.Request.Item("Field_ReceiptKey") _
        , Owner As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
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

        If Not EditOperation Then
            Command += "<AdvancedShipNotice>" & receiptkey & "</AdvancedShipNotice>"
        Else
            Command += "<ReceiptKey>" & receiptkey & "</ReceiptKey>"
        End If
        Command += "<ExternReceiptKey>" & externreceipt & "</ExternReceiptKey>"
        Command += "<StorerKey>" & Owner & "</StorerKey>"
        Command += "<AdvancedShipNoticeDetail> "

        If EditOperation Then
            Dim warehouse As String = Facility
            If LCase(warehouse.Substring(0, 6)) = "infor_" Then warehouse = warehouse.Substring(6, Facility.Length - 6)
            If LCase(warehouse).Contains("_") Then warehouse = warehouse.Split("_")(1)
            Dim sql As String = "select ReceiptLineNumber from " & warehouse & ".RECEIPTDETAIL where ReceiptKey= '" & receiptkey & "' and ExternLineNo = '" & exterline & "'"
            Dim ds As DataSet = (New SQLExec).Cursor(sql)
            If ds.Tables(0).Rows.Count > 0 Then
                line = ds.Tables(0).Rows(0)!ReceiptLineNumber
                Command += "<ReceiptLineNumber>" & line & "</ReceiptLineNumber>"
            Else
                tmp = "Error: cannot get receipt line number <br/>"
                Return tmp
            End If
        End If

        If Not EditOperation Then
            If Not String.IsNullOrEmpty(exterline) Then
                exterline = exterline.ToString.PadLeft(5, "0")
                Command += "<ExternLineNo>" & exterline & "</ExternLineNo>"
                Saved = "<ExternLineNo>" & exterline & "</ExternLineNo>"
            Else
                exterline = CommonMethods.getExternline(Facility, externreceipt, "receiptdetail", "EXTERNRECEIPTKEY")
                Command += "<ExternLineNo>" & exterline & "</ExternLineNo>"
                Saved = "<ExternLineNo>" & exterline & "</ExternLineNo>"
            End If
        Else
            Command += "<ExternLineNo>" & exterline & "</ExternLineNo>"
        End If

        If Not EditOperation Then
            If Not String.IsNullOrEmpty(Sku) Then
                Command += "<StorerKey>" & Owner & "</StorerKey><Sku>" & Sku & "</Sku>"
            Else
                tmp = "Item cannot be empty <br/>"
                Return tmp
            End If
        Else
            Command += "<StorerKey>" & Owner & "</StorerKey><Sku>" & Sku & "</Sku>"
        End If

        If Not String.IsNullOrEmpty(QtyExpected) Then
            Command += "<QtyExpected>" & QtyExpected & "</QtyExpected>"
        Else
            tmp = "Qty Expected cannot be empty <br/>"
            Return tmp
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
                    Return tmp
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
                    Return tmp
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
                    Return tmp
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
                    Return tmp
                End If
            End If
        Catch ex As Exception
        End Try

        Command += "</AdvancedShipNoticeDetail>"

        Dim exist As Integer = 0

        If Not EditOperation Then exist = CommonMethods.checkReceiptLineExist(Facility, receiptkey, exterline)

        If exist = 0 Then
            Try
                Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>AdvancedShipNotice</MessageType><Action>store</Action><Sender><User>" & CommonMethods.username & "</User><Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId>	</Sender><Recipient><SystemID>" & Facility & "</SystemID></Recipient></Head><Body><AdvancedShipNotice><AdvancedShipNoticeHeader>" & Command & "</AdvancedShipNoticeHeader></AdvancedShipNotice></Body></Message>"

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
                        Dim logmessage As String = ""
                        If Not EditOperation Then
                            Dim results As XmlNodeList = doc.SelectNodes("//*[local-name()='AdvancedShipNoticeDetail']")
                            Dim lineno As String = ""
                            For Each node As XmlNode In results
                                If node("ExternLineNo").InnerText.ToString = exterline Then
                                    lineno = node("ReceiptLineNumber").InnerText.ToString
                                    logmessage = CommonMethods.logger(Facility, "ReceiptDetail", receiptkey + "-" + lineno, HttpContext.Current.Session("userkey").ToString)
                                End If
                            Next
                        Else
                            logmessage = CommonMethods.logger(Facility, "ReceiptDetail", receiptkey + "-" + line, HttpContext.Current.Session("userkey").ToString)
                        End If
                        If Not String.IsNullOrEmpty(logmessage) Then
                            tmp = "Logging Error: " + logmessage + "<br />"
                            Dim logger As Logger = LogManager.GetCurrentClassLogger()
                            logger.Error(logmessage, "", "")
                        End If
                    End If
                End If
            Catch ex As Exception
                tmp = "Error: " & ex.Message & vbTab + ex.GetType.ToString & "<br/>"
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(ex, "", "")
            End Try
        Else
            tmp = "Extern line# already exists! <br/>"
        End If

        Return tmp
    End Function
    Private Function SaveSODetail(ByVal MyID As Integer) As String
        Dim tmp As String = "", Command As String = "", Saved As String = "", line As String = ""
        Dim EditOperation As Boolean = MyID <> 0
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Field_Facility")) _
        , externorder As String = HttpContext.Current.Request.Item("Field_ExternOrderKey") _
        , orderkey As String = HttpContext.Current.Request.Item("Field_OrderKey") _
        , Owner As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
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

        If Not EditOperation Then
            Command += "<ShipmentOrder>" & orderkey & "</ShipmentOrder>"
        Else
            Command += "<OrderKey>" & orderkey & "</OrderKey>"
        End If
        Command += "<ExternOrderKey>" & externorder & "</ExternOrderKey>"
        Command += "<StorerKey>" & Owner & "</StorerKey>"
        Command += "<ShipmentOrderDetail> "

        If EditOperation Then
            Dim warehouse As String = Facility
            If LCase(warehouse.Substring(0, 6)) = "infor_" Then warehouse = warehouse.Substring(6, Facility.Length - 6)
            If LCase(warehouse).Contains("_") Then warehouse = warehouse.Split("_")(1)
            Dim sql As String = "select OrderLineNumber from " & warehouse & ".ORDERDETAIL where OrderKey= '" & orderkey & "' and ExternLineNo = '" & exterline & "'"
            Dim ds As DataSet = (New SQLExec).Cursor(sql)
            If ds.Tables(0).Rows.Count > 0 Then
                line = ds.Tables(0).Rows(0)!OrderLineNumber
                Command += "<OrderLineNumber>" & line & "</OrderLineNumber>"
            Else
                tmp = "Error: cannot get order line number <br/>"
                Return tmp
            End If
        End If

        If Not EditOperation Then
            If Not String.IsNullOrEmpty(Sku) Then
                Command += "<StorerKey>" & Owner & "</StorerKey><Sku>" & Sku & "</Sku>"
            Else
                tmp = "Item cannot be empty <br/>"
                Return tmp
            End If
        Else
            Command += "<StorerKey>" & Owner & "</StorerKey><Sku>" & Sku & "</Sku>"
        End If

        If Not String.IsNullOrEmpty(OpenQty) Then
            Command += "<OpenQty>" & OpenQty & "</OpenQty>"
        Else
            tmp = "Open Qty cannot be empty <br/>"
            Return tmp
        End If

        If Not EditOperation Then
            If Not String.IsNullOrEmpty(exterline) Then
                exterline = exterline.ToString.PadLeft(5, "0")
                Command += "<ExternLineNo>" & exterline & "</ExternLineNo>"
                Saved = "<ExternLineNo>" & exterline & "</ExternLineNo>"
            Else
                exterline = CommonMethods.getExternline(Facility, externorder, "orderdetail", "EXTERNORDERKEY")
                Command += "<ExternLineNo>" & exterline & "</ExternLineNo>"
                Saved = "<ExternLineNo>" & exterline & "</ExternLineNo>"
            End If
        Else
            Command += "<ExternLineNo>" & exterline & "</ExternLineNo>"
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
                    Return tmp
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
                    Return tmp
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

        Dim exist As Integer = 0

        If Not EditOperation Then exist = CommonMethods.checkSOLineExist(Facility, orderkey, exterline)

        If exist = 0 Then
            Try
                Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>ShipmentOrder</MessageType><Action>store</Action><Sender><User>" & CommonMethods.username & "</User><Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId>	</Sender><Recipient><SystemID>" & Facility & "</SystemID></Recipient></Head><Body><ShipmentOrder><ShipmentOrderHeader>" & Command & "</ShipmentOrderHeader></ShipmentOrder></Body></Message>"
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
                    ElseIf Not soapResult.Contains(Saved) And Not EditOperation Then
                        tmp = "Receipt line could not be saved <br/>"
                    Else
                        Dim logmessage As String = ""
                        If Not EditOperation Then
                            Dim results As XmlNodeList = doc.SelectNodes("//*[local-name()='ShipmentOrderDetail']")
                            Dim lineno As String = ""
                            For Each node As XmlNode In results
                                If node("ExternLineNo").InnerText.ToString = exterline Then
                                    lineno = node("OrderLineNumber").InnerText.ToString
                                    logmessage = CommonMethods.logger(Facility, "OrderDetail", orderkey + "-" + lineno, HttpContext.Current.Session("userkey").ToString)
                                End If
                            Next
                        Else
                            logmessage = CommonMethods.logger(Facility, "OrderDetail", orderkey + "-" + line, HttpContext.Current.Session("userkey").ToString)
                        End If
                        If Not String.IsNullOrEmpty(logmessage) Then
                            tmp = "Logging Error: " + logmessage + "<br />"
                            Dim logger As Logger = LogManager.GetCurrentClassLogger()
                            logger.Error(logmessage, "", "")
                        End If
                    End If
                End If
            Catch ex As Exception
                tmp = "Error: " & ex.Message & vbTab + ex.GetType.ToString & "<br/>"
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(ex, "", "")
            End Try
        Else
            tmp = "Extern line# already exists! <br/>"
        End If

        Return tmp
    End Function
    Private Function SaveOrderManagementDetail(ByVal MyID As Integer) As String
        Dim tmp As String = "", CommandDetails As String = ""
        Dim IsValid As Boolean = True
        Dim EditOperation As Boolean = MyID <> 0
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Field_Facility")) _
        , externorder As String = HttpContext.Current.Request.Item("Field_ExternOrderKey") _
        , orderkey As String = HttpContext.Current.Request.Item("Field_OrderManagKey") _
        , Owner As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
        , Status As String = HttpContext.Current.Request.Item("Field_OrderManagStatus") _
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

        Dim cmdDetails As SqlCommand = New SqlCommand
        Dim cmdOracleDetails As OracleCommand = New OracleCommand

        If Not EditOperation Then
            CommandDetails += " insert into " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & " ORDERMANAGDETAIL "
            CommandDetails += " (WHSEID, EXTERNORDERKEY, ORDERMANAGKEY, orderkey, OrderLineNumber, ORDERMANAGLINENUMBER, "
            CommandDetails += "  StorerKey, ExternLineNo, Sku, OpenQty, PackKey, UOM, SUsr1, SUsr2, SUsr3, SUsr4, "
            CommandDetails += " Lottable01, Lottable02, Lottable03, Lottable04, Lottable05, Lottable06, "
            CommandDetails += " Lottable07, Lottable08, Lottable09, Lottable10, UNITPRICE, SUsr5) values "

            Dim linenb As String = ""
            Dim sql As String = ""
            If CommonMethods.dbtype = "sql" Then
                sql = "select  (RIGHT('00000' + CAST(ISNULL(max(ORDERMANAGLINENUMBER)+1,1) AS varchar(5)) , 5)) As LineNumber from dbo.ORDERMANAGDETAIL"
            Else
                sql = "select (SUBSTR(CONCAT('00000' , CAST(NVL(max(ORDERMANAGLINENUMBER)+1,1) AS nvarchar2(5))) , -5)) As LineNumber from SYSTEM.ORDERMANAGDETAIL"
            End If
            sql += " where WHSEID ='" & Facility & "' and OrderManagKey='" & orderkey & "' and ExternOrderKey='" & externorder & "' and StorerKey='" & Owner & "'"
            Dim ds As DataSet = (New SQLExec).Cursor(sql)
            If ds.Tables(0).Rows.Count > 0 Then
                linenb = ds.Tables(0).Rows(0)!LineNumber
            Else
                tmp = "Error: cannot get order line number <br/>"
                Return tmp
            End If

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

            If Status = "NOT CREATED in SCE" Then
                exterline = CommonMethods.getExternline(IIf(CommonMethods.dbtype <> "sql", "SYSTEM", "dbo"), externorder, "ordermanagdetail", "EXTERNORDERKEY")
            Else
                exterline = CommonMethods.getExternline(Facility, externorder, "ordermanagdetail", "EXTERNORDERKEY")
            End If
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
                    CommandDetails += " ,''"
                End If
            Catch ex As Exception
                CommandDetails += " ,''"
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
            If String.IsNullOrEmpty(OpenQty) Then
                IsValid = False
                tmp += "Open Qty cannot be empty <br/>"
            End If

            If CommonMethods.dbtype = "sql" Then
                CommandDetails += " OpenQty= @openqty, SUsr1= @udf1, SUsr2 = @udf2, SUsr3 = @udf3, SUsr4 = @udf4, SUsr5 = @currency , Lottable01= @lot1, Lottable02= @lot2, Lottable03=  @lot3, Lottable06=  @lot6, Lottable07=  @lot7, Lottable08=  @lot8,Lottable09=  @lot9, Lottable10=  @lot10 "
                cmdDetails.Parameters.AddWithValue("@openqty", OpenQty)
                cmdDetails.Parameters.AddWithValue("@udf1", UDF1Dtl)
                cmdDetails.Parameters.AddWithValue("@udf2", UDF2Dtl)
                cmdDetails.Parameters.AddWithValue("@udf3", UDF3Dtl)
                cmdDetails.Parameters.AddWithValue("@udf4", UDF4Dtl)
                cmdDetails.Parameters.AddWithValue("@currency", Currency)
                cmdDetails.Parameters.AddWithValue("@lot1", Lottable01)
                cmdDetails.Parameters.AddWithValue("@lot2", Lottable02)
                cmdDetails.Parameters.AddWithValue("@lot3", Lottable03)
                cmdDetails.Parameters.AddWithValue("@lot6", Lottable06)
                cmdDetails.Parameters.AddWithValue("@lot7", Lottable07)
                cmdDetails.Parameters.AddWithValue("@lot8", Lottable08)
                cmdDetails.Parameters.AddWithValue("@lot9", Lottable09)
                cmdDetails.Parameters.AddWithValue("@lot10", Lottable10)
            Else
                CommandDetails += " OpenQty= :openqty, SUsr1= :udf1, SUsr2 =:udf2, SUsr3 =:udf3, SUsr4 =:udf4, SUsr5 =:currency , Lottable01= :lot1, Lottable02= :lot2, Lottable03= :lot3, Lottable06= :lot6, Lottable07= :lot7, Lottable08= :lot8,Lottable09= :lot9, Lottable10= :lot10  "
                cmdOracleDetails.Parameters.Add(New OracleParameter("openqty", OpenQty))
                cmdOracleDetails.Parameters.Add(New OracleParameter("udf1", UDF1Dtl))
                cmdOracleDetails.Parameters.Add(New OracleParameter("udf2", UDF2Dtl))
                cmdOracleDetails.Parameters.Add(New OracleParameter("udf3", UDF3Dtl))
                cmdOracleDetails.Parameters.Add(New OracleParameter("udf4", UDF4Dtl))
                cmdOracleDetails.Parameters.Add(New OracleParameter("currency", Currency))
                cmdOracleDetails.Parameters.Add(New OracleParameter("lot1", Lottable01))
                cmdOracleDetails.Parameters.Add(New OracleParameter("lot2", Lottable02))
                cmdOracleDetails.Parameters.Add(New OracleParameter("lot3", Lottable03))
                cmdOracleDetails.Parameters.Add(New OracleParameter("lot6", Lottable06))
                cmdOracleDetails.Parameters.Add(New OracleParameter("lot7", Lottable07))
                cmdOracleDetails.Parameters.Add(New OracleParameter("lot8", Lottable08))
                cmdOracleDetails.Parameters.Add(New OracleParameter("lot9", Lottable09))
                cmdOracleDetails.Parameters.Add(New OracleParameter("lot10", Lottable10))
            End If

            If Not String.IsNullOrEmpty(PackKey) Then
                If CommonMethods.dbtype = "sql" Then
                    CommandDetails += " , PackKey= @pack"
                    cmdDetails.Parameters.AddWithValue("@pack", PackKey)
                Else
                    CommandDetails += " , PackKey= :pack"
                    cmdOracleDetails.Parameters.Add(New OracleParameter("pack", PackKey))
                End If
            End If

            If Not String.IsNullOrEmpty(UOM) Then
                If CommonMethods.dbtype = "sql" Then
                    CommandDetails += " , UOM= @uom"
                    cmdDetails.Parameters.AddWithValue("@uom", UOM)
                Else
                    CommandDetails += " , UOM= :uom"
                    cmdOracleDetails.Parameters.Add(New OracleParameter("uom", UOM))
                End If
            End If

            If Not String.IsNullOrEmpty(Lottable04) Then
                Dim datetime As DateTime
                If DateTime.TryParseExact(Lottable04, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                    Dim datetime2 As DateTime = DateTime.ParseExact(Lottable04, "MM/dd/yyyy", Nothing)
                    If CommonMethods.dbtype = "sql" Then
                        CommandDetails += " , Lottable04= @lot4"
                    Else
                        CommandDetails += " , Lottable04= :lot4"
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
            End If

            If Not String.IsNullOrEmpty(Lottable05) Then
                Dim datetime As DateTime
                If DateTime.TryParseExact(Lottable05, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                    Dim datetime2 As DateTime = DateTime.ParseExact(Lottable05, "MM/dd/yyyy", Nothing)
                    If CommonMethods.dbtype = "sql" Then
                        CommandDetails += " , Lottable05= @lot5"
                    Else
                        CommandDetails += " , Lottable05= :lot5"
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
            End If

            If Not String.IsNullOrEmpty(Price) Then
                If CommonMethods.dbtype = "sql" Then
                    CommandDetails += " , unitprice= @price"
                    cmdDetails.Parameters.AddWithValue("@price", Price)
                Else
                    CommandDetails += " , unitprice= :price"
                    cmdOracleDetails.Parameters.Add(New OracleParameter("price", Price))
                End If
            End If

        End If

        If IsValid Then
            Dim exist As Integer = 0
            If Not EditOperation Then exist = CommonMethods.checkOrderManagementLineExist(orderkey, externorder, Owner, Facility, exterline)
            If exist = 0 Then
                If CommonMethods.dbtype = "sql" Then
                    Try
                        Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                        conn.Open()
                        If Not EditOperation Then
                            cmdDetails.CommandText = CommandDetails
                        Else
                            Dim update As String = "update  dbo.ORDERMANAGDETAIL set " & CommandDetails & "  where ORDERMANAGKEY = @okey and ExternOrderKey = @externokey and ORDERMANAGLINENUMBER = @externlineno "
                            cmdDetails.Parameters.AddWithValue("@okey", orderkey)
                            cmdDetails.Parameters.AddWithValue("@externokey", externorder)
                            cmdDetails.Parameters.AddWithValue("@externlineno", exterline)
                            cmdDetails.CommandText = update
                        End If
                        cmdDetails.Connection = conn
                        cmdDetails.ExecuteNonQuery()
                        conn.Close()
                    Catch ex As Exception
                        IsValid = False
                        tmp += "Error: " & ex.Message & vbTab + ex.GetType.ToString & "<br/>"
                        Dim logger As Logger = LogManager.GetCurrentClassLogger()
                        logger.Error(ex, "", "")
                    End Try
                Else
                    Try
                        Dim conn As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                        conn.Open()
                        If Not EditOperation Then
                            cmdOracleDetails.CommandText = CommandDetails
                        Else
                            Dim update As String = "update SYSTEM.ORDERMANAGDETAIL set " & CommandDetails & "  where ORDERMANAGKEY = :okey and ExternOrderKey = :externokey and ORDERMANAGLINENUMBER = :externlineno "
                            cmdOracleDetails.Parameters.Add(New OracleParameter("okey", orderkey))
                            cmdOracleDetails.Parameters.Add(New OracleParameter("externokey", externorder))
                            cmdOracleDetails.Parameters.Add(New OracleParameter("externlineno", exterline))
                            cmdOracleDetails.CommandText = update
                        End If
                        cmdOracleDetails.Connection = conn
                        cmdOracleDetails.ExecuteNonQuery()
                        conn.Close()
                    Catch ex As Exception
                        IsValid = False
                        tmp += "Error: " & ex.Message & vbTab + ex.GetType.ToString & "<br/>"
                        Dim logger As Logger = LogManager.GetCurrentClassLogger()
                        logger.Error(ex, "", "")
                    End Try
                End If
            Else
                tmp = "Extern line# already exists! <br/>"
                Return tmp
            End If
        End If
        Return tmp
    End Function

    Private Function GetPurchaseOrderStatus() As String
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Field_Facility")) _
        , pokey As String = HttpContext.Current.Request.Item("Field_POKey") _
        , Status As String = ""

        If LCase(Facility.Substring(0, 6)) = "infor_" Then Facility = Facility.Substring(6, Facility.Length - 6)
        If LCase(Facility).Contains("_") Then Facility = Facility.Split("_")(1)

        Dim sql As String = "select DESCRIPTION as Status from " & Facility & ".codelkup where listname='POSTATUS' and code=(select Status from " & Facility & ".po where POKey= '" & pokey & "' ) "
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then Status = ds.Tables(0).Rows(0)!Status
        Return Status
    End Function
    Private Function GetASNStatus() As String
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Field_Facility")) _
        , receiptkey As String = HttpContext.Current.Request.Item("Field_ReceiptKey") _
        , Status As String = ""

        If LCase(Facility.Substring(0, 6)) = "infor_" Then Facility = Facility.Substring(6, Facility.Length - 6)
        If LCase(Facility).Contains("_") Then Facility = Facility.Split("_")(1)

        Dim sql As String = "select DESCRIPTION as Status from " & Facility & ".codelkup where listname='RECSTATUS' and code=(select Status from " & Facility & ".receipt where ReceiptKey= '" & receiptkey & "' ) "
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then Status = ds.Tables(0).Rows(0)!Status
        Return Status
    End Function
    Private Function GetSOStatus() As String
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Field_Facility")) _
        , orderkey As String = HttpContext.Current.Request.Item("Field_OrderKey") _
        , Status As String = ""

        If LCase(Facility.Substring(0, 6)) = "infor_" Then Facility = Facility.Substring(6, Facility.Length - 6)
        If LCase(Facility).Contains("_") Then Facility = Facility.Split("_")(1)

        Dim sql As String = "select DESCRIPTION as Status from " & Facility & ".orderstatussetup where code=(select Status from " & Facility & ".orders where OrderKey= '" & orderkey & "') "
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then Status = ds.Tables(0).Rows(0)!Status
        Return Status
    End Function
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class