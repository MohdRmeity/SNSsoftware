var AvoidWebServiceRaceCondition = 0;
var NtorKermelElraceCondition = 0;
var MenuClosed = false;
var jcrop_api;
var CurrentPage = 0;
var CurrentPageDetails = 0;
var MaxPages = 1;
var MaxPagesDetails = 1;
var SearchQuery = "";
var SearchQueryDetails = "";
var SortBy = "id desc";
var SortByDetails = SortBy;
var isFirstLoad = true;
var NumberOfRecordsInPage = 0;
var NumberOfRecordsInPageDetails = 0;

$(document).ready(function () {
    SetDefaults();
});

var prm = Sys.WebForms.PageRequestManager.getInstance();
prm.add_endRequest(function () {
    SetDefaults();
});

$(window).load(function () {
    setOnCufex_Resize();
});

function SetDefaults() {
    SNSFunctions();
    //PictureUpload();
    SetMenuFunctionality();
    SetScrolling();
    $(document).unbind("click").on("click", function (e) {
        $('.ActionHiddenButtons').slideUp();
        $('.ProfileData').slideUp();

        if (!$(e.target).closest(".chosen-container").length > 0) {
            $(".NewHeaderRecord").addClass("u-overflowHidden");
        }
    });

    $(window).resize(function () {
        setOnCufex_Resize();
    }).scroll(function () {
        //Scroll Event Here 
    }).keydown(function (e) {
        var keyCode = e.keyCode || e.which;
        if (keyCode == 9) {
            if ($(e.target).is('.InsideMenuDiv *')) {
                e.preventDefault();
            }
        }
    });

    $().KeyPressfunctionsOverride();

    setOnCufex_Resize();
}

function SNSFunctions() {
    NumberOfRecordsInPage = $('#NumberOfRecordsInPage').val();
    NumberOfRecordsInPageDetails = NumberOfRecordsInPage;

    if ($('#SortBy').length > 0) {
        SortBy = $('#SortBy').val();
        SortByDetails = SortBy;
    }

    SetGridActions();

    $('.btnQuickEntry').click(function () {
        $('.AddDetailsBtn').hide();
        $('.MyRecordID').val(0);
        $('.FloatRecordField, .Details_FloatRecordField').find('input:text').each(function () {
            var $this = $(this);
            if ($this.attr("data-value") != null) {
                $this.val($this.attr("data-value"));
            }
            else {
                $this.val('');
            }

            if ($this.attr("data-disabled") != null) {
                $this.prop("disabled", true);
            }
            else {
                $this.prop("disabled", false);
            }
        });

        $('.FloatRecordField').find('input:password').val('');
        $('.FloatRecordField').find('input:checkbox').prop("false");
        $('.FloatRecordField').find('select').val('').trigger('chosen:updated');
        $('.FloatRecordField').find('select').prop('disabled', false).trigger("chosen:updated");

        AutoPostBack("");
        $('.New_Modify_Record_PopUp').fadeIn(function () {
            setOnCufex_Resize();
            $('.AddDetailsBtn').show();
        });
    });

    $('.btnAddNew').click(function () {
        $('.MyRecordID').val(0);
        $('.MyDetailRecordID').val(0);
        NumberOfRecordsInPageDetails = $('#NumberOfRecordsInPage').val();
        if ($('#SortBy').length > 0) SortByDetails = $('#SortBy').val();
        CurrentPageDetails = 0;
        MaxPagesDetails = 1;
        SearchQueryDetails = "";
        $(".DetailsGridView").find(".SortUp,.SortDown").removeClass("Active");

        $(".HeaderGridView,.DetailsGridView,.btnQuickEntry,.ActionsDetails").hide();
        $(this).hide();
        $(this).parent("td").prev("td").hide();
        $(".NewRecord").fadeIn();
        $(".btnSave,.BackHeader,.NewDetailRecord").show();
        $(this).parent("td").next("td").show();
        $(".MainPageDesc").html("Header");

        $('.btnNew,.btnDeleteDetail').show();
        $('.BackDetail').hide();
        $(".VerticalSep,.btnSaveDetail").hide();
        $(".MainPageDetailTitle").html($(".MainPageDetailTitle").data("text"));

        $('.NewHeaderRecord, .NewDetailRecord').find('input:text').each(function () {
            var $this = $(this);
            if ($this.attr("data-value") != null) {
                $this.val($this.attr("data-value"));
            }
            else {
                $this.val('');
            }

            if ($this.attr("data-disabled") != null) {
                $this.prop("disabled", true);
            }
            else {
                $this.prop("disabled", false);
            }
        });
        $('.NewDetailRecord, .NewHeaderRecord').find('select').val('').trigger('chosen:updated');
        $('.NewHeaderRecord').find('select').prop('disabled', false).trigger("chosen:updated");
        setOnCufex_Resize();
    });

    $('.BackHeader').click(function () {
        $('.MyRecordID').val(0);
        $('.MyDetailRecordID').val(0);
        NumberOfRecordsInPageDetails = $('#NumberOfRecordsInPage').val();
        if ($('#SortBy').length > 0) SortByDetails = $('#SortBy').val();
        CurrentPageDetails = 0;
        MaxPagesDetails = 1;
        SearchQueryDetails = "";

        $(".DetailsGridView").find(".SortUp,.SortDown").removeClass("Active");
        $(".HeaderGridView").fadeIn();
        $(".HeaderGridView").mCustomScrollbar("scrollTo", "first");
        $(".HeaderGridView").mCustomScrollbar("update");
        $('.BackHeader').hide();
        $('.btnAddNew').parent("td").next("td").hide();
        $(".NewRecord, .btnSave, .BackHeader").hide();
        $('.btnAddNew,.btnQuickEntry').show();
        $('.btnAddNew').parent("td").prev("td").show();
        $(".MainPageDesc").html($(".MainPageDesc").data("text"));

        setTimeout(function () { InitColResizable(); }, 300);
    });

    $('.btnNew').click(function () {
        $('.MyDetailRecordID').val(0);
        $(".DetailsGridView,.btnDeleteDetail").hide();
        $(this).hide();
        $(this).parent("td").prev("td").hide();
        $(this).parent("td").next("td").show();
        $(".btnSaveDetail").parent("td").next("td").hide();
        $(".NewDetailRecord").fadeIn();
        $(".BackDetail,.VerticalSep,.btnSaveDetail").show();
        $(".MainPageDetailTitle").html("New Record");
        $('.NewDetailRecord').find('input:text').each(function () {
            var $this = $(this);
            if ($this.attr("data-value") != null) {
                $this.val($this.attr("data-value"));
            }
            else {
                $this.val('');
            }

            if ($this.attr("data-disabled") != null) {
                $this.prop("disabled", true);
            }
            else {
                $this.prop("disabled", false);
            }
        });
        $('.NewDetailRecord').find('select').val('').trigger('chosen:updated');
        $('.NewDetailRecord').find('select').prop('disabled', false).trigger("chosen:updated");
        var MyDropDown = $(".NewDetailRecord").find(".InputDetailsSku");
        var MyFacility = $(".NewHeaderRecord").find('.InputFacility').val();
        var MyOwner = $(".NewHeaderRecord").find('.InputStorerKey').val();
        GetSkuDropDown(MyDropDown, MyFacility, MyOwner, "");

        setOnCufex_Resize();
    });

    $('.BackDetail').click(function () {
        $('.MyDetailRecordID').val(0);

        $(".DetailsGridView").fadeIn();
        $(".DetailsGridView").mCustomScrollbar("scrollTo", "first");
        $(".DetailsGridView").mCustomScrollbar("update");
        $('.btnNew,.btnDeleteDetail').show();
        $(this).hide();
        $(".NewDetailRecord,.VerticalSep,.btnSaveDetail").hide();
        $(".MainPageDetailTitle").html($(".MainPageDetailTitle").data("text"));

        if ($(".DetailsGridView").find(".GridContainer").data("resizemode") == "fit") setOnCufex_Resize();

        setTimeout(function () {
            InitColResizable();
        }, 300);
    });

    $(".NewHeaderRecord").resizable({
        handles: "s"
    });

    if ($('.MainPageTitle').attr("data-id") == "Warehouse_PO" || $('.MainPageTitle').attr("data-id") == "Warehouse_ASN" || $('.MainPageTitle').attr("data-id") == "Warehouse_SO" || $('.MainPageTitle').attr("data-id") == "Warehouse_OrderManagement") {
        $(".NewHeaderRecord").find('.InputFacility').on('change', function () {
            var MyDropDown = $(".NewDetailRecord").find(".InputDetailsSku");
            var MyFacility = $(this).val();
            var MyOwner = $(".NewHeaderRecord").find('.InputStorerKey').val();
            GetSkuDropDown(MyDropDown, MyFacility, MyOwner, "");
        });
        $(".NewHeaderRecord").find('.InputStorerKey').on('change', function () {
            var MyDropDown = $(".NewDetailRecord").find(".InputDetailsSku");
            var MyFacility = $(".NewHeaderRecord").find('.InputFacility').val();
            var MyOwner = $(this).val();
            GetSkuDropDown(MyDropDown, MyFacility, MyOwner, "");
        });
        $(".RecordHeader").find('.InputFacility').on('change', function () {
            var MyDropDown = $(".RecordsContainer_Inside").find(".InputDetailsSku");
            var MyFacility = $(this).val();
            var MyOwner = $(".RecordHeader").find('.InputStorerKey').val();
            GetSkuDropDown(MyDropDown, MyFacility, MyOwner, "");
        });
        $(".RecordHeader").find('.InputStorerKey').on('change', function () {
            var MyDropDown = $(".RecordsContainer_Inside").find(".InputDetailsSku");
            var MyFacility = $(".RecordHeader").find('.InputFacility').val();
            var MyOwner = $(this).val();
            GetSkuDropDown(MyDropDown, MyFacility, MyOwner, "");
        });
    }

    $(".NewDetailRecord").find('.InputAutoPostBackDetails').on('change', function () {
        AutoPostBackDetails($(".NewDetailRecord"), $(this).chosen().val());
    });

    $('.CloseACPopup').click(function () {
        $('.Adjust_Columns_PopUp').fadeOut();
    });

    if ($('.MainPageTitle').length > 0) {
        if ($('.MainPageTitle').attr("data-id") == "PROFILEDETAIL") {
            LoadItems();
        }
        GetUserConfiguration();
        DisplayDropDowns();
    }

    $('.InputAutoPostBack').on('change', function () {
        AutoPostBack($(this).chosen().val());
    });

    if ($('.MainPageTitle').attr("data-id") == "Warehouse_PO") {
        $('.InputFacility, .InputStorerKey').on('change', function () {
            $('.InputDetailsSku').empty().trigger("chosen:updated");
        });
    }
    else if ($('.MainPageTitle').attr("data-id") == "Warehouse_ASN" || $('.MainPageTitle').attr("data-id") == "Warehouse_SO" || $('.MainPageTitle').attr("data-id") == "Warehouse_OrderManagement") {
        $('.InputFacility').on('change', function () {
            $('.InputDetailsUOM').empty().trigger("chosen:updated");
            $('.InputDetailsPrice').val("");
            $('.InputDetailsCurrency').val("");
        });
        $('.InputFacility, .InputStorerKey').on('change', function () {
            $('.InputDetailsSku').empty().trigger("chosen:updated");
        });
    }

    if ($('.MainPageTitle').attr("data-id") == "Warehouse_OrderManagement") {
        $(".NewRecord").find('.InputStorerKey, .InputConsigneeKey, .InputDetailsSku').on('change', function () {
            if ($('.NewDetailRecord:visible').length > 0) {
                SetPriceAndCurrency();
            }
        });
    }

    $('.btnDelete').click(function () {
        if ($(".HeaderGridView:visible").length > 0) {
            if ($(".HeaderGridView").find('.chkSelectGrd:checked').length > 0) {
                var MyItems = "";
                $('.chkSelectGrd:checked').each(function () {
                    if (MyItems == "") {
                        MyItems = $(this).attr("data-id");
                    }
                    else {
                        MyItems = MyItems + "," + $(this).attr("data-id");
                    }
                });
                DeleteItems(MyItems);
            }
        } else {
            if ($('.MyRecordID').val() != 0) DeleteItems($('.MyRecordID').val());
        }
    });

    $('.btnDeleteDetail').click(function () {
        if ($(".DetailsGridView:visible").length > 0) {
            if ($(".DetailsGridView").find('.chkSelectGrd:checked').length > 0) {
                var MyItems = "";
                $('.chkSelectGrd:checked').each(function () {
                    if (MyItems == "") {
                        MyItems = $(this).attr("data-id");
                    }
                    else {
                        MyItems = MyItems + "," + $(this).attr("data-id");
                    }
                });
                DeleteItemsDetails(MyItems);
            }
        }
    });

    $('.BtnDoSomeThing').click(function () {
        if ($(".HeaderGridView:visible").length > 0) {
            if ($('.chkSelectGrd:checked').length > 0) {
                var MyItems = "";
                $('.chkSelectGrd:checked').each(function () {
                    if (MyItems == "") {
                        MyItems = $(this).attr("data-id");
                    }
                    else {
                        MyItems = MyItems + "," + $(this).attr("data-id");
                    }
                });
                ExecuteAction(MyItems, $(this).attr("data-id"));
            }
        } else {
            if ($('.MyRecordID').val() != 0) ExecuteAction($('.MyRecordID').val(), $(this).attr("data-id"));
        }
    });

    $('.AddDetailsBtn').click(function () {
        $('.RecordsContainer_Inside').eq(0).clone().prependTo('.RecordsContainer').slideDown();
        setOnCufex_Resize();
        $('.MyContainerPopup').mCustomScrollbar("scrollTo", $(".RecordsContainer:eq(0)"));
        $('.RecordsContainer_Inside').eq(0).find('.chosen-container').remove();
        $('.RecordsContainer_Inside').eq(0).find('.chosen-select').chosen();
        $('.RecordsContainer_Inside').eq(0).find('.chosen-container').css("width", "100%");
        $('.RecordsContainer_Inside').eq(0).find('.chosen-select').trigger("chosen:updated");
        $('.RecordsContainer_Inside').eq(0).find('input:text').each(function () {
            var $this = $(this);
            if ($this.attr("data-value") != null) {
                $this.val($this.attr("data-value"));
            }
            else {
                $this.val('');
            }

            if ($this.attr("data-disabled") != null) {
                $this.prop("disabled", true);
            }
            else {
                $this.prop("disabled", false);
            }
        });
        $('.RecordsContainer_Inside').eq(0).find('select').prop('disabled', false).trigger("chosen:updated");
        $('.RecordsContainer_Inside').eq(0).find('.InputDetailsUOM').empty().trigger("chosen:updated");

        $('.btnDeleteDtl').click(function () {
            var $this = $(this);
            $this.parent('.RecordsContainer_Inside').remove();
            setOnCufex_Resize();
        });

        $('.RecordsContainer_Inside').eq(0).find(".datepicker").removeClass('hasDatepicker');
        $('.RecordsContainer_Inside').eq(0).find(".datepicker").datepicker();

        $(".RecordsContainer_Inside").find('.InputAutoPostBackDetails').on('change', function () {
            AutoPostBackDetails($(this).closest(".RecordsContainer_Inside:visible"), $(this).chosen().val());
        });

        if ($('.MainPageTitle').attr("data-id") == "Warehouse_OrderManagement") {
            $('.InputStorerKey, .InputConsigneeKey, .InputDetailsSku').on('change', function () {
                if ($('.RecordsContainer_Inside:visible').length > 0) {
                    SetPriceAndCurrency();
                }
            });
        }
    });

    $('.btnActions').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        $(".ActionHiddenButtons").slideToggle();
    });

    $('.ClosePopup').click(function () {
        $('.RecordsContainer_Inside:visible').remove();
        $('.New_Modify_Record_PopUp').fadeOut();
        $('.MyRecordID').val(0);
    });

    $('.SaveRecordNow').click(function () {
        SaveItems();
    });

    $('.btnSave').click(function () {
        SaveItemsNew();
    });

    $('.btnSaveDetail').click(function () {
        SaveItemsDetails();
    });

    $(".datepicker").datepicker();

    $(".checkRadio").checkboxradio({
        icon: false
    });

    $(".chosen-select").chosen({
        disable_search_threshold: 10,
        width: "100%"
    });

    $(".NewHeaderRecord").find(".chosen-select").on("chosen:showing_dropdown", function (e) {
        $(".NewHeaderRecord").removeClass("u-overflowHidden");
    });



    if ($(".MyTab").length > 0) {
        function MoveTabLine(index) {
            $(".MyTabLine").animate({
                width: $(".MyTab").eq(index).outerWidth() + "px",
                left: ($(".MyTab").eq(index).offset().left - $(".widthMenu").width()) + "px"
            }, 200);
        }

        MoveTabLine(0);
        $('.MyTab').click(function () {
            $('.MyTab').removeClass("Active");
            $(this).addClass("Active");
            MoveTabLine($(this).index());

            if ($('.MainPageTitle').attr("data-id") == "PROFILEDETAIL") {
                $('.GridContainer').addClass("DisplayNone");
                $('.btnDelete').addClass("DisplayNone");
                var TabID = $(this).data("id");
                if (TabID == "Actions") $('.GridActions').removeClass("DisplayNone");
                else if (TabID == "Reports") $('.GridReports').removeClass("DisplayNone");
                else if (TabID == "Dashboards") {
                    $('.GridDashboards').removeClass("DisplayNone");
                    $('.btnDelete').removeClass("DisplayNone");
                    setOnCufex_Resize();
                }
                CurrentPage = 1;
                $(".SearchClass").val("");
                SearchQuery = "";
                LoadItems();
                setTimeout(function () { InitColResizable(); }, 300);
            }
        });
    }

    if ($(".HiddenID").val() != 0 && $(".HiddenID").length > 0 && isFirstLoad) {
        AvoidWebServiceRaceCondition = 0;
        LoadItems();
    }
}

