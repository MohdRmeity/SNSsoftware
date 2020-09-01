Imports System.Globalization
Imports System.IO
Imports Newtonsoft.Json

Public Class GetItemsDetails
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub GetMore(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim tb As SQLExec = New SQLExec
        Dim SearchQuery As String = HttpContext.Current.Request.Item("SearchQuery")
        Dim SearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Facility"))
        Dim Key As String = HttpContext.Current.Request.Item("Key")
        Dim StorerKey As String = HttpContext.Current.Request.Item("StorerKey")
        Dim SortBy As String = HttpContext.Current.Request.Item("SortBy")
        Dim AndFilter As String = ""
        Dim SQL As String = " set dateformat mdy select * from "

        If SearchTable <> "Warehouse_OrderManagement" And SearchTable <> "Warehouse_OrderTracking" Then
            If Facility = "" Then
                Facility = HttpContext.Current.Request.UrlReferrer.Query.Substring(HttpContext.Current.Request.UrlReferrer.Query.IndexOf("=") + 1).Split("&")(0)
            End If
            Dim warehouselevel As String = ""
            If LCase(Facility.Substring(0, 6)) = "infor_" Then
                warehouselevel = Facility.Substring(6, Facility.Length - 6)
            Else
                warehouselevel = Facility
            End If
            warehouselevel = warehouselevel.Split("_")(1)
            SQL += warehouselevel & "."
            If SearchTable = "Warehouse_PO" Then
                SQL += "PODETAIL"
                AndFilter = "POKey"
            ElseIf SearchTable = "Warehouse_ASN" Then
                SQL += "RECEIPTDETAIL"
                AndFilter = "ReceiptKey"
            ElseIf SearchTable = "Warehouse_SO" Then
                SQL += "ORDERDETAIL"
                AndFilter = "OrderKey"
            End If
            SQL += " where 1=1 and " & AndFilter & " = '" & Key & "'"
        Else
            If SearchTable = "Warehouse_OrderManagement" Then
                SQL += IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAGDETAIL"
                AndFilter = "OrderKey"
                SQL += " where 1=1 and " & AndFilter & " = '" & Key & "'"
            Else
                GetOrderTrackingDetailsQuery(SQL, Key, StorerKey)
                SortBy = "OrderLineNumber asc"
            End If
        End If


        SearchItem(SearchQuery, SearchTable, SQL)
        SQL += " order by " & SortBy

        Dim DS As DataSet = tb.Cursor(SQL)
        Dim OBJTable As DataTable = DS.Tables(0)

        Dim MyRecords As String = ""
        If Not OBJTable Is Nothing Then
            If SearchTable = "Warehouse_PO" Then
                GetPurchaseOrderRecordsDetails(OBJTable, MyRecords, Facility)
            ElseIf SearchTable = "Warehouse_ASN" Then
                GetASNRecordsDetails(OBJTable, MyRecords, Facility)
            ElseIf SearchTable = "Warehouse_SO" Then
                GetSORecordsDetails(OBJTable, MyRecords, Facility)
            ElseIf SearchTable = "Warehouse_OrderManagement" Then
                GetOrderManagementRecordsDetails(OBJTable, MyRecords)
            ElseIf SearchTable = "Warehouse_OrderTracking" Then
                GetOrderTrackingRecordsDetails(OBJTable, MyRecords)
            End If
        End If

        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)
        Using writer As JsonWriter = New JsonTextWriter(sw)
            writer.Formatting = Formatting.Indented
            writer.WriteStartObject()

            writer.WritePropertyName("Records")
            writer.WriteValue(MyRecords)

            writer.WriteEndObject()
        End Using
        context.Response.Write(sw)
        context.Response.End()
    End Sub

    'SeachItem
    Private Sub SearchItem(ByVal SearchQuery As String, ByVal SearchTable As String, ByRef Sql As String)
        Dim MySearchTerms() As String = Split(SearchQuery, ",")

        For i = 0 To MySearchTerms.Length - 1
            Dim MySearchInsideTerms() As String = Split(MySearchTerms(i), ":")

            If MySearchInsideTerms.Length = 2 Then

                If MySearchInsideTerms(0) = "Facility" And SearchTable = "USERCONTROL" Then
                    Sql += " And UserKey in (select UserKey from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & " USERCONTROLFACILITY where Facility in (select DB_Name from wmsadmin.pl_db where isActive='1' and db_enterprise='0' and DB_ALIAS "
                ElseIf LCase(MySearchInsideTerms(0)).Contains("cast") Then
                    Sql += "and ( " & MySearchInsideTerms(0)
                Else
                    Sql += " and " & MySearchInsideTerms(0)
                End If

                If MySearchInsideTerms(1).Contains("%") Then
                    Sql += " Like N'" & MySearchInsideTerms(1) & "'"
                ElseIf MySearchInsideTerms(1).Contains("<") And Not MySearchInsideTerms(1).Contains("<=") Then
                    Sql += " " & MySearchInsideTerms(1).Insert(MySearchInsideTerms(1).IndexOf("<") + 1, "'") & "'"
                ElseIf MySearchInsideTerms(1).Contains(">") And Not MySearchInsideTerms(1).Contains(">=") Then
                    Sql += " " & MySearchInsideTerms(1).Insert(MySearchInsideTerms(1).IndexOf(">") + 1, "'") & "'"
                ElseIf MySearchInsideTerms(1).Contains(">=") Then
                    Sql += " " & MySearchInsideTerms(1).Insert(MySearchInsideTerms(1).IndexOf(">=") + 2, "'") & "'"
                ElseIf MySearchInsideTerms(1).Contains("<=") Then
                    Sql += " " & MySearchInsideTerms(1).Insert(MySearchInsideTerms(1).IndexOf("<=") + 2, "'") & "'"
                ElseIf LCase(MySearchInsideTerms(1)).Contains("between ") Then
                    Sql += " " & MySearchInsideTerms(1)
                ElseIf LCase(MySearchInsideTerms(1)).Contains("today") Then
                    Sql += " = convert(date,getdate()) "
                ElseIf LCase(MySearchInsideTerms(1)).Contains(" and ") Then
                    Dim MySearchAndTerms() As String = Split(LCase(MySearchInsideTerms(1)), " and ")
                    Sql += " in (''"
                    For k = 0 To MySearchAndTerms.Length - 1
                        Sql += ",'" & MySearchAndTerms(k) & "'"
                    Next
                    Sql += " )"
                ElseIf LCase(MySearchInsideTerms(1)).Contains(" or ") Then
                    Dim MySearchAndTerms() As String = Split(LCase(MySearchInsideTerms(1)), " or ")
                    Sql += " in (''"
                    For k = 0 To MySearchAndTerms.Length - 1
                        Sql += ",'" & MySearchAndTerms(k) & "'"
                    Next
                    Sql += " )"
                ElseIf LCase(MySearchInsideTerms(1)).Contains(";") Then
                    Dim MySearchAndTerms() As String = Split(LCase(MySearchInsideTerms(1)), ";")
                    Sql += " in ("
                    For k = 0 To MySearchAndTerms.Length - 1
                        Sql += IIf(k <> 0, ",", "") & "'" & MySearchAndTerms(k).Replace(" ", "") & "'"
                    Next
                    Sql += " )"
                ElseIf LCase(MySearchInsideTerms(0)).Contains("cast") Then
                    Sql += " Like N'%" & MySearchInsideTerms(1) & "%'"
                    Dim MyDateTime As DateTime
                    If DateTime.TryParseExact(MySearchInsideTerms(1), CommonMethods.dformat, CultureInfo.CurrentCulture, DateTimeStyles.None, MyDateTime) Then
                        Sql += " Or " & MySearchInsideTerms(0) & " = CAST ('" & MySearchInsideTerms(1) & "' as Date) "
                    End If
                Else
                    Sql += " Like N'%" & MySearchInsideTerms(1) & "%'"
                End If

                If MySearchInsideTerms(0) = "Facility" And SearchTable = "USERCONTROL" Then
                    Sql += " ))"
                ElseIf LCase(MySearchInsideTerms(0)).Contains("cast") Then
                    Sql += " )"
                End If
            End If
        Next
    End Sub
    'Records
    Private Sub GetPurchaseOrderRecordsDetails(ByVal OBJTable As DataTable, ByRef MyRecords As String, ByVal Facility As String)
        Dim measureunit As Double
        For i = 0 To OBJTable.Rows.Count - 1
            measureunit = 1
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell selectAllWidth'>"
                MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrd' type='checkbox' " & IIf(LCase(!POKey).ToString = "", "disabled", "") & " id='ChkSelectDtl" & i & "' data-id='" & !SerialKey & "' />"
                MyRecords += "                        <label for='ChkSelectDtl" & i & "'><span class='CheckBoxStyle'></span></label>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                        <div class='editStyle' data-id='" & !SerialKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='1'>"
                MyRecords += "                        " & !ExternLineNo
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !Sku
                MyRecords += "                    </td>"
                measureunit = CommonMethods.getUomMeasure(Facility, !PackKey.ToString, !UOM.ToString)
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & (Double.Parse(!QtyOrdered.ToString) / measureunit).ToString
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & (Double.Parse(!QtyReceived.ToString) / measureunit).ToString
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetASNRecordsDetails(ByVal OBJTable As DataTable, ByRef MyRecords As String, ByVal Facility As String)
        Dim Lottable04 As String, Lottable05 As String, Lottable11 As String, Lottable12 As String, measureunit As Double
        For i = 0 To OBJTable.Rows.Count - 1
            Lottable04 = ""
            Lottable05 = ""
            Lottable11 = ""
            Lottable12 = ""
            measureunit = 1
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell selectAllWidth'>"
                MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrd' type='checkbox' " & IIf(LCase(!ExternReceiptKey).ToString = "", "disabled", "") & " id='ChkSelectDtl" & i & "' data-id='" & !SerialKey & "' />"
                MyRecords += "                        <label for='ChkSelectDtl" & i & "'><span class='CheckBoxStyle'></span></label>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                        <div class='editStyle' data-id='" & !SerialKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='1'>"
                MyRecords += "                        " & !ExternLineNo
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !Sku
                MyRecords += "                    </td>"
                measureunit = CommonMethods.getUomMeasure(Facility, !PackKey.ToString, !UOM.ToString)
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & (Double.Parse(!QtyExpected.ToString) / measureunit).ToString
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & (Double.Parse(!QtyReceived.ToString) / measureunit).ToString
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                MyRecords += "                        " & !PackKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                MyRecords += "                        " & !UOM
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='7'>"
                MyRecords += "                        " & !POKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='8'>"
                MyRecords += "                        " & !ToId
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='9'>"
                MyRecords += "                        " & !ToLoc
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='10'>"
                MyRecords += "                        " & !ConditionCode
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='11'>"
                MyRecords += "                        " & !TariffKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='12'>"
                MyRecords += "                        " & !Lottable01
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='13'>"
                MyRecords += "                        " & !Lottable02
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='14'>"
                MyRecords += "                        " & !Lottable03
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='15'>"
                If Not .IsNull("Lottable04") Then
                    Lottable04 = Format(!Lottable04, "MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                        " & Lottable04
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='16'>"
                If Not .IsNull("Lottable05") Then
                    Lottable05 = Format(!Lottable05, "MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                        " & Lottable05
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='17'>"
                MyRecords += "                        " & !Lottable06
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='18'>"
                MyRecords += "                        " & !Lottable07
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='19'>"
                MyRecords += "                        " & !Lottable08
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='20'>"
                MyRecords += "                        " & !Lottable09
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='21'>"
                MyRecords += "                        " & !Lottable10
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='23'>"
                If Not .IsNull("Lottable11") Then
                    Lottable11 = Format(!Lottable11, "MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                        " & Lottable11
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='23'>"
                If Not .IsNull("Lottable12") Then
                    Lottable12 = Format(!Lottable12, "MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                        " & Lottable12
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetSORecordsDetails(ByVal OBJTable As DataTable, ByRef MyRecords As String, ByVal Facility As String)
        Dim Lottable04 As String, Lottable05 As String, measureunit As Double
        For i = 0 To OBJTable.Rows.Count - 1
            Lottable04 = ""
            Lottable05 = ""
            measureunit = 1
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell selectAllWidth'>"
                MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrd' type='checkbox' " & IIf(LCase(!ExternOrderKey).ToString = "", "disabled", "") & " id='ChkSelectDtl" & i & "' data-id='" & !SerialKey & "' />"
                MyRecords += "                        <label for='ChkSelectDtl" & i & "'><span class='CheckBoxStyle'></span></label>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                        <div class='editStyle' data-id='" & !SerialKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='1'>"
                MyRecords += "                        " & !ExternLineNo
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !Sku
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                measureunit = CommonMethods.getUomMeasure(Facility, !PackKey.ToString, !UOM.ToString)
                MyRecords += "                        " & (Double.Parse(!OpenQty.ToString) / measureunit).ToString
                MyRecords += "                    </td>"
				'Mohamad Rmeity - Adding OriginalQty, QtyAllocated, QtyPicked, ShippedQty to Detail Grid - BEGIN
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                measureunit = CommonMethods.getUomMeasure(Facility, !PackKey.ToString, !UOM.ToString)
                MyRecords += "                        " & (Double.Parse(!OriginalQty.ToString) / measureunit).ToString
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                measureunit = CommonMethods.getUomMeasure(Facility, !PackKey.ToString, !UOM.ToString)
                MyRecords += "                        " & (Double.Parse(!QtyAllocated.ToString) / measureunit).ToString
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                measureunit = CommonMethods.getUomMeasure(Facility, !PackKey.ToString, !UOM.ToString)
                MyRecords += "                        " & (Double.Parse(!QtyPicked.ToString) / measureunit).ToString
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='7'>"
                measureunit = CommonMethods.getUomMeasure(Facility, !PackKey.ToString, !UOM.ToString)
                MyRecords += "                        " & (Double.Parse(!ShippedQty.ToString) / measureunit).ToString
                MyRecords += "                    </td>"
                'Mohamad Rmeity - Adding OriginalQty, QtyAllocated, QtyPicked, ShippedQty to Detail Grid - END
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & !PackKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                MyRecords += "                        " & !UOM
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                MyRecords += "                        " & !SUsr1
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='7'>"
                MyRecords += "                        " & !SUsr2
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='8'>"
                MyRecords += "                        " & !SUsr3
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='9'>"
                MyRecords += "                        " & !SUsr4
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='10'>"
                MyRecords += "                        " & !SUsr5
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='11'>"
                MyRecords += "                        " & !Lottable01
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='12'>"
                MyRecords += "                        " & !Lottable02
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='13'>"
                MyRecords += "                        " & !Lottable03
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='14'>"
                If Not .IsNull("Lottable04") Then
                    Lottable04 = Format(!Lottable04, "MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                        " & Lottable04
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='15'>"
                If Not .IsNull("Lottable05") Then
                    Lottable05 = Format(!Lottable05, "MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                        " & Lottable05
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='16'>"
                MyRecords += "                        " & !Lottable06
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='17'>"
                MyRecords += "                        " & !Lottable07
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='18'>"
                MyRecords += "                        " & !Lottable08
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='19'>"
                MyRecords += "                        " & !Lottable09
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='20'>"
                MyRecords += "                        " & !Lottable10
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetOrderManagementRecordsDetails(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim Lottable04 As String, Lottable05 As String
        For i = 0 To OBJTable.Rows.Count - 1
            Lottable04 = ""
            Lottable05 = ""
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell selectAllWidth'>"
                MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrd' type='checkbox' " & IIf(LCase(!ExternOrderKey).ToString = "", "disabled", "") & " id='ChkSelectDtl" & i & "' data-id='" & !SerialKey & "' />"
                MyRecords += "                        <label for='ChkSelectDtl" & i & "'><span class='CheckBoxStyle'></span></label>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                        <div class='editStyle' data-id='" & !SerialKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='1'>"
                MyRecords += "                        " & !ExternLineNo
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !Sku
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & Val(!OpenQty)
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & !PackKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                MyRecords += "                        " & !UOM
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                MyRecords += "                        " & !UnitPrice
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='7'>"
                MyRecords += "                        " & !SUsr5
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='8'>"
                MyRecords += "                        " & !SUsr1
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='9'>"
                MyRecords += "                        " & !SUsr2
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='10'>"
                MyRecords += "                        " & !SUsr3
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='11'>"
                MyRecords += "                        " & !SUsr4
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='12'>"
                MyRecords += "                        " & !Lottable01
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='13'>"
                MyRecords += "                        " & !Lottable02
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='14'>"
                MyRecords += "                        " & !Lottable03
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='15'>"
                If Not String.IsNullOrEmpty(!Lottable04) Then
                    Dim DateTime As DateTime
                    DateTime = DateTime.ParseExact(!Lottable04, CommonMethods.datef, Nothing)
                    Lottable04 = DateTime.ToString("MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                        " & Lottable04
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='16'>"
                If Not String.IsNullOrEmpty(!Lottable05) Then
                    Dim DateTime As DateTime
                    DateTime = DateTime.ParseExact(!Lottable05, CommonMethods.datef, Nothing)
                    Lottable05 = DateTime.ToString("MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                        " & Lottable05
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='17'>"
                MyRecords += "                        " & !Lottable06
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='18'>"
                MyRecords += "                        " & !Lottable07
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='19'>"
                MyRecords += "                        " & !Lottable08
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='20'>"
                MyRecords += "                        " & !Lottable09
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='21'>"
                MyRecords += "                        " & !Lottable10
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetOrderTrackingRecordsDetails(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim OriginalQty, OpenQty, AvailableQty, QtyAllocated, QtyPicked, ShippedQty As Integer
        For i = 0 To OBJTable.Rows.Count - 1
            OriginalQty = 0
            OpenQty = 0
            AvailableQty = 0
            QtyAllocated = 0
            QtyPicked = 0
            ShippedQty = 0
            With OBJTable.Rows(i)
                If Not .IsNull("OriginalQty") Then OriginalQty = Val(!OriginalQty)
                If Not .IsNull("OpenQty") Then OpenQty = Val(!OpenQty)
                If Not .IsNull("AvailableQty") Then AvailableQty = Val(!AvailableQty)
                If Not .IsNull("QtyAllocated") Then QtyAllocated = Val(!QtyAllocated)
                If Not .IsNull("QtyPicked") Then QtyPicked = Val(!QtyPicked)
                If Not .IsNull("ShippedQty") Then ShippedQty = Val(!ShippedQty)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='1'>"
                MyRecords += "                        " & !Facility
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !OrderKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & !OrderLineNumber
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & !ExternLineNo
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                MyRecords += "                        " & !Sku
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                MyRecords += "                        " & !SkuDescription
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='7'>"
                MyRecords += "                        " & !CarrierName
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='8'>"
                MyRecords += "                        " & !PackKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='9'>"
                MyRecords += "                        " & OriginalQty
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='10'>"
                MyRecords += "                        " & OpenQty
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='11'>"
                MyRecords += "                        " & AvailableQty
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='12'>"
                MyRecords += "                        " & QtyAllocated
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='13'>"
                MyRecords += "                        " & QtyPicked
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='14'>"
                MyRecords += "                        " & ShippedQty
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='15'>"
                MyRecords += "                        " & !PortalDescription
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub

    'Queries
    Private Sub GetOrderTrackingDetailsQuery(ByRef Sql As String, ByVal ExternOrderKey As String, ByVal StorerKey As String)
        Dim AndFilter As String = ""
        AndFilter += " And ExternOrderKey = '" & ExternOrderKey & "'"
        AndFilter += " And StorerKey = '" & StorerKey & "'"
        Sql = " SELECT * from ( "
        Sql += " SELECT (CASE WHEN WHSEID = 'MULTI' THEN 'MULTI' ELSE (select db_alias from wmsadmin.pl_db where db_logid = WHSEID) END) AS FACILITY, * from dbo.ORDERTRACKINGDETAIL where 1=1 " & AndFilter
        Sql += " ) DS where 1=1 "
    End Sub
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class