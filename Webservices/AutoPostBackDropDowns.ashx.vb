Imports System.IO
Imports System.Xml
Imports Newtonsoft.Json
Imports NLog

Public Class AutoPostBackDropDowns
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub AutoPostBack(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim SearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim MyValue As String = HttpContext.Current.Request.Item("MyValue")

        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)
        Using writer As JsonWriter = New JsonTextWriter(sw)
            writer.Formatting = Newtonsoft.Json.Formatting.Indented
            writer.WriteStartObject()

            writer.WritePropertyName("DropDownFields")
            If SearchTable = "SKUCATALOGUE" Then
                writer.WriteValue(PopulateSkuCat(MyValue))
            ElseIf SearchTable = "Warehouse_PO" Then
                writer.WriteValue(PopulatePO(MyValue))
            ElseIf SearchTable = "Warehouse_ASN" Then
                writer.WriteValue(PopulateASN(MyValue))
            ElseIf SearchTable = "Warehouse_SO" Or SearchTable = "Warehouse_OrderManagement" Then
                If SearchTable = "Warehouse_OrderManagement" Then
                    writer.WriteValue(PopulateSO(MyValue).Replace("OrderType", "Type"))
                Else
                    writer.WriteValue(PopulateSO(MyValue))
                End If
            Else
                writer.WriteValue("")
            End If

            writer.WritePropertyName("DropDownDetailsFields")
            If SearchTable = "Warehouse_ASN" Then
                writer.WriteValue(PopulateASNDetails(MyValue))
            ElseIf SearchTable = "Warehouse_SO" Or SearchTable = "Warehouse_OrderManagement" Then
                writer.WriteValue(PopulateSODetails(MyValue))
            Else
                writer.WriteValue("")
            End If

            writer.WriteEndObject()
        End Using
        context.Response.Write(sw)
        context.Response.End()
    End Sub

    Private Function PopulateSkuCat(ByVal MyValue As String) As String
        Dim DropDownFields As String = "Sku:::"
        If Trim(MyValue) <> "" Then

            Try
                Dim Xml As String = "<Message>	<Head>		<MessageID>0000000003</MessageID>		<MessageType>ItemMaster</MessageType>		<Action>list</Action>		<Sender>			<User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password>	<SystemID>MOVEX</SystemID>		<TenantId>INFOR</TenantId>		</Sender>		<Recipient>			<SystemID>" & CommonMethods.getEnterpriseDBName() & "</SystemID>		</Recipient>	</Head>	<Body>		<ItemMaster> 		<Item>	<StorerKey>" & MyValue & "	</StorerKey></Item></ItemMaster>	</Body></Message>"
                Dim soapResult As String = CommonMethods.sendwebRequest(Xml)
                If String.IsNullOrEmpty(soapResult) Then
                    DropDownFields = "Error: Unable to connect to webservice, kindly check the logs"
                Else
                    Dim dsresult As DataSet = New DataSet
                    Dim doc As XmlDocument = New XmlDocument
                    doc.LoadXml(soapResult)

                    Dim xmlFile As XmlReader = XmlReader.Create(New StringReader(soapResult), New XmlReaderSettings)

                    Dim results As XmlNodeList = doc.SelectNodes("//*[local-name()='Item']")
                    For Each node As XmlNode In results
                        If Not node("Sku").IsEmpty Then
                            DropDownFields += node("Sku").InnerText.ToString & ","
                        End If
                    Next
                    If DropDownFields.EndsWith(",") Then DropDownFields = DropDownFields.Remove(DropDownFields.Length - 1)
                End If
            Catch exp As Exception
                DropDownFields = "Error: " & exp.Message & vbTab + exp.GetType.ToString
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
            End Try
        End If
        Return DropDownFields
    End Function
    Private Function PopulatePO(ByVal MyValue As String) As String
        Dim DropDownFields As String = ""
        If Trim(MyValue) <> "" Then
            Dim warehouselevel As String = CommonMethods.getFacilityDBName(MyValue)
            Dim ddtype As DataTable = CommonMethods.getCodeDD(warehouselevel, "codelkup", "POTYPE")
            DropDownFields += "POType:::"
            For i = 0 To ddtype.Rows.Count - 1
                With ddtype.Rows(i)
                    DropDownFields += IIf(i <> 0, ",", "") & !DESCRIPTION
                End With
            Next

            Dim sql As String = "", AndFilter As String = ""


            If String.IsNullOrEmpty(warehouselevel) Then
                warehouselevel = "enterprise"
            ElseIf LCase(warehouselevel.Substring(0, 6)) = "infor_" Then
                warehouselevel = warehouselevel.Substring(6, warehouselevel.Length - 6)
                warehouselevel = warehouselevel.Split("_")(1)
            Else
                warehouselevel = warehouselevel.Split("_")(1)
            End If

            For i = 0 To 1
                Dim values As String()
                If i = 0 Then
                    values = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
                Else
                    values = CommonMethods.getSupplierPerUser(HttpContext.Current.Session("userkey").ToString)
                End If
                If values IsNot Nothing Then
                    Dim valuesstr As String = String.Join("','", values)
                    valuesstr = "'" & valuesstr & "'"
                    If Not UCase(valuesstr).Contains("'ALL'") Then
                        AndFilter = " and StorerKey IN (" + valuesstr + ") "
                    End If
                End If
                sql += "select StorerKey from " & warehouselevel & ".storer where type= " & IIf(i = 0, "1", "5") & " " & AndFilter
            Next
            Dim ds As DataSet = (New SQLExec).Cursor(sql)

            If ds IsNot Nothing Then
                DropDownFields += ";;;StorerKey:::"
                For i = 0 To ds.Tables(0).Rows.Count - 1
                    With ds.Tables(0).Rows(i)
                        DropDownFields += IIf(i <> 0, ",", "") & !StorerKey
                    End With
                Next

                DropDownFields += ";;;SellerName:::"
                For i = 0 To ds.Tables(1).Rows.Count - 1
                    With ds.Tables(1).Rows(i)
                        DropDownFields += IIf(i <> 0, ",", "") & !StorerKey
                    End With
                Next
            End If
        Else
            DropDownFields += "POType:::;;;StorerKey:::;;;SellerName:::"
        End If

        Return DropDownFields
    End Function
    Private Function PopulateASN(ByVal MyValue As String) As String
        Dim DropDownFields As String = ""
        If Trim(MyValue) <> "" Then
            Dim warehouselevel As String = CommonMethods.getFacilityDBName(MyValue)
            Dim ddtype As DataTable = CommonMethods.getCodeDD(warehouselevel, "codelkup", "RECEIPTYPE")
            DropDownFields += "ReceiptType:::"
            For i = 0 To ddtype.Rows.Count - 1
                With ddtype.Rows(i)
                    DropDownFields += IIf(i <> 0, ",", "") & !DESCRIPTION
                End With
            Next

            Dim sql As String = "", AndFilter As String = ""


            If String.IsNullOrEmpty(warehouselevel) Then
                warehouselevel = "enterprise"
            ElseIf LCase(warehouselevel.Substring(0, 6)) = "infor_" Then
                warehouselevel = warehouselevel.Substring(6, warehouselevel.Length - 6)
                warehouselevel = warehouselevel.Split("_")(1)
            Else
                warehouselevel = warehouselevel.Split("_")(1)
            End If

            For i = 0 To 1
                Dim values As String() = Nothing
                If i = 0 Then values = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
                If values IsNot Nothing Then
                    Dim valuesstr As String = String.Join("','", values)
                    valuesstr = "'" & valuesstr & "'"
                    If Not UCase(valuesstr).Contains("'ALL'") Then
                        AndFilter = " and STORERKEY IN (" + valuesstr + ") "
                    End If
                End If
                sql += "select StorerKey from " & warehouselevel & ".storer where type= " & IIf(i = 0, "1", "3") & " " & AndFilter
                AndFilter = ""
            Next
            Dim ds As DataSet = (New SQLExec).Cursor(sql)

            If ds IsNot Nothing Then
                DropDownFields += ";;;StorerKey:::"
                For i = 0 To ds.Tables(0).Rows.Count - 1
                    With ds.Tables(0).Rows(i)
                        DropDownFields += IIf(i <> 0, ",", "") & !StorerKey
                    End With
                Next

                DropDownFields += ";;;CarrierKey:::"
                For i = 0 To ds.Tables(1).Rows.Count - 1
                    With ds.Tables(1).Rows(i)
                        DropDownFields += IIf(i <> 0, ",", "") & !StorerKey
                    End With
                Next
            End If
        Else
            DropDownFields += "ReceiptType:::;;;StorerKey:::;;;CarrierKey:::"
        End If

        Return DropDownFields
    End Function
    Private Function PopulateSO(ByVal MyValue As String) As String
        Dim DropDownFields As String = ""
        If Trim(MyValue) <> "" Then
            Dim warehouselevel As String = CommonMethods.getFacilityDBName(MyValue)
            Dim ddtype As DataTable = CommonMethods.getCodeDD(warehouselevel, "codelkup", "ORDERTYPE")
            DropDownFields += "OrderType:::"
            For i = 0 To ddtype.Rows.Count - 1
                With ddtype.Rows(i)
                    DropDownFields += IIf(i <> 0, ",", "") & !DESCRIPTION
                End With
            Next

            Dim sql As String = "", AndFilter As String = ""


            If String.IsNullOrEmpty(warehouselevel) Then
                warehouselevel = "enterprise"
            ElseIf LCase(warehouselevel.Substring(0, 6)) = "infor_" Then
                warehouselevel = warehouselevel.Substring(6, warehouselevel.Length - 6)
                warehouselevel = warehouselevel.Split("_")(1)
            Else
                warehouselevel = warehouselevel.Split("_")(1)
            End If

            For i = 0 To 1
                Dim values As String() = Nothing
                If i = 0 Then
                    values = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
                Else
                    values = CommonMethods.getConsigneePerUser(HttpContext.Current.Session("userkey").ToString)
                End If
                If values IsNot Nothing Then
                    Dim valuesstr As String = String.Join("','", values)
                    valuesstr = "'" & valuesstr & "'"
                    If Not UCase(valuesstr).Contains("'ALL'") Then
                        AndFilter = " and " & IIf(i = 0, "StorerKey", "ConsigneeKey") & " IN (" + valuesstr + ") "
                    End If
                End If
                sql += "select StorerKey from " & warehouselevel & ".storer where type= " & IIf(i = 0, "1", "2") & " " & AndFilter
            Next
            Dim ds As DataSet = (New SQLExec).Cursor(sql)

            If ds IsNot Nothing Then
                DropDownFields += ";;;StorerKey:::"
                For i = 0 To ds.Tables(0).Rows.Count - 1
                    With ds.Tables(0).Rows(i)
                        DropDownFields += IIf(i <> 0, ",", "") & !StorerKey
                    End With
                Next

                DropDownFields += ";;;ConsigneeKey:::"
                For i = 0 To ds.Tables(1).Rows.Count - 1
                    With ds.Tables(1).Rows(i)
                        DropDownFields += IIf(i <> 0, ",", "") & !StorerKey
                    End With
                Next
            End If
        Else
            DropDownFields += "OrderType:::;;;StorerKey:::;;;ConsigneeKey:::"
        End If

        Return DropDownFields
    End Function
    Private Function PopulateASNDetails(ByVal MyValue As String) As String
        Dim DropDownDetailsFields As String = ""
        If Trim(MyValue) <> "" Then
            Dim sql As String = ""
            Dim warehouselevel As String = CommonMethods.getFacilityDBName(MyValue)

            If String.IsNullOrEmpty(warehouselevel) Then
                warehouselevel = "enterprise"
            ElseIf LCase(warehouselevel.Substring(0, 6)) = "infor_" Then
                warehouselevel = warehouselevel.Substring(6, warehouselevel.Length - 6)
                warehouselevel = warehouselevel.Split("_")(1)
            Else
                warehouselevel = warehouselevel.Split("_")(1)
            End If

            For i = 0 To 1
                sql += " select top 1000 " & IIf(i = 0, "PackKey", "Loc") & " from " & warehouselevel & IIf(i = 0, ".Pack", ".Loc")
            Next
            Dim ds As DataSet = (New SQLExec).Cursor(sql)

            If ds IsNot Nothing Then
                DropDownDetailsFields += "PackKey:::"
                For i = 0 To ds.Tables(0).Rows.Count - 1
                    With ds.Tables(0).Rows(i)
                        DropDownDetailsFields += IIf(i <> 0, ",", "") & !PackKey
                    End With
                Next

                DropDownDetailsFields += ";;;ToLoc:::"
                For i = 0 To ds.Tables(1).Rows.Count - 1
                    With ds.Tables(1).Rows(i)
                        DropDownDetailsFields += IIf(i <> 0, ",", "") & !Loc
                    End With
                Next
            End If
        Else
            DropDownDetailsFields += "PackKey:::;;;ToLoc:::"
        End If

        Return DropDownDetailsFields
    End Function
    Private Function PopulateSODetails(ByVal MyValue As String) As String
        Dim DropDownDetailsFields As String = ""
        If Trim(MyValue) <> "" Then
            Dim sql As String = ""
            Dim warehouselevel As String = CommonMethods.getFacilityDBName(MyValue)

            If String.IsNullOrEmpty(warehouselevel) Then
                warehouselevel = "enterprise"
            ElseIf LCase(warehouselevel.Substring(0, 6)) = "infor_" Then
                warehouselevel = warehouselevel.Substring(6, warehouselevel.Length - 6)
                warehouselevel = warehouselevel.Split("_")(1)
            Else
                warehouselevel = warehouselevel.Split("_")(1)
            End If

            sql += " select top 1000 PackKey from " & warehouselevel & ".Pack"
            Dim ds As DataSet = (New SQLExec).Cursor(sql)

            If ds IsNot Nothing Then
                DropDownDetailsFields += "PackKey:::"
                For i = 0 To ds.Tables(0).Rows.Count - 1
                    With ds.Tables(0).Rows(i)
                        DropDownDetailsFields += IIf(i <> 0, ",", "") & !PackKey
                    End With
                Next
            End If
        Else
            DropDownDetailsFields += "PackKey:::"
        End If

        Return DropDownDetailsFields
    End Function
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class