function ChangeColumnsOrder(row) {
    row.each(function () {
        var $this = $(this).children(".GridCell");
        $this.sort(function (a, b) {
            var First = $(a).attr("data-priority") == null ? 0 : parseInt($(a).attr("data-priority"));
            var Second = $(b).attr("data-priority") == null ? 0 : parseInt($(b).attr("data-priority"));
            return First - Second;
        });
        $(this).html($this);
    });
}

function ShowHideColumns(row) {
    row.children(".GridCell").each(function () {
        var $this = $(this);
        if ($this.attr("data-hidden") == "true") {
            $this.hide();
        }
        else {
            $this.show();
        }
    });
}

function PageTable($this) {
    var TableSize = $("." + $this).find(".GridResults").size();
    var MyPage = $this == "HeaderGridView" ? CurrentPage : CurrentPageDetails;
    var MyNumberOfRecordsInPage = $this == "HeaderGridView" ? NumberOfRecordsInPage : NumberOfRecordsInPageDetails;
    var PageFirstRowNo = 1 + (MyPage * MyNumberOfRecordsInPage);
    var PageLastRowNo = (MyPage + 1) * MyNumberOfRecordsInPage > TableSize ? TableSize : (MyPage + 1) * MyNumberOfRecordsInPage
    $("." + $this).find('.PagingNumbers').html(PageFirstRowNo + "-" + PageLastRowNo + " of " + TableSize);
    $("." + $this).find('.GridResults').hide().slice(MyPage * MyNumberOfRecordsInPage, (MyPage + 1) * MyNumberOfRecordsInPage).show();
}

function SetGridActions() {
    function SortTable(n, dir) {
        var table, rows, switching, i, x, y, shouldSwitch, switchcount = 0;
        var GridIndex = $(".HeaderGridView:visible").length > 0 ? 0 : 1;
        table = document.getElementsByClassName("GridContainer")[GridIndex];
        switching = true;
        while (switching) {
            switching = false;
            rows = table.rows;
            for (i = 2; i < rows.length - 1; i++) {
                shouldSwitch = false;
                x = rows[i].getElementsByClassName("GridCell")[n];
                y = rows[i + 1].getElementsByClassName("GridCell")[n];
                if (dir == "asc") {
                    if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
                        shouldSwitch = true;
                        break;
                    }
                } else if (dir == "desc") {
                    if (x.innerHTML.toLowerCase() < y.innerHTML.toLowerCase()) {
                        shouldSwitch = true;
                        break;
                    }
                }
            }
            if (shouldSwitch) {
                rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
                switching = true;
                switchcount++;
            }
        }
    }

    $(".GridContainer").on('click', '.GridHead[data-id]', function (e) {
        var dir;
        var $MyGrid = $(".HeaderGridView:visible").length > 0 ? $(".HeaderGridView") : $(".DetailsGridView");
        var GridName = $(".HeaderGridView:visible").length > 0 ? "HeaderGridView" : "DetailsGridView";

        if ($(this).find(".SortUp").hasClass("Active")) {
            $MyGrid.find(".SortUp,.SortDown").removeClass("Active");
            $(this).find(".SortDown").addClass("Active");
            if ($(".HeaderGridView:visible").length > 0) SortBy = $(this).find(".SortDown").parent().parent().attr("data-id") + " desc";
            else SortByDetails = $(this).find(".SortDown").parent().parent().attr("data-id") + " desc";
            dir = "desc";
        }
        else {
            $MyGrid.find(".SortUp,.SortDown").removeClass("Active");
            $(this).find(".SortUp").addClass("Active");
            if ($(".HeaderGridView:visible").length > 0) SortBy = $(this).find(".SortUp").parent().parent().attr("data-id") + " asc";
            else SortByDetails = $(this).find(".SortUp").parent().parent().attr("data-id") + " asc";
            dir = "asc";
        }

        if ($(".GridResults").length > 0) {
            SortTable($(this).index(), dir);
            PageTable(GridName);
        }
    });

    $(".GridContainer").on('click', '.SortUp', function (e) {
        e.stopPropagation();
        var $MyGrid = $(".HeaderGridView:visible").length > 0 ? $(".HeaderGridView") : $(".DetailsGridView");
        var GridName = $(".HeaderGridView:visible").length > 0 ? "HeaderGridView" : "DetailsGridView";

        $MyGrid.find(".SortUp,.SortDown").removeClass("Active");
        $(this).addClass("Active");

        if ($(".HeaderGridView:visible").length > 0) SortBy = $(this).parent().parent().attr("data-id") + " asc";
        else SortByDetails = $(this).parent().parent().attr("data-id") + " asc";

        if ($(".GridResults").length > 0) {
            SortTable($(this).closest(".GridCell").index(), "asc");
            PageTable(GridName);
        }
    });

    $(".GridContainer").on('click', '.SortDown', function (e) {
        e.stopPropagation();
        var $MyGrid = $(".HeaderGridView:visible").length > 0 ? $(".HeaderGridView") : $(".DetailsGridView");
        var GridName = $(".HeaderGridView:visible").length > 0 ? "HeaderGridView" : "DetailsGridView";

        $MyGrid.find(".SortUp,.SortDown").removeClass("Active");
        $(this).addClass("Active");

        if ($(".HeaderGridView:visible").length > 0) SortBy = $(this).parent().parent().attr("data-id") + " desc";
        else SortByDetails = $(this).parent().parent().attr("data-id") + " desc";

        if ($(".GridResults").length > 0) {
            SortTable($(this).closest(".GridCell").index(), "desc");
            PageTable(GridName);
        }
    });

    $('.AC_PopupAction_Reset').click(function () {
        ResetColumns();
    });

    function ResetColumns() {
        var divToAppend = "";

        var $MyField;
        if ($(".HeaderGridView:visible").length > 0) $MyField = $(".MyFields[data-columnname]");
        else $MyField = $(".MyDetailsFields[data-columnname]");

        $MyField.each(function (e) {
            var $this = $(this);
            var isPrimaryKey = $this.attr("data-primarykey") == "true";
            divToAppend += "<div class='ColumnContainer" + (isPrimaryKey ? " IsPrimaryKey" : "") + "' data-id = '" + $this.val() + "' data-priority='" + (e + 1) + "' data-hidden='false'>" + $this.data("columnname") + (!isPrimaryKey ? "<div class='ColumnChooserAction'></div>" : "") + "</div>"
        });
        $(".GridColumnsChooser").find(".mCSB_container").html(divToAppend);
        SetColumnChooserAction();
    }

    $('.AC_PopupAction_Apply').click(function () {
        ApplyColumns();
    });

    function ApplyColumns() {
        if ($(".HeaderGridView:visible").length > 0) {
            if ($(".HeaderGridView").find(".GridContainer").data("resizemode") == "overflow") $(".preloader").fadeIn();
            $(".ColumnContainer").each(function (e) {
                var field = $(this).data("id");
                var hidden = $(this).data("hidden");
                var priority = $(this).data("priority");
                var $thisChild = $(".HeaderGridView").find(".GridAdjust").children(".GridCell[data-id='" + field + "']");
                var index = $thisChild.index();
                $thisChild.attr("data-priority", priority);
                $thisChild.attr("data-hidden", hidden);
                $(".MyFields[value='" + field + "']").attr("data-priority", priority);
                $(".MyFields[value='" + field + "']").attr("data-hidden", hidden);
                $(".HeaderGridView").find(".SearchStyle").children(".GridCell").eq(index).attr("data-priority", priority);
                $(".HeaderGridView").find(".SearchStyle").children(".GridCell").eq(index).attr("data-hidden", hidden);
                $(".HeaderGridView").find(".NoResults").children(".GridCell").eq(index).attr("data-hidden", hidden);
                $(".HeaderGridView").find(".GridResults").each(function () {
                    $(this).children(".GridCell").eq(index).attr("data-hidden", hidden);
                    $(this).children(".GridCell").eq(index).attr("data-priority", priority);
                });
            });

            ShowHideColumns($(".HeaderGridView").find(".GridRow"));
            ChangeColumnsOrder($(".HeaderGridView").find(".GridRow"));

            $(".HeaderGridView").find(".GridRow").not(".NoResults").each(function () {
                var $thisChild = $(this).children(".GridCell[data-priority]");
                $thisChild.removeClass("borderRight0");
                $thisChild.eq($thisChild.length - 1).addClass("borderRight0");
            });
        } else {
            if ($(".DetailsGridView").find(".GridContainer").data("resizemode") == "overflow") $(".preloader").fadeIn();
            $(".ColumnContainer").each(function (e) {
                var field = $(this).data("id");
                var hidden = $(this).data("hidden");
                var $thisChild = $(".DetailsGridView").find(".GridAdjust").children(".GridCell[data-id='" + field + "']");
                var index = $thisChild.index();
                $thisChild.attr("data-priority", e + 1);
                $thisChild.attr("data-hidden", hidden);
                $(".MyDetailsFields[value='" + field + "']").attr("data-priority", e + 1);
                $(".MyDetailsFields[value='" + field + "']").attr("data-hidden", hidden);
                $(".DetailsGridView").find(".SearchStyle").children(".GridCell").eq(index).attr("data-priority", e + 1);
                $(".DetailsGridView").find(".SearchStyle").children(".GridCell").eq(index).attr("data-hidden", hidden);
                $(".DetailsGridView").find(".NoResults").children(".GridCell").eq(index).attr("data-hidden", hidden);
                $(".DetailsGridView").find(".GridResults").each(function () {
                    $(this).children(".GridCell").eq(index).attr("data-hidden", hidden);
                    $(this).children(".GridCell").eq(index).attr("data-priority", e + 1);
                });
            });

            ShowHideColumns($(".DetailsGridView").find(".GridRow"));
            ChangeColumnsOrder($(".DetailsGridView").find(".GridRow"));

            $(".DetailsGridView").find(".GridRow").not(".NoResults").each(function () {
                var $thisChild = $(this).children(".GridCell[data-priority]");
                $thisChild.removeClass("borderRight0");
                $thisChild.eq($thisChild.length - 1).addClass("borderRight0");
            });
        }

        $('.Adjust_Columns_PopUp').fadeOut();
        SetUserConfigution();
    }

    $(".GridContainer").on('click', '.AdjustColumns', function () {
        var $MyGrid;
        var MyField;
        if ($(this).closest(".HeaderGridView").length > 0) {
            $MyGrid = $(".HeaderGridView")
            MyField = ".MyFields"
        }
        else {
            $MyGrid = $(".DetailsGridView");
            MyField = ".MyDetailsFields"
        }

        var divToAppend = "";
        $MyGrid.find(".GridAdjust").children(".GridCell[data-id]").each(function (e) {
            var $this = $(this);
            var isHidden = $this.attr("data-hidden") == "true";
            var isPrimaryKey = $(MyField + "[value='" + $this.data("id") + "']").attr("data-primarykey") == "true";
            divToAppend += "<div class='ColumnContainer" + (isPrimaryKey ? " IsPrimaryKey" : "") + " " + (isHidden ? " Inactive" : "") + "' data-id = '" + $this.data("id") + "' data-priority='" + (e + 1) + "' data-hidden='" + (isHidden ? "true" : "false") + "'>" + $this.text() + (!isPrimaryKey ? "<div class='ColumnChooserAction" + (isHidden ? " AddColumn" : "") + "'></div>" : "") + "</div >"
        });
        $(".GridColumnsChooser").find(".mCSB_container").html(divToAppend);
        SetColumnChooserAction();

        $('.GridColumnsChooser').sortable({
            items: '.ColumnContainer:not(".IsPrimaryKey")',
            update: function () {
                $(".ColumnContainer").each(function () {
                    $(this).attr("data-priority", $(this).index() + 1);
                });
            }
        });

        $('.Adjust_Columns_PopUp').fadeIn();
    });

    function SetColumnChooserAction() {
        $(".ColumnChooserAction").click(function () {
            if ($(this).hasClass("AddColumn")) {
                $(this).removeClass("AddColumn");
                $(this).parent(".ColumnContainer").removeClass("Inactive");
                $(this).parent(".ColumnContainer").attr("data-hidden", "false");
            }
            else {
                $(this).addClass("AddColumn")
                $(this).parent(".ColumnContainer").addClass("Inactive");
                $(this).parent(".ColumnContainer").attr("data-hidden", "true");
            }
        });
    }

    $(".GridContainer").on('click', '.EraseSearch', function () {
        if ($(".HeaderGridView:visible").length > 0) {
            $(".HeaderGridView").find('.SearchClass').val("");
            SearchQuery = "";
        }
        else {
            $(".DetailsGridView").find('.SearchClass').val("");
            SearchQueryDetails = "";
        }
    });

    $(".GridContainer").on('click', '.GridSearch', function () {
        var SearchCount;
        if ($(".HeaderGridView:visible").length > 0) {
            SearchCount = $(".HeaderGridView").find(".SearchClass").filter(function () {
                return this.value !== "";
            });
            if (SearchCount.length == 0) SearchQuery = "";
            isFirstLoad = false;
            LoadItems();
        }
        else {
            SearchCount = $(".DetailsGridView").find(".SearchClass").filter(function () {
                return this.value !== "";
            });
            if (SearchCount.length == 0) SearchQueryDetails = "";
            LoadItemsDetails();
        }
    });

    $(".GridContainer").on('keypress', '.SearchClass', function (e) {
        SearchQuery = "";
        SearchQueryDetails = "";
        if (e.keyCode == 13) {
            e.preventDefault();
            if ($(".HeaderGridView:visible").length > 0) {
                $(".HeaderGridView").find('.SearchClass').each(function () {
                    if (SearchQuery == "") {
                        if ($(this).val() != "") {
                            SearchQuery = $(this).attr("data-id") + ":" + $(this).val();
                        }
                    }
                    else {
                        if ($(this).val() != "") {
                            SearchQuery = SearchQuery + "," + $(this).attr("data-id") + ":" + $(this).val();
                        }
                    }
                });
                isFirstLoad = false;
                LoadItems();
            }
            else {
                $(".DetailsGridView").find('.SearchClass').each(function () {
                    if (SearchQueryDetails == "") {
                        if ($(this).val() != "") {
                            SearchQueryDetails = $(this).attr("data-id") + ":" + $(this).val();
                        }
                    }
                    else {
                        if ($(this).val() != "") {
                            SearchQueryDetails = SearchQueryDetails + "," + $(this).attr("data-id") + ":" + $(this).val();
                        }
                    }
                });
                LoadItemsDetails();
            }
        }
    });

    $(".GridContainer").on('change', '.chkSelectAll', function () {
        var $MyGrid = $(".HeaderGridView:visible").length > 0 ? $(".HeaderGridView") : $(".DetailsGridView");
        $MyGrid.find('.chkSelectGrd:enabled').prop("checked", $(this).is(':checked'));
    });

    $(".HeaderGridView").find('.Arrow-Left-Back').addClass("Disabled");
    $(".DetailsGridView").find('.Arrow-Left-Back').addClass("Disabled");
    $('.Arrow-Left-Back').unbind('click').click(function () {
        if ($(".HeaderGridView:visible").length > 0) {
            CurrentPage = parseInt(CurrentPage) - 1;
            if (CurrentPage < 0) {
                CurrentPage = 0;
                return;
            }
            if (CurrentPage == 0) {
                $(".HeaderGridView").find('.Arrow-Left-Back').addClass("Disabled");
                $(".HeaderGridView").find('.Arrow-Left-Back-First').addClass("Disabled");
            }
            else {
                $(".HeaderGridView").find('.Arrow-Left-Back').removeClass("Disabled");
                $(".HeaderGridView").find('.Arrow-Left-Back-First').removeClass("Disabled");
            }
            if (MaxPages != 1) {
                $(".HeaderGridView").find('.Arrow-Right-Forward').removeClass("Disabled");
                $(".HeaderGridView").find('.Arrow-Right-Forward-Last').removeClass("Disabled");
            }
            PageTable("HeaderGridView");
        }
        else {
            CurrentPageDetails = parseInt(CurrentPageDetails) - 1;
            if (CurrentPageDetails < 0) {
                CurrentPageDetails = 0;
                return;
            }
            if (CurrentPageDetails == 0) {
                $(".DetailsGridView").find('.Arrow-Left-Back').addClass("Disabled");
                $(".DetailsGridView").find('.Arrow-Left-Back-First').addClass("Disabled");
            }
            else {
                $(".DetailsGridView").find('.Arrow-Left-Back').removeClass("Disabled");
                $(".DetailsGridView").find('.Arrow-Left-Back-First').removeClass("Disabled");
            }
            if (MaxPagesDetails != 1) {
                $(".DetailsGridView").find('.Arrow-Right-Forward').removeClass("Disabled");
                $(".DetailsGridView").find('.Arrow-Right-Forward-Last').removeClass("Disabled");
            }
            PageTable("DetailsGridView");
        }
    });

    $(".HeaderGridView").find('.Arrow-Left-Back-First').addClass("Disabled");
    $(".DetailsGridView").find('.Arrow-Left-Back-First').addClass("Disabled");
    $('.Arrow-Left-Back-First').unbind('click').click(function () {
        if ($(".HeaderGridView:visible").length > 0) {
            if (CurrentPage == 0) return;
            $(".HeaderGridView").find('.Arrow-Left-Back-First').addClass("Disabled");
            $(".HeaderGridView").find('.Arrow-Left-Back').addClass("Disabled");
            if (MaxPages != 1) {
                $(".HeaderGridView").find('.Arrow-Right-Forward').removeClass("Disabled");
                $(".HeaderGridView").find('.Arrow-Right-Forward-Last').removeClass("Disabled");
            }
            CurrentPage = 0;
            PageTable("HeaderGridView");
        }
        else {
            if (CurrentPageDetails == 0) return;
            $(".DetailsGridView").find('.Arrow-Left-Back-First').addClass("Disabled");
            $(".DetailsGridView").find('.Arrow-Left-Back').addClass("Disabled");
            if (MaxPagesDetails != 1) {
                $(".DetailsGridView").find('.Arrow-Right-Forward').removeClass("Disabled");
                $(".DetailsGridView").find('.Arrow-Right-Forward-Last').removeClass("Disabled");
            }
            CurrentPageDetails = 0;
            PageTable("DetailsGridView");
        }
    });

    $(".HeaderGridView").find(".Arrow-Right-Forward").addClass("Disabled");
    $(".DetailsGridView").find(".Arrow-Right-Forward").addClass("Disabled");
    $('.Arrow-Right-Forward').unbind('click').click(function () {
        if ($(".HeaderGridView:visible").length > 0) {
            CurrentPage = parseInt(CurrentPage) + 1;
            if (CurrentPage >= MaxPages) {
                CurrentPage = MaxPages;
                return;
            }
            if (CurrentPage == MaxPages - 1) {
                $(".HeaderGridView").find('.Arrow-Right-Forward').addClass("Disabled");
                $(".HeaderGridView").find('.Arrow-Right-Forward-Last').addClass("Disabled");
            }
            else {
                $(".HeaderGridView").find('.Arrow-Right-Forward').removeClass("Disabled");
                $(".HeaderGridView").find('.Arrow-Right-Forward-Last').removeClass("Disabled");
            }
            if (MaxPages != 1) {
                $(".HeaderGridView").find('.Arrow-Left-Back').removeClass("Disabled");
                $(".HeaderGridView").find('.Arrow-Left-Back-First').removeClass("Disabled");
            }
            PageTable("HeaderGridView");
        } else {
            CurrentPageDetails = parseInt(CurrentPageDetails) + 1;
            if (CurrentPageDetails >= MaxPagesDetails) {
                CurrentPageDetails = MaxPagesDetails;
                return;
            }
            if (CurrentPageDetails == MaxPagesDetails - 1) {
                $(".DetailsGridView").find('.Arrow-Right-Forward').addClass("Disabled");
                $(".DetailsGridView").find('.Arrow-Right-Forward-Last').addClass("Disabled");
            }
            else {
                $(".DetailsGridView").find('.Arrow-Right-Forward').removeClass("Disabled");
                $(".DetailsGridView").find('.Arrow-Right-Forward-Last').removeClass("Disabled");
            }
            if (MaxPagesDetails != 1) {
                $(".DetailsGridView").find('.Arrow-Left-Back').removeClass("Disabled");
                $(".DetailsGridView").find('.Arrow-Left-Back-First').removeClass("Disabled");
            }
            PageTable("DetailsGridView");
        }
    });

    $(".HeaderGridView").find(".Arrow-Right-Forward-Last").addClass("Disabled");
    $(".DetailsGridView").find(".Arrow-Right-Forward-Last").addClass("Disabled");
    $('.Arrow-Right-Forward-Last').unbind('click').click(function () {
        if ($(".HeaderGridView:visible").length > 0) {
            if (CurrentPage == MaxPages - 1) return;
            $(".HeaderGridView").find('.Arrow-Right-Forward-Last').addClass("Disabled");
            $(".HeaderGridView").find('.Arrow-Right-Forward').addClass("Disabled");
            if (MaxPages != 1) {
                $(".HeaderGridView").find('.Arrow-Left-Back').removeClass("Disabled");
                $(".HeaderGridView").find('.Arrow-Left-Back-First').removeClass("Disabled");
            }
            CurrentPage = MaxPages - 1;
            PageTable("HeaderGridView");
        } else {
            if (CurrentPageDetails == MaxPagesDetails - 1) return;
            $(".DetailsGridView").find('.Arrow-Right-Forward-Last').addClass("Disabled");
            $(".DetailsGridView").find('.Arrow-Right-Forward').addClass("Disabled");
            if (MaxPagesDetails != 1) {
                $(".DetailsGridView").find('.Arrow-Left-Back').removeClass("Disabled");
                $(".DetailsGridView").find('.Arrow-Left-Back-First').removeClass("Disabled");
            }
            CurrentPageDetails = MaxPagesDetails - 1;
            PageTable("DetailsGridView");
        }
    });
}

