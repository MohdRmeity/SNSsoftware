Imports System.Data.SqlClient
Imports System.IO
Imports Newtonsoft.Json
Imports NLog
Imports Oracle.ManagedDataAccess.Client

Public Class ImportItems
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub Import(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "text/plain"

        Dim tmp As String = ""
        Dim dirFullPath As String = HttpContext.Current.Server.MapPath("~/DynamicFiles/ImportFiles/")
        Dim SearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim ImportFile As HttpPostedFile = System.Web.HttpContext.Current.Request.Files("ImportFile")
        Dim ImportFileSize As String = HttpContext.Current.Request.Item("ImportFileSize")

        Dim fileName As String = "", fileExtension As String = "", pathToSave As String = "", str_file As String = ""
        fileName = ImportFile.FileName
        If Not String.IsNullOrEmpty(fileName) Then
            fileExtension = Path.GetExtension(fileName)
            str_file = fileName.Substring(0, fileName.LastIndexOf(".")) & "-" & HttpContext.Current.Session("userkey") & "-" & Now.Ticks & fileExtension
            pathToSave = dirFullPath & str_file
            ImportFile.SaveAs(pathToSave)
        End If

        Dim dt As DataTable = clsCSV.ConvertCSVToDataTable(ImportFile)

        If SearchTable = "PORTALUSERS" Then
            tmp = ImportUsers(dt)
        ElseIf SearchTable = "USERCONTROL" Then
            tmp = ImportUserControl(dt)
        ElseIf SearchTable = "PROFILES" Then
            tmp = ImportProfile(dt)
        ElseIf SearchTable = "USERPROFILE" Then
            tmp = ImportUserProfile(dt)
        End If

        CommonMethods.SaveImportLogs(SearchTable, str_file, ImportFileSize, tmp)

        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)
        Using writer As JsonWriter = New JsonTextWriter(sw)
            writer.Formatting = Formatting.Indented
            writer.WriteStartObject()

            writer.WritePropertyName("Error")
            writer.WriteValue(tmp)
            writer.WriteEndObject()
        End Using
        context.Response.Write(sw)
        context.Response.End()
    End Sub
    Private Function ImportUsers(ByVal ImportTable As DataTable) As String
        Dim tmp As String = "", insertquery As String = "", updatequery As String = ""
        Dim EditOperation As Boolean

        If ImportTable IsNot Nothing Then
            If ImportTable.Rows.Count > 0 Then
                Dim dups = From row In ImportTable.AsEnumerable()
                           Let userkey = row.Field(Of String)("UserKey")
                           Group row By userkey Into DupUserKey = Group
                           Where DupUserKey.Count() > 1
                           Select DupUserKey

                If dups.Count > 0 Then
                    tmp = "Duplication values exist in UserKey column of the import file"
                End If

                If tmp = "" Then
                    For i = 0 To ImportTable.Rows.Count - 1
                        With ImportTable.Rows(i)
                            Dim firstname As String = !FirstName _
                            , lastname As String = !LastName _
                            , userkey As String = !UserKey _
                            , email As String = !Email _
                            , password As String = !Password _
                            , confirm As String = !ConfirmPassword _
                            , active As String = !Active _
                            , originalpass As String = "" _
                            , keypass As String = ""

                            If String.IsNullOrEmpty(userkey) Then
                                tmp += "User ID must be defined in line " & (i + 2).ToString & " <br/>"
                            ElseIf userkey.Length > 50 Then
                                tmp += "User ID cannot have more 50 characters in line " & (i + 2).ToString & " <br/>"
                            End If

                            If String.IsNullOrEmpty(firstname) Then
                                tmp += "First Name must be defined in line " & (i + 2).ToString & " <br/>"
                            ElseIf firstname.Length > 50 Then
                                tmp += "First Name cannot have more 50 characters in line " & (i + 2).ToString & " <br/>"
                            End If

                            If String.IsNullOrEmpty(lastname) Then
                                tmp += "Last Name must be defined in line " & (i + 2).ToString & " <br/>"
                            ElseIf lastname.Length > 50 Then
                                tmp += "Last Name cannot have more 50 characters in line " & (i + 2).ToString & " <br/>"
                            End If

                            If String.IsNullOrEmpty(email) Then
                                tmp += "Email must be defined in line " & (i + 2).ToString & " <br/>"
                            ElseIf email.Length > 100 Then
                                tmp += "Email cannot have more 100 characters in line " & (i + 2).ToString & " <br/>"
                            End If

                            If String.IsNullOrEmpty(active) Then
                                tmp += "Active must be defined in line " & (i + 2).ToString & " <br/>"
                            ElseIf active <> "0" And active <> "1" Then
                                tmp += "Active must be 0 or 1 in line " & (i + 2).ToString & " <br/>"
                            End If

                            Dim sql As String = "Select * from " & IIf(CommonMethods.dbtype <> "sql", "", "") & "PORTALUSERS where UserKey = '" & userkey & "'"
                            Dim ds As DataSet = (New SQLExec).Cursor(sql)
                            EditOperation = ds.Tables(0).Rows.Count > 0

                            If Not EditOperation Then
                                If String.IsNullOrEmpty(password) Then
                                    tmp += "Password must be defined in line " & (i + 2).ToString & " <br/>"
                                Else
                                    Dim checkcomplex As Boolean = CommonMethods.checkPassComplexity(password)
                                    If Not checkcomplex Then
                                        If password.Length < 10 Then
                                            tmp += "Password must have at least 10 characters in line " & (i + 2).ToString & " <br/>"
                                        Else
                                            tmp += "Password must have one upper case letter, one lower case letter and one base 10 digits (0 to 9) in line " & (i + 2).ToString & " <br/>"
                                        End If
                                    End If
                                End If

                                If Not String.IsNullOrEmpty(confirm) Then
                                    If confirm <> password Then
                                        tmp += "Password and Confirm Password do not match in line " & (i + 2).ToString & "! <br/>"
                                    End If
                                ElseIf Not String.IsNullOrEmpty(password) Then
                                    tmp += "Confirm Password must be filled in line " & (i + 2).ToString & " <br/>"
                                End If
                            Else
                                originalpass = CommonMethods.getPassword(userkey)
                                keypass = CommonMethods.getpasskey(userkey)
                                If String.IsNullOrEmpty(password) Then
                                    tmp += "Password must be defined in line " & (i + 2).ToString & " <br/>"
                                ElseIf originalpass <> password Then
                                    Dim checkcomplex As Boolean = CommonMethods.checkPassComplexity(password)
                                    If confirm <> password Then
                                        tmp += "Confirm Password must be filled in line " & (i + 2).ToString & " <br/>"
                                    ElseIf Not checkcomplex Then
                                        If password.Length < 10 Then
                                            tmp += "Password must have at least 10 characters in line " & (i + 2).ToString & " <br/>"
                                        Else
                                            tmp += "Password must have one upper case letter, one lower case letter, one special character and one base 10 digits (0 to 9) in line " & (i + 2).ToString & " <br/>"
                                        End If
                                    Else
                                        keypass = CommonMethods.CreateSalt(password.Length)
                                        originalpass = CommonMethods.GenerateHash(password, keypass)
                                    End If
                                End If
                            End If

                            If tmp = "" Then
                                If EditOperation Then
                                    If CommonMethods.dbtype = "sql" Then
                                        updatequery += " set dateformat dmy update dbo.PORTALUSERS set FirstName= '" & firstname & "', LastName = '" & lastname & "', ACTIVE= '" & active & "' , Password= '" & originalpass & "', Email = '" & email & "' , EDITWHO= '" & HttpContext.Current.Session("userkey").ToString & "' , EDITDATE='" & Now & "', HASHKEY= '" & keypass & "' where USERKEY = '" & userkey & "' "
                                    Else
                                        updatequery += " set dateformat dmy update SYSTEM.PORTALUSERS set FirstName = '" & firstname & "', LastName = '" & lastname & "', ACTIVE= '" & active & "' , Password= '" & originalpass & "', Email = '" & email & "' , EDITWHO= '" & HttpContext.Current.Session("userkey").ToString & "' , EDITDATE=SYSDATE, HASHKEY= '" & keypass & "' where USERKEY = '" & userkey & "' "
                                    End If
                                Else
                                    Dim keyh As String = CommonMethods.CreateSalt(password.Length)
                                    Dim pswd As String = CommonMethods.GenerateHash(password, keyh)
                                    If CommonMethods.dbtype = "sql" Then
                                        insertquery += " set dateformat dmy insert into dbo.PORTALUSERS (ACTIVE, USERKEY, FIRSTNAME, LASTNAME, EMAIL, PASSWORD, ADDWHO, EDITWHO , ADDDATE,EDITDATE , HASHKEY) values ('" & active & "', '" & userkey & "', '" & firstname & "', '" & lastname & "', '" & email & "', '" & pswd & "', '" & HttpContext.Current.Session("userkey").ToString & "',  '" & HttpContext.Current.Session("userkey").ToString & "', '" & Now & "', '" & Now & "', '" & keyh & "') "
                                        insertquery += " set dateformat dmy insert into dbo.USERWIDGETS (WIDGETID,USERKEY, ADDDATE) values (1,'" & LCase(userkey) & "','" & Now & "'),(4,'" & LCase(userkey) & "','" & Now & "'),(6,'" & LCase(userkey) & "','" & Now & "'),(7,'" & LCase(userkey) & "','" & Now & "') "
                                    Else
                                        insertquery += " set dateformat dmy insert into SYSTEM.PORTALUSERS (ACTIVE, USERKEY, FIRSTNAME, LASTNAME, EMAIL, PASSWORD, ADDWHO, EDITWHO , ADDDATE,EDITDATE , HASHKEY) values ('" & active & "', '" & userkey & "', '" & firstname & "', '" & lastname & "', '" & email & "', '" & pswd & "', '" & HttpContext.Current.Session("userkey").ToString & "',  '" & HttpContext.Current.Session("userkey").ToString & "', SYSDATE, SYSDATE, '" & keyh & "') "
                                        insertquery += " set dateformat dmy insert into SYSTEM.USERWIDGETS (WIDGETID,USERKEY, ADDDATE) values (1,'" & LCase(userkey) & "',SYSDATE),(4,'" & LCase(userkey) & "',SYSDATE),(6,'" & LCase(userkey) & "',SYSDATE),(7,'" & LCase(userkey) & "',SYSDATE) "
                                    End If
                                End If
                            End If

                        End With
                    Next

                    If tmp = "" Then
                        Try
                            If insertquery <> "" And updatequery = "" Then
                                tmp = (New SQLExec).Execute(insertquery)
                            ElseIf insertquery = "" And updatequery <> "" Then
                                tmp = (New SQLExec).Execute(updatequery)
                            Else
                                tmp = (New SQLExec).Execute(insertquery & " " & updatequery)
                            End If
                        Catch ex As Exception
                            tmp += ex.Message
                        End Try
                    End If
                End If
            Else
                tmp = "No records to import"
            End If
        Else
            tmp = "No records to import"
        End If

        Return tmp
    End Function
    Private Function ImportUserControl(ByVal ImportTable As DataTable) As String
        Dim tmp As String = "", sql As String = "", insertquery As String = "", updatequery As String = "", insertfacilityquery As String = "", deletefacilityquery As String = ""
        Dim EditOperation As Boolean
        Dim Int As Integer

        If ImportTable IsNot Nothing Then
            If ImportTable.Rows.Count > 0 Then
                Dim dups = From row In ImportTable.AsEnumerable()
                           Let userkey = row.Field(Of String)("UserKey")
                           Group row By userkey Into DupUserKey = Group
                           Where DupUserKey.Count() > 1
                           Select DupUserKey

                If dups.Count > 0 Then
                    tmp = "Duplication values exist in UserKey column of the import file"
                End If

                If tmp = "" Then
                    For i = 0 To ImportTable.Rows.Count - 1
                        With ImportTable.Rows(i)
                            Dim userkey As String = !UserKey _
                            , storer As String = UCase(!StorerKey) _
                            , consignee As String = UCase(!ConsigneeKey) _
                            , supplier As String = UCase(!SupplierKey) _
                            , facility As String = !Facility _
                            , exportrowslimit As String = !ExportRowsLimit _
                            , fileimportlimit As String = !FileImportLimit _
                            , fileuploadlimit As String = !FileUploadLimit

                            If String.IsNullOrEmpty(userkey) Then
                                tmp += "User ID must be defined in line " & (i + 2).ToString & " <br/>"
                            ElseIf userkey.Length > 50 Then
                                tmp += "User ID cannot have more 50 characters in line " & (i + 2).ToString & " <br/>"
                            End If

                            If String.IsNullOrEmpty(exportrowslimit) Then
                                exportrowslimit = 1000
                            Else
                                If Not Integer.TryParse(exportrowslimit, Int) Then
                                    tmp += "Export Rows Limit must be an Integer in line " & (i + 2).ToString & " <br/>"
                                Else
                                    If Val(exportrowslimit) < 0 Then
                                        tmp += "Export Rows Limit must be positive in line " & (i + 2).ToString & " <br/>"
                                    End If
                                End If
                            End If

                            If String.IsNullOrEmpty(fileimportlimit) Then
                                fileimportlimit = 5
                            Else
                                If Not Integer.TryParse(fileimportlimit, Int) Then
                                    tmp += "File Import Limit must be an Integer in line " & (i + 2).ToString & " <br/>"
                                Else
                                    If Val(fileimportlimit) < 0 Then
                                        tmp += "File Import Limit must be positive in line " & (i + 2).ToString & " <br/>"
                                    End If
                                End If
                            End If

                            If String.IsNullOrEmpty(fileuploadlimit) Then
                                fileuploadlimit = 5
                            Else
                                If Not Integer.TryParse(fileuploadlimit, Int) Then
                                    tmp += "File Upload Limit must be an Integer in line " & (i + 2).ToString & " <br/>"
                                Else
                                    If Val(fileuploadlimit) < 0 Then
                                        tmp += "File Upload Limit must be positive in line " & (i + 2).ToString & " <br/>"
                                    End If
                                End If
                            End If

                            Dim dt As DataTable = Nothing
                            Dim dr As DataRow() = Nothing
                            If Not String.IsNullOrEmpty(storer) Or Not String.IsNullOrEmpty(consignee) Or Not String.IsNullOrEmpty(supplier) Then
                                sql = " select StorerKey, Type from enterprise.storer where type in (1,2,5) "
                                dt = (New SQLExec).Cursor(sql).Tables(0)
                            End If

                            If String.IsNullOrEmpty(storer) Then
                                storer = "ALL"
                            Else
                                Dim storerArr As String() = storer.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)
                                For j = 0 To storerArr.Length - 1
                                    dr = dt.Select("Type=1 and StorerKey='" & Trim(storerArr(j)) & "'")
                                    If Trim(storerArr(j)) > 500 Then tmp += "Owner " & Trim(storerArr(j)) & " cannot have more than 500 characters in line " & (i + 2).ToString & " <br/>"
                                    If dr.Length = 0 Then tmp += "Owner " & Trim(storerArr(j)) & " does Not exist In line " & (i + 2).ToString & " <br/>"
                                Next
                            End If

                            If String.IsNullOrEmpty(consignee) Then
                                consignee = "ALL"
                            Else
                                Dim consigneArr As String() = consignee.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)
                                For j = 0 To consigneArr.Length - 1
                                    dr = dt.Select("Type=2 And StorerKey='" & Trim(consigneArr(j)) & "'")
                                    If Trim(consigneArr(j)) > 500 Then tmp += "Consignee " & Trim(consigneArr(j)) & " cannot have more than 500 characters in line " & (i + 2).ToString & " <br/>"
                                    If dr.Length = 0 Then tmp += "Consignee " & Trim(consigneArr(j)) & " does not exist in line " & (i + 2).ToString & " <br/>"
                                Next
                            End If

                            If String.IsNullOrEmpty(supplier) Then
                                supplier = "ALL"
                            Else
                                Dim supplierArr As String() = supplier.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)
                                For j = 0 To supplierArr.Length - 1
                                    dr = dt.Select("Type=12 and StorerKey='" & Trim(supplierArr(j)) & "'")
                                    If Trim(supplierArr(j)) > 500 Then tmp += "Supplier " & Trim(supplierArr(j)) & " cannot have more than 500 characters in line " & (i + 2).ToString & " <br/>"
                                    If dr.Length = 0 Then tmp += "Supplier " & Trim(supplierArr(j)) & " does not exist in line " & (i + 2).ToString & " <br/>"
                                Next
                            End If

                            If Not String.IsNullOrEmpty(facility) Then
                                Dim facilityArr As String() = facility.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)
                                For j = 0 To facilityArr.Length - 1
                                    If Trim(facilityArr(j)) > 50 Then tmp += "Facility " & Trim(facilityArr(j)) & " cannot have more than 50 characters in line " & (i + 2).ToString & " <br/>"
                                    If CommonMethods.getFacilityDBName(Trim(facilityArr(j))) = "" Then
                                        tmp += "Facility " & Trim(facilityArr(j)) & " does not exist in line " & (i + 2).ToString & " <br/>"
                                    End If
                                Next
                            End If

                            sql = " Select * from " & IIf(CommonMethods.dbtype <> "sql", "", "") & "USERCONTROL where UserKey = '" & userkey & "' "
                            sql += " Select * from " & IIf(CommonMethods.dbtype <> "sql", "", "") & "PORTALUSERS where UserKey = '" & userkey & "' and Active = 1 "
                            Dim ds As DataSet = (New SQLExec).Cursor(sql)

                            If ds.Tables(1).Rows.Count = 0 Then
                                tmp += "User " & userkey & " does not exist in line " & (i + 2).ToString & " <br/>"
                            Else
                                EditOperation = ds.Tables(0).Rows.Count > 0
                            End If

                            If tmp = "" Then
                                deletefacilityquery += " delete from " & IIf(CommonMethods.dbtype <> "sql", "System.", "") & "USERCONTROLFACILITY where USERKEY = '" & userkey & "' "
                                Dim Facilities As String() = facility.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)
                                For Each MyFacility As String In Facilities
                                    If CommonMethods.dbtype = "sql" Then
                                        insertfacilityquery += " set dateformat dmy insert into dbo.USERCONTROLFACILITY (USERKEY, FACILITY, ADDWHO, EDITWHO, ADDDATE,EDITDATE) values ('" & userkey & "', '" & CommonMethods.getFacilityDBName(Trim(MyFacility)) & "','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "','" & Now & "','" & Now & "'); "
                                    Else
                                        insertfacilityquery += " set dateformat dmy insert into SYSTEM.USERCONTROLFACILITY (USERKEY, FACILITY, ADDWHO, EDITWHO, ADDDATE,EDITDATE) values ('" & userkey & "', '" & CommonMethods.getFacilityDBName(Trim(MyFacility)) & "','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "',SYSDATE,SYSDATE');"
                                    End If
                                Next

                                If EditOperation Then
                                    If CommonMethods.dbtype = "sql" Then
                                        updatequery += " set dateformat dmy update dbo.USERCONTROL set STORERKEY='" & storer.Replace(";", ",").Replace(" ", "") & "', CONSIGNEEKEY='" & consignee.Replace(";", ",").Replace(" ", "") & "', SUPPLIERKEY ='" & supplier.Replace(";", ",").Replace(" ", "") & "', EXPORTROWSLIMIT = '" & exportrowslimit & "', FILEIMPORTLIMIT='" & fileimportlimit & "', FILEUPLOADLIMIT = '" & fileuploadlimit & "', EDITWHO= '" & HttpContext.Current.Session("userkey").ToString & "', EDITDATE = '" & Now & "' where UserKey='" & userkey & "'; "
                                    Else
                                        updatequery += " set dateformat dmy update SYSTEM.USERCONTROL set STORERKEY='" & storer.Replace(";", ",").Replace(" ", "") & "', CONSIGNEEKEY='" & consignee.Replace(";", ",").Replace(" ", "") & "', SUPPLIERKEY ='" & supplier.Replace(";", ",").Replace(" ", "") & "', EXPORTROWSLIMIT = '" & exportrowslimit & "', FILEIMPORTLIMIT='" & fileimportlimit & "', FILEUPLOADLIMIT = '" & fileuploadlimit & "', EDITWHO= '" & HttpContext.Current.Session("userkey").ToString & "', EDITDATE = SYSDATE where UserKey='" & userkey & "'; "
                                    End If
                                Else
                                    If CommonMethods.dbtype = "sql" Then
                                        insertquery += " set dateformat dmy insert into dbo.USERCONTROL (USERKEY,STORERKEY,CONSIGNEEKEY,SUPPLIERKEY, EXPORTROWSLIMIT, FILEIMPORTLIMIT, FILEUPLOADLIMIT, ADDWHO, EDITWHO, ADDDATE,EDITDATE) values ('" & userkey & "', '" & storer.Replace(";", ",").Replace(" ", "") & "', '" & consignee.Replace(";", ",").Replace(" ", "") & "', '" & supplier.Replace(";", ",").Replace(" ", "") & "','" & exportrowslimit & "','" & fileimportlimit & "', '" & fileuploadlimit & "','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "','" & Now & "','" & Now & "'); "
                                    Else
                                        insertquery += " set dateformat dmy insert into SYSTEM.USERCONTROL (USERKEY,STORERKEY,CONSIGNEEKEY,SUPPLIERKEY, EXPORTROWSLIMIT, FILEIMPORTLIMIT, FILEUPLOADLIMIT, ADDWHO, EDITWHO, ADDDATE,EDITDATE) values ('" & userkey & "', '" & storer.Replace(";", ",").Replace(" ", "") & "', '" & consignee.Replace(";", ",").Replace(" ", "") & "', '" & supplier.Replace(";", ",").Replace(" ", "") & "','" & exportrowslimit & "','" & fileimportlimit & "', '" & fileuploadlimit & "','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "',SYSDATE,SYSDATE); "
                                    End If
                                End If
                            End If
                        End With
                    Next

                    If tmp = "" Then
                        Try
                            If deletefacilityquery <> "" Then
                                tmp = (New SQLExec).Execute(deletefacilityquery & " " & insertfacilityquery)
                            End If

                            If tmp = "" Then
                                If insertquery <> "" And updatequery = "" Then
                                    tmp = (New SQLExec).Execute(insertquery)
                                ElseIf insertquery = "" And updatequery <> "" Then
                                    tmp = (New SQLExec).Execute(updatequery)
                                Else
                                    tmp = (New SQLExec).Execute(insertquery & " " & updatequery)
                                End If
                            End If
                        Catch ex As Exception
                            tmp += ex.Message
                        End Try
                    End If

                End If
            Else
                tmp = "No records To import"
            End If
        Else
            tmp = "No records To import"
        End If


        Return tmp
    End Function
    Private Function ImportProfile(ByVal ImportTable As DataTable) As String
        Dim tmp As String = "", insert As String = "", insertDetails As String = "", insertDetailsReport As String = "", insertDetailsDashboard As String = ""

        If ImportTable IsNot Nothing Then
            If ImportTable.Rows.Count > 0 Then
                Dim dups = From row In ImportTable.AsEnumerable()
                           Let userkey = row.Field(Of String)("ProfileName")
                           Group row By userkey Into DupUserKey = Group
                           Where DupUserKey.Count() > 1
                           Select DupUserKey

                If dups.Count > 0 Then
                    tmp = "Duplication values exist in ProfileName column of the import file"
                End If

                If tmp = "" Then
                    Dim ButtonsTable As DataTable = CommonMethods.getButtons("getAll")
                    Dim ReportsTable As DataTable = CommonMethods.getReports("getAll")
                    Dim DashboardsTable As DataTable = CommonMethods.getDashboards()
                    For i = 0 To ImportTable.Rows.Count - 1
                        With ImportTable.Rows(i)
                            Dim profilename As String = UCase(!ProfileName)

                            If String.IsNullOrEmpty(profilename) Then
                                tmp += "Profile Name must be defined in line " & (i + 2).ToString & "<br/>"
                            ElseIf profilename.Length > 50 Then
                                tmp += "Profile Name cannot have more than 50 characters " & (i + 2).ToString & "<br/>"
                            End If

                            If tmp = "" Then
                                Dim exist As Integer = CommonMethods.CheckNameExist(profilename)
                                If exist = 0 Then
                                    If CommonMethods.dbtype = "sql" Then
                                        insert += " set dateformat dmy insert into dbo.PROFILES (PROFILENAME, ADDWHO, EDITWHO, ADDDATE,EDITDATE) values ('" & profilename & "', '" & HttpContext.Current.Session("userkey").ToString & "', '" & HttpContext.Current.Session("userkey").ToString & "','" & Now & "', '" & Now & "'); "

                                        insertDetails += " set dateformat dmy insert into dbo.PROFILEDETAIL (PROFILENAME, SCREENBUTTONNAME, EDIT, READONLY, ADDWHO, EDITWHO, ADDDATE,EDITDATE, BLOCKED) values "
                                        For Each row As DataRow In ButtonsTable.Rows
                                            If row("BUTTON").ToString().Contains("Security->") Then
                                                insertDetails += "('" & profilename & "', '" & row("BUTTON").ToString() & "','0','1','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "','" & Now & "','" & Now & "', '" & row("BLOCKED").ToString & "'),"
                                            Else
                                                insertDetails += "('" & profilename & "','" & row("BUTTON").ToString() & "','1','0','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "','" & Now & "','" & Now & "', '" & row("BLOCKED").ToString & "'),"
                                            End If
                                        Next
                                        insertDetails = insertDetails.Remove(insertDetails.Length - 1)

                                        insertDetailsReport += " set dateformat dmy insert into dbo.REPORTSPROFILEDETAIL (PROFILENAME, REPORT,REPORT_NAME, EDIT,  ADDWHO, EDITWHO, ADDDATE,EDITDATE ) values "
                                        For Each row As DataRow In ReportsTable.Rows
                                            insertDetailsReport += "('" & profilename & "','" & row("RPT_ID").ToString() & "','" & row("RPT_TITLE").ToString() & "','1','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "','" & Now & "','" & Now & "'),"
                                        Next
                                        insertDetailsReport = insertDetailsReport.Remove(insertDetailsReport.Length - 1)

                                        insertDetailsDashboard += " set dateformat dmy insert into  dbo.PROFILEDETAILDASHBOARDS (PROFILENAME, DASHBOARD,DASHBOARD_NAME, EDIT,  ADDWHO, EDITWHO, ADDDATE,EDITDATE ) values "
                                        For Each row As DataRow In DashboardsTable.Rows
                                            insertDetailsDashboard += "('" & profilename & "','" & row("DashboardID").ToString() & "','" & row("DashboardName").ToString() & "','1','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "','" & Now & "','" & Now & "'),"
                                        Next
                                        insertDetailsDashboard = insertDetailsDashboard.Remove(insertDetailsDashboard.Length - 1)
                                    Else
                                        insert += " set dateformat dmy insert into SYSTEM.PROFILES (PROFILENAME, ADDWHO, EDITWHO, ADDDATE,EDITDATE) values ('" & profilename & "', '" & HttpContext.Current.Session("userkey").ToString & "', '" & HttpContext.Current.Session("userkey").ToString & "','SYSDATE','SYSDATE'); "

                                        insertDetails += " set dateformat dmy insert into  SYSTEM.PROFILEDETAIL (PROFILENAME, SCREENBUTTONNAME, EDIT, READONLY, ADDWHO, EDITWHO, ADDDATE,EDITDATE, BLOCKED) values "
                                        Dim count As Integer = ButtonsTable.Rows.Count, y As Integer = 0
                                        For Each row As DataRow In ButtonsTable.Rows
                                            If row("BUTTON").ToString().Contains("Security->") Then
                                                insertDetails += "('" & profilename & "', '" & row("BUTTON").ToString() & "','0','1','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "',SYSDATE, SYSDATE, '" & row("BLOCKED").ToString & "' from dual"
                                            Else
                                                insertDetails += "('" & profilename & "' ,'" & row("BUTTON").ToString() & "','1','0','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "',SYSDATE, SYSDATE, '" & row("BLOCKED").ToString & "' from dual"
                                            End If
                                            y = y + 1
                                            If y < count Then insertDetails += " union all "
                                        Next

                                        insertDetailsReport += " set dateformat dmy insert into SYSTEM.REPORTSPROFILEDETAIL (PROFILENAME, REPORT,REPORT_NAME, EDIT, ADDWHO, EDITWHO, ADDDATE,EDITDATE ) values "
                                        count = ReportsTable.Rows.Count
                                        y = 0
                                        For Each row As DataRow In ReportsTable.Rows
                                            insertDetailsReport += "('" & profilename & "','" & row("RPT_ID").ToString() & "','" & row("RPT_TITLE").ToString() & "','1','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "',SYSDATE, SYSDATE from dual"
                                            y = y + 1
                                            If y < count Then insertDetails += " union all "
                                        Next
                                    End If
                                Else
                                    tmp += "Error: Profile Name already exists in line " & (i + 2).ToString & " <br/>"
                                End If
                            End If
                        End With
                    Next

                    If tmp = "" Then
                        Try
                            tmp = (New SQLExec).Execute(insert & " " & insertDetails & " " & insertDetailsReport & " " & insertDetailsDashboard)
                        Catch ex As Exception
                            tmp += ex.Message
                        End Try
                    End If
                End If
            Else
                tmp = "No records to import"
            End If
        Else
            tmp = "No records to import"
        End If

        Return tmp
    End Function
    Private Function ImportUserProfile(ByVal ImportTable As DataTable) As String
        Dim tmp As String = "", insertquery As String = ""

        If ImportTable IsNot Nothing Then
            If ImportTable.Rows.Count > 0 Then
                Dim dups = From rows In ImportTable.AsEnumerable() _
                            .GroupBy(Function(r) New With {
                            Key .a = r("ProfileName"),
                            Key .b = r("UserKey")
                            }).Where(Function(gr) gr.Count() > 1).ToList()

                If dups.Count > 0 Then
                    tmp = "Duplication rows exist in the import file"
                End If

                If tmp = "" Then
                    Dim UsersTable As DataTable = CommonMethods.getUsers()
                    Dim ProfilesTable As DataTable = CommonMethods.getProfiles()
                    Dim UserProfileTable As DataTable = (New SQLExec).Cursor("Select * from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "USERPROFILE").tables(0)
                    For i = 0 To ImportTable.Rows.Count - 1
                        With ImportTable.Rows(i)
                            Dim profilename As String = UCase(!ProfileName) _
                               , userkey As String = !UserKey

                            If String.IsNullOrEmpty(profilename) Then
                                tmp += "Profile Name must be defined in line " & (i + 2).ToString & " <br/>"
                            ElseIf profilename.Length > 50 Then
                                tmp += "Profile Name cannot have more 50 characters in line " & (i + 2).ToString & " <br/>"
                            Else
                                If ProfilesTable.Select("ProfileName='" & profilename & "'").Length = 0 Then
                                    tmp += "Profile Name " & profilename & " does not exist in line " & (i + 2).ToString & " <br/>"
                                End If
                            End If

                            If String.IsNullOrEmpty(userkey) Then
                                tmp += "User ID Name must be defined in line " & (i + 2).ToString & " <br/>"
                            ElseIf userkey.Length > 50 Then
                                tmp += "User ID cannot have more 50 characters in line " & (i + 2).ToString & " <br/>"
                            Else
                                If UsersTable.Select("UserKey='" & userkey & "'").Length = 0 Then
                                    tmp += "User " & userkey & " does not exist in line " & (i + 2).ToString & " <br/>"
                                End If
                            End If

                            If tmp = "" Then
                                If UserProfileTable.Select("ProfileName='" & profilename & "' and UserKey ='" & userkey & "'").Length > 0 Then
                                    tmp += "Profile Name " & profilename & " is already assigned to User " & userkey & " in line " & (i + 2).ToString & " <br/>"
                                End If
                            End If

                            If tmp = "" Then
                                If CommonMethods.dbtype = "sql" Then
                                    insertquery += " set dateformat dmy insert into dbo.USERPROFILE (PROFILENAME, USERKEY, ADDWHO, EDITWHO, ADDDATE,EDITDATE) values ('" & profilename & "', '" & userkey & "','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "','" & Now & "','" & Now & "'); "
                                Else
                                    insertquery += " set dateformat dmy insert into SYSTEM.USERPROFILE (PROFILENAME, USERKEY, ADDWHO, EDITWHO, ADDDATE,EDITDATE) values ('" & profilename & "', '" & userkey & "','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "',SYSDATE,SYSDATE); "
                                End If
                            End If

                        End With
                    Next

                    If tmp = "" Then
                        Try
                            tmp = (New SQLExec).Execute(insertquery)
                        Catch ex As Exception
                            tmp += ex.Message
                        End Try
                    End If

                End If
            Else
                tmp = "No records to import"
            End If
        Else
            tmp = "No records to import"
        End If

        Return tmp
    End Function

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class