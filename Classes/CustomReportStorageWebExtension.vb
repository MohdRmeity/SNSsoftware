Imports DevExpress.XtraReports.UI
Imports NLog
Imports Oracle.ManagedDataAccess.Client
Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Linq
Imports System.ServiceModel

Public Class CustomReportStorageWebExtension
    Inherits DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension

    Public Sub New()
        UserInfo.ReportID = ""



    End Sub

    Public Overrides Function CanSetData(ByVal url As String) As Boolean
        Dim res As String = GetReportByID(url).AsEnumerable().[Select](Function(x) x.Field(Of String)("DisplayName")).FirstOrDefault()

        If res Is Nothing Then
            Return False
        End If

        UserInfo.ReportID = url
        Return True
    End Function

    Public Overrides Function GetData(ByVal url As String) As Byte()
        UserInfo.ReportID = url
        Return GetReportByID(url).AsEnumerable().[Select](Function(x) x.Field(Of Byte())("LayoutData")).FirstOrDefault()
    End Function

    Public Overrides Function GetUrls() As Dictionary(Of String, String)
        Dim reportTb As DataTable = New DataTable()
        Dim connString As String = ConfigurationManager.ConnectionStrings("DBConnect").ConnectionString

        If (ConfigurationManager.AppSettings("DatabaseType")).ToLower() = "sql" Then
            Dim query As String = "select distinct (REPORT) , REPORT_NAME  from dbo.REPORTSPROFILEDETAIL where  EDIT='1' and profilename in (select profilename from  dbo.USERPROFILE where userkey= @ukey ) order by REPORT_NAME"

            Try

                Using connection As SqlConnection = New SqlConnection(connString)
                    connection.Open()
                    Dim cmd As SqlCommand = New SqlCommand(query, connection)

                    cmd.Parameters.AddWithValue("@ukey", UserInfo.LoginUser)

                    Using reportdata As SqlDataAdapter = New SqlDataAdapter()
                        reportdata.SelectCommand = cmd
                        reportdata.Fill(reportTb)
                    End Using

                    connection.Close()
                End Using

            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.[Error](exp, "", "")
                Return Nothing
            End Try
        Else
            Dim query As String = "select distinct (REPORT) , REPORT_NAME  from system.REPORTSPROFILEDETAIL where  EDIT='1' and profilename in (select profilename from  system.USERPROFILE where userkey= :ukey )"

            Try

                Using connection As OracleConnection = New OracleConnection(connString)
                    connection.Open()
                    Dim cmd As OracleCommand = New OracleCommand(query, connection)
                    cmd.Parameters.Add("ukey", UserInfo.LoginUser)

                    Using reportdata As OracleDataAdapter = New OracleDataAdapter()
                        reportdata.SelectCommand = cmd
                        reportdata.Fill(reportTb)
                    End Using

                    connection.Close()
                End Using

            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.[Error](exp, "", "")
                Return Nothing
            End Try
        End If

        Dim reportDic As Dictionary(Of String, String) = New Dictionary(Of String, String)()

        For Each row As DataRow In reportTb.Rows
            Dim reportID As String = row("REPORT").ToString()
            Dim reportName As String = GetReportByID(reportID).AsEnumerable().[Select](Function(x) x.Field(Of String)("DisplayName")).FirstOrDefault()
            reportDic.Add(reportID, reportName)
        Next

        Return reportDic
    End Function

    Public Overrides Function IsValidUrl(ByVal url As String) As Boolean
        Dim n As Integer
        UserInfo.ReportID = url
        Return Integer.TryParse(url, n)
    End Function

    Public Overrides Sub SetData(ByVal report As XtraReport, ByVal reportID As String)
        Dim connString As String = ConfigurationManager.ConnectionStrings("DBConnect").ConnectionString

        If (ConfigurationManager.AppSettings("DatabaseType")).ToLower() = "sql" Then
            Dim reportLayout As Byte()

            Using ms As MemoryStream = New MemoryStream()
                report.SaveLayoutToXml(ms)
                reportLayout = ms.GetBuffer()
            End Using

            Dim query As String = "update dbo.REPORTSPORTAL set LayoutData =  @pkey , EDITDATE =@EDITDATE, EDITWHO=@EDITWHO where ReportID =" & reportID

            Try
                Dim conn As SqlConnection = New SqlConnection(connString)
                conn.Open()
                Dim cmd As SqlCommand = New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@pkey", reportLayout)
                cmd.Parameters.AddWithValue("@EDITDATE", Date.Now)
                cmd.Parameters.AddWithValue("@EDITWHO", UserInfo.LoginUser)
                cmd.ExecuteNonQuery()
                conn.Close()
            Catch e1 As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.[Error](e1, "", "")
            End Try
        Else
            Dim reportLayout As Byte()

            Using ms As MemoryStream = New MemoryStream()
                report.SaveLayoutToXml(ms)
                reportLayout = ms.GetBuffer()
            End Using

            Dim query As String = "update SYSTEM.REPORTSPORTAL set LayoutData =  :pkey , EDITDATE =:EDITDATE, EDITWHO=:EDITWHO where ReportID =" & reportID

            Try
                Dim conn As OracleConnection = New OracleConnection(connString)
                conn.Open()
                Dim cmd As OracleCommand = New OracleCommand(query, conn)
                cmd.Parameters.Add("pkey", reportLayout)
                cmd.Parameters.Add("EDITDATE", Date.Now)
                cmd.Parameters.Add("EDITWHO", UserInfo.LoginUser)
                cmd.ExecuteNonQuery()
                conn.Close()
            Catch e1 As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.[Error](e1, "", "")
            End Try
        End If

        UserInfo.ReportID = reportID
    End Sub

    Public Overrides Function SetNewData(ByVal report As XtraReport, ByVal defaultUrl As String) As String
        If checkReportExist(defaultUrl) = 0 Then
            report.DisplayName = defaultUrl
            report.Name = defaultUrl
            Dim connString As String = ConfigurationManager.ConnectionStrings("DBConnect").ConnectionString

            If ConfigurationManager.AppSettings("DatabaseType").ToLower() = "sql" Then
                Dim insert As String = " INSERT INTO dbo.REPORTSPORTAL (LayoutData,DisplayName,ADDDATE,ADDWHO,EDITDATE,EDITWHO ) VALUES (@LayoutData, @name,@ADDDATE,@ADDWHO,@EDITDATE,@EDITWHO) "

                Try
                    Dim conn As SqlConnection = New SqlConnection(connString)
                    conn.Open()
                    Dim id As Integer = 0
                    Dim reportLayout As Byte()

                    Using ms As MemoryStream = New MemoryStream()
                        report.SaveLayoutToXml(ms)
                        reportLayout = ms.GetBuffer()
                    End Using

                    Dim cmd2 As SqlCommand = New SqlCommand(insert, conn)
                    cmd2.Parameters.AddWithValue("@LayoutData", reportLayout)
                    cmd2.Parameters.AddWithValue("@name", defaultUrl)
                    cmd2.Parameters.AddWithValue("@ADDDATE", Date.Now)
                    cmd2.Parameters.AddWithValue("@ADDWHO", UserInfo.LoginUser)
                    cmd2.Parameters.AddWithValue("@EDITDATE", Date.Now)
                    cmd2.Parameters.AddWithValue("@EDITWHO", UserInfo.LoginUser)
                    cmd2.ExecuteNonQuery()
                    conn.Close()
                    Dim query As String = "SELECT TOP(1) [ReportID] FROM[REPORTSPORTAL] ORDER BY 1 DESC"

                    Using connection As SqlConnection = New SqlConnection(connString)
                        connection.Open()
                        Dim cmd As SqlCommand = New SqlCommand(query, connection)
                        Dim dt As DataTable = New DataTable()

                        Using orderdata As SqlDataAdapter = New SqlDataAdapter()
                            orderdata.SelectCommand = cmd
                            orderdata.Fill(dt)

                            For Each row As DataRow In dt.Rows
                                id = Integer.Parse(row(0).ToString())
                            Next
                        End Using

                        connection.Close()
                    End Using

                    Dim dtProfiles As DataTable = getProfiles()
                    Dim userPrfiles As DataTable = getUserProfiles(UserInfo.LoginUser)

                    For Each row As DataRow In dtProfiles.Rows
                        Dim profile As String = row(0).ToString()
                        insert = "INSERT INTO [dbo].[REPORTSPROFILEDETAIL] ([PROFILENAME],[REPORT],[REPORT_NAME],[EDIT],[ADDDATE],[ADDWHO],[EDITDATE] ,[EDITWHO]) Values (@PROFILENAME, @REPORT,@REPORT_NAME,@EDIT,@ADDDATE,@ADDWHO,@EDITDATE,@EDITWHO) "
                        Dim conn2 As SqlConnection = New SqlConnection(connString)
                        conn2.Open()
                        Dim cmd3 As SqlCommand = New SqlCommand(insert, conn2)
                        cmd3.Parameters.AddWithValue("@PROFILENAME", profile)
                        cmd3.Parameters.AddWithValue("@REPORT", id.ToString())
                        cmd3.Parameters.AddWithValue("@REPORT_NAME", defaultUrl)

                        If userPrfiles.AsEnumerable().Any(Function(row1) profile = row1.Field(Of String)("profilename")) Then
                            cmd3.Parameters.AddWithValue("@EDIT", "1")
                        Else
                            cmd3.Parameters.AddWithValue("@EDIT", "0")
                        End If

                        cmd3.Parameters.AddWithValue("@ADDDATE", Date.Now)
                        cmd3.Parameters.AddWithValue("@ADDWHO", UserInfo.LoginUser)
                        cmd3.Parameters.AddWithValue("@EDITDATE", Date.Now)
                        cmd3.Parameters.AddWithValue("@EDITWHO", UserInfo.LoginUser)
                        cmd3.ExecuteNonQuery()
                        conn2.Close()
                    Next

                    UserInfo.ReportID = id.ToString()
                    Return id.ToString()
                Catch e1 As Exception
                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                    logger.[Error](e1, "", "")
                End Try
            Else

                Try
                    Dim id As Integer = 0
                    Dim query As String = " INSERT INTO system.REPORTSPORTAL (LayoutData,DisplayName,ADDDATE,ADDWHO,EDITDATE,EDITWHO ) VALUES (:LayoutData, :name,:ADDDATE,:ADDWHO,:EDITDATE,:EDITWHO) "
                    Dim reportLayout As Byte()

                    Using ms As MemoryStream = New MemoryStream()
                        report.SaveLayoutToXml(ms)
                        reportLayout = ms.GetBuffer()
                    End Using

                    Dim cmdOracle As OracleCommand = New OracleCommand()
                    cmdOracle.Parameters.Add("LayoutData", reportLayout)
                    cmdOracle.Parameters.Add("name", defaultUrl)
                    cmdOracle.Parameters.Add("ADDDATE", Date.Now)
                    cmdOracle.Parameters.Add("ADDWHO", UserInfo.LoginUser)
                    cmdOracle.Parameters.Add("EDITDATE", Date.Now)
                    cmdOracle.Parameters.Add("EDITWHO", UserInfo.LoginUser)
                    Dim conn As OracleConnection = New OracleConnection(connString)
                    conn.Open()
                    cmdOracle.CommandText = query
                    cmdOracle.Connection = conn
                    cmdOracle.ExecuteNonQuery()
                    conn.Close()
                    Dim query2 As String = "SELECT REPORTID FROM SYSTEM.REPORTSPORTAL order by reportid DESC FETCH NEXT 1 ROWS ONLY"

                    Using connection As OracleConnection = New OracleConnection(connString)
                        connection.Open()
                        Dim cmd As OracleCommand = New OracleCommand(query2, connection)
                        Dim dt As DataTable = New DataTable()

                        Using orderdata As OracleDataAdapter = New OracleDataAdapter()
                            orderdata.SelectCommand = cmd
                            orderdata.Fill(dt)

                            For Each row As DataRow In dt.Rows
                                id = Integer.Parse(row(0).ToString())
                            Next
                        End Using

                        connection.Close()
                    End Using

                    Dim dtProfiles As DataTable = getProfiles()
                    Dim userPrfiles As DataTable = getUserProfiles(UserInfo.LoginUser)

                    For Each row As DataRow In dtProfiles.Rows
                        Dim profile As String = row(0).ToString()
                        Dim insert As String = "INSERT INTO SYSTEM.REPORTSPROFILEDETAIL (PROFILENAME,REPORT,REPORT_NAME,EDIT,ADDDATE,ADDWHO,EDITDATE ,EDITWHO) Values (:PROFILENAME, :REPORT, :REPORT_NAME, :EDIT, :ADDDATE, :ADDWHO, :EDITDATE, :EDITWHO) "
                        Dim conn2 As OracleConnection = New OracleConnection(connString)
                        conn2.Open()
                        Dim cmd3 As OracleCommand = New OracleCommand(insert, conn2)
                        cmd3.Parameters.Add("PROFILENAME", profile)
                        cmd3.Parameters.Add("REPORT", id.ToString())
                        cmd3.Parameters.Add("REPORT_NAME", defaultUrl)

                        If userPrfiles.AsEnumerable().Any(Function(row1) profile = row1.Field(Of String)("profilename")) Then
                            cmd3.Parameters.Add("EDIT", "1")
                        Else
                            cmd3.Parameters.Add("EDIT", "0")
                        End If

                        cmd3.Parameters.Add("ADDDATE", Date.Now)
                        cmd3.Parameters.Add("ADDWHO", UserInfo.LoginUser)
                        cmd3.Parameters.Add("EDITDATE", Date.Now)
                        cmd3.Parameters.Add("EDITWHO", UserInfo.LoginUser)
                        cmd3.ExecuteNonQuery()
                        conn2.Close()
                    Next

                    UserInfo.ReportID = id.ToString()
                    Return id.ToString()
                Catch e1 As Exception
                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                    logger.[Error](e1, "", "")
                End Try
            End If
        Else
            Dim ex As FaultException = New FaultException(" Report name is already used")
            Throw ex
        End If

        Return ""
    End Function

    Public Function getProfiles() As DataTable
        Dim connString As String = ConfigurationManager.ConnectionStrings("DBConnect").ConnectionString
        Dim dt As DataTable = New DataTable()

        If (ConfigurationManager.AppSettings("DatabaseType")).ToLower() = "sql" Then
            Dim query As String = "select distinct (profilename )from  dbo.PROFILES "

            Try

                Using connection As SqlConnection = New SqlConnection(connString)
                    connection.Open()
                    Dim cmd As SqlCommand = New SqlCommand(query, connection)

                    Using orderdata As SqlDataAdapter = New SqlDataAdapter()
                        orderdata.SelectCommand = cmd
                        orderdata.Fill(dt)
                    End Using

                    connection.Close()
                End Using

            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.[Error](exp, "", "")
            End Try
        Else
            Dim query As String = "SELECT  distinct (PROFILENAME ) FROM SYSTEM.PROFILES"

            Try

                Using connection As OracleConnection = New OracleConnection(connString)
                    connection.Open()

                    Using oracleDataAdapter As OracleDataAdapter = New OracleDataAdapter(query, connection)
                        oracleDataAdapter.Fill(dt)
                    End Using

                    connection.Close()
                End Using

            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.[Error](exp, "", "")
            End Try
        End If

        Return dt
    End Function

    Public Function getUserProfiles(ByVal userkey As String) As DataTable
        Dim connString As String = ConfigurationManager.ConnectionStrings("DBConnect").ConnectionString
        Dim dt As DataTable = New DataTable()

        If (ConfigurationManager.AppSettings("DatabaseType")).ToLower() = "sql" Then
            Dim query As String = "select distinct (profilename ) from  dbo.USERPROFILE "

            Try

                Using connection As SqlConnection = New SqlConnection(connString)
                    connection.Open()
                    Dim cmd As SqlCommand = New SqlCommand(query, connection)
                    cmd.Parameters.AddWithValue("@ukey", userkey)

                    Using sqlDataAdapter As SqlDataAdapter = New SqlDataAdapter()
                        sqlDataAdapter.SelectCommand = cmd
                        sqlDataAdapter.Fill(dt)
                    End Using

                    connection.Close()
                End Using

            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.[Error](exp, "", "")
            End Try
        Else
            Dim query As String = "select distinct (profilename ) from  system.USERPROFILE where USERKEY= :ukey"

            Try

                Using connection As OracleConnection = New OracleConnection(connString)
                    connection.Open()
                    Dim cmd As OracleCommand = New OracleCommand(query, connection)
                    cmd.Parameters.Add(New OracleParameter("ukey", userkey))

                    Using oracleDataAdapter As OracleDataAdapter = New OracleDataAdapter()
                        oracleDataAdapter.SelectCommand = cmd
                        oracleDataAdapter.Fill(dt)
                    End Using

                    connection.Close()
                End Using

            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.[Error](exp, "", "")
            End Try
        End If

        Return dt
    End Function

    Protected Function checkReportExist(ByVal name As String) As Integer
        Dim exist As Integer = 0
        Dim connString As String = ConfigurationManager.ConnectionStrings("DBConnect").ConnectionString

        If (ConfigurationManager.AppSettings("DatabaseType")).ToLower() = "sql" Then
            Dim query As String = "select count(1)  from dbo.REPORTSPORTAL where DisplayName= @uk "

            Try

                Using connection As SqlConnection = New SqlConnection(connString)
                    connection.Open()
                    Dim cmd As SqlCommand = New SqlCommand(query, connection)
                    cmd.Parameters.AddWithValue("@uk", name)

                    Using reportPortal As SqlDataAdapter = New SqlDataAdapter()
                        Dim dt As DataTable = New DataTable()
                        reportPortal.SelectCommand = cmd
                        reportPortal.Fill(dt)

                        For Each row As DataRow In dt.Rows
                            exist = Integer.Parse(row(0).ToString())
                        Next
                    End Using

                    connection.Close()
                End Using

            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.[Error](exp, "", "")
            End Try
        Else
            Dim query As String = "select count(1)  from SYSTEM.REPORTSPORTAL where DisplayName= :displayname"

            Try

                Using connection As OracleConnection = New OracleConnection(connString)
                    connection.Open()
                    Dim cmd As OracleCommand = New OracleCommand(query, connection)
                    cmd.Parameters.Add(New OracleParameter("displayname", name))

                    Using reportPortal As OracleDataAdapter = New OracleDataAdapter()
                        Dim dt As DataTable = New DataTable()
                        reportPortal.SelectCommand = cmd
                        reportPortal.Fill(dt)

                        For Each row As DataRow In dt.Rows
                            exist = Integer.Parse(row(0).ToString())
                        Next
                    End Using

                    connection.Close()
                End Using

            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.[Error](exp, "", "")
            End Try
        End If

        Return exist
    End Function

    Public Function GetReportByID(ByVal reportID As String) As DataTable
        Try
            Dim reportPortaldTb As DataTable = New DataTable()
            Dim connString As String = ConfigurationManager.ConnectionStrings("DBConnect").ConnectionString
            Dim query As String = ""

            If (ConfigurationManager.AppSettings("DatabaseType")).ToLower() = "sql" Then
                query = "select * from dbo.REPORTSPORTAL where ReportID = " & reportID

                Try

                    Using connection As SqlConnection = New SqlConnection(connString)
                        connection.Open()

                        Using sqlDataAdapter As SqlDataAdapter = New SqlDataAdapter(query, connection)
                            sqlDataAdapter.Fill(reportPortaldTb)
                        End Using

                        connection.Close()
                    End Using

                Catch exp As Exception
                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                    logger.[Error](exp, "", "")
                End Try
            Else
                query = "select * from system.REPORTSPORTAL where ReportID = :reportID"

                Using connection As OracleConnection = New OracleConnection(connString)
                    connection.Open()
                    Dim cmd As OracleCommand = New OracleCommand(query, connection)
                    cmd.Parameters.Add(New OracleParameter("reportID", reportID))

                    Using sqlDataAdapter As OracleDataAdapter = New OracleDataAdapter()
                        sqlDataAdapter.SelectCommand = cmd
                        sqlDataAdapter.Fill(reportPortaldTb)
                    End Using

                    connection.Close()
                End Using
            End If

            Return reportPortaldTb
        Catch exp As Exception
            Dim logger As Logger = LogManager.GetCurrentClassLogger()
            logger.[Error](exp, "", "")
            Return Nothing
        End Try
    End Function
End Class
