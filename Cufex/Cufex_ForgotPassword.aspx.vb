
Imports System.Data.SqlClient
Imports NLog
Imports System.Net.Mail

Partial Public Class Cufex_ForgotPassword
    Inherits MultiLingualPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.section = Cufex_Site.SectionName.ForgetPassword
            txtUserName.Focus()
            Session("BUserCode") = 0
            myMasterPage.BUserCode = 0
            Session("BUserID") = ""
            Session("BUserGroup") = 0
            myMasterPage.BUserGroup = 0
            Session("BFullName") = ""
            Session("BCanClone") = 0
            Session("BUserGroupName") = ""
            If Session("BLoggedout") = "True" Then
                Session("BLoggedout") = ""
                'LoggedOut.Visible = True
            End If


            If Not Request.Cookies("Cufex_Username") Is Nothing Then
                If Not Request.Cookies("Cufex_Username").Value = "" Then
                    txtUserName.Text = Request.Cookies("Cufex_Username").Value
                    txtEmail.Focus()
                End If
            Else
                txtUserName.Focus()
            End If

            Session("userkey") = Nothing
            Session("dtTest") = Nothing
        End If
    End Sub
    Public Sub Submit()
        Dim exist As String = ""
        CVError1.IsValid = True
        CVError1.ErrorMessage = ""
        Dim email As String = ""
        Dim userID As String = ""
        Dim dt As DataTable = New DataTable
        Dim username As String = LCase(txtUserName.Text)

        If CommonMethods.dbtype = "sql" Then
            Try
                Using connection As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                    connection.Open()
                    Dim query As SqlCommand = New SqlCommand("select ID, ACTIVE, EMAIL from dbo.PORTALUSERS where USERKEY=@user", connection)
                    query.Parameters.AddWithValue("@user", username)
                    Using people As SqlDataAdapter = New SqlDataAdapter
                        people.SelectCommand = query
                        people.Fill(dt)
                        For Each row As DataRow In dt.Rows
                            exist = row("ACTIVE").ToString
                            email = row("EMAIL").ToString
                            userID = row("ID").ToString
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
        If exist = "" And Not CVError1.IsValid Then
            CVError1.IsValid = False
            CVError1.ErrorMessage = "Error: Username/Email are not valid"
        ElseIf LCase(email) <> LCase(txtEmail.Text) Then
            CVError1.IsValid = False
            CVError1.ErrorMessage = "Error: Invalid email"
        ElseIf exist = "0" Then
            CVError1.IsValid = False
            CVError1.ErrorMessage = "Error: Username is inactive"
        Else
            Dim code As String = CreateResetCode(Integer.Parse(userID))
            Dim msg As String = Resources.Resources.Email_PasswordCode
            CommonMethods.SendEmail(New MailAddress(email, username), "Reset Password Code", String.Format(msg, username, code))
            Session("userID") = userID
            Session("userkey") = username
            Session("email") = email
            Response.Redirect(Page.GetRouteUrl("SNSsoftware-Cufex-Reset_Password", Nothing))
        End If
    End Sub
    Private Function GenerateText(ByVal length As Integer, ByVal chars As String) As String
        Dim random = New Random()
        Return New String(Enumerable.Repeat(chars, length).Select(Function(s) s(random.Next(s.Length))).ToArray())
    End Function
    Public Function CreateResetCode(ByVal userID As Integer) As String
        Dim key As String = ""
        key = GenerateText(7, "abcdefghijklmnopqrstuvwxyz1234567890")
        Dim code = New ResetCode With {
            .code = key,
            .IssueDate = DateTime.UtcNow,
            .EndDate = DateTime.UtcNow.AddMinutes(15),
            .userID = userID
        }
        insertcode(code)
        Return code.code
    End Function
    Private Sub insertcode(ByVal resetCode As ResetCode)
        Dim Command As String = ""
        If CommonMethods.dbtype = "sql" Then
            Try
                Command = "'" & resetCode.code & "','" + resetCode.IssueDate.ToString & "','" + resetCode.EndDate.ToString & "','" + resetCode.userID.ToString & "'"
                Dim insert As String = "set dateformat dmy insert into  dbo.ResetCode (Code, IssueDate, EndDate,UserId) values( " & Command & ");"
                Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                conn.Open()
                Dim cmd As SqlCommand = New SqlCommand(insert, conn)
                cmd.ExecuteNonQuery()
                conn.Close()
            Catch e1 As Exception
                CVError1.IsValid = False
                CVError1.ErrorMessage += "Error: " & e1.Message & vbTab + e1.GetType.ToString & "<br/>"
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(e1, "", "")
            End Try
        End If
    End Sub

    Private Sub BtnSubmit_Click(sender As Object, e As EventArgs) Handles BtnSubmit.Click
        Submit()
    End Sub
End Class

Friend Class ResetCode
    Public Property code As String
    Public Property IssueDate As Date
    Public Property EndDate As Date
    Public Property userID As Integer
End Class
