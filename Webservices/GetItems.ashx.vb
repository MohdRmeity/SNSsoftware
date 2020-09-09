Imports System.Globalization
Imports System.IO
Imports Newtonsoft.Json

Public Class GetItems
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub GetMore(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim tb As SQLExec = New SQLExec
        Dim SearchQuery As String = HttpContext.Current.Request.Item("SearchQuery")
        Dim SearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim SortBy As String = HttpContext.Current.Request.Item("SortBy")
        Dim AndFilter As String = ""
        Dim QueryUrlStr As String = HttpContext.Current.Request.Item("QueryUrlStr")
        Dim TabName As String = HttpContext.Current.Request.Item("TabName")
        Dim SQL As String = " set dateformat mdy "

        If TabName = "Actions" Then
            SearchTable = "PROFILEDETAIL"
        ElseIf TabName = "Reports" Then
            SearchTable = "REPORTSPROFILEDETAIL"
        ElseIf TabName = "Dashboards" Then
            SearchTable = "PROFILEDETAILDASHBOARDS"
        End If

        If SearchTable = "PORTALUSERS" Or SearchTable = "LOGSEXPORT" Or SearchTable = "LOGSIMPORT" Or SearchTable = "LOGSFILES" Or SearchTable = "UITEMPLATES" Then
            If CommonMethods.dbtype <> "sql" Then SearchTable = "System." & SearchTable
        ElseIf SearchTable = "FILEMANAGEMENT" Then
            GetFileManagementQuery(SQL)
        ElseIf SearchTable = "USERPROFILE" Or SearchTable = "USERCONTROL" Or SearchTable = "PROFILES" Then
            If CommonMethods.dbtype <> "sql" Then
                SearchTable = "System." & SearchTable
                SortBy = "SerialKey desc"
            End If
        ElseIf SearchTable = "PROFILEDETAIL" Or SearchTable = "REPORTSPROFILEDETAIL" Or SearchTable = "PROFILEDETAILDASHBOARDS" Then
            AndFilter = " and ProfileName='" & QueryUrlStr & "'" & IIf(SearchTable = "PROFILEDETAIL", " and Blocked = 0 ", "")
            If CommonMethods.dbtype <> "sql" Then
                SearchTable = "System." & SearchTable
                SortBy = "SerialKey desc"
            End If
        ElseIf SearchTable.Contains("enterprise.storer") Then
            Dim type As String = IIf(SearchTable = "enterprise.storer2", "2", "12")
            AndFilter = " and Type=" & type
            If type = "2" Then
                Dim consignees As String() = CommonMethods.getConsigneePerUser(HttpContext.Current.Session("userkey").ToString)
                If Not consignees Is Nothing Then
                    Dim consigneesstr As String = String.Join("','", consignees)
                    consigneesstr = "'" & consigneesstr & "'"
                    If Not UCase(consigneesstr).Contains("'ALL'") Then
                        AndFilter += " and STORERKEY IN (" + consigneesstr + ") "
                    End If
                End If
            End If
            SearchTable = "enterprise.storer"
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
            'Mohamad Rmeity - Sort by storer
            SortBy = "StorerKey asc"
        ElseIf SearchTable = "REPORTSPORTAL" Then
            SearchTable = "REPORTSPROFILEDETAIL"
            If CommonMethods.dbtype <> "sql" Then SearchTable = "System." & SearchTable
            AndFilter = " and report in (" & CommonMethods.getReportsPerUser(HttpContext.Current.Session("userkey").ToString) & ")"
        End If

        If SearchTable <> "Warehouse_PO" And SearchTable <> "Warehouse_ASN" And SearchTable <> "Warehouse_SO" And SearchTable <> "Warehouse_OrderManagement" And SearchTable <> "Inventory_Balance" And SearchTable <> "REPORTSPROFILEDETAIL" And SearchTable <> "FILEMANAGEMENT" And SearchTable <> "Warehouse_OrderTracking" Then
            SQL += " Select top " & CommonMethods.TopCount & " * from " & SearchTable & " where 1=1 " & AndFilter
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

        Dim DS As DataSet = tb.Cursor(SQL)
        Dim OBJTable As DataTable = Nothing

        If DS IsNot Nothing Then OBJTable = DS.Tables(0)

        Dim MyRecords As String = ""
        If Not OBJTable Is Nothing Then
            If SearchTable = "LOGSEXPORT" Then
                GetLogsExportRecords(OBJTable, MyRecords)
            ElseIf SearchTable = "LOGSIMPORT" Then
                GetLogsImportRecords(OBJTable, MyRecords)
            ElseIf SearchTable = "LOGSFILES" Then
                GetLogsFileManagementRecords(OBJTable, MyRecords)
            ElseIf SearchTable = "FILEMANAGEMENT" Then
                GetFileManagementRecords(OBJTable, MyRecords)
            ElseIf SearchTable = "UITEMPLATES" Then
                GetUITemplatesRecords(OBJTable, MyRecords)
            ElseIf SearchTable = "PORTALUSERS" Then
                GetUserRecords(OBJTable, MyRecords)
            ElseIf SearchTable = "USERCONTROL" Then
                GetUserControlRecords(OBJTable, MyRecords, DS.Tables(1))
            ElseIf SearchTable = "USERPROFILE" Then
                GetUserProfileRecords(OBJTable, MyRecords)
            ElseIf SearchTable = "PROFILES" Then
                GetProfilesRecords(OBJTable, MyRecords)
            ElseIf TabName <> "" Then
                If TabName = "Actions" Then
                    GetProfileDetailRecords(OBJTable, MyRecords)
                ElseIf TabName = "Reports" Then
                    GetProfileDetailReportsRecords(OBJTable, MyRecords)
                ElseIf TabName = "Dashboards" Then
                    GetProfileDetailDashboardsRecords(OBJTable, MyRecords)
                End If
            ElseIf SearchTable = "enterprise.storer" Then
                GetConfigurationRecords(OBJTable, MyRecords)
            ElseIf SearchTable = "enterprise.sku" Then
                GetItemRecords(OBJTable, MyRecords)
            ElseIf SearchTable = "SKUCATALOGUE" Then
                GetItemCatalogueRecords(OBJTable, MyRecords)
            ElseIf SearchTable = "Warehouse_PO" Then
                GetPurchaseOrderRecords(OBJTable, MyRecords)
            ElseIf SearchTable = "Warehouse_ASN" Then
                GetASNRecords(OBJTable, MyRecords)
            ElseIf SearchTable = "Warehouse_SO" Then
                GetSORecords(OBJTable, MyRecords)
            ElseIf SearchTable = "Warehouse_OrderManagement" Then
                GetOrderManagementRecords(OBJTable, MyRecords)
            ElseIf SearchTable = "Warehouse_OrderTracking" Then
                GetOrderTrackingRecords(OBJTable, MyRecords)
            ElseIf SearchTable = "Inventory_Balance" Then
                GetInventoryBalanceRecords(OBJTable, MyRecords)
            ElseIf SearchTable = "REPORTSPROFILEDETAIL" Then
                GetViewReportsRecords(OBJTable, MyRecords)
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
    Private Sub GetLogsExportRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell Initial' data-id='1'>"
                MyRecords += "                        " & !UserKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !ScreenName
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & !RecordsCount
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & Format(!AddDate, "MM/dd/yyyy <br/> hh:mm:ss tt")
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetLogsImportRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell Initial' data-id='1'>"
                MyRecords += "                        " & !UserKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !ScreenName
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell Link' data-id='3'>"
                MyRecords += "                        <a href='" & clsGlobals.sAppPath & "DynamicFiles/ImportFiles/" & !FileName & "'>"
                MyRecords += "                        " & !FileName
                MyRecords += "                        </a>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & !FileSize
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                MyRecords += "                        " & !FileStatus
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                MyRecords += "                        " & Format(!AddDate, "MM/dd/yyyy <br/> hh:mm:ss tt")
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetLogsFileManagementRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell Initial' data-id='1'>"
                MyRecords += "                        " & !AddWho
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !ScreenName
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & !WhseID
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & !RecKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell Link' data-id='5'>"
                Dim FileExtension As String = LCase(!FileName.ToString.Substring(!FileName.ToString.LastIndexOf(".") + 1))
                Dim IsImage As Boolean = FileExtension = "png" Or FileExtension = "tiff" Or FileExtension = "jpg" Or FileExtension = "jpeg" Or FileExtension = "gif"
                MyRecords += "                        <a href='" & clsGlobals.sAppPath & "DynamicFiles/FileManagement/" & !FileName & "' " & IIf(IsImage, "target='_blank' rel='noopener'", "") & ">"
                MyRecords += "                        " & !FileName
                MyRecords += "                        </a>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                MyRecords += "                        " & CommonMethods.FormatFileSize(!FileSize)
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='7'>"
                MyRecords += "                        " & !ActivityType
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='8'>"
                MyRecords += "                        " & Format(!AddDate, "MM/dd/yyyy <br/> hh:mm:ss tt")
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetFileManagementRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell Initial' data-id='1'>"
                MyRecords += "                        " & !UserKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !ScreenName
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & !WhseID
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & !RecKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell Link' data-id='5'>"
                Dim FileExtension As String = LCase(!FileName.ToString.Substring(!FileName.ToString.LastIndexOf(".") + 1))
                Dim IsImage As Boolean = FileExtension = "png" Or FileExtension = "tiff" Or FileExtension = "jpg" Or FileExtension = "jpeg" Or FileExtension = "gif"
                MyRecords += "                        <a href='" & clsGlobals.sAppPath & "DynamicFiles/FileManagement/" & !FileName & "' " & IIf(IsImage, "target='_blank' rel='noopener'", "") & ">"
                MyRecords += "                        " & !FileName
                MyRecords += "                        </a>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                MyRecords += "                        " & CommonMethods.FormatFileSize(!FileSize)
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='7'>"
                MyRecords += "                        " & Format(!AddDate, "MM/dd/yyyy <br/> hh:mm:ss tt")
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetUITemplatesRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim page As Page = New Page
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell selectAllWidth'>"
                MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrd' type='checkbox' id='ChkSelect" & i & "' data-id='" & !SerialKey & "' />"
                MyRecords += "                        <label for='ChkSelect" & i & "'><span class='CheckBoxStyle'></span></label>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                        <div class='editStyleNew' data-id='" & !SerialKey & "' data-queryurl='?template=" & !UITEMPLATEID & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='1'>"
                MyRecords += "                        <a target='_blank' rel='noopener' href='" & HttpContext.Current.Server.UrlDecode(page.GetRouteUrl("SNSsoftware-Cufex-Administration_UITemplates", Nothing)) & "?template=" & !UITEMPLATEID & "'>"
                MyRecords += "                        " & !UITEMPLATEID
                MyRecords += "                        </a>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell Link' data-id='2'>"
                MyRecords += "                        <a href='" & clsGlobals.sAppPath & "DynamicImages/LogosImages/" & !PortalLogo & "' target='_blank' rel='noopener' >"
                MyRecords += "                        " & !PortalLogo
                MyRecords += "                        </a>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & !MenuBackgroundColor
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & !ScreenBackgroundColor
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                MyRecords += "                        " & !GridBackgroundColor
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                MyRecords += "                        " & !ButtonBackgroundColor
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='7'>"
                MyRecords += "                        " & !TextBackgroundColor
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetUserRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim page As Page = New Page
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell selectAllWidth'>"
                MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrd' type='checkbox' " & IIf(LCase(!UserKey).ToString = "admin", "disabled", "") & " id='ChkSelect" & i & "' data-id='" & !ID & "' />"
                MyRecords += "                        <label for='ChkSelect" & i & "'><span class='CheckBoxStyle'></span></label>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                        <div class='editStyleNew' data-id='" & !ID & "' data-queryurl='?user=" & !ID & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='1'>"
                MyRecords += "                        <a target='_blank' rel='noopener' href='" & HttpContext.Current.Server.UrlDecode(page.GetRouteUrl("SNSsoftware-Cufex-Security_Users", Nothing)) & "?user=" & !ID & "'>"
                MyRecords += "                        " & !UserKey
                MyRecords += "                        </a>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !FirstName
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & !LastName
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & !Email
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                MyRecords += "                        " & !TimeZone
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrdActive' type='checkbox' " & IIf(Val(!Active) = 1, "checked", "") & " id='ChkActive" & i & "' data-id='" & !ID & "' />"
                MyRecords += "                        <label for='ChkActive" & i & "'><span class='CheckBoxStyle'></span></label>"
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetUserControlRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String, ByVal OBJTable2 As DataTable)
        Dim MyID As Integer = 0
        Dim page As Page = New Page
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                If CommonMethods.dbtype = "sql" Then
                    MyID = !ID
                Else
                    MyID = !SerialKey
                End If
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell selectAllWidth'>"
                MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrd' type='checkbox' " & IIf(LCase(!UserKey).ToString = "admin", "disabled", "") & " id='ChkSelect" & i & "' data-id='" & MyID & "' />"
                MyRecords += "                        <label for='ChkSelect" & i & "'><span class='CheckBoxStyle'></span></label>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                        <div class='editStyleNew' data-id='" & MyID & "' data-queryurl='?user=" & !UserKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='1'>"
                MyRecords += "                        <a target='_blank' rel='noopener' href='" & HttpContext.Current.Server.UrlDecode(page.GetRouteUrl("SNSsoftware-Cufex-Security_UsersControl", Nothing)) & "?user=" & !UserKey & "'>"
                MyRecords += "                        " & !UserKey
                MyRecords += "                        </a>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !StorerKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & !ConsigneeKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & !SupplierKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                Dim dr As DataRow() = OBJTable2.Select("UserKey = '" & !UserKey & "'")
                For j = 0 To dr.Count - 1
                    If j = 0 Then
                        MyRecords += "" & dr(j)!Facility
                    Else
                        MyRecords += ", " & dr(j)!Facility
                    End If
                Next
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                MyRecords += "                        " & !ExportRowsLimit
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='7'>"
                MyRecords += "                        " & !FileImportLimit
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='8'>"
                MyRecords += "                        " & !FileUploadLimit
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='9'>"
                MyRecords += "                        " & !UITemplateID
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetProfilesRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim MyID As Integer = 0
        Dim page As Page = New Page
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                With OBJTable.Rows(i)
                    If CommonMethods.dbtype = "sql" Then
                        MyID = !ID
                    Else
                        MyID = !SerialKey
                    End If
                    MyRecords += "		<tr Class='GridRow GridResults'>"
                    MyRecords += "                    <td class='GridCell selectAllWidth'>"
                    MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrd' type='checkbox' " & IIf(LCase(!ProfileName).ToString.Contains("admin"), "disabled", "") & " id='ChkSelect" & i & "' data-id='" & MyID & "' />"
                    MyRecords += "                        <label for='ChkSelect" & i & "'><span class='CheckBoxStyle'></span></label>"
                    MyRecords += "                    </td>"
                    MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                    MyRecords += "                        <a href='" & HttpContext.Current.Server.UrlDecode(page.GetRouteUrl("SNSsoftware-Cufex-Security_ProfilesDetails", Nothing)) & "?profile=" & !ProfileName & "'><div class='editStyle' data-id='" & MyID & "'></div></a>"
                    MyRecords += "                    </td>"
                    MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                    MyRecords += "                    </td>"
                    MyRecords += "                    <td class='GridCell GridContentCell' data-id='1'>"
                    MyRecords += "                        <a target='_blank' rel='noopener' href='" & HttpContext.Current.Server.UrlDecode(page.GetRouteUrl("SNSsoftware-Cufex-Security_ProfilesDetails", Nothing)) & "?profile=" & !ProfileName & "'>"
                    MyRecords += "                        " & !ProfileName
                    MyRecords += "                        </a>"
                    MyRecords += "                    </td>"
                    MyRecords += "                </tr>"
                End With
            End With
        Next
    End Sub
    Private Sub GetProfileDetailRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim MyID As Integer = 0
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                With OBJTable.Rows(i)
                    If CommonMethods.dbtype = "sql" Then
                        MyID = !ID
                    Else
                        MyID = !SerialKey
                    End If
                    MyRecords += "		<tr Class='GridRow GridResults'>"
                    MyRecords += "                    <td class='GridCell borderRight0 selectAllWidth'>"
                    MyRecords += "                    </td>"
                    MyRecords += "                    <td class='GridCell GridHeadSearch'>"
                    MyRecords += "                        <div class='editStyle' data-id='" & MyID & "' style='display:none;'></div>"
                    MyRecords += "                    </td>"
                    MyRecords += "                    <td class='GridCell GridContentCell Initial' data-id='1'>"
                    MyRecords += "                        " & !ScreenButtonName
                    MyRecords += "                    </td>"
                    MyRecords += "                    <td class='GridCell GridContentCell'>"
                    MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrdEdit' type='checkbox' " & IIf(Val(!Edit) = 1, "checked", "") & " id='ChkEdit" & i & "' data-id='2' />"
                    MyRecords += "                        <label for='ChkEdit" & i & "'><span class='CheckBoxStyle'></span></label>"
                    MyRecords += "                    </td>"
                    MyRecords += "                    <td class='GridCell GridContentCell'>"
                    MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrdReadOnly' type='checkbox' " & IIf(Val(!ReadOnly) = 1, "checked", "") & " id='ChkReadOnly" & i & "' data-id='3' />"
                    MyRecords += "                        <label for='ChkReadOnly" & i & "'><span class='CheckBoxStyle'></span></label>"
                    MyRecords += "                    </td>"
                    MyRecords += "                </tr>"
                End With
            End With
        Next
    End Sub
    Private Sub GetProfileDetailReportsRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim MyID As Integer = 0
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                With OBJTable.Rows(i)
                    If CommonMethods.dbtype = "sql" Then
                        MyID = !ID
                    Else
                        MyID = !SerialKey
                    End If
                    MyRecords += "		<tr Class='GridRow GridResults'>"
                    MyRecords += "                    <td class='GridCell borderRight0 selectAllWidth'>"
                    MyRecords += "                    </td>"
                    MyRecords += "                    <td class='GridCell GridHeadSearch'>"
                    MyRecords += "                        <div class='editStyle' data-id='" & MyID & "' style='display:none;'></div>"
                    MyRecords += "                    </td>"
                    MyRecords += "                    <td class='GridCell GridContentCell Initial' data-id='1'>"
                    MyRecords += "                        " & !Report_Name
                    MyRecords += "                    </td>"
                    MyRecords += "                    <td class='GridCell GridContentCell'>"
                    MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrdEdit' type='checkbox' " & IIf(Val(!Edit) = 1, "checked", "") & " id='ChkEditReport" & i & "' data-id='2' />"
                    MyRecords += "                        <label for='ChkEditReport" & i & "'><span class='CheckBoxStyle'></span></label>"
                    MyRecords += "                    </td>"
                    MyRecords += "                </tr>"
                End With
            End With
        Next
    End Sub
    Private Sub GetProfileDetailDashboardsRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim MyID As Integer = 0
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                With OBJTable.Rows(i)
                    If CommonMethods.dbtype = "sql" Then
                        MyID = !ID
                    Else
                        MyID = !SerialKey
                    End If
                    MyRecords += "		<tr Class='GridRow GridResults'>"
                    MyRecords += "                    <td class='GridCell selectAllWidth'>"
                    MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrd' type='checkbox' id='ChkSelect" & i & "' data-id='" & MyID & "' />"
                    MyRecords += "                        <label for='ChkSelect" & i & "'><span class='CheckBoxStyle'></span></label>"
                    MyRecords += "                    </td>"
                    MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                    MyRecords += "                    </td>"
                    MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                    MyRecords += "                    </td>"
                    MyRecords += "                    <td class='GridCell GridContentCell Initial' data-id='1'>"
                    MyRecords += "                        " & !Dashboard_Name
                    MyRecords += "                    </td>"
                    MyRecords += "                    <td class='GridCell GridContentCell'>"
                    MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrdEdit' type='checkbox' " & IIf(Val(!Edit) = 1, "checked", "") & " id='ChkEditReport" & i & "' data-id='2' />"
                    MyRecords += "                        <label for='ChkEditReport" & i & "'><span class='CheckBoxStyle'></span></label>"
                    MyRecords += "                    </td>"
                    MyRecords += "                </tr>"
                End With
            End With
        Next
    End Sub
    Private Sub GetUserProfileRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim MyID As Integer = 0
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                With OBJTable.Rows(i)
                    If CommonMethods.dbtype = "sql" Then
                        MyID = !ID
                    Else
                        MyID = !SerialKey
                    End If
                    MyRecords += "		<tr Class='GridRow GridResults'>"
                    MyRecords += "                    <td class='GridCell selectAllWidth'>"
                    MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrd' type='checkbox' " & IIf(LCase(!UserKey).ToString = "admin", "disabled", "") & " id='ChkSelect" & i & "' data-id='" & MyID & "' />"
                    MyRecords += "                        <label for='ChkSelect" & i & "'><span class='CheckBoxStyle'></span></label>"
                    MyRecords += "                    </td>"
                    MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                    MyRecords += "                        <div class='editStyleNew' data-id='" & MyID & "' style='display:none;'></div>"
                    MyRecords += "                    </td>"
                    MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                    MyRecords += "                    </td>"
                    MyRecords += "                    <td class='GridCell GridContentCell Initial' data-id='1'>"
                    MyRecords += "                        " & !ProfileName
                    MyRecords += "                    </td>"
                    MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                    MyRecords += "                        " & !UserKey
                    MyRecords += "                    </td>"
                    MyRecords += "                </tr>"
                End With
            End With
        Next
    End Sub
    Private Sub GetConfigurationRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim page As Page = New Page
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell selectAllWidth'>"
                MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrd' type='checkbox' id='ChkSelect" & i & "' data-id='" & !SerialKey & "' />"
                MyRecords += "                        <label for='ChkSelect" & i & "'><span class='CheckBoxStyle'></span></label>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                        <div class='editStyleNew' data-id='" & !SerialKey & "' data-queryurl='?" & IIf(!Type = 2, "cust", "sup") & "=" & !StorerKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                        <div class='editStyle' data-id='" & !SerialKey & "' data-queryurl='?" & IIf(!Type = 2, "cust", "sup") & "=" & !StorerKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='1'>"
                MyRecords += "                        <a target='_blank' rel='noopener' href='" & HttpContext.Current.Server.UrlDecode(page.GetRouteUrl("SNSsoftware-Cufex-Configuration_Ship" & IIf(!Type = 2, "To", "From"), Nothing)) & "?" & IIf(!Type = 2, "cust", "sup") & "=" & !StorerKey & "'>"
                MyRecords += "                        " & !StorerKey
                MyRecords += "                        </a>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !Company
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & !Description
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & !Country
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                MyRecords += "                        " & !City
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                MyRecords += "                        " & !State
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='7'>"
                MyRecords += "                        " & !Zip
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='8'>"
                MyRecords += "                        " & !Address1
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='9'>"
                MyRecords += "                        " & !Address2
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='10'>"
                MyRecords += "                        " & !Contact1
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='11'>"
                MyRecords += "                        " & !Contact2
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='12'>"
                MyRecords += "                        " & !Email1
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='13'>"
                MyRecords += "                        " & !Email2
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='14'>"
                MyRecords += "                        " & !Phone1
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='15'>"
                MyRecords += "                        " & !Phone2
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='16'>"
                MyRecords += "                        " & !SUsr1
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='17'>"
                MyRecords += "                        " & !SUsr2
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='18'>"
                MyRecords += "                        " & !SUsr3
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='19'>"
                MyRecords += "                        " & !SUsr4
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='20'>"
                MyRecords += "                        " & !SUsr5
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetItemRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim page As Page = New Page
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell selectAllWidth'>"
                MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrd' type='checkbox' id='ChkSelect" & i & "' data-id='" & !SerialKey & "' />"
                MyRecords += "                        <label for='ChkSelect" & i & "'><span class='CheckBoxStyle'></span></label>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                        <div class='editStyleNew' data-id='" & !SerialKey & "' data-queryurl='?storer=" & !StorerKey & "&sku=" & !Sku & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                        <div class='editStyle' data-id='" & !SerialKey & "' data-queryurl='?storer=" & !StorerKey & "&sku=" & !Sku & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='1'>"
                MyRecords += "                        " & !StorerKey
                MyRecords += "                    </td>"
				'Mohamad Rmeity - Switching SKU & Owner
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
				MyRecords += "                        <a target='_blank' rel='noopener' href='" & HttpContext.Current.Server.UrlDecode(page.GetRouteUrl("SNSsoftware-Cufex-Configuration_Items", Nothing)) & "?storer=" & !StorerKey & "&sku=" & !Sku & "'>"
                MyRecords += "                        " & !Sku
                MyRecords += "                        </a>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & !Descr
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & !PackKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                MyRecords += "                        " & !StdCube
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                MyRecords += "                        " & !StdNetWgt
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='7'>"
                MyRecords += "                        " & !StdGrossWgt
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='8'>"
                MyRecords += "                        " & !SkuGroup
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetItemCatalogueRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim page As Page = New Page
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell selectAllWidth'>"
                MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrd' type='checkbox' id='ChkSelect" & i & "' data-id='" & !SerialKey & "' />"
                MyRecords += "                        <label for='ChkSelect" & i & "'><span class='CheckBoxStyle'></span></label>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                        <div class='editStyleNew' data-id='" & !SerialKey & "' data-queryurl='?item=" & !SerialKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                        <div class='editStyle' data-id='" & !SerialKey & "' data-queryurl='?item=" & !SerialKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='1'>"
                MyRecords += "                        <a target='_blank' rel='noopener' href='" & HttpContext.Current.Server.UrlDecode(page.GetRouteUrl("SNSsoftware-Cufex-Configuration_ItemCatalogue", Nothing)) & "?item=" & !SerialKey & "'>"
                MyRecords += "                        " & !Sku
                MyRecords += "                        </a>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !StorerKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & !ConsigneeKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & !Price
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                MyRecords += "                        " & !Currency
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
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetPurchaseOrderRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim PODate As String
        Dim page As Page = New Page
        For i = 0 To OBJTable.Rows.Count - 1
            PODate = ""
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell selectAllWidth'>"
                MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrd' type='checkbox' " & IIf(LCase(!ExternPOKey).ToString = "", "disabled", "") & " id='ChkSelect" & i & "' data-id='" & !SerialKey & "' data-keys = '" & !Facility & "~~~" & !POKey & "~~~" & !ExternPOKey & "' />"
                MyRecords += "                        <label for='ChkSelect" & i & "'><span class='CheckBoxStyle'></span></label>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                        <div class='editStyleNew' data-id='" & !SerialKey & "' data-queryurl='?warehouse=" & CommonMethods.getFacilityDBName(!Facility) & "&po=" & !POKey & "' data-keys = '" & !Facility & "~~~" & !POKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                        <div class='editStyle' data-id='" & !SerialKey & "' data-queryurl='?warehouse=" & CommonMethods.getFacilityDBName(!Facility) & "&po=" & !POKey & "' data-keys = '" & !Facility & "~~~" & !POKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='1'>"
                MyRecords += "                        <a target='_blank' rel='noopener' href='" & HttpContext.Current.Server.UrlDecode(page.GetRouteUrl("SNSsoftware-Cufex-Warehouse_PO", Nothing)) & "?warehouse=" & CommonMethods.getFacilityDBName(!Facility) & "&po=" & !POKey & "'>"
                MyRecords += "                        " & !POKey
                MyRecords += "                        </a>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !Facility
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & !ExternPOKey
                MyRecords += "                    </td>"
                If Not .IsNull("PODate") Then
                    PODate = Format(!PODate, "MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & PODate
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                MyRecords += "                        " & !Status
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                MyRecords += "                        " & !POType
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='7'>"
                MyRecords += "                        " & !StorerKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='8'>"
                MyRecords += "                        " & !BuyerName
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='9'>"
                MyRecords += "                        " & !BuyersReference
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='10'>"
                MyRecords += "                        " & !SellerName
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='11'>"
                MyRecords += "                        " & !SellersReference
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='12'>"
                MyRecords += "                        " & Format(!EffectiveDate, "MM/dd/yyyy <br/> hh:mm:ss tt")
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='13'>"
                MyRecords += "                        " & !SUsr1
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='14'>"
                MyRecords += "                        " & !SUsr2
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='15'>"
                MyRecords += "                        " & !SUsr3
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='16'>"
                MyRecords += "                        " & !SUsr4
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='17'>"
                MyRecords += "                        " & !SUsr5
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetASNRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim ReceiptDate As String
        Dim page As Page = New Page
        For i = 0 To OBJTable.Rows.Count - 1
            ReceiptDate = ""
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell selectAllWidth'>"
                MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrd' type='checkbox' " & IIf(LCase(!ExternReceiptKey).ToString = "", "disabled", "") & " id='ChkSelect" & i & "' data-id='" & !SerialKey & "' data-keys = '" & !Facility & "~~~" & !ReceiptKey & "~~~" & !ExternReceiptKey & "' />"
                MyRecords += "                        <label for='ChkSelect" & i & "'><span class='CheckBoxStyle'></span></label>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                        <div class='editStyleNew' data-id='" & !SerialKey & "' data-queryurl='?warehouse=" & CommonMethods.getFacilityDBName(!Facility) & "&receipt=" & !ReceiptKey & "' data-keys = '" & !Facility & "~~~" & !ReceiptKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                        <div class='editStyle' data-id='" & !SerialKey & "' data-queryurl='?warehouse=" & CommonMethods.getFacilityDBName(!Facility) & "&receipt=" & !ReceiptKey & "' data-keys = '" & !Facility & "~~~" & !ReceiptKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='1'>"
                MyRecords += "                        <a target='_blank' rel='noopener' href='" & HttpContext.Current.Server.UrlDecode(page.GetRouteUrl("SNSsoftware-Cufex-Warehouse_ASN", Nothing)) & "?warehouse=" & CommonMethods.getFacilityDBName(!Facility) & "&receipt=" & !ReceiptKey & "'>"
                MyRecords += "                        " & !ReceiptKey
                MyRecords += "                        </a>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !Facility
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & !StorerKey
                MyRecords += "                    </td>"
                If Not .IsNull("ReceiptDate") Then
                    ReceiptDate = Format(!ReceiptDate, "MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & ReceiptDate
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                MyRecords += "                        " & !Status
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                MyRecords += "                        " & !ReceiptType
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='7'>"
                MyRecords += "                        " & !POKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='8'>"
                MyRecords += "                        " & !ExternReceiptKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='9'>"
                MyRecords += "                        " & !CarrierKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='10'>"
                MyRecords += "                        " & !WarehouseReference
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='11'>"
                MyRecords += "                        " & !ContainerKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='12'>"
                MyRecords += "                        " & !ContainerType
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='13'>"
                MyRecords += "                        " & !OriginCountry
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='14'>"
                MyRecords += "                        " & CommonMethods.GetTransMode(!TransportationMode.ToString)
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetSORecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim OrderDate As String, RequestedShipDate As String, ActualShipDate As String
        Dim page As Page = New Page
        For i = 0 To OBJTable.Rows.Count - 1
            OrderDate = ""
            RequestedShipDate = ""
            ActualShipDate = ""
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell selectAllWidth'>"
                MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrd' type='checkbox' " & IIf(LCase(!ExternOrderKey).ToString = "", "disabled", "") & " id='ChkSelect" & i & "' data-id='" & !SerialKey & "' data-keys='" & !Facility & "~~~" & !OrderKey & "~~~" & !ExternOrderKey & "' />"
                MyRecords += "                        <label for='ChkSelect" & i & "'><span class='CheckBoxStyle'></span></label>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                        <div class='editStyleNew' data-id='" & !SerialKey & "' data-queryurl='?warehouse=" & CommonMethods.getFacilityDBName(!Facility) & "&order=" & !OrderKey & "' data-keys='" & !Facility & "~~~" & !OrderKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                        <div class='editStyle' data-id='" & !SerialKey & "' data-queryurl='?warehouse=" & CommonMethods.getFacilityDBName(!Facility) & "&order=" & !OrderKey & "' data-keys='" & !Facility & "~~~" & !OrderKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='1'>"
                MyRecords += "                        <a target='_blank' rel='noopener' href='" & HttpContext.Current.Server.UrlDecode(page.GetRouteUrl("SNSsoftware-Cufex-Warehouse_Shipment", Nothing)) & "?warehouse=" & CommonMethods.getFacilityDBName(!Facility) & "&order=" & !OrderKey & "'>"
                MyRecords += "                        " & !OrderKey
                MyRecords += "                        </a>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !Facility
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & !StorerKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & !ExternOrderKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                MyRecords += "                        " & !ConsigneeKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                MyRecords += "                        " & !ConsigneeName
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='7'>"
                MyRecords += "                        " & !Status
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='8'>"
                MyRecords += "                        " & !OrderType
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='9'>"
                If Not .IsNull("OrderDate") Then
                    OrderDate = Format(!OrderDate, "MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                        " & OrderDate
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='10'>"
                If Not .IsNull("RequestedShipDate") Then
                    RequestedShipDate = Format(!RequestedShipDate, "MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                        " & RequestedShipDate
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='11'>"
                If Not .IsNull("ActualShipDate") Then
                    ActualShipDate = Format(!ActualShipDate, "MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                        " & ActualShipDate
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='12'>"
                MyRecords += "                        " & !SUsr1
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='13'>"
                MyRecords += "                        " & !SUsr2
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='14'>"
                MyRecords += "                        " & !SUsr3
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='15'>"
                MyRecords += "                        " & !SUsr4
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='16'>"
                MyRecords += "                        " & !SUsr5
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetOrderManagementRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim OrderDate As String, RequestedShipDate As String, Type As String
        Dim page As Page = New Page
        For i = 0 To OBJTable.Rows.Count - 1
            OrderDate = ""
            RequestedShipDate = ""
            Type = ""
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell selectAllWidth'>"
                MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrd' type='checkbox' " & IIf(LCase(!ExternOrderKey).ToString = "", "disabled", "") & " id='ChkSelect" & i & "' data-id='" & !SerialKey & "' />"
                MyRecords += "                        <label for='ChkSelect" & i & "'><span class='CheckBoxStyle'></span></label>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                        <div class='editStyleNew' data-id='" & !SerialKey & "' data-queryurl='?ordermanagkey=" & !OrderManagKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                        <div class='editStyle' data-id='" & !SerialKey & "' data-queryurl='?ordermanagkey=" & !OrderManagKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='1'>"
                MyRecords += "                        <a target='_blank' rel='noopener' href='" & HttpContext.Current.Server.UrlDecode(page.GetRouteUrl("SNSsoftware-Cufex-Warehouse_OrderManagement", Nothing)) & "?ordermanagkey=" & !OrderManagKey & "'>"
                MyRecords += "                        " & !OrderManagKey
                MyRecords += "                        </a>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !Facility
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & !StorerKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & !ExternOrderKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                MyRecords += "                        " & !ConsigneeKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                MyRecords += "                        " & !ConsigneeName
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='7'>"
                MyRecords += "                        " & !OrderManagStatus
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='8'>"
                Dim DTOrderType = CommonMethods.getCodeDD(!WHSEID, "codelkup", "ORDERTYPE")
                Dim DROrderType() As DataRow = DTOrderType.Select("CODE='" & !Type & "'")
                If DROrderType.Length > 0 Then Type = DROrderType(0)!DESCRIPTION
                MyRecords += "                        " & Type
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='9'>"
                If Not .IsNull("OrderDate") Then
                    OrderDate = Format(!OrderDate, "MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                        " & OrderDate
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='10'>"
                If Not .IsNull("RequestedShipDate") Then
                    RequestedShipDate = Format(!RequestedShipDate, "MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                        " & RequestedShipDate
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='12'>"
                MyRecords += "                        " & !SUsr1
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='13'>"
                MyRecords += "                        " & !SUsr2
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='14'>"
                MyRecords += "                        " & !SUsr3
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='15'>"
                MyRecords += "                        " & !SUsr4
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='16'>"
                MyRecords += "                        " & !SUsr5
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetOrderTrackingRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim CustomOrderDate As String, CustReqDate As String, CustActShipDate, VASStartDate, VASEndDate As String
        Dim page As Page = New Page
        For i = 0 To OBJTable.Rows.Count - 1
            CustomOrderDate = ""
            CustReqDate = ""
            CustActShipDate = ""
            VASStartDate = ""
            VASEndDate = ""
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                        <div class='editStyleNew' data-id='" & !ExternOrderKey & "~~~" & !StorerKey & "' data-queryurl='?externorderkey=" & !ExternOrderKey & "&storerkey=" & !StorerKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                        <div class='editStyle' data-id='" & !ExternOrderKey & "~~~" & !StorerKey & "' data-queryurl='?externorderkey=" & !ExternOrderKey & "&storerkey=" & !StorerKey & "'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='1'>"
                MyRecords += "                        <a target='_blank' rel='noopener' href='" & HttpContext.Current.Server.UrlDecode(page.GetRouteUrl("SNSsoftware-Cufex-Warehouse_OrderTracking", Nothing)) & "?externorderkey=" & !ExternOrderKey & "&storerkey=" & !StorerKey & "'>"
                MyRecords += "                        " & !ExternOrderKey
                MyRecords += "                        </a>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !StorerKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & !Facility
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                If Not .IsNull("CustomOrderDate") Then
                    CustomOrderDate = Format(!CustomOrderDate, "MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                        " & CustomOrderDate
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                If Not .IsNull("CustReqDate") Then
                    CustReqDate = Format(!CustReqDate, "MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                        " & CustReqDate
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                If Not .IsNull("CustActShipDate") Then
                    CustActShipDate = Format(!CustActShipDate, "MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                        " & CustActShipDate
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='7'>"
                MyRecords += "                        " & !OrderType
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='8'>"
                MyRecords += "                        " & !PortalDescription
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='9'>"
                MyRecords += "                        " & !Order_Transport_Status
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='10'>"
                MyRecords += "                        " & !VASStatus
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='11'>"
                MyRecords += "                        " & !CarrierName
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='12'>"
                MyRecords += "                        " & !C_Company
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='13'>"
                MyRecords += "                        " & !C_Address1
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='14'>"
                MyRecords += "                        " & !BuyerPO
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='15'>"
                MyRecords += "                        " & !SUsr1
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='16'>"
                MyRecords += "                        " & !SUsr2
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='17'>"
                MyRecords += "                        " & !SUsr3
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='18'>"
                MyRecords += "                        " & !SUsr4
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='19'>"
                MyRecords += "                        " & !SUsr5
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='20'>"
                If Not .IsNull("VASStartDate") Then
                    VASStartDate = Format(!VASStartDate, "MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                        " & VASStartDate
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='21'>"
                If Not .IsNull("VASEndDate") Then
                    VASEndDate = Format(!VASEndDate, "MM/dd/yyyy <br/> hh:mm:ss tt")
                End If
                MyRecords += "                        " & VASEndDate
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetInventoryBalanceRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                        <div class='editStyleNew' data-id='" & !Status & "~~~" & !StorerKey & "~~~" & !Sku & "~~~" & !Facility & "' data-queryurl=''></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell Initial' data-id='1'>"
                MyRecords += "                        " & !Facility
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !StorerKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & !Sku
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & !Description
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                MyRecords += "                        " & Val(!Qty)
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                MyRecords += "                        " & Val(!Available)
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='7'>"
                MyRecords += "                        " & !Status
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetViewReportsRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim MyID As Integer = 0
        Dim page As Page = New Page
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                With OBJTable.Rows(i)
                    If CommonMethods.dbtype = "sql" Then
                        MyID = !report
                    Else
                        MyID = !SerialKey
                    End If
                    MyRecords += "		<div Class='GridRow GridResults'>"
                    MyRecords += "                    <div class='GridCell selectAllWidth'>"
                    MyRecords += "                        <input class='CheckBoxCostumizedNS2 chkSelectGrd' type='checkbox' " & IIf(LCase(!report_name).ToString.Contains("admin"), "disabled", "") & " id='ChkSelect" & i & "' data-id='" & MyID & "' />"
                    MyRecords += "                        <label for='ChkSelect" & i & "'><span class='CheckBoxStyle'></span></label>"
                    MyRecords += "                    </div>"
                    MyRecords += "                    <div class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                    MyRecords += "                        <a href='" & HttpContext.Current.Server.UrlDecode(page.GetRouteUrl("SNSsoftware-Cufex-Reporting_ReportViewer", Nothing)) & "?reportid=" & MyID & "'><div class='editStyleView'></div></a>"
                    MyRecords += "                    </div>"
                    MyRecords += "                    <div class='GridCell GridHeadSearch selectAllWidth'>"
                    MyRecords += "                    </div>"
                    MyRecords += "                    <div class='GridCell GridContentCell' data-id='1'>"
                    MyRecords += "                        <a target='_blank' rel='noopener' href='" & HttpContext.Current.Server.UrlDecode(page.GetRouteUrl("SNSsoftware-Cufex-Reporting_ReportViewer", Nothing)) & "?reportid=" & MyID & "'>"
                    MyRecords += "                        " & !report_name
                    MyRecords += "                        </a>"
                    MyRecords += "                    </div>"
                    MyRecords += "                </div>"
                End With
            End With
        Next
    End Sub

    'Queries
    Private Sub GetFileManagementQuery(ByRef SQL As String)
        SQL += " select top " & CommonMethods.TopCount & " * from ("
        SQL += " select SerialKey, AddWho As UserKey,WHSEID, 'Warehouse_PO' as ScreenName,POKey as RecKey,FileName,FileSize,AddDate "
        SQL += " From " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "PO_FILES UNION ALL"
        SQL += " select SerialKey, AddWho As UserKey,WHSEID, 'Warehouse_ASN' as ScreenName,ReceiptKey as RecKey,FileName,FileSize,AddDate "
        SQL += " From " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "RECEIPT_FILES UNION ALL"
        SQL += " select SerialKey, AddWho As UserKey,WHSEID, 'Warehouse_SO' as ScreenName,OrderKey as RecKey,FileName,FileSize,AddDate "
        SQL += " From " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERS_FILES UNION ALL"
        SQL += " select SerialKey, AddWho As UserKey,WHSEID, 'Warehouse_OrderManagement' as ScreenName,OrderManagKey as RecKey,FileName,FileSize,AddDate "
        SQL += " From " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAG_FILES"
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

                    'Mohamad Rmeity - Changing SO QUery
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

                    'Mohamad Rmeity - Grouping by SKU/Facility rather than each LLI records
                    SQL += " SELECT '" & wname & "' as Facility, lli.StorerKey, lli.Sku, sum(lli.Qty) Qty, "
                    SQL += " case when status ='OK' then  sum((lli.Qty-lli.QtyAllocated-lli.QtyPicked)) else 0 end Available, "
                    SQL += " lli.Status , s.DESCR as Description FROM " & warehouselevel & ".LOTxLOCxID lli , " & warehouselevel & ".SKU s "
                    SQL += " WHERE s.sku=lli.Sku And lli.StorerKey= s.STORERKEY And (lli.StorerKey >= '0') "
                    SQL += " AND(lli.StorerKey <= 'ZZZZZZZZZZ') AND(lli.Sku >= '0') AND(lli.Sku <= 'ZZZZZZZZZZ') "
                    SQL += " AND(Lot >= '0') AND(Lot <= 'ZZZZZZZZZZ') AND(Loc >= '0') AND(Loc <= 'ZZZZZZZZZZ') "
                    SQL += " AND(Id >= ' ') AND(Id <= 'ZZZZZZZZZZZZZZZZZZ') " & AndFilter
                    SQL += " GROUP BY lli.WhseID, lli.StorerKey, lli.Sku, lli.Status, s.descr "
                    SQL += " HAVING sum(lli.Qty) > 0 "
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