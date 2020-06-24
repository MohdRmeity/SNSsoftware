<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Cufex/Cufex_Site.Master" CodeBehind="Cufex_Warehouse_ASN.aspx.vb" Inherits="SNSsoftware.Cufex_Warehouse_ASN" %>

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
                        <div class="MainPageTitle" data-id="Warehouse_ASN">
                            Advance Ship Notice
                        </div>
                        <div class="MainPageDesc" data-text="Ship Notice List">
                            Ship Notice List
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
                                <a id="btnAddNew" runat="server" class="btnAddNew AnimateMe">New ASN
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
                    <td class="GridCell GridHead" data-id="ReceiptKey">
                        <span class="MyTitleHead">Receipt</span>
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
                    <td class="GridCell GridHead" data-id="ReceiptDate">
                        <span class="MyTitleHead">Receipt Date</span>
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
                    <td class="GridCell GridHead" data-id="ReceiptType">
                        <span class="MyTitleHead">Type</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="POKey">
                        <span class="MyTitleHead">PO Number</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="ExternReceiptKey">
                        <span class="MyTitleHead">Extern Receipt Key</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="CarrierKey">
                        <span class="MyTitleHead">Carrier</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="WarehouseReference">
                        <span class="MyTitleHead">Warehouse Reference</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="ContainerKey">
                        <span class="MyTitleHead">Container Reference</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="ContainerType">
                        <span class="MyTitleHead">Container Type</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="OriginCountry">
                        <span class="MyTitleHead">Origin Country</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead borderRight0" data-id="TransportationMode">
                        <span class="MyTitleHead">Transportation Mode</span>
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
                        <input type="text" placeholder="Search" class="SearchClass" data-id="ReceiptKey" />
                    </td>
                    <td class="GridCell GridHeadSearch" style="overflow: visible;">
                        <select data-placeholder="Search" multiple class="chosen-select SearchClass InputFacility" data-id="Facility" data-mode="single">
                        </select>
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="StorerKey" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="CAST(ReceiptDate AS date)" />
                    </td>
                    <td class="GridCell GridHeadSearch" style="overflow: visible;">
                        <select data-placeholder="Search" multiple class="chosen-select SearchClass InputReceiptStatusSearch" data-id="Status" data-mode="single">
                        </select>
                    </td>
                    <td class="GridCell GridHeadSearch" style="overflow: visible;">
                        <select data-placeholder="Search" multiple class="chosen-select SearchClass InputReceiptTypeSearch" data-id="ReceiptType" data-mode="single">
                        </select>
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="POKey" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="ExternReceiptKey" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="CarrierKey" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="WarehouseReference" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="ContainerKey" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="ContainerType" />
                    </td>
                    <td class="GridCell GridHeadSearch" style="overflow: visible;">
                        <select data-placeholder="Search" multiple class="chosen-select SearchClass InputOriginCountry" data-id="OriginCountry" data-mode="single">
                        </select>
                    </td>
                    <td class="GridCell GridHeadSearch borderRight0" style="overflow: visible;">
                        <select data-placeholder="Select Types" multiple class="chosen-select SearchClass InputTransportationMode" data-id="TransportationMode" data-mode="single">
                            <option value="PARCEL">Parcel</option>
                            <option value="LTL">Less than Truckload</option>
                            <option value="MOTOR">Truckload</option>
                            <option value="AIR">Air</option>
                            <option value="OCEAN">Ocean</option>
                            <option value="FORWARDER">Forwarder</option>
                            <option value="INTERMODAL">Intermodal</option>
                        </select>
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
                            <div class="FloatRecordTitleNew">Receipt Key</div>
                            <div>
                                <input type="text" class="textRecordStyle InputReceiptKey" data-value="Auto Generated" data-disabled="" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Extern Receipt Key</div>
                            <div>
                                <input type="text" class="textRecordStyle InputExternReceiptKey" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Receipt Date</div>
                            <div>
                                <input type="text" class="textRecordStyle datepicker InputReceiptDate" data-value="<%= Format(Now, "MM/dd/yyyy")  %>" data-disabled="" />
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
                            <div class="FloatRecordTitleNew">Status</div>
                            <div>
                                <input type="text" class="textRecordStyle InputStatus" data-value="New" data-disabled="" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Type<span>*</span></div>
                            <div>
                                <select data-placeholder="Select Types" multiple class="chosen-select InputReceiptType" data-mode="single">
                                </select>
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">PO Number</div>
                            <div>
                                <input type="text" class="textRecordStyle InputPOKey" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Carrier</div>
                            <div>
                                <select data-placeholder="Select Carriers" multiple class="chosen-select InputCarrierKey" data-mode="single">
                                </select>
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Warehouse Ref</div>
                            <div>
                                <input type="text" class="textRecordStyle InputWarehouseReference" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Container Ref</div>
                            <div>
                                <input type="text" class="textRecordStyle InputContainerKey" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Container Type</div>
                            <div>
                                <input type="text" class="textRecordStyle InputContainerType" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Origin Country</div>
                            <div>
                                <select data-placeholder="Select Countries" multiple class="chosen-select InputOriginCountry" data-mode="single">
                                </select>
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Transportation Mode</div>
                            <div>
                                <select data-placeholder="Select Types" multiple class="chosen-select InputTransportationMode" data-mode="single">
                                    <option value="PARCEL">Parcel</option>
                                    <option value="LTL">Less than Truckload</option>
                                    <option value="MOTOR">Truckload</option>
                                    <option value="AIR">Air</option>
                                    <option value="OCEAN">Ocean</option>
                                    <option value="FORWARDER">Forwarder</option>
                                    <option value="INTERMODAL">Intermodal</option>
                                </select>
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
                                <td class="GridCell GridHead" data-id="QtyExpected">
                                    <span class="MyTitleHead">Qty Expected</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="QtyReceived">
                                    <span class="MyTitleHead">Qty Received</span>
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
                                <td class="GridCell GridHead" data-id="POKey">
                                    <span class="MyTitleHead">PO</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="ToId">
                                    <span class="MyTitleHead">To LPN</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="ToLoc">
                                    <span class="MyTitleHead">To Location</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="ConditionCode">
                                    <span class="MyTitleHead">Hold</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="TariffKey">
                                    <span class="MyTitleHead">Tarrif Key</span>
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
                                <td class="GridCell GridHead" data-id="Lottable10">
                                    <span class="MyTitleHead">Lottable10</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead" data-id="Lottable11">
                                    <span class="MyTitleHead">Lottable11</span>
                                    <div class="AbsoSorting">
                                        <div class="SortUp"></div>
                                        <div class="SortDown"></div>
                                    </div>
                                </td>
                                <td class="GridCell GridHead borderRight0" data-id="Lottable12">
                                    <span class="MyTitleHead">Lottable12</span>
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
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="QtyExpected" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="QtyReceived" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="PackKey" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="UOM" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="POKey" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="ToId" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="ToLoc" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="ConditionCode" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="TariffKey" />
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
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="Lottable04" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="Lottable05" />
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
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="Lottable10" />
                                </td>
                                <td class="GridCell GridHeadSearch">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="Lottable11" />
                                </td>
                                <td class="GridCell GridHeadSearch borderRight0">
                                    <input type="text" placeholder="Search" class="SearchClass" data-id="Lottable12" />
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
                                <td class="GridCell GridContentCell borderRight0" data-id="22"></td>
                                <td class="GridCell GridContentCell borderRight0" data-id="23"></td>
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
                            <div class="Details_FloatRecordTitleNew">Item<span>*</span></div>
                            <div class="PositionRelative">
                                <select data-placeholder="Select Items" multiple class="chosen-select InputDetailsSku" data-mode="single">
                                </select>
                                <div class="SearchDropDown AnimateMe" data-requiredfieldsname="Facility,Owner" data-requiredfields=".InputFacility,.InputStorerKey" data-url="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Popup_Items", Nothing)) & "?warehouse=.InputFacility&storer=.InputStorerKey" %>"></div>
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Qty Expected<span>*</span></div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsQtyExpected" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Qty Received</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsQtyReceived" data-value="0" data-disabled="" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Pack</div>
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
                            <div class="Details_FloatRecordTitleNew">PO</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsPOKey" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">To LPN</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsToId" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">To Location</div>
                            <div class="PositionRelative">
                                <select data-placeholder="Select Locations" multiple class="chosen-select InputDetailsToLoc">
                                </select>
                                <div class="SearchDropDown AnimateMe" data-requiredfieldsname="Facility" data-requiredfields=".InputFacility" data-url="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Popup_Locations", Nothing)) & "?warehouse=.InputFacility" %>"></div>
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Hold</div>
                            <div>
                                <select data-placeholder="Select Holds" multiple class="chosen-select InputDetailsConditionCode" data-mode="single">
                                    <option value="CUSTOMS">Customs</option>
                                    <option value="DAMAGED">Damaged</option>
                                    <option value="EXPIRED">Expired</option>
                                    <option value="HOST PROCESSING">Host Processing</option>
                                    <option value="INSPECTION REQUIRED">Insepection Required</option>
                                    <option value="LOST">Lost</option>
                                    <option value="OK">Ok</option>
                                    <option value="OTHER">Other</option>
                                    <option value="PHYSICAL INVENTORY">Physical Inventory</option>
                                    <option value="QUARANTINE">Quarantine</option>
                                    <option value="RETURNS">Returns</option>
                                </select>
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Tarrif Key</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle InputDetailsTariffKey" />
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
                            <div class="Details_FloatRecordTitleNew">Lottable03</div>
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
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Lottable11</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle datepicker InputDetailsLottable11" placeholder="MM/DD/YYYY" />
                            </div>
                        </div>
                        <div class="Details_FloatRecordNew floatL">
                            <div class="Details_FloatRecordTitleNew">Lottable12</div>
                            <div>
                                <input type="text" class="Details_textRecordStyle datepicker InputDetailsLottable12" placeholder="MM/DD/YYYY" />
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
                        ASN and Details
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
                                    <div class="FloatRecordTitle floatL">Receipt Key</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputReceiptKey" data-value="Auto Generated" data-disabled="" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Extern Receipt Key</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputExternReceiptKey" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Receipt Date</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle datepicker InputReceiptDate" data-value="<%= Format(Now, "MM/dd/yyyy")  %>" data-disabled="" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Owner*</div>
                                    <div class="FloatRecordField floatL">
                                        <select data-placeholder="Select Owners" multiple class="chosen-select InputStorerKey" data-mode="single">
                                        </select>
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Status</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputStatus" data-value="New" data-disabled="" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Type*</div>
                                    <div class="FloatRecordField floatL">
                                        <select data-placeholder="Select Types" multiple class="chosen-select InputReceiptType" data-mode="single">
                                        </select>
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">PO Number</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputPOKey" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Carrier</div>
                                    <div class="FloatRecordField floatL">
                                        <select data-placeholder="Select Carriers" multiple class="chosen-select InputCarrierKey" data-mode="single">
                                        </select>
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Warehouse Ref</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputWarehouseReference" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Container Ref</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputContainerKey" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Container Type</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputContainerType" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Origin Country</div>
                                    <div class="FloatRecordField floatL">
                                        <select data-placeholder="Select Countries" multiple class="chosen-select InputOriginCountry" data-mode="single">
                                        </select>
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Transportation Mode</div>
                                    <div class="FloatRecordField floatL">
                                        <select data-placeholder="Select Types" multiple class="chosen-select InputTransportationMode" data-mode="single">
                                            <option value="PARCEL">Parcel</option>
                                            <option value="LTL">Less than Truckload</option>
                                            <option value="MOTOR">Truckload</option>
                                            <option value="AIR">Air</option>
                                            <option value="OCEAN">Ocean</option>
                                            <option value="FORWARDER">Forwarder</option>
                                            <option value="INTERMODAL">Intermodal</option>
                                        </select>
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
                                                <div class="Details_FloatRecordTitle floatL">Qty Expected*</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsQtyExpected" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Qty Received</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsQtyReceived" data-value="0" data-disabled="" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Pack</div>
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
                                                <div class="Details_FloatRecordTitle floatL">PO</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsPOKey" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">To LPN</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsToId" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">To Location</div>
                                                <div class="Details_FloatRecordField floatL" style="position: relative;">
                                                    <select data-placeholder="Select Locations" multiple class="chosen-select InputDetailsToLoc" data-mode="single">
                                                    </select>
                                                    <div class="SearchDropDown AnimateMe" data-requiredfieldsname="Facility" data-requiredfields=".InputFacility" data-url="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Popup_Locations", Nothing)) & "?warehouse=.InputFacility" %>"></div>
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Hold</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <select data-placeholder="Select Holds" multiple class="chosen-select InputDetailsConditionCode" data-mode="single">
                                                        <option value="CUSTOMS">Customs</option>
                                                        <option value="DAMAGED">Damaged</option>
                                                        <option value="EXPIRED">Expired</option>
                                                        <option value="HOST PROCESSING">Host Processing</option>
                                                        <option value="INSPECTION REQUIRED">Insepection Required</option>
                                                        <option value="LOST">Lost</option>
                                                        <option value="OK">Ok</option>
                                                        <option value="OTHER">Other</option>
                                                        <option value="PHYSICAL INVENTORY">Physical Inventory</option>
                                                        <option value="QUARANTINE">Quarantine</option>
                                                        <option value="RETURNS">Returns</option>
                                                    </select>
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Tarrif Key</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsTariffKey" />
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
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Lottable11</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle datepicker InputDetailsLottable11" placeholder="MM/DD/YYYY" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Lottable12</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle datepicker InputDetailsLottable12" placeholder="MM/DD/YYYY" />
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
        <input type="hidden" class="HiddenDetailLink" value="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Warehouse_ASN", Nothing)) %>" />
        <input type="hidden" id="HiddenID" runat="server" class="HiddenID" value="0" />

        <input type="hidden" class="MyFields" value="ReceiptKey" data-columnname="Receipt" data-priority="1" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyFields" value="Facility" data-columnname="Facility" data-priority="2" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyFields" value="StorerKey" data-columnname="Owner" data-priority="3" data-hidden="false" />
        <input type="hidden" class="MyFields" value="ReceiptDate" data-columnname="Receipt Date" data-priority="4" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Status" data-columnname="Status" data-priority="5" data-hidden="false" />
        <input type="hidden" class="MyFields" value="ReceiptType" data-columnname="Type" data-priority="6" data-hidden="false" />
        <input type="hidden" class="MyFields" value="POKey" data-columnname="PO Number" data-priority="7" data-hidden="false" />
        <input type="hidden" class="MyFields" value="ExternReceiptKey" data-columnname="Extern Receipt Key" data-priority="8" data-hidden="false" />
        <input type="hidden" class="MyFields" value="CarrierKey" data-columnname="Carrier" data-priority="9" data-hidden="false" />
        <input type="hidden" class="MyFields" value="WarehouseReference" data-columnname="Warehouse Reference" data-priority="10" data-hidden="false" />
        <input type="hidden" class="MyFields" value="ContainerKey" data-columnname="Container Key" data-priority="11" data-hidden="false" />
        <input type="hidden" class="MyFields" value="ContainerType" data-columnname="Container Type" data-priority="12" data-hidden="false" />
        <input type="hidden" class="MyFields" value="OriginCountry" data-columnname="Origin Country" data-priority="13" data-hidden="false" />
        <input type="hidden" class="MyFields" value="TransportationMode" data-columnname="Transportation Mode" data-priority="14" data-hidden="false" />

        <input type="hidden" class="MyDetailsFields" value="ExternLineNo" data-columnname="Extern Line#" data-priority="1" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyDetailsFields" value="Sku" data-columnname="Item" data-priority="2" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyDetailsFields" value="QtyExpected" data-columnname="Qty Expected" data-priority="3" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="QtyReceived" data-columnname="Qty Received" data-priority="4" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="PackKey" data-columnname="Pack" data-priority="5" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="UOM" data-columnname="UOM" data-priority="6" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="POKey" data-columnname="PO" data-priority="7" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="ToId" data-columnname="To LPN" data-priority="8" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="ToLoc" data-columnname="To Location" data-priority="9" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="ConditionCode" data-columnname="Hold" data-priority="10" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="TariffKey" data-columnname="Tarrif Key" data-priority="11" data-hidden="false" />
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
        <input type="hidden" class="MyDetailsFields" value="Lottable11" data-columnname="Lottable11" data-priority="22" data-hidden="false" />
        <input type="hidden" class="MyDetailsFields" value="Lottable12" data-columnname="Lottable12" data-priority="23" data-hidden="false" />
    </div>
</asp:Content>
