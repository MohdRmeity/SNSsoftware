Imports System.IO
Imports Newtonsoft.Json
Public Class UserConfiguration
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub UserConfigurationAction(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim ActionType As String = HttpContext.Current.Request.Item("ActionType")

        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)
        Using writer As JsonWriter = New JsonTextWriter(sw)
            writer.Formatting = Newtonsoft.Json.Formatting.Indented
            writer.WriteStartObject()

            If ActionType = "set" Then
                writer.WritePropertyName("Error")
                writer.WriteValue(SetUserConfiguration())
            ElseIf ActionType = "get" Then
                Dim UserConfigTable = GetUserConfiguration()
                Dim ColumnsNames As String = ""
                Dim ColumnsPriorities As String = ""
                Dim ColumnsWidthes As String = ""
                Dim ColumnsHidden As String = ""
                Dim GridTypes As String = ""
                Dim MenuOpen As String = ""

                If UserConfigTable IsNot Nothing Then
                    MenuOpen = UserConfigTable.Rows(0)!MenuOpen
                    For i = 0 To UserConfigTable.Rows.Count - 1
                        With UserConfigTable.Rows(i)
                            ColumnsNames += IIf(i <> 0, ",", "") & !ColumnName.ToString
                            ColumnsPriorities += IIf(i <> 0, ",", "") & !ColumnPriority.ToString
                            ColumnsWidthes += IIf(i <> 0, ",", "") & !ColumnWidth.ToString
                            ColumnsHidden += IIf(i <> 0, ",", "") & !ColumnHidden.ToString
                            GridTypes += IIf(i <> 0, ",", "") & !GridType.ToString
                        End With
                    Next
                End If

                writer.WritePropertyName("ColumnsNames")
                writer.WriteValue(ColumnsNames)

                writer.WritePropertyName("ColumnsPriorities")
                writer.WriteValue(ColumnsPriorities)

                writer.WritePropertyName("ColumnsWidthes")
                writer.WriteValue(ColumnsWidthes)

                writer.WritePropertyName("ColumnsHidden")
                writer.WriteValue(ColumnsHidden)

                writer.WritePropertyName("GridTypes")
                writer.WriteValue(GridTypes)

                writer.WritePropertyName("MenuOpen")
                writer.WriteValue(MenuOpen)
            End If

            writer.WriteEndObject()
        End Using
        context.Response.Write(sw)
        context.Response.End()
    End Sub

    Private Function SetUserConfiguration() As String
        Dim sql As String = ""
        Dim tb As SQLExec = New SQLExec
        Dim SearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim UserKey As String = HttpContext.Current.Session("userkey").ToString
        Dim MenuOpen As String = HttpContext.Current.Request.Item("MenuOpen")
        Dim ColumnsNames As String = HttpContext.Current.Request.Item("ColumnsNames")
        Dim ColumnsPriorities As String = HttpContext.Current.Request.Item("ColumnsPriorities")
        Dim ColumnsWidthes As String = HttpContext.Current.Request.Item("ColumnsWidthes")
        Dim ColumnsHidden As String = HttpContext.Current.Request.Item("ColumnsHidden")
        Dim GridTypes As String = HttpContext.Current.Request.Item("GridTypes")
        Dim tmp As String = ""

        sql += " delete from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "PORTALUSERSCONFIGURATION "
        sql += " where UserKey = '" & UserKey & "' and ScreenName = '" & SearchTable & "'"
        tmp = tb.Execute(sql)

        If tmp = "" Then
            sql = ""
            Dim ColumnsNamesArray As String() = ColumnsNames.Split(",")
            Dim ColumnsPrioritiesArray As String() = ColumnsPriorities.Split(",")
            Dim ColumnsWidthesArray As String() = ColumnsWidthes.Split(",")
            Dim ColumnsHiddenArray As String() = ColumnsHidden.Split(",")
            Dim GridTypesArray As String() = GridTypes.Split(",")

            For i = 0 To ColumnsNamesArray.Length - 1
                sql += " insert into " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "PORTALUSERSCONFIGURATION "
                sql += " (ID, USERKEY, SCREENNAME, GRIDTYPE, COLUMNNAME, COLUMNPRIORITY, COLUMNWIDTH, COLUMNHIDDEN, MENUOPEN) VALUES ( "
                sql += "'" & (i + 1).ToString & "',"
                sql += "'" & UserKey & "',"
                sql += "'" & SearchTable & "',"
                sql += "'" & GridTypesArray(i).ToString & "',"
                sql += "'" & ColumnsNamesArray(i).ToString & "',"
                sql += "'" & ColumnsPrioritiesArray(i).ToString & "',"
                sql += "'" & ColumnsWidthesArray(i).ToString & "',"
                sql += "'" & ColumnsHiddenArray(i).ToString & "',"
                sql += "'" & MenuOpen & "')"
            Next
        End If

        tmp = tb.Execute(sql)
        Return tmp
    End Function
    Private Function GetUserConfiguration() As DataTable
        Dim sql As String = ""
        Dim tb As SQLExec = New SQLExec
        Dim ds As DataSet
        Dim SearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim UserKey As String = HttpContext.Current.Session("userkey").ToString

        sql += " Select * from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "PORTALUSERSCONFIGURATION "
        sql += " where UserKey = '" & UserKey & "' and ScreenName = '" & SearchTable & "' order by ID asc "
        ds = tb.Cursor(sql)
        If ds IsNot Nothing Then
            If ds.Tables(0).Rows.Count > 0 Then
                Return ds.Tables(0)
            End If
        End If
        Return Nothing
    End Function
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class