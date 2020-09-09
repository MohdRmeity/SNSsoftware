
Public Class clsGlobals
    Implements IRequiresSessionState
    Public Const ConstAppVersion As Integer = 4
    Public Shared WebAppVersion As Integer = Val(ConfigurationManager.AppSettings("AppVersion"))
    Public Shared AppVersion As Integer = IIf(ConstAppVersion > WebAppVersion, ConstAppVersion, WebAppVersion)
    Public Shared ReadOnly Property sAppPath() As String
        Get
            'Edited by Nader 
            'If HttpContext.Current.Session Is Nothing Then HttpContext.Current.Session("clsGlobals_sAppPath") = ""
            If Not HttpContext.Current.Session Is Nothing Then

                If HttpContext.Current.Session("clsGlobals_sAppPath") Is Nothing Then HttpContext.Current.Session("clsGlobals_sAppPath") = ""
                If HttpContext.Current.Session("clsGlobals_sAppPath") = "" Then
                    HttpContext.Current.Session("clsGlobals_sAppPath") = HttpContext.Current.Request.ApplicationPath() & IIf(HttpContext.Current.Request.ApplicationPath() = "/", "", "/").ToString()
                End If
                sAppPath = HttpContext.Current.Session("clsGlobals_sAppPath")
            Else
                sAppPath = HttpContext.Current.Request.ApplicationPath() & IIf(HttpContext.Current.Request.ApplicationPath() = "/", "", "/").ToString()
            End If
        End Get
    End Property
    Public Shared Function GetLebanonTime() As Date
        Dim MyDate As Date = DateAdd(DateInterval.Hour, 3, Date.UtcNow)
        Return MyDate
    End Function
    Public Shared Function fixMyString(ByVal strWithHtml As String, Optional ByVal iLength As Integer = 0) As String
        Dim tb As New SQLExec
        'Nader Added the converttostring wrapped around the striphtml to allow the pass of double quotes
        Dim str As String
        str = tb.ConvertString(strWithHtml, Convert.ToString)
        str = StripHTML(str)
        str = tb.ConvertString(str, Convert.ToHTML)


        If Len(str) > iLength And iLength > 0 Then
            str = Left(str, iLength)
            Dim iLastSpaceIndex As Integer = str.LastIndexOf(" ")

            If iLastSpaceIndex > 0 Then
                str = str.Substring(0, iLastSpaceIndex) + "..."
            Else
                str = str + "..."

            End If
        End If
        Return str
    End Function
    Public Shared Function StripHTML(ByVal source As String) As String
        Try
            Dim result As String

            ' Remove HTML Development formatting
            ' Replace line breaks with space
            ' because browsers inserts space
            result = source.Replace(vbCr, " ")
            ' Replace line breaks with space
            ' because browsers inserts space
            result = result.Replace(vbLf, " ")
            ' Remove step-formatting
            result = result.Replace(vbTab, String.Empty)
            ' Remove repeating spaces because browsers ignore them
            result = Regex.Replace(result, "( )+", " ")

            ' Remove the header (prepare first by clearing attributes)
            result = Regex.Replace(result, "<( )*head([^>])*>", "<head>", RegexOptions.IgnoreCase)
            result = Regex.Replace(result, "(<( )*(/)( )*head( )*>)", "</head>", RegexOptions.IgnoreCase)
            result = Regex.Replace(result, "(<head>).*(</head>)", String.Empty, RegexOptions.IgnoreCase)

            ' remove all scripts (prepare first by clearing attributes)
            result = Regex.Replace(result, "<( )*script([^>])*>", "<script>", RegexOptions.IgnoreCase)
            result = Regex.Replace(result, "(<( )*(/)( )*script( )*>)", "</script>", RegexOptions.IgnoreCase)
            'result = Regex.Replace(result,
            '         @"(<script>)([^(<script>\.</script>)])*(</script>)",
            '         string.Empty,
            '         RegexOptions.IgnoreCase);
            result = Regex.Replace(result, "(<script>).*(</script>)", String.Empty, RegexOptions.IgnoreCase)

            ' remove all styles (prepare first by clearing attributes)
            result = Regex.Replace(result, "<( )*style([^>])*>", "<style>", RegexOptions.IgnoreCase)
            result = Regex.Replace(result, "(<( )*(/)( )*style( )*>)", "</style>", RegexOptions.IgnoreCase)
            result = Regex.Replace(result, "(<style>).*(</style>)", String.Empty, RegexOptions.IgnoreCase)

            ' insert tabs in spaces of <td> tags
            result = Regex.Replace(result, "<( )*td([^>])*>", vbTab, RegexOptions.IgnoreCase)

            ' insert line breaks in places of <br /> and <LI> tags
            result = Regex.Replace(result, "<( )*br( )*>", vbCr, RegexOptions.IgnoreCase)
            result = Regex.Replace(result, "<( )*li( )*>", vbCr, RegexOptions.IgnoreCase)

            ' insert line paragraphs (double line breaks) in place
            ' if <P>, <DIV> and <TR> tags
            result = Regex.Replace(result, "<( )*div([^>])*>", vbCr & vbCr, RegexOptions.IgnoreCase)
            result = Regex.Replace(result, "<( )*tr([^>])*>", vbCr & vbCr, RegexOptions.IgnoreCase)
            result = Regex.Replace(result, "<( )*p([^>])*>", vbCr & vbCr, RegexOptions.IgnoreCase)

            ' Remove remaining tags like <a>, links, images,
            ' comments etc - anything that's enclosed inside < >
            result = Regex.Replace(result, "<[^>]*>", String.Empty, RegexOptions.IgnoreCase)

            ' replace special characters:
            result = Regex.Replace(result, " ", " ", RegexOptions.IgnoreCase)

            result = Regex.Replace(result, "&bull;", " * ", RegexOptions.IgnoreCase)
            result = Regex.Replace(result, "&lsaquo;", "<", RegexOptions.IgnoreCase)
            result = Regex.Replace(result, "&rsaquo;", ">", RegexOptions.IgnoreCase)

            result = Regex.Replace(result, "&nbsp;", " ", RegexOptions.IgnoreCase)

            result = Regex.Replace(result, "&trade;", "(tm)", RegexOptions.IgnoreCase)
            result = Regex.Replace(result, "&frasl;", "/", RegexOptions.IgnoreCase)
            result = Regex.Replace(result, "&lt;", "<", RegexOptions.IgnoreCase)
            result = Regex.Replace(result, "&gt;", ">", RegexOptions.IgnoreCase)
            result = Regex.Replace(result, "&copy;", "(c)", RegexOptions.IgnoreCase)
            result = Regex.Replace(result, "&reg;", "(r)", RegexOptions.IgnoreCase)
            ' Remove all others. More can be added, see
            ' http://hotwired.lycos.com/webmonkey/reference/special_characters/
            result = Regex.Replace(result, "&(.{2,6});", String.Empty, RegexOptions.IgnoreCase)

            ' for testing
            'Regex.Replace(result,
            '       this.txtRegex.Text,string.Empty,
            '       RegexOptions.IgnoreCase);

            ' make line breaking consistent
            result = result.Replace(vbLf, vbCr)

            ' Remove extra line breaks and tabs:
            ' replace over 2 breaks with 2 and over 4 tabs with 4.
            ' Prepare first to remove any whitespaces in between
            ' the escaped characters and remove redundant tabs in between line breaks
            result = Regex.Replace(result, "(" & vbCr & ")( )+(" & vbCr & ")", vbCr & vbCr, RegexOptions.IgnoreCase)
            result = Regex.Replace(result, "(" & vbTab & ")( )+(" & vbTab & ")", vbTab & vbTab, RegexOptions.IgnoreCase)
            result = Regex.Replace(result, "(" & vbTab & ")( )+(" & vbCr & ")", vbTab & vbCr, RegexOptions.IgnoreCase)
            result = Regex.Replace(result, "(" & vbCr & ")( )+(" & vbTab & ")", vbCr & vbTab, RegexOptions.IgnoreCase)
            ' Remove redundant tabs
            result = Regex.Replace(result, "(" & vbCr & ")(" & vbTab & ")+(" & vbCr & ")", vbCr & vbCr, RegexOptions.IgnoreCase)
            ' Remove multiple tabs following a line break with just one tab
            result = Regex.Replace(result, "(" & vbCr & ")(" & vbTab & ")+", vbCr & vbTab, RegexOptions.IgnoreCase)
            ' Initial replacement target string for line breaks
            Dim breaks As String = vbCr & vbCr & vbCr
            ' Initial replacement target string for tabs
            Dim tabs As String = vbTab & vbTab & vbTab & vbTab & vbTab
            For index As Integer = 0 To result.Length - 1
                result = result.Replace(breaks, vbCr & vbCr)
                result = result.Replace(tabs, vbTab & vbTab & vbTab & vbTab)
                breaks = breaks & vbCr
                tabs = tabs & vbTab
            Next

            ' That's it.
            Return result
        Catch
            'MessageBox.Show("Error");
            Return source
        End Try
    End Function
    Public Shared Function FilterStr(ByVal RequestVariable As String) As String
        Dim EndResult As String
        Dim NumericResult As Integer
        Dim MyStr As String
        NumericResult = 0
        EndResult = System.Web.HttpContext.Current.Request(RequestVariable)
        If IsNumeric(EndResult) Then
            NumericResult = EndResult
            FilterStr = NumericResult
            Exit Function
        End If
        If InStr(LCase(EndResult), "master..xp_cmdshell") > 0 Then
            MyStr = Mid(EndResult, InStr(LCase(EndResult), "master..xp_cmdshell"), 19)
            EndResult = Replace(EndResult, MyStr, "")
        End If
        If InStr(LCase(EndResult), "master..xp_startmail") > 0 Then
            MyStr = Mid(EndResult, InStr(LCase(EndResult), "master..xp_startmail"), 20)
            EndResult = Replace(EndResult, MyStr, "")
        End If
        If InStr(LCase(EndResult), "master..xp_sendmail") > 0 Then
            MyStr = Mid(EndResult, InStr(LCase(EndResult), "master..xp_sendmail"), 19)
            EndResult = Replace(EndResult, MyStr, "")
        End If
        If InStr(LCase(EndResult), "master..sp_makewebtask") > 0 Then
            MyStr = Mid(EndResult, InStr(LCase(EndResult), "master..sp_makewebtask"), 22)
            EndResult = Replace(EndResult, MyStr, "")
        End If
        If InStr(LCase(EndResult), "union ") > 0 Then
            If InStr(LCase(EndResult), "union se") > 0 Then
                MyStr = Mid(EndResult, InStr(LCase(EndResult), "union se"), 8)
                EndResult = Replace(EndResult, MyStr, "")
            End If
            If InStr(LCase(EndResult), "union  se") > 0 Then
                MyStr = Mid(EndResult, InStr(LCase(EndResult), "union  se"), 9)
                EndResult = Replace(EndResult, MyStr, "")
            End If
            If InStr(LCase(EndResult), "union   se") > 0 Then
                MyStr = Mid(EndResult, InStr(LCase(EndResult), "union   se"), 10)
                EndResult = Replace(EndResult, MyStr, "")
            End If
            If InStr(LCase(EndResult), "union    se") > 0 Then
                MyStr = Mid(EndResult, InStr(LCase(EndResult), "union    se"), 11)
                EndResult = Replace(EndResult, MyStr, "")
            End If
            If InStr(LCase(EndResult), "union     se") > 0 Then
                MyStr = Mid(EndResult, InStr(LCase(EndResult), "union     se"), 12)
                EndResult = Replace(EndResult, MyStr, "")
            End If
        End If

        EndResult = Replace(EndResult, ";", ",")
        EndResult = Replace(EndResult, "--", "")
        EndResult = Replace(EndResult, "../", "~~~")
        EndResult = Replace(EndResult, "..\", "")
        EndResult = Replace(EndResult, "..", "")
        EndResult = Replace(EndResult, "'", "&#39;")
        EndResult = Replace(EndResult, """", "&quot;")
        EndResult = Replace(EndResult, Chr(0), "")

        FilterStr = IIf(EndResult Is Nothing, "", EndResult)
        'Response.Write FilterStr
        'Response.End 
    End Function
End Class
