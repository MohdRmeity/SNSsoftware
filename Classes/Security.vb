Public Class Security
    Public Shared Sub FillCombobox(ByRef combobox As DropDownList, ByVal dt As DataTable, ByVal DataTextField As String, ByVal DataValueField As String, Optional ByVal DefaultString As String = "None")
        'Dim tb As SQLExec = New SQLExec
        'Dim ds As DataSet = New DataSet
        'Dim objDataTable As New DataTable
        Dim selectedValue As String = "0"
        'ds = tb.Cursor(query)
        combobox.ClearSelection()
        combobox.SelectedIndex = -1
        If Not combobox.SelectedItem Is Nothing Then
            selectedValue = combobox.SelectedValue
        End If
        If Not dt Is Nothing Then
            If dt.Rows.Count > 0 Then
                combobox.DataSource = dt
                combobox.DataTextField = DataTextField
                combobox.DataValueField = DataValueField
                combobox.DataBind()
            Else
                combobox.Items.Clear()
            End If
        End If

        If DefaultString <> "" Then
            combobox.Items.Insert(0, DefaultString)
            combobox.Items(0).Value = "0"
        End If


        If selectedValue <> "0" Then
            For i = 0 To combobox.Items.Count - 1
                If combobox.Items(i).Value = selectedValue Then
                    combobox.SelectedValue = selectedValue
                    Exit For
                End If
            Next
        End If
    End Sub
    Public Shared Sub FillHtmlSelect(ByRef combobox As HtmlSelect, ByVal dt As DataTable, ByVal DataTextField As String, ByVal DataValueField As String, Optional ByVal DefaultString As String = "None")
        Dim selectedValue As String = "0"

        'HtmlSelect Clear Selection
        For i = 0 To combobox.Items.Count - 1
            combobox.Items(i).Selected = False
        Next

        combobox.SelectedIndex = -1
        If Not combobox.Value Is Nothing Then
            selectedValue = combobox.Value
        End If
        If Not dt Is Nothing Then
            If dt.Rows.Count > 0 Then
                combobox.DataSource = dt
                combobox.DataTextField = DataTextField
                combobox.DataValueField = DataValueField
                combobox.DataBind()
            Else
                combobox.Items.Clear()
            End If
        End If

        If DefaultString <> "" Then
            combobox.Items.Insert(0, DefaultString)
            combobox.Items(0).Value = "0"
        End If


        If selectedValue <> "0" Then
            For i = 0 To combobox.Items.Count - 1
                If combobox.Items(i).Value = selectedValue Then
                    combobox.DataValueField = selectedValue
                    Exit For
                End If
            Next
        End If
    End Sub
    Public Sub SetLoginSession(ByVal ID As Integer)
        Dim tb As SQLExec = New SQLExec
        Dim sqlstr As String
        Dim ds As DataSet

        sqlstr = "Select * from dbo.PORTALUSERS  where id = " & ID & " and active = 1"
        ds = tb.Cursor(sqlstr)
        If ds.Tables(0).Rows.Count > 0 Then
            With ds.Tables(0).Rows(0)
                'HttpContext.Current.Session("BCounter") = 0
                HttpContext.Current.Session("BUserCode") = !ID
                HttpContext.Current.Session("BUserID") = !USERKEY
                HttpContext.Current.Session("BUserEmail") = !EMAIL
                'HttpContext.Current.Session("BUserGroup") = !GroupID
                HttpContext.Current.Session("BFullName") = !FIRSTNAME & " " & !LASTNAME
                'HttpContext.Current.Session("BCanClone") = !CanClone
                'HttpContext.Current.Session("BUserGroupName") = !UserGroupName
                HttpContext.Current.Session("BLog") = 1
            End With

        End If
    End Sub
End Class

