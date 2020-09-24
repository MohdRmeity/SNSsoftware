Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports Newtonsoft.Json
Imports NLog
Imports Oracle.ManagedDataAccess.Client

Public Class ExportItems
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub Export(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim tb As SQLExec = New SQLExec
        Dim DS As DataSet = Nothing
        Dim SearchQuery As String = HttpContext.Current.Request.Item("SearchQuery")
        Dim SearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim ColumnsNames As String = HttpContext.Current.Request.Item("ColumnsNames")
        Dim SortBy As String = HttpContext.Current.Request.Item("SortBy")
        Dim AndFilter As String = ""
        Dim tmp As String = ""
        Dim CSVData As String = ""
        Dim QueryUrlStr As String = HttpContext.Current.Request.Item("QueryUrlStr")
        Dim TabName As String = HttpContext.Current.Request.Item("TabName")
        Dim UserControlInfo As DataTable = CommonMethods.GetUserControlInfo()
        Dim ExportRowsLimit As Integer = 0
        If UserControlInfo.Rows.Count > 0 Then
            ExportRowsLimit = UserControlInfo.Rows(0)!ExportRowsLimit
        End If
        Dim RecordsCount As Integer = 0
        Dim SQL As String = " set dateformat mdy "

        If ExportRowsLimit > 0 Then
            If TabName = "Actions" Then
                ColumnsNames = "SCREENBUTTONNAME as [Screen Action], Edit, [READONLY]"
                SearchTable = "PROFILEDETAIL"
            ElseIf TabName = "Reports" Then
                SearchTable = "REPORTSPROFILEDETAIL"
            ElseIf TabName = "Dashboards" Then
                ColumnsNames = "DASHBOARD_Name as [Dashboard Name], Edit as [View]"
                SearchTable = "PROFILEDETAILDASHBOARDS"
            End If

            If SearchTable = "PORTALUSERS" Or SearchTable = "LOGSEXPORT" Or SearchTable = "LOGSIMPORT" Or SearchTable = "LOGSFILES" Then
                If CommonMethods.dbtype <> "sql" Then SearchTable = "System." & SearchTable
            ElseIf SearchTable = "FILEMANAGEMENT" Then
                GetFileManagementQuery(SQL)
            ElseIf SearchTable = "USERCONTROL" Then
                ColumnsNames = ColumnsNames.Replace(",Facility", "")
                If CommonMethods.dbtype <> "sql" Then
                    SearchTable = "System." & SearchTable
                    SortBy = "SerialKey desc"
                End If
            ElseIf SearchTable = "USERPROFILE" Or SearchTable = "PROFILES" Then
                If CommonMethods.dbtype <> "sql" Then
                    SearchTable = "System." & SearchTable
                    SortBy = "SerialKey desc"
                End If
            ElseIf SearchTable = "PROFILEDETAIL" Or SearchTable = "REPORTSPROFILEDETAIL" Or SearchTable = "PROFILEDETAILDASHBOARDS" Then
                AndFilter = " and ProfileName='" & QueryUrlStr & "'"
                If CommonMethods.dbtype <> "sql" Then
                    SearchTable = "System." & SearchTable
                    SortBy = "SerialKey desc"
                End If
            ElseIf SearchTable.Contains("enterprise.storer") Then
                Dim type As String = SearchTable(SearchTable.Length - 1)
                AndFilter = " and Type=" & type
                SearchTable = SearchTable.Remove(SearchTable.Length - 1)
            ElseIf SearchTable = "enterprise.sku" Or SearchTable = "SKUCATALOGUE" Then
                Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
                If Not owners Is Nothing Then
                    Dim ownersstr As String = String.Join("','", owners)
                    ownersstr = "'" & ownersstr & "'"
                    If Not UCase(ownersstr).Contains("'ALL'") Then
                        AndFilter += " and STORERKEY IN (" + ownersstr + ") "
                    End If
                End If
                If SearchTable = "SKUCATALOGUE" Then
                    If CommonMethods.dbtype <> "sql" Then SearchTable = "System." & SearchTable
                    Dim consignees As String() = CommonMethods.getConsigneePerUser(HttpContext.Current.Session("userkey").ToString)
                    If Not consignees Is Nothing Then
                        Dim consigneesstr As String = String.Join("','", consignees)
                        consigneesstr = "'" & consigneesstr & "'"
                        If Not UCase(consigneesstr).Contains("'ALL'") Then
                            AndFilter += " and CONSIGNEEKEY IN (" + consigneesstr + ") "
                        End If
                    End If
                End If
            ElseIf SearchTable = "Warehouse_PO" Then
                GetPurchaseOrderQuery(SQL)
            ElseIf SearchTable = "Warehouse_ASN" Then
                GetASNQuery(SQL)
            ElseIf SearchTable = "Warehouse_SO" Then
                GetSOQuery(SQL)
            ElseIf SearchTable = "Warehouse_OrderManagement" Then
                GetOrderManagementQuery(SQL)
            ElseIf SearchTable = "Warehouse_OrderTracking" Then
                GetOrderTrackingQuery(SQL, SearchQuery)
            ElseIf SearchTable = "Inventory_Balance" Then
                GetInventoryBalanceQuery(SQL)
            ElseIf SearchTable = "REPORTSPORTAL" Then
                SearchTable = "REPORTSPROFILEDETAIL"
                If CommonMethods.dbtype <> "sql" Then SearchTable = "System." & SearchTable
                AndFilter = " and report in (" & CommonMethods.getReportsPerUser(HttpContext.Current.Session("userkey").ToString) & ")"
            End If

            If SearchTable <> "Warehouse_PO" And SearchTable <> "Warehouse_ASN" And SearchTable <> "Warehouse_SO" And SearchTable <> "Warehouse_OrderManagement" And SearchTable <> "Inventory_Balance" And SearchTable <> "REPORTSPROFILEDETAIL" And SearchTable <> "FILEMANAGEMENT" And SearchTable <> "Warehouse_OrderTracking" Then
                SQL += " Select top " & CommonMethods.TopCount & " " & ColumnsNames & " from " & SearchTable & " where 1=1 " & AndFilter
            End If

            If SearchTable = "REPORTSPROFILEDETAIL" Then
                SQL += " select distinct top " & CommonMethods.TopCount & " (REPORT) , REPORT_NAME " & IIf(TabName <> "", ",ID, EDIT", "") & " from " & SearchTable & " where 1=1 " & AndFilter
            End If

            If SearchTable <> "Warehouse_OrderTracking" Then SearchItem(SearchQuery, SearchTable, SQL)
            SQL += " order by " & SortBy

            '1
            If SearchTable = "USERCONTROL" Then
                SQL += " Select UserKey, (Select DB_ALIAS from wmsadmin.pl_db where isActive='1' and db_enterprise='0' and db_name = USERCONTROLFACILITY.FACILITY) as Facility "
                SQL += " from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "USERCONTROLFACILITY"
            End If

            DS = tb.Cursor(SQL)

            If SearchTable = "USERCONTROL" Then
                DS.Tables(0).Columns.Add(New DataColumn("Facility", GetType(String)))
                For Each row As DataRow In DS.Tables(0).Rows
                    Dim dr As DataRow() = DS.Tables(1).Select("UserKey = '" & row("UserKey") & "'")
                    Dim Facility As String = ""
                    For i = 0 To dr.Count - 1
                        Facility += dr(i)!Facility & ","
                    Next
                    If Facility <> "" Then
                        row("Facility") = Facility.Remove(Facility.Length - 1)
                    Else
                        row("Facility") = Facility
                    End If
                Next
            ElseIf SearchTable = "Warehouse_ASN" Then
                For i = 0 To DS.Tables(0).Rows.Count - 1
                    DS.Tables(0).Rows(i)!TransportationMode = CommonMethods.GetTransMode(DS.Tables(0).Rows(i)!TransportationMode.ToString)
                Next
            End If

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

            If tmp = "" Then tmp = CommonMethods.SaveExportLogs(HttpContext.Current.Request.Item("SearchTable"), RecordsCount)
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
            writer.WriteValue(Trim(HttpContext.Current.Request.Item("SearchTable")) & IIf(TabName <> "", "-" & UCase(TabName), "") & "-" & Now.Ticks & ".csv")

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

    'Queries
    Private Sub GetFileManagementQuery(ByRef SQL As String)
        SQL += " select top " & CommonMethods.TopCount & " * from ("
        SQL += " select SerialKey, AddWho As UserKey,WHSEID, 'Warehouse_PO' as ScreenName,POKey as RecKey,OriginalFileName,FileName,FileSize,AddDate "
        SQL += " From PO_FILES UNION ALL"
        SQL += " select SerialKey, AddWho As UserKey,WHSEID, 'Warehouse_ASN' as ScreenName,ReceiptKey as RecKey,OriginalFileName,FileName,FileSize,AddDate "
        SQL += " From RECEIPT_FILES UNION ALL"
        SQL += " select SerialKey, AddWho As UserKey,WHSEID, 'Warehouse_SO' as ScreenName,OrderKey as RecKey,OriginalFileName,FileName,FileSize,AddDate "
        SQL += " From ORDERS_FILES UNION ALL"
        SQL += " select SerialKey, AddWho As UserKey,WHSEID, 'Warehouse_OrderManagement' as ScreenName,OrderManagKey as RecKey,OriginalFileName,FileName,FileSize,AddDate "
        SQL += " From ORDERMANAG_FILES"
        SQL += " ) as ds where 1=1 "
    End Sub
    Private Sub GetPurchaseOrderQuery(ByRef SQL As String)
        Dim wname As String = "", warehouselevel As String = "", AndFilter As String = ""
        Dim wnameRow As DataRow() = Nothing
        Dim TimeZone As Integer = 0

        If Integer.TryParse(HttpContext.Current.Session("timezone").ToString, TimeZone) Then
            TimeZone = Integer.Parse(HttpContext.Current.Session("timezone").ToString)
        End If

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

                SQL += " select top " & CommonMethods.TopCount & " * from ("
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

                    SQL += " select SerialKey, '" & wname & "' as Facility, StorerKey,BuyerName, "
                    SQL += " BuyersReference, SellerName, SellersReference, POKey, ExternPOKey, "
                    SQL += IIf(CommonMethods.dbtype = "sql", "DATEADD (hh , " & TimeZone & " , PODate )", "(PODate + interval '" & TimeZone & "' hour)") & " as PODate, "
                    SQL += " (select DESCRIPTION from " & warehouselevel & ".codelkup where code=po.POType And listname = 'POTYPE' ) as POType, "
                    SQL += " (select DESCRIPTION from " & warehouselevel & ".codelkup where code=po.Status AND listname = 'POSTATUS' ) as Status, "
                    SQL += " EffectiveDate, SUsr1, SUsr2, SUsr3, SUsr4, SUsr5 "
                    SQL += " from " & warehouselevel & ".po where 1=1  " & AndFilter
                    SQL += " UNION"
                Next
                If SQL.EndsWith("UNION") Then SQL = SQL.Remove(SQL.Length - 5)
                SQL += ") as ds where 1=1 "
            End If
        End If
    End Sub
    Private Sub GetASNQuery(ByRef SQL As String)
        Dim wname As String = "", warehouselevel As String = "", AndFilter As String = ""
        Dim wnameRow As DataRow() = Nothing
        Dim TimeZone As Integer = 0

        If Integer.TryParse(HttpContext.Current.Session("timezone").ToString, TimeZone) Then
            TimeZone = Integer.Parse(HttpContext.Current.Session("timezone").ToString)
        End If

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

                SQL += " select top " & CommonMethods.TopCount & " * from ("
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

                    SQL += " select SerialKey, '" & wname & "' as Facility, StorerKey, ReceiptKey, "
                    SQL += " POKey, CarrierKey, WarehouseReference, TransportationMode, ContainerKey, "
                    SQL += " ExternReceiptKey, ContainerType, "
                    SQL += IIf(CommonMethods.dbtype = "sql", "DATEADD (hh , " & TimeZone & " , ReceiptDate )", "(ReceiptDate + interval '" & TimeZone & "' hour)") & " as ReceiptDate, "
                    SQL += " (select DESCRIPTION from " & warehouselevel & ".codelkup where code=RECEIPT.Type And listname = 'RECEIPTYPE' ) as ReceiptType, "
                    SQL += " (select DESCRIPTION from " & warehouselevel & ".codelkup where code=RECEIPT.Status AND listname = 'RECSTATUS' ) as Status, "
                    SQL += " (select DESCRIPTION from enterprise.codelkup where code = OriginCountry and  listname = 'ISOCOUNTRY') as OriginCountry "
                    SQL += " from " & warehouselevel & ".Receipt where 1=1  " & AndFilter
                    SQL += " UNION"
                Next
                If SQL.EndsWith("UNION") Then SQL = SQL.Remove(SQL.Length - 5)
                SQL += ") as ds where 1=1 "
            End If
        End If
    End Sub
    Private Sub GetSOQuery(ByRef SQL As String)
        Dim wname As String = "", warehouselevel As String = "", AndFilter As String = ""
        Dim wnameRow As DataRow() = Nothing
        Dim TimeZone As Integer = 0

        If Integer.TryParse(HttpContext.Current.Session("timezone").ToString, TimeZone) Then
            TimeZone = Integer.Parse(HttpContext.Current.Session("timezone").ToString)
        End If

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

                SQL += " select top " & CommonMethods.TopCount & " * from ("
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

                    SQL += " select SerialKey, '" & wname & "' as Facility, StorerKey, OrderKey, "
                    SQL += " ConsigneeKey, ExternOrderKey, SUsr1, SUsr2, SUsr3, SUsr4, SUsr5, "
                    SQL += IIf(CommonMethods.dbtype = "sql", "DATEADD (hh , " & TimeZone & " , OrderDate )", "(OrderDate + interval '" & TimeZone & "' hour)") & " as OrderDate, "
                    SQL += IIf(CommonMethods.dbtype = "sql", "DATEADD (hh , " & TimeZone & " , ActualShipDate )", "(ActualShipDate + interval '" & TimeZone & "' hour)") & " as ActualShipDate, "
                    SQL += IIf(CommonMethods.dbtype = "sql", "DATEADD (hh , " & TimeZone & " , RequestedShipDate )", "(RequestedShipDate + interval '" & TimeZone & "' hour)") & " as RequestedShipDate, "
                    SQL += " (select DESCRIPTION from " & warehouselevel & ".codelkup where code=Orders.Type And listname = 'ORDERTYPE' ) as OrderType, "
                    SQL += " (select DESCRIPTION from " & warehouselevel & ".orderstatussetup where code=orders.Status ) as Status, "
                    SQL += " (select Company from " & warehouselevel & ".storer where type=2 and storerkey=orders.ConsigneeKey) as ConsigneeName "
                    SQL += " from " & warehouselevel & ".Orders where 1=1  " & AndFilter
                    SQL += " UNION"
                Next

                'SQL += " select top 1 SerialKey, '" & wname & "' as Facility, StorerKey, OrderKey, "
                'SQL += " ConsigneeKey, ExternOrderKey, SUsr1, SUsr2, SUsr3, SUsr4, SUsr5, "
                'SQL += IIf(CommonMethods.dbtype = "sql", "DATEADD (hh , " & TimeZone & " , OrderDate )", "(OrderDate + interval '" & TimeZone & "' hour)") & " as OrderDate, "
                'SQL += IIf(CommonMethods.dbtype = "sql", "DATEADD (hh , " & TimeZone & " , ActualShipDate )", "(ActualShipDate + interval '" & TimeZone & "' hour)") & " as ActualShipDate, "
                'SQL += IIf(CommonMethods.dbtype = "sql", "DATEADD (hh , " & TimeZone & " , RequestedShipDate )", "(RequestedShipDate + interval '" & TimeZone & "' hour)") & " as RequestedShipDate, "
                'SQL += " (select DESCRIPTION from wmwhse1.codelkup where code=Orders.Type And listname = 'ORDERTYPE' ) as OrderType, "
                'SQL += " (select DESCRIPTION from wmwhse1.orderstatussetup where code=orders.Status ) as Status, "
                'SQL += " (select Company from wmwhse1.storer where type=2 and storerkey=orders.ConsigneeKey) as ConsigneeName "
                'SQL += " from  wmwhse1.Orders where 1=1  and Orderkey = '0000273661'"
                'SQL += " UNION"

                If SQL.EndsWith("UNION") Then SQL = SQL.Remove(SQL.Length - 5)
                SQL += ") as ds where 1=1 "
            End If
        End If
    End Sub
    Private Sub GetOrderManagementQuery(ByRef SQL As String)
        Dim AndFilter As String = "", warehouses As String = ""
        Dim dtw As DataTable = CommonMethods.getFacilitiesPerUser(HttpContext.Current.Session("userkey").ToString)
        Dim i As Integer = 0

        For Each row As DataRow In dtw.Rows
            warehouses = warehouses & "'" & row("DB_NAME").ToString() & "',"
            i = i + 1
        Next
        warehouses = warehouses & "'1'"

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

                AndFilter += " and WHSEID IN (" & warehouses & ")"

                SQL += " select top " & CommonMethods.TopCount & " * from ("
                SQL += " select (select db_alias from wmsadmin.pl_db where isActive=1 and db_enterprise=0 and db_name = WHSEID) as Facility, "
                SQL += " SerialKey, WHSEID, StorerKey, OrderManagKey, OrderDate, RequestedShipDate, "
                SQL += " ConsigneeKey, ExternOrderKey, SUsr1, SUsr2, SUsr3, SUsr4, SUsr5, ORDERMANAGSTATUS, Type, "
                SQL += " (select Company from enterprise.storer where type=2 And storerkey=ORDERMANAG.ConsigneeKey) as ConsigneeName "
                SQL += " from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAG where 1=1  " & AndFilter
                SQL += ") as ds where 1=1 "
            End If
        End If
    End Sub
    Private Sub GetOrderTrackingQuery(ByRef SQL As String, ByVal SearchQuery As String)
        Dim AndFilter As String = "", HavingFilter As String = "", FinalFilter As String = ""
        Dim TimeZone As Integer = 0

        If Integer.TryParse(HttpContext.Current.Session("timezone").ToString, TimeZone) Then
            TimeZone = Integer.Parse(HttpContext.Current.Session("timezone").ToString)
        End If

        Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
        Dim consignees As String() = CommonMethods.getConsigneePerUser(HttpContext.Current.Session("userkey").ToString)
        Dim types As String() = CommonMethods.getOrderTypePerUser(HttpContext.Current.Session("userkey").ToString)
        Dim datefilter As String = CommonMethods.getDateFilterPerUser(HttpContext.Current.Session("userkey").ToString)

        If owners IsNot Nothing And consignees IsNot Nothing And types IsNot Nothing Then
            Dim ownersstr As String = String.Join("','", owners)
            ownersstr = "'" & ownersstr & "'"
            If Not UCase(ownersstr).Contains("'ALL'") Then AndFilter += " and OT.STORERKEY IN (" & ownersstr & ")"

            Dim consigneesstr As String = String.Join("','", consignees)
            consigneesstr = "'" & consigneesstr & "'"
            If Not UCase(consigneesstr).Contains("'ALL'") Then AndFilter += " and OT.ConsigneeKey IN (" & consigneesstr & ")"

            Dim typesstr As String = String.Join("','", types)
            typesstr = "'" & typesstr & "'"
            If Not UCase(typesstr).Contains("'ALL'") Then AndFilter += " and OT.TYPE IN (" & typesstr & ")"

            If datefilter <> "0" Then HavingFilter += " and MIN(OT.ADDDATE) >= GETUTCDATE() - " & datefilter

            Dim MySearchTerms() As String = Split(SearchQuery, ",")

            For i = 0 To MySearchTerms.Length - 1
                Dim MySearchInsideTerms() As String = Split(MySearchTerms(i), ":")

                If MySearchInsideTerms.Length = 2 Then
                    If MySearchInsideTerms(0) <> "CarrierName" And MySearchInsideTerms(0) <> "OrderType" And MySearchInsideTerms(0) <> "PortalDescription" And MySearchInsideTerms(0) <> "Order_Transport_Status" And MySearchInsideTerms(0) <> "VASStatus" And MySearchInsideTerms(0) <> "Facility" And MySearchInsideTerms(0) <> "VASStartDate" And MySearchInsideTerms(0) <> "VASEndDate" And MySearchInsideTerms(0) <> "CustomOrderDate" And MySearchInsideTerms(0) <> "CustReqDate" And MySearchInsideTerms(0) <> "CustActShipDate" Then
                        AndFilter += " And OT." & MySearchInsideTerms(0) & " Like N'"
                        If MySearchInsideTerms(1).Contains("%") Then
                            AndFilter += MySearchInsideTerms(1) & "'"
                        Else
                            AndFilter += "%" & MySearchInsideTerms(1) & "%'"
                        End If
                    Else
                        If MySearchInsideTerms(0) = "CarrierName" Then
                            AndFilter += " And OT2.MAXCN Like N'"
                            If MySearchInsideTerms(1).Contains("%") Then
                                AndFilter += MySearchInsideTerms(1) & "'"
                            Else
                                AndFilter += "%" & MySearchInsideTerms(1) & "%'"
                            End If
                        ElseIf MySearchInsideTerms(0) = "OrderType" Or MySearchInsideTerms(0) = "PortalDescription" Then
                            FinalFilter += " And " & MySearchInsideTerms(0) & "='" & MySearchInsideTerms(1) & "'"
                        ElseIf MySearchInsideTerms(0) = "Order_Transport_Status" Or MySearchInsideTerms(0) = "VASStatus" Then
                            FinalFilter += " And " & MySearchInsideTerms(0) & " Like N'"
                            If MySearchInsideTerms(1).Contains("%") Then
                                FinalFilter += MySearchInsideTerms(1) & "'"
                            Else
                                FinalFilter += "%" & MySearchInsideTerms(1) & "%'"
                            End If
                        ElseIf MySearchInsideTerms(0) = "Facility" Then
                            If MySearchInsideTerms(1) <> "MULTI" Then
                                FinalFilter += " And " & MySearchInsideTerms(0) & "='" & MySearchInsideTerms(1) & "'"
                            Else
                                HavingFilter += " AND COUNT(OT.WHSEID) > 1 "
                            End If
                        Else
                            Dim StrJoin As String = "", MyField As String = ""

                            If MySearchInsideTerms(0) = "VASStartDate" Then
                                MyField = "convert(char,DATEADD(hh , " & TimeZone & " , VS.VASStartDate),101)"
                            ElseIf MySearchInsideTerms(0) = "VASEndDate" Then
                                MyField = "convert(char,DATEADD(hh , " & TimeZone & " , VS.VASEndDate),101)"
                            ElseIf MySearchInsideTerms(0) = "CustomOrderDate" Then
                                MyField = "convert(char,DATEADD(hh , " & TimeZone & " , MIN(OT.OrderDate)),101)"
                            ElseIf MySearchInsideTerms(0) = "CustReqDate" Then
                                MyField = "convert(char,DATEADD(hh , " & TimeZone & " , MIN(OT.REQUESTEDSHIPDATE)),101)"
                            ElseIf MySearchInsideTerms(0) = "CustActShipDate" Then
                                MyField = "convert(char,DATEADD(hh , " & TimeZone & " , MIN(OT.ACTUALSHIPDATE)),101)"
                            End If

                            If MySearchInsideTerms(1).Contains("<") And Not MySearchInsideTerms(1).Contains("<=") Then
                                StrJoin += " " & MySearchInsideTerms(1).Insert(MySearchInsideTerms(1).IndexOf("<") + 1, "'") & "'"
                            ElseIf MySearchInsideTerms(1).Contains(">") And Not MySearchInsideTerms(1).Contains(">=") Then
                                StrJoin += " " & MySearchInsideTerms(1).Insert(MySearchInsideTerms(1).IndexOf(">") + 1, "'") & "'"
                            ElseIf MySearchInsideTerms(1).Contains(">=") Then
                                StrJoin += " " & MySearchInsideTerms(1).Insert(MySearchInsideTerms(1).IndexOf(">=") + 2, "'") & "'"
                            ElseIf MySearchInsideTerms(1).Contains("<=") Then
                                StrJoin += " " & MySearchInsideTerms(1).Insert(MySearchInsideTerms(1).IndexOf("<=") + 2, "'") & "'"
                            ElseIf LCase(MySearchInsideTerms(1)).Contains("today") Then
                                StrJoin += " = convert(date,getdate()) "
                            Else
                                StrJoin += " Like N'%" & MySearchInsideTerms(1) & "%'"
                                Dim MyDateTime As DateTime
                                If DateTime.TryParseExact(MySearchInsideTerms(1), CommonMethods.dformat, CultureInfo.CurrentCulture, DateTimeStyles.None, MyDateTime) Then
                                    StrJoin += " Or " & MyField & " = convert(char,DATEADD(hh , " & TimeZone & " , '" & MySearchInsideTerms(1) & "'),101) "
                                End If
                            End If
                            StrJoin += " )"
                            If MySearchInsideTerms(0).Contains("VAS") Then
                                AndFilter += " AND (" & MyField & StrJoin
                            Else
                                HavingFilter += " AND (" & MyField & StrJoin
                            End If
                        End If
                    End If
                End If
            Next

            SQL += " select top " & CommonMethods.TopCount & " * from ("
            SQL += " SELECT OT.EXTERNORDERKEY, (CASE WHEN COUNT(OT.WHSEID) > 1 then 'MULTI' else (select db_alias from wmsadmin.pl_db where db_logid = MAX(OT.WHSEID)) END) AS FACILITY,  (select DESCRIPTION from enterprise.codelkup where code=OT.Type AND listname = 'ORDERTYPE' ) AS ORDERTYPE,   MIN(OT.STATUS) AS STATUS,   (SELECT PORTALDESCRIPTION from dbo.PORTALORDERSTATUSSETUP WITH (NOLOCK) WHERE CODE =CAST(MIN(OT.STATUS) as INT)) AS PORTALDESCRIPTION, DATEADD(hh , " & TimeZone & ", MIN(OT.ORDERDATE)) AS CustomOrderDate,   DATEADD(hh , " & TimeZone & " , MIN(OT.REQUESTEDSHIPDATE)) AS CustReqDate,   DATEADD (hh , " & TimeZone & " , MIN(OT.ACTUALSHIPDATE)) AS CustActShipDate,   OT.STORERKEY, (CASE WHEN OT2.CCC > 1 then 'MULTI' WHEN OT2.MAXCC IS NULL THEN 'No Carrier Assigned' else OT2.MAXCC END) AS CarrierCode,  (CASE WHEN OT2.CCN > 1 then 'MULTI' WHEN OT2.MAXCN IS NULL THEN 'No Carrier Assigned' else OT2.MAXCN END) AS CarrierName,   OT.BUYERPO,   MAX(OT.CONSIGNEEKEY) CONSIGNEEKEY,   OT.C_COMPANY,   OT.C_CITY,   OT.C_STATE,  OT.C_ZIP,   OT.C_COUNTRY,  OT.C_ADDRESS1,   OT.C_ADDRESS2,   OT.C_ADDRESS3,   OT.C_ADDRESS4,   OT.C_ADDRESS5,   MIN(OT.SUSR1) SUSR1,  MIN(OT.SUSR2) SUSR2,   MIN(OT.SUSR3) SUSR3,   MIN(OT.SUSR4) SUSR4,   MIN(OT.SUSR5) SUSR5,   VS.VASSTARTDATE,  VS.VASENDDATE,  (COUNT(DISTINCT(OT.WHSEID))) AS NBROFSPLIT,  CASE WHEN CET.ORDER_TRANSPORT_STATUS IS NULL THEN 'Not Started' ELSE CET.ORDER_TRANSPORT_STATUS END ORDER_TRANSPORT_STATUS,  (CASE WHEN (VS.VASENDDATE IS NOT NULL AND VS.VASSTARTDATE IS NOT NULL) THEN 'VAS Out' WHEN  VS.VASSTARTDATE IS NOT NULL THEN 'VAS In' ELSE 'Not Started' END) AS VASSTATUS,  MIN(OT.ADDDATE) as ADDDATE  FROM ENTERPRISE.ALL_ORDERS OT  LEFT JOIN dbo.VAS VS WITH (NOLOCK)  ON VS.EXTERNALORDERKEY = OT.EXTERNORDERKEY   AND VS.STORERKEY = OT.STORERKEY  LEFT JOIN (SELECT R3.STORERKEY, R3.EXTERNALORDERKEY, R3.CARRIERNAME, R3.CONSOLIDATIONSEQ, CASE WHEN MIN(CARRIEREVENTSTATUSCODES.EVENTDESCRIPTION) IS NULL THEN 'Not Started' ELSE MIN(CARRIEREVENTSTATUSCODES.EVENTDESCRIPTION) END ORDER_TRANSPORT_STATUS FROM   CARRIEREVENTSTATUSCODES,  ( SELECT R2.STORERKEY, R2.EXTERNALORDERKEY, R2.CARRIERNAME, MIN(R2.CONSOLIDATIONSEQ) CONSOLIDATIONSEQ FROM ( SELECT R1.STORERKEY, R1.EXTERNALORDERKEY, R1.CARRIERNAME, R1.CONSIGNMENTID, CASE WHEN MIN(R1.CONSOLIDATIONSEQ) IS NULL THEN '000' ELSE MIN(R1.CONSOLIDATIONSEQ) END CONSOLIDATIONSEQ FROM ( SELECT CE.STORERKEY, CE.EXTERNALORDERKEY, CE.CARRIERNAME, CE.CONSIGNMENTID, MAX(CESC.CONSOLIDATIONSEQ) CONSOLIDATIONSEQ FROM   CARRIEREVENTS CE LEFT JOIN CARRIEREVENTSTATUSCODES CESC ON  CE.EVENTSTATUSCODE = CESC.EVENTCODE  AND          CESC.CONSOLIDATE = 'Y' GROUP  BY CE.STORERKEY, CE.EXTERNALORDERKEY, CE.CARRIERNAME, CE.CONSIGNMENTID, CE.ARTICLEID ) R1 GROUP BY R1.STORERKEY, R1.EXTERNALORDERKEY, R1.CARRIERNAME, R1.CONSIGNMENTID ) R2 GROUP BY R2.STORERKEY, R2.EXTERNALORDERKEY, R2.CARRIERNAME ) R3 WHERE  CARRIEREVENTSTATUSCODES.CONSOLIDATIONSEQ = R3.CONSOLIDATIONSEQ GROUP  BY R3.STORERKEY, R3.EXTERNALORDERKEY, R3.CARRIERNAME, R3.CONSOLIDATIONSEQ) CET   ON CET.EXTERNALORDERKEY = OT.EXTERNORDERKEY   AND CET.STORERKEY = OT.STORERKEY LEFT JOIN (SELECT EXTERNORDERKEY, STORERKEY, COUNT(DISTINCT(CarrierCode)) CCC , MAX(CarrierCode) MAXCC, MAX(CarrierName) MAXCN, COUNT(DISTINCT(CarrierName)) CCN FROM ENTERPRISE.ALL_ORDERS  WITH (NOLOCK) GROUP BY EXTERNORDERKEY, STORERKEY) OT2  ON OT2.EXTERNORDERKEY = OT.EXTERNORDERKEY  AND OT2.STORERKEY = OT.STORERKEY WHERE 1=1 " & AndFilter & " GROUP BY OT.EXTERNORDERKEY, OT.TYPE, OT.STORERKEY, OT.BUYERPO,  OT.C_COMPANY, OT.C_CITY, OT.C_STATE, OT.C_ZIP, OT.C_COUNTRY,  OT.C_ADDRESS1, OT.C_ADDRESS2,  OT.C_ADDRESS3, OT.C_ADDRESS4, OT.C_ADDRESS5,  VS.VASSTARTDATE, VS.VASENDDATE, CET.ORDER_TRANSPORT_STATUS, OT2.CCC, OT2.MAXCC,  OT2.MAXCN, OT2.CCN  HAVING 1=1  " & HavingFilter
            SQL += ") as ds where 1=1 " & FinalFilter
        End If
    End Sub
    Private Sub GetInventoryBalanceQuery(ByRef SQL As String)
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
                If Not UCase(ownersstr).Contains("'ALL'") Then AndFilter += " and lli.STORERKEY IN (" & ownersstr & ")"

                SQL += " select top " & CommonMethods.TopCount & " * from ("
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

                    SQL += " SELECT lli.SerialKey, '" & wname & "' as Facility, lli.StorerKey, lli.Sku, sum(lli.Qty) Qty, "
                    SQL += " case when status ='OK' then  sum((lli.Qty-lli.QtyAllocated-lli.QtyPicked)) else 0 end Available, "
                    SQL += " lli.Status , s.DESCR as Description FROM " & warehouselevel & ".LOTxLOCxID lli , " & warehouselevel & ".SKU s "
                    SQL += " WHERE s.sku=lli.Sku And lli.StorerKey= s.STORERKEY And (lli.StorerKey >= '0') "
                    SQL += " AND(lli.StorerKey <= 'ZZZZZZZZZZ') AND(lli.Sku >= '0') AND(lli.Sku <= 'ZZZZZZZZZZ') "
                    SQL += " AND(Lot >= '0') AND(Lot <= 'ZZZZZZZZZZ') AND(Loc >= '0') AND(Loc <= 'ZZZZZZZZZZ') "
                    SQL += " AND(Id >= ' ') AND(Id <= 'ZZZZZZZZZZZZZZZZZZ') " & AndFilter
                    SQL += " GROUP BY lli.SerialKey, lli.WhseID, lli.StorerKey, lli.Sku, lli.Status, s.descr"
                    SQL += " UNION"
                Next
                If SQL.EndsWith("UNION") Then SQL = SQL.Remove(SQL.Length - 5)
                SQL += ") as ds where 1=1 "
            End If
        End If
    End Sub
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class