
Public Class Cufex_Site
    Inherits MultiLingualMasterPage
    'for method 1
    Private Shared ReadOnly Regex1 As New Regex(">(?! )\s+", RegexOptions.Compiled)
    Private Shared ReadOnly Regex2 As New Regex("([\n\s])+?(?<= {2,})<", RegexOptions.Compiled)

    'for method 2
    'Private Shared ReadOnly REGEX_BETWEEN_TAGS As New Regex(">\s+<", RegexOptions.Compiled)
    'Private Shared ReadOnly REGEX_LINE_BREAKS As New Regex("\n\s+", RegexOptions.Compiled)
    Protected Overrides Sub Render(writer As HtmlTextWriter)
        Using htmlwriter As New HtmlTextWriter(New System.IO.StringWriter())
            MyBase.Render(htmlwriter)
            Dim html As String = htmlwriter.InnerWriter.ToString()

            If (ConfigurationManager.AppSettings.[Get]("RemoveWhitespace") + String.Empty).Equals("true", StringComparison.OrdinalIgnoreCase) Then
                'for method 1
                html = Regex1.Replace(html, ">")
                html = Regex2.Replace(html, "<")

                'for method 2
                'html = REGEX_BETWEEN_TAGS.Replace(html, "> <")
                'html = REGEX_LINE_BREAKS.Replace(html, String.Empty)
                'for method 3
                html = Regex.Replace(html, "(?<=[^])\t{2,}|(?<=[>])\s{2,}(?=[<])|(?<=[>])\s{2,11}(?=[<])|(?=[\n])\s{2,}", String.Empty)
                'html = Regex.Replace(html, "[ \f\r\t\v]?([\n\xFE\xFF/{}[\],<>*%&|^!~?:=])[\f\r\t\v]?", "$1")
                html = html.Replace(";" & vbLf, ";")
            End If

            writer.Write(html.Trim())
        End Using
    End Sub
    Enum SectionName
        NothingSelected
        LogIn
        ForgetPassword
        ResetPassword
        Home
        Home_Def
        Security
        Configuration
        Warehouse
        Inventory
        Reporting
    End Enum
    Public Property section As SectionName
        Get
            If ViewState("Section") Is Nothing Then ViewState("Section") = 0
            section = ViewState("Section")
        End Get
        Set(ByVal value As SectionName)
            TableNotloggedIn.Visible = value <> SectionName.LogIn And value <> SectionName.ForgetPassword And value <> SectionName.ResetPassword
            ViewState("Section") = value
            MenuLevel = 0
        End Set
    End Property
    Enum SubSectionName
        NothingSelected
        Security_ChangePassword
        Security_Users
        Security_UsersControl
        Security_UserProfile
        Security_Profiles
        Configuration_ShipTo
        Configuration_ShipFrom
        Configuration_Items
        Configuration_ItemCatalogue
        Warehouse_PO
        Warehouse_ASN
        Warehouse_Shipment
        Warehouse_OrderManagement
        Inventory_Balance
        Reporting_ViewReports
    End Enum
    Public Property Subsection As SubSectionName
        Get
            If ViewState("Subsection") Is Nothing Then ViewState("Subsection") = 0
            Subsection = ViewState("Subsection")
        End Get
        Set(ByVal value As SubSectionName)
            ViewState("Subsection") = value
            'Subsection2 = SubSectionName2.NothingSelected
            MenuLevel = 1
        End Set
    End Property
    Public Property MenuLevel() As Integer
        Get
            If ViewState("MenuLevel") = Nothing Then ViewState("MenuLevel") = 0
            MenuLevel = ViewState("MenuLevel")
        End Get
        Set(ByVal value As Integer)
            ViewState("MenuLevel") = value
        End Set
    End Property
    Public ReadOnly Property LoggedIn() As Boolean
        Get
            If Session("BLog") Is Nothing Then Session("BLog") = 0
            LoggedIn = Session("BLog") = 1
        End Get
    End Property
    Public Property BUserCode As Long
        Get
            If ViewState("BUserCode") Is Nothing Then
                ViewState("BUserCode") = Val(Session("BUserCode"))
            Else
                If Val(ViewState("BUserCode")) = 0 Then ViewState("BUserCode") = Val(Session("BUserCode"))
            End If
            BUserCode = ViewState("BUserCode")
        End Get
        Set(ByVal value As Long)
            ViewState("BUserCode") = Val(value)
        End Set
    End Property
    Public Property BUserGroup As Long
        Get
            If ViewState("BUserGroup") Is Nothing Then
                ViewState("BUserGroup") = Val(Session("BUserGroup"))
            Else
                If Val(ViewState("BUserGroup")) = 0 Then ViewState("BUserGroup") = Val(Session("BUserGroup"))
            End If
            BUserGroup = ViewState("BUserGroup")
        End Get
        Set(ByVal value As Long)
            ViewState("BUserGroup") = Val(value)
        End Set
    End Property
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not section = SectionName.LogIn And Not section = SectionName.ForgetPassword And Not section = SectionName.ResetPassword Then
            CheckIfLogged()
            UserInfo.LoginUser = HttpContext.Current.Session("userkey").ToString
            If Not Page.IsPostBack Then
                FixMenuVisibleItems()
            End If

            If Not section = SectionName.Home Then
                If Not Page.IsPostBack Then
                    DivMain.Visible = CanView
                    DivDenied.Visible = Not CanView
                    If CanView Then
                        If CType(Cufex_MainContent.FindControl("btnQuickEntry"), HtmlAnchor) IsNot Nothing Then
                            CType(Cufex_MainContent.FindControl("btnQuickEntry"), HtmlAnchor).Visible = CanAddNew
                        End If

                        If CType(Cufex_MainContent.FindControl("btnAddNew"), HtmlAnchor) IsNot Nothing Then
                            CType(Cufex_MainContent.FindControl("btnAddNew"), HtmlAnchor).Visible = CanAddNew
                        End If

                        If CType(Cufex_MainContent.FindControl("btnNew"), HtmlAnchor) IsNot Nothing Then
                            CType(Cufex_MainContent.FindControl("btnNew"), HtmlAnchor).Visible = CanAddNew
                        End If

                        If CType(Cufex_MainContent.FindControl("btnSave"), HtmlGenericControl) IsNot Nothing Then
                            CType(Cufex_MainContent.FindControl("btnSave"), HtmlGenericControl).Visible = CanSave
                        End If

                        If CType(Cufex_MainContent.FindControl("btnSaveHeader"), HtmlAnchor) IsNot Nothing Then
                            CType(Cufex_MainContent.FindControl("btnSaveHeader"), HtmlAnchor).Visible = CanSave
                        End If

                        If CType(Cufex_MainContent.FindControl("btnSaveDetail"), HtmlAnchor) IsNot Nothing Then
                            CType(Cufex_MainContent.FindControl("btnSaveDetail"), HtmlAnchor).Visible = CanSave
                        End If

                        If CType(Cufex_MainContent.FindControl("TableAction1"), HtmlTableCell) IsNot Nothing Then
                            CType(Cufex_MainContent.FindControl("TableAction1"), HtmlTableCell).Visible = CanSave
                        End If

                        If CType(Cufex_MainContent.FindControl("TableAction2"), HtmlTableCell) IsNot Nothing Then
                            CType(Cufex_MainContent.FindControl("TableAction2"), HtmlTableCell).Visible = CanSave
                        End If

                        If CType(Cufex_MainContent.FindControl("btnDelete"), HtmlAnchor) IsNot Nothing Then
                            CType(Cufex_MainContent.FindControl("btnDelete"), HtmlAnchor).Visible = CanDelete
                        End If

                        If CType(Cufex_MainContent.FindControl("btnDeleteDetail"), HtmlAnchor) IsNot Nothing Then
                            CType(Cufex_MainContent.FindControl("btnDeleteDetail"), HtmlAnchor).Visible = CanDelete
                        End If

                        If Not CanSearch Then
                            If CType(Cufex_MainContent.FindControl("SearchRow"), HtmlTableRow) IsNot Nothing Then
                                CType(Cufex_MainContent.FindControl("SearchRow"), HtmlTableRow).Style.Add("display", "none")
                            End If
                            If CType(Cufex_MainContent.FindControl("SearchRow2"), HtmlTableRow) IsNot Nothing Then
                                CType(Cufex_MainContent.FindControl("SearchRow2"), HtmlTableRow).Style.Add("display", "none")
                            End If
                            If CType(Cufex_MainContent.FindControl("SearchRow3"), HtmlTableRow) IsNot Nothing Then
                                CType(Cufex_MainContent.FindControl("SearchRow3"), HtmlTableRow).Style.Add("display", "none")
                            End If
                        End If
                    End If
                End If
            End If

            Dim localZone As TimeZone = TimeZone.CurrentTimeZone
            Dim CurrentDate As DateTime = Now
            Dim currentOffset As TimeSpan = localZone.GetUtcOffset(CurrentDate)
            HttpContext.Current.Session("timezone") = currentOffset.Hours.ToString
        End If
    End Sub
    Public Sub FixMenuVisibleItems()
        SetMenuItemsClass()

        DivMain_Home_Def.Visible = True

        DivSubMain_Security_ChangePassword.Visible = CommonMethods.getPermission("Security->Change Password (Screen)", Session("userkey").ToString) <> "0"
        DivSubMain_Security_Users.Visible = CommonMethods.getPermission("Security->Users (Screen)", Session("userkey").ToString) <> "0"
        DivSubMain_Security_UsersControl.Visible = CommonMethods.getPermission("Security->User Control (Screen)", Session("userkey").ToString) <> "0"
        DivSubMain_Security_UserProfile.Visible = CommonMethods.getPermission("Security->User Profile (Screen)", Session("userkey").ToString) <> "0"
        DivSubMain_Security_Profiles.Visible = CommonMethods.getPermission("Security->Profiles (Screen)", Session("userkey").ToString) <> "0"
        DivMain_Security.Visible = DivSubMain_Security_ChangePassword.Visible Or DivSubMain_Security_Users.Visible Or DivSubMain_Security_UsersControl.Visible Or DivSubMain_Security_UserProfile.Visible Or DivSubMain_Security_Profiles.Visible

        DivSubMain_Configuration_ShipTo.Visible = CommonMethods.getPermission("Configuration->Ship To (Screen)", Session("userkey").ToString) <> "0"
        DivSubMain_Configuration_ShipFrom.Visible = CommonMethods.getPermission("Configuration->Suppliers (Screen)", Session("userkey").ToString) <> "0"
        DivSubMain_Configuration_Items.Visible = CommonMethods.getPermission("Configuration->Items (Screen)", Session("userkey").ToString) <> "0"
        DivSubMain_Configuration_ItemCatalogue.Visible = CommonMethods.getPermission("Configuration->Item Catalogue (Screen)", Session("userkey").ToString) <> "0"
        DivMain_Configuration.Visible = DivSubMain_Configuration_ShipTo.Visible Or DivSubMain_Configuration_ShipFrom.Visible Or DivSubMain_Configuration_Items.Visible Or DivSubMain_Configuration_ItemCatalogue.Visible

        DivSubMain_Warehouse_PO.Visible = CommonMethods.getPermission("Warehouse->Purchase Order (Screen)", Session("userkey").ToString) <> "0"
        DivSubMain_Warehouse_ASN.Visible = CommonMethods.getPermission("Warehouse->ASN Receipt (Screen)", Session("userkey").ToString) <> "0"
        DivSubMain_Warehouse_Shipment.Visible = CommonMethods.getPermission("Warehouse->Shipment Order (Screen)", Session("userkey").ToString) <> "0"
        DivSubMain_Warehouse_OrderManagement.Visible = CommonMethods.getPermission("Warehouse->Order Management (Screen)", Session("userkey").ToString) <> "0"
        DivMain_Warehouse.Visible = DivSubMain_Warehouse_PO.Visible Or DivSubMain_Warehouse_ASN.Visible Or DivSubMain_Warehouse_Shipment.Visible Or DivSubMain_Warehouse_OrderManagement.Visible

        DivSubMain_Inventory_Balance.Visible = CommonMethods.getPermission("Inventory->Inventory Balance (Screen)", Session("userkey").ToString) <> "0"
        DivMain_Inventory.Visible = DivSubMain_Inventory_Balance.Visible

        DivSubMain_Reporting_ViewReports.Visible = CommonMethods.getPermission("ReportsAndKPIs->Reports (Screen)", Session("userkey").ToString) <> "0"
        DivMain_Reporting.Visible = DivSubMain_Reporting_ViewReports.Visible
    End Sub

    Private Sub SetMenuItemsClass()
        'First Level
        DivMain_Home_Def.Attributes("class") = IIf(section = SectionName.Home_Def, IIf(MenuLevel = 0, "MenuItemStyleSel", ""), "")
        DivMain_Security.Attributes("class") = "MenuItemStyle " & IIf(section = SectionName.Security, IIf(MenuLevel = 0, "MenuItemStyleActive", "MenuItemStyleSel"), "")
        DivMain_Configuration.Attributes("class") = "MenuItemStyle " & IIf(section = SectionName.Configuration, IIf(MenuLevel = 0, "MenuItemStyleActive", "MenuItemStyleSel"), "")
        DivMain_Warehouse.Attributes("class") = "MenuItemStyle " & IIf(section = SectionName.Warehouse, IIf(MenuLevel = 0, "MenuItemStyleActive", "MenuItemStyleSel"), "")
        DivMain_Inventory.Attributes("class") = "MenuItemStyle " & IIf(section = SectionName.Inventory, IIf(MenuLevel = 0, "MenuItemStyleActive", "MenuItemStyleSel"), "")
        DivMain_Reporting.Attributes("class") = "MenuItemStyle " & IIf(section = SectionName.Reporting, IIf(MenuLevel = 0, "MenuItemStyleActive", "MenuItemStyleSel"), "")

        'First Level Menu Arrow
        DivMain_Home_Def_MenuArrow.Attributes("class") += IIf(section = SectionName.Home_Def, IIf(MenuLevel = 0, " Opened", ""), "")
        DivMain_Security_MenuArrow.Attributes("class") += IIf(section = SectionName.Security, IIf(MenuLevel = 0, "", " Opened"), "")
        DivMain_Configuration_MenuArrow.Attributes("class") += IIf(section = SectionName.Configuration, IIf(MenuLevel = 0, "", " Opened"), "")
        DivMain_Warehouse_MenuArrow.Attributes("class") += IIf(section = SectionName.Warehouse, IIf(MenuLevel = 0, "", " Opened"), "")
        DivMain_Inventory_MenuArrow.Attributes("class") += IIf(section = SectionName.Inventory, IIf(MenuLevel = 0, "", " Opened"), "")
        DivMain_Reporting_MenuArrow.Attributes("class") += IIf(section = SectionName.Reporting, IIf(MenuLevel = 0, "", " Opened"), "")

        'Second Level
        DivSubMain_Security_ChangePassword.Attributes("class") = "MenuSubItem " & IIf(Subsection = SubSectionName.Security_ChangePassword, IIf(MenuLevel = 1, "MenuSubItemActive", "MenuSubItemSel"), "")
        DivSubMain_Security_Users.Attributes("class") = "MenuSubItem " & IIf(Subsection = SubSectionName.Security_Users, IIf(MenuLevel = 1, "MenuSubItemActive", "MenuSubItemSel"), "")
        DivSubMain_Security_UsersControl.Attributes("class") = "MenuSubItem " & IIf(Subsection = SubSectionName.Security_UsersControl, IIf(MenuLevel = 1, "MenuSubItemActive", "MenuSubItemSel"), "")
        DivSubMain_Security_UserProfile.Attributes("class") = "MenuSubItem " & IIf(Subsection = SubSectionName.Security_UserProfile, IIf(MenuLevel = 1, "MenuSubItemActive", "MenuSubItemSel"), "")
        DivSubMain_Security_Profiles.Attributes("class") = "MenuSubItem " & IIf(Subsection = SubSectionName.Security_Profiles, IIf(MenuLevel = 1, "MenuSubItemActive", "MenuSubItemSel"), "")
        DivSubMain_Configuration_ShipTo.Attributes("class") = "MenuSubItem " & IIf(Subsection = SubSectionName.Configuration_ShipTo, IIf(MenuLevel = 1, "MenuSubItemActive", "MenuSubItemSel"), "")
        DivSubMain_Configuration_ShipFrom.Attributes("class") = "MenuSubItem " & IIf(Subsection = SubSectionName.Configuration_ShipFrom, IIf(MenuLevel = 1, "MenuSubItemActive", "MenuSubItemSel"), "")
        DivSubMain_Configuration_Items.Attributes("class") = "MenuSubItem " & IIf(Subsection = SubSectionName.Configuration_Items, IIf(MenuLevel = 1, "MenuSubItemActive", "MenuSubItemSel"), "")
        DivSubMain_Configuration_ItemCatalogue.Attributes("class") = "MenuSubItem " & IIf(Subsection = SubSectionName.Configuration_ItemCatalogue, IIf(MenuLevel = 1, "MenuSubItemActive", "MenuSubItemSel"), "")
        DivSubMain_Warehouse_PO.Attributes("class") = "MenuSubItem " & IIf(Subsection = SubSectionName.Warehouse_PO, IIf(MenuLevel = 1, "MenuSubItemActive", "MenuSubItemSel"), "")
        DivSubMain_Warehouse_ASN.Attributes("class") = "MenuSubItem " & IIf(Subsection = SubSectionName.Warehouse_ASN, IIf(MenuLevel = 1, "MenuSubItemActive", "MenuSubItemSel"), "")
        DivSubMain_Warehouse_Shipment.Attributes("class") = "MenuSubItem " & IIf(Subsection = SubSectionName.Warehouse_Shipment, IIf(MenuLevel = 1, "MenuSubItemActive", "MenuSubItemSel"), "")
        DivSubMain_Warehouse_OrderManagement.Attributes("class") = "MenuSubItem " & IIf(Subsection = SubSectionName.Warehouse_OrderManagement, IIf(MenuLevel = 1, "MenuSubItemActive", "MenuSubItemSel"), "")
        DivSubMain_Inventory_Balance.Attributes("class") = "MenuSubItem " & IIf(Subsection = SubSectionName.Inventory_Balance, IIf(MenuLevel = 1, "MenuSubItemActive", "MenuSubItemSel"), "")
        DivSubMain_Reporting_ViewReports.Attributes("class") = "MenuSubItem " & IIf(Subsection = SubSectionName.Reporting_ViewReports, IIf(MenuLevel = 1, "MenuSubItemActive", "MenuSubItemSel"), "")
    End Sub
    Private Sub CheckIfLogged()
        On Error Resume Next
        If Not LoggedIn Then
            'Response.Redirect("Cufex.aspx")
            If Val(fromdefault.Value) > 0 And Page.IsPostBack Then

                Dim sec As New Security
                sec.SetLoginSession(fromdefault.Value)
                If LoggedIn Then
                    CheckIfLogged()
                    Exit Sub
                End If
            Else
                HttpContext.Current.Session("Cufex_AfterLoginURL") = Page.Request.Url.AbsoluteUri
                'did not pass through the Cufex Login screen to reach this form, then redirect
                Response.Redirect(Page.GetRouteUrl("SNSsoftware-CMS", Nothing))
            End If
        Else
            'winLogIn.VisibleOnPageLoad = False
            fromdefault.Value = Session("BUserCode")
        End If
    End Sub
    Public Property FormParentName As String
        Get
            If ViewState("FormParentName") Is Nothing Then ViewState("FormParentName") = ""
            FormParentName = ViewState("FormParentName")
        End Get
        Set(ByVal value As String)
            ViewState("FormParentName") = value
        End Set
    End Property
    Public Property FormName As String
        Get
            If ViewState("FormName") Is Nothing Then ViewState("FormName") = ""
            FormName = ViewState("FormName")
        End Get
        Set(ByVal value As String)
            ViewState("FormName") = value
        End Set
    End Property
    Public Property CanView As Boolean
        Get
            If HttpContext.Current.Session("CanView_" & FormParentName & "_" & FormName) Is Nothing Then HttpContext.Current.Session("CanView_" & FormParentName & "_" & FormName) = CommonMethods.getPermission(FormParentName & "->" & FormName & " (Screen)", Session("userkey").ToString) <> "0"
            CanView = HttpContext.Current.Session("CanView_" & FormParentName & "_" & FormName)
        End Get
        Set(ByVal value As Boolean)
            HttpContext.Current.Session("CanView_" & FormParentName & "_" & FormName) = value
        End Set
    End Property
    Public Property CanAddNew As Boolean
        Get
            If HttpContext.Current.Session("CanAddNew_" & FormName) Is Nothing Then HttpContext.Current.Session("CanAddNew_" & FormName) = CommonMethods.getPermission(FormName & "->New (Action)", HttpContext.Current.Session("userkey").ToString) <> "0"
            CanAddNew = HttpContext.Current.Session("CanAddNew_" & FormName)
        End Get
        Set(ByVal value As Boolean)
            HttpContext.Current.Session("CanAddNew_" & FormName) = value
        End Set
    End Property
    Public Property CanSave As Boolean
        Get
            If HttpContext.Current.Session("CanSave_" & FormName) Is Nothing Then HttpContext.Current.Session("CanSave_" & FormName) = CommonMethods.getPermission(FormName & "->Save (Action)", HttpContext.Current.Session("userkey").ToString) <> "0"
            CanSave = HttpContext.Current.Session("CanSave_" & FormName)
        End Get
        Set(ByVal value As Boolean)
            HttpContext.Current.Session("CanSave_" & FormName) = value
        End Set
    End Property
    Public Property CanDelete As Boolean
        Get
            If HttpContext.Current.Session("CanDelete_" & FormName) Is Nothing Then HttpContext.Current.Session("CanDelete_" & FormName) = CommonMethods.getPermission(FormName & "->Delete (Action)", HttpContext.Current.Session("userkey").ToString) <> "0"
            CanDelete = HttpContext.Current.Session("CanDelete_" & FormName)
        End Get
        Set(ByVal value As Boolean)
            HttpContext.Current.Session("CanDelete_" & FormName) = value
        End Set
    End Property
    Public Property CanSearch As Boolean
        Get
            If HttpContext.Current.Session("CanSearch_" & FormName) Is Nothing Then HttpContext.Current.Session("CanSearch_" & FormName) = CommonMethods.getPermission(FormName & "->Search (Action)", HttpContext.Current.Session("userkey").ToString) <> "0"
            CanSearch = HttpContext.Current.Session("CanSearch_" & FormName)
        End Get
        Set(ByVal value As Boolean)
            HttpContext.Current.Session("CanSearch_" & FormName) = value
        End Set
    End Property
End Class