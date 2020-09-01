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
            Dim sql As String = "Select top 100 Sku, Descr from enterprise.sku where StorerKey ='" & MyValue & "'"
            Dim ds As DataSet = (New SQLExec).Cursor(sql)
            If ds IsNot Nothing Then
                For i = 0 To ds.Tables(0).Rows.Count - 1
                    With ds.Tables(0).Rows(i)
                        DropDownFields += IIf(i <> 0, ",", "") & !Sku & "~~~" & !Descr
                    End With
                Next
            End If
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
                sql += "select StorerKey, Company from " & warehouselevel & ".storer where type= " & IIf(i = 0, "1", "5") & " " & AndFilter
            Next
            Dim ds As DataSet = (New SQLExec).Cursor(sql)

            If ds IsNot Nothing Then
                DropDownFields += ";;;StorerKey:::"
                For i = 0 To ds.Tables(0).Rows.Count - 1
                    With ds.Tables(0).Rows(i)
                        DropDownFields += IIf(i <> 0, ",", "") & !StorerKey & "~~~" & !Company
                    End With
                Next

                DropDownFields += ";;;SellerName:::"
                For i = 0 To ds.Tables(1).Rows.Count - 1
                    With ds.Tables(1).Rows(i)
                        DropDownFields += IIf(i <> 0, ",", "") & !StorerKey & "~~~" & !Company
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
                sql += "select StorerKey, Company from " & warehouselevel & ".storer where type= " & IIf(i = 0, "1", "3") & " " & AndFilter
                AndFilter = ""
            Next
            Dim ds As DataSet = (New SQLExec).Cursor(sql)

            If ds IsNot Nothing Then
                DropDownFields += ";;;StorerKey:::"
                For i = 0 To ds.Tables(0).Rows.Count - 1
                    With ds.Tables(0).Rows(i)
                        DropDownFields += IIf(i <> 0, ",", "") & !StorerKey & "~~~" & !Company
                    End With
                Next

                DropDownFields += ";;;CarrierKey:::"
                For i = 0 To ds.Tables(1).Rows.Count - 1
                    With ds.Tables(1).Rows(i)
                        DropDownFields += IIf(i <> 0, ",", "") & !StorerKey & "~~~" & !Company
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
                        AndFilter = " and STORERKEY IN (" + valuesstr + ") "
					Else
                        'Mohamad Rmeity - AndFilter was taking the previous value
                        AndFilter = " "
                    End If
                End If
                sql += "select StorerKey,Company from " & warehouselevel & ".storer where type= " & IIf(i = 0, "1", "2") & " " & AndFilter
            Next
            Dim ds As DataSet = (New SQLExec).Cursor(sql)

            If ds IsNot Nothing Then
                DropDownFields += ";;;StorerKey:::"
                For i = 0 To ds.Tables(0).Rows.Count - 1
                    With ds.Tables(0).Rows(i)
                        DropDownFields += IIf(i <> 0, ",", "") & !StorerKey & "~~~" & !Company
                    End With
                Next

                DropDownFields += ";;;ConsigneeKey:::"
                For i = 0 To ds.Tables(1).Rows.Count - 1
                    With ds.Tables(1).Rows(i)
                        DropDownFields += IIf(i <> 0, ",", "") & !StorerKey & "~~~" & !Company
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
                sql += " select top 1000 " & IIf(i = 0, "PackKey, PackDescr", "Loc") & " from " & warehouselevel & IIf(i = 0, ".Pack", ".Loc")
            Next
            Dim ds As DataSet = (New SQLExec).Cursor(sql)

            If ds IsNot Nothing Then
                DropDownDetailsFields += "PackKey:::"
                For i = 0 To ds.Tables(0).Rows.Count - 1
                    With ds.Tables(0).Rows(i)
                        DropDownDetailsFields += IIf(i <> 0, ",", "") & !PackKey & "~~~" & !PackDescr
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

            sql += " select top 1000 PackKey,PackDescr from " & warehouselevel & ".Pack"
            Dim ds As DataSet = (New SQLExec).Cursor(sql)

            If ds IsNot Nothing Then
                DropDownDetailsFields += "PackKey:::"
                For i = 0 To ds.Tables(0).Rows.Count - 1
                    With ds.Tables(0).Rows(i)
                        DropDownDetailsFields += IIf(i <> 0, ",", "") & !PackKey & "~~~" & !PackDescr
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