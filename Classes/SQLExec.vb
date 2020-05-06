
Imports System.Data.SqlClient
Imports Oracle.ManagedDataAccess.Client

Public Enum Convert
    ToString = 1
    ToHTML = 2
End Enum
Public Class SQLExec
    Public Function Cursor(ByVal SQL As String, ByRef DA As SqlDataAdapter, ByRef CN As SqlConnection, Optional ByVal CNString As String = "", Optional ByRef Errorstring As String = "") As DataSet
        CN = New SqlConnection(IIf(CNString = "", ConfigurationManager.AppSettings("ConnectionString2"), CNString))

        Try
            CN.Open()
            DA = New SqlDataAdapter(SQL, CN)
            Dim objDataSet As New DataSet

            DA.Fill(objDataSet, "Cursor")
            objDataSet.AcceptChanges()

            Return objDataSet
        Catch Objerror As Exception
            Errorstring = Objerror.Message & "<br />" & Objerror.Source
            Return Nothing
        End Try
    End Function

    Public Function Cursor(ByVal SQL() As String, ByRef DA() As SqlDataAdapter, ByRef CN As SqlConnection, Optional ByVal CNString As String = "", Optional ByRef Errorstring As String = "") As DataSet


        CN = New SqlConnection(IIf(CNString = "", ConfigurationManager.AppSettings("ConnectionString2"), CNString))
        Try
            CN.Open()
            Dim objDataSet As New DataSet
            ReDim DA(UBound(SQL))
            For i = 0 To UBound(SQL)
                DA(i) = New SqlDataAdapter(SQL(i), CN)
                DA(i).Fill(objDataSet, "Cursor" & CStr(i))
            Next
            objDataSet.AcceptChanges()

            Return objDataSet
        Catch Objerror As Exception
            Errorstring = "<b>* Error while updating original data</b>.<br />" & Objerror.Message & "<br />" & Objerror.Source

            Return Nothing
        End Try

    End Function
    Public Function Cursor(ByVal SQL() As String, Optional ByVal CNString As String = "", Optional ByRef Errorstring As String = "") As DataSet
        Dim DA() As SqlDataAdapter = Nothing, CN As SqlConnection = Nothing



        CN = New SqlConnection(IIf(CNString = "", ConfigurationManager.AppSettings("ConnectionString2"), CNString))
        Try
            CN.Open()
            Dim objDataSet As New DataSet
            ReDim DA(UBound(SQL))
            For i = 0 To UBound(SQL)
                DA(i) = New SqlDataAdapter(SQL(i), CN)
                DA(i).Fill(objDataSet, "Cursor" & CStr(i))
            Next
            objDataSet.AcceptChanges()

            CN.Close()
            CN.Dispose()
            CN = Nothing

            Return objDataSet
        Catch Objerror As Exception
            Errorstring = "<b>* Error while updating original data</b>.<br />" & Objerror.Message & "<br />" & Objerror.Source

            Return Nothing
        End Try

    End Function
    'Public Function Cursor(ByVal SQL As String, Optional ByVal CNString As String = "", Optional ByRef Errorstring As String = "") As DataSet
    '    Try
    '        'Dim CN As SqlConnection = New SqlConnection(IIf(CNString = "", ConfigurationManager.AppSettings("ConnectionString2"), CNString))
    '        Dim CN As SqlConnection = New SqlConnection(IIf(CNString = "", ConfigurationManager.ConnectionStrings("DBConnect").ConnectionString, CNString))

    '        CN.Open()
    '        Dim da As SqlDataAdapter = New SqlDataAdapter(SQL, CN)
    '        Dim objDataSet As New DataSet

    '        da.Fill(objDataSet, "Cursor")
    '        objDataSet.AcceptChanges()

    '        CN.Close()
    '        CN.Dispose()
    '        CN = Nothing

    '        Return objDataSet
    '    Catch Objerror As Exception
    '        Errorstring = Objerror.Message & "<br />" & Objerror.Source
    '        Return Nothing
    '    End Try
    'End Function
    Public Function Cursor(ByVal SQL As String, Optional ByVal CNString As String = "", Optional ByRef Errorstring As String = "") As DataSet
        Try
            Dim objDataSet As New DataSet
            If CommonMethods.dbtype = "sql" Then
                Dim CN As SqlConnection = New SqlConnection(IIf(CNString = "", ConfigurationManager.ConnectionStrings("DBConnect").ConnectionString, CNString))

                CN.Open()
                Dim da As SqlDataAdapter = New SqlDataAdapter(SQL, CN)

                da.Fill(objDataSet, "Cursor")
                objDataSet.AcceptChanges()

                CN.Close()
                CN.Dispose()
                CN = Nothing
            Else
                Dim CN As OracleConnection = New OracleConnection(IIf(CNString = "", ConfigurationManager.ConnectionStrings("DBConnect").ConnectionString, CNString))

                CN.Open()
                Dim da As OracleDataAdapter = New OracleDataAdapter(SQL, CN)

                da.Fill(objDataSet, "Cursor")
                objDataSet.AcceptChanges()

                CN.Close()
                CN.Dispose()
                CN = Nothing
            End If

            Return objDataSet
        Catch Objerror As Exception
            Errorstring = Objerror.Message & "<br />" & Objerror.Source
            Return Nothing
        End Try
    End Function
    Public Function Execute(ByVal sql As String, Optional ByVal CNString As String = "") As String
        'Dim cn As SqlConnection = New SqlConnection(IIf(CNString = "", ConfigurationManager.AppSettings("ConnectionString2"), CNString))
        'Dim cn As SqlConnection = New SqlConnection(IIf(CNString = "", CommonMethods.dbconx, CNString))
        Try
            If CommonMethods.dbtype = "sql" Then
                Dim cn As SqlConnection = New SqlConnection(IIf(CNString = "", CommonMethods.dbconx, CNString))
                cn.Open()
                Dim cmd As New SqlCommand(sql, cn)
                cmd.CommandType = CommandType.Text
                cmd.ExecuteNonQuery()
                cn.Close()
                cn.Dispose()
                cn = Nothing
            Else
                Dim cn As OracleConnection = New OracleConnection(IIf(CNString = "", CommonMethods.dbconx, CNString))
                cn.Open()
                Dim cmd As New OracleCommand(sql, cn)
                cmd.CommandType = CommandType.Text
                cmd.ExecuteNonQuery()
                cn.Close()
                cn.Dispose()
                cn = Nothing
            End If

            Return Nothing
        Catch Objerror As Exception
            Return "<b>* Error while updating original data</b>.<br />" _
                & Objerror.Message & "<br />" & Objerror.Source
        End Try
    End Function

    Public Function commitChanges(ByVal objDataAdapter As SqlDataAdapter, ByVal objDataSet As DataSet, ByVal cn As SqlConnection, Optional ByVal CNString As String = "") As String
        Dim objTransaction As SqlTransaction = Nothing
        Try
            Dim objCommandBuilder As New SqlCommandBuilder(objDataAdapter)
            objDataAdapter.InsertCommand = objCommandBuilder.GetInsertCommand()
            objDataAdapter.UpdateCommand = objCommandBuilder.GetUpdateCommand()
            objDataAdapter.DeleteCommand = objCommandBuilder.GetDeleteCommand()

            'start a transaction so that we can roll back the changes
            'must do this on an open Connection object
            objTransaction = cn.BeginTransaction()

            'attach the current transaction to all the Command objects
            'must be done after setting Connection property
            objDataAdapter.InsertCommand.Transaction = objTransaction
            objDataAdapter.UpdateCommand.Transaction = objTransaction
            objDataAdapter.DeleteCommand.Transaction = objTransaction

            'perform the update on the original data
            'For i = 0 To objDataSet.tables.Count - 1
            objDataAdapter.Update(objDataSet, "Cursor")
            'Next
            objTransaction.Commit()
            cn.Close()
            cn.Dispose()
            cn = Nothing
            Return Nothing
        Catch objError As Exception

            'rollback the transaction undoing any updates
            objTransaction.Rollback()

            'display error details
            Return "<b>* Error while updating original data</b>.<br />" _
               & objError.Message & "<br />" & objError.Source
        End Try

    End Function
    Public Function commitChanges(ByVal objDataAdapter() As SqlDataAdapter, ByVal objDataSet As DataSet, ByVal cn As SqlConnection, Optional ByVal CNString As String = "") As String
        Dim objTransaction As SqlTransaction = Nothing
        Dim i As Byte
        Try
            For i = 0 To UBound(objDataAdapter)
                Dim objCommandBuilder As New SqlCommandBuilder(objDataAdapter(i))
                objDataAdapter(i).InsertCommand = objCommandBuilder.GetInsertCommand()
                objDataAdapter(i).UpdateCommand = objCommandBuilder.GetUpdateCommand()
                objDataAdapter(i).DeleteCommand = objCommandBuilder.GetDeleteCommand()

            Next
            'start a transaction so that we can roll back the changes
            'must do this on an open Connection object
            objTransaction = cn.BeginTransaction()

            'attach the current transaction to all the Command objects
            'must be done after setting Connection property
            For i = 0 To UBound(objDataAdapter)
                objDataAdapter(i).InsertCommand.Transaction = objTransaction
                objDataAdapter(i).UpdateCommand.Transaction = objTransaction
                objDataAdapter(i).DeleteCommand.Transaction = objTransaction
            Next
            'perform the update on the original data
            'For i = 0 To objDataSet.tables.Count - 1
            For i = 0 To UBound(objDataAdapter)
                objDataAdapter(i).Update(objDataSet, "Cursor" & CStr(i))
            Next
            objTransaction.Commit()
            cn.Close()
            cn.Dispose()
            cn = Nothing

            Return Nothing
        Catch objError As Exception
            'rollback the transaction undoing any updates
            objTransaction.Rollback()
            'display error details
            Return "<b>* Error while updating original data</b>.<br />" _
               & objError.Message & "<br />" & objError.Source
        End Try

    End Function
    Public Function ConvertString(ByVal Expression As String, ByVal Direction As Convert) As String
        Const ArraySize As Integer = 20
        Dim Ents(ArraySize) As String
        Dim Chrs(ArraySize) As String
        Dim j As Byte
        Dim cnvrt As String
        Chrs(0) = Chr(224) : Ents(0) = "&agrave;"
        Chrs(1) = Chr(226) : Ents(1) = "&acirc;"
        Chrs(2) = Chr(63) : Ents(2) = "&aelig;"
        Chrs(3) = Chr(231) : Ents(3) = "&ccedil;"
        Chrs(4) = Chr(232) : Ents(4) = "&egrave;"
        Chrs(5) = Chr(233) : Ents(5) = "&eacute;"
        Chrs(6) = Chr(234) : Ents(6) = "&ecirc;"
        Chrs(7) = Chr(235) : Ents(7) = "&euml;"
        Chrs(8) = Chr(63) : Ents(8) = "&igrave;"
        Chrs(9) = Chr(238) : Ents(9) = "&icirc;"
        Chrs(10) = Chr(239) : Ents(10) = "&iuml;"
        Chrs(11) = Chr(244) : Ents(11) = "&ocirc;"
        Chrs(12) = Chr(249) : Ents(12) = "&ugrave;"
        Chrs(13) = Chr(251) : Ents(13) = "&ucirc;"
        Chrs(14) = Chr(252) : Ents(14) = "&uuml;"
        Chrs(15) = Chr(39) : Ents(15) = "&#39;"
        Chrs(16) = Chr(156) : Ents(16) = "&oelig;"
        Chrs(17) = Chr(169) : Ents(17) = "&#169"
        Chrs(18) = vbCrLf : Ents(18) = "<br />"
        Chrs(19) = Chr(34) : Ents(19) = "&quot;"
        Chrs(20) = vbCrLf : Ents(20) = "<br/>"
        cnvrt = Expression
        For j = 0 To ArraySize
            If Direction = Convert.ToHTML Then
                If cnvrt <> "" Then cnvrt = Replace(cnvrt, Chrs(j), Ents(j))
            Else
                If cnvrt <> "" Then cnvrt = Replace(cnvrt, Ents(j), Chrs(j))

            End If
        Next
        Return cnvrt
    End Function
    Function FilterStr(ByVal RequestVariable As String) As String
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
    Function FilterText(ByVal Text As String) As String
        Dim EndResult As String
        Dim MyStr As String
        EndResult = Text
        If IsNumeric(EndResult) Then
            FilterText = EndResult
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

        FilterText = IIf(EndResult Is Nothing, "", EndResult)
    End Function
    Function StripHTML(ByVal htmlString As String, ByVal ReplaceSpacewidthash As Boolean) As String
        Dim pattern As String = "<(.|\n)*?>"
        Dim tmp As String
        tmp = Text.RegularExpressions.Regex.Replace(htmlString, pattern, String.Empty)
        If ReplaceSpacewidthash Then
            tmp = Replace(tmp, " ", "-") 'avoid -- in URL since it is a hack sequence
            tmp = Replace(tmp, "---", "-")
            tmp = Replace(tmp, "--", "-")
            tmp = Replace(tmp, "'", "")
        End If
        Return tmp
    End Function

End Class