function DeleteItems(MyItems) {
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;

        swal({
            title: "Delete",
            text: 'Are you sure you want to remove this record?',
            type: 'warning',
            confirmButtonColor: $('.AlertconfirmButtonColor').val(),
            showCancelButton: true
        },
            function (isConfirm) {
                if (isConfirm) {
                    $(".preloader").fadeIn();

                    var pageUrl = sAppPath + 'WebServices/DeleteItems.ashx';

                    var data = new FormData();
                    data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
                    data.append("MyItems", MyItems);
                    data.append("QueryUrlStr", $(".QueryUrlStr").val());

                    $.ajax({
                        type: "POST",
                        contentType: false,
                        processData: false,
                        data: data,
                        url: pageUrl + "/ExecuteQuery",
                        success: OnLoadSuccess,
                        error: OnLoadError
                    });
                }
                else {
                    AvoidWebServiceRaceCondition = 0;
                }
            });
    }
    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        var success = true;
        if (Object.keys(obj).length > 0) {
            if ($.trim(obj.Result) !== '') {
                success = false;
                setTimeout(function () {
                    swal({
                        title: "Delete",
                        text: obj.Result.replace(/<br\s*\/?>/gim, "\n"),
                        type: 'error',
                        confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                        showCancelButton: false
                    });
                    $('.preloader').fadeOut(300, function () {
                        AvoidWebServiceRaceCondition = 0;
                    });
                }, 500);
            }
        } else {
            alert("Couldn't Delete Your Record");
        }
        if (success) {
            AvoidWebServiceRaceCondition = 0;
            $(".BackHeader").trigger("click");
            $(".HeaderGridView").mCustomScrollbar("scrollTo", "first");
            $(".HeaderGridView").mCustomScrollbar("update");
            isFirstLoad = true;
            LoadItems();
        }
    }

    function OnLoadError(response) {
        alert(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function DeleteItemsDetails(MyItems) {
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;

        swal({
            title: "Delete",
            text: 'Are you sure you want to remove this record?',
            type: 'warning',
            confirmButtonColor: $('.AlertconfirmButtonColor').val(),
            showCancelButton: true
        },
            function (isConfirm) {
                if (isConfirm) {

                    $(".preloader").fadeIn();

                    var pageUrl = sAppPath + 'WebServices/DeleteItemsDetails.ashx';

                    var data = new FormData();
                    data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
                    data.append("MyItems", MyItems);
                    data.append("Facility", $('.NewHeaderRecord').find(".InputFacility").val());

                    $.ajax({
                        type: "POST",
                        contentType: false,
                        processData: false,
                        data: data,
                        url: pageUrl + "/ExecuteQuery",
                        success: OnLoadSuccess,
                        error: OnLoadError
                    });
                }
                else {
                    AvoidWebServiceRaceCondition = 0;
                }
            });
    }
    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        var success = true;
        if (Object.keys(obj).length > 0) {
            if ($.trim(obj.Result) !== '') {
                success = false;
                setTimeout(function () {
                    swal({
                        title: "Delete",
                        text: obj.Result.replace(/<br\s*\/?>/gim, "\n"),
                        type: 'error',
                        confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                        showCancelButton: false
                    });
                    $('.preloader').fadeOut(300, function () {
                        AvoidWebServiceRaceCondition = 0;
                    });
                }, 500);
            }
        } else {
            alert("Couldn't Delete Your Record");
        }
        if (success) {
            AvoidWebServiceRaceCondition = 0;
            isFirstLoad = true;
            LoadItemsDetails();
        }
    }

    function OnLoadError(response) {
        alert(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function SaveItems() {
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;
        //if ($('.MainPageTitle').attr("data-id") == "Warehouse_PO" || $('.MainPageTitle').attr("data-id") == "Warehouse_ASN" || $('.MainPageTitle').attr("data-id") == "Warehouse_SO") $('.preloader').fadeIn();

        $(".preloader").fadeIn();

        var pageUrl = sAppPath + 'WebServices/SaveItems.ashx';

        var data = new FormData();
        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
        data.append("MyID", $('.MyRecordID').val());
        data.append("DetailsCount", $('.RecordsContainer_Inside:visible').length);

        $('.MyFields').each(function () {
            var myField = $(this).val();
            if ($('.RecordHeader').find('.Input' + myField).is("input")) {
                if ($('.RecordHeader').find('.Input' + myField).attr("type") == "text" || $('.RecordHeader').find('.Input' + myField).attr("type") == "password") {
                    data.append("Field_" + myField, $('.RecordHeader').find('.Input' + myField).val());
                }
                else if ($('.RecordHeader').find('.Input' + myField).attr("type") == "checkbox") {
                    data.append("Field_" + myField, $('.RecordHeader').find('.Input' + myField).prop("checked"));
                }
            }
            else if ($('.RecordHeader').find('.Input' + myField).is("select")) {
                data.append("Field_" + myField, $('.RecordHeader').find('.Input' + myField).val());
            }
        });

        $('.MyDetailsFields').each(function () {
            var myField = $(this).val();
            var myFieldValue = "";
            $('.RecordsContainer_Inside:visible').each(function (e) {
                if (e == 0) {
                    myFieldValue += $(this).find('.InputDetails' + myField).eq(e).val();
                }
                else {
                    myFieldValue += "~~~" + $(this).find('.InputDetails' + myField).eq(e).val();
                }
            });
            if ($('.RecordsContainer_Inside:visible').find('.InputDetails' + myField).is("input")) {
                if ($('.RecordsContainer_Inside:visible').find('.InputDetails' + myField).attr("type") == "text") {
                    data.append("DetailsField_" + myField, myFieldValue);
                }
            }
            else if ($('.RecordsContainer_Inside:visible').find('.InputDetails' + myField).is("select")) {
                data.append("DetailsField_" + myField, myFieldValue);
            }
        });

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/SaveItem",
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }

    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        var success = true;
        if (Object.keys(obj).length > 0) {
            if ($.trim(obj.tmp) == '') {
                $('.ClosePopup').trigger("click");
                if ($('.MainPageTitle').attr("data-id") == "ChangePassword") {
                    success = false;
                    setTimeout(function () {
                        swal({
                            title: "Save",
                            text: "Password Changed",
                            type: 'success',
                            confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                            showCancelButton: false
                        });
                        $('.preloader').fadeOut(300, function () {
                            AvoidWebServiceRaceCondition = 0;
                        });
                    }, 500);
                }
            }
            else {
                success = false;
                setTimeout(function () {
                    swal({
                        title: "Save",
                        text: obj.tmp.replace(/<br\s*\/?>/gim, "\n"),
                        type: 'error',
                        confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                        showCancelButton: false
                    });
                    $('.preloader').fadeOut(300, function () {
                        AvoidWebServiceRaceCondition = 0;
                    });
                }, 500);
            }
        } else {
            alert("Couldn't Save Your Record");
        }
        if (success) {
            AvoidWebServiceRaceCondition = 0;
            isFirstLoad = true;
            LoadItems();
        }
    }

    function OnLoadError(response) {
        alert(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function SaveItemsNew() {
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;
        $(".preloader").fadeIn();
        var pageUrl = sAppPath + 'WebServices/SaveItemsNew.ashx';

        var data = new FormData();
        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
        data.append("MyID", $('.MyRecordID').val());

        $('.MyFields').each(function () {
            var myField = $(this).val();
            var myClass = $(".FloatRecordNew").find(".Input" + myField);
            var myFieldValue = myClass.val();

            if (myClass.is("input")) {
                if (myClass.attr("type") == "text") {
                    data.append("Field_" + myField, myFieldValue);
                }
            }
            else if (myClass.is("select")) {
                data.append("Field_" + myField, myFieldValue);
            }
        });

        $('.MyDetailsFields').each(function () {
            var myField = $(this).val();
            var myClass = $(".Details_FloatRecordNew").find(".InputDetails" + myField);
            var myFieldValue = myClass.val();

            if (myClass.is("input")) {
                if (myClass.attr("type") == "text") {
                    data.append("DetailsField_" + myField, myFieldValue);
                }
            }
            else if (myClass.is("select")) {
                data.append("DetailsField_" + myField, myFieldValue);
            }
        });

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/SaveItem",
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }

    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        var success = true;
        if (Object.keys(obj).length > 0) {
            if ($.trim(obj.tmp) != '') {
                success = false;
                setTimeout(function () {
                    swal({
                        title: "Save",
                        text: obj.tmp.replace(/<br\s*\/?>/gim, "\n"),
                        type: 'error',
                        confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                        showCancelButton: false
                    });
                    $('.preloader').fadeOut(300, function () {
                        AvoidWebServiceRaceCondition = 0;
                    });
                }, 500);
            }
        } else {
            alert("Couldn't Save Your Record");
        }
        if (success) {
            AvoidWebServiceRaceCondition = 0;
            $('.BackHeader').trigger("click");
            $(".HeaderGridView").mCustomScrollbar("scrollTo", "first");
            $(".HeaderGridView").mCustomScrollbar("update");
            isFirstLoad = true;
            LoadItems();
        }
    }

    function OnLoadError(response) {
        alert(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function SaveItemsDetails() {
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;
        $(".preloader").fadeIn();
        var pageUrl = sAppPath + 'WebServices/SaveItemsDetails.ashx';

        var data = new FormData();
        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
        data.append("MyID", $('.MyDetailRecordID').val());

        $('.MyFields').each(function () {
            var myField = $(this).val();
            var myClass = $(".FloatRecordNew").find(".Input" + myField);
            var myFieldValue = myClass.val();

            if (myClass.is("input")) {
                if (myClass.attr("type") == "text") {
                    data.append("Field_" + myField, myFieldValue);
                }
            }
            else if (myClass.is("select")) {
                data.append("Field_" + myField, myFieldValue);
            }
        });

        $('.MyDetailsFields').each(function () {
            var myField = $(this).val();
            var myClass = $(".Details_FloatRecordNew").find(".InputDetails" + myField);
            var myFieldValue = myClass.val();

            if (myClass.is("input")) {
                if (myClass.attr("type") == "text") {
                    data.append("DetailsField_" + myField, myFieldValue);
                }
            }
            else if (myClass.is("select")) {
                data.append("DetailsField_" + myField, myFieldValue);
            }
        });

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/SaveItem",
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }

    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        var success = true;
        if (Object.keys(obj).length > 0) {
            if ($.trim(obj.tmp) != '') {
                success = false;
                setTimeout(function () {
                    swal({
                        title: "Save",
                        text: obj.tmp.replace(/<br\s*\/?>/gim, "\n"),
                        type: 'error',
                        confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                        showCancelButton: false
                    });
                    $('.preloader').fadeOut(300, function () {
                        AvoidWebServiceRaceCondition = 0;
                    });
                }, 500);
            }
        } else {
            alert("Couldn't Save Your Record");
        }
        if (success) {
            AvoidWebServiceRaceCondition = 0;
            $('.BackDetail').trigger("click");
            $(".DetailsGridView").mCustomScrollbar("scrollTo", "first");
            $(".DetailsGridView").mCustomScrollbar("update");
            LoadItemsDetails();
            $(".NewHeaderRecord").find(".InputStatus").val(obj.Status);
        }
    }

    function OnLoadError(response) {
        alert(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function DisplayItem(DisplayID, QueryURL) {
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;

        $(".preloader").fadeIn();

        var pageUrl = sAppPath + 'WebServices/DisplayItems.ashx';

        var data = new FormData();
        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
        data.append("MyID", DisplayID);

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/DisplayItem",
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }

    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        var success = true;
        if (Object.keys(obj).length > 0) {
            if ($.trim(obj.Error) == '') {

                var MySavedValues = obj.SavedFields;
                var MyItems = MySavedValues.split(";;;");
                $.each(MyItems, function (i) {
                    var myvalue = "";
                    var myclass = "";

                    var MyItemsValues = MyItems[i].split(":::");
                    if (MyItemsValues.length > 0) { myclass = $(".RecordHeader").find('.Input' + $.trim(MyItemsValues[0])); }
                    if (MyItemsValues.length > 1) { myvalue = MyItemsValues[1]; }

                    if (myclass.is("input")) {
                        if (myclass.attr("type") == "text") {
                            myclass.val(myvalue);
                        }
                        else if ($(myclass).attr("type") == "checkbox") {
                            myclass.prop("checked", myvalue);
                        }
                    } else if (myclass.is("select")) {
                        if ($('.MainPageTitle').attr("data-id") == "Warehouse_PO" || $('.MainPageTitle').attr("data-id") == "Warehouse_ASN" || $('.MainPageTitle').attr("data-id") == "Warehouse_SO" || $('.MainPageTitle').attr("data-id") == "Warehouse_OrderManagement" || $('.MainPageTitle').attr("data-id") == "SKUCATALOGUE") {
                            if (myclass.hasClass("InputAutoPostBack")) {
                                AutoPostBack(myvalue.split(','));
                                AvoidWebServiceRaceCondition = 1;
                            }
                        }
                        myclass.val(myvalue.split(',')).trigger("chosen:updated");
                        var myInterval = setInterval(function () {
                            if (!IsAutoPostBack) {
                                myclass.val(myvalue.split(',')).trigger("chosen:updated");
                                clearInterval(myInterval);
                            }
                        }, 100);
                    }
                });
                var MyReadOnlyValues = obj.ReadOnlyFields;
                var MyReadOnlyItems = MyReadOnlyValues.split("~~~");
                $.each(MyReadOnlyItems, function (i) {
                    var myclass = "";
                    var MyReadOnlyValues = MyReadOnlyItems[i];
                    if (MyReadOnlyValues.length > 0) { myclass = $(".RecordHeader").find('.Input' + $.trim(MyReadOnlyValues)); }
                    if (myclass.is("input")) { myclass.prop("disabled", true); }
                    else if (myclass.is("select")) { myclass.prop('disabled', true).trigger("chosen:updated"); }
                });

                $('.AddDetailsBtn').hide();
                $('.MyRecordID').val(DisplayID);
                $('.FloatRecordField').find('input:password').val('');

                $('.New_Modify_Record_PopUp').fadeIn(function () {
                    setOnCufex_Resize();
                    $('.AddDetailsBtn').show();

                    $('.btnDeleteDtl').click(function () {
                        var $this = $(this);
                        $this.parent('.RecordsContainer_Inside').remove();
                        setOnCufex_Resize();
                    });
                });

                if (obj.DetailsCount > 0) {
                    setTimeout(function () {
                        var MyDropDown = $(".RecordsContainer_Inside").find(".InputDetailsSku");
                        var MyFacility = $(".RecordHeader").find('.InputFacility').val();
                        var MyOwner = $(".RecordHeader").find('.InputStorerKey').val();
                        GetSkuDropDown(MyDropDown, MyFacility, MyOwner, "");
                    }, 2000);

                    for (i = 0; i < obj.DetailsCount; i++) {
                        $('.RecordsContainer_Inside').eq(0).clone().prependTo('.RecordsContainer').show();
                        setOnCufex_Resize();
                        $('.RecordsContainer_Inside').eq(0).find('.chosen-container').remove();
                        $('.RecordsContainer_Inside').eq(0).find('.chosen-select').chosen();
                        $('.RecordsContainer_Inside').eq(0).find('.chosen-container').css("width", "100%");
                        $('.RecordsContainer_Inside').eq(0).find('.chosen-select').trigger("chosen:updated");
                        $('.RecordsContainer_Inside').eq(0).find(".datepicker").removeClass('hasDatepicker');
                        $('.RecordsContainer_Inside').eq(0).find(".datepicker").datepicker();
                    }

                    if ($('.MainPageTitle').attr("data-id") == "Warehouse_OrderManagement") {
                        $('.InputStorerKey, .InputConsigneeKey, .InputDetailsSku').on('change', function () {
                            if ($('.RecordsContainer_Inside:visible').length > 0) {
                                SetPriceAndCurrency();
                            }
                        });
                    }

                    var MySavedDetailsValues = obj.SavedDetailsFields;
                    var MyDetailsItems = MySavedDetailsValues.split(";;;");
                    $.each(MyDetailsItems, function (i) {
                        var myvalue = "";
                        var myclass = "";

                        var MyDetailsItemsValues = MyDetailsItems[i].split(":::");
                        if (MyDetailsItemsValues.length > 0) { myclass = $('.RecordsContainer_Inside:visible').find('.InputDetails' + $.trim(MyDetailsItemsValues[0])); }
                        if (MyDetailsItemsValues.length > 1) { myvalue = MyDetailsItemsValues[1].split("~~~"); }

                        $('.RecordsContainer_Inside:visible').each(function (e) {
                            if (myvalue[e] != null) {
                                if (myclass.eq(e).is("input")) {
                                    if (myclass.eq(e).attr("type") == "text") {
                                        myclass.eq(e).val(myvalue[e]);
                                    }
                                    else if (myclass.eq(e).attr("type") == "checkbox") {
                                        myclass.eq(e).prop("checked", myvalue[e]);
                                    }
                                } else if (myclass.eq(e).is("select")) {
                                    if ($('.MainPageTitle').attr("data-id") == "Warehouse_PO" || $('.MainPageTitle').attr("data-id") == "Warehouse_ASN" || $('.MainPageTitle').attr("data-id") == "Warehouse_SO" || $('.MainPageTitle').attr("data-id") == "Warehouse_OrderManagement") {
                                        if (myclass.eq(e).hasClass("InputAutoPostBackDetails")) {
                                            AutoPostBackDetails($('.RecordsContainer_Inside').eq(e), myvalue[e].split(','));
                                            AvoidWebServiceRaceCondition = 1;
                                        }
                                        myclass.eq(e).val(myvalue[e].split(',')).trigger("chosen:updated");
                                        var myInterval = setInterval(function () {
                                            if (!IsAutoPostBackDetails) {
                                                myclass.eq(e).val(myvalue[e].split(',')).trigger("chosen:updated");
                                                clearInterval(myInterval);
                                            }
                                        }, 3000);
                                    }
                                    else {
                                        myclass.eq(e).val(myvalue[e].split(',')).trigger("chosen:updated");
                                    }
                                }
                            }
                        });
                    });

                    var MyReadOnlyDetailsValues = obj.ReadOnlyDetailsFields;
                    var MyReadOnlyDetailsItems = MyReadOnlyDetailsValues.split("~~~");
                    $('.RecordsContainer_Inside:visible').each(function (e) {
                        $.each(MyReadOnlyDetailsItems, function (i) {
                            var myclass = "";
                            var MyReadOnlyDetailsValues = MyReadOnlyDetailsItems[i];
                            if (MyReadOnlyDetailsValues.length > 0) { myclass = $('.RecordsContainer_Inside:visible').find('.InputDetails' + $.trim(MyReadOnlyDetailsValues)); }
                            if (myclass.eq(e).is("input")) { myclass.eq(e).prop("disabled", true); }
                            else if (myclass.eq(e).is("select")) { myclass.eq(e).prop('disabled', true).trigger("chosen:updated"); }
                        });
                    });

                    var MyInputDetailsValues = obj.InputDetailsFields;
                    if (MyInputDetailsValues != "") {
                        var MyDetailsInput = MyInputDetailsValues.split(";;;");
                        $.each(MyDetailsInput, function (i) {
                            var myvalue = "";
                            var myclass = "";

                            var MyDetailsInputValues = MyDetailsInput[i].split(":::");
                            if (MyDetailsInputValues.length > 0) { myclass = $('.RecordsContainer_Inside:visible').find('.InputDetails' + $.trim(MyDetailsInputValues[0])); }
                            if (MyDetailsInputValues.length > 1) { myvalue = MyDetailsInputValues[1].split("~~~"); }

                            $('.RecordsContainer_Inside:visible').each(function (e) {
                                myclass.eq(e).parents(".Details_FloatRecordField").siblings(".Details_FloatRecordTitle").html(myvalue[e]);
                            });
                        });
                    }
                }

                if (QueryURL != "") {
                    var MyLink = $('.HiddenDetailLink').val();
                    window.history.pushState('object or string', DisplayID, MyLink + QueryURL);
                }
            }
            else {
                showPreloader = false;
                setTimeout(function () {
                    swal({
                        title: "View",
                        text: obj.Error.replace(/<br\s*\/?>/gim, "\n"),
                        type: 'error',
                        confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                        showCancelButton: false
                    });
                    $('.preloader').fadeOut(300, function () {
                        AvoidWebServiceRaceCondition = 0;
                    });
                }, 500);
            }
        } else {
            alert("This record does not exist");
        }

        if (success) {
            $('.preloader').fadeOut(300, function () {
                AvoidWebServiceRaceCondition = 0;
            });
        }
    }

    function OnLoadError(response) {
        alert(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function DisplayItemNew(DisplayID) {
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;
        $(".preloader").fadeIn();
        $('html, body').animate({ scrollTop: 0 }, 'slow', "easeInOutCubic");
        var pageUrl = sAppPath + 'WebServices/DisplayItemsNew.ashx';

        var data = new FormData();
        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
        data.append("MyID", DisplayID);

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/DisplayItem",
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }

    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        var success = true;
        if (Object.keys(obj).length > 0) {
            if ($.trim(obj.Error) == '') {
                $(".btnAddNew").trigger("click");
                var MySavedValues = obj.SavedFields;
                var MyItems = MySavedValues.split(";;;");
                $.each(MyItems, function (i) {
                    var myvalue = "";
                    var myclass = "";

                    var MyItemsValues = MyItems[i].split(":::");
                    if (MyItemsValues.length > 0) { myclass = $(".NewHeaderRecord").find(".Input" + MyItemsValues[0].trim()); }
                    if (MyItemsValues.length > 1) { myvalue = MyItemsValues[1]; }

                    if (myclass.is("input")) {
                        if (myclass.attr("type") == "text") {
                            myclass.val(myvalue);
                        }
                    } else if (myclass.is("select")) {
                        if ($('.MainPageTitle').attr("data-id") == "Warehouse_PO" || $('.MainPageTitle').attr("data-id") == "Warehouse_ASN" || $('.MainPageTitle').attr("data-id") == "Warehouse_SO" || $('.MainPageTitle').attr("data-id") == "Warehouse_OrderManagement" || $('.MainPageTitle').attr("data-id") == "SKUCATALOGUE") {
                            if (myclass.hasClass("InputAutoPostBack")) {
                                AutoPostBack(myvalue.split(','));
                                AvoidWebServiceRaceCondition = 1;
                            }
                        }
                        myclass.val(myvalue.split(',')).trigger("chosen:updated");
                        var myInterval = setInterval(function () {
                            if (!IsAutoPostBack) {
                                myclass.val(myvalue.split(',')).trigger("chosen:updated");
                                clearInterval(myInterval);
                            }
                        }, 100);
                    }
                });

                var MyReadOnlyValues = obj.ReadOnlyFields;
                var MyReadOnlyItems = MyReadOnlyValues.split("~~~");

                $.each(MyReadOnlyItems, function (i) {
                    var myclass = "";
                    var MyReadOnlyValues = MyReadOnlyItems[i];
                    if (MyReadOnlyValues.length > 0) { myclass = $(".NewHeaderRecord").find(".Input" + MyReadOnlyValues.trim()); }
                    if (myclass.is("input:text")) { myclass.prop("disabled", true); }
                    else if (myclass.is("select")) { myclass.prop('disabled', true).trigger("chosen:updated"); }
                });

                $('.MyRecordID').val(DisplayID);
                $(".ActionsDetails,.DetailsGridView").show();
                $(".NewDetailRecord").hide();
            }
            else {
                success = false;
                setTimeout(function () {
                    swal({
                        title: "View",
                        text: obj.Error.replace(/<br\s*\/?>/gim, "\n"),
                        type: 'error',
                        confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                        showCancelButton: false
                    });
                    $('.preloader').fadeOut(300, function () {
                        AvoidWebServiceRaceCondition = 0;
                    });
                }, 500);
            }
        } else {
            alert("This record does not exist");
        }

        if (success) {
            if ($(".DetailsGridView").length > 0) {
                AvoidWebServiceRaceCondition = 0;
                setTimeout(function () {
                    isFirstLoad = true;
                    GetUserConfiguration();
                    LoadItemsDetails();
                    $(".DetailsGridView").find(".chkSelectAll").prop("checked", false);
                }, 500);
            } else {
                $('.preloader').fadeOut(300, function () {
                    AvoidWebServiceRaceCondition = 0;
                });
            }
        }
    }

    function OnLoadError(response) {
        alert(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function DisplayItemDetails(DisplayID) {
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;
        $(".preloader").fadeIn();
        var pageUrl = sAppPath + 'WebServices/DisplayItemsDetails.ashx';

        var data = new FormData();
        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
        data.append("MyID", DisplayID);
        data.append("Facility", $('.NewHeaderRecord').find(".InputFacility").val());

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/DisplayItem",
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }

    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        var success = true;
        if (Object.keys(obj).length > 0) {
            if ($.trim(obj.Error) == '') {
                $(".btnNew").trigger("click");
                var MySavedDetailsValues = obj.SavedDetailsFields;
                var MyDetailsItems = MySavedDetailsValues.split(";;;");
                $.each(MyDetailsItems, function (i) {
                    var myvalue = "";
                    var myclass = "";

                    var MyDetailsItemsValues = MyDetailsItems[i].split(":::");
                    if (MyDetailsItemsValues.length > 0) { myclass = $(".NewDetailRecord").find(".InputDetails" + MyDetailsItemsValues[0].trim()); }
                    if (MyDetailsItemsValues.length > 1) { myvalue = MyDetailsItemsValues[1]; }

                    if (myclass.is("input")) {
                        if (myclass.attr("type") == "text") {
                            myclass.val(myvalue);
                        }
                    }
                    else if (myclass.is("select")) {
                        if ($('.MainPageTitle').attr("data-id") == "Warehouse_PO" || $('.MainPageTitle').attr("data-id") == "Warehouse_ASN" || $('.MainPageTitle').attr("data-id") == "Warehouse_SO" || $('.MainPageTitle').attr("data-id") == "Warehouse_OrderManagement") {
                            if (myclass.hasClass("InputAutoPostBackDetails")) {
                                AutoPostBackDetails($('.NewDetailRecord'), myvalue.split(','));
                                AvoidWebServiceRaceCondition = 1;
                            }
                        }
                        myclass.val(myvalue.split(',')).trigger("chosen:updated");
                        var myInterval = setInterval(function () {
                            if (!IsAutoPostBackDetails) {
                                myclass.val(myvalue.split(',')).trigger("chosen:updated");
                                clearInterval(myInterval);
                            }
                        }, 100);
                    }
                });

                var MyReadOnlyDetailsValues = obj.ReadOnlyDetailsFields;
                var MyReadOnlyDetailsItems = MyReadOnlyDetailsValues.split("~~~");
                $.each(MyReadOnlyDetailsItems, function (i) {
                    var myclass = "";
                    var MyReadOnlyDetailsValues = MyReadOnlyDetailsItems[i];
                    if (MyReadOnlyDetailsValues.length > 0) { myclass = $(".NewDetailRecord").find(".InputDetails" + MyReadOnlyDetailsValues.trim()); }
                    if (myclass.is("input")) { myclass.prop("disabled", true); }
                    else if (myclass.is("select")) { myclass.prop('disabled', true).trigger("chosen:updated"); }
                });
                $('.MyDetailRecordID').val(DisplayID);
                $(".MainPageDetailTitle").html("Edit Record");
            }
            else {
                success = false;
                setTimeout(function () {
                    swal({
                        title: "View",
                        text: obj.Error.replace(/<br\s*\/?>/gim, "\n"),
                        type: 'error',
                        confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                        showCancelButton: false
                    });
                    $('.preloader').fadeOut(300, function () {
                        AvoidWebServiceRaceCondition = 0;
                    });
                }, 500);
            }
        } else {
            alert("This record does not exist");
        }

        if (success) {
            $('.preloader').fadeOut(300, function () {
                AvoidWebServiceRaceCondition = 0;
            });
        }
    }

    function OnLoadError(response) {
        alert(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function LoadItems() {
    var TabName = $(".MyTab.Active").data("id");
    var QueryUrlStr = $(".QueryUrlStr").val();

    if ($('.MainPageTitle').attr("data-id") == "ChangePassword") {
        $('.btnQuickEntry').trigger("click");
    }
    else {
        if (AvoidWebServiceRaceCondition == 0) {
            AvoidWebServiceRaceCondition = 1;
            $(".HeaderGridView").find('.GridResults').remove();
            $(".HeaderGridView").find('.NoResults').hide();
            if (!isFirstLoad) $('.preloader').fadeIn();
            isFirstLoad = false;

            var pageUrl = sAppPath + 'WebServices/GetItems.ashx';

            var data = new FormData();
            data.append("SearchQuery", SearchQuery);
            data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
            data.append("SortBy", SortBy);
            data.append("QueryUrlStr", QueryUrlStr);
            data.append("TabName", TabName);

            $.ajax({
                type: "POST",
                contentType: false,
                processData: false,
                data: data,
                url: pageUrl + "/GetMore",
                success: OnLoadSuccess,
                error: OnLoadError
            });
        }

        function OnLoadSuccess(response) {
            var obj = jQuery.parseJSON(response);
            if (Object.keys(obj).length > 0) {
                $('.PagingNumbers').html("0 of 0");
                if ($.trim(obj.Records) !== '') {

                    if (TabName == "Actions") {
                        $(".GridActions").find('.SearchStyle').after(obj.Records);
                        $('.PagingNumbers').html("1-" + $(".GridActions").find('.GridResults').size() + " of " + $(".GridActions").find('.GridResults').size());
                    }
                    else if (TabName == "Reports") {
                        $(".GridReports").find('.SearchStyle').after(obj.Records);
                        $('.PagingNumbers').html("1-" + $(".GridReports").find('.GridResults').size() + " of " + $(".GridReports").find('.GridResults').size());

                    }
                    else if (TabName == "Dashboards") {
                        $(".GridDashboards").find('.SearchStyle').after(obj.Records);
                        $('.PagingNumbers').html("1-" + $(".GridDashboards").find('.GridResults').size() + " of " + $(".GridDashboards").find('.GridResults').size());
                    }
                    else {
                        $(".HeaderGridView").find('.SearchStyle').after(obj.Records);
                    }
                    if (TabName === undefined) {
                        $(".HeaderGridView").find(".GridResults").each(function () {
                            var $this = $(this);
                            $this.children(".GridCell[data-id]").each(function (e) {
                                var $thisChild = $(this);
                                $thisChild.attr("data-priority", $(".MyFields[data-priority]").eq(e).attr("data-priority"));
                                $thisChild.attr("data-hidden", $(".MyFields[data-hidden]").eq(e).attr("data-hidden"));
                            });
                        });
                        ShowHideColumns($(".HeaderGridView").find(".GridResults"));
                        ChangeColumnsOrder($(".HeaderGridView").find(".GridResults"));

                        PageTable("HeaderGridView");
                        MaxPages = Math.ceil($(".HeaderGridView").find('.GridResults').size() / NumberOfRecordsInPage);
                    }
                    else {
                        MaxPages = 1;
                    }

                    setTimeout(function () { InitColResizable(); }, 300);



                    if (MaxPages == 1 || CurrentPage == MaxPages - 1) {
                        $(".HeaderGridView").find(".Arrow-Right-Forward").addClass("Disabled");
                        $(".HeaderGridView").find(".Arrow-Right-Forward-Last").addClass("Disabled");
                    } else {
                        $(".HeaderGridView").find(".Arrow-Right-Forward").removeClass("Disabled");
                        $(".HeaderGridView").find(".Arrow-Right-Forward-Last").removeClass("Disabled");
                    }

                    $(".HeaderGridView").find(".GridContainer").on('click', '.editStyleNew', function () {
                        if ($('.MainPageTitle').attr("data-id") != "PROFILES") {
                            var myID = $(this).attr("data-id");
                            var myQueryURL = $(this).attr("data-queryurl");
                            DisplayItem(myID, myQueryURL);
                        }
                    });

                    $(".HeaderGridView").find(".GridContainer").on('click', '.editStyle', function () {
                        var myID = $(this).attr("data-id");
                        DisplayItemNew(myID);
                    });

                    if (TabName != "") {
                        var ActionName = "";
                        $('.chkSelectGrdEdit').change(function () {
                            ActionName = $.trim($(this).closest('.GridResults').find(".Initial").html());
                            UpdateProfileDetail($(this), "Edit", QueryUrlStr, TabName, ActionName);
                        });
                        $('.chkSelectGrdReadOnly').change(function () {
                            ActionName = $.trim($(this).closest('.GridResults').find(".Initial").html());
                            UpdateProfileDetail($(this), "Read Only", QueryUrlStr, TabName, ActionName);
                        });
                    }

                    if ($(".HiddenID").val() != 0 && $(".HiddenID").length > 0) {
                        setTimeout(function () {
                            AvoidWebServiceRaceCondition = 0;
                            DisplayItem($(".HiddenID").val(), '');
                        }, 1000);
                    }

                } else {
                    $(".HeaderGridView").find('.NoResults').show();
                }
            } else {
                $(".HeaderGridView").find('.NoResults').show();
            }

            $('.preloader').fadeOut(300, function () {
                AvoidWebServiceRaceCondition = 0;
            });
        }

        function OnLoadError(response) {
            alert(response.error);
            $('.preloader').fadeOut(300, function () {
                AvoidWebServiceRaceCondition = 0;
            });
        }
    }
}

function LoadItemsDetails() {
    AvoidWebServiceRaceCondition = 0
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;
        $(".DetailsGridView").find('.GridResults').remove();
        $(".DetailsGridView").find('.NoResults').hide();
        if (!isFirstLoad) { $('.preloader').fadeIn(); }
        isFirstLoad = false;

        var pageUrl = sAppPath + 'WebServices/GetItemsDetails.ashx';

        var data = new FormData();
        data.append("SearchQuery", SearchQueryDetails);
        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
        data.append("Facility", $('.NewHeaderRecord').find(".InputFacility").val());
        var Key;
        if ($('.MainPageTitle').attr("data-id") == "Warehouse_PO") {
            Key = $('.NewHeaderRecord').find(".InputPOKey").val();
        }
        else if ($('.MainPageTitle').attr("data-id") == "Warehouse_ASN") {
            Key = $('.NewHeaderRecord').find(".InputReceiptKey").val();
        }
        else if ($('.MainPageTitle').attr("data-id") == "Warehouse_SO") {
            Key = $('.NewHeaderRecord').find(".InputOrderKey").val();
        }
        else if ($('.MainPageTitle').attr("data-id") == "Warehouse_OrderManagement") {
            Key = $('.NewHeaderRecord').find(".InputOrderManagKey").val();
        }
        data.append("Key", Key);
        data.append("SortBy", SortByDetails);

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/GetMore",
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }

    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        if (Object.keys(obj).length > 0) {
            if ($.trim(obj.Records) !== '') {

                $(".DetailsGridView").find('.SearchStyle').after(obj.Records);

                $(".DetailsGridView").find(".GridResults").each(function () {
                    var $this = $(this);
                    $this.children(".GridCell[data-id]").each(function (e) {
                        var $thisChild = $(this);
                        $thisChild.attr("data-priority", $(".MyDetailsFields[data-priority]").eq(e).attr("data-priority"));
                        $thisChild.attr("data-hidden", $(".MyDetailsFields[data-hidden]").eq(e).attr("data-hidden"));
                    });
                });

                ShowHideColumns($(".DetailsGridView").find(".GridResults"));
                ChangeColumnsOrder($(".DetailsGridView").find(".GridResults"));

                setTimeout(function () { InitColResizable(); }, 300);

                PageTable("DetailsGridView");
                MaxPagesDetails = Math.ceil($(".DetailsGridView").find('.GridResults').size() / NumberOfRecordsInPage);

                if (MaxPagesDetails == 1 || CurrentPageDetails == MaxPagesDetails - 1) {
                    $(".DetailsGridView").find(".Arrow-Right-Forward").addClass("Disabled");
                    $(".DetailsGridView").find(".Arrow-Right-Forward-Last").addClass("Disabled");
                } else {
                    $(".DetailsGridView").find(".Arrow-Right-Forward").removeClass("Disabled");
                    $(".DetailsGridView").find(".Arrow-Right-Forward-Last").removeClass("Disabled");
                }

                $(".DetailsGridView").find(".GridContainer").on('click', '.editStyle', function () {
                    var myID = $(this).attr("data-id");
                    DisplayItemDetails(myID);
                });

            } else {
                $(".DetailsGridView").find('.NoResults').show();
            }
        } else {
            $(".DetailsGridView").find('.NoResults').show();
        }

        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
            if ($(".DetailsGridView").find(".GridContainer").data("resizemode") == "fit") setOnCufex_Resize();
        });
    }

    function OnLoadError(response) {
        alert(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function DisplayDropDowns() {
    AvoidWebServiceRaceCondition = 0;
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;
        //$('.preloader').fadeIn();
        var pageUrl = sAppPath + 'WebServices/DisplayDropDowns.ashx';

        var data = new FormData();
        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/DisplayDropDown",
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }

    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        if (Object.keys(obj).length > 0) {
            if ($.trim(obj.DropDownFields) != '') {
                var MyDropDownValues = obj.DropDownFields;
                var MyItems = MyDropDownValues.split(";;;");
                $.each(MyItems, function (i) {
                    var myvalue = "";
                    var myclass = "";

                    var MyItemsValues = MyItems[i].split(":::");
                    if (MyItemsValues.length > 0) { myclass = '.Input' + $.trim(MyItemsValues[0]); }
                    if (MyItemsValues.length > 1) { myvalue = MyItemsValues[1]; }

                    if ($(myclass).is("select")) {
                        $(myclass).empty();
                        if ($.trim(myvalue) !== '') {
                            var ItemArr = myvalue.split(',');
                            for (i = 0; i < ItemArr.length; i++) {
                                $(myclass).append("<option value='" + ItemArr[i] + "' >" + ItemArr[i] + "</option>");
                            }
                        }
                        $(myclass).trigger("chosen:updated");
                    }
                });
            }
        } else {
            alert("Couldn't Display Drop Downs");
        }

        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }

    function OnLoadError(response) {
        alert(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

var IsAutoPostBack = false;
function AutoPostBack(value) {
    AvoidWebServiceRaceCondition = 0;
    if (AvoidWebServiceRaceCondition == 0) {
        IsAutoPostBack = true;
        AvoidWebServiceRaceCondition = 1;
        if ($('.MainPageTitle').attr("data-id") == "SKUCATALOGUE" && value != "") $('.preloader').fadeIn();

        var pageUrl = sAppPath + 'WebServices/AutoPostBackDropDowns.ashx';

        var data = new FormData();
        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
        data.append("MyValue", value);

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/AutoPostBack",
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }
    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        var success = true;
        if (Object.keys(obj).length > 0) {
            if (obj.DropDownFields.indexOf("Error") >= 0) {
                success = false;
                setTimeout(function () {
                    swal({
                        title: "Load Items",
                        text: obj.DropDownFields,
                        type: 'error',
                        confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                        showCancelButton: false
                    });
                    $('.preloader').fadeOut(300, function () {
                        AvoidWebServiceRaceCondition = 0;
                        IsAutoPostBack = false;
                    });
                }, 500);
            }
            else {
                var MyDropDownValues = obj.DropDownFields;
                var MyItems = MyDropDownValues.split(";;;");
                var MyParentDivHeader = $(".HeaderGridView:visible").length > 0 ? $(".RecordHeader") : $(".NewHeaderRecord");

                $.each(MyItems, function (i) {
                    var myvalue = "";
                    var myclass = "";

                    var MyItemsValues = MyItems[i].split(":::");
                    if (MyItemsValues.length > 0) { myclass = MyParentDivHeader.find('.Input' + $.trim(MyItemsValues[0])); }
                    if (MyItemsValues.length > 1) { myvalue = MyItemsValues[1]; }

                    if (myclass.is("select")) {
                        myclass.empty();
                        if ($.trim(myvalue) !== '') {
                            var ItemArr = myvalue.split(',');
                            for (i = 0; i < ItemArr.length; i++) {
                                myclass.append("<option value='" + ItemArr[i] + "' >" + ItemArr[i] + "</option>");
                            }
                        }
                        myclass.trigger("chosen:updated");
                    }
                });

                var MyDropDownDetailsValues = obj.DropDownDetailsFields;
                var MyDetailsItems = MyDropDownDetailsValues.split(";;;");
                $.each(MyDetailsItems, function (i) {
                    var myvalue = "";
                    var myclass = "";

                    var MyItemsDetailsValues = MyDetailsItems[i].split(":::");
                    if (MyItemsDetailsValues.length > 0) { myclass = '.InputDetails' + $.trim(MyItemsDetailsValues[0]); }
                    if (MyItemsDetailsValues.length > 1) { myvalue = MyItemsDetailsValues[1]; }

                    if ($(".HeaderGridView:visible").length > 0) {
                        myclass = $(".RecordsContainer_Inside").find(myclass);
                        $('.RecordsContainer_Inside').each(function (e) {
                            if (myclass.eq(e).is("select")) {
                                myclass.eq(e).empty();
                                if ($.trim(myvalue) !== '') {
                                    var ItemArr = myvalue.split(',');
                                    for (i = 0; i < ItemArr.length; i++) {
                                        myclass.eq(e).append("<option value='" + ItemArr[i] + "' >" + ItemArr[i] + "</option>");
                                    }
                                }
                                myclass.eq(e).trigger("chosen:updated");
                            }
                        });
                    }
                    else {
                        myclass = $(".NewDetailRecord").find(myclass);
                        if (myclass.is("select")) {
                            myclass.empty();
                            if ($.trim(myvalue) !== '') {
                                var ItemArr = myvalue.split(',');
                                for (i = 0; i < ItemArr.length; i++) {
                                    myclass.append("<option value='" + ItemArr[i] + "' >" + ItemArr[i] + "</option>");
                                }
                            }
                            myclass.trigger("chosen:updated");
                        }
                    }
                });
            }
        } else {
            alert("Couldn't Display Drop Downs");
        }

        if (success) {
            setTimeout(function () {
                $('.preloader').fadeOut(300, function () {
                    AvoidWebServiceRaceCondition = 0;
                    IsAutoPostBack = false;
                });
            }, 500);
        }
    }

    function OnLoadError(response) {
        alert(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
            IsAutoPostBack = false;
        });
    }
}

var IsAutoPostBackDetails = false;
function AutoPostBackDetails(thiss, value) {
    AvoidWebServiceRaceCondition = 0;
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;
        IsAutoPostBackDetails = true;

        var pageUrl = sAppPath + 'WebServices/AutoPostBackDetailsDropDowns.ashx';

        var data = new FormData();
        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
        data.append("MyFacility", $('.InputFacility').val());
        data.append("MyValue", value);

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/AutoPostBack",
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }
    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        if (Object.keys(obj).length > 0) {
            var MyDropDownValues = obj.DropDownFields;
            var MyItems = MyDropDownValues.split(";;;");
            $.each(MyItems, function (i) {
                var myvalue = "";
                var myclass = "";

                var MyItemsValues = MyItems[i].split(":::");
                if (MyItemsValues.length > 0) { myclass = thiss.find('.InputDetails' + $.trim(MyItemsValues[0])); }
                if (MyItemsValues.length > 1) { myvalue = MyItemsValues[1]; }

                if ($(myclass).is("select")) {
                    $(myclass).empty();
                    if ($.trim(myvalue) !== '') {
                        var ItemArr = myvalue.split(',');
                        for (i = 0; i < ItemArr.length; i++) {
                            $(myclass).append("<option value='" + ItemArr[i] + "' >" + ItemArr[i] + "</option>");
                        }
                    }
                    $(myclass).trigger("chosen:updated");
                }
            });
        } else {
            alert("Couldn't Display Drop Downs");
        }
        AvoidWebServiceRaceCondition = 0;
        IsAutoPostBackDetails = false;
    }

    function OnLoadError(response) {
        alert(response.error);
        AvoidWebServiceRaceCondition = 0;
        IsAutoPostBackDetails = false;
    }
}

function GetSkuDropDown(MyDropDown, MyFacility, MyOwner, MySelectedValue) {
    if (MyDropDown.length > 0) {
        AvoidWebServiceRaceCondition = 0;
        if (AvoidWebServiceRaceCondition == 0) {
            AvoidWebServiceRaceCondition = 1;
            var pageUrl = sAppPath + 'WebServices/SearchDropDowns.ashx';

            var data = new FormData();
            data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
            data.append("MyFacility", MyFacility);
            data.append("MyOwner", MyOwner);

            $.ajax({
                type: "POST",
                contentType: false,
                processData: false,
                data: data,
                url: pageUrl + "/Search",
                success: OnLoadSuccess,
                error: OnLoadError
            });
        }

        function OnLoadSuccess(response) {
            var obj = jQuery.parseJSON(response);
            var showPreloader = true;
            if (Object.keys(obj).length > 0) {
                if ($.trim(obj.DropDownFields) != '') {
                    var MyDropDownValues = obj.DropDownFields;
                    var MyItems = MyDropDownValues.split(";;;");
                    $.each(MyItems, function (i) {
                        var myvalue = "";
                        var myclass = MyDropDown;

                        var MyItemsValues = MyItems[i].split(":::");
                        if (MyItemsValues.length > 1) { myvalue = MyItemsValues[1]; }

                        if ($(myclass).is("select") && $(myclass).val() == "") {
                            $(myclass).empty();
                            if ($.trim(myvalue) !== '') {
                                var ItemArr = myvalue.split(',');
                                for (i = 0; i < ItemArr.length; i++) {
                                    $(myclass).append("<option value='" + ItemArr[i] + "' >" + ItemArr[i] + "</option>");
                                }
                            }
                            if (MySelectedValue != "") $(myclass).val(MySelectedValue);
                            $(myclass).trigger("chosen:updated");
                        }
                    });
                }
            } else {
                alert("Couldn't Load Items");
            }

            if (showPreloader) {
                $('.preloader').fadeOut(300, function () {
                    AvoidWebServiceRaceCondition = 0;
                });
            }
        }

        function OnLoadError(response) {
            alert(response.error);
            $('.preloader').fadeOut(300, function () {
                AvoidWebServiceRaceCondition = 0;
            });
        }
    }
}

function SetPriceAndCurrency() {
    AvoidWebServiceRaceCondition = 0;
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;

        var items = "", owner = "", consignee = "";

        if ($('.RecordsContainer_Inside:visible').length > 0) {
            owner = $(".RecordHeader").find(".InputStorerKey").val();
            consignee = $(".RecordHeader").find(".InputConsigneeKey").val();
            $('.RecordsContainer_Inside:visible').find(".InputDetailsSku").each(function (e) {
                items += (e > 0 ? "," : "") + "'" + $(this).val() + "'";
            });
        }
        else {
            owner = $(".NewHeaderRecord").find(".InputStorerKey").val();
            consignee = $(".NewHeaderRecord").find(".InputConsigneeKey").val();
            items = "'" + $(".NewDetailRecord").find(".InputDetailsSku").val() + "'";
        }

        var pageUrl = sAppPath + 'WebServices/FetchPriceCurrency.ashx';

        var data = new FormData();

        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
        data.append("MyOwner", owner);
        data.append("MyConsignee", consignee);
        data.append("MyItems", items);

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/FetchData",
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }
    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        if (Object.keys(obj).length > 0) {
            var MyFieldsValues = obj.Fields;
            var MyFieldsItems = MyFieldsValues.split(";;;");

            $.each(MyFieldsItems, function (i) {
                var myvalue = "";
                var myclass = "";

                var MyFieldsItemsValues = MyFieldsItems[i].split(":::");
                if (MyFieldsItemsValues.length > 0) { myclass = '.InputDetails' + $.trim(MyFieldsItemsValues[0]); }
                if (MyFieldsItemsValues.length > 1) { myvalue = MyFieldsItemsValues[1].split("~~~"); }

                if ($(".HeaderGridView:visible").length > 0) {
                    myclass = $(".RecordsContainer_Inside").find(myclass);
                    $('.RecordsContainer_Inside').each(function (e) {
                        myclass.eq(e).val(myvalue[e]);
                    });
                }
                else {
                    myclass = $(".NewDetailRecord").find(myclass);
                    myclass.val(myvalue[0]);
                }
            });
        } else {
            alert("Couldn't Set Price and Currency");
        }
        AvoidWebServiceRaceCondition = 0;
    }
    function OnLoadError(response) {
        alert(response.error);
        AvoidWebServiceRaceCondition = 0;
    }
}

function ExecuteAction(MyItems, ActionID) {
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;

        $(".preloader").fadeIn();

        var pageUrl = sAppPath + 'WebServices/ActionButtons.ashx';

        var data = new FormData();
        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
        data.append("ActionID", ActionID);
        data.append("MyItems", MyItems);

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/ExecuteAction",
            success: OnLoadSuccess,
            error: OnLoadError
        });

    }
    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        var success = true;
        if (Object.keys(obj).length > 0) {
            if ($.trim(obj.Result) !== '') {
                success = false;
                swal({
                    title: "Action",
                    text: obj.Result.replace(/<br\s*\/?>/gim, "\n"),
                    type: 'error',
                    confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                    showCancelButton: false
                });
                $('.preloader').fadeOut(300, function () {
                    AvoidWebServiceRaceCondition = 0;
                });
            }
        } else {
            alert("Couldn't Execute Action");
        }

        if (success) {
            AvoidWebServiceRaceCondition = 0;
            $(".BackHeader").trigger("click");
            $(".HeaderGridView").mCustomScrollbar("scrollTo", "first");
            $(".HeaderGridView").mCustomScrollbar("update");
            isFirstLoad = true;
            LoadItems();
        }
    }

    function OnLoadError(response) {
        alert(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function UpdateProfileDetail(MyCheckbox, Type, QueryUrlStr, TabName, ActionName) {
    AvoidWebServiceRaceCondition = 0;
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;

        var pageUrl = sAppPath + 'WebServices/UpdateProfileDetail.ashx';

        var data = new FormData();
        data.append("QueryUrlStr", QueryUrlStr);
        data.append("TabName", TabName);
        data.append("ActionName", ActionName);
        if (Type == "Edit") {
            data.append("Edit", MyCheckbox.is(':checked'));
            data.append("ReadOnly", !MyCheckbox.is(':checked'));
        }
        else if (Type == "Read Only") {
            data.append("Edit", !MyCheckbox.is(':checked'));
            data.append("ReadOnly", MyCheckbox.is(':checked'));
        }

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/Update",
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }
    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        var showPreloader = true;
        if (Object.keys(obj).length > 0) {
            if (obj.Error != null) {
                showPreloader = false;
                swal({
                    title: "Update",
                    text: obj.Error.replace(/<br\s*\/?>/gim, "\n"),
                    type: 'error',
                    confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                    showCancelButton: false
                });
                AvoidWebServiceRaceCondition = 0;
            }
            else {
                if (Type == "Edit") {
                    MyCheckbox.closest('.GridResults').find(".chkSelectGrdReadOnly").prop("checked", !MyCheckbox.is(':checked'));
                }
                else if (Type == "Read Only") {
                    MyCheckbox.closest('.GridResults').find(".chkSelectGrdEdit").prop("checked", !MyCheckbox.is(':checked'));
                }
            }
        } else {
            alert("Couldn't Update Permission");
        }
        if (showPreloader) {
            AvoidWebServiceRaceCondition = 0;
        }
    }
    function OnLoadError(response) {
        alert(response.error);
        AvoidWebServiceRaceCondition = 0;
    }
}

