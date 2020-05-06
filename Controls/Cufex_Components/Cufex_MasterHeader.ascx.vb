Partial Public Class Cufex_MasterHeader
    Inherits MultiLingualControl
    Public BUserCode As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            If myMasterPage.BUserCode <> 0 Then welcomeNames(myMasterPage.BUserCode)
            BUserCode = myMasterPage.BUserCode
        End If
    End Sub
    Private Sub welcomeNames(ByVal code As Integer)
        Dim tb As SQLExec = New SQLExec
        Dim DS As DataSet
        Dim sql, Name As String

        'sql = "select isnull(ProfilePicture,'')Profile, * from Cufex_Users where id = " & code
        sql = "select * from dbo.PORTALUSERS where id = " & code
        DS = tb.Cursor(sql)
        If DS.Tables(0).Rows.Count > 0 Then
            Name = DS.Tables(0).Rows(0)("FIRSTNAME") & " " & DS.Tables(0).Rows(0)("LASTNAME")
            lblWelcome.Text = "Hello, " & Name & "!"
            'If DS.Tables(0).Rows(0)!Profile <> "" Then
            '    imgProfile.Src = sAppPath & "DynamicImages/CufexProfiles/" & DS.Tables(0).Rows(0)!Profile
            'Else
            '    imgProfile.Src = sAppPath & "images/Cufex_Images/defaultP.jpg"
            'End If
            imgProfile.Src = sAppPath & "images/Cufex_Images/defaultP.jpg"
            lblWelcome.Visible = True
        End If

    End Sub


End Class