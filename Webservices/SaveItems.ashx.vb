Imports Newtonsoft.Json
Imports System.Data.SqlClient
Imports Oracle.ManagedDataAccess.Client
Imports NLog
Imports System.IO
Imports System.Globalization
Imports System.Xml

Public Class SaveItems
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub SaveItem(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim mySearchTable As String = HttpContext.Current.Request.Item("SearchTable")
        Dim MyID As String = HttpContext.Current.Request.Item("MyID")
        Dim DetailsCount As Integer = Val(HttpContext.Current.Request.Item("DetailsCount"))

        If MyID.Contains("?") Then
            MyID = GetMyID(mySearchTable, MyID)
        End If

        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)
        Using writer As JsonWriter = New JsonTextWriter(sw)
            writer.Formatting = Newtonsoft.Json.Formatting.Indented
            writer.WriteStartObject()

            writer.WritePropertyName("tmp")

            If mySearchTable = "PORTALUSERS" Then
                writer.WriteValue(SaveUser(MyID))
            ElseIf mySearchTable = "USERCONTROL" Then
                writer.WriteValue(SaveUserControl(MyID))
            ElseIf mySearchTable = "USERPROFILE" Then
                writer.WriteValue(SaveUserProfile())
            ElseIf mySearchTable = "PROFILES" Then
                writer.WriteValue(SaveProfile())
            ElseIf mySearchTable = "ChangePassword" Then
                writer.WriteValue(SaveNewPassword())
            ElseIf mySearchTable.Contains("enterprise.storer") Then
                Dim type As String = mySearchTable(mySearchTable.Length - 1)
                writer.WriteValue(SaveConfiguration(MyID, type))
            ElseIf mySearchTable = "enterprise.sku" Then
                writer.WriteValue(SaveItem(MyID))
            ElseIf mySearchTable = "SKUCATALOGUE" Then
                writer.WriteValue(SaveItemCatalogue(MyID))
            ElseIf mySearchTable = "Warehouse_PO" Then
                writer.WriteValue(SavePurchaseOrder(MyID, DetailsCount))
            ElseIf mySearchTable = "Warehouse_ASN" Then
                writer.WriteValue(SaveASN(MyID, DetailsCount))
            ElseIf mySearchTable = "Warehouse_SO" Then
                writer.WriteValue(SaveSO(MyID, DetailsCount))
            ElseIf mySearchTable = "Warehouse_OrderManagement" Then
                writer.WriteValue(SaveOrderManagement(MyID, DetailsCount))
            End If

            writer.WriteEndObject()
        End Using
        context.Response.Write(sw)
        context.Response.End()

    End Sub
    Private Function SaveNewPassword() As String
        Dim tmp As String = ""
        Dim IsValid As Boolean = True
        Dim OriginalPassword As String = HttpContext.Current.Request.Item("Field_OriginalPassword") _
        , NewPassword As String = HttpContext.Current.Request.Item("Field_NewPassword") _
        , ConfirmPassword As String = HttpContext.Current.Request.Item("Field_ConfirmPassword")

        If String.IsNullOrEmpty(OriginalPassword) Then
            IsValid = False
            tmp += "Original Password must be defined <br/>"
        End If

        If String.IsNullOrEmpty(NewPassword) Then
            IsValid = False
            tmp += "New Password must be defined <br/>"
        Else
            Dim checkcomplex As Boolean = CommonMethods.checkPassComplexity(NewPassword)
            If Not checkcomplex Then
                IsValid = False
                If NewPassword.Length < 10 Then
                    tmp += "New Password must have at least 10 characters <br/>"
                Else
                    tmp += "New Password must have one upper case letter, one lower case letter and one base 10 digits (0 to 9) <br/>"
                End If
            End If
        End If

        If Not String.IsNullOrEmpty(ConfirmPassword) Then
            If ConfirmPassword <> NewPassword Then
                IsValid = False
                tmp += "New Password and Confirm Password do not match! <br/>"
            End If
        ElseIf Not String.IsNullOrEmpty(NewPassword) Then
            IsValid = False
            tmp += "Confirm Password must be filled <br/>"
        End If

        If IsValid Then
            Dim exist As Integer = 0
            Dim userkey As String = HttpContext.Current.Session("userkey").ToString
            Dim keypass As String = CommonMethods.getpasskey(userkey)
            Dim sql As String = "select * from " & IIf(CommonMethods.dbtype <> "sql", "System.", "") & " PORTALUSERS where USERKEY = '" & userkey & "' and ACTIVE=1 and PASSWORD = '" & CommonMethods.GenerateHash(OriginalPassword, keypass) & "'"
            Dim ds As DataSet = (New SQLExec).Cursor(sql)
            If ds.Tables(0).Rows.Count = 0 Then
                tmp += "Original password is not valid"
            Else
                Dim keyh As String = CommonMethods.CreateSalt(NewPassword.Length)
                If CommonMethods.dbtype = "sql" Then
                    Dim update As String = "set dateformat dmy update dbo.PORTALUSERS set EDITWHO= @edit , EDITDATE= @editdate , PASSWORDDATE= @passd ,PASSWORD= @passw, HASHKEY=@keyh where  USERKEY= @ukey ;"
                    Try
                        Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                        conn.Open()
                        Dim cmd As SqlCommand = New SqlCommand(update, conn)
                        cmd.Parameters.AddWithValue("@edit", userkey)
                        cmd.Parameters.AddWithValue("@editdate", Now)
                        cmd.Parameters.AddWithValue("@passd", Now)
                        cmd.Parameters.AddWithValue("@passw", CommonMethods.GenerateHash(NewPassword, keyh))
                        cmd.Parameters.AddWithValue("@keyh", keyh)
                        cmd.Parameters.AddWithValue("@ukey", userkey)
                        cmd.ExecuteNonQuery()
                        conn.Close()
                    Catch e1 As Exception
                        tmp += "Error: " & e1.Message & vbTab + e1.GetType.ToString
                        Dim logger As Logger = LogManager.GetCurrentClassLogger()
                        logger.Error(e1, "", "")
                    End Try
                Else
                    Dim update As String = "set dateformat dmy update SYSTEM.PORTALUSERS set EDITWHO= :Userk , EDITDATE=SYSDATE,PASSWORDDATE=SYSDATE, PASSWORD= :passw, HASHKEY=:keyh where USERKEY= :Userky"
                    Try
                        Dim conn As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                        conn.Open()
                        Dim cmd As OracleCommand = New OracleCommand(update, conn)
                        cmd.Parameters.Add(New OracleParameter("Userk", userkey))
                        cmd.Parameters.Add(New OracleParameter("passw", CommonMethods.GenerateHash(NewPassword, keyh)))
                        cmd.Parameters.Add(New OracleParameter("keyh", keyh))
                        cmd.Parameters.Add(New OracleParameter("Userky", userkey))
                        cmd.ExecuteNonQuery()
                        conn.Close()
                    Catch e1 As Exception
                        tmp += "Error: " & e1.Message & vbTab + e1.GetType.ToString
                        Dim logger As Logger = LogManager.GetCurrentClassLogger()
                        logger.Error(e1, "", "")
                    End Try
                End If
            End If
        End If

        Return tmp
    End Function
    Private Function SaveUser(ByVal MyID As Integer) As String
        Dim tmp As String = ""
        Dim IsValid As Boolean = True
        Dim EditOperation As Boolean = MyID <> 0
        Dim firstname As String = HttpContext.Current.Request.Item("Field_FirstName") _
        , lastname As String = HttpContext.Current.Request.Item("Field_LastName") _
        , userkey As String = HttpContext.Current.Request.Item("Field_UserKey") _
        , email As String = HttpContext.Current.Request.Item("Field_Email") _
        , password As String = HttpContext.Current.Request.Item("Field_Password") _
        , confirm As String = HttpContext.Current.Request.Item("Field_ConfirmPassword") _
        , active As String = HttpContext.Current.Request.Item("Field_Active") _
        , originalpass As String = "", keypass As String = ""

        If String.IsNullOrEmpty(userkey) Then
            IsValid = False
            tmp += "User ID must be defined <br/>"
        End If

        If String.IsNullOrEmpty(firstname) Then
            IsValid = False
            tmp += "First Name must be defined <br/>"
        End If

        If String.IsNullOrEmpty(lastname) Then
            IsValid = False
            tmp += "Last Name must be defined <br/>"
        End If

        If String.IsNullOrEmpty(email) Then
            IsValid = False
            tmp += "Email must be defined <br/>"
        End If

        If Not EditOperation Then
            If String.IsNullOrEmpty(password) Then
                IsValid = False
                tmp += "Password must be defined <br/>"
            Else
                Dim checkcomplex As Boolean = CommonMethods.checkPassComplexity(password)
                If Not checkcomplex Then
                    IsValid = False
                    If password.Length < 10 Then
                        tmp += "Password must have at least 10 characters <br/>"
                    Else
                        tmp += "Password must have one upper case letter, one lower case letter and one base 10 digits (0 to 9) <br/>"
                    End If
                End If
            End If

            If Not String.IsNullOrEmpty(confirm) Then
                If confirm <> password Then
                    IsValid = False
                    tmp += "Password and Confirm Password do not match! <br/>"
                End If
            ElseIf Not String.IsNullOrEmpty(password) Then
                IsValid = False
                tmp += "Confirm Password must be filled <br/>"
            End If
        Else
            originalpass = CommonMethods.getPassword(userkey)
            keypass = CommonMethods.getpasskey(userkey)
            If String.IsNullOrEmpty(password) Then
                IsValid = False
                tmp += "Password must be defined <br/>"
            ElseIf originalpass <> password Then
                Dim checkcomplex As Boolean = CommonMethods.checkPassComplexity(password)
                If confirm <> password Then
                    IsValid = False
                    tmp += "Confirm Password must be filled <br/>"
                ElseIf Not checkcomplex Then
                    IsValid = False
                    If password.Length < 10 Then
                        tmp += "Password must have at least 10 characters <br/>"
                    Else
                        tmp += "Password must have one upper case letter, one lower case letter and one base 10 digits (0 to 9) <br/>"
                    End If
                Else
                    keypass = CommonMethods.CreateSalt(password.Length)
                    originalpass = CommonMethods.GenerateHash(password, keypass)
                End If
            End If
        End If

        If IsValid Then
            If EditOperation Then
                Try
                    If CommonMethods.dbtype = "sql" Then
                        Dim updatequery As String = "set dateformat dmy update dbo.PORTALUSERS set ACTIVE= @flag , Password= @passw , Email = @email , EDITWHO= @ukey , EDITDATE='" & Now & "', HASHKEY= @hkey where USERKEY = @userk"
                        Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                        conn.Open()
                        Dim cmd As SqlCommand = New SqlCommand(updatequery, conn)
                        cmd.Parameters.AddWithValue("@flag", active)
                        cmd.Parameters.AddWithValue("@passw", originalpass)
                        cmd.Parameters.AddWithValue("@email", email)
                        cmd.Parameters.AddWithValue("@ukey", HttpContext.Current.Session("userkey").ToString)
                        cmd.Parameters.AddWithValue("@hkey", keypass)
                        cmd.Parameters.AddWithValue("@userk", userkey)
                        cmd.ExecuteNonQuery()
                        conn.Close()
                    Else
                        Dim updatequery As String = "update SYSTEM.PORTALUSERS set ACTIVE= :flag , Password= :passw , Email = :email, EDITWHO=:ukey , EDITDATE=SYSDATE, HASHKEY=:hkey where   USERKEY = :userk"
                        Dim conn As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                        conn.Open()
                        Dim cmd As OracleCommand = New OracleCommand(updatequery, conn)
                        cmd.Parameters.Add(New OracleParameter("flag", active))
                        cmd.Parameters.Add(New OracleParameter("passw", originalpass))
                        cmd.Parameters.Add(New OracleParameter("email", email))
                        cmd.Parameters.Add(New OracleParameter("ukey", HttpContext.Current.Session("userkey").ToString))
                        cmd.Parameters.Add(New OracleParameter("hkey", keypass))
                        cmd.Parameters.Add(New OracleParameter("userk", userkey))
                        cmd.ExecuteNonQuery()
                        conn.Close()
                    End If
                Catch e1 As Exception
                    tmp += "Error: " & e1.Message & vbTab + e1.GetType.ToString & "<br/>"
                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                    logger.Error(e1, "", "")
                End Try
            Else
                Dim exist As Integer = CommonMethods.checkUserExist(LCase(userkey))
                If exist = 0 Then
                    Dim keyh As String = CommonMethods.CreateSalt(password.Length)
                    Try
                        If CommonMethods.dbtype = "sql" Then
                            Dim insert As String = "set dateformat dmy insert into  dbo.PORTALUSERS (ACTIVE, USERKEY, FIRSTNAME, LASTNAME, EMAIL, PASSWORD, ADDWHO, EDITWHO , ADDDATE,EDITDATE , HASHKEY) values ( @act, @ukey, @fname, @lname, @email, @passw, '" & HttpContext.Current.Session("userkey").ToString & "',  '" & HttpContext.Current.Session("userkey").ToString & "', '" & Now & "', '" & Now & "', @keyh);"
                            Dim widgets As String = "set dateformat dmy insert into dbo.USERWIDGETS (WIDGETID,USERKEY, ADDDATE) values (1,'" & LCase(userkey) & "','" & Now & "'),(4,'" & LCase(userkey) & "','" & Now & "'),(6,'" & LCase(userkey) & "','" & Now & "'),(7,'" & LCase(userkey) & "','" & Now & "'); "
                            Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                            conn.Open()

                            Dim cmd As SqlCommand = New SqlCommand(insert, conn)
                            cmd.Parameters.AddWithValue("@act", active)
                            cmd.Parameters.AddWithValue("@ukey", LCase(userkey))
                            cmd.Parameters.AddWithValue("@fname", firstname)
                            cmd.Parameters.AddWithValue("@lname", lastname)
                            cmd.Parameters.AddWithValue("@email", email)
                            cmd.Parameters.AddWithValue("@passw", CommonMethods.GenerateHash(password, keyh))
                            cmd.Parameters.AddWithValue("@keyh", keyh)
                            cmd.ExecuteNonQuery()

                            conn.Close()
                        Else
                            Dim insert As String = "insert into  SYSTEM.PORTALUSERS (ACTIVE, USERKEY, FIRSTNAME, LASTNAME, EMAIL, PASSWORD, ADDWHO, EDITWHO , ADDDATE,EDITDATE  , HASHKEY) values( :act, :ukey, :fname, :lname, :email, :passw, '" & HttpContext.Current.Session("userkey").ToString & "',  '" & HttpContext.Current.Session("userkey").ToString & "', SYSDATE,SYSDATE, :keyh )"
                            Dim widgets As String = "insert into SYSTEM.USERWIDGETS (WIDGETID,USERKEY, ADDDATE) values (1,'" & LCase(userkey) & "','" & Now & "'),(4,'" & LCase(userkey) & "','" & Now & "'),(6,'" & LCase(userkey) & "','" & Now & "'),(7,'" & LCase(userkey) & "','" & Now & "'); "
                            Dim conn As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                            conn.Open()

                            Dim cmd As OracleCommand = New OracleCommand(insert, conn)
                            cmd.Parameters.Add(New OracleParameter("act", active))
                            cmd.Parameters.Add(New OracleParameter("ukey", LCase(userkey)))
                            cmd.Parameters.Add(New OracleParameter("fname", firstname))
                            cmd.Parameters.Add(New OracleParameter("lname", lastname))
                            cmd.Parameters.Add(New OracleParameter("email", email))
                            cmd.Parameters.Add(New OracleParameter("passw", CommonMethods.GenerateHash(password, keyh)))
                            cmd.Parameters.Add(New OracleParameter("keyh", keyh))
                            cmd.ExecuteNonQuery()

                            conn.Close()
                        End If
                    Catch e2 As Exception
                        tmp += "Error: " & e2.Message & vbTab + e2.GetType.ToString & "<br/>"
                        Dim logger As Logger = LogManager.GetCurrentClassLogger()
                        logger.Error(e2, "", "")
                    End Try
                Else
                    tmp += "Error: User ID is already assigned to a different user <br/>"
                End If
            End If
        End If
        Return tmp
    End Function
    Private Function SaveUserControl(ByVal MyID As Integer) As String
        Dim tmp As String = ""
        Dim IsValid As Boolean = True
        Dim EditOperation As Boolean = MyID <> 0
        Dim userkey As String = HttpContext.Current.Request.Item("Field_UserKey") _
        , storer As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
        , consignee As String = UCase(HttpContext.Current.Request.Item("Field_ConsigneeKey")) _
        , supplier As String = UCase(HttpContext.Current.Request.Item("Field_SupplierKey")) _
        , facility As String = HttpContext.Current.Request.Item("Field_Facility")

        If String.IsNullOrEmpty(userkey) Then
            IsValid = False
            tmp += "User ID must be defined <br/>"
        End If

        If storer = "" Then storer = "ALL"
        If consignee = "" Then consignee = "ALL"
        If supplier = "" Then supplier = "ALL"

        If IsValid Then
            Dim tmp2 As String = (New SQLExec).Execute("delete from " & IIf(CommonMethods.dbtype <> "sql", "System.", "") & "USERCONTROLFACILITY where USERKEY = '" & userkey & "'")
            If tmp2 = "" Then
                Dim Facilities As String() = facility.Split(New Char() {","c})
                For Each MyFacility As String In Facilities
                    Try
                        If CommonMethods.dbtype = "sql" Then
                            Dim insert As String = "set dateformat dmy insert into dbo.USERCONTROLFACILITY (USERKEY, FACILITY, ADDWHO, EDITWHO, ADDDATE,EDITDATE) values (@ukey, @facility,'" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "','" & Now & "','" & Now & "');"

                            Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                            conn.Open()

                            Dim cmd As SqlCommand = New SqlCommand(insert, conn)
                            cmd.Parameters.AddWithValue("@ukey", userkey)
                            cmd.Parameters.AddWithValue("@facility", CommonMethods.getFacilityDBName(MyFacility))
                            cmd.ExecuteNonQuery()

                            conn.Close()
                        Else
                            Dim insert As String = "set dateformat dmy insert into SYSTEM.USERCONTROLFACILITY (USERKEY, FACILITY, ADDWHO, EDITWHO, ADDDATE,EDITDATE) values (:ukey, :facility,'" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "',SYSDATE,SYSDATE');"
                            Dim conn As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                            conn.Open()

                            Dim cmd As OracleCommand = New OracleCommand(insert, conn)
                            cmd.Parameters.Add(New OracleParameter("ukey", userkey))
                            cmd.Parameters.Add(New OracleParameter("facility", CommonMethods.getFacilityDBName(MyFacility)))
                            cmd.ExecuteNonQuery()

                            conn.Close()
                        End If
                    Catch e2 As Exception
                        tmp += "Error: " & e2.Message & vbTab + e2.GetType.ToString & "<br/>"
                        Dim logger As Logger = LogManager.GetCurrentClassLogger()
                        logger.Error(e2, "", "")
                    End Try
                Next
            Else
                Return tmp2
            End If

            If EditOperation Then
                Try
                    If CommonMethods.dbtype = "sql" Then
                        Dim updatequery As String = "set dateformat dmy update dbo.USERCONTROL set STORERKEY=@storer , CONSIGNEEKEY=@consignee, SUPPLIERKEY =@supplier, EDITWHO= '" & HttpContext.Current.Session("userkey").ToString & "', EDITDATE = '" & Now & "' where ID=@id"
                        Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                        conn.Open()
                        Dim cmd As SqlCommand = New SqlCommand(updatequery, conn)
                        cmd.Parameters.AddWithValue("@storer", storer)
                        cmd.Parameters.AddWithValue("@consignee", consignee)
                        cmd.Parameters.AddWithValue("@supplier", supplier)
                        cmd.Parameters.AddWithValue("@id", MyID)
                        cmd.ExecuteNonQuery()
                        conn.Close()
                    Else
                        Dim updatequery As String = "set dateformat dmy update SYSTEM.USERCONTROL set STORERKEY=:storer , CONSIGNEEKEY=:consignee, SUPPLIERKEY =:supplier, EDITWHO= '" & HttpContext.Current.Session("userkey").ToString & "', EDITDATE = SYSDATE where SERIALKEY=:serialkey"
                        Dim conn As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                        conn.Open()
                        Dim cmd As OracleCommand = New OracleCommand(updatequery, conn)
                        cmd.Parameters.Add(New OracleParameter("storer", storer))
                        cmd.Parameters.Add(New OracleParameter("consignee", consignee))
                        cmd.Parameters.Add(New OracleParameter("supplier", supplier))
                        cmd.Parameters.Add(New OracleParameter("serialkey", MyID))
                        cmd.ExecuteNonQuery()
                        conn.Close()
                    End If
                Catch e1 As Exception
                    tmp += "Error: " & e1.Message & vbTab + e1.GetType.ToString & "<br/>"
                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                    logger.Error(e1, "", "")
                End Try
            Else
                Dim exist As Integer = CommonMethods.checkUserControlExist(userkey)
                If exist = 0 Then
                    Try
                        If CommonMethods.dbtype = "sql" Then
                            Dim insert As String = "set dateformat dmy insert into dbo.USERCONTROL (USERKEY,STORERKEY,CONSIGNEEKEY,SUPPLIERKEY, ADDWHO, EDITWHO, ADDDATE,EDITDATE) values (@ukey, @storer, @consignee, @supplier ,'" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "','" & Now & "','" & Now & "');"

                            Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                            conn.Open()

                            Dim cmd As SqlCommand = New SqlCommand(insert, conn)
                            cmd.Parameters.AddWithValue("@ukey", userkey)
                            cmd.Parameters.AddWithValue("@storer", storer)
                            cmd.Parameters.AddWithValue("@consignee", consignee)
                            cmd.Parameters.AddWithValue("@supplier", supplier)
                            cmd.ExecuteNonQuery()

                            conn.Close()
                        Else
                            Dim insert As String = "set dateformat dmy insert into SYSTEM.USERCONTROL (USERKEY,STORERKEY,CONSIGNEEKEY,SUPPLIERKEY, ADDWHO, EDITWHO, ADDDATE,EDITDATE) values (:ukey, :storer, :consignee, :supplier ,'" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "',SYSDATE,SYSDATE');"
                            Dim conn As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                            conn.Open()

                            Dim cmd As OracleCommand = New OracleCommand(insert, conn)
                            cmd.Parameters.Add(New OracleParameter("ukey", userkey))
                            cmd.Parameters.Add(New OracleParameter("storer", storer))
                            cmd.Parameters.Add(New OracleParameter("consignee", consignee))
                            cmd.Parameters.Add(New OracleParameter("supplier", supplier))
                            cmd.ExecuteNonQuery()

                            conn.Close()
                        End If
                    Catch e2 As Exception
                        tmp += "Error: " & e2.Message & vbTab + e2.GetType.ToString & "<br/>"
                        Dim logger As Logger = LogManager.GetCurrentClassLogger()
                        logger.Error(e2, "", "")
                    End Try
                Else
                    tmp += "Error: Record already defined for this User ID <br/>"
                End If
            End If
        End If
        Return tmp
    End Function
    Private Function SaveUserProfile() As String
        Dim tmp As String = ""
        Dim IsValid As Boolean = True
        Dim profilename As String = UCase(HttpContext.Current.Request.Item("Field_ProfileName")) _
        , userkey As String = HttpContext.Current.Request.Item("Field_UserKey")

        If String.IsNullOrEmpty(profilename) Then
            IsValid = False
            tmp += "Profile Name must be defined <br/>"
        End If

        If String.IsNullOrEmpty(userkey) Then
            IsValid = False
            tmp += "User ID must be defined <br/>"
        End If

        If IsValid Then
            Dim columns As String = "PROFILENAME, USERKEY, ADDWHO, EDITWHO, ADDDATE,EDITDATE"
            Dim Command As String = ""
            Try
                If CommonMethods.dbtype = "sql" Then
                    Command = "@pname, @ukey,'" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "','" & Now & "','" & Now & "'"
                    Dim insert As String = "set dateformat dmy insert into  dbo.USERPROFILE (" & columns & " )values( " & Command & ");"
                    Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                    conn.Open()
                    Dim cmd As SqlCommand = New SqlCommand(insert, conn)
                    cmd.Parameters.AddWithValue("@pname", profilename)
                    cmd.Parameters.AddWithValue("@ukey", userkey)
                    cmd.ExecuteNonQuery()
                    conn.Close()
                Else
                    Command = ":pname, :ukey ,'" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "',SYSDATE,SYSDATE"
                    Dim insert As String = "set dateformat dmy insert into SYSTEM.USERPROFILE (" & columns & " )values( " & Command & ")"
                    Dim conn As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                    conn.Open()
                    Dim cmd As OracleCommand = New OracleCommand(insert, conn)
                    cmd.Parameters.Add(New OracleParameter("pname", profilename))
                    cmd.Parameters.Add(New OracleParameter("ukey", userkey))
                    cmd.ExecuteNonQuery()
                    conn.Close()
                End If
            Catch e1 As Exception
                If LCase(e1.Message).Contains("combination_index") Or LCase(e1.Message).Contains("unique constraint") Then
                    tmp += "Error: Record already exists for this Profile/User ID" & "<br/>"
                Else
                    tmp += "Error: " & e1.Message & vbTab + e1.GetType.ToString & "<br/>"
                End If
                Dim logger As Logger = LogManager.GetCurrentClassLogger()
                logger.Error(e1, "", "")
            End Try
        End If
        Return tmp
    End Function
    Private Function SaveProfile() As String
        Dim tmp As String = ""
        Dim IsValid As Boolean = True
        Dim profilename As String = UCase(HttpContext.Current.Request.Item("Field_ProfileName"))

        If String.IsNullOrEmpty(profilename) Then
            IsValid = False
            tmp += "Profile Name must be defined <br/>"
        End If

        If IsValid Then
            Dim Command As String = ""
            Dim exist As Integer = CommonMethods.CheckNameExist(profilename)
            If exist = 0 Then
                Try
                    If CommonMethods.dbtype = "sql" Then
                        Command = "@pname,'" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "','" & Now & "','" & Now & "'"
                        Dim insert As String = "set dateformat dmy insert into  dbo.PROFILES (PROFILENAME, ADDWHO, EDITWHO, ADDDATE,EDITDATE) values ( " & Command & ");"
                        Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                        conn.Open()
                        Dim cmd As SqlCommand = New SqlCommand(insert, conn)
                        cmd.Parameters.AddWithValue("@pname", profilename)
                        cmd.ExecuteNonQuery()

                        Dim insertDetails As String = "set dateformat dmy insert into  dbo.PROFILEDETAIL (PROFILENAME, SCREENBUTTONNAME, EDIT, READONLY, ADDWHO, EDITWHO, ADDDATE,EDITDATE) values "
                        Dim ButtonsTable As DataTable = CommonMethods.getButtons("getAll")
                        For Each row As DataRow In ButtonsTable.Rows
                            If row("BUTTON").ToString().Contains("Security->") Then
                                insertDetails += "( @pname, '" & row("BUTTON").ToString() & "','0','1','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "','" & Now & "','" & Now & "'),"
                            Else
                                insertDetails += "( @pname ,'" & row("BUTTON").ToString() & "','1','0','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "','" & Now & "','" & Now & "'),"
                            End If
                        Next
                        insertDetails = insertDetails.Remove(insertDetails.Length - 1)
                        Dim cmd2 As SqlCommand = New SqlCommand(insertDetails, conn)
                        cmd2.Parameters.AddWithValue("@pname", profilename)
                        cmd2.ExecuteNonQuery()

                        Dim insertdetailsReport As String = " set dateformat dmy insert into  dbo.PROFILEDETAILREPORTS (PROFILENAME, REPORT,REPORT_NAME, EDIT,  ADDWHO, EDITWHO, ADDDATE,EDITDATE ) values "
                        Dim ReportsTable As DataTable = CommonMethods.getReports("getAll")
                        For Each row As DataRow In ReportsTable.Rows
                            insertdetailsReport += "( @pname,'" & row("RPT_ID").ToString() & "','" & row("RPT_TITLE").ToString() & "','1','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "','" & Now & "','" & Now & "'),"
                        Next
                        insertdetailsReport = insertdetailsReport.Remove(insertdetailsReport.Length - 1)
                        Dim cmd3 As SqlCommand = New SqlCommand(insertdetailsReport, conn)
                        cmd3.Parameters.AddWithValue("@pname", profilename)
                        cmd3.ExecuteNonQuery()

                        Dim insertDetailsDashBorad As String = " set dateformat dmy insert into  dbo.PROFILEDETAILDASHBOARDS (PROFILENAME, DASHBOARD,DASHBOARD_NAME, EDIT,  ADDWHO, EDITWHO, ADDDATE,EDITDATE ) values "
                        Dim DashboardsTable As DataTable = CommonMethods.getDashboards()
                        For Each row As DataRow In DashboardsTable.Rows
                            insertDetailsDashBorad += "( @pname,'" & row("DashboardID").ToString() & "','" & row("DashboardName").ToString() & "','1','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "','" & Now & "','" & Now & "'),"
                        Next
                        insertDetailsDashBorad = insertDetailsDashBorad.Remove(insertDetailsDashBorad.Length - 1)
                        Dim cmd4 As SqlCommand = New SqlCommand(insertDetailsDashBorad, conn)
                        cmd4.Parameters.AddWithValue("@pname", profilename)
                        cmd4.ExecuteNonQuery()

                        conn.Close()
                    Else
                        Command = ":pname, :ukey ,'" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "',SYSDATE,SYSDATE"
                        Dim insert As String = "set dateformat dmy insert into SYSTEM.PROFILES (PROFILENAME, ADDWHO, EDITWHO, ADDDATE,EDITDATE) values ( " & Command & ")"
                        Dim conn As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                        conn.Open()
                        Dim cmd As OracleCommand = New OracleCommand(insert, conn)
                        cmd.Parameters.Add(New OracleParameter("pname", profilename))
                        cmd.ExecuteNonQuery()

                        Dim insertDetails As String = "set dateformat dmy insert into  SYSTEM.PROFILEDETAIL (PROFILENAME, SCREENBUTTONNAME, EDIT, READONLY, ADDWHO, EDITWHO, ADDDATE,EDITDATE) values "
                        Dim ButtonsTable As DataTable = CommonMethods.getButtons("getAll")
                        Dim count As Integer = ButtonsTable.Rows.Count, y As Integer = 0
                        For Each row As DataRow In ButtonsTable.Rows
                            If row("BUTTON").ToString().Contains("Security->") Then
                                insertDetails += "( :pname, '" & row("BUTTON").ToString() & "','0','1','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "',SYSDATE, SYSDATE from dual"
                            Else
                                insertDetails += "( :pname ,'" & row("BUTTON").ToString() & "','1','0','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "',SYSDATE, SYSDATE from dual"
                            End If
                            y = y + 1
                            If y < count Then insertDetails += " union all "
                        Next
                        Dim cmd2 As OracleCommand = New OracleCommand(insertDetails, conn)
                        cmd2.Parameters.Add(New OracleParameter("pname", profilename))
                        cmd2.ExecuteNonQuery()

                        Dim insertdetailsReport As String = "set dateformat dmy insert into  SYSTEM.PROFILEDETAILREPORTS (PROFILENAME, REPORT,REPORT_NAME, EDIT,  ADDWHO, EDITWHO, ADDDATE,EDITDATE ) values "
                        Dim ReportsTable As DataTable = CommonMethods.getReports("getAll")
                        count = ReportsTable.Rows.Count
                        y = 0

                        For Each row As DataRow In ReportsTable.Rows
                            insertdetailsReport += "( :pname,'" & row("RPT_ID").ToString() & "','" & row("RPT_TITLE").ToString() & "','1','" & HttpContext.Current.Session("userkey").ToString & "','" & HttpContext.Current.Session("userkey").ToString & "',SYSDATE, SYSDATE from dual"
                            y = y + 1
                            If y < count Then insertDetails += " union all "
                        Next
                        Dim cmd3 As OracleCommand = New OracleCommand(insertdetailsReport, conn)
                        cmd3.Parameters.Add(New OracleParameter("pname", profilename))
                        cmd3.ExecuteNonQuery()

                        conn.Close()
                    End If
                Catch e1 As Exception
                    If LCase(e1.Message).Contains("duplicate") Then
                        tmp += "Error: Cannot create duplicate profile name"
                    Else
                        tmp += "Error: " & e1.Message & vbTab + e1.GetType.ToString
                    End If
                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                    logger.Error(e1, "", "")
                End Try
            Else
                tmp = "Error: Profile Name already exists!"
            End If
        End If
        Return tmp
    End Function
    Private Function SaveConfiguration(ByVal MyID As Integer, ByVal type As String) As String
        Dim tmp As String = "", Command As String = ""
        Dim IsValid As Boolean = True
        Dim EditOperation As Boolean = MyID <> 0
        Dim r As Regex = New Regex("[~`!#$%^&*()+=|\{}':;,<>/?[\]""]")
        Dim storer As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
        , company As String = HttpContext.Current.Request.Item("Field_Company") _
        , descr As String = HttpContext.Current.Request.Item("Field_Description") _
        , country As String = HttpContext.Current.Request.Item("Field_Country") _
        , city As String = HttpContext.Current.Request.Item("Field_City") _
        , state As String = HttpContext.Current.Request.Item("Field_State") _
        , zip As String = HttpContext.Current.Request.Item("Field_Zip") _
        , Address1 As String = HttpContext.Current.Request.Item("Field_Address1") _
        , Address2 As String = HttpContext.Current.Request.Item("Field_Address2") _
        , Address3 As String = HttpContext.Current.Request.Item("Field_Address3") _
        , Address4 As String = HttpContext.Current.Request.Item("Field_Address4") _
        , Address5 As String = HttpContext.Current.Request.Item("Field_Address5") _
        , Address6 As String = HttpContext.Current.Request.Item("Field_Address6") _
        , Contact1 As String = HttpContext.Current.Request.Item("Field_Contact1") _
        , Contact2 As String = HttpContext.Current.Request.Item("Field_Contact2") _
        , Email1 As String = HttpContext.Current.Request.Item("Field_Email1") _
        , Email2 As String = HttpContext.Current.Request.Item("Field_Email2") _
        , Phone1 As String = HttpContext.Current.Request.Item("Field_Phone1") _
        , Phone2 As String = HttpContext.Current.Request.Item("Field_Phone2") _
        , UDf1 As String = HttpContext.Current.Request.Item("Field_SUsr1") _
        , UDf2 As String = HttpContext.Current.Request.Item("Field_SUsr2") _
        , UDf3 As String = HttpContext.Current.Request.Item("Field_SUsr3") _
        , UDf4 As String = HttpContext.Current.Request.Item("Field_SUsr4") _
        , UDf5 As String = HttpContext.Current.Request.Item("Field_SUsr5") _
        , isoCode As String = ""

        If Not String.IsNullOrEmpty(storer) And Trim(storer) <> "" Then
            If r.IsMatch(storer) Then
                IsValid = False
                tmp += IIf(type = "2", "Ship To", "Supplier") & " cannot have any special characters <br/>"
            Else
                Command += "<StorerKey>" & storer & "</StorerKey><Type>" & type & "</Type>"
            End If
        Else
            IsValid = False
            tmp += IIf(type = "2", "Ship To", "Supplier") & " cannot be Empty <br/>"
        End If

        If Not String.IsNullOrEmpty(company) Then
            If r.IsMatch(company) Then
                IsValid = False
                tmp += "Company cannot have any special characters <br/>"
            Else
                Command += "<Company>" & company & "</Company>"
            End If
        End If

        If Not String.IsNullOrEmpty(descr) Then
            If r.IsMatch(descr) Then
                IsValid = False
                tmp += "Description cannot have any special characters <br/>"
            Else
                Command += "<Description>" & descr & "</Description>"
            End If
        End If

        If Not String.IsNullOrEmpty(country) Then
            Command += "<Country>" & country & "</Country>"
            isoCode = CommonMethods.getISOCountryCode(country)
            If Not String.IsNullOrEmpty(isoCode) Then
                Command += "<ISOCntryCode>" & isoCode & "</ISOCntryCode>"
            End If
        End If

        If Not String.IsNullOrEmpty(city) Then
            If r.IsMatch(city) Then
                IsValid = False
                tmp += "City cannot have any special characters <br/>"
            Else
                Command += "<City>" & city & "</City>"
            End If
        End If

        If Not String.IsNullOrEmpty(state) Then
            If r.IsMatch(state) Then
                IsValid = False
                tmp += "State cannot have any special characters <br/>"
            Else
                Command += "<State>" & state & "</State>"
            End If
        End If

        If Not String.IsNullOrEmpty(zip) Then
            If r.IsMatch(zip) Then
                IsValid = False
                tmp += "Zip Code cannot have any special characters <br/>"
            Else
                Command += "<Zip>" & zip & "</Zip>"
            End If
        End If

        If Not String.IsNullOrEmpty(Address1) Then
            If r.IsMatch(Address1) Then
                IsValid = False
                tmp += "Address1 cannot have any special characters <br/>"
            Else
                Command += "<Address1>" & Address1 & "</Address1>"
            End If
        End If

        If Not String.IsNullOrEmpty(Address2) Then
            If r.IsMatch(Address2) Then
                IsValid = False
                tmp += "Address2 cannot have any special characters <br/>"
            Else
                Command += "<Address2>" & Address2 & "</Address2>"
            End If
        End If

        If Not String.IsNullOrEmpty(Address3) Then
            If r.IsMatch(Address3) Then
                IsValid = False
                tmp += "Address3 cannot have any special characters <br/>"
            Else
                Command += "<Address3>" & Address3 & "</Address3>"
            End If
        End If

        If Not String.IsNullOrEmpty(Address4) Then
            If r.IsMatch(Address4) Then
                IsValid = False
                tmp += "Address4 cannot have any special characters <br/>"
            Else
                Command += "<Address4>" & Address4 & "</Address4>"
            End If
        End If

        If Not String.IsNullOrEmpty(Address5) Then
            If r.IsMatch(Address5) Then
                IsValid = False
                tmp += "Address5 cannot have any special characters <br/>"
            Else
                Command += "<Address5>" & Address5 & "</Address5>"
            End If
        End If

        If Not String.IsNullOrEmpty(Address6) Then
            If r.IsMatch(Address6) Then
                IsValid = False
                tmp += "Address6 cannot have any special characters <br/>"
            Else
                Command += "<Address6>" & Address6 & "</Address6>"
            End If
        End If

        If Not String.IsNullOrEmpty(Contact1) Then
            If r.IsMatch(Contact1) Then
                IsValid = False
                tmp += "Contact1 cannot have any special characters <br/>"
            Else
                Command += "<Contact1>" & Contact1 & "</Contact1>"
            End If
        End If

        If Not String.IsNullOrEmpty(Contact2) Then
            If r.IsMatch(Contact2) Then
                IsValid = False
                tmp += "Contact2 cannot have any special characters <br/>"
            Else
                Command += "<Contact2>" & Contact2 & "</Contact2>"
            End If
        End If

        If Not String.IsNullOrEmpty(Phone1) Then
            If r.IsMatch(Phone1) Then
                IsValid = False
                tmp += "Phone1 cannot have any special characters <br/>"
            Else
                Command += "<Phone1>" & Phone1 & "</Phone1>"
            End If
        End If

        If Not String.IsNullOrEmpty(Phone2) Then
            If r.IsMatch(Phone2) Then
                IsValid = False
                tmp += "Phone2 cannot have any special characters <br/>"
            Else
                Command += "<Phone2>" & Phone2 & "</Phone2>"
            End If
        End If

        If Not String.IsNullOrEmpty(Email1) Then
            If r.IsMatch(Email1) Then
                IsValid = False
                tmp += "Email1 cannot have any special characters <br/>"
            Else
                Command += "<Email1>" & Email1 & "</Email1>"
            End If
        End If

        If Not String.IsNullOrEmpty(Email2) Then
            If r.IsMatch(Email2) Then
                IsValid = False
                tmp += "Email2 cannot have any special characters <br/>"
            Else
                Command += "<Email2>" & Email2 & "</Email2>"
            End If
        End If

        If Not String.IsNullOrEmpty(UDf1) Then
            If r.IsMatch(UDf1) Then
                IsValid = False
                tmp += "UDF1 cannot have any special characters <br/>"
            Else
                Command += "<SUsr1>" & UDf1 & "</SUsr1>"
            End If
        End If

        If Not String.IsNullOrEmpty(UDf2) Then
            If r.IsMatch(UDf2) Then
                IsValid = False
                tmp += "UDF2 cannot have any special characters <br/>"
            Else
                Command += "<SUsr2>" & UDf2 & "</SUsr2>"
            End If
        End If

        If Not String.IsNullOrEmpty(UDf3) Then
            If r.IsMatch(UDf3) Then
                IsValid = False
                tmp += "UDF3 cannot have any special characters <br/>"
            Else
                Command += "<SUsr3>" & UDf3 & "</SUsr3>"
            End If
        End If

        If Not String.IsNullOrEmpty(UDf4) Then
            If r.IsMatch(UDf4) Then
                IsValid = False
                tmp += "UDF4 cannot have any special characters <br/>"
            Else
                Command += "<SUsr4>" & UDf4 & "</SUsr4>"
            End If
        End If

        If Not String.IsNullOrEmpty(UDf5) Then
            If r.IsMatch(UDf5) Then
                IsValid = False
                tmp += "UDF5 cannot have any special characters <br/>"
            Else
                Command += "<SUsr5>" & UDf5 & "</SUsr5>"
            End If
        End If

        If IsValid Then
            Dim exist As Integer = 0
            If Not EditOperation Then exist = CommonMethods.checkConfigurationExist(storer, type)
            If exist = 0 Then
                Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>storer</MessageType><Action>store</Action><Sender><User>" & CommonMethods.username & "</User><Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID><TenantId>INFOR</TenantId></Sender><Recipient><SystemID>" & CommonMethods.getEnterpriseDBName() & "</SystemID></Recipient></Head><Body><Storer><StorerHeader>" & Command & "</StorerHeader></Storer></Body></Message>"
                tmp = CommonMethods.SaveXml(Xml, IIf(type = "2", "Ship To", "Supplier"), storer)
            Else
                tmp = "Error: " & IIf(type = "2", "Ship To", "Supplier") & " already exists!"
            End If
        End If
        Return tmp
    End Function
    Private Function SaveItem(ByVal MyID As Integer) As String
        Dim tmp As String = "", Command As String = ""
        Dim IsValid As Boolean = True
        Dim EditOperation As Boolean = MyID <> 0
        Dim storer As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
        , Sku As String = UCase(HttpContext.Current.Request.Item("Field_Sku")) _
        , PackKey As String = HttpContext.Current.Request.Item("Field_PackKey") _
        , Descr As String = HttpContext.Current.Request.Item("Field_Descr") _
        , TariffKey As String = HttpContext.Current.Request.Item("Field_TariffKey") _
        , StdCube As String = HttpContext.Current.Request.Item("Field_StdCube") _
        , StdNetWgt As String = HttpContext.Current.Request.Item("Field_StdNetWgt") _
        , StdGrossWgt As String = HttpContext.Current.Request.Item("Field_StdGrossWgt") _
        , SkuGroup As String = HttpContext.Current.Request.Item("Field_SkuGroup")

        If Not String.IsNullOrEmpty(storer) And Trim(storer) <> "" Then
            Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
            If owners.Any(Function(x) x = storer) Or owners.Any(Function(x) x = "ALL") Then
                Command += "<StorerKey>" & storer & "</StorerKey>"
            Else
                IsValid = False
                tmp += "This owner is not authorized <br/>"
            End If
        Else
            IsValid = False
            tmp += "Owner cannot be Empty <br/>"
        End If

        If Not String.IsNullOrEmpty(Sku) Then
            Command += "<Sku>" & Sku & "</Sku>"
        Else
            IsValid = False
            tmp += "Item cannot be Empty <br/>"
        End If

        If Not String.IsNullOrEmpty(Descr) Then Command += "<Descr>" & Descr & "</Descr>"
        If Not String.IsNullOrEmpty(PackKey) Then Command += "<PackKey>" & PackKey & "</PackKey>"
        If Not String.IsNullOrEmpty(TariffKey) Then Command += "<TariffKey>" & TariffKey & "</TariffKey>"
        If Not String.IsNullOrEmpty(StdCube) Then Command += "<StdCube>" & StdCube & "</StdCube>"
        If Not String.IsNullOrEmpty(StdNetWgt) Then Command += "<StdNetWgt>" & StdNetWgt & "</StdNetWgt>"
        If Not String.IsNullOrEmpty(StdGrossWgt) Then Command += "<StdGrossWgt>" & StdGrossWgt & "</StdGrossWgt>"
        If Not String.IsNullOrEmpty(SkuGroup) Then Command += "<SkuGroup>" & SkuGroup & "</SkuGroup>"

        If IsValid Then
            Dim exist As Integer = 0
            If Not EditOperation Then exist = CommonMethods.checkItemExist(storer, Sku)
            If exist = 0 Then
                Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>ItemMaster</MessageType><Action>store</Action><Sender><User>" & CommonMethods.username & "</User><Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID><TenantId>INFOR</TenantId></Sender><Recipient><SystemID>" & CommonMethods.getEnterpriseDBName() & "</SystemID></Recipient></Head><Body><ItemMaster><Item>" & Command & "</Item></ItemMaster></Body></Message>"
                tmp = CommonMethods.SaveXml(Xml, "Item", storer & "-" & Sku)
            Else
                tmp = "Error: Item already exists!"
            End If
        End If
        Return tmp
    End Function
    Private Function SaveItemCatalogue(ByVal MyID As Integer) As String
        Dim tmp As String = ""
        Dim IsValid As Boolean = True
        Dim EditOperation As Boolean = MyID <> 0
        Dim storer As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
        , consignee As String = UCase(HttpContext.Current.Request.Item("Field_ConsigneeKey")) _
        , sku As String = UCase(HttpContext.Current.Request.Item("Field_Sku")) _
        , price As String = HttpContext.Current.Request.Item("Field_Price") _
        , currency As String = HttpContext.Current.Request.Item("Field_Currency") _
        , udf1 As String = HttpContext.Current.Request.Item("Field_SUsr1") _
        , udf2 As String = HttpContext.Current.Request.Item("Field_SUsr2") _
        , udf3 As String = HttpContext.Current.Request.Item("Field_SUsr3") _
        , udf4 As String = HttpContext.Current.Request.Item("Field_SUsr4") _
        , udf5 As String = HttpContext.Current.Request.Item("Field_SUsr5")

        If String.IsNullOrEmpty(storer) Then
            IsValid = False
            tmp += "Owner must be defined <br/>"
        End If

        If String.IsNullOrEmpty(consignee) Then
            IsValid = False
            tmp += "Consignee must be defined <br/>"
        End If

        If String.IsNullOrEmpty(sku) Then
            IsValid = False
            tmp += "Item must be defined <br/>"
        End If

        If String.IsNullOrEmpty(price) Then
            IsValid = False
            tmp += "Price must be defined <br/>"
        End If

        If String.IsNullOrEmpty(currency) Then
            IsValid = False
            tmp += "Currency be defined <br/>"
        End If

        If IsValid Then
            If EditOperation Then
                Try
                    If CommonMethods.dbtype = "sql" Then
                        Dim updatequery As String = "set dateformat dmy update dbo.SKUCATALOGUE set PRICE =@price , SUsr1=@susr1 ,SUsr2= @susr2 , SUsr3= @susr3, SUsr4= @susr4 , SUsr5= @susr4  where SERIALKEY = @serialkey "
                        Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                        conn.Open()
                        Dim cmd As SqlCommand = New SqlCommand(updatequery, conn)
                        cmd.Parameters.AddWithValue("@price", price)
                        cmd.Parameters.AddWithValue("@susr1", udf1)
                        cmd.Parameters.AddWithValue("@susr2", udf2)
                        cmd.Parameters.AddWithValue("@susr3", udf3)
                        cmd.Parameters.AddWithValue("@susr4", udf4)
                        cmd.Parameters.AddWithValue("@susr5", udf5)
                        cmd.Parameters.AddWithValue("@serialkey", MyID)
                        cmd.ExecuteNonQuery()
                        conn.Close()
                    Else
                        Dim updatequery As String = "set dateformat dmy update SYSTEM.SKUCATALOGUE set PRICE =:price , SUsr1=:susr1 ,SUsr2= :susr2 , SUsr3= :susr3, SUsr4= :susr4 , SUsr5= :susr4  where SERIALKEY = :serialkey "
                        Dim conn As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                        conn.Open()
                        Dim cmd As OracleCommand = New OracleCommand(updatequery, conn)
                        cmd.Parameters.Add(New OracleParameter("price", price))
                        cmd.Parameters.Add(New OracleParameter("susr1", udf1))
                        cmd.Parameters.Add(New OracleParameter("susr2", udf2))
                        cmd.Parameters.Add(New OracleParameter("susr3", udf3))
                        cmd.Parameters.Add(New OracleParameter("susr4", udf4))
                        cmd.Parameters.Add(New OracleParameter("susr5", udf5))
                        cmd.Parameters.Add(New OracleParameter("serialkey", MyID))
                        cmd.ExecuteNonQuery()
                        conn.Close()
                    End If
                Catch e1 As Exception
                    tmp += "Error: " & e1.Message & vbTab + e1.GetType.ToString & "<br/>"
                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                    logger.Error(e1, "", "")
                End Try
            Else
                Dim exist As Integer = CommonMethods.checkItemExist(storer, consignee, sku, currency)
                If exist = 0 Then
                    Try
                        If CommonMethods.dbtype = "sql" Then
                            Dim insert As String = "set dateformat dmy insert into dbo.SKUCATALOGUE  (StorerKey, ConsigneeKey, Sku, Price, Currency, SUsr1, SUsr2, SUsr3 , SUsr4,SUsr5) values (@storer, @consignee, @sku, @price, @currency, @susr1, @susr2, @susr3, @susr4, @susr5);"
                            Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                            conn.Open()

                            Dim cmd As SqlCommand = New SqlCommand(insert, conn)
                            cmd.Parameters.AddWithValue("@storer", storer)
                            cmd.Parameters.AddWithValue("@consignee", consignee)
                            cmd.Parameters.AddWithValue("@sku", sku)
                            cmd.Parameters.AddWithValue("@price", price)
                            cmd.Parameters.AddWithValue("@currency", currency)
                            cmd.Parameters.AddWithValue("@susr1", udf1)
                            cmd.Parameters.AddWithValue("@susr2", udf2)
                            cmd.Parameters.AddWithValue("@susr3", udf3)
                            cmd.Parameters.AddWithValue("@susr4", udf4)
                            cmd.Parameters.AddWithValue("@susr5", udf5)
                            cmd.ExecuteNonQuery()

                            conn.Close()
                        Else
                            Dim insert As String = "set dateformat dmy insert into SYSTEM.SKUCATALOGUE (StorerKey, ConsigneeKey, Sku, Price, Currency, SUsr1, SUsr2, SUsr3 , SUsr4,SUsr5) values (:storer, :consignee, :sku, :price, :currency, :susr1, :susr2, :susr3, :susr4, :susr5);"
                            Dim conn As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                            conn.Open()

                            Dim cmd As OracleCommand = New OracleCommand(insert, conn)
                            cmd.Parameters.Add(New OracleParameter("storer", storer))
                            cmd.Parameters.Add(New OracleParameter("consignee", consignee))
                            cmd.Parameters.Add(New OracleParameter("sku", sku))
                            cmd.Parameters.Add(New OracleParameter("price", price))
                            cmd.Parameters.Add(New OracleParameter("currency", currency))
                            cmd.Parameters.Add(New OracleParameter("susr1", udf1))
                            cmd.Parameters.Add(New OracleParameter("susr2", udf2))
                            cmd.Parameters.Add(New OracleParameter("susr3", udf3))
                            cmd.Parameters.Add(New OracleParameter("susr4", udf4))
                            cmd.Parameters.Add(New OracleParameter("susr5", udf5))
                            cmd.ExecuteNonQuery()

                            conn.Close()
                        End If
                    Catch e2 As Exception
                        tmp += "Error: " & e2.Message & vbTab + e2.GetType.ToString & "<br/>"
                        Dim logger As Logger = LogManager.GetCurrentClassLogger()
                        logger.Error(e2, "", "")
                    End Try
                Else
                    tmp += "Error: this item was already defined for this Currency <br/>"
                End If
            End If
        End If
        Return tmp
    End Function
    Private Function SavePurchaseOrder(ByVal MyID As Integer, ByVal DetailsCount As Integer) As String
        Dim tmp As String = "", Command As String = ""
        Dim IsValid As Boolean = True
        Dim EditOperation As Boolean = MyID <> 0
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Field_Facility")) _
        , Owner As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
        , Buyer As String = HttpContext.Current.Request.Item("Field_BuyerName") _
        , Buyerref As String = HttpContext.Current.Request.Item("Field_BuyersReference") _
        , Sellerref As String = HttpContext.Current.Request.Item("Field_SellersReference") _
        , supplier As String = UCase(HttpContext.Current.Request.Item("Field_SellerName")) _
        , pokey As String = HttpContext.Current.Request.Item("Field_POKey") _
        , externpo As String = HttpContext.Current.Request.Item("Field_ExternPOKey") _
        , podate As String = HttpContext.Current.Request.Item("Field_PODate") _
        , effectDate As String = HttpContext.Current.Request.Item("Field_EffectiveDate") _
        , type As String = HttpContext.Current.Request.Item("Field_POType") _
        , UDF1 As String = HttpContext.Current.Request.Item("Field_SUsr1") _
        , UDF2 As String = HttpContext.Current.Request.Item("Field_SUsr2") _
        , UDF3 As String = HttpContext.Current.Request.Item("Field_SUsr3") _
        , UDF4 As String = HttpContext.Current.Request.Item("Field_SUsr4") _
        , UDF5 As String = HttpContext.Current.Request.Item("Field_SUsr5") _
        , exterline As String = HttpContext.Current.Request.Item("DetailsField_ExternLineNo") _
        , Sku = UCase(HttpContext.Current.Request.Item("DetailsField_Sku")) _
        , QtyOrdered = HttpContext.Current.Request.Item("DetailsField_QtyOrdered")

        If String.IsNullOrEmpty(Facility) Then
            IsValid = False
            tmp += "Facility must be defined <br/>"
        End If

        If Not EditOperation Then
            If String.IsNullOrEmpty(externpo) And Not String.IsNullOrEmpty(Facility) Then
                externpo = CommonMethods.getExternKey(Facility, "EXTERNPO")
            End If
            Command += "<ExternPOKey>" & externpo & "</ExternPOKey><Status>0</Status>"
        Else
            Command += "<ExternPOKey>" & externpo & "</ExternPOKey><POKey>" & pokey & "</POKey>"
        End If

        If Not String.IsNullOrEmpty(type) Then
            Dim DTPOType = CommonMethods.getCodeDD(Facility, "codelkup", "POTYPE")
            Dim DRPOType() As DataRow = DTPOType.Select("DESCRIPTION='" & type & "'")
            If DRPOType.Length > 0 Then
                Command += "<POType>" & DRPOType(0)!CODE & "</POType>"
            Else
                IsValid = False
                tmp += "Type must be defined <br/>"
            End If
        Else
            IsValid = False
            tmp += "Type must be defined <br/>"
        End If

        If Not String.IsNullOrEmpty(Owner) And Trim(Owner) <> "" Then
            Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
            If owners.Any(Function(x) x = Owner) Or owners.Any(Function(x) x = "ALL") Then
                Command += "<StorerKey>" & Owner & "</StorerKey>"
            Else
                IsValid = False
                tmp += "This owner is not authorized <br/>"
            End If
        Else
            IsValid = False
            tmp += "Owner must be defined <br/>"
        End If

        If Not String.IsNullOrEmpty(Buyer) Then Command += "<BuyerName>" & Buyer & "</BuyerName>"
        If Not String.IsNullOrEmpty(Buyerref) Then Command += "<BuyersReference>" & Buyerref & "</BuyersReference>"

        If Not String.IsNullOrEmpty(supplier) And Trim(supplier) <> "" Then
            Dim suppliers As String() = CommonMethods.getSupplierPerUser(HttpContext.Current.Session("userkey").ToString)
            If suppliers.Any(Function(x) x = supplier) Or suppliers.Any(Function(x) x = "ALL") Then
                Command += "<SellerName>" & supplier & "</SellerName>"
            Else
                IsValid = False
                tmp += "This supplier is not authorized <br/>"
            End If
        End If

        If Not String.IsNullOrEmpty(Sellerref) Then Command += "<SellersReference>" & Sellerref & "</SellersReference>"

        If Not String.IsNullOrEmpty(effectDate) Then
            Dim datetime As DateTime
            If DateTime.TryParseExact(effectDate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                Dim datetime2 As DateTime = DateTime.ParseExact(effectDate, "MM/dd/yyyy", Nothing)
                If Not String.IsNullOrEmpty(CommonMethods.dformat) Then
                    Command += "<EffectiveDate>" & datetime2.ToString(CommonMethods.dformat & " 14:00:00") & " </EffectiveDate>"
                ElseIf Double.Parse(CommonMethods.version) >= 11 Then
                    Command += "<EffectiveDate>" & datetime2.ToString("MM/dd/yyyy 14:00:00") & " </EffectiveDate>"
                Else
                    Command += "<EffectiveDate>" & datetime2.ToString("dd/MM/yyyy 14:00:00") & " </EffectiveDate>"
                End If
            Else
                IsValid = False
                tmp += "This Effective Date doesn't match the required date format <br/>"
            End If
        End If

        If Not String.IsNullOrEmpty(UDF1) Then Command += "<SUsr1>" & UDF1 & "</SUsr1>"
        If Not String.IsNullOrEmpty(UDF2) Then Command += "<SUsr2>" & UDF2 & "</SUsr2>"
        If Not String.IsNullOrEmpty(UDF3) Then Command += "<SUsr3>" & UDF3 & "</SUsr3>"
        If Not String.IsNullOrEmpty(UDF4) Then Command += "<SUsr4>" & UDF4 & "</SUsr4>"
        If Not String.IsNullOrEmpty(UDF5) Then Command += "<SUsr5>" & UDF5 & "</SUsr5>"

        If IsValid Then
            If DetailsCount > 0 Then
                Dim exterlineArr() As String = Nothing, SkuArr() As String = Nothing, QtyOrderedArr() As String = Nothing

                If exterline IsNot Nothing Then exterlineArr = exterline.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Sku IsNot Nothing Then SkuArr = Sku.Split(New String() {"~~~"}, StringSplitOptions.None)
                If QtyOrdered IsNot Nothing Then QtyOrderedArr = QtyOrdered.Split(New String() {"~~~"}, StringSplitOptions.None)

                If exterlineArr.Length > 1 Then
                    For i = 0 To exterlineArr.Length - 1
                        If Not exterlineArr(i) Is Nothing And Not String.IsNullOrEmpty(exterlineArr(i)) Then
                            If Array.LastIndexOf(exterlineArr, exterlineArr(i)) <> i Then
                                tmp = "Duplicate Extern Line# " & exterlineArr(i) & " on line " & (i + 1).ToString
                                Return tmp
                            End If
                        End If
                    Next
                End If

                If SkuArr.Length = DetailsCount Then
                    For i As Integer = 0 To SkuArr.Length - 1
                        If Not SkuArr(i) Is Nothing Then
                            If Array.LastIndexOf(SkuArr, SkuArr(i)) <> i Then
                                tmp = "Duplicate Item " & SkuArr(i) & " on line " & (i + 1).ToString
                                Return tmp
                            End If
                        End If
                    Next
                End If

                For i = 0 To DetailsCount - 1
                    Command += "<PurchaseOrderDetail>"
                    Try
                        If Not String.IsNullOrEmpty(exterlineArr(i)) Then
                            exterline = exterlineArr(i)
                        Else
                            exterline = (i + 1).ToString.PadLeft(5, "0")
                        End If
                    Catch ex As Exception
                        exterline = (i + 1).ToString.PadLeft(5, "0")
                    End Try
                    Command += "<ExternLineNo>" & exterline & "</ExternLineNo>"

                    Try
                        If Not String.IsNullOrEmpty(SkuArr(i)) Then
                            Command += "<StorerKey>" & Owner & "</StorerKey><Sku>" & SkuArr(i) & "</Sku>"
                        Else
                            IsValid = False
                            tmp += "Item cannot be empty on line " & (i + 1).ToString & " <br/>"
                            Exit For
                        End If
                    Catch ex As Exception
                        IsValid = False
                        tmp += "Item cannot be empty on line " & (i + 1).ToString & " <br/>"
                        Exit For
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(QtyOrderedArr(i)) Then
                            Command += "<QtyOrdered>" & QtyOrderedArr(i) & "</QtyOrdered>"
                        Else
                            IsValid = False
                            tmp += "Qty Ordered cannot be empty on line " & (i + 1).ToString & " <br/>"
                            Exit For
                        End If
                    Catch ex As Exception
                        IsValid = False
                        tmp += "Qty Ordered cannot be empty on line " & (i + 1).ToString & " <br/>"
                        Exit For
                    End Try

                    Command += "</PurchaseOrderDetail> "
                Next
            Else
                IsValid = False
                tmp += "Detail line cannot be empty <br/>"
            End If
        End If

        If IsValid Then
            Dim exist As Integer = 0
            If Not EditOperation Then
                exist = CommonMethods.checkPOExist(Facility, externpo)
            Else
                Dim MyCommand As String = "<PurchaseOrderHeader><ExternPOKey>" & externpo & "</ExternPOKey><POKey>" & pokey & "</POKey></PurchaseOrderHeader>"
                Dim MyXml As String = "<Message> <Head> <MessageID>0000000003</MessageID> <MessageType>PurchaseOrder</MessageType> <Action>delete</Action> <Sender> <User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>		<TenantId>INFOR</TenantId>		</Sender>		<Recipient>			<SystemID>" & Facility & "</SystemID>		</Recipient>	</Head>	<Body><PurchaseOrder> " & MyCommand & "</PurchaseOrder></Body></Message>"
                tmp = CommonMethods.DeleteXml(MyXml, HttpContext.Current.Request.Item("Field_Facility"), "Po", pokey)
                If tmp <> "" Then Return tmp
            End If
            If exist = 0 Then
                Try
                    Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>PurchaseOrder</MessageType><Action>store</Action><Sender>	<User>" & CommonMethods.username & "</User>			<Password>" + CommonMethods.password + "</Password><SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId>	</Sender><Recipient><SystemID>" & Facility & "</SystemID></Recipient></Head><Body><PurchaseOrder><PurchaseOrderHeader>" & Command & "</PurchaseOrderHeader></PurchaseOrder></Body></Message>"

                    Dim soapResult As String = CommonMethods.sendwebRequest(Xml)

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
                            tmp = CommonMethods.incremenetKey(Facility, "EXTERNPO")
                            If tmp = "" Then
                                Dim po As String = "", lineno As String = "", logmessage As String = ""

                                Dim nodeList As XmlNodeList = doc.GetElementsByTagName("POKey")
                                For Each node As XmlNode In nodeList
                                    po = node.InnerText
                                    logmessage = CommonMethods.logger(Facility, "Po", po, HttpContext.Current.Session("userkey").ToString)
                                Next

                                Dim nodeList2 As XmlNodeList = doc.GetElementsByTagName("POLineNumber")
                                For Each node As XmlNode In nodeList2
                                    lineno = node.InnerText
                                    logmessage += CommonMethods.logger(Facility, "PoDetail", po & "-" & lineno, HttpContext.Current.Session("userkey").ToString)
                                Next

                                If Not String.IsNullOrEmpty(logmessage) Then
                                    tmp = "Logging Error: " + logmessage + "<br />"
                                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                                    logger.Error(logmessage, "", "")
                                End If
                            End If
                        End If
                    End If
                Catch ex As Exception
                    tmp = "Error: " & ex.Message & vbTab + ex.GetType.ToString & "<br/>"
                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                    logger.Error(ex, "", "")
                End Try
            Else
                tmp = "Error: Extern PO Key already exists!"
            End If
        End If
        Return tmp
    End Function
    Private Function SaveASN(ByVal MyID As Integer, ByVal DetailsCount As Integer) As String
        Dim tmp As String = "", Command As String = ""
        Dim IsValid As Boolean = True
        Dim EditOperation As Boolean = MyID <> 0
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Field_Facility")) _
        , externreceipt As String = HttpContext.Current.Request.Item("Field_ExternReceiptKey") _
        , receiptkey As String = HttpContext.Current.Request.Item("Field_ReceiptKey") _
        , Owner As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
        , CarrierKey As String = HttpContext.Current.Request.Item("Field_CarrierKey") _
        , WarehouseReference As String = HttpContext.Current.Request.Item("Field_WarehouseReference") _
        , pokey As String = HttpContext.Current.Request.Item("Field_POKey") _
        , type As String = HttpContext.Current.Request.Item("Field_ReceiptType") _
        , receiptdate As String = HttpContext.Current.Request.Item("Field_ReceiptDate") _
        , ContainerKey As String = HttpContext.Current.Request.Item("Field_ContainerKey") _
        , ContainerType As String = HttpContext.Current.Request.Item("Field_ContainerType") _
        , OriginCountry As String = HttpContext.Current.Request.Item("Field_OriginCountry") _
        , TransportationMode As String = HttpContext.Current.Request.Item("Field_TransportationMode") _
        , exterline As String = HttpContext.Current.Request.Item("DetailsField_ExternLineNo") _
        , Sku = UCase(HttpContext.Current.Request.Item("DetailsField_Sku")) _
        , QtyExpected = HttpContext.Current.Request.Item("DetailsField_QtyExpected") _
        , QtyReceived = HttpContext.Current.Request.Item("DetailsField_QtyReceived") _
        , PackKey = HttpContext.Current.Request.Item("DetailsField_PackKey") _
        , UOM = HttpContext.Current.Request.Item("DetailsField_UOM") _
        , POKeyDtl = HttpContext.Current.Request.Item("DetailsField_POKey") _
        , ToId = HttpContext.Current.Request.Item("DetailsField_ToId") _
        , ToLoc = HttpContext.Current.Request.Item("DetailsField_ToLoc") _
        , ConditionCode = HttpContext.Current.Request.Item("DetailsField_ConditionCode") _
        , TariffKey = HttpContext.Current.Request.Item("DetailsField_TariffKey") _
        , Lottable01 = HttpContext.Current.Request.Item("DetailsField_Lottable01") _
        , Lottable02 = HttpContext.Current.Request.Item("DetailsField_Lottable02") _
        , Lottable03 = HttpContext.Current.Request.Item("DetailsField_Lottable03") _
        , Lottable04 = HttpContext.Current.Request.Item("DetailsField_Lottable04") _
        , Lottable05 = HttpContext.Current.Request.Item("DetailsField_Lottable05") _
        , Lottable06 = HttpContext.Current.Request.Item("DetailsField_Lottable06") _
        , Lottable07 = HttpContext.Current.Request.Item("DetailsField_Lottable07") _
        , Lottable08 = HttpContext.Current.Request.Item("DetailsField_Lottable08") _
        , Lottable09 = HttpContext.Current.Request.Item("DetailsField_Lottable09") _
        , Lottable10 = HttpContext.Current.Request.Item("DetailsField_Lottable10") _
        , Lottable11 = HttpContext.Current.Request.Item("DetailsField_Lottable11") _
        , Lottable12 = HttpContext.Current.Request.Item("DetailsField_Lottable12")

        If String.IsNullOrEmpty(Facility) Then
            IsValid = False
            tmp += "Facility must be defined <br/>"
        End If

        If Not EditOperation Then
            If String.IsNullOrEmpty(externreceipt) And Not String.IsNullOrEmpty(Facility) Then
                externreceipt = CommonMethods.getExternKey(Facility, "EXTERNASN")
            End If
            Command += "<ExternReceiptKey>" & externreceipt & "</ExternReceiptKey><Status>0</Status>"
        Else
            Command += "<ExternReceiptKey>" & externreceipt & "</ExternReceiptKey><ReceiptKey>" & receiptkey & "</ReceiptKey>"
        End If

        If Not String.IsNullOrEmpty(type) Then
            Dim DTReceiptType = CommonMethods.getCodeDD(Facility, "codelkup", "RECEIPTYPE")
            Dim DRReceiptType() As DataRow = DTReceiptType.Select("DESCRIPTION='" & type & "'")
            If DRReceiptType.Length > 0 Then
                Command += "<ReceiptType>" & DRReceiptType(0)!CODE & "</ReceiptType>"
            Else
                IsValid = False
                tmp += "Type must be defined <br/>"
            End If
        Else
            IsValid = False
            tmp += "Type must be defined <br/>"
        End If

        If Not String.IsNullOrEmpty(Owner) And Trim(Owner) <> "" Then
            Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
            If owners.Any(Function(x) x = Owner) Or owners.Any(Function(x) x = "ALL") Then
                Command += "<StorerKey>" & Owner & "</StorerKey>"
            Else
                IsValid = False
                tmp += "This owner is not authorized <br/>"
            End If
        Else
            IsValid = False
            tmp += "Owner must be defined <br/>"
        End If

        If Not String.IsNullOrEmpty(pokey) Then Command += "<POKey>" & pokey & "</POKey>"
        If Not String.IsNullOrEmpty(CarrierKey) Then Command += "<CarrierKey>" & CarrierKey & "</CarrierKey>"
        If Not String.IsNullOrEmpty(WarehouseReference) Then Command += "<WarehouseReference>" & WarehouseReference & "</WarehouseReference>"
        If Not String.IsNullOrEmpty(ContainerKey) Then Command += "<ContainerKey>" & ContainerKey & "</ContainerKey>"
        If Not String.IsNullOrEmpty(ContainerType) Then Command += "<ContainerType>" & ContainerType & "</ContainerType>"
        If Not String.IsNullOrEmpty(OriginCountry) Then Command += "<OriginCountry>" & CommonMethods.getISOCountryCode(OriginCountry) & "</OriginCountry>"
        If Not String.IsNullOrEmpty(TransportationMode) Then Command += "<TransportationMode>" & TransportationMode & "</TransportationMode>"

        If IsValid Then
            If DetailsCount > 0 Then
                Dim exterlineArr() As String = Nothing, SkuArr() As String = Nothing, QtyExpectedArr() As String = Nothing,
                    QtyReceivedArr() As String = Nothing, PackKeyArr() As String = Nothing, UOMArr() As String = Nothing,
                    POKeyDtlArr() As String = Nothing, ToIdArr() As String = Nothing, ToLocArr() As String = Nothing,
                    ConditionCodeArr() As String = Nothing, TariffKeyArr() As String = Nothing,
                    Lottable01Arr() As String = Nothing, Lottable02Arr() As String = Nothing,
                    Lottable03Arr() As String = Nothing, Lottable04Arr() As String = Nothing,
                    Lottable05Arr() As String = Nothing, Lottable06Arr() As String = Nothing,
                    Lottable07Arr() As String = Nothing, Lottable08Arr() As String = Nothing,
                    Lottable09Arr() As String = Nothing, Lottable10Arr() As String = Nothing,
                    Lottable11Arr() As String = Nothing, Lottable12Arr() As String = Nothing

                If exterline IsNot Nothing Then exterlineArr = exterline.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Sku IsNot Nothing Then SkuArr = Sku.Split(New String() {"~~~"}, StringSplitOptions.None)
                If QtyExpected IsNot Nothing Then QtyExpectedArr = QtyExpected.Split(New String() {"~~~"}, StringSplitOptions.None)
                If QtyReceived IsNot Nothing Then QtyReceivedArr = QtyReceived.Split(New String() {"~~~"}, StringSplitOptions.None)
                If PackKey IsNot Nothing Then PackKeyArr = PackKey.Split(New String() {"~~~"}, StringSplitOptions.None)
                If UOM IsNot Nothing Then UOMArr = UOM.Split(New String() {"~~~"}, StringSplitOptions.None)
                If POKeyDtl IsNot Nothing Then POKeyDtlArr = POKeyDtl.Split(New String() {"~~~"}, StringSplitOptions.None)
                If ToId IsNot Nothing Then ToIdArr = ToId.Split(New String() {"~~~"}, StringSplitOptions.None)
                If ToLoc IsNot Nothing Then ToLocArr = ToLoc.Split(New String() {"~~~"}, StringSplitOptions.None)
                If ConditionCode IsNot Nothing Then ConditionCodeArr = ConditionCode.Split(New String() {"~~~"}, StringSplitOptions.None)
                If TariffKey IsNot Nothing Then TariffKeyArr = TariffKey.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable01 IsNot Nothing Then Lottable01Arr = Lottable01.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable02 IsNot Nothing Then Lottable02Arr = Lottable02.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable03 IsNot Nothing Then Lottable03Arr = Lottable03.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable04 IsNot Nothing Then Lottable04Arr = Lottable04.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable05 IsNot Nothing Then Lottable05Arr = Lottable05.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable06 IsNot Nothing Then Lottable06Arr = Lottable06.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable07 IsNot Nothing Then Lottable07Arr = Lottable07.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable08 IsNot Nothing Then Lottable08Arr = Lottable08.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable09 IsNot Nothing Then Lottable09Arr = Lottable09.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable10 IsNot Nothing Then Lottable10Arr = Lottable10.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable11 IsNot Nothing Then Lottable11Arr = Lottable11.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable12 IsNot Nothing Then Lottable12Arr = Lottable12.Split(New String() {"~~~"}, StringSplitOptions.None)

                If exterlineArr.Length > 1 Then
                    For i = 0 To exterlineArr.Length - 1
                        If Not exterlineArr(i) Is Nothing And Not String.IsNullOrEmpty(exterlineArr(i)) Then
                            If Array.LastIndexOf(exterlineArr, exterlineArr(i)) <> i Then
                                tmp = "Duplicate Extern Line# " & exterlineArr(i) & " on line " & (i + 1).ToString
                                Return tmp
                            End If
                        End If
                    Next
                End If

                If SkuArr.Length = DetailsCount Then
                    For i As Integer = 0 To SkuArr.Length - 1
                        If Not SkuArr(i) Is Nothing Then
                            If Array.LastIndexOf(SkuArr, SkuArr(i)) <> i Then
                                tmp = "Duplicate Item " & SkuArr(i) & " on line " & (i + 1).ToString
                                Return tmp
                            End If
                        End If
                    Next
                End If

                For i = 0 To DetailsCount - 1
                    Command += "<AdvancedShipNoticeDetail>"
                    Try
                        If Not String.IsNullOrEmpty(exterlineArr(i)) Then
                            exterline = exterlineArr(i)
                        Else
                            exterline = (i + 1).ToString.PadLeft(5, "0")
                        End If
                    Catch ex As Exception
                        exterline = (i + 1).ToString.PadLeft(5, "0")
                    End Try
                    Command += "<ExternLineNo>" & exterline & "</ExternLineNo>"

                    Try
                        If Not String.IsNullOrEmpty(SkuArr(i)) Then
                            Command += "<StorerKey>" & Owner & "</StorerKey><Sku>" & SkuArr(i) & "</Sku>"
                        Else
                            IsValid = False
                            tmp += "Item cannot be empty on line " & (i + 1).ToString & " <br/>"
                            Exit For
                        End If
                    Catch ex As Exception
                        IsValid = False
                        tmp += "Item cannot be empty on line " & (i + 1).ToString & " <br/>"
                        Exit For
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(QtyExpectedArr(i)) Then
                            Command += "<QtyExpected>" & QtyExpectedArr(i) & "</QtyExpected>"
                        Else
                            IsValid = False
                            tmp += "Qty Expected cannot be empty on line " & (i + 1).ToString & "<br/>"
                            Exit For
                        End If
                    Catch ex As Exception
                        IsValid = False
                        tmp += "Qty Expected cannot be empty on line " & (i + 1).ToString & "<br/>"
                        Exit For
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(QtyReceivedArr(i)) Then
                            Command += "<QtyReceived>" + QtyReceivedArr(i) + "</QtyReceived>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(PackKeyArr(i)) Then
                            Command += "<PackKey>" + PackKeyArr(i) + "</PackKey>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(UOMArr(i)) Then
                            Command += "<UOM>" + UOMArr(i) + "</UOM>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(POKeyDtlArr(i)) Then
                            Command += "<POKey>" + POKeyDtlArr(i) + "</POKey>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(ToIdArr(i)) Then
                            Command += "<ToId>" + ToIdArr(i) + "</ToId>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(ToLocArr(i)) Then
                            Command += "<ToLoc>" + ToLocArr(i) + "</ToLoc>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(ConditionCodeArr(i)) Then
                            Command += "<ConditionCode>" + ConditionCodeArr(i) + "</ConditionCode>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(TariffKeyArr(i)) Then
                            Command += "<TariffKey>" + TariffKeyArr(i) + "</TariffKey>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable01Arr(i)) Then
                            Command += "<Lottable01>" + Lottable01Arr(i) + "</Lottable01>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable02Arr(i)) Then
                            Command += "<Lottable02>" + Lottable02Arr(i) + "</Lottable02>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable03Arr(i)) Then
                            Command += "<Lottable03>" + Lottable03Arr(i) + "</Lottable03>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable04Arr(i)) Then
                            Dim datetime As DateTime
                            If DateTime.TryParseExact(Lottable04Arr(i), "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                                Dim datetime2 As DateTime = DateTime.ParseExact(Lottable04Arr(i), "MM/dd/yyyy", Nothing)
                                If Not String.IsNullOrEmpty(CommonMethods.dformat) Then
                                    Command += "<Lottable04>" & datetime2.ToString(CommonMethods.dformat & " 14:00:00") & " </Lottable04>"
                                ElseIf Double.Parse(CommonMethods.version) >= 11 Then
                                    Command += "<Lottable04>" & datetime2.ToString("MM/dd/yyyy 14:00:00") & " </Lottable04>"
                                Else
                                    Command += "<Lottable04>" & datetime2.ToString("dd/MM/yyyy 14:00:00") & " </Lottable04>"
                                End If
                            Else
                                IsValid = False
                                tmp += "Lottable04 doesn't match the required date format one line " & (i + 1).ToString & " <br/>"
                                Exit For
                            End If
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable05Arr(i)) Then
                            Dim datetime As DateTime
                            If DateTime.TryParseExact(Lottable05Arr(i), "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                                Dim datetime2 As DateTime = DateTime.ParseExact(Lottable05Arr(i), "MM/dd/yyyy", Nothing)
                                If Not String.IsNullOrEmpty(CommonMethods.dformat) Then
                                    Command += "<Lottable05>" & datetime2.ToString(CommonMethods.dformat & " 14:00:00") & " </Lottable05>"
                                ElseIf Double.Parse(CommonMethods.version) >= 11 Then
                                    Command += "<Lottable05>" & datetime2.ToString("MM/dd/yyyy 14:00:00") & " </Lottable05>"
                                Else
                                    Command += "<Lottable05>" & datetime2.ToString("dd/MM/yyyy 14:00:00") & " </Lottable05>"
                                End If
                            Else
                                IsValid = False
                                tmp += "Lottable05 doesn't match the required date format one line " & (i + 1).ToString & " <br/>"
                                Exit For
                            End If
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable06Arr(i)) Then
                            Command += "<Lottable06>" + Lottable06Arr(i) + "</Lottable06>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable07Arr(i)) Then
                            Command += "<Lottable07>" + Lottable07Arr(i) + "</Lottable07>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable08Arr(i)) Then
                            Command += "<Lottable08>" + Lottable08Arr(i) + "</Lottable08>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable09Arr(i)) Then
                            Command += "<Lottable09>" + Lottable09Arr(i) + "</Lottable09>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable10Arr(i)) Then
                            Command += "<Lottable10>" + Lottable10Arr(i) + "</Lottable10>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable11Arr(i)) Then
                            Dim datetime As DateTime
                            If DateTime.TryParseExact(Lottable11Arr(i), "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                                Dim datetime2 As DateTime = DateTime.ParseExact(Lottable11Arr(i), "MM/dd/yyyy", Nothing)
                                If Not String.IsNullOrEmpty(CommonMethods.dformat) Then
                                    Command += "<Lottable11>" & datetime2.ToString(CommonMethods.dformat & " 14:00:00") & " </Lottable11>"
                                ElseIf Double.Parse(CommonMethods.version) >= 11 Then
                                    Command += "<Lottable11>" & datetime2.ToString("MM/dd/yyyy 14:00:00") & " </Lottable11>"
                                Else
                                    Command += "<Lottable11>" & datetime2.ToString("dd/MM/yyyy 14:00:00") & " </Lottable11>"
                                End If
                            Else
                                IsValid = False
                                tmp += "Lottable11 doesn't match the required date format one line " & (i + 1).ToString & " <br/>"
                                Exit For
                            End If
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable12Arr(i)) Then
                            Dim datetime As DateTime
                            If DateTime.TryParseExact(Lottable12Arr(i), "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                                Dim datetime2 As DateTime = DateTime.ParseExact(Lottable12Arr(i), "MM/dd/yyyy", Nothing)
                                If Not String.IsNullOrEmpty(CommonMethods.dformat) Then
                                    Command += "<Lottable12>" & datetime2.ToString(CommonMethods.dformat & " 14:00:00") & " </Lottable12>"
                                ElseIf Double.Parse(CommonMethods.version) >= 11 Then
                                    Command += "<Lottable12>" & datetime2.ToString("MM/dd/yyyy 14:00:00") & " </Lottable12>"
                                Else
                                    Command += "<Lottable12>" & datetime2.ToString("dd/MM/yyyy 14:00:00") & " </Lottable12>"
                                End If
                            Else
                                IsValid = False
                                tmp += "Lottable12 doesn't match the required date format one line " & (i + 1).ToString & " <br/>"
                                Exit For
                            End If
                        End If
                    Catch ex As Exception
                    End Try

                    Command += "</AdvancedShipNoticeDetail> "
                Next
            Else
                IsValid = False
                tmp += "Detail line cannot be empty <br/>"
            End If
        End If

        If IsValid Then
            Dim exist As Integer = 0
            If Not EditOperation Then
                exist = CommonMethods.checkASNExist(Facility, externreceipt)
            Else
                Dim MyCommand As String = "<AdvancedShipNoticeHeader><ExternReceiptKey>" & externreceipt & "</ExternReceiptKey><ReceiptKey>" & receiptkey & "</ReceiptKey></AdvancedShipNoticeHeader>"
                Dim MyXml As String = "<Message> <Head> <MessageID>0000000003</MessageID> <MessageType>AdvancedShipNotice</MessageType> <Action>delete</Action> <Sender> 	<User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password>	<SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId></Sender>		<Recipient>			<SystemID>" & Facility & "</SystemID>		</Recipient>	</Head>	<Body><AdvancedShipNotice> " & MyCommand & "</AdvancedShipNotice></Body></Message>"
                tmp = CommonMethods.DeleteXml(MyXml, HttpContext.Current.Request.Item("Field_Facility"), "Receipt", receiptkey)
                If tmp <> "" Then Return tmp
            End If
            If exist = 0 Then
                Try
                    Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>AdvancedShipNotice</MessageType><Action>store</Action><Sender><User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId>	</Sender><Recipient><SystemID>" & Facility & "</SystemID></Recipient></Head><Body><AdvancedShipNotice><AdvancedShipNoticeHeader>" & Command & "</AdvancedShipNoticeHeader></AdvancedShipNotice></Body></Message>"

                    Dim soapResult As String = CommonMethods.sendwebRequest(Xml)

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
                            tmp = CommonMethods.incremenetKey(Facility, "EXTERNASN")
                            If tmp = "" Then
                                Dim receipt As String = "", lineno As String = "", logmessage As String = ""

                                Dim nodeList As XmlNodeList = doc.GetElementsByTagName("ReceiptKey")
                                For Each node As XmlNode In nodeList
                                    receipt = node.InnerText
                                    logmessage = CommonMethods.logger(Facility, "Receipt", receipt, HttpContext.Current.Session("userkey").ToString)
                                Next

                                Dim nodeList2 As XmlNodeList = doc.GetElementsByTagName("ReceiptLineNumber")
                                For Each node As XmlNode In nodeList2
                                    lineno = node.InnerText
                                    logmessage += CommonMethods.logger(Facility, "ReceiptDetail", receipt & "-" & lineno, HttpContext.Current.Session("userkey").ToString)
                                Next

                                If Not String.IsNullOrEmpty(logmessage) Then
                                    tmp = "Logging Error: " + logmessage + "<br />"
                                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                                    logger.Error(logmessage, "", "")
                                End If
                            End If
                        End If
                    End If
                Catch ex As Exception
                    tmp = "Error: " & ex.Message & vbTab + ex.GetType.ToString & "<br/>"
                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                    logger.Error(ex, "", "")
                End Try
            Else
                tmp = "Error: Extern Receipt Key already exists!"
            End If
        End If
        Return tmp
    End Function
    Private Function SaveSO(ByVal MyID As Integer, ByVal DetailsCount As Integer) As String
        Dim tmp As String = "", Command As String = ""
        Dim IsValid As Boolean = True
        Dim EditOperation As Boolean = MyID <> 0
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Field_Facility")) _
        , externorder As String = HttpContext.Current.Request.Item("Field_ExternOrderKey") _
        , orderkey As String = HttpContext.Current.Request.Item("Field_OrderKey") _
        , Owner As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
        , ConsigneeKey As String = UCase(HttpContext.Current.Request.Item("Field_ConsigneeKey")) _
        , type As String = HttpContext.Current.Request.Item("Field_OrderType") _
        , orderdate As String = HttpContext.Current.Request.Item("Field_OrderDate") _
        , requestedshipdate As String = HttpContext.Current.Request.Item("Field_RequestedShipDate") _
        , actualshipdate As String = HttpContext.Current.Request.Item("Field_ActualShipDate") _
        , UDF1 As String = HttpContext.Current.Request.Item("Field_SUsr1") _
        , UDF2 As String = HttpContext.Current.Request.Item("Field_SUsr2") _
        , UDF3 As String = HttpContext.Current.Request.Item("Field_SUsr3") _
        , UDF4 As String = HttpContext.Current.Request.Item("Field_SUsr4") _
        , UDF5 As String = HttpContext.Current.Request.Item("Field_SUsr5") _
        , exterline As String = HttpContext.Current.Request.Item("DetailsField_ExternLineNo") _
        , Sku = UCase(HttpContext.Current.Request.Item("DetailsField_Sku")) _
        , OpenQty = HttpContext.Current.Request.Item("DetailsField_OpenQty") _
        , PackKey = HttpContext.Current.Request.Item("DetailsField_PackKey") _
        , UOM = HttpContext.Current.Request.Item("DetailsField_UOM") _
        , UDF1Dtl = HttpContext.Current.Request.Item("DetailsField_SUsr1") _
        , UDF2Dtl = HttpContext.Current.Request.Item("DetailsField_SUsr2") _
        , UDF3Dtl = HttpContext.Current.Request.Item("DetailsField_SUsr3") _
        , UDF4Dtl = HttpContext.Current.Request.Item("DetailsField_SUsr4") _
        , UDF5Dtl = HttpContext.Current.Request.Item("DetailsField_SUsr5") _
        , Lottable01 = HttpContext.Current.Request.Item("DetailsField_Lottable01") _
        , Lottable02 = HttpContext.Current.Request.Item("DetailsField_Lottable02") _
        , Lottable03 = HttpContext.Current.Request.Item("DetailsField_Lottable03") _
        , Lottable04 = HttpContext.Current.Request.Item("DetailsField_Lottable04") _
        , Lottable05 = HttpContext.Current.Request.Item("DetailsField_Lottable05") _
        , Lottable06 = HttpContext.Current.Request.Item("DetailsField_Lottable06") _
        , Lottable07 = HttpContext.Current.Request.Item("DetailsField_Lottable07") _
        , Lottable08 = HttpContext.Current.Request.Item("DetailsField_Lottable08") _
        , Lottable09 = HttpContext.Current.Request.Item("DetailsField_Lottable09") _
        , Lottable10 = HttpContext.Current.Request.Item("DetailsField_Lottable10")

        If String.IsNullOrEmpty(Facility) Then
            IsValid = False
            tmp += "Facility must be defined <br/>"
        End If

        If Not EditOperation Then
            If String.IsNullOrEmpty(externorder) And Not String.IsNullOrEmpty(Facility) Then
                externorder = CommonMethods.getExternKey(Facility, "EXTERNSO")
            End If
            Command += "<ExternOrderKey>" & externorder & "</ExternOrderKey>"
        Else
            Command += "<ExternOrderKey>" & externorder & "</ExternOrderKey><OrderKey>" & orderkey & "</OrderKey>"
        End If

        If Not String.IsNullOrEmpty(type) Then
            Dim DTOrderType = CommonMethods.getCodeDD(Facility, "codelkup", "ORDERTYPE")
            Dim DROrderType() As DataRow = DTOrderType.Select("DESCRIPTION='" & type & "'")
            If DROrderType.Length > 0 Then
                Command += "<OrderType>" & DROrderType(0)!CODE & "</OrderType>"
            Else
                IsValid = False
                tmp += "Type must be defined <br/>"
            End If
        Else
            IsValid = False
            tmp += "Type must be defined <br/>"
        End If

        If Not String.IsNullOrEmpty(Owner) And Trim(Owner) <> "" Then
            Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
            If owners.Any(Function(x) x = Owner) Or owners.Any(Function(x) x = "ALL") Then
                Command += "<StorerKey>" & Owner & "</StorerKey>"
            Else
                IsValid = False
                tmp += "This owner is not authorized <br/>"
            End If
        Else
            IsValid = False
            tmp += "Owner must be defined <br/>"
        End If

        If Not String.IsNullOrEmpty(ConsigneeKey) And Trim(ConsigneeKey) <> "" Then
            Dim consignees As String() = CommonMethods.getConsigneePerUser(HttpContext.Current.Session("userkey").ToString)
            If consignees.Any(Function(x) x = ConsigneeKey) Or consignees.Any(Function(x) x = "ALL") Then
                Command += "<ConsigneeKey>" & ConsigneeKey & "</ConsigneeKey>"
            Else
                IsValid = False
                tmp += "This consignee is not authorized <br/>"
            End If
        End If

        If Not String.IsNullOrEmpty(requestedshipdate) Then
            Dim datetime As DateTime
            If DateTime.TryParseExact(requestedshipdate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                Dim datetime2 As DateTime = DateTime.ParseExact(requestedshipdate, "MM/dd/yyyy", Nothing)
                If Not String.IsNullOrEmpty(CommonMethods.dformat) Then
                    Command += "<RequestedShipDate>" & datetime2.ToString(CommonMethods.dformat & " 14:00:00") & " </RequestedShipDate>"
                Else
                    Command += "<RequestedShipDate>" & datetime2.ToString("dd/MM/yyyy 14:00:00") & " </RequestedShipDate>"
                End If
            Else
                IsValid = False
                tmp += "Requested ship date doesn't match the required date format <br/>"
            End If
        End If

        If EditOperation Then
            If Not String.IsNullOrEmpty(actualshipdate) Then
                Command += "<ActualShipDate>" & actualshipdate & " </ActualShipDate>"
            End If
        End If

        If Not String.IsNullOrEmpty(UDF1) Then Command += "<SUsr1>" & UDF1 & "</SUsr1>"
        If Not String.IsNullOrEmpty(UDF2) Then Command += "<SUsr2>" & UDF2 & "</SUsr2>"
        If Not String.IsNullOrEmpty(UDF3) Then Command += "<SUsr3>" & UDF3 & "</SUsr3>"
        If Not String.IsNullOrEmpty(UDF4) Then Command += "<SUsr4>" & UDF4 & "</SUsr4>"
        If Not String.IsNullOrEmpty(UDF5) Then Command += "<SUsr5>" & UDF5 & "</SUsr5>"

        If IsValid Then
            If DetailsCount > 0 Then
                Dim exterlineArr() As String = Nothing, SkuArr() As String = Nothing, OpenQtyArr() As String = Nothing,
                    PackKeyArr() As String = Nothing, UOMArr() As String = Nothing, UDF1DtlArr() As String = Nothing,
                    UDF2DtlArr() As String = Nothing, UDF3DtlArr() As String = Nothing, UDF4DtlArr() As String = Nothing,
                    UDF5DtlArr() As String = Nothing,
                    Lottable01Arr() As String = Nothing, Lottable02Arr() As String = Nothing,
                    Lottable03Arr() As String = Nothing, Lottable04Arr() As String = Nothing,
                    Lottable05Arr() As String = Nothing, Lottable06Arr() As String = Nothing,
                    Lottable07Arr() As String = Nothing, Lottable08Arr() As String = Nothing,
                    Lottable09Arr() As String = Nothing, Lottable10Arr() As String = Nothing

                If exterline IsNot Nothing Then exterlineArr = exterline.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Sku IsNot Nothing Then SkuArr = Sku.Split(New String() {"~~~"}, StringSplitOptions.None)
                If OpenQty IsNot Nothing Then OpenQtyArr = OpenQty.Split(New String() {"~~~"}, StringSplitOptions.None)
                If PackKey IsNot Nothing Then PackKeyArr = PackKey.Split(New String() {"~~~"}, StringSplitOptions.None)
                If UOM IsNot Nothing Then UOMArr = UOM.Split(New String() {"~~~"}, StringSplitOptions.None)
                If UDF1Dtl IsNot Nothing Then UDF1DtlArr = UDF1Dtl.Split(New String() {"~~~"}, StringSplitOptions.None)
                If UDF2Dtl IsNot Nothing Then UDF2DtlArr = UDF2Dtl.Split(New String() {"~~~"}, StringSplitOptions.None)
                If UDF3Dtl IsNot Nothing Then UDF3DtlArr = UDF3Dtl.Split(New String() {"~~~"}, StringSplitOptions.None)
                If UDF4Dtl IsNot Nothing Then UDF4DtlArr = UDF4Dtl.Split(New String() {"~~~"}, StringSplitOptions.None)
                If UDF5Dtl IsNot Nothing Then UDF5DtlArr = UDF5Dtl.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable01 IsNot Nothing Then Lottable01Arr = Lottable01.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable02 IsNot Nothing Then Lottable02Arr = Lottable02.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable03 IsNot Nothing Then Lottable03Arr = Lottable03.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable04 IsNot Nothing Then Lottable04Arr = Lottable04.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable05 IsNot Nothing Then Lottable05Arr = Lottable05.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable06 IsNot Nothing Then Lottable06Arr = Lottable06.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable07 IsNot Nothing Then Lottable07Arr = Lottable07.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable08 IsNot Nothing Then Lottable08Arr = Lottable08.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable09 IsNot Nothing Then Lottable09Arr = Lottable09.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable10 IsNot Nothing Then Lottable10Arr = Lottable10.Split(New String() {"~~~"}, StringSplitOptions.None)

                If exterlineArr.Length > 1 Then
                    For i = 0 To exterlineArr.Length - 1
                        If Not exterlineArr(i) Is Nothing And Not String.IsNullOrEmpty(exterlineArr(i)) Then
                            If Array.LastIndexOf(exterlineArr, exterlineArr(i)) <> i Then
                                tmp = "Duplicate Extern Line# " & exterlineArr(i) & " on line " & (i + 1).ToString
                                Return tmp
                            End If
                        End If
                    Next
                End If

                If SkuArr.Length = DetailsCount Then
                    For i As Integer = 0 To SkuArr.Length - 1
                        If Not SkuArr(i) Is Nothing Then
                            If Array.LastIndexOf(SkuArr, SkuArr(i)) <> i Then
                                tmp = "Duplicate Item " & SkuArr(i) & " on line " & (i + 1).ToString
                                Return tmp
                            End If
                        End If
                    Next
                End If

                For i = 0 To DetailsCount - 1
                    Command += "<ShipmentOrderDetail>"
                    Try
                        If Not String.IsNullOrEmpty(exterlineArr(i)) Then
                            exterline = exterlineArr(i)
                        Else
                            exterline = (i + 1).ToString.PadLeft(5, "0")
                        End If
                    Catch ex As Exception
                        exterline = (i + 1).ToString.PadLeft(5, "0")
                    End Try
                    Command += "<ExternLineNo>" & exterline & "</ExternLineNo>"

                    Try
                        If Not String.IsNullOrEmpty(SkuArr(i)) Then
                            Command += "<StorerKey>" & Owner & "</StorerKey><Sku>" & SkuArr(i) & "</Sku>"
                        Else
                            IsValid = False
                            tmp += "Item cannot be empty on line " & (i + 1).ToString & " <br/>"
                            Exit For
                        End If
                    Catch ex As Exception
                        IsValid = False
                        tmp += "Item cannot be empty on line " & (i + 1).ToString & " <br/>"
                        Exit For
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(OpenQtyArr(i)) Then
                            Command += "<OpenQty>" & OpenQtyArr(i) & "</OpenQty>"
                        Else
                            IsValid = False
                            tmp += "Open Qty cannot be empty on line " & (i + 1).ToString & "<br/>"
                            Exit For
                        End If
                    Catch ex As Exception
                        IsValid = False
                        tmp += "Open Qty cannot be empty on line " & (i + 1).ToString & "<br/>"
                        Exit For
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(PackKeyArr(i)) Then
                            Command += "<PackKey>" + PackKeyArr(i) + "</PackKey>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(UOMArr(i)) Then
                            Command += "<UOM>" + UOMArr(i) + "</UOM>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(UDF1DtlArr(i)) Then
                            Command += "<SUsr1>" + UDF1DtlArr(i) + "</SUsr1>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(UDF2DtlArr(i)) Then
                            Command += "<SUsr2>" + UDF2DtlArr(i) + "</SUsr2>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(UDF3DtlArr(i)) Then
                            Command += "<SUsr3>" + UDF3DtlArr(i) + "</SUsr3>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(UDF4DtlArr(i)) Then
                            Command += "<SUsr4>" + UDF4DtlArr(i) + "</SUsr4>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(UDF5DtlArr(i)) Then
                            Command += "<SUsr5>" + UDF5DtlArr(i) + "</SUsr5>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable01Arr(i)) Then
                            Command += "<Lottable01>" + Lottable01Arr(i) + "</Lottable01>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable02Arr(i)) Then
                            Command += "<Lottable02>" + Lottable02Arr(i) + "</Lottable02>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable03Arr(i)) Then
                            Command += "<Lottable03>" + Lottable03Arr(i) + "</Lottable03>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable04Arr(i)) Then
                            Dim datetime As DateTime
                            If DateTime.TryParseExact(Lottable04Arr(i), "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                                Dim datetime2 As DateTime = DateTime.ParseExact(Lottable04Arr(i), "MM/dd/yyyy", Nothing)
                                If Not String.IsNullOrEmpty(CommonMethods.dformat) Then
                                    Command += "<Lottable04>" & datetime2.ToString(CommonMethods.dformat & " 14:00:00") & " </Lottable04>"
                                ElseIf Double.Parse(CommonMethods.version) >= 11 Then
                                    Command += "<Lottable04>" & datetime2.ToString("MM/dd/yyyy 14:00:00") & " </Lottable04>"
                                Else
                                    Command += "<Lottable04>" & datetime2.ToString("dd/MM/yyyy 14:00:00") & " </Lottable04>"
                                End If
                            Else
                                IsValid = False
                                tmp += "Lottable04 doesn't match the required date format one line " & (i + 1).ToString & " <br/>"
                                Exit For
                            End If
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable05Arr(i)) Then
                            Dim datetime As DateTime
                            If DateTime.TryParseExact(Lottable05Arr(i), "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                                Dim datetime2 As DateTime = DateTime.ParseExact(Lottable05Arr(i), "MM/dd/yyyy", Nothing)
                                If Not String.IsNullOrEmpty(CommonMethods.dformat) Then
                                    Command += "<Lottable05>" & datetime2.ToString(CommonMethods.dformat & " 14:00:00") & " </Lottable05>"
                                ElseIf Double.Parse(CommonMethods.version) >= 11 Then
                                    Command += "<Lottable05>" & datetime2.ToString("MM/dd/yyyy 14:00:00") & " </Lottable05>"
                                Else
                                    Command += "<Lottable05>" & datetime2.ToString("dd/MM/yyyy 14:00:00") & " </Lottable05>"
                                End If
                            Else
                                IsValid = False
                                tmp += "Lottable05 doesn't match the required date format one line " & (i + 1).ToString & " <br/>"
                                Exit For
                            End If
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable06Arr(i)) Then
                            Command += "<Lottable06>" + Lottable06Arr(i) + "</Lottable06>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable07Arr(i)) Then
                            Command += "<Lottable07>" + Lottable07Arr(i) + "</Lottable07>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable08Arr(i)) Then
                            Command += "<Lottable08>" + Lottable08Arr(i) + "</Lottable08>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable09Arr(i)) Then
                            Command += "<Lottable09>" + Lottable09Arr(i) + "</Lottable09>"
                        End If
                    Catch ex As Exception
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable10Arr(i)) Then
                            Command += "<Lottable10>" + Lottable10Arr(i) + "</Lottable10>"
                        End If
                    Catch ex As Exception
                    End Try

                    Command += "</ShipmentOrderDetail> "
                Next
            Else
                IsValid = False
                tmp += "Detail line cannot be empty <br/>"
            End If
        End If

        If IsValid Then
            Dim exist As Integer = 0
            If Not EditOperation Then
                exist = CommonMethods.checkSOExist(Facility, externorder)
            Else
                Dim MyCommand As String = "<ShipmentOrderHeader><ExternOrderKey>" & externorder & "</ExternOrderKey><OrderKey>" & orderkey & "</OrderKey></ShipmentOrderHeader>"
                Dim MyXml As String = "<Message> <Head> <MessageID>0000000003</MessageID> <MessageType>ShipmentOrder</MessageType> <Action>delete</Action> <Sender> 	<User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password>	<SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId></Sender>		<Recipient>			<SystemID>" & Facility & "</SystemID>		</Recipient>	</Head>	<Body><ShipmentOrder> " & MyCommand & "</ShipmentOrder></Body></Message>"
                tmp = CommonMethods.DeleteXml(MyXml, HttpContext.Current.Request.Item("Field_Facility"), "Order", orderkey)
                If tmp <> "" Then Return tmp
            End If
            If exist = 0 Then
                Try
                    Dim Xml As String = "<Message><Head><MessageID>0000000003</MessageID><MessageType>ShipmentOrder</MessageType><Action>store</Action><Sender><User>" & CommonMethods.username & "</User>			<Password>" & CommonMethods.password & "</Password><SystemID>MOVEX</SystemID>	<TenantId>INFOR</TenantId>	</Sender><Recipient><SystemID>" & Facility & "</SystemID></Recipient></Head><Body><ShipmentOrder><ShipmentOrderHeader>" & Command & "</ShipmentOrderHeader></ShipmentOrder></Body></Message>"
                    Dim soapResult As String = CommonMethods.sendwebRequest(Xml)

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
                            tmp = CommonMethods.incremenetKey(Facility, "EXTERNSO")
                            If tmp = "" Then
                                Dim order As String = "", lineno As String = "", logmessage As String = ""

                                Dim nodeList As XmlNodeList = doc.GetElementsByTagName("OrderKey")
                                For Each node As XmlNode In nodeList
                                    order = node.InnerText
                                    logmessage = CommonMethods.logger(Facility, "Order", order, HttpContext.Current.Session("userkey").ToString)
                                Next

                                Dim nodeList2 As XmlNodeList = doc.GetElementsByTagName("OrderLineNumber")
                                For Each node As XmlNode In nodeList2
                                    lineno = node.InnerText
                                    logmessage += CommonMethods.logger(Facility, "OrderDetail", order & "-" & lineno, HttpContext.Current.Session("userkey").ToString)
                                Next

                                If Not String.IsNullOrEmpty(logmessage) Then
                                    tmp = "Logging Error: " + logmessage + "<br />"
                                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                                    logger.Error(logmessage, "", "")
                                End If
                            End If
                        End If
                    End If
                Catch ex As Exception
                    tmp = "Error: " & ex.Message & vbTab + ex.GetType.ToString & "<br/>"
                    Dim logger As Logger = LogManager.GetCurrentClassLogger()
                    logger.Error(ex, "", "")
                End Try
            Else
                tmp = "Error: Extern Order Key already exists!"
            End If
        End If
        Return tmp
    End Function
    Private Function SaveOrderManagement(ByVal MyID As Integer, ByVal DetailsCount As Integer) As String
        Dim tmp As String = "", Command As String = "", CommandDetails As String = "", columns As String = ""
        Dim IsValid As Boolean = True
        Dim EditOperation As Boolean = MyID <> 0
        Dim Facility As String = CommonMethods.getFacilityDBName(HttpContext.Current.Request.Item("Field_Facility")) _
        , externorder As String = HttpContext.Current.Request.Item("Field_ExternOrderKey") _
        , orderkey As String = HttpContext.Current.Request.Item("Field_OrderManagKey") _
        , Owner As String = UCase(HttpContext.Current.Request.Item("Field_StorerKey")) _
        , ConsigneeKey As String = UCase(HttpContext.Current.Request.Item("Field_ConsigneeKey")) _
        , type As String = HttpContext.Current.Request.Item("Field_Type") _
        , orderdate As String = HttpContext.Current.Request.Item("Field_OrderDate") _
        , requestedshipdate As String = HttpContext.Current.Request.Item("Field_RequestedShipDate") _
        , UDF1 As String = HttpContext.Current.Request.Item("Field_SUsr1") _
        , UDF2 As String = HttpContext.Current.Request.Item("Field_SUsr2") _
        , UDF3 As String = HttpContext.Current.Request.Item("Field_SUsr3") _
        , UDF4 As String = HttpContext.Current.Request.Item("Field_SUsr4") _
        , UDF5 As String = HttpContext.Current.Request.Item("Field_SUsr5") _
        , exterline As String = HttpContext.Current.Request.Item("DetailsField_ExternLineNo") _
        , Sku = UCase(HttpContext.Current.Request.Item("DetailsField_Sku")) _
        , OpenQty = HttpContext.Current.Request.Item("DetailsField_OpenQty") _
        , PackKey = HttpContext.Current.Request.Item("DetailsField_PackKey") _
        , UOM = HttpContext.Current.Request.Item("DetailsField_UOM") _
        , UDF1Dtl = HttpContext.Current.Request.Item("DetailsField_SUsr1") _
        , UDF2Dtl = HttpContext.Current.Request.Item("DetailsField_SUsr2") _
        , UDF3Dtl = HttpContext.Current.Request.Item("DetailsField_SUsr3") _
        , UDF4Dtl = HttpContext.Current.Request.Item("DetailsField_SUsr4") _
        , Lottable01 = HttpContext.Current.Request.Item("DetailsField_Lottable01") _
        , Lottable02 = HttpContext.Current.Request.Item("DetailsField_Lottable02") _
        , Lottable03 = HttpContext.Current.Request.Item("DetailsField_Lottable03") _
        , Lottable04 = HttpContext.Current.Request.Item("DetailsField_Lottable04") _
        , Lottable05 = HttpContext.Current.Request.Item("DetailsField_Lottable05") _
        , Lottable06 = HttpContext.Current.Request.Item("DetailsField_Lottable06") _
        , Lottable07 = HttpContext.Current.Request.Item("DetailsField_Lottable07") _
        , Lottable08 = HttpContext.Current.Request.Item("DetailsField_Lottable08") _
        , Lottable09 = HttpContext.Current.Request.Item("DetailsField_Lottable09") _
        , Lottable10 = HttpContext.Current.Request.Item("DetailsField_Lottable10") _
        , Price = HttpContext.Current.Request.Item("DetailsField_Price") _
        , Currency = HttpContext.Current.Request.Item("DetailsField_Currency")


        Dim cmd As SqlCommand = New SqlCommand, cmdDetails As SqlCommand = New SqlCommand
        Dim cmdOracle As OracleCommand = New OracleCommand, cmdOracleDetails As OracleCommand = New OracleCommand

        If Not EditOperation Then
            If Not String.IsNullOrEmpty(Facility) Then
                columns += "WHSEID, SUsr1, SUsr2, SUsr3, SUsr4, SUsr5 "
                If CommonMethods.dbtype = "sql" Then
                    Command += " @facility, @SUsr1, @SUsr2, @SUsr3, @SUsr4, @SUsr5 "
                    cmd.Parameters.AddWithValue("@facility", Facility)
                    cmd.Parameters.AddWithValue("@SUsr1", UDF1)
                    cmd.Parameters.AddWithValue("@SUsr2", UDF2)
                    cmd.Parameters.AddWithValue("@SUsr3", UDF3)
                    cmd.Parameters.AddWithValue("@SUsr4", UDF4)
                    cmd.Parameters.AddWithValue("@SUsr5", UDF5)
                Else
                    Command += " :facility, :SUsr1, :SUsr2, :SUsr3, :SUsr4, :SUsr5 "
                    cmdOracle.Parameters.Add(New OracleParameter("facility", Facility))
                    cmdOracle.Parameters.Add(New OracleParameter("SUsr1", UDF1))
                    cmdOracle.Parameters.Add(New OracleParameter("SUsr2", UDF2))
                    cmdOracle.Parameters.Add(New OracleParameter("SUsr3", UDF3))
                    cmdOracle.Parameters.Add(New OracleParameter("SUsr4", UDF4))
                    cmdOracle.Parameters.Add(New OracleParameter("SUsr5", UDF5))
                End If
            Else
                IsValid = False
                tmp += "Facility must be defined <br/>"
            End If

            If Not String.IsNullOrEmpty(externorder) Then
                columns += ", EXTERNORDERKEY"
                If CommonMethods.dbtype = "sql" Then
                    Command += ", @externokey "
                    cmd.Parameters.AddWithValue("@externokey", externorder)
                Else
                    Command += ", :externokey "
                    cmdOracle.Parameters.Add(New OracleParameter("externokey", externorder))
                End If
            ElseIf IsValid Then
                externorder = CommonMethods.getExternKey(Facility, "EXTERNSO")
                columns += ", EXTERNORDERKEY"
                Command += ",'" & externorder & "'"
            End If

            If Not String.IsNullOrEmpty(orderdate) Then
                Dim datetime As DateTime
                If DateTime.TryParseExact(orderdate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                    Dim datetime2 As DateTime = DateTime.ParseExact(orderdate, "MM/dd/yyyy", Nothing)
                    columns += ", ORDERDATE"
                    orderdate = datetime2.ToString("yyyy-MM-dd HH:mm:ss")
                    If CommonMethods.dbtype = "sql" Then
                        Command += ", @orderdate "
                        cmd.Parameters.AddWithValue("@orderdate", orderdate)
                    Else
                        Command += ",TO_DATE( :orderdate,'yyyy/mm/dd hh24:mi:ss')  "
                        cmdOracle.Parameters.Add(New OracleParameter("orderdate", orderdate))
                    End If
                Else
                    IsValid = False
                    tmp += "Order date doesn't match the required date format <br/>"
                End If
            End If

            If Not String.IsNullOrEmpty(Owner) Then
                Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
                If owners.Any(Function(x) x = Owner) Or owners.Any(Function(x) x = "ALL") Then
                    columns += ", STORERKEY"
                    If CommonMethods.dbtype = "sql" Then
                        Command += ", @Owner "
                        cmd.Parameters.AddWithValue("@Owner", Owner)
                    Else
                        Command += ", :Owner "
                        cmdOracle.Parameters.Add(New OracleParameter("Owner", Owner))
                    End If
                Else
                    IsValid = False
                    tmp += "This owner is not authorized <br/>"
                End If
            Else
                IsValid = False
                tmp += "Owner must be defined <br/>"
            End If

            If Not String.IsNullOrEmpty(type) Then
                Dim DTOrderType = CommonMethods.getCodeDD(Facility, "codelkup", "ORDERTYPE")
                Dim DROrderType() As DataRow = DTOrderType.Select("DESCRIPTION='" & type & "'")
                If DROrderType.Length > 0 Then
                    columns += ", TYPE"
                    If CommonMethods.dbtype = "sql" Then
                        Command += ", @type "
                        cmd.Parameters.AddWithValue("@type", DROrderType(0)!CODE)
                    Else
                        Command += ", :type "
                        cmdOracle.Parameters.Add(New OracleParameter("type", DROrderType(0)!CODE))
                    End If
                Else
                    IsValid = False
                    tmp += "Type must be defined <br/>"
                End If
            Else
                IsValid = False
                tmp += "Type must be defined <br/>"
            End If

            If Not String.IsNullOrEmpty(ConsigneeKey) Then
                Dim consignees As String() = CommonMethods.getConsigneePerUser(HttpContext.Current.Session("userkey").ToString)
                If consignees.Any(Function(x) x = ConsigneeKey) Or consignees.Any(Function(x) x = "ALL") Then
                    columns += ", ConsigneeKey"
                    If CommonMethods.dbtype = "sql" Then
                        Command += ", @consignee "
                        cmd.Parameters.AddWithValue("@consignee", ConsigneeKey)
                    Else
                        Command += ", :consignee "
                        cmdOracle.Parameters.Add(New OracleParameter("consignee", ConsigneeKey))
                    End If
                Else
                    IsValid = False
                    tmp += "This consignee is not authorized <br/>"
                End If
            End If

            If Not String.IsNullOrEmpty(requestedshipdate) Then
                Dim datetime As DateTime
                If DateTime.TryParseExact(requestedshipdate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                    Dim datetime2 As DateTime = DateTime.ParseExact(requestedshipdate, "MM/dd/yyyy", Nothing)
                    columns += ", RequestedShipDate"
                    requestedshipdate = datetime2.ToString("yyyy-MM-dd HH:mm:ss")
                    If CommonMethods.dbtype = "sql" Then
                        Command += ", @Reqdate "
                        cmd.Parameters.AddWithValue("@Reqdate", requestedshipdate)
                    Else
                        Command += ",TO_DATE( :Reqdate,'yyyy/mm/dd hh24:mi:ss')  "
                        cmdOracle.Parameters.Add(New OracleParameter("Reqdate", requestedshipdate))
                    End If
                Else
                    IsValid = False
                    tmp += "Requested ship date doesn't match the required date format <br/>"
                End If
            End If
        Else
            If CommonMethods.dbtype = "sql" Then
                Command += "  SUsr1 = @SUsr1, SUsr2 = @SUsr2, SUsr3 = @SUsr3, SUsr4 = @SUsr4, SUsr5 = @SUsr5 "
                cmd.Parameters.AddWithValue("@SerialKey", MyID)
                cmd.Parameters.AddWithValue("@SUsr1", UDF1)
                cmd.Parameters.AddWithValue("@SUsr2", UDF2)
                cmd.Parameters.AddWithValue("@SUsr3", UDF3)
                cmd.Parameters.AddWithValue("@SUsr4", UDF4)
                cmd.Parameters.AddWithValue("@SUsr5", UDF5)
            Else
                Command += "  SUsr1 = :SUsr1, SUsr2 = :SUsr2, SUsr3 = :SUsr3, SUsr4 = :SUsr4, SUsr5 = :SUsr5 "
                cmdOracle.Parameters.Add(New OracleParameter(":SerialKey", UDF1))
                cmdOracle.Parameters.Add(New OracleParameter("SUsr1", UDF1))
                cmdOracle.Parameters.Add(New OracleParameter("SUsr2", UDF2))
                cmdOracle.Parameters.Add(New OracleParameter("SUsr3", UDF3))
                cmdOracle.Parameters.Add(New OracleParameter("SUsr4", UDF4))
                cmdOracle.Parameters.Add(New OracleParameter("SUsr5", UDF5))
            End If

            If Not String.IsNullOrEmpty(ConsigneeKey) Then
                Dim consignees As String() = CommonMethods.getConsigneePerUser(HttpContext.Current.Session("userkey").ToString)
                If consignees.Any(Function(x) x = ConsigneeKey) Or consignees.Any(Function(x) x = "ALL") Then
                    If CommonMethods.dbtype = "sql" Then
                        Command += ", ConsigneeKey = @consignee "
                        cmd.Parameters.AddWithValue("@consignee", ConsigneeKey)
                    Else
                        Command += ", ConsigneeKey = :consignee "
                        cmdOracle.Parameters.Add(New OracleParameter("consignee", ConsigneeKey))
                    End If
                Else
                    IsValid = False
                    tmp += "This consignee is not authorized <br/>"
                End If
            End If

            If Not String.IsNullOrEmpty(requestedshipdate) Then
                Dim datetime As DateTime
                If DateTime.TryParseExact(requestedshipdate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                    Dim datetime2 As DateTime = DateTime.ParseExact(requestedshipdate, "MM/dd/yyyy", Nothing)
                    requestedshipdate = datetime2.ToString("yyyy-MM-dd HH:mm:ss")
                    If CommonMethods.dbtype = "sql" Then
                        Command += ", RequestedShipDate = @Reqdate "
                        cmd.Parameters.AddWithValue("@Reqdate", requestedshipdate)
                    Else
                        Command += ", RequestedShipDate = TO_DATE( :Reqdate,'yyyy/mm/dd hh24:mi:ss')  "
                        cmdOracle.Parameters.Add(New OracleParameter("Reqdate", requestedshipdate))
                    End If
                Else
                    IsValid = False
                    tmp += "Requested ship date doesn't match the required date format <br/>"
                End If
            End If
        End If

        If IsValid Then
            If DetailsCount > 0 Then
                Dim exterlineArr() As String = Nothing, SkuArr() As String = Nothing, OpenQtyArr() As String = Nothing,
                    PackKeyArr() As String = Nothing, UOMArr() As String = Nothing, UDF1DtlArr() As String = Nothing,
                    UDF2DtlArr() As String = Nothing, UDF3DtlArr() As String = Nothing, UDF4DtlArr() As String = Nothing,
                    Lottable01Arr() As String = Nothing, Lottable02Arr() As String = Nothing,
                    Lottable03Arr() As String = Nothing, Lottable04Arr() As String = Nothing,
                    Lottable05Arr() As String = Nothing, Lottable06Arr() As String = Nothing,
                    Lottable07Arr() As String = Nothing, Lottable08Arr() As String = Nothing,
                    Lottable09Arr() As String = Nothing, Lottable10Arr() As String = Nothing,
                    PriceArr() As String = Nothing, CurrencyArr() As String = Nothing

                If exterline IsNot Nothing Then exterlineArr = exterline.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Sku IsNot Nothing Then SkuArr = Sku.Split(New String() {"~~~"}, StringSplitOptions.None)
                If OpenQty IsNot Nothing Then OpenQtyArr = OpenQty.Split(New String() {"~~~"}, StringSplitOptions.None)
                If PackKey IsNot Nothing Then PackKeyArr = PackKey.Split(New String() {"~~~"}, StringSplitOptions.None)
                If UOM IsNot Nothing Then UOMArr = UOM.Split(New String() {"~~~"}, StringSplitOptions.None)
                If UDF1Dtl IsNot Nothing Then UDF1DtlArr = UDF1Dtl.Split(New String() {"~~~"}, StringSplitOptions.None)
                If UDF2Dtl IsNot Nothing Then UDF2DtlArr = UDF2Dtl.Split(New String() {"~~~"}, StringSplitOptions.None)
                If UDF3Dtl IsNot Nothing Then UDF3DtlArr = UDF3Dtl.Split(New String() {"~~~"}, StringSplitOptions.None)
                If UDF4Dtl IsNot Nothing Then UDF4DtlArr = UDF4Dtl.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable01 IsNot Nothing Then Lottable01Arr = Lottable01.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable02 IsNot Nothing Then Lottable02Arr = Lottable02.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable03 IsNot Nothing Then Lottable03Arr = Lottable03.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable04 IsNot Nothing Then Lottable04Arr = Lottable04.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable05 IsNot Nothing Then Lottable05Arr = Lottable05.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable06 IsNot Nothing Then Lottable06Arr = Lottable06.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable07 IsNot Nothing Then Lottable07Arr = Lottable07.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable08 IsNot Nothing Then Lottable08Arr = Lottable08.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable09 IsNot Nothing Then Lottable09Arr = Lottable09.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Lottable10 IsNot Nothing Then Lottable10Arr = Lottable10.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Price IsNot Nothing Then PriceArr = Price.Split(New String() {"~~~"}, StringSplitOptions.None)
                If Currency IsNot Nothing Then CurrencyArr = Currency.Split(New String() {"~~~"}, StringSplitOptions.None)

                If exterlineArr.Length > 1 Then
                    For i = 0 To exterlineArr.Length - 1
                        If Not exterlineArr(i) Is Nothing And Not String.IsNullOrEmpty(exterlineArr(i)) Then
                            If Array.LastIndexOf(exterlineArr, exterlineArr(i)) <> i Then
                                tmp = "Duplicate Extern Line# " & exterlineArr(i) & " on line " & (i + 1).ToString
                                Return tmp
                            End If
                        End If
                    Next
                End If

                If SkuArr.Length = DetailsCount Then
                    For i As Integer = 0 To SkuArr.Length - 1
                        If Not SkuArr(i) Is Nothing Then
                            If Array.LastIndexOf(SkuArr, SkuArr(i)) <> i Then
                                tmp = "Duplicate Item " & SkuArr(i) & " on line " & (i + 1).ToString
                                Return tmp
                            End If
                        End If
                    Next
                End If

                Dim linenumber As Integer = 0
                Dim linenb As String = ""
                For i = 0 To DetailsCount - 1
                    CommandDetails += " insert into " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & " ORDERMANAGDETAIL "
                    CommandDetails += " (WHSEID, EXTERNORDERKEY, ORDERMANAGKEY, orderkey, OrderLineNumber, ORDERMANAGLINENUMBER, "
                    CommandDetails += "  StorerKey, ExternLineNo, Sku, OpenQty, PackKey, UOM, SUsr1, SUsr2, SUsr3, SUsr4, "
                    CommandDetails += " Lottable01, Lottable02, Lottable03, Lottable04, Lottable05, Lottable06, "
                    CommandDetails += " Lottable07, Lottable08, Lottable09, Lottable10, UNITPRICE, SUsr5) values "

                    linenumber += 1
                    linenb = linenumber.ToString.PadLeft(5, "0")

                    If CommonMethods.dbtype = "sql" Then
                        CommandDetails += "(@warehouse" & i & " ,@externorderkey" & i
                        If Not EditOperation Then
                            CommandDetails += ",(select ORDERMANAGKEY from  ORDERMANAG where WHSEID=@warehouse" & i & "  and EXTERNORDERKEY=@externorderkey" & i & "),(select ORDERMANAGKEY from ORDERMANAG where WHSEID=@warehouse" & i & "  and EXTERNORDERKEY=@externorderkey" & i & ")"
                        Else
                            CommandDetails += ",@ordermanagkey" & i & ", @orderkey" & i
                            cmdDetails.Parameters.AddWithValue("@ordermanagkey" & i, orderkey)
                            cmdDetails.Parameters.AddWithValue("@orderkey" & i, orderkey)
                        End If
                        CommandDetails += ",@linenb" & i & ", @linenb" & i & ", @owner" & i
                        cmdDetails.Parameters.AddWithValue("@warehouse" & i, Facility)
                        cmdDetails.Parameters.AddWithValue("@externorderkey" & i, externorder)
                        cmdDetails.Parameters.AddWithValue("@linenb" & i, linenb)
                        cmdDetails.Parameters.AddWithValue("@owner" & i, Owner)
                    Else
                        CommandDetails += "(:warehouse" & i & " ,:externorderkey" & i
                        If Not EditOperation Then
                            CommandDetails += ",(select ORDERMANAGKEY from  ORDERMANAG where WHSEID=:warehouse" & i & "  and EXTERNORDERKEY=:externorderkey" & i & "),(select ORDERMANAGKEY from ORDERMANAG where WHSEID=:warehouse" & i & "  and EXTERNORDERKEY=:externorderkey" & i & ")"
                        Else
                            CommandDetails += ",:ordermanagkey" & i & ", :orderkey" & i
                            cmdDetails.Parameters.AddWithValue("ordermanagkey" & i, orderkey)
                            cmdDetails.Parameters.AddWithValue("orderkey" & i, orderkey)
                        End If
                        CommandDetails += ",:linenb" & i & ", :linenb" & i & ", :owner" & i
                        cmdOracleDetails.Parameters.Add(New OracleParameter("warehouse" & i, Facility))
                        cmdOracleDetails.Parameters.Add(New OracleParameter("externorderkey" & i, externorder))
                        cmdOracleDetails.Parameters.Add(New OracleParameter("linenb" & i, linenb))
                        cmdOracleDetails.Parameters.Add(New OracleParameter("owner" & i, Owner))
                    End If

                    Try
                        If Not String.IsNullOrEmpty(exterlineArr(i)) Then
                            exterline = exterlineArr(i)
                        Else
                            exterline = (i + 1).ToString.PadLeft(5, "0")
                        End If
                    Catch ex As Exception
                        exterline = (i + 1).ToString.PadLeft(5, "0")
                    End Try
                    If CommonMethods.dbtype = "sql" Then
                        CommandDetails += " , @exterline" & i
                        cmdDetails.Parameters.AddWithValue("@exterline" & i, exterline)
                    Else
                        CommandDetails += " , :exterline" & i
                        cmdOracleDetails.Parameters.Add(New OracleParameter("exterline" & i, exterline))
                    End If

                    Try
                        If Not String.IsNullOrEmpty(SkuArr(i)) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @Sku" & i
                                cmdDetails.Parameters.AddWithValue("@Sku" & i, SkuArr(i))
                            Else
                                CommandDetails += " , :Sku" & i
                                cmdOracleDetails.Parameters.Add(New OracleParameter("Sku" & i, SkuArr(i)))
                            End If
                        Else
                            IsValid = False
                            tmp += "Item cannot be empty on line " & (i + 1).ToString & " <br/>"
                            Exit For
                        End If
                    Catch ex As Exception
                        IsValid = False
                        tmp += "Item cannot be empty on line " & (i + 1).ToString & " <br/>"
                        Exit For
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(OpenQtyArr(i)) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @OpenQty" & i
                                cmdDetails.Parameters.AddWithValue("@OpenQty" & i, OpenQtyArr(i))
                            Else
                                CommandDetails += " , :OpenQty" & i
                                cmdOracleDetails.Parameters.Add(New OracleParameter("OpenQty" & i, OpenQtyArr(i)))
                            End If
                        Else
                            IsValid = False
                            tmp += "Open Qty cannot be empty on line " & (i + 1).ToString & "<br/>"
                            Exit For
                        End If
                    Catch ex As Exception
                        IsValid = False
                        tmp += "Open Qty cannot be empty on line " & (i + 1).ToString & "<br/>"
                        Exit For
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(PackKeyArr(i)) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @pack" & i
                                cmdDetails.Parameters.AddWithValue("@pack" & i, PackKeyArr(i))
                            Else
                                CommandDetails += " , :pack" & i
                                cmdOracleDetails.Parameters.Add(New OracleParameter("pack" & i, PackKeyArr(i)))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(UOMArr(i)) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @uom" & i
                                cmdDetails.Parameters.AddWithValue("@uom" & i, UOMArr(i))
                            Else
                                CommandDetails += " , :uom" & i
                                cmdOracleDetails.Parameters.Add(New OracleParameter("uom" & i, UOMArr(i)))
                            End If
                        Else
                            CommandDetails += " ,'EA'"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,'EA'"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(UDF1DtlArr(i)) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @SUsr1" & i
                                cmdDetails.Parameters.AddWithValue("@SUsr1" & i, UDF1DtlArr(i))
                            Else
                                CommandDetails += " , :SUsr1" & i
                                cmdOracleDetails.Parameters.Add(New OracleParameter("SUsr1" & i, UDF1DtlArr(i)))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(UDF2DtlArr(i)) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @SUsr2" & i
                                cmdDetails.Parameters.AddWithValue("@SUsr2" & i, UDF2DtlArr(i))
                            Else
                                CommandDetails += " , :SUsr2" & i
                                cmdOracleDetails.Parameters.Add(New OracleParameter("SUsr2" & i, UDF2DtlArr(i)))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(UDF3DtlArr(i)) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @SUsr3" & i
                                cmdDetails.Parameters.AddWithValue("@SUsr3" & i, UDF3DtlArr(i))
                            Else
                                CommandDetails += " , :SUsr3" & i
                                cmdOracleDetails.Parameters.Add(New OracleParameter("SUsr3" & i, UDF3DtlArr(i)))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(UDF4DtlArr(i)) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @SUsr4" & i
                                cmdDetails.Parameters.AddWithValue("@SUsr4" & i, UDF4DtlArr(i))
                            Else
                                CommandDetails += " , :SUsr4" & i
                                cmdOracleDetails.Parameters.Add(New OracleParameter("SUsr4" & i, UDF4DtlArr(i)))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable01Arr(i)) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @lot1" & i
                                cmdDetails.Parameters.AddWithValue("@lot1" & i, Lottable01Arr(i))
                            Else
                                CommandDetails += " , :lot1" & i
                                cmdOracleDetails.Parameters.Add(New OracleParameter("lot1" & i, Lottable01Arr(i)))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable02Arr(i)) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @lot2" & i
                                cmdDetails.Parameters.AddWithValue("@lot2" & i, Lottable02Arr(i))
                            Else
                                CommandDetails += " , :lot2" & i
                                cmdOracleDetails.Parameters.Add(New OracleParameter("lot2" & i, Lottable02Arr(i)))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable03Arr(i)) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @lot3" & i
                                cmdDetails.Parameters.AddWithValue("@lot3" & i, Lottable03Arr(i))
                            Else
                                CommandDetails += " , :lot3" & i
                                cmdOracleDetails.Parameters.Add(New OracleParameter("lot3" & i, Lottable03Arr(i)))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable04Arr(i)) Then
                            Dim datetime As DateTime
                            If DateTime.TryParseExact(Lottable04Arr(i), "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                                Dim datetime2 As DateTime = DateTime.ParseExact(Lottable04Arr(i), "MM/dd/yyyy", Nothing)
                                If CommonMethods.dbtype = "sql" Then
                                    CommandDetails += " , @lot4" & i
                                Else
                                    CommandDetails += " , :lot4" & i
                                End If
                                If Double.Parse(CommonMethods.version) >= 11 Then
                                    If CommonMethods.dbtype = "sql" Then
                                        cmdDetails.Parameters.AddWithValue("@lot4" & i, datetime2.ToString("MM/dd/yyyy 14:00:00"))
                                    Else
                                        cmdOracleDetails.Parameters.Add(New OracleParameter("lot4" & i, datetime2.ToString("MM/dd/yyyy 14:00:00")))
                                    End If
                                Else
                                    If CommonMethods.dbtype = "sql" Then
                                        cmdDetails.Parameters.AddWithValue("@lot4" & i, datetime2.ToString("dd/MM/yyyy 14:00:00"))
                                    Else
                                        cmdOracleDetails.Parameters.Add(New OracleParameter("lot4" & i, datetime2.ToString("dd/MM/yyyy 14:00:00")))
                                    End If
                                End If
                            Else
                                IsValid = False
                                tmp += "Lottable04 doesn't match the required date format one line " & (i + 1).ToString & " <br/>"
                                Exit For
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable05Arr(i)) Then
                            Dim datetime As DateTime
                            If DateTime.TryParseExact(Lottable05Arr(i), "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datetime) Then
                                Dim datetime2 As DateTime = DateTime.ParseExact(Lottable05Arr(i), "MM/dd/yyyy", Nothing)
                                If CommonMethods.dbtype = "sql" Then
                                    CommandDetails += " , @lot5" & i
                                Else
                                    CommandDetails += " , :lot5" & i
                                End If
                                If Double.Parse(CommonMethods.version) >= 11 Then
                                    If CommonMethods.dbtype = "sql" Then
                                        cmdDetails.Parameters.AddWithValue("@lot5" & i, datetime2.ToString("MM/dd/yyyy 14:00:00"))
                                    Else
                                        cmdOracleDetails.Parameters.Add(New OracleParameter("lot5" & i, datetime2.ToString("MM/dd/yyyy 14:00:00")))
                                    End If
                                Else
                                    If CommonMethods.dbtype = "sql" Then
                                        cmdDetails.Parameters.AddWithValue("@lot5" & i, datetime2.ToString("dd/MM/yyyy 14:00:00"))
                                    Else
                                        cmdOracleDetails.Parameters.Add(New OracleParameter("lot5" & i, datetime2.ToString("dd/MM/yyyy 14:00:00")))
                                    End If
                                End If
                            Else
                                IsValid = False
                                tmp += "Lottable05 doesn't match the required date format one line " & (i + 1).ToString & " <br/>"
                                Exit For
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable06Arr(i)) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @lot6" & i
                                cmdDetails.Parameters.AddWithValue("@lot6" & i, Lottable06Arr(i))
                            Else
                                CommandDetails += " , :lot6" & i
                                cmdOracleDetails.Parameters.Add(New OracleParameter("lot6" & i, Lottable06Arr(i)))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable07Arr(i)) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @lot7" & i
                                cmdDetails.Parameters.AddWithValue("@lot7" & i, Lottable07Arr(i))
                            Else
                                CommandDetails += " , :lot7" & i
                                cmdOracleDetails.Parameters.Add(New OracleParameter("lot7" & i, Lottable07Arr(i)))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable08Arr(i)) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @lot8" & i
                                cmdDetails.Parameters.AddWithValue("@lot8" & i, Lottable08Arr(i))
                            Else
                                CommandDetails += " , :lot8" & i
                                cmdOracleDetails.Parameters.Add(New OracleParameter("lot8" & i, Lottable08Arr(i)))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable09Arr(i)) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @lot9" & i
                                cmdDetails.Parameters.AddWithValue("@lot9" & i, Lottable09Arr(i))
                            Else
                                CommandDetails += " , :lot9" & i
                                cmdOracleDetails.Parameters.Add(New OracleParameter("lot9" & i, Lottable09Arr(i)))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(Lottable10Arr(i)) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @lot10" & i
                                cmdDetails.Parameters.AddWithValue("@lot10" & i, Lottable10Arr(i))
                            Else
                                CommandDetails += " , :lot10" & i
                                cmdOracleDetails.Parameters.Add(New OracleParameter("lot10" & i, Lottable10Arr(i)))
                            End If
                        Else
                            CommandDetails += " ,''"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,''"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(PriceArr(i)) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @unitprice" & i
                                cmdDetails.Parameters.AddWithValue("@unitprice" & i, Double.Parse(PriceArr(i)))
                            Else
                                CommandDetails += " , :unitprice" & i
                                cmdOracleDetails.Parameters.Add(New OracleParameter("unitprice" & i, Double.Parse(PriceArr(i))))
                            End If
                        Else
                            CommandDetails += " , 0"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,0"
                    End Try

                    Try
                        If Not String.IsNullOrEmpty(CurrencyArr(i)) Then
                            If CommonMethods.dbtype = "sql" Then
                                CommandDetails += " , @currency" & i & ")"
                                cmdDetails.Parameters.AddWithValue("@currency" & i, CurrencyArr(i))
                            Else
                                CommandDetails += " , :currency" & i & " from dual union all"
                                cmdOracleDetails.Parameters.Add(New OracleParameter("currency" & i, CurrencyArr(i)))
                            End If
                        Else
                            CommandDetails += " ,'')"
                        End If
                    Catch ex As Exception
                        CommandDetails += " ,'')"
                    End Try

                Next
            Else
                IsValid = False
                tmp += "Detail line cannot be empty <br/>"
            End If
        End If

        If IsValid Then
            Dim exist As Integer = 0
            If Not EditOperation Then
                exist = CommonMethods.checkOrderManagementExist(Facility, externorder)
            Else
                Dim SqlDelete As String = ""
                SqlDelete += " delete from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ordermanagdetail where WHSEID= '" & Facility & "'  and EXTERNORDERKEY = '" & externorder & "' "
                Dim tmp2 As String = (New SQLExec).Execute(SqlDelete)
                If tmp2 <> "" Then
                    tmp += tmp2
                    Return tmp
                End If
            End If
            If exist = 0 Then
                If CommonMethods.dbtype = "sql" Then
                    Try
                        Dim query As String = ""
                        If EditOperation Then
                            query += " update dbo.ORDERMANAG set " & Command & " where SerialKey = @SerialKey "
                        Else
                            query += " insert into dbo.ORDERMANAG (ORDERMANAGKEY, ORDERKEY, ORDERMANAGSTATUS," & columns & " )values((select  (RIGHT('0000000000' + CAST(ISNULL(max(ORDERMANAGKEY)+1,1) AS varchar(10)) , 10) ) from dbo.ORDERMANAG), (select  (RIGHT('0000000000' + CAST(ISNULL(max(ORDERMANAGKEY)+1,1) AS varchar(10)) , 10) ) from dbo.ORDERMANAG),'NOT CREATED in SCE', " & Command & ");"
                        End If

                        Dim conn As SqlConnection = New SqlConnection(CommonMethods.dbconx)
                        conn.Open()
                        cmd.CommandText = query
                        cmd.Connection = conn
                        cmd.ExecuteNonQuery()
                        conn.Close()
                        If Not EditOperation Then CommonMethods.incremenetKey(Facility, "EXTERNSO")

                        conn = New SqlConnection(CommonMethods.dbconx)
                        conn.Open()
                        cmdDetails.CommandText = CommandDetails
                        cmdDetails.Connection = conn
                        cmdDetails.ExecuteNonQuery()
                        conn.Close()
                    Catch ex As Exception
                        IsValid = False
                        tmp += "Error: " & ex.Message & vbTab + ex.GetType.ToString & "<br/>"
                        Dim logger As Logger = LogManager.GetCurrentClassLogger()
                        logger.Error(ex, "", "")
                    End Try
                Else
                    Try
                        Dim query As String = ""
                        If EditOperation Then
                            query += " update dbo.ORDERMANAG set " & Command & " where SerialKey = :SerialKey "
                        Else
                            query += " insert into  SYSTEM.ORDERMANAG (ORDERMANAGKEY, ORDERKEY, ORDERMANAGSTATUS," & columns & " )values((select  (SUBSTR(CONCAT('000000000' , CAST(NVL(max(ORDERMANAGKEY)+1,1) AS nvarchar2(10))) , -10) ) from SYSTEM.ORDERMANAG), (select  (SUBSTR(CONCAT('000000000' , CAST(NVL(max(ORDERMANAGKEY)+1,1) AS nvarchar2(10))) , -10) ) from SYSTEM.ORDERMANAG),'NOT CREATED in SCE', " & Command & ")"
                        End If

                        Dim conn As OracleConnection = New OracleConnection(CommonMethods.dbconx)
                        conn.Open()
                        cmdOracle.CommandText = query
                        cmdOracle.Connection = conn
                        cmdOracle.ExecuteNonQuery()
                        conn.Close()
                        If Not EditOperation Then CommonMethods.incremenetKey(Facility, "EXTERNSO")

                        conn = New OracleConnection(CommonMethods.dbconx)
                        conn.Open()
                        cmdOracleDetails.CommandText = CommandDetails
                        cmdOracleDetails.Connection = conn
                        cmdOracleDetails.ExecuteNonQuery()
                        conn.Close()
                    Catch ex As Exception
                        IsValid = False
                        tmp += "Error: " & ex.Message & vbTab + ex.GetType.ToString & "<br/>"
                        Dim logger As Logger = LogManager.GetCurrentClassLogger()
                        logger.Error(ex, "", "")
                    End Try
                End If
                If Not IsValid Then
                    Dim SqlDelete As String = ""
                    SqlDelete += " delete from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ordermanagdetail where WHSEID= '" & Facility & "'  and EXTERNORDERKEY = '" & externorder & "' "
                    SqlDelete += " delete from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ordermanag where WHSEID= '" & Facility & "'  and EXTERNORDERKEY = '" & externorder & "' "
                    Dim tmp2 As String = (New SQLExec).Execute(SqlDelete)
                    If tmp2 <> "" Then tmp += tmp2
                End If
            Else
                tmp = "Error: Extern Order Key already exists!"
            End If
        End If
        Return tmp
    End Function

    Private Function GetMyID(ByVal SearchTable As String, ByVal StrID As String) As Integer
        Dim MyID As Integer = 0
        Dim AndFilter As String = ""
        Dim sql As String = ""
        Dim tb As SQLExec = New SQLExec
        Dim ds As DataSet = Nothing
        Select Case SearchTable
            Case "PORTALUSERS", "SKUCATALOGUE"
                MyID = StrID.Split("=")(1)
            Case "USERCONTROL"
                sql += "Select " & IIf(CommonMethods.dbtype = "sql", "ID", "SerialKey") & " From " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & SearchTable & " where UserKey = '" & StrID.Split("=")(1) & "'"
            Case "enterprise.storer2", "enterprise.storer5"
                sql += "Select SerialKey from " & SearchTable.Remove(SearchTable.Length - 1) & " where StorerKey = '" & StrID.Split("=")(1) & "' and Type = " & SearchTable(SearchTable.Length - 1)
            Case "enterprise.sku"
                sql += "Select SerialKey from " & SearchTable & " where StorerKey = '" & StrID.Split("=")(1).Split("&")(0) & "' and Sku = '" & StrID.Split("=")(2) & "'"
            Case "Warehouse_PO", "Warehouse_ASN", "Warehouse_SO", "Warehouse_OrderManagement"
                Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
                Dim suppliers As String() = CommonMethods.getSupplierPerUser(HttpContext.Current.Session("userkey").ToString)
                Dim consignees As String() = CommonMethods.getConsigneePerUser(HttpContext.Current.Session("userkey").ToString)
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
                        sql = "Select SerialKey from " & IIf(CommonMethods.dbtype <> "sql", "SYSTEM.", "") & "ORDERMANAG where WHSEID = '" & warehouselevel & "' and ExternOrderKey = "
                    End If
                    sql += "'" & StrID.Split("=")(2) & "'" & AndFilter
                End If
        End Select

        If sql <> "" Then
            ds = tb.Cursor(sql)
            If ds.Tables(0).Rows.Count > 0 Then MyID = ds.Tables(0).Rows(0)(0)
        End If

        Return Val(MyID)
    End Function
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class