function GetUserConfiguration() {
    AvoidWebServiceRaceCondition = 0;
    if (AvoidWebServiceRaceCondition == 0) {
        $(".DetailsGridView").find(".mCSB_container").css({ left: 0 });
        $(".DetailsGridView").find(".mCSB_dragger").css({ left: 0 });
        AvoidWebServiceRaceCondition = 1;
        var pageUrl = sAppPath + 'WebServices/UserConfiguration.ashx';

        var data = new FormData();
        data.append("ActionType", "get");
        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/UserConfigurationAction",
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }
    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        if (Object.keys(obj).length > 0) {
            if (obj.ColumnsNames != "") {
                if (obj.MenuOpen == "0") CloseMenu(false);

                var ColumnsNamesArr = obj.ColumnsNames.split(",");
                var ColumnsPrioritiesArr = obj.ColumnsPriorities.split(",");
                var ColumnsHiddenArr = obj.ColumnsHidden.split(",");
                var ColumnsWidthesArr = obj.ColumnsWidthes.split(",");
                var GridTypesArr = obj.GridTypes.split(",");
                var timer = obj.MenuOpen == "0" ? 500 : 0;

                setTimeout(function () {
                    var MyWidth = 0;
                    var MyWidthDetails = 0;
                    var $MyGrid;

                    for (var i = 0; i < ColumnsNamesArr.length; i++) {
                        var width = ColumnsWidthesArr[i];
                        if (GridTypesArr[i] == "Header") {
                            $MyGrid = $(".HeaderGridView");
                            MyWidth += parseFloat(width);
                        }
                        else {
                            $MyGrid = $(".DetailsGridView");
                            MyWidthDetails += parseFloat(width);
                        }
                        if (ColumnsPrioritiesArr[i] != "0") {
                            var field = ColumnsNamesArr[i];
                            var priority = ColumnsPrioritiesArr[i];
                            var hidden = ColumnsHiddenArr[i] == "0" ? "false" : "true";
                            var $thisChild = $MyGrid.find(".GridAdjust").children(".GridCell[data-id='" + field + "']");
                            var index = $thisChild.index();
                            $thisChild.attr("data-priority", priority);
                            $thisChild.outerWidth(width);
                            $thisChild.attr("data-hidden", hidden);
                            if (GridTypesArr[i] == "Header") {
                                $(".MyFields[value='" + field + "']").attr("data-priority", priority);
                                $(".MyFields[value='" + field + "']").attr("data-hidden", hidden);
                            }
                            else {
                                $(".MyDetailsFields[value='" + field + "']").attr("data-priority", priority);
                                $(".MyDetailsFields[value='" + field + "']").attr("data-hidden", hidden);
                            }
                            $MyGrid.find(".SearchStyle").children(".GridCell").eq(index).attr("data-priority", priority);
                            $MyGrid.find(".SearchStyle").children(".GridCell").eq(index).attr("data-hidden", hidden);
                            $MyGrid.find(".NoResults").children(".GridCell").eq(index).attr("data-hidden", hidden);
                            $MyGrid.find(".GridResults").each(function () {
                                $(this).children(".GridCell").eq(index).attr("data-hidden", hidden);
                                $(this).children(".GridCell").eq(index).attr("data-priority", priority);
                            });
                        }
                        else {
                            $MyGrid.find(".GridAdjust").children(".GridCell").eq(i).outerWidth(width);
                        }
                    }

                    ShowHideColumns($(".GridRow"));
                    ChangeColumnsOrder($(".GridRow"));

                    $(".GridRow").not(".NoResults").each(function () {
                        var $thisChild = $(this).children(".GridCell[data-priority]");
                        $thisChild.removeClass("borderRight0");
                        $thisChild.eq($thisChild.length - 1).addClass("borderRight0");
                    });

                    var isHeader = $(".HeaderGridView").find(".GridContainer").data("resizemode") == "overflow";
                    var isDetail = $(".DetailsGridView").find(".GridContainer").data("resizemode") == "overflow";
                    setTimeout(function () {
                        if (isHeader) $(".HeaderGridView").find(".mCSB_container").width(MyWidth);
                        if (isDetail) $(".DetailsGridView").find(".mCSB_container").width(MyWidthDetails);
                    }, 100);

                }, timer);
            }
        } else {
            alert("Couldn't Get User Configuration");
        }

        AvoidWebServiceRaceCondition = 0;
        setTimeout(function () {
            InitColResizable();
        }, 300);
    }
    function OnLoadError(response) {
        alert(response.error);
        AvoidWebServiceRaceCondition = 0;
        setTimeout(function () {
            InitColResizable();
        }, 300);
    }
}

