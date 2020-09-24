<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Cufex/Cufex_Site.Master" CodeBehind="Cufex_Warehouse_OrderManagement.aspx.vb" Inherits="SNSsoftware.Cufex_Warehouse_OrderManagement" %>

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
                        <div class="MainPageTitle" data-id="Warehouse_OrderManagement">
                            Order Management
                        </div>
                        <div class="MainPageDesc" data-text="Orders List">
                            Orders List
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
                                <a id="btnAddNew" runat="server" class="btnAddNew AnimateMe">New Order
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
                            <td id="TableAction1" runat="server" style="width: 13px;"></td>
                            <td id="TableAction2" runat="server">
                                <div class="btnActions">Actions</div>
                                <div class="ActionHiddenButtons">
                                    <!-- Mohamad Rmeity - Changing label from Release to SCE to Release to WMS -->
                                    <div class="BtnDoSomeThing AnimateMe" data-id="1"><span class="ion-ios-trending-up MyFontIon"></span><span style="vertical-align: 4px;">Release to WMS</span></div>
                                    <div class="BtnDoSomeThing AnimateMe" data-id="2"><span class="ion-ios-trending-up MyFontIon"></span><span style="vertical-align: 4px;">Release & Allocate</span></div>
                                </div>
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
                    <td class="GridCell GridHead" data-id="OrderManagKey">
                        <!-- Mohamad Rmeity - Changing label from Order Manag Key to Order Management No. -->
                        <span class="MyTitleHead">Order Management No.</span>
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
                    <td class="GridCell GridHead" data-id="StorerKey">
                        <span class="MyTitleHead">Owner</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="ExternOrderKey">
                        <!-- Mohamad Rmeity - Changing label from Extern Order Key to External Order No. -->
                        <span class="MyTitleHead">External Order No.</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <!-- Mohamad Rmeity - Changing label from Consignee to Ship To -->
                    <td class="GridCell GridHead" data-id="ConsigneeKey">
                        <span class="MyTitleHead">Ship To</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="ConsigneeName">
                        <span class="MyTitleHead">Ship To Name</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="OrderManagStatus">
                        <span class="MyTitleHead">Status</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Type">
                        <span class="MyTitleHead">Type</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="OrderDate">
                        <span class="MyTitleHead">Order Date</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="RequestedShipDate">
                        <span class="MyTitleHead">Req Ship Date</span>
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
                        <input type="text" placeholder="Search" class="SearchClass" data-id="OrderManagKey" />
                    </td>
                    <td class="GridCell GridHeadSearch" style="overflow: visible;">
                        <select data-placeholder="Search" multiple class="chosen-select SearchClass InputFacility" data-id="Facility">
                        </select>
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="StorerKey" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="ExternOrderKey" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="ConsigneeKey" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="ConsigneeName" />
                    </td>
                    <td class="GridCell GridHeadSearch" style="overflow: visible;">
                        <select data-placeholder="Search" multiple class="chosen-select SearchClass InputOrderManagStatusSearch" data-id="OrderManagStatus">
                            <option value="CREATED in SCE">CREATED in SCE</option>
                            <option value="CREATED & ALLOCATED">CREATED & ALLOCATED</option>
                            <option value="NOT CREATED in SCE">NOT CREATED in SCE</option>
                        </select>
                    </td>
                    <td class="GridCell GridHeadSearch" style="overflow: visible;">
                        <select data-placeholder="Search" multiple class="chosen-select SearchClass InputOrderTypeSearch" data-id="Type">
                        </select>
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="CAST(OrderDate AS date)" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="CAST(RequestedShipDate AS date)" />
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
                                <select data-placeholder="Select Facilities" multiple class="chosen-select InputFacility InputAutoPostBack" data-mode="single">
                                </select>
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <!-- Mohamad Rmeity - Changing label from Order Key to Order Management No. -->
                            <div class="FloatRecordTitleNew">Order Management No.</div>
                            <div>
                                <input type="text" class="textRecordStyle InputOrderManagKey" data-value="Auto Generated" data-disabled="" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <!-- Mohamad Rmeity - Changing label from Extern Order Key to External Order No. -->
                            <div class="FloatRecordTitleNew">External Order No.</div>
                            <div>
                                <input type="text" class="textRecordStyle InputExternOrderKey" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Owner<span>*</span></div>
                            <div>
                                <select data-placeholder="Select Owners" multiple class="chosen-select InputStorerKey" data-mode="single">
                                </select>
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Order Date</div>
                            <div>
                                <input type="text" class="textRecordStyle datepicker InputOrderDate" data-value="<%= Format(Now, "MM/dd/yyyy")  %>" data-disabled="" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Status</div>
                            <div>
                                <input type="text" class="textRecordStyle InputOrderManagStatus InputStatus" data-value="Created Externally" data-disabled="" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Type<span>*</span></div>
                            <div>
                                <select data-placeholder="Select Types" multiple class="chosen-select InputType" data-mode="single">
                                </select>
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <!-- Mohamad Rmeity - Changing label from Consignee to Ship To -->
                            <div class="FloatRecordTitleNew">Ship To</div>
                            <div>
                                <select data-placeholder="Select Ship To" multiple class="chosen-select InputConsigneeKey" data-mode="single">
                                </select>
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Req Ship Date</div>
                            <div>
                                <input type="text" class="textRecordStyle datepicker InputRequestedShipDate" placeholder="MM/DD/YYYY" />
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
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">WMS Order Key</div>
                            <div>
                                <input type="text" class="textRecordStyle InputWMSOrderKey" data-disabled="" />
                            </div>
                        </div>
                        <div class="floatL Width100" style="padding-top: 10px">
                            <div class="dropzone"></div>
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
                                <td style="width: 13px;"></td>
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
                                <td style="width: 13px;"></td>
                                <td>
                                    <a id="btnNew" runat="server" class="btnNew AnimateMe">New
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
                        <table class="GridContainer" data-resizemode="overflow">
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
                                <td class="GridCell GridHead" data-id="OpenQty" data-numeric="true">
                                    <span class="MyTitleHead">Open Qty</span>
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
                                <td class="GridCell GridHead" data-id="UOM">
                                    <span class="MyTitleHead">UOM</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="UnitPrice" data-numeric="true">
                                    <span class="MyTitleHead">Price</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="SUsr5">
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
                                <td class="GridCell GridHead" data-id="Lottable01">
                                    <span class="MyTitleHead">Lottable01</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="Lottable02">
                                    <span class="MyTitleHead">Lottable02</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="Lottable03">
                                    <span class="MyTitleHead">Lottable03</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="Lottable04">
                                    <span class="MyTitleHead">Lottable04</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="Lottable05">
                                    <span class="MyTitleHead">Lottable05</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="Lottable06">
                                    <span class="MyTitleHead">Lottable06</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="Lottable07">
                                    <span class="MyTitleHead">Lottable07</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="Lottable08">
                                    <span class="MyTitleHead">Lottable08</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="Lottable09">
                                    <span class="MyTitleHead">Lottable09</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead borderRight0" data-id="Lottable10">
                                    <span class="MyTitleHead">Lottable10</span>
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
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="OpenQty" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="PackKey" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="UOM" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="UnitPrice" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="SUsr5" />
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
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="Lottable01" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="Lottable02" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="Lottable03" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="Cast(Lottable04 As Date)" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="Cast(Lottable05 As Date)" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="Lottable06" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="Lottable07" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="Lottable08" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="Lottable09" />
                                </td>
                                <td class="GridCell GridHeadSearch borderRight0">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="Lottable10" />
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
                    <div class="floatL Width100 NewDetailRecord">
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Extern Line#</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsExternLineNo" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew floatL">Item<span>*</span></div>
                            <div class="PositionRelative">
                                <select data-placeholder="Select Items" multiple class="chosen-select InputDetailsSku" data-mode="single">
                                </select>
                                <div class="SearchDropDown AnimateMe" data-requiredfieldsname="Facility,Owner" data-requiredfields=".InputFacility,.InputStorerKey" data-url="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Popup_Items", Nothing)) & "?warehouse=.InputFacility&storer=.InputStorerKey" %>"></div>
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Open Qty<span>*</span></div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsOpenQty" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Pack<span>*</span></div>
                            <div class="PositionRelative">
                                <select data-placeholder="Select Packs" multiple class="chosen-select InputDetailsPackKey InputAutoPostBackDetails" data-mode="single">
                                </select>
                                <div class="SearchDropDown AnimateMe" data-requiredfieldsname="Facility" data-requiredfields=".InputFacility" data-url="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Popup_Packs", Nothing)) & "?warehouse=.InputFacility" %>"></div>
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">UOM</div>
                            <div>
                                <select data-placeholder="Select UOMs" multiple class="chosen-select InputDetailsUOM" data-mode="single">
                                </select>
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Price</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsPrice" data-disabled="" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Currency</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsCurrency" data-disabled="" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">UDF1</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsSUsr1" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">UDF2</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsSUsr2" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">UDF3</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsSUsr3" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">UDF4</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsSUsr4" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Lottable01</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsLottable01" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew floatL">Lottable02</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsLottable02" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew floatL">Lottable03</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsLottable03" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Lottable04</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle datepicker InputDetailsLottable04" placeholder="MM/DD/YYYY" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Lottable05</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle datepicker InputDetailsLottable05" placeholder="MM/DD/YYYY" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Lottable06</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsLottable06" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Lottable07</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsLottable07" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Lottable08</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsLottable08" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Lottable09</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsLottable09" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Lottable10</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsLottable10" />
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
                        Order and Details
                        <div class="SaveRecordNow AnimateMe" id="btnSave" runat="server"></div>
                        <div class="ClosePopup AnimateMe"></div>
                    </div>
                    <div style="position: relative; height: 500px; width: 100%;" class="MyContainerPopup GetFullHeightForPopup content_4">
                        <div class="iWantMyChildrenFloatHeight">
                            <div class="floatL Width100 RecordHeader">
                                <div class="FloatRecord floatL">
                                    <input type="hidden" id="MyID" class="MyRecordID" value="0" />
                                    <div class="FloatRecordTitle floatL">Facility*</div>
                                    <div class="FloatRecordField floatL">
                                        <select data-placeholder="Select Facilities" multiple class="chosen-select InputFacility InputAutoPostBack" data-mode="single">
                                        </select>
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <!-- Mohamad Rmeity - Changing label from Order Manag Key to Order Management No. -->
                                    <div class="FloatRecordTitle floatL">Order Management No.</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputOrderManagKey" data-value="Auto Generated" data-disabled="" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <!-- Mohamad Rmeity - Changing label from Extern Order Key to External Order No. -->
                                    <div class="FloatRecordTitle floatL">External Order No.</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputExternOrderKey" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Owner*</div>
                                    <div class="FloatRecordField floatL">
                                        <select data-placeholder="Select Owners" multiple class="chosen-select InputStorerKey" data-mode="single">
                                        </select>
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Order Date</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle datepicker InputOrderDate" data-value="<%= Format(Now, "MM/dd/yyyy")  %>" data-disabled="" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Status</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputOrderManagStatus InputStatus" data-value="Created Externally" data-disabled="" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Type*</div>
                                    <div class="FloatRecordField floatL">
                                        <select data-placeholder="Select Types" multiple class="chosen-select InputType" data-mode="single">
                                        </select>
                                    </div>
                                </div>
                                <!-- Mohamad Rmeity - Changing label from Consignee to Ship To -->
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Ship To</div>
                                    <div class="FloatRecordField floatL">
                                        <select data-placeholder="Select Ship To" multiple class="chosen-select InputConsigneeKey" data-mode="single">
                                        </select>
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Req Ship Date</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle datepicker InputRequestedShipDate" placeholder="MM/DD/YYYY" />
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
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">WMS Order Key</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputWMSOrderKey" data-disabled="" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                            </div>
                        </div>
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
                                                    <select data-placeholder="Select Items" multiple class="chosen-select InputDetailsSku" data-mode="single">
                                                    </select>
                                                    <div class="SearchDropDown AnimateMe" data-requiredfieldsname="Facility,Owner" data-requiredfields=".InputFacility,.InputStorerKey" data-url="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Popup_Items", Nothing)) & "?warehouse=.InputFacility&storer=.InputStorerKey" %>"></div>
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Open Qty*</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsOpenQty" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Pack*</div>
                                                <div class="Details_FloatRecordField floatL" style="position: relative;">
                                                    <select data-placeholder="Select Packs" multiple class="chosen-select InputDetailsPackKey InputAutoPostBackDetails" data-mode="single">
                                                    </select>
                                                    <div class="SearchDropDown AnimateMe" data-requiredfieldsname="Facility" data-requiredfields=".InputFacility" data-url="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Popup_Packs", Nothing)) & "?warehouse=.InputFacility" %>"></div>
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">UOM</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <select data-placeholder="Select UOMs" multiple class="chosen-select InputDetailsUOM" data-mode="single">
                                                    </select>
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">UDF1</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsSUsr1" />
                                                </div>
                                            </div>
                                            <!-- Mohamad Rmeity - Changing Order Management Detail record to have less fields -->
                                            <%--<div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Price</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsPrice" data-disabled="" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Currency</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsCurrency" data-disabled="" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">UDF2</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsSUsr2" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">UDF3</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsSUsr3" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">UDF4</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsSUsr4" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Lottable01</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsLottable01" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Lottable02</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsLottable02" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Lottable03</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsLottable03" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Lottable04</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle datepicker InputDetailsLottable04" placeholder="MM/DD/YYYY" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Lottable05</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle datepicker InputDetailsLottable05" placeholder="MM/DD/YYYY" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Lottable06</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsLottable06" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Lottable07</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsLottable07" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Lottable08</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsLottable08" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Lottable09</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsLottable09" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Lottable10</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsLottable10" />
                                                </div>
                                            </div>--%>
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
        <input type="hidden" class="HiddenDetailLink" value="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Warehouse_OrderManagement", Nothing)) %>" />
        <input type="hidden" id="HiddenID" runat="server" class="HiddenID" value="0" />

        <!-- Mohamad Rmeity - Changing label from Order Manag Key to Order Management No. -->
        <input type="hidden" class="MyFields" value="OrderManagKey" data-columnname="Order Management No." data-priority="1" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyFields" value="Facility" data-columnname="Facility" data-priority="2" data-hidden="false" />
        <input type="hidden" class="MyFields" value="StorerKey" data-columnname="Owner" data-priority="3" data-hidden="false" />
        <!-- Mohamad Rmeity - Changing label from Extern Order Key to External Order No. -->
        <input type="hidden" class="MyFields" value="ExternOrderKey" data-columnname="External Order No." data-priority="4" data-hidden="false" />
        <!-- Mohamad Rmeity - Changing label from Consignee to Ship To -->
        <input type="hidden" class="MyFields" value="ConsigneeKey" data-columnname="Ship To" data-priority="5" data-hidden="false" />
        <input type="hidden" class="MyFields" value="ConsigneeName" data-columnname="Ship To Name" data-priority="6" data-hidden="false" />
        <input type="hidden" class="MyFields" value="OrderManagStatus" data-columnname="Status" data-priority="7" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Type" data-columnname="Type" data-priority="8" data-hidden="false" />
        <input type="hidden" class="MyFields" value="OrderDate" data-columnname="Order Date" data-priority="9" data-hidden="false" />
        <input type="hidden" class="MyFields" value="RequestedShipDate" data-columnname="Requested Ship Date" data-priority="10" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr1" data-columnname="UDF1" data-priority="11" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr2" data-columnname="UDF2" data-priority="12" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr3" data-columnname="UDF3" data-priority="13" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr4" data-columnname="UDF4" data-priority="14" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr5" data-columnname="UDF5" data-priority="15" data-hidden="false" />

        <input type="hidden" id="HiddenCanUploadFiles" runat="server" class="HiddenCanUploadFiles" value="0" />
        <input type="hidden" id="HiddenCanViewOwnFiles" runat="server" class="HiddenCanViewOwnFiles" value="0" />
        <input type="hidden" id="HiddenCanViewAllFiles" runat="server" class="HiddenCanViewAllFiles" value="0" />
        <input type="hidden" id="HiddenCanRemoveOwnFiles" runat="server" class="HiddenCanRemoveOwnFiles" value="0" />
        <input type="hidden" id="HiddenCanRemoveAllFiles" runat="server" class="HiddenCanRemoveAllFiles" value="0" />

        <input type="hidden" class="MyDetailsFields" value="ExternLineNo" data-columnname="Extern Line#" data-priority="1" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyDetailsFields" value="Sku" data-columnname="Item" data-priority="2" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyDetailsFields" value="OpenQty" data-columnname="Open Qty" data-priority="3" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="PackKey" data-columnname="Pack" data-priority="4" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="UOM" data-columnname="UOM" data-priority="5" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="Price" data-columnname="Price" data-priority="6" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="Currency" data-columnname="Currency" data-priority="7" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="SUsr1" data-columnname="UDF1" data-priority="8" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="SUsr2" data-columnname="UDF2" data-priority="9" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="SUsr3" data-columnname="UDF3" data-priority="10" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="SUsr4" data-columnname="UDF4" data-priority="11" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="Lottable01" data-columnname="Lottable01" data-priority="12" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="Lottable02" data-columnname="Lottable02" data-priority="13" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="Lottable03" data-columnname="Lottable03" data-priority="14" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="Lottable04" data-columnname="Lottable04" data-priority="15" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="Lottable05" data-columnname="Lottable05" data-priority="16" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="Lottable06" data-columnname="Lottable06" data-priority="17" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="Lottable07" data-columnname="Lottable07" data-priority="18" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="Lottable08" data-columnname="Lottable08" data-priority="19" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="Lottable09" data-columnname="Lottable09" data-priority="20" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="Lottable10" data-columnname="Lottable10" data-priority="21" data-hidden="false" />
    </div>
</asp:Content>