<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Cufex/Cufex_Site.Master" CodeBehind="Cufex_Security_Users.aspx.vb" Inherits="SNSsoftware.Cufex_Security_Users" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Cufex_MainContent" runat="server">

    <%--Page Title--%>
    <div class="NormalDiv1118Max GetFullHeightAtLeast">
        <div style="height: 33px;"></div>
        <div class="MainHeader">
            <div class="iWantMyChildrenFloatHeight" style="position: relative;">
                <div class="floatL Width100">
                    <div class="floatL">
                        <div class="MainPageTitle" data-id="PORTALUSERS">
                            Users
                        </div>
                        <div class="MainPageDesc">
                            Manage portal users
                        </div>
                    </div>
                    <table class="floatR">
                        <tr>
                            <td>
                                <a id="btnQuickEntry" runat="server" class="btnQuickEntry AnimateMe">Quick Entry
                                </a>
                            </td>
                            <td style="width: 13px;"></td>
                            <td>
                                <a id="btnDelete" runat="server" class="btnDelete AnimateMe">Delete
                                </a>
                            </td>
                            <td id="TableAction1" runat="server" style="width: 13px;"></td>
                            <td id="TableAction2" runat="server">
                                <div class="btnActions">Actions</div>
                                <div class="ActionHiddenButtons">
                                    <div class="BtnDoSomeThing AnimateMe" data-id="1"><span class="ion-ios-trending-up MyFontIon"></span><span style="vertical-align: 4px;">Reset Configuration</span></div>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
        <div style="position: relative; padding-top: 25px; width: 100%;" class="HeaderGridView content_3">
            <table class="GridContainer" data-resizemode="fit" id="GridContainer">
                <tr class="GridRow GridAdjust">
                    <td class="GridCell GridHead borderRight0 selectAllWidth"></td>
                    <td class="GridCell GridHead borderRight0 selectAllWidth"></td>
                    <td class="GridCell GridHead selectAllWidth">
                        <div class="AdjustColumns"></div>
                    </td>
                    <td class="GridCell GridHead" data-id="UserKey">
                        <span class="MyTitleHead">User ID</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="FirstName">
                        <span class="MyTitleHead">First Name</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="LastName">
                        <span class="MyTitleHead">Last Name</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Email">
                        <span class="MyTitleHead">Email</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead borderRight0" data-id="Active">
                        <span class="MyTitleHead">Active</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                </tr>
                <tr class="GridRow SearchStyle" id="SearchRow" runat="server">
                    <td class="GridCell GridHead selectAllWidth">
                        <input class="CheckBoxCostumizedNS2 chkSelectAll" type="checkbox" id="chkSelectAll" />
                        <label for="chkSelectAll"><span class="CheckBoxStyle"></span></label>
                    </td>
                    <td class="GridCell GridHeadSearch borderRight0 selectAllWidth">
                        <div class="EraseSearch"></div>
                    </td>
                    <td class="GridCell GridHeadSearch selectAllWidth">
                        <div class="GridSearch"></div>
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="UserKey" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="FirstName" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="LastName" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Email" />
                    </td>
                    <td class="GridCell GridHeadSearch borderRight0">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Active" />
                    </td>
                </tr>
                <tr class="GridRow NoResults">
                    <td class="GridCell borderRight0 selectAllWidth"></td>
                    <td class="GridCell GridHeadSearch borderRight0 selectAllWidth"></td>
                    <td class="GridCell GridHeadSearch selectAllWidth"></td>
                    <td class="GridCell GridContentCell borderRight0" data-id="1">No Results
                    </td>
                    <td class="GridCell GridContentCell borderRight0" data-id="2"></td>
                    <td class="GridCell GridContentCell borderRight0" data-id="3"></td>
                    <td class="GridCell GridContentCell borderRight0" data-id="4"></td>
                    <td class="GridCell GridContentCell borderRight0" data-id="5"></td>
                </tr>
            </table>
            <div class="PagingContainer">
                <table class="u-marginAuto">
                    <tr>
                        <td>
                            <div class="Arrow-Left-Back-First AnimateMe">
                                <div class="First First1 AnimateMe"></div>
                                <div class="First First2 AnimateMe"></div>
                            </div>
                        </td>
                        <td>
                            <div class="Arrow-Left-Back AnimateMe"></div>
                        </td>
                        <td>
                            <div class="PagingNumbers AnimateMe">0 of 0</div>
                        </td>
                        <td>
                            <div class="Arrow-Right-Forward AnimateMe"></div>
                        </td>
                        <td>
                            <div class="Arrow-Right-Forward-Last AnimateMe">
                                <div class="Last Last1 AnimateMe"></div>
                                <div class="Last Last2 AnimateMe"></div>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div style="position: relative; height: 15px;"></div>
        <div class="New_Modify_Record_PopUp">
            <div style="position: relative;">
                <div class="MyAbso_Record_PopUpContainer">
                    <div class="R_PopupTitle">
                        User
                        <div class="SaveRecordNow AnimateMe" id="btnSave" runat="server"></div>
                        <div class="ClosePopup AnimateMe"></div>
                    </div>
                    <div style="position: relative; height: 500px; width: 100%;" class="MyContainerPopup GetFullHeightForPopup content_4">
                        <div class="iWantMyChildrenFloatHeight">
                            <div class="floatL Width100 RecordHeader">
                                <div class="FloatRecord floatL">
                                    <input type="hidden" id="MyID" class="MyRecordID" value="0" />
                                    <div class="FloatRecordTitle floatL">User ID*</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputUserKey" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">First Name*</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputFirstName" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Last Name*</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputLastName" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Email*</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputEmail" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Password*</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="password" class="textRecordStyle textRecordStylePassword InputPassword" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL DisplayNone">
                                    <div class="FloatRecordTitle floatL">Status*</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputActive" data-value="1" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Confirm Password*</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="password" class="textRecordStyle textRecordStylePassword InputConfirmPassword" />
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
        <div class="Adjust_Columns_PopUp">
            <div style="position: relative;">
                <div class="MyAbso_Adjust_Columns_PopUpContainer">
                    <div class="CloseACPopup"></div>
                    <div class="AC_PopupDesc">
                        Drag to customize your preferred columns order
                    </div>
                    <div class="circle"></div>
                    <div style="position: relative; height: 380px; width: 100%;" class="content_4 GridColumnsChooser">
                    </div>
                    <div class="iWantMyChildrenFloatHeight">
                        <div class="floatL Width100">
                            <div class="floatL AC_PopupAction_Reset PositionRelative AnimateMe">
                                <div class="AC_PopupAction_Text">Reset</div>
                            </div>
                            <div class="floatL AC_PopupAction_Apply PositionRelative AnimateMe">
                                <div class="AC_PopupAction_Text">Apply</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="FormSettings">
        <input type="hidden" id="NumberOfRecordsInPage" value="10" />
        <%--<input type="hidden" class="HiddenDetailLink" value="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Security_Profile_Users-details", New With {.id = 0})) %>" />--%>
        <input type="hidden" class="HiddenDetailLink" value="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Security_Users", Nothing)) %>" />
        <input type="hidden" id="HiddenID" runat="server" class="HiddenID" value="0" />

        <input type="hidden" class="MyFields" value="UserKey" data-columnname="User ID" data-priority="1" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyFields" value="FirstName" data-columnname="First Name" data-priority="2" data-hidden="false" />
        <input type="hidden" class="MyFields" value="LastName" data-columnname="Last Name" data-priority="3" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Email" data-columnname="Email" data-priority="4" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Active" data-columnname="Active" data-priority="5" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Password" />
        <input type="hidden" class="MyFields" value="ConfirmPassword" />
    </div>
</asp:Content>