function SetUserConfigution() {
    AvoidWebServiceRaceCondition = 0;
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;
        var ColumnsNames = ""
        var ColumnsPriorities = ""
        var ColumnsWidthes = ""
        var ColumnsHidden = ""
        var GridTypes = ""

        $(".GridAdjust").children(".GridCell").each(function (e) {
            var $this = $(this);
            if ($this.closest(".HeaderGridView").length > 0) GridTypes += "Header" + ",";
            else GridTypes += "Detail" + ",";

            if ($this.data("id") == null) {
                ColumnsNames += e.toString() + ",";
                ColumnsPriorities += "0" + ",";
                ColumnsHidden += "0" + ",";
            }
            else {
                ColumnsNames += $this.data("id") + ",";
                ColumnsPriorities += ($this.data("priority") == null ? "500" : $this.data("priority")) + ",";
                ColumnsHidden += ($this.attr("data-hidden") == "true" ? "1" : "0") + ",";
            }
            ColumnsWidthes += $this.outerWidth() + ",";
        });

        var pageUrl = sAppPath + 'WebServices/UserConfiguration.ashx';

        var data = new FormData();
        data.append("ActionType", "set");
        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
        data.append("MenuOpen", MenuClosed ? 0 : 1);
        data.append("ColumnsNames", ColumnsNames.slice(0, -1));
        data.append("ColumnsPriorities", ColumnsPriorities.slice(0, -1));
        data.append("ColumnsWidthes", ColumnsWidthes.slice(0, -1));
        data.append("ColumnsHidden", ColumnsHidden.slice(0, -1));
        data.append("GridTypes", GridTypes.slice(0, -1));

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/UserConfigurationAction",
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }
    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        if (Object.keys(obj).length > 0) {
            if (obj.Error != null) {
                swal({
                    title: "Update",
                    text: obj.Error,
                    type: 'error',
                    confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                    showCancelButton: false
                });
                AvoidWebServiceRaceCondition = 0;
            }
        } else {
            alert("Couldn't Set User Configuration");
        }
        setTimeout(function () {
            AvoidWebServiceRaceCondition = 0;
            $(".preloader").fadeOut();
        }, 300);
    }
    function OnLoadError(response) {
        alert(response.error);
        setTimeout(function () {
            AvoidWebServiceRaceCondition = 0;
            $(".preloader").fadeOut();
        }, 300);
    }
}

