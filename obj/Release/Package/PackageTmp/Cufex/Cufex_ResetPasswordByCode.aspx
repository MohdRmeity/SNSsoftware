﻿<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Cufex/Cufex_Site.Master" CodeBehind="Cufex_ResetPasswordByCode.aspx.vb" Inherits="SNSsoftware.Cufex_ResetPasswordByCode" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cufex_HeadContent" runat="server">
    <style type="text/css">
        body {
            min-width: 100%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="LogInContainer" runat="server">
    <div class="LogInMainDiv CufexBG GetFullHeightAtLeast CoverImage">

        <div class="NormalDiv">
           <%-- <div class="HeaderFloat1">
                <a href="<%= sAppPath %>" title="Cufex">
                    <img src="<%= sAppPath%>images/Cufex_Images/CufexLoginLogo.png" alt="Cufex Logo" /></a>
            </div>--%>
            <div class="iWantMyChildrenFloatHeight">
                <div class="floatL Width100">
                    <div class="floatL Width100 Login_Details">
                        <div class="floatL CredentialsDiv">
                            <div class="floatL Width100 LoginText">
                                <h3 class="UpperCaseMe">Reset Password By Code</h3>
                            </div>
                            <div class="floatL Width100 MarginMe">
                                <asp:TextBox ID="txtCode" runat="server" placeholder="Code" ClientIDMode="Static" CssClass="MyLogInTextBox TxtUsername"></asp:TextBox>
                            </div>
                            <div class="floatL Width100 MarginMe">
                                <asp:TextBox ID="txtPassword" runat="server" placeholder="Password" ClientIDMode="Static" TextMode="Password" CssClass="MyLogInTextBox TxtPassword"></asp:TextBox>
                            </div>
                            <div class="floatL Width100">
                                <asp:TextBox ID="txtConfirmPassword" runat="server" placeholder="Confirm Password" ClientIDMode="Static" TextMode="Password" CssClass="MyLogInTextBox TxtPassword"></asp:TextBox>
                                <div class="LoginValidator">
                                    <asp:CustomValidator ID="CVError1" runat="server" Display="Dynamic" CssClass="LogInError" ErrorMessage=""></asp:CustomValidator>
                                    <asp:RequiredFieldValidator ID="rqCode" runat="server" ControlToValidate="txtCode" CssClass="LogInError" Display="Dynamic" ErrorMessage=""></asp:RequiredFieldValidator>
                                    <asp:RequiredFieldValidator ID="rqPassword" runat="server" ControlToValidate="txtPassword" CssClass="LogInError" Display="Dynamic" ErrorMessage=""></asp:RequiredFieldValidator>
                                    <asp:RequiredFieldValidator ID="rqConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword" CssClass="LogInError" Display="Dynamic" ErrorMessage=""></asp:RequiredFieldValidator>
                                </div>
                            </div>
                             <div class="floatL Width100 PasswordInfo" style="padding-left: 0; padding-right: 0; padding-top: 0;">
                                * Password must be at least 10 characters, have one upper case letter, one lower case letter, one special character and one base 10 digits (0 to 9)
                            </div>
                            <div class="floatL Width100 MarginMe">
                                <asp:Button ID="BtnSubmit" runat="server" CssClass="MyButtonLogin IWantAMouseOVerNowBut80" Text="RESET PASSWORD" OnClientClick="return Cufex_Valid();" />
                            </div>
                            <div class="floatL Width100 textalignC">
                                <a href="<%= Page.GetRouteUrl("SNSsoftware-Home", Nothing)%>" class="MyLink">Return Login</a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Cufex_ScriptContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            Cufex_Set();
        });
        $(window).load(function () {
            $('.LogInMainDiv').show();
            $('.DivFooter').show();
        });
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function () { Cufex_Set(); });

        function Cufex_Set() {
            $('#txtPassword').keypress(function (e) {
                if ($('#txtPassword').val() != '') {
                    var code = (e.keyCode ? e.keyCode : e.which);
                    if (code == 13 || code == 9) {
                        $('.BtnSubmit').click();
                    }
                }
            });
            $('#txtConfirmPassword').keypress(function (e) {
                if ($('#txtConfirmPassword').val() != '') {
                    var code = (e.keyCode ? e.keyCode : e.which);
                    if (code == 13 || code == 9) {
                        $('.BtnSubmit').click();
                    }
                }
            });
        }

        function Cufex_Valid() {

            var val = Page_ClientValidate();
            if (!val) {

                for (var i = 0; i < Page_Validators.length; i++) {
                    var myPageID = "#" + Page_Validators[i].controltovalidate
                    $(myPageID).css({ "border": "0px", "background-position": "left top" });

                }

                for (var i = 0; i < Page_Validators.length; i++) {
                    if (!Page_Validators[i].isvalid) {
                        var myPageID = "#" + Page_Validators[i].controltovalidate
                        $(myPageID).css({ "border": "1px solid #C60929", "background-position": "left bottom" });
                    }
                }
            }
            return val;
        }
    </script>
</asp:Content>