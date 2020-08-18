<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Cufex/Cufex_Site.Master" CodeBehind="Cufex_Administration_UITemplates.aspx.vb" Inherits="SNSsoftware.Cufex_Administration_UITemplates" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Cufex_MainContent" runat="server">

    <%--Page Title--%>
    <div class="NormalDiv1118Max GetFullHeightAtLeast">
        <div style="height: 33px;"></div>
        <div class="MainHeader">
            <div class="iWantMyChildrenFloatHeight" style="position: relative;">
                <div class="floatL Width100">
                    <div class="floatL">
                        <div class="MainPageTitle" data-id="UITEMPLATES">
                            UI Templates
                        </div>
                        <div class="MainPageDesc">
                            Manage UI Templates
                        </div>
                    </div>
                    <table class="floatR">
                        <tr>
                            <td>
                                <a id="btnExport" runat="server" class="btnExport AnimateMe" title="Export"></a>
                            </td>
                            <td style="width: 13px;"></td>
                            <td>
                                <div class="VerticalSep"></div>
                            </td>
                            <td style="width: 13px;"></td>
                            <td>
                                <a id="btnRefresh" runat="server" class="btnRefresh AnimateMe" title="Refresh"></a>
                            </td>
                            <td style="width: 13px;"></td>
                            <td>
                                <a id="btnQuickEntry" runat="server" class="btnQuickEntry AnimateMe">Quick Entry
                                </a>
                            </td>
                            <td style="width: 13px;"></td>
                            <td>
                                <a id="btnDelete" runat="server" class="btnDelete AnimateMe">Delete
                                </a>
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
                    <td class="GridCell GridHead" data-id="UITemplateID">
                        <span class="MyTitleHead">Template ID</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="PortalLogo">
                        <span class="MyTitleHead">Portal Logo</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="MenuBackgroundColor">
                        <span class="MyTitleHead">Menu Color</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="ScreenBackgroundColor">
                        <span class="MyTitleHead">Screen Color</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="GridBackgroundColor">
                        <span class="MyTitleHead">Grid Color</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="ButtonBackgroundColor">
                        <span class="MyTitleHead">Button Color</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead borderRight0" data-id="TextBackgroundColor">
                        <span class="MyTitleHead">Text Color</span>
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
                        <input type="text" placeholder="Search" class="SearchClass" data-id="UITemplateID" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="PortalLogo" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="MenuBackgroundColor" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="ScreenBackgroundColor" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="GridBackgroundColor" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="ButtonBackgroundColor" />
                    </td>
                    <td class="GridCell GridHeadSearch borderRight0">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="TextBackgroundColor" />
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
                    <td class="GridCell GridContentCell borderRight0" data-id="6"></td>
                    <td class="GridCell GridContentCell borderRight0" data-id="7"></td>
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
                        UI Template & Colors
                        <div class="SaveRecordNow AnimateMe" id="btnSave" runat="server"></div>
                        <div class="ClosePopup AnimateMe"></div>
                    </div>
                    <div style="position: relative; height: 500px; width: 100%;" class="MyContainerPopup GetFullHeightForPopup content_4">
                        <div class="iWantMyChildrenFloatHeight">
                            <div class="floatL Width100 RecordHeader">
                                <div class="FloatRecord floatL">
                                    <input type="hidden" id="MyID" class="MyRecordID" value="0" />
                                    <div class="FloatRecordTitle floatL">UI Template ID*</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputUITemplateID" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Portal Logo</div>
                                    <div class="FloatRecordField floatL">
                                        <img class="PortalLogo" src="<%= sAppPath & "images/Cufex_Images/logo.png" %>" alt="Portal Logo" style="width: 30px; height: 30px; position: absolute; top: 50%; right: 5px; transform: translate(0,-50%)" />
                                        <div class="textRecordStyle">
                                            <input type="file" class="InputPortalLogo" accept="image/x-png,image/gif,image/jpeg" style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; opacity: 0; cursor: pointer;" />
                                        </div>
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Menu Color</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputMenuBackgroundColor jscolor {hash:true,value:'#FAFAFA',required:false}" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Screen Color</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputScreenBackgroundColor jscolor {hash:true,value:'#FAFAFA',required:false}" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Grid Color</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputGridBackgroundColor jscolor {hash:true,value:'#FAFAFA',required:false}" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Button Color</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputButtonBackgroundColor jscolor {hash:true,value:'#FAFAFA',required:false}" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Text Color</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputTextBackgroundColor jscolor {hash:true,value:'#FAFAFA',required:false}" />
                                    </div>
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
        <input type="hidden" id="SortBy" value="SerialKey desc" />

        <input type="hidden" class="HiddenDetailLink" value="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Administration_UITemplates", Nothing)) %>" />
        <input type="hidden" id="HiddenID" runat="server" class="HiddenID" value="0" />

        <input type="hidden" class="MyFields" value="UITemplateID" data-columnname="UI Template ID" data-priority="1" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyFields" value="PortalLogo" data-columnname="Portal Logo" data-priority="2" data-hidden="false" />
        <input type="hidden" class="MyFields" value="MenuBackgroundColor" data-columnname="Menu Color" data-priority="3" data-hidden="false" />
        <input type="hidden" class="MyFields" value="ScreenBackgroundColor" data-columnname="Screen Color" data-priority="4" data-hidden="false" />
        <input type="hidden" class="MyFields" value="GridBackgroundColor" data-columnname="Grid Color" data-priority="5" data-hidden="false" />
        <input type="hidden" class="MyFields" value="ButtonBackgroundColor" data-columnname="Button Color" data-priority="6" data-hidden="false" />
        <input type="hidden" class="MyFields" value="TextBackgroundColor" data-columnname="Text Color" data-priority="7" data-hidden="false" />

    </div>
</asp:Content>
