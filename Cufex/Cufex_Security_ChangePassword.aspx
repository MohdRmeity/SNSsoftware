<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Cufex/Cufex_Site.Master" CodeBehind="Cufex_Security_ChangePassword.aspx.vb" Inherits="SNSsoftware.Cufex_Security_ChangePassword" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Cufex_HeadContent" runat="server">
    <style>
        .ui-resizable-s {
            display: none !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Cufex_MainContent" runat="server">
    <%--Page Title--%>
    <div class="NormalDiv1118Max GetFullHeightAtLeast">
        <div style="height: 33px;"></div>
        <div class="MainHeader">
            <div class="iWantMyChildrenFloatHeight" style="position: relative;">
                <div class="floatL Width100">
                    <div class="floatL">
                        <div class="MainPageTitle" data-id="ChangePassword">
                            Change Password
                        </div>
                    </div>
                    <table class="floatR">
                        <tr>
                            <td>
                                <a id="btnSaveHeader" runat="server" class="btnSave AnimateMe">Save
                                </a>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
        <div class="NewRecord Width100">
            <div class="iWantMyChildrenFloatHeight">
                <div class="floatL Width100">
                    <div class="floatL Width100 u-overflowHidden NewHeaderRecord">
                        <div class="FloatRecordNew floatL">
                            <input type="hidden" id="MyID" class="MyRecordID" value="0" />
                            <div class="FloatRecordTitleNew">Original Password<span>*</span></div>
                            <div style="position:relative;">
                                <input type="password" class="textRecordStyle textRecordStylePassword InputOriginalPassword" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">New Password<span>*</span></div>
                            <div style="position:relative;">
                                <input type="password" class="textRecordStyle textRecordStylePassword InputNewPassword" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Confirm Password<span>*</span></div>
                            <div style="position:relative;">
                                <input type="password" class="textRecordStyle textRecordStylePassword InputConfirmPassword" />
                            </div>
                        </div>
                        <div class="floatL Width100 PasswordInfo" style="padding-left: 0; padding-right: 0;">
                            * Password must be at least 10 characters, have one upper case letter, one lower case letter and one base 10 digits (0 to 9)
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="FormSettings">
        <input type="hidden" class="MyFields" value="OriginalPassword" />
        <input type="hidden" class="MyFields" value="NewPassword" />
        <input type="hidden" class="MyFields" value="ConfirmPassword" />
    </div>
</asp:Content>
