Imports System.Globalization
Imports System.IO
Imports Newtonsoft.Json

Public Class GetItemsPopup
    Implements IHttpHandler
    Implements IRequiresSessionState
    Sub GetMore(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim tb As SQLExec = New SQLExec
        Dim SearchQuery As String = HttpContext.Current.Request.Item("SearchQuery")
        Dim SearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim SortBy As String = HttpContext.Current.Request.Item("SortBy")
        Dim AndFilter As String = ""
        Dim QueryUrlStr As String = HttpContext.Current.Request.Item("QueryUrlStr")
        Dim SQL As String = " set dateformat mdy "

        Dim QueryUrlStrArr As String() = QueryUrlStr.Split("&")
        Dim Warehouse As String = "enterprise"

        If SearchTable = "warehouselevel.sku" Then
            If QueryUrlStr <> "" Then
                If QueryUrlStrArr.Length > 0 Then
                    Warehouse = QueryUrlStrArr(0).Split("=")(1)
                    If LCase(Warehouse.Substring(0, 6)) = "infor_" Then Warehouse = Warehouse.Substring(6, Warehouse.Length - 6)
                    If LCase(Warehouse).Contains("_") Then Warehouse = Warehouse.Split("_")(1)
                    SearchTable = Warehouse & ".sku"
                End If
                If QueryUrlStrArr.Length > 1 Then AndFilter = " and StorerKey ='" & QueryUrlStrArr(1).Split("=")(1) & "'"
            Else
                SearchTable = "enterprise.sku"
                Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
                If Not owners Is Nothing Then
                    Dim ownersstr As String = String.Join("','", owners)
                    ownersstr = "'" & ownersstr & "'"
                    If Not UCase(ownersstr).Contains("'ALL'") Then
                        AndFilter += " and STORERKEY IN (" + ownersstr + ") "
                    End If
                End If
            End If
        ElseIf SearchTable = "warehouselevel.pack" Then
            If QueryUrlStr <> "" Then
                If QueryUrlStrArr.Length > 0 Then
                    Warehouse = QueryUrlStrArr(0).Split("=")(1)
                    If LCase(Warehouse.Substring(0, 6)) = "infor_" Then Warehouse = Warehouse.Substring(6, Warehouse.Length - 6)
                    If LCase(Warehouse).Contains("_") Then Warehouse = Warehouse.Split("_")(1)
                    SearchTable = Warehouse & ".pack"
                End If
            Else
                SearchTable = "enterprise.pack"
            End If
        ElseIf SearchTable = "warehouselevel.loc" Then
            If QueryUrlStr <> "" Then
                If QueryUrlStrArr.Length > 0 Then
                    Warehouse = QueryUrlStrArr(0).Split("=")(1)
                    If LCase(Warehouse.Substring(0, 6)) = "infor_" Then Warehouse = Warehouse.Substring(6, Warehouse.Length - 6)
                    If LCase(Warehouse).Contains("_") Then Warehouse = Warehouse.Split("_")(1)
                    SearchTable = Warehouse & ".loc"
                End If
            Else
                SearchTable = "enterprise.loc"
            End If
        End If

        If SearchTable.Contains("sku") Then
            SQL += "select top " & CommonMethods.TopCount & " * from ( "
            SQL += " Select *, (select PackDescr from " & Warehouse & ".pack where PackKey = S.PackKey) PackDescr " & " from " & SearchTable & " S " & " where 1=1 " & AndFilter
            SQL += " ) As ds where 1=1 "
        Else
            SQL += " Select top " & CommonMethods.TopCount & " * from " & SearchTable & " where 1=1 " & AndFilter
        End If

        SearchItem(SearchQuery, SearchTable, SQL)
        SQL += " order by " & SortBy

        Dim DS As DataSet = tb.Cursor(SQL)
        Dim OBJTable As DataTable = DS.Tables(0)

        Dim MyRecords As String = ""
        If Not OBJTable Is Nothing Then
            If SearchTable.Contains("sku") Then
                GetItemRecords(OBJTable, MyRecords)
            ElseIf SearchTable.Contains("pack") Then
                GetPackRecords(OBJTable, MyRecords)
            ElseIf SearchTable.Contains("loc") Then
                GetLocationRecords(OBJTable, MyRecords)
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
    Private Sub GetItemRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim page As Page = New Page
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell selectAllWidth'>"
                MyRecords += "                        <div class='selectStyle'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell Initial' data-id='1'>"
                MyRecords += "                        " & !Sku
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !Descr
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & !PackKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & !PackDescr
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetPackRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim page As Page = New Page
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell selectAllWidth'>"
                MyRecords += "                        <div class='selectStyle'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell Initial' data-id='1'>"
                MyRecords += "                        " & !PackKey
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !PackDescr
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    Private Sub GetLocationRecords(ByVal OBJTable As DataTable, ByRef MyRecords As String)
        Dim page As Page = New Page
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                MyRecords += "		<tr Class='GridRow GridResults'>"
                MyRecords += "                    <td class='GridCell selectAllWidth'>"
                MyRecords += "                        <div class='selectStyle'></div>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch borderRight0 selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridHeadSearch selectAllWidth'>"
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell Initial' data-id='1'>"
                MyRecords += "                        " & !Loc
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='2'>"
                MyRecords += "                        " & !LocationType
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='3'>"
                MyRecords += "                        " & !LocationFlag
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='4'>"
                MyRecords += "                        " & !LocationCategory
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='5'>"
                MyRecords += "                        " & IIf(!LoseID = 0, "No", "Yes")
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='6'>"
                MyRecords += "                        " & IIf(!CommingleSku = 0, "No", "Yes")
                MyRecords += "                    </td>"
                MyRecords += "                    <td class='GridCell GridContentCell' data-id='7'>"
                MyRecords += "                        " & IIf(!CommingleLot = 0, "No", "Yes")
                MyRecords += "                    </td>"
                MyRecords += "                </tr>"
            End With
        Next
    End Sub
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class