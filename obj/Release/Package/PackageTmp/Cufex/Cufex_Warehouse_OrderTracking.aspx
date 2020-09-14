<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Cufex/Cufex_Site.Master" CodeBehind="Cufex_Warehouse_OrderTracking.aspx.vb" Inherits="SNSsoftware.Cufex_Warehouse_OrderTracking" %>

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
                        <div class="MainPageTitle" data-id="Warehouse_OrderTracking">
                            Order Tracking
                        </div>
                        <div class="MainPageDesc" data-text="Order Tracking List">
                            Order Tracking List
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
                                <a id="btnAddNew" runat="server" class="btnAddNew AnimateMe DisplayNone">New Order
                                </a>
                            </td>
                            <td>
                                <a id="btnVAS" runat="server" class="btnVAS AnimateMe">VAS
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
                    <td class="GridCell GridHead selectAllWidth">
                        <div class="AdjustColumns"></div>
                    </td>
                    <td class="GridCell GridHead" data-id="ExternOrderKey">
                        <span class="MyTitleHead">External Order #</span>
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
                    <td class="GridCell GridHead" data-id="Facility">
                        <span class="MyTitleHead">Facility</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="CustomOrderDate">
                        <span class="MyTitleHead">Order Date</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="CustReqDate">
                        <span class="MyTitleHead">Requested Ship Date</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="CustActShipDate">
                        <span class="MyTitleHead">First Actual Ship Date</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="OrderType">
                        <span class="MyTitleHead">Order Type</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="PortalDescription">
                        <span class="MyTitleHead">Order Status</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Order_Transport_Status">
                        <span class="MyTitleHead">Transport Status</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="VASStatus">
                        <span class="MyTitleHead">VAS Status</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="CarrierName">
                        <span class="MyTitleHead">Carrier</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="C_Company">
                        <span class="MyTitleHead">Ship To</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="C_Address1">
                        <span class="MyTitleHead">Address</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="BuyerPO">
                        <span class="MyTitleHead">Buyer PO</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="SUsr1">
                        <span class="MyTitleHead">Reference 1</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="SUsr2">
                        <span class="MyTitleHead">Reference 2</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="SUsr3">
                        <span class="MyTitleHead">Reference 3</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="SUsr4">
                        <span class="MyTitleHead">Reference 4</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="SUsr5">
                        <span class="MyTitleHead">Reference 5</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="VASStartDate">
                        <span class="MyTitleHead">VAS Start Date</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead borderRight0" data-id="VASSEndDate">
                        <span class="MyTitleHead">VAS End Date</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                </tr>
                <tr class="GridRow SearchStyle" id="SearchRow" runat="server">
                    <td class="GridCell GridHeadSearch borderRight0 selectAllWidth">
                        <div class="EraseSearch"></div>
                    </td>
                    <td class="GridCell GridHeadSearch selectAllWidth">
                        <div class="GridSearch"></div>
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="ExternOrderKey" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="StorerKey" />
                    </td>
                    <td class="GridCell GridHeadSearch" style="overflow: visible;">
                        <select data-placeholder="Search" multiple class="chosen-select SearchClass InputFacility" data-id="Facility">
                        </select>
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="CustomOrderDate" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="CustReqDate" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="CustActShipDate" />
                    </td>
                    <td class="GridCell GridHeadSearch" style="overflow: visible;">
                        <select data-placeholder="Search" multiple class="chosen-select SearchClass InputOrderType" data-id="OrderType">
                        </select>
                    </td>
                    <td class="GridCell GridHeadSearch" style="overflow: visible;">
                        <select data-placeholder="Search" multiple class="chosen-select SearchClass InputPortalDescription" data-id="PortalDescription">
                        </select>
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Order_Transport_Status" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="VASStatus" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="CarrierName" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="C_Company" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="C_Address1" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="BuyerPO" />
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
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="SUsr5" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="VASStartDate" />
                    </td>
                    <td class="GridCell GridHeadSearch borderRight0">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="VASEndDate" />
                    </td>
                </tr>
                <tr class="GridRow NoResults">
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
                    <td class="GridCell GridContentCell borderRight0" data-id="18"></td>
                    <td class="GridCell GridContentCell borderRight0" data-id="19"></td>
                    <td class="GridCell GridContentCell borderRight0" data-id="20"></td>
                    <td class="GridCell GridContentCell borderRight0" data-id="21"></td>
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
                            <div class="FloatRecordTitleNew floatL">External Order Key</div>
                            <div>
                                <input type="text" class="textRecordStyle InputExternOrderKey" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Owner</div>
                            <div>
                                <input type="text" class="textRecordStyle InputStorerKey" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Facility</div>
                            <div>
                                <input type="text" class="textRecordStyle InputFacility" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Order Date</div>
                            <div>
                                <input type="text" class="textRecordStyle InputCustomOrderDate" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Requested Ship Date</div>
                            <div>
                                <input type="text" class="textRecordStyle InputCustReqDate" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">First Actual Ship Date</div>
                            <div>
                                <input type="text" class="textRecordStyle InputCustActShipDate" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Order Type</div>
                            <div>
                                <input type="text" class="textRecordStyle InputOrderType" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Order Status</div>
                            <div>
                                <input type="text" class="textRecordStyle InputPortalDescription" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Transort Status</div>
                            <div>
                                <input type="text" class="textRecordStyle InputOrder_Transport_Status" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">VAS Status</div>
                            <div>
                                <input type="text" class="textRecordStyle InputVASStatus" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Carrier Name</div>
                            <div>
                                <input type="text" class="textRecordStyle InputCarrierName" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Ship To Name</div>
                            <div>
                                <input type="text" class="textRecordStyle InputC_Company" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Ship To City</div>
                            <div>
                                <input type="text" class="textRecordStyle InputC_City" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Ship To State</div>
                            <div>
                                <input type="text" class="textRecordStyle InputC_State" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Ship To Zip</div>
                            <div>
                                <input type="text" class="textRecordStyle InputC_Zip" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Ship To Address Line 1</div>
                            <div>
                                <input type="text" class="textRecordStyle InputC_Address1" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Ship To Address Line 2</div>
                            <div>
                                <input type="text" class="textRecordStyle InputC_Address2" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Ship To Address Line 3</div>
                            <div>
                                <input type="text" class="textRecordStyle InputC_Address3" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Ship To Address Line 4</div>
                            <div>
                                <input type="text" class="textRecordStyle InputC_Address4" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Ship To Address Line 5</div>
                            <div>
                                <input type="text" class="textRecordStyle InputC_Address5" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Buyer PO</div>
                            <div>
                                <input type="text" class="textRecordStyle InputBuyerPO" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Reference 1</div>
                            <div>
                                <input type="text" class="textRecordStyle InputSUsr1" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Reference 2</div>
                            <div>
                                <input type="text" class="textRecordStyle InputSUsr2" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Reference 3</div>
                            <div>
                                <input type="text" class="textRecordStyle InputSUsr3" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Reference 4</div>
                            <div>
                                <input type="text" class="textRecordStyle InputSUsr4" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Reference 5</div>
                            <div>
                                <input type="text" class="textRecordStyle InputSUsr5" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">VAS Start Date</div>
                            <div>
                                <input type="text" class="textRecordStyle InputVASStartDate" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">VAS End Date</div>
                            <div>
                                <input type="text" class="textRecordStyle InputVASEndDate" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Number of Facilities</div>
                            <div>
                                <input type="text" class="textRecordStyle InputNbrOfSplit" disabled="disabled" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew floatL">Date of First POD</div>
                            <div>
                                <input type="text" class="textRecordStyle InputPOD" disabled="disabled" />
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
                                    <a id="btnExportDetails" runat="server" class="btnExportDetails AnimateMe" title="Export"></a>
                                </td>
                                <td style="width: 13px;"></td>
                                <td>
                                    <div class="VerticalSep2"></div>
                                </td>
                                <td style="width: 13px;"></td>
                                <td>
                                    <a id="btnRefreshDetails" runat="server" class="btnRefreshDetails AnimateMe" title="Refresh"></a>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="floatL Width100 PositionRelative DetailsGridView content_3" style="padding-top: 30px; display: none;">
                        <table class="GridContainer" data-resizemode="overflow">
                            <tr class="GridRow GridAdjust">
                                <td class="GridCell GridHead borderRight0 selectAllWidth"></td>
                                <td class="GridCell GridHead selectAllWidth">
                                    <div class="AdjustColumns"></div>
                                </td>
                                <td class="GridCell GridHead" data-id="Facility">
                                    <span class="MyTitleHead">Facility</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="OrderKey">
                                    <span class="MyTitleHead">Order #</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="OrderLineNumber">
                                    <span class="MyTitleHead">Line #</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="ExternLineNo">
                                    <span class="MyTitleHead">External Line #</span>
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
                                <td class="GridCell GridHead" data-id="SkuDescription">
                                    <span class="MyTitleHead">Item Description</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="CarrierName">
                                    <span class="MyTitleHead">Carrier</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="PackKey">
                                    <span class="MyTitleHead">Pack</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="OriginalQty">
                                    <span class="MyTitleHead">Order Qty</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="OpenQty">
                                    <span class="MyTitleHead">Open Qty</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="AvailableQty">
                                    <span class="MyTitleHead">Available Qty</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="QtyAllocated">
                                    <span class="MyTitleHead">Allocated Qty</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="QtyPicked">
                                    <span class="MyTitleHead">Picked Qty</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="ShippedQty">
                                    <span class="MyTitleHead">Shipped Qty</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead borderRight0" data-id="PortalDescription">
                                    <span class="MyTitleHead">Status</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                            </tr>
                            <tr class="GridRow SearchStyle" id="SearchRowDetails" runat="server">
                                <td class="GridCell GridHeadSearch borderRight0 selectAllWidth">
                                    <div class="EraseSearch"></div>
                                </td>
                                <td class="GridCell GridHeadSearch selectAllWidth">
                                    <div class="GridSearch"></div>
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="Facility" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="OrderKey" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="OrderLineNumber" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="ExternLineNo" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="Sku" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="SkuDescription" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="CarrierName" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="PackKey" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="OriginalQty" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="OpenQty" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="AvailableQty" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="QtyAllocated" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="QtyPicked" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="ShippedQty" />
                                </td>
                                <td class="GridCell GridHeadSearch borderRight0">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="PortalDescription" />
                                </td>
                            </tr>
                            <tr class="GridRow NoResults">
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
                </div>
            </div>
        </div>

        <div style="position: relative; height: 15px;"></div>

        <div class="New_Modify_Record_PopUp">
            <div style="position: relative;">
                <div class="MyAbso_Record_PopUpContainer">
                    <div class="R_PopupTitle">
                        Order Tracking and Details
                        <div class="ClosePopup AnimateMe"></div>
                    </div>
                    <div style="position: relative; height: 500px; width: 100%;" class="MyContainerPopup GetFullHeightForPopup content_4">
                        <div class="iWantMyChildrenFloatHeight">
                            <div class="floatL Width100 RecordHeader">
                                <div class="FloatRecord floatL">
                                    <input type="hidden" id="MyID" class="MyRecordID" value="0" />
                                    <div class="FloatRecordTitle floatL">External Order Key</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputExternOrderKey" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Owner</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputStorerKey" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Facility</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputFacility" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Order Date</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputCustomOrderDate" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Requested Ship Date</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputCustReqDate" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">First Actual Ship Date</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputCustActShipDate" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Order Type</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputOrderType" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Order Status</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputPortalDescription" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Transport Status</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputOrder_Transport_Status" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">VAS Status</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputVASStatus" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Carrier Name</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputCarrierName" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Ship To Name</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputC_Company" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Ship To City</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputC_City" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Ship To State</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputC_State" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Ship To Zip</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputC_Zip" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Ship To Address Line 1</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputC_Address1" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Ship To Address Line 2</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputC_Address2" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Ship To Address Line 3</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputC_Address3" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Ship To Address Line 4</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputC_Address4" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Ship To Address Line 5</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputC_Address5" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Buyer PO</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputBuyerPO" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Reference 1</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputSUsr1" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Reference 2</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputSUsr2" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Reference 3</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputSUsr3" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Reference 4</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputSUsr4" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Reference 5</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputSUsr5" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">VAS Start Date</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputVASStartDate" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">VAS End Date</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputVASEndDate" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Number of Facilities</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputNbrOfSplit" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Date of First POD</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputPOD" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                            </div>
                        </div>
                        <div style="position: relative; margin-bottom: 20px;">
                            <div class="RecordsContainer">
                                <div class="RecordsContainer_Inside" style="display: none; position: relative;">
                                    <div class="iWantMyChildrenFloatHeight">
                                        <div class="floatL Width100">
                                            <div class="Details_FloatRecord floatL">
                                                <input type="hidden" class="MyDetailRecordID" value="0" />
                                                <div class="Details_FloatRecordTitle floatL">Facility</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsFacility" disabled="disabled" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Order #</div>
                                                <div class="Details_FloatRecordField floatL" style="position: relative;">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsOrderKey" disabled="disabled" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Line #</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsOrderLineNo" disabled="disabled" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">External Line #</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsExternLineNo" disabled="disabled" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Item</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsSku" disabled="disabled" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Item Description</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsSkuDescription" disabled="disabled" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Carrier</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsCarrierName" disabled="disabled" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Pack</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsPackKey" disabled="disabled" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Order Qty</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsOriginalQty" disabled="disabled" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Open Qty</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsOpenQty" disabled="disabled" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Available Qty</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsAvailableQty" disabled="disabled" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Allocated Qty</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsQtyAllocated" disabled="disabled" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Picked Qty</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsQtyPicked" disabled="disabled" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Shipped Qty</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsShippedQty" disabled="disabled" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Status</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsPortalDescription" disabled="disabled" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <%--<div class="AddDetailsBtn AnimateMe">ADD DETAIL</div>--%>
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

        <div class="VAS_PopUp">
            <div style="position: relative;">
                <div class="MyAbso_VAS_PopUpContainer">
                    <div class="CloseVASPopup"></div>
                    <div class="MainVAS">
                        <div class="iWantMyChildrenFloatHeight">
                            <div class="floatL Width100">
                                <div class="floatL Width100 VASDateLabel">
                                    Start Date & Time
                                </div>
                                <div class="floatL VASDate">
                                    <input type="text" class="textRecordStyle datepicker" placeholder="MM/DD/YYYY" />
                                </div>
                                <div class="floatL VASTime">
                                    <div class="floatL Hours">
                                        <input type="text" class="textRecordStyle NumericInput" placeholder="hh" maxlength="2" />
                                    </div>
                                    <div class="floatL TwoPoints">:</div>
                                    <div class="floatL Minutes">
                                        <input type="text" class="textRecordStyle NumericInput" placeholder="mm" maxlength="2" />
                                    </div>
                                    <div class="floatL TwoPoints">:</div>
                                    <div class="floatL Seconds">
                                        <input type="text" class="textRecordStyle NumericInput" placeholder="ss" maxlength="2" />
                                    </div>
                                    <div class="floatL AMPM">
                                        <input type="text" class="textRecordStyle" placeholder="tt" style="cursor:pointer;" />
                                        <div class="TimeOptions">
                                            <div class="TimeLabel TimeLabel1 AnimateMe">AM</div>
                                            <div class="TimeLabel TimeLabel2 AnimateMe">PM</div>
                                        </div>
                                    </div>
                                </div>
                                <div class="floatL VASSettings">
                                </div>
                                <div class="floatL Width100 VASDateLabel End">
                                    End Date & Time
                                </div>
                                <div class="floatL VASDate">
                                    <input type="text" class="textRecordStyle datepicker" placeholder="MM/DD/YYYY" />
                                </div>
                                <div class="floatL VASTime">
                                    <div class="floatL Hours">
                                        <input type="text" class="textRecordStyle NumericInput" placeholder="hh" maxlength="2" />
                                    </div>
                                    <div class="floatL TwoPoints">:</div>
                                    <div class="floatL Minutes">
                                        <input type="text" class="textRecordStyle NumericInput" placeholder="mm" maxlength="2" />
                                    </div>
                                    <div class="floatL TwoPoints">:</div>
                                    <div class="floatL Seconds">
                                        <input type="text" class="textRecordStyle NumericInput" placeholder="ss" maxlength="2" />
                                    </div>
                                    <div class="floatL AMPM">
                                        <input type="text" class="textRecordStyle" placeholder="tt" style="cursor:pointer;" />
                                        <div class="TimeOptions">
                                            <div class="TimeLabel TimeLabel1 AnimateMe">AM</div>
                                            <div class="TimeLabel TimeLabel2 AnimateMe">PM</div>
                                        </div>
                                    </div>
                                </div>
                                <div class="floatL VASSettings">
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="FormSettings">
        <input type="hidden" id="NumberOfRecordsInPage" value="10" />
        <input type="hidden" id="SortBy" value="CustomOrderDate desc" />
        <input type="hidden" class="HiddenDetailLink" value="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Warehouse_OrderTracking", Nothing)) %>" />
        <input type="hidden" id="HiddenID" runat="server" class="HiddenID" value="0" />

        <input type="hidden" class="MyFields" value="ExternOrderKey" data-columnname="External Order #" data-priority="1" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyFields" value="StorerKey" data-columnname="Owner" data-priority="2" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyFields" value="Facility" data-columnname="Facility" data-priority="3" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyFields" value="CustomOrderDate" data-columnname="Order Date" data-priority="4" data-hidden="false" />
        <input type="hidden" class="MyFields" value="CustReqDate" data-columnname="Requested Ship Date" data-priority="5" data-hidden="false" />
        <input type="hidden" class="MyFields" value="CustActShipDate" data-columnname="First Actual Ship Date" data-priority="6" data-hidden="false" />
        <input type="hidden" class="MyFields" value="OrderType" data-columnname="Order Type" data-priority="7" data-hidden="false" />
        <input type="hidden" class="MyFields" value="PortalDescription" data-columnname="Order Status" data-priority="8" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Order_Transport_Status" data-columnname="Transport Status" data-priority="9" data-hidden="false" />
        <input type="hidden" class="MyFields" value="VASStatus" data-columnname="VAS Status" data-priority="10" data-hidden="false" />
        <input type="hidden" class="MyFields" value="CarrierName" data-columnname="Carrier" data-priority="11" data-hidden="false" />
        <input type="hidden" class="MyFields" value="C_Company" data-columnname="Ship To" data-priority="12" data-hidden="false" />
        <input type="hidden" class="MyFields" value="C_Address1" data-columnname="Address" data-priority="13" data-hidden="false" />
        <input type="hidden" class="MyFields" value="BuyerPO" data-columnname="Buyer PO" data-priority="14" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr1" data-columnname="Reference 1" data-priority="15" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr2" data-columnname="Reference 2" data-priority="16" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr3" data-columnname="Reference 3" data-priority="17" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr4" data-columnname="Reference 4" data-priority="18" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr5" data-columnname="Reference 5" data-priority="19" data-hidden="false" />
        <input type="hidden" class="MyFields" value="VASStartDate" data-columnname="VAS Start Date" data-priority="20" data-hidden="false" />
        <input type="hidden" class="MyFields" value="VASEndDate" data-columnname="VAS End Date" data-priority="21" data-hidden="false" />

        <input type="hidden" class="MyDetailsFields" value="Facility" data-columnname="Facility" data-priority="1" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyDetailsFields" value="OrderKey" data-columnname="Order #" data-priority="2" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyDetailsFields" value="OrderLineNumber" data-columnname="Line #" data-priority="3" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyDetailsFields" value="ExternLineNo" data-columnname="External Line #" data-priority="4" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyDetailsFields" value="Sku" data-columnname="Item" data-priority="5" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyDetailsFields" value="SkuDescription" data-columnname="Item Description" data-priority="6" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="CarrierName" data-columnname="Carrier" data-priority="7" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="PackKey" data-columnname="Pack" data-priority="8" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="OriginalQty" data-columnname="Order Qty" data-priority="9" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="OpenQty" data-columnname="Open Qty" data-priority="10" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="AvailableQty" data-columnname="Available Qty" data-priority="11" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="QtyAllocated" data-columnname="Allocated Qty" data-priority="12" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="QtyPicked" data-columnname="Picked Qty" data-priority="13" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="ShippedQty" data-columnname="Shipped Qty" data-priority="14" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="PortalDescription" data-columnname="Status" data-priority="15" data-hidden="false" />
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Cufex_ScriptContent" runat="server">
    <script async defer src="<%= "https://maps.googleapis.com/maps/api/js?key=" & ConfigurationManager.AppSettings("GoogleMapAPIKey") & "&callback=initMap" %>"></script>
</asp:Content>
