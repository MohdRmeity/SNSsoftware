
Imports System.Data.SqlClient
Imports Oracle.ManagedDataAccess.Client
Imports NLog
Partial Public Class Cufex
    Inherits MultiLingualPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.section = Cufex_Site.SectionName.LogIn

            txtUserName.Focus()
            Session("BUserCode") = 0
            myMasterPage.BUserCode = 0
            Session("BUserID") = ""
            'Session("BUserGroup") = 0
            'myMasterPage.BUserGroup = 0
            Session("BFullName") = ""
            'Session("BCanClone") = 0
            'Session("BUserGroupName") = ""
            If Session("BLoggedout") = "True" Then
                Session("BLoggedout") = ""
                'LoggedOut.Visible = True
            End If


            If Not Request.Cookies("Cufex_Username") Is Nothing Then
                If Not Request.Cookies("Cufex_Username").Value = "" Then
                    txtUserName.Text = Request.Cookies("Cufex_Username").Value
                    txtPassword.Focus()
                End If
            Else
                txtUserName.Focus()
            End If

            If Not HttpContext.Current.Session Is Nothing Then
                If Not Val(Session("LogMeOut")) = 1 Then
                    Session("LogMeOut") = 0
                    If Not Request.Cookies("Cufex_Password") Is Nothing Then
                        If Not Request.Cookies("Cufex_Password").Value = "" Then
                            txtPassword.Text = Request.Cookies("Cufex_Password").Value
                            'Submit()
                        End If
                    Else
                        txtPassword.Focus()
                    End If
                End If

            End If

            Session("userkey") = Nothing
        End If

        'Session.Clear()
    End Sub
    Public Sub Submit()
        Dim exist As String = ""
        CVError1.IsValid = True
        CVError1.ErrorMessage = ""
        Dim hashed As String = ""
        Dim key As String = ""
        Dim userID As Integer = 0
        Dim dt As DataTable = New DataTable
        Dim refreshTime As String = ""

        If CommonMethods.dbtype = "sql" Then
            Try
                Using connection As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                    connection.Open()
                    Dim query As SqlCommand = New SqlCommand("select ID, ACTIVE, PASSWORD, HASHKEY, DASHBOARDREFRESHTIME  from dbo.PORTALUSERS where USERKEY=@user", connection)
                    query.Parameters.AddWithValue("@user", LCase(txtUserName.Text))
                    Using people As SqlDataAdapter = New SqlDataAdapter
                        people.SelectCommand = query
                        people.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            exist = row("ACTIVE").ToString
                            hashed = row("PASSWORD").ToString
                            key = row("HASHKEY").ToString
                            userID = Val(row("ID"))
                            refreshTime = row("DASHBOARDREFRESHTIME").ToString
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
                CVError1.IsValid = False
                CVError1.ErrorMessage = "Error: Database server is unreachable"
            End Try
        Else
            Try
                Using connection As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                    connection.Open()
                    Dim query As OracleCommand = New OracleCommand("select ID, ACTIVE, PASSWORD, HASHKEY  from SYSTEM.PORTALUSERS where USERKEY = :Userk", connection)
                    query.Parameters.Add(New OracleParameter("Userk", LCase(txtUserName.Text)))
                    Using orderdata As OracleDataAdapter = New OracleDataAdapter
                        orderdata.SelectCommand = query
                        orderdata.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            exist = row("ACTIVE").ToString
                            hashed = row("PASSWORD").ToString
                            key = row("HASHKEY").ToString
                            userID = Val(row("ID").ToString)
                            refreshTime = row("DASHBOARDREFRESHTIME").ToString
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
                CVError1.IsValid = False
                CVError1.ErrorMessage = "Error: Database server is unreachable"
            End Try
        End If
        If exist = "" And CVError1.IsValid Then
            CVError1.IsValid = False
            CVError1.ErrorMessage = "Error: Username/Password are not valid"
        ElseIf exist = "0" Then
            CVError1.IsValid = False
            CVError1.ErrorMessage = "Error: Username is inactive"
        ElseIf Not CommonMethods.AreEqual(txtPassword.Text, hashed, key) Then
            CVError1.IsValid = False
            CVError1.ErrorMessage = "Error: Password is not valid"
            CommonMethods.incremenetLogins(LCase(txtUserName.Text))
        ElseIf CVError1.IsValid Then
            Dim locked As Boolean = CommonMethods.getLockedOut(LCase(txtUserName.Text))
            If locked Then
                CVError1.IsValid = False
                CVError1.ErrorMessage = "Error: Your user has been locked for multiple login failed attempts, kindly wait for 5 minutes."
            Else
                Dim updatepass As Boolean = False
                CommonMethods.resettrials(LCase(txtUserName.Text))
                updatepass = ValidateDate()
                If updatepass And LCase(txtUserName.Text) <> "admin" Then
                    LoginButton.Visible = False
                    CVError1.IsValid = False
                    CVError1.ErrorMessage = "Your Password has expired, kindly update your password"
                    matching.Visible = True
                    UpdatePasswordButton.Visible = True
                    txtUserName.Enabled = False
                    errorS.Visible = True
                    CVError2.IsValid = False
                Else
                    Dim sec As Security = New Security()
                    sec.SetLoginSession(userID)

                    Dim Cufex_Cookie As New HttpCookie("Cufex_Username")
                    Cufex_Cookie.Value = LCase(txtUserName.Text)
                    Cufex_Cookie.Expires = GetLebanonTime().AddDays(365)
                    HttpContext.Current.Response.Cookies.Add(Cufex_Cookie)

                    Dim Cufex_CookieP As New HttpCookie("Cufex_Password")
                    Cufex_CookieP.Value = txtPassword.Text
                    Cufex_CookieP.Expires = GetLebanonTime().AddDays(365)
                    HttpContext.Current.Response.Cookies.Add(Cufex_CookieP)

                    If Request("InWindow") = "yes" Then
                        ScriptManager.RegisterStartupScript(Page, GetType(Page), "alertscript", "Close();", True)
                    Else
                        Session("userkey") = LCase(txtUserName.Text)
                        Session("refershTime") = refreshTime

                        If Not Session("Cufex_AfterLoginURL") Is Nothing Then
                            If Session("Cufex_AfterLoginURL") <> "" Then
                                Dim Cufex_AfterLoginURL As String = Session("Cufex_AfterLoginURL")
                                Session("Cufex_AfterLoginURL") = ""
                                Response.Redirect(Cufex_AfterLoginURL)
                            End If
                        End If
                        Response.Redirect(Page.GetRouteUrl("SNSsoftware-Cufex-Home", Nothing))
                    End If
                End If
            End If
        End If
    End Sub
    Public Sub UpdatePassword()
        CVError1.IsValid = True
        CVError1.ErrorMessage = ""
        If txtPassword.Text <> txtConfirmPassword.Text Then
            errorU.Visible = True
            CVError3.IsValid = False
            CVError3.ErrorMessage = "Error: Password and confirm password do not match!"
        Else
            Dim originalpass As String = CommonMethods.getPassword(LCase(txtUserName.Text))
            Dim keypass As String = CommonMethods.getpasskey(LCase(txtUserName.Text))
            Dim checkmatch As Boolean = CommonMethods.AreEqual(txtConfirmPassword.Text, originalpass, keypass)
            If checkmatch Then
                errorU.Visible = True
                CVError3.IsValid = False
                CVError3.ErrorMessage = "Error: New Password cannot match the old password!"
            Else
                Dim chekcomplex As Boolean = CommonMethods.checkPassComplexity(txtConfirmPassword.Text)
                If Not chekcomplex Then
                    errorU.Visible = True
                    CVError3.IsValid = False
                    If txtConfirmPassword.Text.Length < 10 Then
                        CVError3.ErrorMessage = "Error: Password must have at least 10 characters"
                    Else
                        CVError3.ErrorMessage = "Error: Password must have one upper case letter, one lower case letter and one base 10 digits (0 to 9)"
                    End If
                Else
                    Dim dt As DataTable = New DataTable
                    Dim key As String = CommonMethods.CreateSalt(txtConfirmPassword.Text.Length)
                    If CommonMethods.dbtype = "sql" Then
                        Dim update As String = "update dbo.PORTALUSERS set EDITWHO= @edit, EDITDATE= @editdate , PASSWORDDATE=@passdate ,PASSWORD= @passw, HASHKEY= @keyh  where  USERKEY= @ukey ;"
                        Try
                            Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                            conn.Open()
                            Dim cmd As SqlCommand = New SqlCommand(update, conn)
                            cmd.Parameters.AddWithValue("@edit", LCase(txtUserName.Text))
                            cmd.Parameters.AddWithValue("@editdate", Now)
                            cmd.Parameters.AddWithValue("@passdate", Now)
                            cmd.Parameters.AddWithValue("@passw", CommonMethods.GenerateHash(txtConfirmPassword.Text, key))
                            cmd.Parameters.AddWithValue("@keyh", key)
                            cmd.Parameters.AddWithValue("@ukey", LCase(txtUserName.Text))
                            cmd.ExecuteNonQuery()
                            conn.Close()
                            Session("userkey") = LCase(txtUserName.Text)
                            'Response.Redirect("~/index.aspx");
                        Catch e1 As Exception
                            errorU.Visible = True
                            CVError3.IsValid = False
                            CVError3.ErrorMessage += "Error: " & e1.Message & vbTab + e1.GetType.ToString & "<br/>"
                            Dim logger As Logger = LogManager.GetCurrentClassLogger()
                            logger.Error(e1, "", "")
                        End Try
                    Else
                        Dim update As String = "update SYSTEM.PORTALUSERS set EDITWHO = :Userk, EDITDATE=SYSDATE,PASSWORDDATE=SYSDATE,PASSWORD= :Passw, HASHKEY= :keyh  where  USERKEY= :Ukey"
                        Try
                            Dim conn As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                            conn.Open()
                            Dim cmd As OracleCommand = New OracleCommand(update, conn)
                            cmd.Parameters.Add(New OracleParameter("Userk", LCase(txtUserName.Text)))
                            cmd.Parameters.Add(New OracleParameter("Passw", CommonMethods.GenerateHash(txtConfirmPassword.Text, key)))
                            cmd.Parameters.Add(New OracleParameter("keyh", key))
                            cmd.Parameters.Add(New OracleParameter("Ukey", LCase(txtUserName.Text)))
                            cmd.ExecuteNonQuery()
                            conn.Close()
                            Session("userkey") = LCase(txtUserName.Text)
                            'Response.Redirect("~/index.aspx");
                        Catch e1 As Exception
                            errorU.Visible = True
                            CVError3.IsValid = False
                            CVError3.ErrorMessage += "Error: " & e1.Message & vbTab + e1.GetType.ToString & "<br/>"
                            Dim logger As Logger = LogManager.GetCurrentClassLogger()
                            logger.Error(e1, "", "")
                        End Try
                    End If
                End If
            End If
        End If
    End Sub
    Private Function ValidateDate() As Boolean
        Dim days As Integer = 0
        Dim dt As DataTable = New DataTable

        If CommonMethods.dbtype = "sql" Then
            Try
                Using connection As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                    connection.Open()
                    Dim query As SqlCommand = New SqlCommand("select DATEDIFF(day, passworddate, getdate())   from dbo.PORTALUSERS where USERKEY= @user ", connection)
                    query.Parameters.AddWithValue("@user", LCase(txtUserName.Text))
                    Using people As SqlDataAdapter = New SqlDataAdapter
                        people.SelectCommand = query
                        people.Fill(dt)
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
                CVError1.IsValid = False
                CVError1.ErrorMessage = "Error: Database server is unreachable"
            End Try
        Else
            Try
                Using connection As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                    connection.Open()
                    Dim query As OracleCommand = New OracleCommand("select trunc(sysdate) - TRUNC(passworddate) DAYS   from SYSTEM.PORTALUSERS where USERKEY= :Userk ", connection)
                    query.Parameters.Add(New OracleParameter("Userk", LCase(txtUserName.Text)))
                    Using orderdata As OracleDataAdapter = New OracleDataAdapter
                        orderdata.SelectCommand = query
                        orderdata.Fill(dt)
                    End Using
                    connection.Close()
                End Using
            Catch exp As Exception
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(exp, "", "")
                CVError1.IsValid = False
                CVError1.ErrorMessage = "Error: Database server is unreachable"
            End Try
        End If

        For Each row As DataRow In dt.Rows
            days = Integer.Parse(row(0).ToString)
        Next

        Return days > 90
    End Function
    Private Sub BtnLogin_Click(sender As Object, e As EventArgs) Handles BtnLogin.Click
        Submit()
    End Sub
    Private Sub BtnUpdatePassword_Click(sender As Object, e As EventArgs) Handles BtnUpdatePassword.Click
        UpdatePassword()
    End Sub
End Class