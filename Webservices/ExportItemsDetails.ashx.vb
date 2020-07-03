Imports System.Globalization
Imports System.IO
Imports Newtonsoft.Json

Public Class ExportItemsDetails
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub Export(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim tb As SQLExec = New SQLExec
        Dim DS As DataSet = Nothing
        Dim SearchQuery As String = HttpContext.Current.Request.Item("SearchQuery")
        Dim SearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim ColumnsNames As String = HttpContext.Current.Request.Item("ColumnsNames")
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Facility"))
        Dim Key As String = HttpContext.Current.Request.Item("Key")
        Dim SortBy As String = HttpContext.Current.Request.Item("SortBy")
        Dim AndFilter As String = ""
        Dim tmp As String = ""
        Dim CSVData As String = ""
        Dim UserControlInfo As DataTable = CommonMethods.GetUserControlInfo()
        Dim ExportRowsLimit As Integer = 0
        If UserControlInfo.Rows.Count > 0 Then
            ExportRowsLimit = UserControlInfo.Rows(0)!ExportRowsLimit
        End If
        Dim RecordsCount As Integer = 0
        If SearchTable = "Warehouse_OrderManagement" Then ColumnsNames = ColumnsNames.Replace("Price", "UnitPrice as Price").Replace("Currency", "SUsr5 as Currency")
        Dim SQL As String = " set dateformat dmy select " & ColumnsNames & " from "
        Dim warehouselevel As String = ""

        If ExportRowsLimit > 0 Then
            If LCase(Facility.Substring(0, 6)) = "infor_" Then
                warehouselevel = Facility.Substring(6, Facility.Length - 6)
            Else
                warehouselevel = Facility
            End If
            warehouselevel = warehouselevel.Split("_")(1)
            If SearchTable <> "Warehouse_OrderManagement" Then
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
            Else
                SQL += IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAGDETAIL"
                AndFilter = "OrderKey"
            End If

            SQL += " where 1=1 and " & AndFilter & " = '" & Key & "'"

            SearchItem(SearchQuery, SearchTable, SQL)
            SQL += " order by " & SortBy

            DS = tb.Cursor(SQL)

            If DS.Tables(0).Rows.Count > ExportRowsLimit Then
                RecordsCount = ExportRowsLimit
            Else
                RecordsCount = DS.Tables(0).Rows.Count
            End If

            Try
                CSVData = clsCSV.ToCSV(DS.Tables(0).AsEnumerable().Take(ExportRowsLimit).CopyToDataTable())
            Catch ex As Exception
                tmp = ex.Message
            End Try

            If tmp = "" Then tmp = CommonMethods.SaveExportLogs(HttpContext.Current.Request.Item("SearchTable") & "DETAILS", RecordsCount)
        Else
            tmp = "You do not have permission to extract any record"
        End If

        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)
        Using writer As JsonWriter = New JsonTextWriter(sw)
            writer.Formatting = Formatting.Indented
            writer.WriteStartObject()

            writer.WritePropertyName("CSVData")
            writer.WriteValue(CSVData)

            writer.WritePropertyName("FileName")
            writer.WriteValue(Trim(HttpContext.Current.Request.Item("SearchTable")) & "Details-" & warehouselevel & "-" & Key & "-" & Now.Ticks & ".csv")

            writer.WritePropertyName("ExportRowsLimit")
            If DS IsNot Nothing Then
                writer.WriteValue(IIf(DS.Tables(0).Rows.Count > ExportRowsLimit, ExportRowsLimit, 0))
            Else
                writer.WriteValue(0)
            End If

            writer.WritePropertyName("Error")
            writer.WriteValue(tmp)

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
                ElseIf MySearchInsideTerms(1).Contains("<") Then
                    Sql += " " & MySearchInsideTerms(1)
                ElseIf MySearchInsideTerms(1).Contains(">") Then
                    Sql += " " & MySearchInsideTerms(1)
                ElseIf MySearchInsideTerms(1).Contains(">=") Then
                    Sql += " " & MySearchInsideTerms(1)
                ElseIf MySearchInsideTerms(1).Contains("=<") Then
                    Sql += " " & MySearchInsideTerms(1)
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
                    If DateTime.TryParse(MySearchInsideTerms(1), MyDateTime) Then
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
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class