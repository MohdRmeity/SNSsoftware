Imports System.CodeDom
Imports System.CodeDom.Compiler
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Reflection
Imports System.Security.Cryptography
Imports System.Web.Services.Description
Imports System.Xml
Imports System.Xml.Serialization
Imports NLog
Imports Oracle.ManagedDataAccess.Client

Public Class CommonMethods
    Public Shared dbconx As String = ConfigurationManager.ConnectionStrings("DBConnect").ConnectionString
    Public Shared dbtype As String = LCase(ConfigurationManager.AppSettings("DatabaseType"))
    Public Shared username As String = ConfigurationManager.AppSettings("username")
    Public Shared password As String = ConfigurationManager.AppSettings("password")
    Public Shared wmslink As String = ConfigurationManager.AppSettings("WMSLink")
    Public Shared version As String = ConfigurationManager.AppSettings("version")
    Public Shared dformat As String = ConfigurationManager.AppSettings("datefday")
    Public Shared datef As String = ConfigurationManager.AppSettings("datef")
    Public Shared DashboardRefreshTime As String = ConfigurationManager.AppSettings("DashboardRefreshTimeInSeconds")
    Public Shared TopCount As String = ConfigurationManager.AppSettings("TopCount")
    Public Shared Function AreEqual(ByVal plainTextInput As String, ByVal hashedInput As String, ByVal salt As String) As Boolean
        Dim newHashedPin As String = GenerateHash(plainTextInput, salt)
        Return newHashedPin.Equals(hashedInput)
    End Function
    Public Shared Function GenerateHash(ByVal input As String, ByVal salt As String) As String
        Dim bytes As Byte() = Encoding.UTF8.GetBytes(input & salt)
        Dim sHA256ManagedString As SHA256Managed = New SHA256Managed
        Dim hash As Byte() = sHA256ManagedString.ComputeHash(bytes)
        Return System.Convert.ToBase64String(hash)
    End Function
    Public Shared Sub incremenetLogins(ByVal key As String)
        Dim hostName As String = Dns.GetHostName()
        Console.WriteLine(hostName)
        Dim myIP As String = Dns.GetHostEntry(hostName).AddressList(0).ToString()
        If dbtype = "sql" Then
            Dim updt As String = "UPDATE dbo.portalusers set trial=trial+1  where userkey= @ukey "
            Dim insert As String = " INSERT INTO dbo.LOGINLOGS (USERKEY,IP ) VALUES (@ukey, @ip) "
            Try
                Dim conn As SqlConnection = New SqlConnection(dbconx)
                conn.Open()
                Dim cmd As SqlCommand = New SqlCommand(updt, conn)
                cmd.Parameters.AddWithValue("@ukey", key)
                cmd.ExecuteNonQuery()
                Dim cmd2 As SqlCommand = New SqlCommand(insert, conn)
                cmd2.Parameters.AddWithValue("@ukey", key)
                cmd2.Parameters.AddWithValue("@ip", myIP)
                cmd2.ExecuteNonQuery()
                conn.Close()
            Catch e1 As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(e1, "", "")
            End Try
        Else
            Dim updt As String = "UPDATE system.portalusers set trial=trial+1  where userkey= :userkey "
            Dim insert As String = " INSERT INTO system.LOGINLOGS (USERKEY,IP, adddate ) VALUES (:userkey, :ip, SYSDATE) "
            Try
                Dim conn As OracleConnection = New OracleConnection(dbconx)
                conn.Open()
                Dim cmd As OracleCommand = New OracleCommand(updt, conn)
                cmd.Parameters.Add(New OracleParameter("userkey", key))
                cmd.ExecuteNonQuery()
                Dim cmd2 As OracleCommand = New OracleCommand(insert, conn)
                cmd2.Parameters.Add(New OracleParameter("userkey", key))
                cmd2.Parameters.Add(New OracleParameter("ip", myIP))
                cmd2.ExecuteNonQuery()
                conn.Close()
            Catch e1 As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(e1, "", "")
            End Try
        End If
    End Sub
    Public Shared Function checkPassComplexity(ByVal pass As String) As Boolean
        Dim regex1 As Regex = New Regex("[0-9]")
        If pass.Length > 9 And pass.Any(AddressOf Char.IsUpper) And pass.Any(AddressOf Char.IsLower) And regex1.IsMatch(pass) Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Shared Function CreateSalt(ByVal size As Integer) As String
        Dim rng As RNGCryptoServiceProvider = New RNGCryptoServiceProvider()
        Dim buff As Byte() = New Byte(size - 1) {}
        rng.GetBytes(buff)
        Return System.Convert.ToBase64String(buff)
    End Function
    Public Shared Function getLockedOut(ByVal ukey As String) As Boolean
        Dim hashed As String = ""
        Dim dt As DataTable = New DataTable
        If dbtype = "sql" Then
            Try
                Using connection As SqlConnection = New SqlConnection(dbconx)
                    connection.Open()
                    Dim query As SqlCommand = New SqlCommand("select trial from dbo.portalusers where  USERKEY=@user and trial>3 and 5>(select TOP 1 DATEDIFF(mi, adddate, GETDATE())  from dbo.LOGINLOGS where USERKEY=@userk order by adddate desc)", connection)
                    query.Parameters.AddWithValue("@user", LCase(ukey))
                    query.Parameters.AddWithValue("@userk", LCase(ukey))
                    Using people As SqlDataAdapter = New SqlDataAdapter
                        people.SelectCommand = query
                        people.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            hashed = row("trial").ToString
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
            End Try
        Else
            Try
                Using connection As OracleConnection = New OracleConnection(dbconx)
                    connection.Open()
                    Dim query As OracleCommand = New OracleCommand("select trial from system.portalusers where  USERKEY=:userk and trial>3 and  (SELECT round((sysdate - adddate) * 24 * 60)  FROM(SELECT adddate FROM system.LOGINLOGS where USERKEY =:userkk ORDER BY adddate DESC) WHERE ROWNUM = 1) < 5", connection)
                    query.Parameters.Add(New OracleParameter("userk", LCase(ukey)))
                    query.Parameters.Add(New OracleParameter("userkk", LCase(ukey)))
                    Using orderdata As OracleDataAdapter = New OracleDataAdapter
                        orderdata.SelectCommand = query
                        orderdata.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            hashed = row("trial").ToString
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
            End Try
        End If

        If Not String.IsNullOrEmpty(hashed) Then
            Return True
        End If

        Return False
    End Function
    Public Shared Sub resettrials(ByVal key As String)

        If dbtype = "sql" Then
            Dim updt As String = "UPDATE dbo.portalusers set trial=0  where userkey= @ukey"
            Try
                Dim conn As SqlConnection = New SqlConnection(dbconx)
                conn.Open()
                Dim cmd As SqlCommand = New SqlCommand(updt, conn)
                cmd.Parameters.AddWithValue("@ukey", key)
                cmd.ExecuteNonQuery()
                conn.Close()
            Catch e1 As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(e1, "", "")
            End Try
        Else
            Dim updt As String = "UPDATE system.portalusers set trial=0  where userkey= :userkey"
            Try
                Dim conn As OracleConnection = New OracleConnection(dbconx)
                conn.Open()
                Dim cmd As OracleCommand = New OracleCommand(updt, conn)
                cmd.Parameters.Add(New OracleParameter("userkey", key))
                cmd.ExecuteNonQuery()
                conn.Close()
            Catch e1 As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(e1, "", "")
            End Try
        End If
    End Sub
    Public Shared Function getPassword(ByVal ukey As String) As String
        Dim hashed As String = ""
        Dim dt As DataTable = New DataTable
        If dbtype = "sql" Then
            Try
                Using connection As SqlConnection = New SqlConnection(dbconx)
                    connection.Open()
                    Dim query As SqlCommand = New SqlCommand("select PASSWORD from dbo.PORTALUSERS where USERKEY=@user", connection)
                    query.Parameters.AddWithValue("@user", LCase(ukey))
                    Using people As SqlDataAdapter = New SqlDataAdapter
                        people.SelectCommand = query
                        people.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            hashed = row("PASSWORD").ToString
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
            End Try
        Else
            Try
                Using connection As OracleConnection = New OracleConnection(dbconx)
                    connection.Open()
                    Dim query As OracleCommand = New OracleCommand("select PASSWORD from SYSTEM.PORTALUSERS where USERKEY = :Userk", connection)
                    query.Parameters.Add(New OracleParameter("Userk", LCase(ukey)))
                    Using orderdata As OracleDataAdapter = New OracleDataAdapter
                        orderdata.SelectCommand = query
                        orderdata.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            hashed = row("PASSWORD").ToString
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
            End Try
        End If

        Return hashed
    End Function
    Public Shared Function getpasskey(ByVal ukey As String) As String
        Dim hashed As String = ""
        Dim dt As DataTable = New DataTable
        If dbtype = "sql" Then
            Try
                Using connection As SqlConnection = New SqlConnection(dbconx)
                    connection.Open()
                    Dim query As SqlCommand = New SqlCommand("select HASHKEY from dbo.PORTALUSERS where USERKEY=@user", connection)
                    query.Parameters.AddWithValue("@user", LCase(ukey))
                    Using people As SqlDataAdapter = New SqlDataAdapter
                        people.SelectCommand = query
                        people.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            hashed = row("HASHKEY").ToString
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
            End Try
        Else
            Try
                Using connection As OracleConnection = New OracleConnection(dbconx)
                    connection.Open()
                    Dim query As OracleCommand = New OracleCommand("select HASHKEY from SYSTEM.PORTALUSERS where USERKEY = :Userk", connection)
                    query.Parameters.Add(New OracleParameter("Userk", LCase(ukey)))
                    Using orderdata As OracleDataAdapter = New OracleDataAdapter
                        orderdata.SelectCommand = query
                        orderdata.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            hashed = row("HASHKEY").ToString
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
            End Try
        End If

        Return hashed
    End Function
    Public Shared Sub SendEmail(ByVal toAddress As MailAddress, ByVal subject As String, ByVal body As String)
        Try
            Dim fromAddress = New MailAddress(ConfigurationManager.AppSettings("smtp_email"), "SNS Support")
            Dim fromPassword As String = ConfigurationManager.AppSettings("smtp_password")
            Dim smtp = New SmtpClient With {
                .Host = ConfigurationManager.AppSettings("smtp_host"),
                .Port = Integer.Parse(ConfigurationManager.AppSettings("smtp_port")),
                .EnableSsl = True,
                .DeliveryMethod = SmtpDeliveryMethod.Network,
                .UseDefaultCredentials = False,
                .Credentials = New NetworkCredential(fromAddress.Address, fromPassword)
            }
            Dim message = New MailMessage(fromAddress, toAddress) With {
                .Subject = subject,
                .Body = body,
                .IsBodyHtml = True
            }
            smtp.Send(message)
        Catch e1 As Exception
            Dim logger As Logger = LogManager.GetCurrentClassLogger()
            logger.Error(e1, "", "")
        End Try
    End Sub
    Public Shared Function checkUserExist(ByVal where As String) As Integer
        Dim exist As Integer = 0
        If dbtype = "sql" Then
            Dim query As String = "select count(1) from dbo.PORTALUSERS where USERKEY= @uk "
            Try
                Using connection As SqlConnection = New SqlConnection(dbconx)
                    connection.Open()
                    Dim cmd As SqlCommand = New SqlCommand(query, connection)
                    cmd.Parameters.AddWithValue("@uk", where)
                    Using people As SqlDataAdapter = New SqlDataAdapter
                        Dim dt As DataTable = New DataTable
                        people.SelectCommand = cmd
                        people.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            exist = Integer.Parse(row(0).ToString)
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
            End Try
        Else
            Dim query As String = "select count(1) from SYSTEM.PORTALUSERS where USERKEY= :ukey"
            Try
                Using connection As OracleConnection = New OracleConnection(dbconx)
                    connection.Open()
                    Dim cmd As OracleCommand = New OracleCommand(query, connection)
                    cmd.Parameters.Add(New OracleParameter("ukey", where))
                    Using people As OracleDataAdapter = New OracleDataAdapter
                        Dim dt As DataTable = New DataTable
                        people.SelectCommand = cmd
                        people.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            exist = Integer.Parse(row(0).ToString)
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
            End Try
        End If
        Return exist
    End Function
    Public Shared Function getPermission(ByVal button As String, ByVal userkey As String) As String
        Dim edit As String = "1"
        Dim dt As DataTable = New DataTable
        If dbtype = "sql" Then
            Try
                Using connection As SqlConnection = New SqlConnection(dbconx)
                    connection.Open()
                    Dim query As SqlCommand = New SqlCommand("select top(1) EDIT from dbo.PROFILEDETAIL where SCREENBUTTONNAME=@buttonk and profilename in (select profilename from dbo.USERPROFILE where USERKEY= @ukey ) order by edit desc", connection)
                    query.Parameters.AddWithValue("@buttonk", button)
                    query.Parameters.AddWithValue("@ukey", userkey)
                    Using people As SqlDataAdapter = New SqlDataAdapter
                        people.SelectCommand = query
                        people.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            edit = row(0).ToString
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
            End Try
        Else
            Try
                Using connection As OracleConnection = New OracleConnection(dbconx)
                    connection.Open()
                    Dim query As OracleCommand = New OracleCommand("select MAX(EDIT) from SYSTEM.PROFILEDETAIL where  SCREENBUTTONNAME= :buttonk  and profilename in (select profilename from SYSTEM.USERPROFILE where USERKEY= :ukey)", connection)
                    query.Parameters.Add(New OracleParameter("buttonk", button))
                    query.Parameters.Add(New OracleParameter("ukey", userkey))
                    Using orderdata As OracleDataAdapter = New OracleDataAdapter
                        orderdata.SelectCommand = query
                        orderdata.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            edit = row("trial").ToString
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
            End Try
        End If
        Return edit
    End Function
    Public Shared Function getOwnerPerUser(ByVal userkey As String) As String()
        Dim owners As String()
        Dim s As String = ""
        Dim dt As DataTable = New DataTable
        If dbtype = "sql" Then
            Try
                Using connection As SqlConnection = New SqlConnection(dbconx)
                    connection.Open()
                    Dim query As SqlCommand = New SqlCommand("select STORERKEY  from dbo.USERCONTROL where USERKEY = @ukey", connection)
                    query.Parameters.AddWithValue("@ukey", userkey)
                    Using people As SqlDataAdapter = New SqlDataAdapter
                        people.SelectCommand = query
                        people.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            s = row("STORERKEY").ToString
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
            End Try
        Else
            Try
                Using connection As OracleConnection = New OracleConnection(dbconx)
                    connection.Open()
                    Dim query As OracleCommand = New OracleCommand("select STORERKEY from SYSTEM.USERCONTROL where USERKEY = :ukey", connection)
                    query.Parameters.Add(New OracleParameter("ukey", userkey))
                    Using orderdata As OracleDataAdapter = New OracleDataAdapter
                        orderdata.SelectCommand = query
                        orderdata.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            s = row("STORERKEY").ToString
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
            End Try
        End If
        If s.Equals("") Then
            owners = Nothing
        Else
            owners = s.Split(","c)
        End If
        Return owners
    End Function
    Public Shared Function getSupplierPerUser(ByVal userkey As String) As String()
        Dim suppliers As String()
        Dim s As String = ""
        Dim dt As DataTable = New DataTable
        If dbtype = "sql" Then
            Try
                Using connection As SqlConnection = New SqlConnection(dbconx)
                    connection.Open()
                    Dim query As SqlCommand = New SqlCommand("select SUPPLIERKEY from dbo.USERCONTROL where USERKEY = @ukey", connection)
                    query.Parameters.AddWithValue("@ukey", userkey)
                    Using people As SqlDataAdapter = New SqlDataAdapter
                        people.SelectCommand = query
                        people.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            s = row("SUPPLIERKEY").ToString
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
            End Try
        Else
            Try
                Using connection As OracleConnection = New OracleConnection(dbconx)
                    connection.Open()
                    Dim query As OracleCommand = New OracleCommand("select SUPPLIERKEY from SYSTEM.USERCONTROL where USERKEY = :ukey", connection)
                    query.Parameters.Add(New OracleParameter("ukey", userkey))
                    Using orderdata As OracleDataAdapter = New OracleDataAdapter
                        orderdata.SelectCommand = query
                        orderdata.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            s = row("SUPPLIERKEY").ToString
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
            End Try
        End If
        If s.Equals("") Then
            suppliers = Nothing
        Else
            suppliers = s.Split(","c)
        End If
        Return suppliers
    End Function
    Public Shared Function getConsigneePerUser(ByVal userkey As String) As String()
        Dim consignees As String()
        Dim s As String = ""
        Dim dt As DataTable = New DataTable
        If dbtype = "sql" Then
            Try
                Using connection As SqlConnection = New SqlConnection(dbconx)
                    connection.Open()
                    Dim query As SqlCommand = New SqlCommand("select CONSIGNEEKEY from dbo.USERCONTROL where USERKEY = @ukey", connection)
                    query.Parameters.AddWithValue("@ukey", userkey)
                    Using people As SqlDataAdapter = New SqlDataAdapter
                        people.SelectCommand = query
                        people.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            s = row("CONSIGNEEKEY").ToString
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
            End Try
        Else
            Try
                Using connection As OracleConnection = New OracleConnection(dbconx)
                    connection.Open()
                    Dim query As OracleCommand = New OracleCommand("select CONSIGNEEKEY from SYSTEM.USERCONTROL where USERKEY = :ukey", connection)
                    query.Parameters.Add(New OracleParameter("ukey", userkey))
                    Using orderdata As OracleDataAdapter = New OracleDataAdapter
                        orderdata.SelectCommand = query
                        orderdata.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            s = row("CONSIGNEEKEY").ToString
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
            End Try
        End If
        If s.Equals("") Then
            consignees = Nothing
        Else
            consignees = s.Split(","c)
        End If
        Return consignees
    End Function
    Public Shared Function getReportsPerUser(ByVal userkey As String) As String
        Dim reports As String = ""
        Dim sql As String = "select distinct(report) AS Report from " & IIf(dbtype <> "sql", "SYSTEM.", "") & "REPORTSPROFILEDETAIL where  EDIT=1 and profilename in (select profilename from " & IIf(dbtype <> "sql", "SYSTEM.", "") & "USERPROFILE where userkey= '" & userkey & "' )"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        For i = 0 To ds.Tables(0).Rows.Count - 1
            With ds.Tables(0).Rows(i)
                reports += IIf(i <> 0, ",", "") & "'" & !Report & "'"
            End With
        Next
        Return reports
    End Function
    Public Shared Function getButtons(ByVal profile As String) As DataTable
        Dim sql As String = ""
        If profile = "getAll" Then
            sql += "select BUTTON from " & IIf(dbtype <> "sql", "SYSTEM.", "") & "PORTALBUTTONS"
        Else
            sql += "select BUTTON  from " & IIf(dbtype <> "sql", "SYSTEM.", "") & "PORTALBUTTONS WHERE BUTTON NOT IN (SELECT SCREENBUTTONNAME FROM " & IIf(dbtype <> "sql", "SYSTEM.", "") & "PROFILEDETAIL WHERE PROFILENAME='" & profile & "')"
        End If
        sql += " ORDER BY BUTTON ASC "
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        Return ds.Tables(0)
    End Function
    Public Shared Function getReports(ByVal profile As String) As DataTable
        Dim sql As String = ""
        If profile = "getAll" Then
            sql += "select RPT_ID,RPT_TITLE  from " & IIf(dbtype <> "sql", "SYSTEM.", "") & "PORTALREPORTS"
        Else
            sql += "select RPT_ID,RPT_TITLE from " & IIf(dbtype <> "sql", "SYSTEM.", "") & "PORTALREPORTS WHERE RPT_ID NOT IN (SELECT REPORT FROM " & IIf(dbtype <> "sql", "SYSTEM.", "") & "PROFILEDETAILREPORTS WHERE PROFILENAME= '" & profile & "') order by RPT_ID ASC"
        End If
        sql += " ORDER BY RPT_ID ASC "
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        Return ds.Tables(0)
    End Function
    Public Shared Function getDashboards() As DataTable
        Dim sql As String = "select DashboardID,DashboardName from Dashboards ORDER BY DashboardID ASC"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        Return ds.Tables(0)
    End Function

    Public Shared Function CheckNameExist(ByVal pname As String) As Integer
        Dim counter As Integer = 0
        Dim sql As String = "select count(1) AS counter from " & IIf(dbtype <> "sql", "SYSTEM.", "") & "PROFILES where PROFILENAME = '" & pname & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then counter = Val(ds.Tables(0).Rows(0)!counter)
        Return counter
    End Function
    Public Shared Function checkItemExist(ByVal storer As String, ByVal sku As String) As Integer
        Dim counter As Integer = 0
        Dim sql As String = "select count(1) AS counter from enterprise.Sku where StorerKey = '" & storer & "' and Sku = '" & sku & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then counter = Val(ds.Tables(0).Rows(0)!counter)
        Return counter
    End Function
    Public Shared Function checkItemExist(ByVal storer As String, ByVal consignee As String, ByVal sku As String, ByVal currency As String) As Integer
        Dim counter As Integer = 0
        Dim sql As String = "select count(1) AS counter from " & IIf(dbtype <> "sql", "System.", "") & "SKUCATALOGUE where StorerKey = '" & storer & "' and ConsigneeKey = '" & consignee & "' and Sku = '" & sku & "' and Currency = '" & currency & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then counter = Val(ds.Tables(0).Rows(0)!counter)
        Return counter
    End Function
    Public Shared Function checkConfigurationExist(ByVal storer As String, ByVal type As String) As Integer
        Dim counter As Integer = 0
        Dim sql As String = "select count(1) AS counter from enterprise.storer where StorerKey = '" & storer & "' and type=" & type
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then counter = Val(ds.Tables(0).Rows(0)!counter)
        Return counter
    End Function
    Public Shared Function checkUserControlExist(ByVal userkey As String) As Integer
        Dim counter As Integer = 0
        Dim sql As String = "select count(1) AS counter from " & IIf(dbtype <> "sql", "System.", "") & "USERCONTROL where UserKey = '" & userkey & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then counter = Val(ds.Tables(0).Rows(0)!counter)
        Return counter
    End Function
    Public Shared Function checkPOExist(ByVal Facility As String, ByVal externpo As String) As Integer
        Dim counter As Integer = 0
        If LCase(Facility.Substring(0, 6)) = "infor_" Then Facility = Facility.Substring(6, Facility.Length - 6)
        If LCase(Facility).Contains("_") Then Facility = Facility.Split("_")(1)
        Dim sql As String = "select count(1) As Counter from " & Facility & ".po where ExternPOKey='" & externpo & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then counter = Val(ds.Tables(0).Rows(0)!counter)
        Return counter
    End Function
    Public Shared Function checkASNExist(ByVal Facility As String, ByVal externrkey As String) As Integer
        Dim counter As Integer = 0
        If LCase(Facility.Substring(0, 6)) = "infor_" Then Facility = Facility.Substring(6, Facility.Length - 6)
        If LCase(Facility).Contains("_") Then Facility = Facility.Split("_")(1)
        Dim sql As String = "select count(1) As Counter from " & Facility & ".receipt where ExternReceiptKey='" & externrkey & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then counter = Val(ds.Tables(0).Rows(0)!counter)
        Return counter
    End Function
    Public Shared Function checkSOExist(ByVal Facility As String, ByVal exterorderkey As String) As Integer
        Dim counter As Integer = 0
        If LCase(Facility.Substring(0, 6)) = "infor_" Then Facility = Facility.Substring(6, Facility.Length - 6)
        If LCase(Facility).Contains("_") Then Facility = Facility.Split("_")(1)
        Dim sql As String = "select count(1) As Counter from " & Facility & ".orders where ExternOrderKey='" & exterorderkey & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then counter = Val(ds.Tables(0).Rows(0)!counter)
        Return counter
    End Function
    Public Shared Function checkOrderManagementExist(ByVal Facility As String, ByVal exterorderkey As String) As Integer
        Dim counter As Integer = 0
        If LCase(Facility.Substring(0, 6)) = "infor_" Then Facility = Facility.Substring(6, Facility.Length - 6)
        If LCase(Facility).Contains("_") Then Facility = Facility.Split("_")(1)
        Dim sql As String = "select count(1) As Counter from " & Facility & ".orders where ExternOrderKey='" & exterorderkey & "'"
        sql += "select count(1) As Counter from " & IIf(dbtype <> "sql", "System.", "") & "ORDERMANAG where ExternOrderKey='" & exterorderkey & "' and WHSEID='" & Facility & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then counter = Val(ds.Tables(0).Rows(0)!counter)
        If counter = 1 Then Return counter
        If ds.Tables(1).Rows.Count > 0 Then counter = Val(ds.Tables(1).Rows(0)!counter)
        Return counter
    End Function
    Public Shared Function getEnterpriseDBName() As String
        Dim key As String = ""
        Dim sql As String = "select DB_NAME from wmsadmin.pl_db where isActive=1 and db_enterprise=1"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then key = ds.Tables(0).Rows(0)!DB_NAME.ToString
        Return key
    End Function
    Public Shared Function logger(ByVal facility As String, ByVal type As String, ByVal source As String, ByVal userkey As String) As String
        Dim myError As String = "", query As String = ""
        Dim cmd As SqlCommand = New SqlCommand
        Dim cmdOracle As OracleCommand = New OracleCommand
        Dim flag As String = loggerFlag()
        If flag = "1" Then
            If dbtype = "sql" Then
                query = "set dateformat dmy insert into  dbo.PORTALTRN (PORTALTRNKEY,FACILITY,TRNTYPE,TRANSACTIONDATE, TRNSOURCE, USERKEY)values((select  (RIGHT('0000000000' + CAST(ISNULL(max(PORTALTRNKEY)+1,1) AS varchar(10)) , 10) ) from dbo.PORTALTRN), @faci , @typestr ,GETDATE(), @sourcestr,@ukey);"
                cmd.Parameters.AddWithValue("@faci", facility)
                cmd.Parameters.AddWithValue("@typestr", type)
                cmd.Parameters.AddWithValue("@sourcestr", source)
                cmd.Parameters.AddWithValue("@ukey", userkey)
            Else
                query = "set dateformat dmy insert into  SYSTEM.PORTALTRN (PORTALTRNKEY,FACILITY,TRNTYPE,TRANSACTIONDATE, TRNSOURCE, USERKEY)values((select  (SUBSTR(CONCAT('0000000000' , CAST(NVL(max(cast(PORTALTRNKEY as integer))+1,1) AS nvarchar2(10))) , -10) ) from SYSTEM.PORTALTRN), :faci , :typestr ,SYSDATE, :sourcestr , :ukey )"
                cmdOracle.Parameters.Add(New OracleParameter("faci", facility))
                cmdOracle.Parameters.Add(New OracleParameter("typestr", type))
                cmdOracle.Parameters.Add(New OracleParameter("sourcestr", source))
                cmdOracle.Parameters.Add(New OracleParameter("ukey", userkey))
            End If
        Else
            Dim key As String = checkLastUpdate(facility, type, source, userkey)
            If Not String.IsNullOrEmpty(key) Then
                If dbtype = "sql" Then
                    query = "UPDATE dbo.PORTALTRN SET TRANSACTIONDATE = GETDATE() WHERE PORTALTRNKEY= @pkey"
                    cmd.Parameters.AddWithValue("@pkey", key)
                Else
                    query = "UPDATE SYSTEM.PORTALTRN SET TRANSACTIONDATE= SYSDATE WHERE PORTALTRNKEY= :pkey"
                    cmdOracle.Parameters.Add(New OracleParameter("pkey", key))
                End If
            Else
                If dbtype = "sql" Then
                    query = "set dateformat dmy insert into  dbo.PORTALTRN (PORTALTRNKEY,FACILITY,TRNTYPE,TRANSACTIONDATE, TRNSOURCE, USERKEY)values((select  (RIGHT('0000000000' + CAST(ISNULL(max(PORTALTRNKEY)+1,1) AS varchar(10)) , 10) ) from dbo.PORTALTRN), @faci, @typestr,GETDATE(), @sourcestr, @ukey );"
                    cmd.Parameters.AddWithValue("@faci", facility)
                    cmd.Parameters.AddWithValue("@typestr", type)
                    cmd.Parameters.AddWithValue("@sourcestr", source)
                    cmd.Parameters.AddWithValue("@ukey", userkey)
                Else
                    query = "set dateformat dmy insert into  SYSTEM.PORTALTRN (PORTALTRNKEY,FACILITY,TRNTYPE,TRANSACTIONDATE, TRNSOURCE, USERKEY)values((select  (SUBSTR(CONCAT('0000000000' , CAST(NVL(max(cast(PORTALTRNKEY as integer))+1,1) AS nvarchar2(10))) , -10) ) from SYSTEM.PORTALTRN), :faci , :typestr ,SYSDATE, :sourcestr, :ukey )"
                    cmdOracle.Parameters.Add(New OracleParameter("faci", facility))
                    cmdOracle.Parameters.Add(New OracleParameter("typestr", type))
                    cmdOracle.Parameters.Add(New OracleParameter("sourcestr", source))
                    cmdOracle.Parameters.Add(New OracleParameter("ukey", userkey))
                End If
            End If
        End If
        If dbtype = "sql" Then
            Try
                Dim conn As SqlConnection = New SqlConnection(dbconx)
                conn.Open()
                cmd.CommandText = query
                cmd.Connection = conn
                cmd.ExecuteNonQuery()
                conn.Close()
            Catch e1 As Exception
                myError = "Error: " & e1.Message & vbTab + e1.GetType.ToString & "<br/>"
                Dim mylogger As Logger = LogManager.GetCurrentClassLogger()
                mylogger.Error(e1, "", "")
            End Try
        Else
            Try
                Dim conn As OracleConnection = New OracleConnection(dbconx)
                conn.Open()
                cmdOracle.CommandText = query
                cmdOracle.Connection = conn
                cmdOracle.ExecuteNonQuery()
                conn.Close()
            Catch e1 As Exception
                myError = "Error: " & e1.Message & vbTab + e1.GetType.ToString & "<br/>"
                Dim mylogger As Logger = LogManager.GetCurrentClassLogger()
                mylogger.Error(e1, "", "")
            End Try
        End If
        Return myError
    End Function
    Public Shared Function loggerFlag() As String
        Dim flag As String = "0"
        Dim sql As String = ""
        If dbtype = "sql" Then
            sql += "SELECT TOP(1) NSQLVALUE FROM DBO.SYSCONFIG WHERE CONFIGKEY='LogEverySaveTransaction'"
        Else
            sql += "SELECT NSQLVALUE FROM SYSTEM.SYSCONFIG WHERE rownum=1 and CONFIGKEY='LogEverySaveTransaction' order by NSQLVALUE desc"
        End If
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then flag = ds.Tables(0).Rows(0)!NSQLVALUE.ToString
        Return flag
    End Function
    Public Shared Function checkLastUpdate(ByVal facility As String, ByVal type As String, ByVal source As String, ByVal userkey As String) As String
        Dim key As String = ""
        Dim sql As String = "SELECT MAX(PORTALTRNKEY) as MyKey FROM " & IIf(dbtype <> "sql", "System.", "") & "PORTALTRN WHERE FACILITY='" & facility & "' and TRNTYPE = '" & type & "' and TRNSOURCE = '" & source & "' and USERKEY = '" & userkey & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then key = ds.Tables(0).Rows(0)!MyKey.ToString
        Return key
    End Function
    Public Shared Function sendwebRequest(ByVal message As String) As String
        Dim soap As String = ""
        Dim client As WebClient = New WebClient
        client.Credentials = New NetworkCredential(username, password)
        'Connect To the web service
        'ServicePointManager.ServerCertificateValidationCallback = Function(se As Object, cert As System.Security.Cryptography.X509Certificates.X509Certificate, chain As System.Security.Cryptography.X509Certificates.X509Chain, sslerror As System.Net.Security.SslPolicyErrors) True
        ServicePointManager.ServerCertificateValidationCallback = Function(o, c, ch, er) True
        Dim stream As Stream = client.OpenRead(wmslink)
        'Now read the WSDL file describing a service.
        Dim description As ServiceDescription = ServiceDescription.Read(stream)
        'LOAD THE DOM 
        'Initialize a service description importer.
        Dim importer As ServiceDescriptionImporter = New ServiceDescriptionImporter
        importer.ProtocolName = "Soap"
        importer.AddServiceDescription(description, Nothing, Nothing)
        importer.Style = ServiceDescriptionImportStyle.Client
        'Generate properties to represent primitive values.
        importer.CodeGenerationOptions = System.Xml.Serialization.CodeGenerationOptions.GenerateProperties
        'Initialize a Code-DOM tree into which we will import the service.
        Dim nmspace As CodeNamespace = New CodeNamespace
        Dim unit1 As CodeCompileUnit = New CodeCompileUnit
        unit1.Namespaces.Add(nmspace)
        'Import the service into the Code-DOM tree. This creates proxy code that uses the service.
        Dim warning As ServiceDescriptionImportWarnings = importer.Import(nmspace, unit1)
        If warning = 0 Then
            'Generate the proxy code
            Dim provider1 As CodeDomProvider = CodeDomProvider.CreateProvider("CSharp")
            'Compile the assembly proxy with the appropriate references
            Dim assemblyReferences As String() = New String(4) {"System.dll", "System.Web.Services.dll", "System.Web.dll", "System.Xml.dll", "System.Data.dll"}
            Dim parms As CompilerParameters = New CompilerParameters(assemblyReferences)
            Dim results As CompilerResults = provider1.CompileAssemblyFromDom(parms, unit1)
            'Check For Errors
            If results.Errors.Count > 0 Then
                For Each oops As CompilerError In results.Errors
                    System.Diagnostics.Debug.WriteLine("========Compiler error============")
                    System.Diagnostics.Debug.WriteLine(oops.ErrorText)
                Next
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error("Compile Error Occured calling webservice", "", "")
            End If
            'Finally, Invoke the web service method
            Dim wsvcClass As Object
            If Double.Parse(version) < 10.3 Then
                wsvcClass = results.CompiledAssembly.CreateInstance("MobilityService")
            Else
                wsvcClass = results.CompiledAssembly.CreateInstance("MobilityBean")
            End If
            Dim mi As MethodInfo = wsvcClass.GetType.GetMethod("executeAPI")
            Dim parametersArray As Object() = New Object() {message}
            'NADER CHECK ERROR HERE!!!
            Dim result As Object = mi.Invoke(wsvcClass, parametersArray)
            Using stringwriter As StringWriter = New StringWriter
                Dim serializer As XmlSerializer = New XmlSerializer(result.GetType)
                serializer.Serialize(stringwriter, result)
                soap = Regex.Replace(Regex.Replace(stringwriter.ToString(), "(\&lt;)", "<"), "(\&gt;)", ">")
            End Using
        Else
            Dim logger As Logger = LogManager.GetCurrentClassLogger()
            logger.Error("Warning Webservice " & warning.ToString(), "", "")
        End If
        Return soap
    End Function
    Public Shared Function getCodeDD(ByVal warehouse As String, ByVal table As String, ByVal listname As String) As DataTable
        Dim sql As String = ""
        If warehouse <> "enterprise" Then
            If LCase(warehouse.Substring(0, 6)) = "infor_" Then warehouse = warehouse.Substring(6, warehouse.Length - 6)
            If LCase(warehouse).Contains("_") Then warehouse = warehouse.Split("_")(1)
            sql += "select CODE,DESCRIPTION from " & warehouse & "." & table
            If table <> "orderstatussetup" Then sql += " where listname='" & listname & "'"
        Else
            Dim dtw As DataTable = getFacilities()
            If table <> "orderstatussetup" Then sql += "select CODE,DESCRIPTION from enterprisse." & table & " where listname ='" & listname & "'"
            For Each row As DataRow In dtw.Rows
                warehouse = row("DB_NAME").ToString()
                If LCase(warehouse.Substring(0, 6)) = "infor_" Then warehouse = warehouse.Substring(6, warehouse.Length - 6)
                If LCase(warehouse).Contains("_") Then warehouse = warehouse.Split("_")(1)
                If sql.Length > 0 Then sql += " union "
                sql += "select CODE,DESCRIPTION from " & warehouse & "." & table
                If table <> "orderstatussetup" Then sql += " where listname='" & listname & "'"
            Next
        End If
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        Return ds.Tables(0)
    End Function
    Public Shared Function getProfiles() As DataTable
        Dim sql As String = "select PROFILENAME from " & IIf(dbtype <> "sql", "System.", "") & "PROFILES"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        Return ds.Tables(0)
    End Function
    Public Shared Function getUsers() As DataTable
        Dim sql As String = "select USERKEY from " & IIf(dbtype <> "sql", "System.", "") & "PORTALUSERS WHERE ACTIVE=1"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        Return ds.Tables(0)
    End Function
    Public Shared Function getCountries() As DataTable
        Dim sql As String = "select DESCRIPTION from enterprise.codelkup where listname = 'ISOCOUNTRY' ORDER BY DESCRIPTION"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        Return ds.Tables(0)
    End Function
    Public Shared Function getISOCountryCode(ByVal CountryName As String) As String
        Dim key As String = ""
        Dim sql As String = "select CODE from enterprise.codelkup where listname = 'ISOCOUNTRY' and Description = '" & CountryName & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then key = ds.Tables(0).Rows(0)!CODE
        Return key
    End Function
    Public Shared Function getISOCountryName(ByVal Code As String) As String
        Dim key As String = ""
        Dim sql As String = "select DESCRIPTION from enterprise.codelkup where listname = 'ISOCOUNTRY' and CODE = '" & Code & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then key = ds.Tables(0).Rows(0)!DESCRIPTION
        Return key
    End Function
    Public Shared Function getFacilitiesPerUser(ByVal userkey As String) As DataTable
        Dim sql As String = "select DB_NAME, DB_ALIAS as DB_LOGID from wmsadmin.pl_db where isActive='1' and db_enterprise='0' and DB_NAME in (select FACILITY from " & IIf(dbtype <> "sql", "System.", "") & "USERCONTROLFACILITY where USERKEY= '" & userkey & "')"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        Return ds.Tables(0)
    End Function
    Public Shared Function getFacilities() As DataTable
        Dim sql As String = "select db_alias from wmsadmin.pl_db where isActive=1 and db_enterprise=0"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        Return ds.Tables(0)
    End Function
    Public Shared Function getFacilityDBName(ByVal Facility As String) As String
        Dim key As String = ""
        Dim sql As String = "select db_name from wmsadmin.pl_db where isActive=1 and db_enterprise=0 and db_alias= '" & Facility & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then key = ds.Tables(0).Rows(0)!db_name
        Return key
    End Function
    Public Shared Function getFacilityDBAlias(ByVal Facility As String) As String
        Dim key As String = ""
        Dim sql As String = "select db_alias from wmsadmin.pl_db where isActive=1 and db_enterprise=0 and db_name= '" & Facility & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then key = ds.Tables(0).Rows(0)!db_alias
        Return key
    End Function
    Public Shared Function getExternKey(ByVal warehouse As String, ByVal keyname As String) As String
        Dim key As String = ""
        If LCase(warehouse.Substring(0, 6)) = "infor_" Then warehouse = warehouse.Substring(6, warehouse.Length - 6)
        If LCase(warehouse).Contains("_") Then warehouse = warehouse.Split("_")(1)
        Dim sql As String = "select "
        If dbtype = "sql" Then
            sql += "(RIGHT('000000000' + CAST((ISNULL(keycount,0))+1 AS varchar(10)) , 10) )"
        Else
            sql += "(SUBSTR(CONCAT('000000000' , CAST((NVL(keycount,0)+1) AS nvarchar2(10))) , -10) )"
        End If
        sql += " As ExternKey from " & warehouse & ".NCOUNTER where KEYNAME='" & keyname & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then key = ds.Tables(0).Rows(0)!ExternKey
        Return key
    End Function
    Public Shared Function SaveXml(ByVal Xml As String, ByVal type As String, ByVal source As String) As String
        Dim tmp As String = ""
        Try
            Dim soapResult As String = sendwebRequest(Xml)

            If String.IsNullOrEmpty(soapResult) Then
                tmp = "Error: Unable to connect to webservice, kindly check the logs"
            Else
                Dim dsresult As DataSet = New DataSet
                Dim doc As XmlDocument = New XmlDocument
                doc.LoadXml(soapResult)
                Dim xmlFile As XmlReader = XmlReader.Create(New StringReader(soapResult), New XmlReaderSettings)
                If LCase(soapResult).Contains("error") Then
                    Dim nodeList As XmlNodeList
                    If soapResult.Contains("ERROR") Then
                        nodeList = doc.GetElementsByTagName("Error")
                    Else
                        nodeList = doc.GetElementsByTagName("string")
                    End If
                    Dim message As String = ""
                    For Each node As XmlNode In nodeList
                        message = node.InnerText
                    Next
                    message = Regex.Replace(message, "&.*?;", "")
                    tmp = "Error: " & message & "<br/>"
                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                    logger.Error(message, "", "")
                Else
                    Dim logmessage As String = logger("ALL", type, source, HttpContext.Current.Session("userkey").ToString)
                    If Not String.IsNullOrEmpty(logmessage) Then
                        tmp = "Logging Error: " & logmessage & "<br/>"
                        Dim logger As Logger = LogManager.GetCurrentClassLogger()
                        logger.Error(logmessage, "", "")
                    End If
                End If
            End If
        Catch ex As Exception
            tmp = "Error: " & ex.Message & vbTab & ex.GetType.ToString & "<br/>"
            Dim logger As Logger = LogManager.GetCurrentClassLogger()
            logger.Error(ex, "", "")
        End Try
        Return tmp
    End Function
    Public Shared Function DeleteXml(ByVal Xml As String, ByVal facility As String, ByVal type As String, ByVal source As String) As String
        Dim tmp As String = ""
        Try
            Dim soapResult As String = sendwebRequest(Xml)
            If String.IsNullOrEmpty(soapResult) Then
                tmp = "Error: Unable to connect to webservice, kindly check the logs"
            Else
                Dim dsresult As DataSet = New DataSet
                Dim doc As XmlDocument = New XmlDocument
                doc.LoadXml(soapResult)
                Dim xmlFile As XmlReader = XmlReader.Create(New StringReader(soapResult), New XmlReaderSettings)
                If LCase(soapResult).Contains("error") Then
                    Dim nodeList As XmlNodeList
                    If soapResult.Contains("ERROR") Then
                        nodeList = doc.GetElementsByTagName("Error")
                    Else
                        nodeList = doc.GetElementsByTagName("string")
                    End If
                    Dim message As String = ""
                    For Each node As XmlNode In nodeList
                        message = node.InnerText
                    Next
                    message = Regex.Replace(message, "&.*?;", "")
                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                    logger.Error(message, "", "")
                    If message.Contains("01:00096:") Then
                        message = "Error: Unable to delete item, it is already used in WMS"
                    End If
                    If Not tmp.Contains(message) Then
                        tmp = "Error: " & message & "<br/>"
                    End If
                Else
                    Dim logmessage As String = logger(facility, type, source, HttpContext.Current.Session("userkey").ToString)
                    If Not String.IsNullOrEmpty(logmessage) Then
                        tmp = "Logging Error: " & logmessage & "<br/>"
                        Dim logger As Logger = LogManager.GetCurrentClassLogger()
                        logger.Error(logmessage, "", "")
                    End If
                End If
            End If
        Catch ex As Exception
            tmp = "Error: " & ex.Message & vbTab & ex.GetType.ToString & "<br/>"
            Dim logger As Logger = LogManager.GetCurrentClassLogger()
            logger.Error(ex, "", "")
        End Try
        Return tmp
    End Function
    Public Shared Function ViewXml(ByVal Xml As String) As DataTable
        Dim table As New DataTable
        table.Columns.Add("doc", GetType(XmlDocument))
        table.Columns.Add("error", GetType(String))
        Dim tmp As String = ""
        Dim doc As XmlDocument = New XmlDocument
        Try
            Dim soapResult As String = sendwebRequest(Xml)
            If String.IsNullOrEmpty(soapResult) Then
                tmp = "Error: Unable to connect to webservice, kindly check the logs"
            Else
                Dim dsresult As DataSet = New DataSet
                doc.LoadXml(soapResult)
                Dim xmlFile As XmlReader = XmlReader.Create(New StringReader(soapResult), New XmlReaderSettings)
                If LCase(soapResult).Contains("error") Then
                    Dim nodeList As XmlNodeList
                    If soapResult.Contains("ERROR") Then
                        nodeList = doc.GetElementsByTagName("Error")
                    Else
                        nodeList = doc.GetElementsByTagName("string")
                    End If
                    Dim message As String = ""
                    For Each node As XmlNode In nodeList
                        message = node.InnerText
                    Next
                    message = Regex.Replace(message, "&.*?;", "")
                    tmp = "Error: " & message & "<br/>"
                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                    logger.Error(message, "", "")
                End If
            End If
        Catch ex As Exception
            tmp = "Error: " & ex.Message & vbTab & ex.GetType.ToString & "<br/>"
            Dim logger As Logger = LogManager.GetCurrentClassLogger()
            logger.Error(ex, "", "")
        End Try
        table.Rows.Add(doc, tmp)
        Return table
    End Function
    Public Shared Function ActionXml(ByVal Xml As String) As String
        Dim tmp As String = ""
        Try
            Dim soapResult As String = sendwebRequest(Xml)
            If String.IsNullOrEmpty(soapResult) Then
                tmp = "Error: Unable to connect to webservice, kindly check the logs"
            Else
                Dim dsresult As DataSet = New DataSet
                Dim doc As XmlDocument = New XmlDocument
                doc.LoadXml(soapResult)
                Dim xmlFile As XmlReader = XmlReader.Create(New StringReader(soapResult), New XmlReaderSettings)
                If LCase(soapResult).Contains("error") Then
                    Dim nodeList As XmlNodeList
                    If soapResult.Contains("ERROR") Then
                        nodeList = doc.GetElementsByTagName("Error")
                    Else
                        nodeList = doc.GetElementsByTagName("string")
                    End If
                    Dim message As String = ""
                    For Each node As XmlNode In nodeList
                        message = node.InnerText
                    Next
                    message = Regex.Replace(message, "&.*?;", "")
                    message = Regex.Replace(message, "\n", "\\n")
                    If Not tmp.Contains(message) Then
                        tmp = "Error: " & message & "<br/>"
                    End If
                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                    logger.Error(message, "", "")
                End If
            End If
        Catch ex As Exception
            tmp = "Error: " & ex.Message & vbTab & ex.GetType.ToString & "<br/>"
            Dim logger As Logger = LogManager.GetCurrentClassLogger()
            logger.Error(ex, "", "")
        End Try
        Return tmp
    End Function
    Public Shared Function ConvertLottableToDate(ByVal lottable As String, ByVal TimeZone As Integer) As String
        Dim lots As String = ""
        Dim enteredDate As DateTime, dateTime As DateTime
        If DateTime.TryParseExact(lottable, "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, dateTime) Then
            enteredDate = DateTime.ParseExact(lottable, "dd/MM/yyyy hh:mm:ss", Nothing)
            Dim timezonedate As DateTime = enteredDate.AddHours(TimeZone)
            lots += timezonedate.ToShortDateString()
        ElseIf DateTime.TryParseExact(lottable, "yyyy-MM-dd hh:mm:ss.s", CultureInfo.InvariantCulture, DateTimeStyles.None, dateTime) Then
            enteredDate = DateTime.ParseExact(lottable, "yyyy-MM-dd hh:mm:ss.s", Nothing)
            Dim timezonedate As DateTime = enteredDate.AddHours(TimeZone)
            lots += timezonedate.ToShortDateString()
        ElseIf DateTime.TryParseExact(lottable, "MM/dd/yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, dateTime) Then
            enteredDate = DateTime.ParseExact(lottable, "MM/dd/yyyy hh:mm:ss", Nothing)
            Dim timezonedate As DateTime = enteredDate.AddHours(TimeZone)
            lots += timezonedate.ToShortDateString()
        ElseIf DateTime.TryParseExact(lottable, "d/M/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, dateTime) Then
            enteredDate = DateTime.ParseExact(lottable, "d/M/yyyy hh:mm:ss tt", Nothing)
            Dim timezonedate As DateTime = enteredDate.AddHours(TimeZone)
            lots += timezonedate.ToShortDateString()
        Else
            lots = lottable
        End If
        Return lots
    End Function
    Public Shared Function incremenetKey(ByVal warehouse As String, ByVal keyname As String) As String
        If LCase(warehouse.Substring(0, 6)) = "infor_" Then warehouse = warehouse.Substring(6, warehouse.Length - 6)
        If LCase(warehouse).Contains("_") Then warehouse = warehouse.Split("_")(1)
        Dim updt As String = "UPDATE " & warehouse & ".NCOUNTER set keycount=keycount+1  where KEYNAME='" & keyname & "' "
        Return (New SQLExec).Execute(updt)
    End Function
    Public Shared Function getStatusPo(ByVal code As Integer) As String
        If code = 0 Then
            Return "New"
        ElseIf code = 2 Then
            Return "In Transit"
        ElseIf code = 4 Then
            Return "Scheduled"
        ElseIf code = 5 Then
            Return "In Receiving"
        ElseIf code = 9 Then
            Return "Received"
        ElseIf code = 11 Then
            Return "Closed"
        ElseIf code = 15 Then
            Return "Verified Closed"
        ElseIf code = 20 Then
            Return "Cancelled"
        Else
            Return "Undefinied"
        End If
    End Function
    Public Shared Function getTypePo(ByVal code As Integer) As String
        If code = 0 Then
            Return "Standard"
        ElseIf code = 1 Then
            Return "Flow Thru"
        Else
            Return "Undefinied"
        End If
    End Function
    Public Shared Function getUomMeasure(ByVal warehouse As String, ByVal pack As String, ByVal uom As String) As Double
        Dim UomMeasure As Double = 1
        If LCase(warehouse.Substring(0, 6)) = "infor_" Then warehouse = warehouse.Substring(6, warehouse.Length - 6)
        If LCase(warehouse).Contains("_") Then warehouse = warehouse.Split("_")(1)
        Dim sql As String = "SELECT CASE  WHEN PACKUOM1 =  '" & uom & "' THEN CASECNT  WHEN PACKUOM2 =  '" & uom & "' THEN INNERPACK  WHEN PACKUOM3 =  '" & uom & "' THEN QTY	WHEN PACKUOM4 =  '" & uom & "' THEN PALLET  WHEN PACKUOM5 =  '" & uom & "' THEN CUBE WHEN packuom6 =  '" & uom & "' THEN GROSSWGT WHEN PACKUOM7 =  '" & uom & "' THEN NETWGT 	WHEN PACKUOM8 =  '" & uom & "' THEN OTHERUNIT1   WHEN PACKUOM9 =  '" & uom & "' THEN OTHERUNIT2  ELSE 1 END as UomMeasure from " & warehouse & ".PACK WHERE PACKKEY= '" & pack & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then UomMeasure = Double.Parse(ds.Tables(0).Rows(0)!UomMeasure.ToString)
        Return UomMeasure
    End Function
    Public Shared Function getUOMPerPack(ByVal warehouse As String, ByVal packkey As String) As String
        Dim UOM As String = "", UOMFinal As String = ""
        Dim sql As String = ""
        If warehouse <> "enterprise" Then
            If LCase(warehouse.Substring(0, 6)) = "infor_" Then warehouse = warehouse.Substring(6, warehouse.Length - 6)
            If LCase(warehouse).Contains("_") Then warehouse = warehouse.Split("_")(1)
        End If

        If dbtype = "sql" Then
            sql += "select PackUOM1+','+PackUOM2+','+PackUOM3+','+PackUOM4+','+Packuom5+','+ Packuom6+','+Packuom7+','+Packuom8+','+Packuom9 as UOM from " & warehouse & ".pack "
        Else
            sql += "select concat(PackUOM1,',',PackUOM2,',',PackUOM3,',',PackUOM4,',',Packuom5,',', Packuom6,',',Packuom7,',',Packuom8,',',Packuom9) as UOM from " + warehouse + ".pack "
        End If
        sql += " where packkey ='" & packkey & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then
            UOM = ds.Tables(0).Rows(0)!UOM.ToString
            Dim token As String() = UOM.Split(",")
            For Each s As String In token
                If Not String.IsNullOrEmpty(Trim(s)) Then UOMFinal += s & ","
            Next
            Return UOMFinal.Remove(UOMFinal.Length - 1)
        End If
        Return String.Empty
    End Function
    Public Shared Function getwmsOkeyFromWMS(ByVal warehouse As String, ByVal externokey As String) As String
        Dim key As String = ""
        If LCase(warehouse.Substring(0, 6)) = "infor_" Then warehouse = warehouse.Substring(6, warehouse.Length - 6)
        If LCase(warehouse).Contains("_") Then warehouse = warehouse.Split("_")(1)
        Dim sql As String = "select orderkey from " & warehouse & ".orders where ExternOrderKey='" & externokey & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then key = ds.Tables(0).Rows(0)!orderkey
        Return key
    End Function
    Public Shared Function CheckOrderState(ByVal warehouse As String, ByVal externokey As String) As Boolean
        If LCase(warehouse.Substring(0, 6)) = "infor_" Then warehouse = warehouse.Substring(6, warehouse.Length - 6)
        If LCase(warehouse).Contains("_") Then warehouse = warehouse.Split("_")(1)
        Dim sql As String = "select status from " & warehouse & ".orders where ExternOrderKey='" & externokey & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then
            With ds.Tables(0).Rows(0)
                Return Val(!Status) = 0 Or Val(!Status) = 2 Or Val(!Status) = 4
            End With
        End If
        Return False
    End Function
    Public Shared Function checkNewLines(ByVal ordermanagkey As String, ByVal warehouse As String) As Integer
        Dim counter As Integer = 0
        Dim sql As String = "select count(1) As Counter from " & IIf(dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAGDETAIL where ORDERMANAGKEY='" & ordermanagkey & "' and WHSEID = '" & warehouse & "' and Status = '02'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then counter = Val(ds.Tables(0).Rows(0)!counter)
        Return counter
    End Function
    Public Shared Function getExternline(ByVal warehouse As String, ByVal externokey As String, ByVal table As String, ByVal field As String)
        Dim LineNo As String = "0"
        Dim sql As String = ""
        If warehouse <> "dbo" And warehouse <> "SYSTEM" Then
            If LCase(warehouse.Substring(0, 6)) = "infor_" Then
                warehouse = warehouse.Substring(6, (warehouse.Length - 6))
            End If
            If LCase(warehouse.Contains("_")) Then warehouse = warehouse.Split("_")(1)
        End If
        If CommonMethods.dbtype = "sql" Then
            sql = "select (RIGHT('00000' + CAST(ISNULL(max(cast(EXTERNLINENO as int))+1,1) AS varchar(5)) , 5)) as LineNum from " & warehouse & "." & table & "  where " & field & "='" & externokey & "' and EXTERNLINENO not like '%[^0-9]%'"
        Else
            sql = "select (SUBSTR(CONCAT('00000' , CAST(NVL(max(cast(EXTERNLINENO as integer))+1,1) AS nvarchar2(5))) , -5)) as LineNum from " & warehouse & "." & table & "  where " & field & "='" & externokey & "' and EXTERNLINENO not like '%[^0-9]%'"
        End If
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then LineNo = ds.Tables(0).Rows(0)!LineNum
        Return LineNo
    End Function
    Public Shared Function checkPOLineExist(ByVal Facility As String, ByVal pokey As String, ByVal externlineno As String) As Integer
        Dim counter As Integer = 0
        If LCase(Facility.Substring(0, 6)) = "infor_" Then Facility = Facility.Substring(6, Facility.Length - 6)
        If LCase(Facility).Contains("_") Then Facility = Facility.Split("_")(1)
        Dim sql As String = "select count(1) as Counter from " & Facility & ".podetail where POKey='" & pokey & "' and ExternLineNo= '" & externlineno & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then counter = Val(ds.Tables(0).Rows(0)!Counter)
        Return counter
    End Function
    Public Shared Function checkReceiptLineExist(ByVal Facility As String, ByVal receiptkey As String, ByVal externlineno As String) As Integer
        Dim counter As Integer = 0
        If LCase(Facility.Substring(0, 6)) = "infor_" Then Facility = Facility.Substring(6, Facility.Length - 6)
        If LCase(Facility).Contains("_") Then Facility = Facility.Split("_")(1)
        Dim sql As String = "select count(1) as Counter from " & Facility & ".RECEIPTDETAIL where ReceiptKey='" & receiptkey & "' and ExternLineNo= '" & externlineno & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then counter = Val(ds.Tables(0).Rows(0)!Counter)
        Return counter
    End Function
    Public Shared Function checkSOLineExist(ByVal Facility As String, ByVal orderkey As String, ByVal externlineno As String) As Integer
        Dim counter As Integer = 0
        If LCase(Facility.Substring(0, 6)) = "infor_" Then Facility = Facility.Substring(6, Facility.Length - 6)
        If LCase(Facility).Contains("_") Then Facility = Facility.Split("_")(1)
        Dim sql As String = "select count(1) as Counter from " & Facility & ".orderdetail where OrderKey='" & orderkey & "' and ExternLineNo= '" & externlineno & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then counter = Val(ds.Tables(0).Rows(0)!Counter)
        Return counter
    End Function
    Public Shared Function checkOrderManagementLineExist(ByVal orderkey As String, ByVal externokey As String, ByVal owner As String, ByVal warehouse As String, ByVal externlineno As String) As Integer
        Dim counter As Integer = 0
        Dim sql As String = "select count(1) as Counter from " & IIf(dbtype <> "sql", "SYSTEM.", "") & ".ORDERMANAGDETAIL "
        sql += " where OrderManagKey ='" & orderkey & "' and ExternOrderKey= '" & externokey & "'"
        sql += " and StorerKey ='" & owner & "' and WHSEID= '" & warehouse & "' and ExternLineNo='" & externlineno & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then counter = Val(ds.Tables(0).Rows(0)!Counter)
        Return counter
    End Function
    Public Shared Function GetMyID(ByVal SearchTable As String, ByVal StrID As String) As Integer
        Dim MyID As Integer = 0
        Dim AndFilter As String = ""
        Dim sql As String = ""
        Dim tb As SQLExec = New SQLExec
        Dim ds As DataSet = Nothing
        Select Case SearchTable
            Case "PORTALUSERS", "SKUCATALOGUE"
                MyID = StrID.Split("=")(1)
            Case "USERCONTROL"
                sql += "Select " & IIf(dbtype = "sql", "ID", "SerialKey") & " From " & IIf(dbtype <> "sql", "SYSTEM.", "") & SearchTable & " where UserKey = '" & StrID.Split("=")(1) & "'"
            Case "enterprise.storer2", "enterprise.storer12"
                sql += "Select SerialKey from enterprise.storer where StorerKey = '" & StrID.Split("=")(1) & "' and Type = '" & IIf(SearchTable = "enterprise.storer2", "2", "12") & "'"
            Case "enterprise.sku"
                sql += "Select SerialKey from " & SearchTable & " where StorerKey = '" & StrID.Split("=")(1).Split("&")(0) & "' and Sku = '" & StrID.Split("=")(2) & "'"
            Case "Warehouse_PO", "Warehouse_ASN", "Warehouse_SO", "Warehouse_OrderManagement"
                Dim owners As String() = getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
                Dim suppliers As String() = getSupplierPerUser(HttpContext.Current.Session("userkey").ToString)
                Dim consignees As String() = getConsigneePerUser(HttpContext.Current.Session("userkey").ToString)
                If owners IsNot Nothing And suppliers IsNot Nothing And consignees IsNot Nothing Then
                    Dim ownersstr As String = String.Join("','", owners)
                    ownersstr = "'" & ownersstr & "'"
                    If Not UCase(ownersstr).Contains("'ALL'") Then AndFilter += " and STORERKEY IN (" & ownersstr & ")"

                    If SearchTable = "Warehouse_PO" Then
                        Dim suppliersstr As String = String.Join("','", suppliers)
                        suppliersstr = "'" & suppliersstr & "'"
                        If Not UCase(suppliersstr).Contains("'ALL'") Then AndFilter += " and SellerName IN (" & suppliersstr & ")"
                    End If

                    If SearchTable = "Warehouse_SO" Or SearchTable = "Warehouse_OrderManagement" Then
                        Dim consigneesstr As String = String.Join("','", consignees)
                        consigneesstr = "'" & consigneesstr & "'"
                        If Not UCase(consigneesstr).Contains("'ALL'") Then AndFilter += " and ConsigneeKey IN (" & consigneesstr & ")"
                    End If

                    Dim warehouse As String = StrID.Split("=")(1).Split("&")(0)
                    Dim warehouselevel As String = warehouse
                    If SearchTable <> "Warehouse_OrderManagement" Then
                        If LCase(warehouse.Substring(0, 6)) = "infor_" Then
                            warehouselevel = warehouse.Substring(6, warehouse.Length - 6)
                        End If
                        warehouselevel = warehouselevel.Split("_")(1)
                    End If

                    sql += "Select SerialKey from " & warehouselevel
                    If SearchTable = "Warehouse_PO" Then
                        sql += ".PO where POKey = "
                    ElseIf SearchTable = "Warehouse_ASN" Then
                        sql += ".Receipt where ReceiptKey = "
                    ElseIf SearchTable = "Warehouse_SO" Then
                        sql += ".Orders where OrderKey = "
                    ElseIf SearchTable = "Warehouse_OrderManagement" Then
                        sql = "Select SerialKey from " & IIf(dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAG where OrderManagKey = "
                    End If
                    sql += "'" & StrID.Split("=")(IIf(SearchTable = "Warehouse_OrderManagement", 1, 2)) & "'" & AndFilter
                End If
        End Select

        If sql <> "" Then
            ds = tb.Cursor(sql)
            If ds.Tables(0).Rows.Count > 0 Then MyID = ds.Tables(0).Rows(0)(0)
        End If

        Return Val(MyID)
    End Function
    Public Shared Function GetUserMenuOpen() As Integer
        Dim MenuOpen As Integer = 1
        Dim sql As String = "Select MenuOpen from " & IIf(dbtype <> "sql", "SYSTEM.", "") & "PORTALUSERS Where UserKey = '" & HttpContext.Current.Session("userkey") & "'"
        Dim ds As DataSet = (New SQLExec).Cursor(sql)
        If ds.Tables(0).Rows.Count > 0 Then MenuOpen = Val(ds.Tables(0).Rows(0)!MenuOpen)
        Return MenuOpen
    End Function
End Class
