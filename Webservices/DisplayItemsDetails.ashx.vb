
Imports System.Globalization
Imports System.IO
Imports System.Xml
Imports Newtonsoft.Json

Public Class DisplayItemsDetails
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub DisplayItem(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim tb As SQLExec = New SQLExec
        Dim mySearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Facility"))
        Dim MyID As String = HttpContext.Current.Request.Item("MyID")
        Dim MyError As String = ""
        Dim SavedDetailsFields As String = ""
        Dim ReadOnlyDetailsFields As String = ""
        Dim Sql As String = " set dateformat dmy select * from "

        If mySearchTable <> "Warehouse_OrderManagement" Then
            Dim warehouselevel As String = ""
            If LCase(Facility.Substring(0, 6)) = "infor_" Then
                warehouselevel = Facility.Substring(6, Facility.Length - 6)
            Else
                warehouselevel = Facility
            End If
            warehouselevel = warehouselevel.Split("_")(1)

            Sql += warehouselevel & "."
            If mySearchTable = "Warehouse_PO" Then
                Sql += "PODETAIL"
            ElseIf mySearchTable = "Warehouse_ASN" Then
                Sql += "RECEIPTDETAIL"
            ElseIf mySearchTable = "Warehouse_SO" Then
                Sql += "ORDERDETAIL"
            End If
        Else
            Sql += IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAGDETAIL"
        End If
        Sql += " where SerialKey ='" & MyID & "'"

        Dim ds As DataSet = tb.Cursor(Sql)

        If ds.Tables(0).Rows.Count = 0 Then MyError = "This record does not exist"

        If Val(MyID) > 0 Then
            If ds.Tables(0).Rows.Count > 0 Then
                Select Case mySearchTable
                    Case "Warehouse_PO"
                        With ds.Tables(0).Rows(0)
                            Dim measureunit As Double = CommonMethods.getUomMeasure(Facility, !PackKey.ToString(), !UOM.ToString)
                            Dim externln As String = "", Sku As String = "", QtyOrdered As String = "", QtyReceived As String = ""

                            If Not .IsNull("ExternLineNo") Then externln = !ExternLineNo
                            If Not .IsNull("Sku") Then Sku = !Sku
                            If Not .IsNull("QtyOrdered") Then QtyOrdered = (Double.Parse(!QtyOrdered.ToString) / measureunit).ToString
                            If Not .IsNull("QtyReceived") Then QtyReceived = (Double.Parse(!QtyReceived.ToString) / measureunit).ToString

                            SavedDetailsFields += "ExternLineNo:::" & externln
                            SavedDetailsFields += ";;;Sku:::" & Sku
                            SavedDetailsFields += ";;;QtyOrdered:::" & QtyOrdered
                            SavedDetailsFields += ";;;QtyReceived:::" & QtyReceived

                            ReadOnlyDetailsFields += "ExternLineNo~~~Sku~~~QtyReceived"
                        End With
                    Case "Warehouse_ASN"
                        With ds.Tables(0).Rows(0)
                            Dim measureunit As Double = CommonMethods.getUomMeasure(Facility, !PackKey.ToString(), !UOM.ToString)
                            Dim externln As String = "", Sku As String = "", QtyExpected As String = "", QtyReceived As String = "",
                            PackKey As String = "", UOM As String = "", POKey As String = "", ToId As String = "",
                                ToLoc As String = "", ConditionCode As String = "", TariffKey As String = "", Lottable01 As String = "",
                                Lottable02 As String = "", Lottable03 As String = "", Lottable04 As String = "", Lottable05 As String = "",
                                Lottable06 As String = "", Lottable07 As String = "", Lottable08 As String = "", Lottable09 As String = "",
                                Lottable10 As String = "", Lottable11 As String = "", Lottable12 As String = ""

                            If Not .IsNull("ExternLineNo") Then externln = !ExternLineNo
                            If Not .IsNull("Sku") Then Sku = !Sku
                            If Not .IsNull("QtyExpected") Then QtyExpected = (Double.Parse(!QtyExpected.ToString) / measureunit).ToString
                            If Not .IsNull("QtyReceived") Then QtyReceived = (Double.Parse(!QtyReceived.ToString) / measureunit).ToString
                            If Not .IsNull("PackKey") Then PackKey = !PackKey
                            If Not .IsNull("UOM") Then UOM = !UOM
                            If Not .IsNull("POKey") Then POKey = !POKey
                            If Not .IsNull("ToId") Then ToId = !ToId
                            If Not .IsNull("ToLoc") Then ToLoc = !ToLoc
                            If Not .IsNull("ConditionCode") Then ConditionCode = !ConditionCode
                            If Not .IsNull("TariffKey") Then TariffKey = !TariffKey
                            If Not .IsNull("Lottable01") Then Lottable01 = !Lottable01
                            If Not .IsNull("Lottable02") Then Lottable02 = !Lottable02
                            If Not .IsNull("Lottable03") Then Lottable03 = !Lottable03

                            If Not .IsNull("Lottable04") Then
                                Dim DateTime As DateTime
                                Dim DateStr = Format(!Lottable04, "MM/dd/yyyy HH:mm:ss")
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
                                Lottable04 = DateTime.ToString("MM/dd/yyyy")
                            End If

                            If Not .IsNull("Lottable05") Then
                                Dim DateTime As DateTime
                                Dim DateStr = Format(!Lottable05, "MM/dd/yyyy HH:mm:ss")
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
                                Lottable05 = DateTime.ToString("MM/dd/yyyy")
                            End If

                            If Not .IsNull("Lottable06") Then Lottable06 = !Lottable06
                            If Not .IsNull("Lottable07") Then Lottable07 = !Lottable07
                            If Not .IsNull("Lottable08") Then Lottable08 = !Lottable08
                            If Not .IsNull("Lottable09") Then Lottable09 = !Lottable09
                            If Not .IsNull("Lottable10") Then Lottable10 = !Lottable10

                            If Not .IsNull("Lottable11") Then
                                Dim DateTime As DateTime
                                Dim DateStr = Format(!Lottable11, "MM/dd/yyyy HH:mm:ss")
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
                                Lottable11 = DateTime.ToString("MM/dd/yyyy")
                            End If

                            If Not .IsNull("Lottable12") Then
                                Dim DateTime As DateTime
                                Dim DateStr = Format(!Lottable12, "MM/dd/yyyy HH:mm:ss")
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
                                Lottable12 = DateTime.ToString("MM/dd/yyyy")
                            End If

                            SavedDetailsFields += "ExternLineNo:::" & externln
                            SavedDetailsFields += ";;;Sku:::" & Sku
                            SavedDetailsFields += ";;;QtyExpected:::" & QtyExpected
                            SavedDetailsFields += ";;;QtyReceived:::" & QtyReceived
                            SavedDetailsFields += ";;;PackKey:::" & PackKey
                            SavedDetailsFields += ";;;UOM:::" & UOM
                            SavedDetailsFields += ";;;POKey:::" & POKey
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

                            ReadOnlyDetailsFields += "ExternLineNo~~~Sku~~~QtyReceived"
                        End With
                    Case "Warehouse_SO"
                        With ds.Tables(0).Rows(0)
                            Dim measureunit As Double = CommonMethods.getUomMeasure(Facility, !PackKey.ToString(), !UOM.ToString)
                            Dim externln As String = "", Sku As String = "", OpenQty As String = "", OriginalQty As String = "",
                                QtyAllocated As String = "", QtyPicked As String = "", ShippedQty As String = "",
                            PackKey As String = "", UOM As String = "", UDF1Dtl As String = "", UDF2Dtl As String = "",
                                UDF3Dtl As String = "", UDF4Dtl As String = "", UDF5Dtl As String = "", Lottable01 As String = "",
                                Lottable02 As String = "", Lottable03 As String = "", Lottable04 As String = "", Lottable05 As String = "",
                                Lottable06 As String = "", Lottable07 As String = "", Lottable08 As String = "", Lottable09 As String = "",
                                Lottable10 As String = ""

                            If Not .IsNull("ExternLineNo") Then externln = !ExternLineNo
                            If Not .IsNull("Sku") Then Sku = !Sku
                            If Not .IsNull("OpenQty") Then OpenQty = (Double.Parse(!OpenQty.ToString) / measureunit).ToString
							 'Mohamad Rmeity - Adding OriginalQty, QtyAllocated, QtyPicked, ShippedQty to Detail Grid - BEGIN
                            If Not .IsNull("OriginalQty") Then OriginalQty = (Double.Parse(!OriginalQty.ToString) / measureunit).ToString
                            If Not .IsNull("QtyAllocated") Then QtyAllocated = (Double.Parse(!QtyAllocated.ToString) / measureunit).ToString
                            If Not .IsNull("QtyPicked") Then QtyPicked = (Double.Parse(!QtyPicked.ToString) / measureunit).ToString
                            If Not .IsNull("ShippedQty") Then ShippedQty = (Double.Parse(!ShippedQty.ToString) / measureunit).ToString
                            'Mohamad Rmeity - Adding OriginalQty, QtyAllocated, QtyPicked, ShippedQty to Detail Grid - END
                            If Not .IsNull("PackKey") Then PackKey = !PackKey
                            If Not .IsNull("UOM") Then UOM = !UOM
                            If Not .IsNull("SUsr1") Then UDF1Dtl = !SUsr1
                            If Not .IsNull("SUsr2") Then UDF2Dtl = !SUsr2
                            If Not .IsNull("SUsr3") Then UDF3Dtl = !SUsr3
                            If Not .IsNull("SUsr4") Then UDF4Dtl = !SUsr4
                            If Not .IsNull("SUsr5") Then UDF5Dtl = !SUsr5
                            If Not .IsNull("Lottable01") Then Lottable01 = !Lottable01
                            If Not .IsNull("Lottable02") Then Lottable02 = !Lottable02
                            If Not .IsNull("Lottable03") Then Lottable03 = !Lottable03
                            If Not .IsNull("Lottable04") Then
                                Dim DateTime As DateTime
                                Dim DateStr = Format(!Lottable04, "MM/dd/yyyy HH:mm:ss")
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
                                Lottable04 = DateTime.ToString("MM/dd/yyyy")
                            End If

                            If Not .IsNull("Lottable05") Then
                                Dim DateTime As DateTime
                                Dim DateStr = Format(!Lottable05, "MM/dd/yyyy HH:mm:ss")
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
                                Lottable05 = DateTime.ToString("MM/dd/yyyy")
                            End If
                            If Not .IsNull("Lottable06") Then Lottable06 = !Lottable06
                            If Not .IsNull("Lottable07") Then Lottable07 = !Lottable07
                            If Not .IsNull("Lottable08") Then Lottable08 = !Lottable08
                            If Not .IsNull("Lottable09") Then Lottable09 = !Lottable09
                            If Not .IsNull("Lottable10") Then Lottable10 = !Lottable10

                            SavedDetailsFields += "ExternLineNo:::" & externln
                            SavedDetailsFields += ";;;Sku:::" & Sku
                            SavedDetailsFields += ";;;OpenQty:::" & OpenQty
							'Mohamad Rmeity - Adding OriginalQty, QtyAllocated, QtyPicked, ShippedQty to Detail Grid - BEGIN
                            SavedDetailsFields += ";;;OriginalQty:::" & OriginalQty
                            SavedDetailsFields += ";;;QtyAllocated:::" & QtyAllocated
                            SavedDetailsFields += ";;;QtyPicked:::" & QtyPicked
                            SavedDetailsFields += ";;;ShippedQty:::" & ShippedQty
                            'Mohamad Rmeity - Adding OriginalQty, QtyAllocated, QtyPicked, ShippedQty to Detail Grid - END
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

                            ReadOnlyDetailsFields += "ExternLineNo~~~Sku~~~SUsr5"
                        End With
                    Case "Warehouse_OrderManagement"
                        With ds.Tables(0).Rows(0)
                            Dim externln As String = "", Sku As String = "", OpenQty As String = "",
                                PackKey As String = "", UOM As String = "", Price As String = "",
                                Currency As String = "", UDF1Dtl As String = "", UDF2Dtl As String = "",
                                UDF3Dtl As String = "", UDF4Dtl As String = "", Lottable01 As String = "",
                                Lottable02 As String = "", Lottable03 As String = "", Lottable04 As String = "", Lottable05 As String = "",
                                Lottable06 As String = "", Lottable07 As String = "", Lottable08 As String = "", Lottable09 As String = "",
                                Lottable10 As String = ""

                            If Not .IsNull("ExternLineNo") Then externln = !ExternLineNo
                            If Not .IsNull("Sku") Then Sku = !Sku
                            If Not .IsNull("OpenQty") Then OpenQty = Val(!OpenQty)
                            If Not .IsNull("PackKey") Then PackKey = !PackKey
                            If Not .IsNull("UOM") Then UOM = !UOM
                            If Not .IsNull("UnitPrice") Then Price = !UnitPrice
                            If Not .IsNull("SUsr5") Then Currency = !SUsr5
                            If Not .IsNull("SUsr1") Then UDF1Dtl = !SUsr1
                            If Not .IsNull("SUsr2") Then UDF2Dtl = !SUsr2
                            If Not .IsNull("SUsr3") Then UDF3Dtl = !SUsr3
                            If Not .IsNull("SUsr4") Then UDF4Dtl = !SUsr4
                            If Not .IsNull("Lottable01") Then Lottable01 = !Lottable01
                            If Not .IsNull("Lottable02") Then Lottable02 = !Lottable02
                            If Not .IsNull("Lottable03") Then Lottable03 = !Lottable03
                            If Not String.IsNullOrEmpty(!Lottable04) Then
                                Dim DateTime As DateTime
                                DateTime = DateTime.ParseExact(!Lottable04, CommonMethods.datef, Nothing)
                                Lottable04 = DateTime.ToString("MM/dd/yyyy")
                            End If
                            If Not String.IsNullOrEmpty(!Lottable05) Then
                                Dim DateTime As DateTime
                                DateTime = DateTime.ParseExact(!Lottable05, CommonMethods.datef, Nothing)
                                Lottable05 = DateTime.ToString("MM/dd/yyyy")
                            End If
                            If Not .IsNull("Lottable06") Then Lottable06 = !Lottable06
                            If Not .IsNull("Lottable07") Then Lottable07 = !Lottable07
                            If Not .IsNull("Lottable08") Then Lottable08 = !Lottable08
                            If Not .IsNull("Lottable09") Then Lottable09 = !Lottable09
                            If Not .IsNull("Lottable10") Then Lottable10 = !Lottable10

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

                            ReadOnlyDetailsFields += "ExternLineNo~~~Sku~~~Price~~~Currency"
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

            writer.WritePropertyName("SavedDetailsFields")
            writer.WriteValue(SavedDetailsFields)

            writer.WritePropertyName("ReadOnlyDetailsFields")
            writer.WriteValue(ReadOnlyDetailsFields)

            writer.WriteEndObject()
        End Using
        context.Response.Write(sw)
        context.Response.End()
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class