<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Cufex/Cufex_Site.Master" CodeBehind="Cufex_Configuration_ShipTo.aspx.vb" Inherits="SNSsoftware.Cufex_Configuration_ShipTo" %>

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
                        <div class="MainPageTitle" data-id="enterprise.storer2">
                            Ship To
                        </div>
                        <div class="MainPageDesc" data-text="Ship To List">
                            Ship To List
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
                                <a id="btnAddNew" runat="server" class="btnAddNew AnimateMe">New Ship To
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
                    <td class="GridCell GridHead" data-id="StorerKey">
                        <span class="MyTitleHead">Ship To</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Company">
                        <span class="MyTitleHead">Company</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Description">
                        <span class="MyTitleHead">Description</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Country">
                        <span class="MyTitleHead">Country</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="City">
                        <span class="MyTitleHead">City</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="State">
                        <span class="MyTitleHead">State</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Zip">
                        <span class="MyTitleHead">Zip Code</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Address1">
                        <span class="MyTitleHead">Address1</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Address2">
                        <span class="MyTitleHead">Address2</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Contact1">
                        <span class="MyTitleHead">Contact1</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Contact2">
                        <span class="MyTitleHead">Contact2</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Email1">
                        <span class="MyTitleHead">Email1</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Email2">
                        <span class="MyTitleHead">Email2</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Phone1">
                        <span class="MyTitleHead">Phone1</span>
                        <div class="AbsoSorting">
                            <div class="SortUp"></div>
                            <div class="SortDown"></div>
                        </div>
                    </td>
                    <td class="GridCell GridHead" data-id="Phone2">
                        <span class="MyTitleHead">Phone2</span>
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
                        <input type="text" placeholder="Search" class="SearchClass" data-id="StorerKey" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Company" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Description" />
                    </td>
                    <td class="GridCell GridHeadSearch" style="overflow: visible;">
                        <select data-placeholder="Search" multiple class="chosen-select SearchClass InputCountry" data-id="Country">
                        </select>
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="City" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="State" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Zip" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Address1" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Address2" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Contact1" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Contact2" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Email1" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Email2" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Phone1" />
                    </td>
                    <td class="GridCell GridHeadSearch">
                        <input type="text" placeholder="Search" class="SearchClass" data-id="Phone2" />
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
                    <td class="GridCell GridContentCell borderRight0" data-id="18"></td>
                    <td class="GridCell GridContentCell borderRight0" data-id="19"></td>
                    <td class="GridCell GridContentCell borderRight0" data-id="20"></td>
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
                            <div class="FloatRecordTitleNew">Ship To<span>*</span></div>
                            <div>
                                <input type="text" class="textRecordStyle InputStorerKey" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Company</div>
                            <div>
                                <input type="text" class="textRecordStyle InputCompany" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Description</div>
                            <div>
                                <input type="text" class="textRecordStyle InputDescription" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Country</div>
                            <div>
                                <select data-placeholder="Select Country" multiple class="chosen-select InputCountry" data-mode="single">
                                </select>
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">City</div>
                            <div>
                                <input type="text" class="textRecordStyle InputCity" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">State</div>
                            <div>
                                <input type="text" class="textRecordStyle InputState" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Zip</div>
                            <div>
                                <input type="text" class="textRecordStyle InputZip" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Address1</div>
                            <div>
                                <input type="text" class="textRecordStyle InputAddress1" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Address2</div>
                            <div>
                                <input type="text" class="textRecordStyle InputAddress2" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Address3</div>
                            <div>
                                <input type="text" class="textRecordStyle InputAddress3" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Address4</div>
                            <div>
                                <input type="text" class="textRecordStyle InputAddress4" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Address5</div>
                            <div>
                                <input type="text" class="textRecordStyle InputAddress5" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Address6</div>
                            <div>
                                <input type="text" class="textRecordStyle InputAddress6" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Contact1</div>
                            <div>
                                <input type="text" class="textRecordStyle InputContact1" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Contact2</div>
                            <div>
                                <input type="text" class="textRecordStyle InputContact2" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Email1</div>
                            <div>
                                <input type="text" class="textRecordStyle InputEmail1" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Email2</div>
                            <div>
                                <input type="text" class="textRecordStyle InputEmail2" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Phone1</div>
                            <div>
                                <input type="text" class="textRecordStyle InputPhone1" />
                            </div>
                        </div>
                        <div class="FloatRecordNew floatL">
                            <div class="FloatRecordTitleNew">Phone2</div>
                            <div>
                                <input type="text" class="textRecordStyle InputPhone2" />
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
                        Ship To
                        <div class="SaveRecordNow AnimateMe" id="btnSave" runat="server"></div>
                        <div class="ClosePopup AnimateMe"></div>
                    </div>
                    <div style="position: relative; height: 500px; width: 100%;" class="MyContainerPopup GetFullHeightForPopup content_4">
                        <div class="iWantMyChildrenFloatHeight">
                            <div class="floatL Width100 RecordHeader">
                                <div class="FloatRecord floatL">
                                    <input type="hidden" id="MyID" class="MyRecordID" value="0" />
                                    <div class="FloatRecordTitle floatL">Ship To*</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputStorerKey" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Company</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputCompany" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Description</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputDescription" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Country</div>
                                    <div class="FloatRecordField floatL">
                                        <select data-placeholder="Select Country" multiple class="chosen-select InputCountry" data-mode="single">
                                        </select>
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">City</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputCity" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">State</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputState" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Zip</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputZip" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Address1</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputAddress1" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Address2</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputAddress2" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Address3</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputAddress3" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Address4</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputAddress4" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Address5</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputAddress5" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Address6</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputAddress6" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Contact1</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputContact1" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Contact2</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputContact2" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Email1</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputEmail1" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Email2</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputEmail2" />
                                    </div>
                                </div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Phone1</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputPhone1" />
                                    </div>
                                </div>
                                <div class="FloatRecordSep floatL"></div>
                                <div class="FloatRecord floatL">
                                    <div class="FloatRecordTitle floatL">Phone2</div>
                                    <div class="FloatRecordField floatL">
                                        <input type="text" class="textRecordStyle InputPhone2" />
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
        <input type="hidden" class="HiddenDetailLink" value="<%= Server.UrlDecode(Page.GetRouteUrl("SNSsoftware-Cufex-Configuration_ShipTo", Nothing)) %>" />
        <input type="hidden" id="HiddenID" runat="server" class="HiddenID" value="0" />

        <input type="hidden" class="MyFields" value="StorerKey" data-columnname="Ship To" data-priority="1" data-hidden="false" data-primarykey="true" />
        <input type="hidden" class="MyFields" value="Company" data-columnname="Company" data-priority="2" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Description" data-columnname="Description" data-priority="3" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Country" data-columnname="Country" data-priority="4" data-hidden="false" />
        <input type="hidden" class="MyFields" value="City" data-columnname="City" data-priority="5" data-hidden="false" />
        <input type="hidden" class="MyFields" value="State" data-columnname="State" data-priority="6" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Zip" data-columnname="Zip" data-priority="7" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Address1" data-columnname="Address1" data-priority="8" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Address2" data-columnname="Address2" data-priority="9" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Address3" />
        <input type="hidden" class="MyFields" value="Address4" />
        <input type="hidden" class="MyFields" value="Address5" />
        <input type="hidden" class="MyFields" value="Address6" />
        <input type="hidden" class="MyFields" value="Contact1" data-columnname="Contact1" data-priority="10" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Contact2" data-columnname="Contact2" data-priority="11" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Email1" data-columnname="Email1" data-priority="12" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Email2" data-columnname="Email2" data-priority="13" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Phone1" data-columnname="Phone1" data-priority="14" data-hidden="false" />
        <input type="hidden" class="MyFields" value="Phone2" data-columnname="Phone2" data-priority="15" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr1" data-columnname="UDF1" data-priority="16" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr2" data-columnname="UDF2" data-priority="17" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr3" data-columnname="UDF3" data-priority="18" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr4" data-columnname="UDF4" data-priority="19" data-hidden="false" />
        <input type="hidden" class="MyFields" value="SUsr5" data-columnname="UDF5" data-priority="20" data-hidden="false" />
    </div>
</asp:Content>
