<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Cufex/Cufex_Site.Master" CodeBehind="Cufex_Configuration_ItemCatalogue.aspx.vb" Inherits="SNSsoftware.Cufex_Configuration_ItemCatalogue" %>

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

        <div class="BackBtn BackHeader AnimateMe" style="display: none;">
            Back to List
        </div>

        <div class="MainHeader">
            <div class="iWantMyChildrenFloatHeight" style="position: relative;">
                <div class="floatL Width100">
                    <div class="floatL">
                        <div class="MainPageTitle" data-id="SKUCATALOGUE">
                            Item Catalogue
                        </div>
                        <div class="MainPageDesc" data-text="Items Catalogues List">
                            Items Catalogues List
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
                                <a id="btnAddNew" runat="server" class="btnAddNew AnimateMe">New Catalogue
                                </a>
                            </td>
                            <td style="width: 13px; display: none;"></td>
                            <td>
                                <a id="btnSaveHeader" runat="server" class="btnSave AnimateMe" style="display: none;">Save
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
            <table class="GridContainer" data-resizemode="fit">
                <tr class="GridRow GridAdjust">
                    <td class="GridCell GridHead borderRight0 selectAllWidth"></td>
                    <td class="GridCell GridHead borderRight0 selectAllWidth"></td>
                    <td class="GridCell GridHead selectAllWidth">
                        <div class="AdjustColumns"></div>
                    </td>
                    <td class="GridCell GridHead" data-id="Sku">
                        <span class="MyTitleHead">Item</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="StorerKey">
                        <span class="MyTitleHead">Owner</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="ConsigneeKey">
                        <span class="MyTitleHead">Consignee</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Price">
                        <span class="MyTitleHead">Price</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Currency">
                        <span class="MyTitleHead">Currency</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="SUsr1">
                        <span class="MyTitleHead">UDF1</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="SUsr2">
                        <span class="MyTitleHead">UDF2</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="SUsr3">
                        <span class="MyTitleHead">UDF3</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="SUsr4">
                        <span class="MyTitleHead">UDF4</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead borderRight0" data-id="SUsr5">
                        <span class="MyTitleHead">UDF5</span>
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
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Sku" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="StorerKey" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="ConsigneeKey" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Price" />
                    </td>
                    <td class="GridCell GridHeadSearch" style="overflow: visible;">
                        <select data-placeholder="Search" multiple class="chosen-select SearchClass InputCurrency" data-id="Currency">
                        </select>
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="SUsr1" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="SUsr2" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="SUsr3" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="SUsr4" />
                    </td>
                    <td class="GridCell GridHeadSearch borderRight0">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="SUsr5" />
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
                    <td class="GridCell GridContentCell borderRight0" data-id="8"></td>
                    <td class="GridCell GridContentCell borderRight0" data-id="9"></td>
                    <td class="GridCell GridContentCell borderRight0" data-id="10"></td>
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

        <div class="NewRecord Width100" style="display: none;">
            <div class="iWantMyChildrenFloatHeight">
                <div class="floatL Width100">
                    <div class="floatL Width100 u-overflowHidden NewHeaderRecord">
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Owner<span>*</span></div>
                            <div>
                                <select data-placeholder="Select Owners" multiple class="chosen-select InputStorerKey InputAutoPostBack" data-mode="single">
                                </select>
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Consignee<span>*</span></div>
                            <div>
                                <select data-placeholder="Select Consignee" multiple class="chosen-select InputConsigneeKey" data-mode="single">
                                </select>
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Item<span>*</span></div>
                            <div class="PositionRelative">
                                <select data-placeholder="Select Item" multiple class="chosen-select InputSku" data-mode="single">
                                </select>
                                <div class="SearchDropDown AnimateMe" data-requiredfieldsname="Owner" data-requiredfields=".InputStorerKey" data-url="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Popup_Items", Nothing)) & "?storer=.InputStorerKey" %>"></div>
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Price<span>*</span></div>
                            <div>
                                <input type="text" class="textRecordStyle InputPrice" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Currency<span>*</span></div>
                            <div>
                                <select data-placeholder="Select Currency" multiple class="chosen-select InputCurrency" data-mode="single">
                                </select>
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">UDF1</div>
                            <div>
                                <input type="text" class="textRecordStyle InputSUsr1" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">UDF2</div>
                            <div>
                                <input type="text" class="textRecordStyle InputSUsr2" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">UDF3</div>
                            <div>
                                <input type="text" class="textRecordStyle InputSUsr3" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">UDF4</div>
                            <div>
                                <input type="text" class="textRecordStyle InputSUsr4" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">UDF5</div>
                            <div>
                                <input type="text" class="textRecordStyle InputSUsr5" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div style="position: relative; height: 15px;"></div>
        <div class="New_Modify_Record_PopUp">
            <div style="position: relative;">
                <div class="MyAbso_Record_PopUpContainer">

                    <div class="R_PopupTitle">
                        Item Catalogue
                        <div class="SaveRecordNow AnimateMe" id="btnSave" runat="server"></div>
                        <div class="ClosePopup AnimateMe"></div>
                    </div>
                    <div style="position: relative; height: 500px; width: 100%;" class="MyContainerPopup GetFullHeightForPopup content_4">
                        <div class="iWantMyChildrenFloatHeight">
                            <div class="floatL Width100 RecordHeader">
                                <div class="FloatRecord floatL">
                                    <input type="hidden" id="MyID" class="MyRecordID" value="0" />
                                    <div class="FloatRecordTitle floatL">Owner*</div>
                                    <div class="FloatRecordField floatL">
                                        <select data-placeholder="Select Owners" multiple class="chosen-select InputStorerKey InputAutoPostBack" data-mode="single">
                                        </select>
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Consignee*</div>
                                    <div class="FloatRecordField floatL">
                                        <select data-placeholder="Select Consignee" multiple class="chosen-select InputConsigneeKey" data-mode="single">
                                        </select>
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Item*</div>
                                    <div class="FloatRecordField floatL" style="position: relative;">
                                        <select data-placeholder="Select Item" multiple class="chosen-select InputSku" data-mode="single">
                                        </select>
                                        <div class="SearchDropDown AnimateMe" data-requiredfieldsname="Owner" data-requiredfields=".InputStorerKey" data-url="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Popup_Items", Nothing)) & "?storer=.InputStorerKey" %>"></div>
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Price*</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputPrice" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Currency*</div>
                                    <div class="FloatRecordField floatL">
                                        <select data-placeholder="Select Currency" multiple class="chosen-select InputCurrency" data-mode="single">
                                        </select>
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">UDF1</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputSUsr1" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">UDF2</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputSUsr2" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">UDF3</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputSUsr3" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">UDF4</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputSUsr4" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">UDF5</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputSUsr5" />
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
        <input type="hidden" class="HiddenDetailLink" value="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Configuration_ItemCatalogue", Nothing)) %>" />
        <input type="hidden" id="HiddenID" runat="server" class="HiddenID" value="0" />

        <input type="hidden" class="MyFields" value="Sku" data-columnname="Item" data-priority="1" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyFields" value="StorerKey" data-columnname="Owner" data-priority="2" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyFields" value="ConsigneeKey" data-columnname="Consignee" data-priority="3" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyFields" value="Price" data-columnname="Price" data-priority="4" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Currency" data-columnname="Currency" data-priority="5" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr1" data-columnname="UDF1" data-priority="6" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr2" data-columnname="UDF2" data-priority="7" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr3" data-columnname="UDF3" data-priority="8" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr4" data-columnname="UDF4" data-priority="9" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr5" data-columnname="UDF5" data-priority="10" data-hidden="false" />
    </div>
</asp:Content>