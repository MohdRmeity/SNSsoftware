<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Cufex/Cufex_Site.Master" CodeBehind="Cufex_Inventory_Balance.aspx.vb" Inherits="SNSsoftware.Cufex_Inventory_Balance" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Cufex_MainContent" runat="server">

    <%--Page Title--%>
    <div class="NormalDiv1118Max GetFullHeightAtLeast">
        <div style="height: 33px;"></div>
        <div class="MainHeader">
            <div class="MainPageTitle" data-id="Inventory_Balance">
                Inventory Balance
            </div>
            <div class="MainPageDesc">
                Check your inventory balance
            </div>
        </div>
        <div style="position: relative; padding-top: 25px; width: 100%;" class="HeaderGridView content_3">
            <table class="GridContainer" data-resizemode="fit">
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
                    <td class="GridCell GridHead" data-id="StorerKey">
                        <span class="MyTitleHead">Owner</span>
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
                    <td class="GridCell GridHead" data-id="Description">
                        <span class="MyTitleHead">Item Description</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Qty">
                        <span class="MyTitleHead">Qty on Hand</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Available">
                        <span class="MyTitleHead">Qty Available</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead borderRight0" data-id="Status">
                        <span class="MyTitleHead">Status</span>
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
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Facility" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="StorerKey" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Sku" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Description" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Qty" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Available" />
                    </td>
                    <td class="GridCell GridHeadSearch borderRight0">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Status" />
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

        <div style="position: relative; height: 70px;"></div>
        <div class="New_Modify_Record_PopUp">
            <div style="position: relative;">
                <div class="MyAbso_Record_PopUpContainer">

                    <div class="R_PopupTitle">
                        Inventory Balance Details
                        <div class="ClosePopup AnimateMe"><span class="ion-ios-exit"></span></div>
                    </div>
                    <div style="position: relative; height: 500px; width: 100%;" class="MyContainerPopup GetFullHeightForPopup content_4">
                        <div class="iWantMyChildrenFloatHeight">
                            <div class="floatL Width100 RecordHeader">
                                <div class="FloatRecord floatL">
                                    <input type="hidden" id="MyID" class="MyRecordID" value="0" />
                                    <div class="FloatRecordTitle floatL">Facility</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputFacility" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Owner</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputStorerKey" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Item</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputSku" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                            </div>
                        </div>
                        <div style="position: relative; margin-bottom: 20px;">
                            <div class="RecordsContainer">
                                <div class="RecordsContainer_Inside" style="display: none;">
                                    <div class="iWantMyChildrenFloatHeight">
                                        <div class="floatL Width100">

                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Lot</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsLot" />
                                                </div>
                                            </div>

                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Qty</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsQty" />
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
                                                    <input type="text" class="Details_textRecordStyle InputDetailsLottable04" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Lottable05</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsLottable05" />
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
                                                    <input type="text" class="Details_textRecordStyle InputDetailsLottable11" />
                                                </div>
                                            </div>
                                            <div class="Details_FloatRecord floatL">
                                                <div class="Details_FloatRecordTitle floatL">Lottable12</div>
                                                <div class="Details_FloatRecordField floatL">
                                                    <input type="text" class="Details_textRecordStyle InputDetailsLottable12" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="AddDetailsBtn AnimateMe DisplayNone">ADD DETAIL</div>
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
                    <div style="position: relative; height: 400px; width: 100%;" class="content_4 GridColumnsChooser">
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
        <input type="hidden" class="HiddenDetailLink" value="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Inventory_Balance-details", New With {.id = 0})) %>" />

        <input type="hidden" class="MyFields" value="Facility" data-columnname="Facility" data-priority="1" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyFields" value="StorerKey" data-columnname="Owner" data-priority="2" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyFields" value="Sku" data-columnname="Item" data-priority="3" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyFields" value="Description" data-columnname="Item Description" data-priority="4" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Qty" data-columnname="Qty on Hand" data-priority="5" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Available" data-columnname="Qty Available" data-priority="6" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Status" data-columnname="Status" data-priority="7" data-hidden="false" />
    </div>
</asp:Content>