function SetScrolling() {
    $(".content_3").mCustomScrollbar({
        updateOnContentResize: true,
        scrollButtons: {
            enable: false
        },
        axis: "x",
        theme: "dark"
    });

    $(".content_4").mCustomScrollbar({
        updateOnContentResize: true,
        scrollButtons: {
            enable: false
        },
        axis: "y",
        theme: "dark",
        scrollInertia: 100
    });
}

//function ShowAlert() {
//    setTimeout(function () {
//        swal({
//            title: $('.AlertTitle').val(),
//            text: $('.AlertText').val(),
//            type: $('.AlertType').val(),
//            confirmButtonColor: $('.AlertconfirmButtonColor').val(),
//            cancelButtonText: $('.AlertcancelButtonText').val(),
//            imageUrl: $('.AlertimageUrl').val(),
//            imageSize: $('.AlertimageSize').val(),
//            timer: $('.Alerttimer').val(),
//            allowOutsideClick: $('.AlertallowOutsideClick').val(),
//            showCancelButton: $('.AlertshowCancelButton').val(),
//            confirmButtonText: $('.AlertconfirmButtonText').val(),
//            closeOnConfirm: $('.AlertcloseOnConfirm').val()
//        });
//    }, 100);
//}

function OpenMenu() {
    if (NtorKermelElraceCondition == 0) {
        NtorKermelElraceCondition = 1;
        ClearConfigTimer();
        MenuClosed = false;
        $(".widthMenu,.MenuContainer,.widthContent").removeClass("Closed");
        $(".AllMenuItems,.Gtitle").hide();
        $("table.u-marginAuto").fadeOut();
        setTimeout(function () {
            $(".MainLogoContainer,.MenuItemName,.MenuSubItem,.MenuSubSubItem,.MenuItemStyleSel,.u-pl-xs-20,.u-pb-xs-30").removeClass("Closed");
            $(".OpenMenu").addClass("Closed");
            $(".AllMenuItems").fadeIn();
            $(".Gtitle").show();
            setOnCufex_Resize();
            $("table.u-marginAuto").fadeIn();
            InitColResizable();
            SetConfigTimer();
            NtorKermelElraceCondition = 0;
        }, 300);
    }
}

