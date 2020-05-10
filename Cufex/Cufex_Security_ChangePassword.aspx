<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Cufex/Cufex_Site.Master" CodeBehind="Cufex_Security_ChangePassword.aspx.vb" Inherits="SNSsoftware.Cufex_Security_ChangePassword" %>

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
                                <a id="btnQuickEntry" runat="server" class="btnQuickEntry AnimateMe">Change Password
                                </a>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
        <div class="New_Modify_Record_PopUp">
            <div style="position: relative;">
                <div class="MyAbso_Record_PopUpContainer">

                    <div class="R_PopupTitle">
                        Change Password
                        <div class="SaveRecordNow AnimateMe" id="btnSave" runat="server"><span class="ion-ios-checkmark-circle-outline"></span></div>
                        <div class="ClosePopup AnimateMe"><span class="ion-ios-exit"></span></div>
                    </div>
                    <div style="position: relative; height: 500px; width: 100%;" class="MyContainerPopup GetFullHeightForPopup content_4">
                        <div class="iWantMyChildrenFloatHeight">
                            <div class="floatL Width100 RecordHeader">
                                <div class="FloatRecord floatL">
                                    <input type="hidden" id="MyID" class="MyRecordID" value="0" />
                                    <div class="FloatRecordTitle floatL">Original Password*</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="password" class="textRecordStyle InputOriginalPassword" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">New Password*</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="password" class="textRecordStyle InputNewPassword" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Confirm Password*</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="password" class="textRecordStyle InputConfirmPassword" />
                                    </div>
                                </div>
                                <div class="floatL Width100 PasswordInfo">
                                    * Password must be at least 10 characters, have one upper case letter, one lower case letter and one base 10 digits (0 to 9)
                                </div>
                            </div>
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
