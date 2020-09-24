Imports System.Data.SqlClient
Imports System.IO
Imports Newtonsoft.Json
Imports NLog
Imports Oracle.ManagedDataAccess.Client

Public Class FileUpload
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub Upload(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "text/plain"

        Dim dirFullPath As String = HttpContext.Current.Server.MapPath("~/DynamicFiles/FileManagement/")
        Dim Key As String = HttpContext.Current.Request.Item("Key")
        Dim SearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim ActivityType As String = HttpContext.Current.Request.Item("ActivityType")
        Dim str_file As String = "", fileSize As String = "", tmp As String = ""

        Dim Warehouse As String = IIf(ActivityType = "upload", HttpContext.Current.Request.Item("Facility"), CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Facility")))
        If LCase(Warehouse.Substring(0, 6)) = "infor_" Then Warehouse = Warehouse.Substring(6, Warehouse.Length - 6)
        If LCase(Warehouse).Contains("_") Then Warehouse = Warehouse.Split("_")(1)

        If ActivityType = "upload" Then
            Dim fileName As String = "", fileExtension As String = "", pathToSave As String = ""
            Dim File As HttpPostedFile
            For Each s As String In context.Request.Files
                File = context.Request.Files(s)
                fileName = File.FileName
                fileSize = File.ContentLength
                If Not String.IsNullOrEmpty(fileName) Then
                    fileExtension = Path.GetExtension(fileName)
                    str_file = SearchTable & "-" & Warehouse & "-" & Key & "-" & fileName.Substring(0, fileName.LastIndexOf(".")) & "-" & Now.Ticks & fileExtension
                    pathToSave = dirFullPath & str_file
                    tmp = SaveUploadedFile(SearchTable, Warehouse, Key, fileName, str_file, fileSize)
                    If tmp = "" Then tmp = CommonMethods.SaveFileActiviyLogs(SearchTable, Warehouse, Key, fileName, str_file, Val(fileSize), ActivityType)
                    If tmp = "" Then File.SaveAs(pathToSave)
                End If
            Next
        Else
            Dim OriginalFileName As String = HttpContext.Current.Request.Item("OriginalFileName")
            str_file = HttpContext.Current.Request.Item("FileName")
            fileSize = HttpContext.Current.Request.Item("FileSize")
            Dim TableName As String = "", KeyName As String = ""
            If SearchTable = "Warehouse_PO" Then
                TableName = "PO_FILES"
                KeyName = "POKEY"
            ElseIf SearchTable = "Warehouse_ASN" Then
                TableName = "RECEIPT_FILES"
                KeyName = "RECEIPTKEY"
            ElseIf SearchTable = "Warehouse_SO" Then
                TableName = "ORDERS_FILES"
                KeyName = "ORDERKEY"
            ElseIf SearchTable = "Warehouse_OrderManagement" Then
                TableName = "ORDERMANAG_FILES"
                KeyName = "ORDERMANAGKEY"
            End If
            If ActivityType = "delete" Then tmp = (New SQLExec).Execute("Delete from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & TableName & " Where WHSEID='" & Warehouse & "' and " & KeyName & "='" & Key & "' and FileName = '" & str_file & "'")
            If tmp = "" Then tmp = CommonMethods.SaveFileActiviyLogs(SearchTable, Warehouse, Key, OriginalFileName, str_file, Val(fileSize), ActivityType)
            If tmp = "" Then
                Dim FileToDelete As String = dirFullPath & str_file
                If File.Exists(FileToDelete) Then File.Delete(FileToDelete)
            End If
        End If

        context.Response.Write(tmp & "~~~" & str_file)
    End Sub

    Private Function SaveUploadedFile(SearchTable, Warehouse, Key, OriginalFileName, FileName, FileSize) As String
        Dim tmp As String = ""
        Dim TableName As String = ""
        Dim KeyName As String = ""

        If SearchTable = "Warehouse_PO" Then
            TableName = "PO_FILES"
            KeyName = "POKEY"
        ElseIf SearchTable = "Warehouse_ASN" Then
            TableName = "RECEIPT_FILES"
            KeyName = "RECEIPTKEY"
        ElseIf SearchTable = "Warehouse_SO" Then
            TableName = "ORDERS_FILES"
            KeyName = "ORDERKEY"
        ElseIf SearchTable = "Warehouse_OrderManagement" Then
            TableName = "ORDERMANAG_FILES"
            KeyName = "ORDERMANAGKEY"
        End If

        Try
            If CommonMethods.dbtype = "sql" Then
                Dim insert As String = "set dateformat dmy insert into dbo." & TableName & " (WHSEID," & KeyName & ",OriginalFileName,FileName,FileSize,ADDDATE,ADDWHO) values (@warehouse, @key,@originalfilename,@filename,@filesize,'" & Now & "',@ukey);"

                Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                conn.Open()

                Dim cmd As SqlCommand = New SqlCommand(insert, conn)
                cmd.Parameters.AddWithValue("@warehouse", Warehouse)
                cmd.Parameters.AddWithValue("@key", Key)
                cmd.Parameters.AddWithValue("@originalfilename", OriginalFileName)
                cmd.Parameters.AddWithValue("@filename", FileName)
                cmd.Parameters.AddWithValue("@filesize", FileSize)
                cmd.Parameters.AddWithValue("@ukey", HttpContext.Current.Session("userkey"))
                cmd.ExecuteNonQuery()

                conn.Close()
            Else
                Dim insert As String = "set dateformat dmy insert into SYSTEM." & TableName & " (WHSEID," & KeyName & ",OriginalFileName,FileName,ADDDATE,ADDWHO) values (:warehouse, :key,:originalfilename, :filename,:filesize,SYSDATE,:ukey);"
                Dim conn As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                conn.Open()

                Dim cmd As OracleCommand = New OracleCommand(insert, conn)
                cmd.Parameters.Add(New OracleParameter("warehouse", Warehouse))
                cmd.Parameters.Add(New OracleParameter("key", Key))
                cmd.Parameters.Add(New OracleParameter("originalfilename", OriginalFileName))
                cmd.Parameters.Add(New OracleParameter("filename", FileName))
                cmd.Parameters.Add(New OracleParameter("filesize", FileSize))
                cmd.Parameters.Add(New OracleParameter("ukey", HttpContext.Current.Session("userkey")))
                cmd.ExecuteNonQuery()

                conn.Close()
            End If
        Catch ex As Exception
            tmp += "Error: " & ex.Message & vbTab + ex.GetType.ToString & "<br/>"
            Dim logger As Logger = LogManager.GetCurrentClassLogger()
            logger.Error(ex, "", "")
        End Try
        Return tmp
    End Function

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class