function CloseMenu(resize) {
    if (NtorKermelElraceCondition == 0) {
        NtorKermelElraceCondition = 1;
        ClearConfigTimer();
        MenuClosed = true;
        $(".MainLogoContainer,.MenuItemName,.MenuSubItem,.MenuSubSubItem,.MenuItemStyleSel,.u-pl-xs-20,.u-pb-xs-30,.widthMenu,.MenuContainer,.widthContent").addClass("Closed");
        $(".OpenMenu").removeClass("Closed");
        $(".MenuPin").removeClass("Pinned");
        $("table.u-marginAuto").fadeOut();
        setTimeout(function () {
            setOnCufex_Resize();
            $("table.u-marginAuto").fadeIn();
            setTimeout(function () { InitColResizable(); }, 100);
            if (resize) {
                setTimeout(function () { setOnCufex_Resize(); }, 100);
                SetConfigTimer();
            }
            NtorKermelElraceCondition = 0;
        }, 300);
    }
}

function InitColResizable() {
    if ($('.MainPageTitle').attr("data-id") != "PROFILEDETAIL") {
        var $MyGrid = $(".HeaderGridView:visible").length > 0 ? $(".HeaderGridView") : $(".DetailsGridView");

        $MyGrid.find(".GridContainer").removeClass("JCLRFlex").removeClass("JColResizer");
        $MyGrid.find(".GridContainer").removeAttr("id");
        $MyGrid.find(".JCLRgrips").remove();

        var resizeMode = $MyGrid.find(".GridContainer").data("resizemode");
        $MyGrid.find(".GridContainer").colResizable({
            liveDrag: true,
            resizeMode: resizeMode,
            onDrag: DragColumns,
            onResize: ResizeColumns
        });

        //var ColNoResize = $(".GridAdjust").children(".GridCell:not([data-id])").length;
        //$('.JCLRgrips').find(".JCLRgrip:lt(" + ColNoResize + ")").remove();

        function ResizeColumns() {
            if ($(".GridContainer").data("resizemode") == "overflow") {
                if ($(".HeaderGridView:visible").length > 0) $(".HeaderGridView").mCustomScrollbar("update");
                else $(".DetailsGridView").mCustomScrollbar("update");
            }
            SetConfigTimer();
        }

        function DragColumns() {
            ClearConfigTimer();
        }
    }
}

var ConfigTimer;
function SetConfigTimer() {
    ConfigTimer = setTimeout(function () {
        SetUserConfigution();
    }, 3000);
}

function ClearConfigTimer() {
    clearTimeout(ConfigTimer);
}

