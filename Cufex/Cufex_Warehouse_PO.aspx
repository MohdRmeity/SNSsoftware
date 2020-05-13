﻿<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Cufex/Cufex_Site.Master" CodeBehind="Cufex_Warehouse_PO.aspx.vb" Inherits="SNSsoftware.Cufex_Warehouse_PO" %>

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
                        <div class="MainPageTitle" data-id="Warehouse_PO">
                            Purchase Order
                        </div>
                        <div class="MainPageDesc" data-text="Orders List">
                            Orders List
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
                                <a id="btnAddNew" runat="server" class="btnAddNew AnimateMe">New PO
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
            <table class="GridContainer" data-resizemode="overflow">
                <tr class="GridRow GridAdjust">
                    <td class="GridCell GridHead borderRight0 selectAllWidth"></td>
                    <td class="GridCell GridHead borderRight0 selectAllWidth"></td>
                    <td class="GridCell GridHead selectAllWidth">
                        <div class="AdjustColumns"></div>
                    </td>
                    <td class="GridCell GridHead" data-id="POKey">
                        <span class="MyTitleHead">PO Key</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Facility">
                        <span class="MyTitleHead">Facility</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="ExternPOKey">
                        <span class="MyTitleHead">Extern PO</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="PODate">
                        <span class="MyTitleHead">PO Date</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Status">
                        <span class="MyTitleHead">Status</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="POType">
                        <span class="MyTitleHead">Type</span>
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
                    <td class="GridCell GridHead" data-id="BuyerName">
                        <span class="MyTitleHead">Buyer</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="BuyersReference">
                        <span class="MyTitleHead">Buyer Reference</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="SellerName">
                        <span class="MyTitleHead">Supplier</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="SellersReference">
                        <span class="MyTitleHead">Seller Reference</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="EffectiveDate">
                        <span class="MyTitleHead">Effective Date</span>
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
                        <input type="text" placeholder="Search" class="SearchClass" data-id="POKey" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Facility" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="ExternPOKey" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="CAST(PODate AS date)" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Status" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="POType" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="StorerKey" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="BuyerName" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="BuyersReference" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="SellerName" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="SellersReference" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="CAST(EffectiveDate AS date)" />
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
                    <td class="GridCell GridContentCell borderRight0" data-id="11"></td>
                    <td class="GridCell GridContentCell borderRight0" data-id="12"></td>
                    <td class="GridCell GridContentCell borderRight0" data-id="13"></td>
                    <td class="GridCell GridContentCell borderRight0" data-id="14"></td>
                    <td class="GridCell GridContentCell borderRight0" data-id="15"></td>
                    <td class="GridCell GridContentCell borderRight0" data-id="16"></td>
                    <td class="GridCell GridContentCell borderRight0" data-id="17"></td>
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
                            <div class="FloatRecordTitleNew">Facility<span>*</span></div>
                            <div>
                                <select data-placeholder="Select Facilities" multiple class="chosen-select InputFacility InputAutoPostBack">
                                </select>
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">PO Key</div>
                            <div>
                                <input type="text" class="textRecordStyle InputPOKey" data-value="Auto Generated" data-disabled="" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Extern PO</div>
                            <div>
                                <input type="text" class="textRecordStyle InputExternPOKey" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">PO Date</div>
                            <div>
                                <input type="text" class="textRecordStyle datepicker InputPODate" data-value="<%= Format(Now, "MM/dd/yyyy")  %>" data-disabled="" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Status</div>
                            <div>
                                <input type="text" class="textRecordStyle InputStatus" data-value="New" data-disabled="" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Type<span>*</span></div>
                            <div>
                                <select data-placeholder="Select Types" multiple class="chosen-select InputPOType">
                                </select>
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Owner<span>*</span></div>
                            <div>
                                <select data-placeholder="Select Owners" multiple class="chosen-select InputStorerKey">
                                </select>
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Buyer</div>
                            <div>
                                <input type="text" class="textRecordStyle InputBuyerName" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Buyer Reference</div>
                            <div>
                                <input type="text" class="textRecordStyle InputBuyersReference" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Supplier</div>
                            <div>
                                <select data-placeholder="Select Suppliers" multiple class="chosen-select InputSellerName">
                                </select>
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Seller Reference</div>
                            <div>
                                <input type="text" class="textRecordStyle InputSellersReference" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Effective Date</div>
                            <div>
                                <input type="text" class="textRecordStyle datepicker InputEffectiveDate" data-value="<%= Format(Now, "MM/dd/yyyy")  %>" />
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
                    <div class="floatL Width100 RecordDetail">
                        <div class="floatL">
                            <div class="MainPageTitle MainPageDetailTitle" data-text="Records Details">
                                Records Details
                            </div>
                        </div>
                        <table class="floatR ActionsDetails" style="display: none;">
                            <tr>
                                <td>
                                    <div class="BackBtn BackDetail AnimateMe" style="display: none;">
                                        Back to List
                                    </div>
                                </td>
                                <td>
                                    <div class="VerticalSep" style="display: none;"></div>
                                </td>
                                <td>
                                    <a id="btnNew" runat="server" class="btnNew AnimateMe">New
                                    </a>
                                </td>
                                <td style="width: 13px; display: none;"></td>
                                <td>
                                    <a id="btnSaveDetail" runat="server" class="btnSaveDetail AnimateMe" style="display: none;">Add
                                    </a>
                                </td>
                                <td style="width: 13px;"></td>
                                <td>
                                    <a id="btnDeleteDetail" runat="server" class="btnDeleteDetail AnimateMe">Delete
                                    </a>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="floatL Width100 PositionRelative DetailsGridView content_3" style="padding-top: 30px; display: none;">
                        <table class="GridContainer" data-resizemode="fit">
                            <tr class="GridRow GridAdjust">
                                <td class="GridCell GridHead borderRight0 selectAllWidth"></td>
                                <td class="GridCell GridHead borderRight0 selectAllWidth"></td>
                                <td class="GridCell GridHead selectAllWidth">
                                    <div class="AdjustColumns"></div>
                                </td>
                                <td class="GridCell GridHead" data-id="ExternLineNo">
                                    <span class="MyTitleHead">Extern Line#</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="Sku">
                                    <span class="MyTitleHead">Item</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="QtyOrdered">
                                    <span class="MyTitleHead">Open Ordered</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead borderRight0" data-id="QtyReceived">
                                    <span class="MyTitleHead">Qty Received</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                            </tr>
                            <tr class="GridRow SearchStyle" id="SearchRowDetails" runat="server">
                                <td class="GridCell GridHead selectAllWidth">
                                    <input class="CheckBoxCostumizedNS2 chkSelectAll" type="checkbox" id="chkSelectAllDtl" />
                                    <label for="chkSelectAllDtl"><span class="CheckBoxStyle"></span></label>
                                </td>
                                <td class="GridCell GridHeadSearch borderRight0 selectAllWidth">
                                    <div class="EraseSearch"></div>
                                </td>
                                <td class="GridCell GridHeadSearch selectAllWidth">
                                    <div class="GridSearch"></div>
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="ExternLineNo" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="Sku" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="QtyOrdered" />
                                </td>
                                <td class="GridCell GridHeadSearch borderRight0">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="QtyReceived" />
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
                    <div class="floatL Width100 NewDetailRecord">
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Extern Line#</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsExternLineNo" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew floatL">Item<span>*</span></div>
                            <div>
                                <select data-placeholder="Select Items" multiple class="chosen-select InputDetailsSku">
                                </select>
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Qty Ordered<span>*</span></div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsQtyOrdered" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Qty Received</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsQtyReceived" data-value="0" data-disabled="" />
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
                        PO and Details
                        <div class="SaveRecordNow AnimateMe" id="btnSave" runat="server"><span class="ion-ios-checkmark-circle-outline"></span></div>
                        <div class="ClosePopup AnimateMe"><span class="ion-ios-exit"></span></div>
                    </div>
                    <div style="position: relative; height: 500px; width: 100%;" class="MyContainerPopup GetFullHeightForPopup content_4">
                        <div class="iWantMyChildrenFloatHeight">
                            <div class="floatL Width100 RecordHeader">
                                <div class="FloatRecord floatL">
                                    <input type="hidden" id="MyID" class="MyRecordID" value="0" />
                                    <div class="FloatRecordTitle floatL">Facility*</div>
                                    <div class="FloatRecordField floatL">
                                        <select data-placeholder="Select Facilities" multiple class="chosen-select InputFacility InputAutoPostBack">
                                        </select>
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">PO Key</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputPOKey" data-value="Auto Generated" data-disabled="" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Extern PO</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputExternPOKey" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">PO Date</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle datepicker InputPODate" data-value="<%= Format(Now, "MM/dd/yyyy")  %>" data-disabled="" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Status</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputStatus" data-value="New" data-disabled="" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Type*</div>
                                    <div class="FloatRecordField floatL">
                                        <select data-placeholder="Select Types" multiple class="chosen-select InputPOType">
                                        </select>
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Owner*</div>
                                    <div class="FloatRecordField floatL">
                                        <select data-placeholder="Select Owners" multiple class="chosen-select InputStorerKey">
                                        </select>
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Buyer</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputBuyerName" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Buyer Reference</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputBuyersReference" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Supplier</div>
                                    <div class="FloatRecordField floatL">
                                        <select data-placeholder="Select Suppliers" multiple class="chosen-select InputSellerName">
                                        </select>
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Seller Reference</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputSellersReference" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Effective Date</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle datepicker InputEffectiveDate" data-value="<%= Format(Now, "MM/dd/yyyy")  %>" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">UDF1</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputSUsr1" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">UDF2</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputSUsr2" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">UDF3</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputSUsr3" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">UDF4</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputSUsr4" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">UDF5</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputSUsr5" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                            </div>
                        </div>
                        <%--<div class="AddDetailsBtn AnimateMe">ADD DETAIL</div>--%>
                        <div style="position: relative; margin-bottom: 20px;">
                            <div class="RecordsContainer">
                                <div class="RecordsContainer_Inside" style="display: none; position: relative;">
                                    <div class="btnDeleteDtl AnimateMe"></div>
                                    <div class="iWantMyChildrenFloatHeight">
                                        <div class="floatL Width100">
                                            <div class="Details_FloatRecord floatL">
                                                <input type="hidden" class="MyDetailRecordID" value="0" />
                                                <div class="Details_FloatRecordTitle floatL">Extern Line#</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsExternLineNo" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Item*</div>
                                                <div class="Details_FloatRecordField floatL" style="position: relative;">
                                                    <select data-placeholder="Select Items" multiple class="chosen-select InputDetailsSku">
                                                    </select>
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Qty Ordered*</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsQtyOrdered" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Qty Received</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsQtyReceived" data-value="0" data-disabled="" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="AddDetailsBtn AnimateMe">ADD DETAIL</div>
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
        <input type="hidden" class="HiddenDetailLink" value="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Warehouse_PO", Nothing)) %>" />
        <input type="hidden" id="HiddenID" runat="server" class="HiddenID" value="0" />

        <input type="hidden" class="MyFields" value="POKey" data-columnname="PO Key" data-priority="1" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyFields" value="Facility" data-columnname="Facility" data-priority="2" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyFields" value="ExternPOKey" data-columnname="Extern PO" data-priority="3" data-hidden="false" />
        <input type="hidden" class="MyFields" value="PODate" data-columnname="PO Date" data-priority="4" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Status" data-columnname="Status" data-priority="5" data-hidden="false" />
        <input type="hidden" class="MyFields" value="POType" data-columnname="Type" data-priority="6" data-hidden="false" />
        <input type="hidden" class="MyFields" value="StorerKey" data-columnname="Owner" data-priority="7" data-hidden="false" />
        <input type="hidden" class="MyFields" value="BuyerName" data-columnname="Buyer Name" data-priority="8" data-hidden="false" />
        <input type="hidden" class="MyFields" value="BuyersReference" data-columnname="Buyers Reference" data-priority="9" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SellerName" data-columnname="Seller Name" data-priority="10" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SellersReference" data-columnname="Seller Reference" data-priority="11" data-hidden="false" />
        <input type="hidden" class="MyFields" value="EffectiveDate" data-columnname="Effecive Date" data-priority="12" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr1" data-columnname="UDF1" data-priority="13" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr2" data-columnname="UDF2" data-priority="14" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr3" data-columnname="UDF3" data-priority="15" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr4" data-columnname="UDF4" data-priority="16" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr5" data-columnname="UDF5" data-priority="17" data-hidden="false" />

        <input type="hidden" class="MyDetailsFields" value="ExternLineNo" data-columnname="Extern Line#" data-priority="1" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyDetailsFields" value="Sku" data-columnname="Item" data-priority="2" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyDetailsFields" value="QtyOrdered" data-columnname="Qty Ordered" data-priority="3" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="QtyReceived" data-columnname="Qty Received" data-priority="4" data-hidden="false" />
    </div>
</asp:Content>
