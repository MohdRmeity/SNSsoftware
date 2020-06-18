Imports System.Data.SqlClient
Imports DevExpress.DashboardWeb

Public Class CustomDashboardStorage
    Implements IEditableDashboardStorage

#Region "Properies and Constructor"

    Private Property userName As String
    Private Property DashboardTable As DataTable

    Public Sub New(ByVal v As String)
        userName = v
    End Sub

#End Region

#Region "Implemented Functions"

    Public Function AddDashboard(dashboard As XDocument, dashboardName As String) As String Implements IEditableDashboardStorage.AddDashboard
        If checkDashBoardExist(dashboardName) = 0 Then

            If CommonMethods.dbtype = "sql" Then

                CType(CType(dashboard.FirstNode, XContainer).FirstNode, XElement).LastAttribute.Value = dashboardName

                Dim insert As String = " INSERT INTO dbo.Dashboards (DashboardXml,DashboardName ) VALUES (@xml, @name) "

                Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                conn.Open()

                Dim cmd2 As SqlCommand = New SqlCommand(insert, conn)
                cmd2.Parameters.AddWithValue("@xml", dashboard.ToString())
                cmd2.Parameters.AddWithValue("@name", dashboardName)
                cmd2.ExecuteNonQuery()
                conn.Close()

                Dim id As Integer = 0
                Dim sql As String = "SELECT TOP(1) [DashboardID] FROM [Dashboards] ORDER BY 1 DESC"
                Dim ds As DataSet = (New SQLExec).Cursor(sql)
                If ds.Tables(0).Rows.Count > 0 Then id = ds.Tables(0).Rows(0)!DashboardID
                If id <> 0 Then
                    Dim dtProfiles As DataTable = getProfiles()
                    Dim userProfiles As DataTable = getUserProfiles()

                    For Each row In dtProfiles.Rows
                        Dim profile As String = row(0).ToString
                        insert = "set dateformat dmy INSERT INTO PROFILEDETAILDASHBOARDS (PROFILENAME,DASHBOARD,DASHBOARD_NAME,EDIT,ADDDATE,ADDWHO,EDITDATE,EDITWHO) Values ( "
                        insert += "'" & profile & "'"
                        insert += ",'" & id.ToString & "'"
                        insert += ",'" & dashboardName & "'"
                        If userProfiles.AsEnumerable().Any(Function(row1) profile = row1.Field(Of String)("profilename")) Then
                            insert += ",'1'"
                        Else
                            insert += ",'0'"
                        End If
                        insert += ",'" & Now.ToString & "'"
                        insert += ",'" & userName & "'"
                        insert += ",'" & Now.ToString & "'"
                        insert += ",'" & userName & "'"
                        insert += ")"
                        Dim tmp = (New SQLExec).Execute(insert)
                        If tmp <> "" Then Return "0"
                    Next
                    GetAvailableDashboardsInfo()
                    Return id.ToString
                End If
            End If
        Else
            Return "Dashbord name is already used"
        End If
        Return String.Empty
    End Function

    Public Function GetAvailableDashboardsInfo() As IEnumerable(Of DashboardInfo) Implements IDashboardStorage.GetAvailableDashboardsInfo
        Dim DashboardTable As DataTable = New DataTable()
        If CommonMethods.dbtype = "sql" Then
            Dim sql As String = "select distinct (DASHBOARD) , DASHBOARD_NAME  from PROFILEDETAILDASHBOARDS where  EDIT='1' and profilename in (select profilename from USERPROFILE where userkey= '" & userName & "') order by DASHBOARD_NAME asc"
            Dim ds As DataSet = (New SQLExec).Cursor(sql)
            DashboardTable = ds.Tables(0)
        End If
        Dim dashboardInfos As List(Of DashboardInfo) = New List(Of DashboardInfo)()
        For Each row In DashboardTable.Rows
            Dim DashboardInfo As DashboardInfo = New DashboardInfo
            DashboardInfo.ID = row("DASHBOARD").ToString
            DashboardInfo.Name = row("DASHBOARD_NAME").ToString()
            dashboardInfos.Add(DashboardInfo)
        Next
        Return dashboardInfos
    End Function

    Public Function LoadDashboard(dashboardID As String) As XDocument Implements IDashboardStorage.LoadDashboard
        DashboardTable = New DataTable
        If CommonMethods.dbtype = "sql" Then
            Dim sql As String = "Select * from Dashboards"
            Dim ds As DataSet = (New SQLExec).Cursor(sql)
            DashboardTable = ds.Tables(0)
            Dim currentRow As DataRow = (From row In DashboardTable.AsEnumerable() Where row.Field(Of Integer)("DashboardID") = Integer.Parse(dashboardID) Select row).FirstOrDefault()
            If currentRow IsNot Nothing Then
                Dim dashboardXml As XDocument = XDocument.Parse(currentRow("DashboardXml").ToString)
                Return dashboardXml
            End If
        End If
        Return Nothing
    End Function

    Public Sub SaveDashboard(dashboardID As String, dashboard As XDocument) Implements IDashboardStorage.SaveDashboard
        If CommonMethods.dbtype = "sql" Then
            Dim filteredXmlData As String = dashboard.ToString.Replace("'", "''")
            Dim title As String = (CType((CType(dashboard.FirstNode, XContainer)).FirstNode, XElement)).LastAttribute.Value
            Dim update As String = " update Dashboards set DashboardXml =  '" & filteredXmlData & "', DashboardName = '" & title & "'"
            update += " where DashboardID = " & dashboardID
            update += " update PROFILEDETAILDASHBOARDS set DASHBOARD_NAME = '" & title & "'  where DASHBOARD = " & dashboardID
            Dim tmp As String = (New SQLExec).Execute(update)
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Function checkDashBoardExist(ByVal name As String) As Integer
        Dim exist As Integer = 0
        If CommonMethods.dbtype = "sql" Then
            Dim sql As String = "Select * from dbo.Dashboards where DashboardName= '" & name & "'"
            Dim ds As DataSet = (New SQLExec).Cursor(sql)
            exist = ds.Tables(0).Rows.Count
        End If
        Return exist
    End Function

    Private Function getProfiles() As DataTable
        Dim datatable As DataTable = New DataTable
        If CommonMethods.dbtype = "sql" Then
            Dim sql As String = "select distinct profilename from dbo.PROFILES"
            Dim ds As DataSet = (New SQLExec).Cursor(sql)
            datatable = ds.Tables(0)
        End If
        Return datatable
    End Function

    Private Function getUserProfiles() As DataTable
        Dim datatable As DataTable = New DataTable
        If CommonMethods.dbtype = "sql" Then
            Dim sql As String = "select distinct profilename from dbo.USERPROFILE where userkey = '" & userName & "'"
            Dim ds As DataSet = (New SQLExec).Cursor(sql)
            datatable = ds.Tables(0)
        End If
        Return datatable
    End Function

#End Region

End Class