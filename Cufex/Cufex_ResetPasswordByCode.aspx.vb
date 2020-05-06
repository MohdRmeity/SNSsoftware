
Imports System.Data.SqlClient
Imports NLog
Imports System.Net.Mail

Partial Public Class Cufex_ResetPasswordByCode
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.section = Cufex_Site.SectionName.ResetPassword
            txtCode.Focus()
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
        End If
    End Sub
    Public Sub Submit()
        CVError1.IsValid = True
        CVError1.ErrorMessage = ""
        Dim dt As DataTable = New DataTable()
        Dim userID As String = Session("userID").ToString
        Dim userkey As String = Session("userkey").ToString
        Dim email As String = Session("email").ToString
        Dim Code As String = "", EndDate As String = ""

        If txtPassword.Text <> txtConfirmPassword.Text Then
            CVError1.IsValid = False
            CVError1.ErrorMessage = "Error: Password and confirm password do not match!"
        Else
            If CommonMethods.dbtype = "sql" Then
                Try
                    Using connection As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                        connection.Open()
                        Dim query As SqlCommand = New SqlCommand("select Code, EndDate, UserId  from dbo.ResetCode where UserId=@userID and code =@code", connection)
                        query.Parameters.AddWithValue("@userID", userID)
                        query.Parameters.AddWithValue("@code", Trim(txtCode.Text))
                        Using people As SqlDataAdapter = New SqlDataAdapter
                            people.SelectCommand = query
                            people.Fill(dt)
                            For Each row As DataRow In dt.Rows
                                Code = row("Code").ToString()
                                EndDate = row("EndDate").ToString()
                                userID = row("userID").ToString()
                            Next
                        End Using
                        connection.Close()
                    End Using
                    If String.IsNullOrEmpty(Code) Or String.IsNullOrEmpty(EndDate) Or String.IsNullOrEmpty(userID) Then
                        CVError1.IsValid = False
                        CVError1.ErrorMessage = "Invalid Code."
                    Else
                        If DateTime.UtcNow > DateTime.Parse(EndDate) Then
                            CVError1.IsValid = False
                            CVError1.ErrorMessage = "Code Expired!"
                        Else
                            Dim chekcomplex As Boolean = CommonMethods.checkPassComplexity(txtConfirmPassword.Text)
                            If Not chekcomplex Then
                                CVError1.IsValid = False
                                If txtConfirmPassword.Text.Length < 10 Then
                                    CVError1.ErrorMessage = "Error: Password must have at least 10 characters"
                                Else
                                    CVError1.ErrorMessage = "Error: Password must have one upper case letter, one lower case letter and one base 10 digits (0 to 9)"
                                End If
                            Else
                                Dim key As String = CommonMethods.CreateSalt(txtConfirmPassword.Text.Length)
                                If CommonMethods.dbtype = "sql" Then
                                    Dim update As String = "update dbo.PORTALUSERS set EDITWHO= @edit, EDITDATE= @editdate , PASSWORDDATE=@passdate ,PASSWORD= @passw, HASHKEY= @keyh  where  USERKEY= @ukey ;"
                                    Try
                                        Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                                        conn.Open()
                                        Dim cmd As SqlCommand = New SqlCommand(update, conn)
                                        cmd.Parameters.AddWithValue("@edit", userkey)
                                        cmd.Parameters.AddWithValue("@editdate", Now)
                                        cmd.Parameters.AddWithValue("@passdate", Now)
                                        cmd.Parameters.AddWithValue("@passw", CommonMethods.GenerateHash(txtConfirmPassword.Text, key))
                                        cmd.Parameters.AddWithValue("@keyh", key)
                                        cmd.Parameters.AddWithValue("@ukey", userkey)
                                        cmd.ExecuteNonQuery()
                                        conn.Close()
                                        Dim msg As String = Resources.Resources.Email_PasswordReset
                                        CommonMethods.SendEmail(New MailAddress(email, userkey), "SNS Password Reset", String.Format(msg, userkey, txtConfirmPassword.Text))
                                        Session("userID") = Nothing
                                        Session("userkey") = Nothing
                                        Response.Redirect(Page.GetRouteUrl("SNSsoftware-CMS", Nothing))
                                    Catch e1 As Exception
                                        CVError1.IsValid = False
                                        CVError1.ErrorMessage += "Error: " & e1.Message & vbTab + e1.GetType.ToString & "<br/>"
                                        Dim logger As Logger = LogManager.GetCurrentClassLogger()
                                        logger.Error(e1, "", "")
                                    End Try
                                End If
                            End If
                        End If
                    End If
                Catch exp As Exception
                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                    logger.Error(exp, "", "")
                    CVError1.IsValid = False
                    CVError1.ErrorMessage = "Error: Database server is unreachable"
                End Try
            End If
        End If
    End Sub
    Private Sub BtnSubmit_Click(sender As Object, e As EventArgs) Handles BtnSubmit.Click
        Submit()
    End Sub
End Class