function SetMenuFunctionality() {
    $('.openProfMenu').unbind("click").bind("click", function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('.ProfileData').stop(true, true).slideToggle(200);
        $('.iconOpen').stop(true, true).toggleClass('rotate180');
    });

    $(".widthMenu").mouseleave(function (e) {
        e.stopPropagation();
        if (!$('.MenuPin').hasClass("Pinned") && !MenuClosed) {
            CloseMenu(true);
        }
    });

    $('.MenuItemStyle,.MenuSubItem,.MenuSubSubItem ').attr({ "tabindex": "-1" });

    $('.MenuItemStyle').unbind('click').click(function (e) {
        if (NtorKermelElraceCondition == 0) {
            if ($('.MenuItemName').is(":visible")) {
                var myObj = $(this);

                if (myObj.find(".MenuArrow").hasClass("Opened")) myObj.find(".MenuArrow").removeClass("Opened");
                else myObj.find(".MenuArrow").addClass("Opened");
                $(".MenuItemStyleSel").not(myObj).find(".MenuArrow").removeClass("Opened");

                $(".MenuItemStyleSel").not(myObj).removeClass("MenuItemStyleSel").animate({ "height": "55px" }, 200);
                if (myObj.hasClass("MenuItemStyleSel")) myObj.removeClass("MenuItemStyleSel").animate({ "height": "55px" }, 200);
                else myObj.addClass("MenuItemStyleSel").css({ "height": "55px" }).animate({ "height": (parseInt(myObj.children('div').height()) + 20) + "px" }, 200);

                setTimeout(function () {
                    $(".MenuItemStyleSel").each(function () {
                        $(this).animate({ "height": (parseInt($(this).children('div').height()) + 20) + "px" });
                    });
                    $('.GoToChildOnClick').unbind(myClickEvent).bind(myClickEvent, function () {
                        //GotoParentIfInIfram(false, $(this).find('a').attr("href"), 0, 0);
                    });
                }, 200);
            }
            else {
                OpenMenu();
                var myObj = $(this);
                myObj.find(".MenuArrow").addClass("Opened");
                $(".MenuItemStyleSel").not(myObj).find(".MenuArrow").removeClass("Opened");
                setTimeout(function () {
                    $(".MenuItemStyleSel").not(myObj).removeClass("MenuItemStyleSel").height("55px");
                    myObj.addClass("MenuItemStyleSel").css({ "height": "55px" }).animate({ "height": (parseInt(myObj.children('div').height()) + 20) + "px" }, 200);
                }, 300);
            }
        }
        e.stopPropagation();
    });

    $('.MenuSubItem').unbind('click').click(function (e) {
        if (NtorKermelElraceCondition == 0) {
            var myObj = $(this);
            $(".MenuSubItemSel").not(myObj).removeClass("MenuSubItemSel").animate({ "height": "22px" }, 200);

            if (myObj.hasClass("MenuSubItemSel")) myObj.removeClass("MenuSubItemSel").animate({ "height": "22px" }, 200);
            else myObj.addClass("MenuSubItemSel").css({ "height": "22px" }).animate({ "height": parseInt(myObj.children('div').height()) + "px" }, 200);
            setTimeout(function () {
                $(".MenuItemStyleSel").each(function () {
                    $(this).animate({ "height": (parseInt($(this).children('div').height()) + 20) + "px" });
                });
            }, 200);

        }
        e.stopPropagation();
    });


    $('.DivMenuInLogoResp').unbind('click').click(function () {
        if (parseInt($('.MenuItemsContainer').css("margin-top")) == 0) $('.MenuItemsContainer').stop(true, true).animate({ "margin-top": "-300%" }, 600, "easeInCubic");
        else $('.MenuItemsContainer').stop(true, true).animate({ "margin-top": "0px" }, 600, "easeOutCubic");
    });

    $('.PinMenu').unbind('click').click(function (e) {
        if ($(".MenuPin").hasClass("Pinned")) {
            CloseMenu(true);
        }
        else {
            $(".MenuPin").addClass("Pinned");
        }
    });


    $('.OpenMenu').click(function () {
        if (MenuClosed) {
            OpenMenu();
        }
        else {
            CloseMenu(true);
        }
    });
}

//function isValidDate(s) {
//    var bits = s.split('-');
//    var d = new Date(bits[2] + '-' + bits[1] + '-' + bits[0]);
//    return !!(d && (d.getMonth() + 1) == bits[1] && d.getDate() == Number(bits[0]));
//}
//function ClickToPrintClass(ClassName) {
//    var MyBody = $('.' + ClassName).val();

//    var mywindow = window.open('', 'SNSsoftware', 'scrollbars=1,height=600,width=800');
//    mywindow.document.write('<html>');
//    mywindow.document.write('<body style="direction:ltr; text-align: left;">');
//    mywindow.document.write(MyBody);
//    mywindow.document.write('</body></html>');
//    mywindow.focus();
//    mywindow.print();
//    mywindow.document.close();
//    return true;
//}
//function PictureUpload() {

//    $('.BtnUploadPicture').unbind("click").on("click", function () {
//        var $this = $(this);
//        var $Parent = $this.closest(".Cufex_Attachment");
//        $(".Cufex_PopUpAttachment").hide();
//        $Parent.find(".Cufex_PopUpAttachment").fadeIn("fast", function () {
//            setOnCufex_Resize();
//            setTimeout(function () {
//                $('html, body').animate({ scrollTop: ($Parent.offset().top - $(window).height() * 0.4) }, 'slow', "easeInOutCubic");
//            }, 100);
//        });
//    });

//    $("html").on("dragover", function (e) {
//        e.preventDefault();
//        e.stopPropagation();
//        $(".ImgDivMessage").text("Drag here");
//    });

//    $("html").on("drop", function (e) {
//        e.preventDefault();
//        e.stopPropagation();
//        $(".ImgDivMessage").text("Drop");
//    });

//    $("html").on("dragleave", function (e) {
//        e.preventDefault();
//        e.stopPropagation();
//        $(".ImgDivMessage").text("click browse or drop only one image here");
//    });

//    $('.ImgDiv').on('dragenter', function (e) {
//        e.stopPropagation();
//        e.preventDefault();
//    });

//    // Drag over
//    $('.ImgDiv').on('dragover', function (e) {
//        $parent = $(this).closest(".Cufex_Attachment");
//        e.stopPropagation();
//        e.preventDefault();

//        $parent.find(".ImgDivMessage").text("Drop");
//    });

//    // Drop
//    $('.ImgDiv').on('drop', function (e) {
//        $parent = $(this).closest(".Cufex_Attachment");
//        e.stopPropagation();
//        e.preventDefault();
//        $parent.find(".ImgDivMessage").text("Upload");
//        $parent.find(".fileImages").prop('files', e.originalEvent.dataTransfer.files);
//        $parent.find(".fileImages").trigger('change');
//    });

//    $(".fileImages").change(function () {
//        readAndSaveImgURL(this, $(this).closest(".Cufex_Attachment"));
//    });

//    function readAndSaveImgURL(input, Parent) {
//        if (input.files && input.files[0]) {
//            if (jcrop_api) { jcrop_api.disable(); jcrop_api.release(); jcrop_api.destroy(); }
//            var Img = Parent.find('.target');
//            Img.removeAttr('style');
//            Img.attr('src', '#');
//            //Img.hide();
//            Parent.find('.BtnAttach').fadeOut();
//            Parent.find('.lblBrowseError').text();
//            Parent.find('.DivBrowseError').hide();
//            Parent.find('.HiddenRatio').val(1);

//            var ImgType = input.files[0].type.toLowerCase();
//            if (ImgType.indexOf("png") >= 0 || ImgType.indexOf("jpg") >= 0 || ImgType.indexOf("jpeg") >= 0 || ImgType.indexOf("gif") >= 0 || ImgType.indexOf("tiif") >= 0 || ImgType.indexOf("svg") >= 0) {
//                var reader = new FileReader();
//                reader.onload = function (e) {
//                    var binimage = reader.result;
//                    var VirtualImg = new Image();
//                    VirtualImg.onload = function () {
//                        if (this.width > Parent.find('.ImgDiv').width()) {
//                            Parent.find('.HiddenRatio').val(Parent.find('.ImgDiv').width() / this.width);
//                        } else Parent.find('.HiddenRatio').val(1);
//                        Parent.find('.BtnAttach').fadeIn();
//                        //if (jcrop_api) jcrop_api.destroy();

//                        if (!Img.is(":visible")) Img.show();
//                        Img.attr('src', binimage);
//                        if (ImgType.indexOf("svg") <= 0) SetJcrop(Img);
//                        setOnCufex_Resize();
//                    };
//                    VirtualImg.src = binimage;
//                };
//                reader.readAsDataURL(input.files[0]);
//            } else {
//                Parent.find('.DivBrowseError').show();
//                Parent.find('.lblBrowseError').text("Invalid Format");
//            }
//        }
//    }

//    $('.Close_Cufex_PopUpAttachment').unbind("click").on("click", function () {
//        $('.Cufex_PopUpAttachment').fadeOut();
//    });

//    function SetJcrop($this) {
//        var $Parent = $this.closest(".Cufex_Attachment");

//        var SelectWidth = $Parent.find('.HiddenWidth').val() * $Parent.find('.HiddenRatio').val();
//        var SelectHeight = $Parent.find('.HiddenHeight').val() * $Parent.find('.HiddenRatio').val();

//        var Ratio = "";

//        if (SelectWidth != 0 && SelectHeight != 0) Ratio = SelectWidth / SelectHeight;

//        if (SelectWidth == 0) SelectWidth = $this.width();
//        if (SelectHeight == 0) SelectHeight = $this.height();

//        $this.Jcrop({
//            onChange: showCoords
//            , onSelect: showCoords
//            //, minSize: [$Parent.find('.HiddenMinWidth').val() * $Parent.find('.HiddenRatio').val(), $Parent.find('.HiddenMinHeight').val() * $Parent.find('.HiddenRatio').val()]
//            //, maxSize: [$Parent.find('.HiddenMaxWidth').val() * $Parent.find('.HiddenRatio').val(), $Parent.find('.HiddenMaxHeight').val() * $Parent.find('.HiddenRatio').val()]
//            , aspectRatio: Ratio

//        }, function () {
//            jcrop_api = this;
//            SelectWidth = $Parent.find('.HiddenWidth').val() * $Parent.find('.HiddenRatio').val();
//            SelectHeight = $Parent.find('.HiddenHeight').val() * $Parent.find('.HiddenRatio').val();

//            if (SelectWidth == 0) SelectWidth = $this.width();
//            if (SelectHeight == 0) SelectHeight = $this.height();

//            //if (SelectWidth == 0) SelectWidth = $Parent.find('.HiddenMaxWidth').val() * $Parent.find('.HiddenRatio').val();
//            //if (SelectHeight == 0) SelectHeight = $Parent.find('.HiddenMaxHeight').val() * $Parent.find('.HiddenRatio').val();
//            // alert(SelectWidth + " " + SelectHeight)
//            jcrop_api.setSelect([0, 0, SelectWidth, SelectHeight]);
//            setOnCufex_Resize();
//        });

//        function showCoords(c) {
//            $Parent.find('.HiddenCropX').val(c.x);
//            $Parent.find('.HiddenCropY').val(c.y);
//            $Parent.find('.HiddenCropWidth').val(c.w);
//            $Parent.find('.HiddenCropHeight').val(c.h);

//            $Parent.find('.HiddenOriginalWidth').val($Parent.find('.target').width());
//            $Parent.find('.HiddenOriginalHeight').val($Parent.find('.target').height());
//        }
//    }

//    var AvoidWebAttachmentRaceCondition = 0;
//    $('.AttachPicture').unbind("click").on("click", function () {
//        var $Parent = $(this).closest(".Cufex_Attachment");
//        $Parent.find('.BtnAttach').hide();
//        $Parent.find('.BtnAttachLoader').show();
//        if (AvoidWebAttachmentRaceCondition == 0) {
//            var pageUrl = sAppPath + 'WebServices/Attachment_Upload.ashx';

//            var data = new FormData();
//            data.append("UploadedFile", $Parent.find(".fileImages").get(0).files[0]);
//            data.append("PhysicalAttachmentLocation", $Parent.find('.HiddenPhysicalAttachmentLocation').val().split("\\").join("/"));
//            data.append("SectionName", $Parent.find('.HiddenSectionName').val());
//            data.append("FolderPath", $Parent.find('.HiddenFolderPath').val().split("\\").join("/"));
//            data.append("OriginalHeight", $Parent.find('.HiddenOriginalHeight').val());
//            data.append("OriginalWidth", $Parent.find('.HiddenOriginalWidth').val());
//            data.append("WidthValue", $Parent.find(".HiddenWidth").val());
//            data.append("HeightValue", $Parent.find(".HiddenHeight").val());
//            data.append("CropWidth", $Parent.find(".HiddenCropWidth").val());
//            data.append("CropHeight", $Parent.find(".HiddenCropHeight").val());
//            data.append("CropX", $Parent.find(".HiddenCropX").val());
//            data.append("CropY", $Parent.find(".HiddenCropY").val());
//            data.append("Quality", $Parent.find(".cmbQuality").val());
//            data.append("sAppPath", $(".sAppPath").val().split("\\").join("/"));

//            $.ajax({
//                type: "POST",
//                contentType: false,
//                processData: false,
//                data: data,
//                url: pageUrl + "/ProcessRequest",
//                //data: data,
//                //contentType: "application/json; charset=utf-8",
//                //dataType: "json",
//                //async: true,
//                success: OnLoadSuccess,
//                error: OnLoadError
//            });
//        }

//        function OnLoadSuccess(response) {

//            var obj = jQuery.parseJSON(response);
//            $Parent.find('.txtPicture').val(obj.NewImageName);
//            $Parent.find('.hrViewPicture').attr('href', obj.ImagePath);
//            $Parent.find('.hrViewPicture').attr('title', 'Found in ' + obj.FolderPath);

//            $Parent.find('.img').attr('src', obj.ImagePath);
//            $Parent.find('.img').attr('title', 'Found in ' + obj.FolderPath);

//            $Parent.find('.Cufex_PopUpAttachment').fadeOut(function () {
//                if (Pirobox) SetPiroBox();
//                AvoidWebAttachmentRaceCondition = 0;
//                $Parent.find('.BtnAttach').show();
//                $Parent.find('.BtnAttachLoader').hide();
//            });
//        }

//        function OnLoadError(response) {
//            console.log('Load Error: ' + response);
//            AvoidWebAttachmentRaceCondition = 0;
//        }
//    });
//}

function setOnCufex_Resize() {
    $('.CufexLogoTD,.addSpacebeforeHeader').css({ "height": "72px" });
    $('.BackMenuDiv').css({ "width": "0px" });

    $('.MainPageContainer').height($(window).height());
    $('.BeforeMainPage').css({ "height": (parseInt($(window).height()) - parseInt($('.MainPageDivInside').height())) / 2 + "px" });
    SetMasterResize();
    $('.Cufex_PopUpAttachment').css({ "top": $('.ImgTxt').height() });

    if ($('.Containerbase').length > 0) {
        $('.Containerbase').each(function () {
            var myHeight = $(this).height();
            var ParentHt = $(this).parent('.AbsoPopup').height();
            if (myHeight < ParentHt) $(this).css({ 'top': (ParentHt - myHeight) / 2 + 'px' });
        });
    }

    $(".content_3").mCustomScrollbar("update");
    $(".content_3").width(0);
    $(".content_3").width($('.MainHeader').width());
    $(".mCSB_container").width($(".content_3").width());

    $(".content_4").mCustomScrollbar("update");
}