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
var myDropzone;
Dropzone.autoDiscover = false;
var MyWidth = 0;
var MyWidthDetails = 0;


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
    if ($(".HiddenMenuOpen").val() == 0) {
        CloseMenu(false);
        $(".DivMain").css({ opacity: 0 });
        setTimeout(function () {
            $(".DivMain").css({ opacity: 1 });
        }, 500);
    } else {
        $(".DivMain").css({ opacity: 1 });
    }

    SNSFunctions();
    SetMenuFunctionality();
    SetScrolling();
    $(document).unbind("click").on("click", function (e) {
        $('.ActionHiddenButtons').slideUp();
        $('.ProfileData').slideUp();

        if (!$(e.target).closest(".chosen-container").length > 0) {
            $(".NewHeaderRecord").addClass("u-overflowHidden");
        }

        if (!$(e.target).closest(".widthMenu").length > 0 && !$('.MenuPin').hasClass("Pinned") && !MenuClosed) {
            CloseMenu(true);
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
    SetDropZone();
    SetUITemplate();
    SetCarrierEvents();

    $('.btnQuickEntry').click(function () {
        $('.AddDetailsBtn').hide();
        $(".ErrorIcon").remove();
        $('.MyRecordID').val(0);
        $('.FloatRecordField, .Details_FloatRecordField').find('input:text,select').each(function () {
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
        $('.FloatRecordField').find('input:password').removeClass("Error");
        $('.FloatRecordField').find('input:checkbox').prop("false");
        //$('.FloatRecordField').find('select').val('').trigger('chosen:updated');
        $('.FloatRecordField').find('select').prop('disabled', false).trigger("chosen:updated");

        AutoPostBack("");
        $('.New_Modify_Record_PopUp').fadeIn(function () {
            setOnCufex_Resize();
            $('.AddDetailsBtn').show();
            $('.RecordsContainer_Inside').eq(0).find('.InputDetailsSku').empty().trigger("chosen:updated");
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
        $('.BackDetail,.btnExport,.btnImport,.btnRefresh,.VerticalSep').hide();
        $(".MainPageDetailTitle").html($(".MainPageDetailTitle").data("text"));
        if ($(window).width() > 550) {
            $(".btnDelete").parent("td").css("padding-right", "0px");
            $(".ActionHiddenButtons").css("right", "0");
        }

        $('.NewDetailRecord').find('.InputDetailsSku,.InputDetailsPackKey,.InputDetailsUOM').empty().trigger("chosen:updated");

        if ($('.MainPageTitle').attr("data-id") == "Warehouse_PO" || $('.MainPageTitle').attr("data-id") == "Warehouse_ASN" || $('.MainPageTitle').attr("data-id") == "Warehouse_SO" || $('.MainPageTitle').attr("data-id") == "Warehouse_OrderManagement") {
            $('.NewHeaderRecord').find('.InputStorerKey,.InputPOType,.InputSellerName,.InputCarrierKey,.InputOrderType,.InputReceiptType,.InputConsigneeKey,.InputType').empty().trigger("chosen:updated");
        }
        else if ($('.MainPageTitle').attr("data-id") == "SKUCATALOGUE") {
            $('.NewHeaderRecord').find('.InputSku').empty().trigger("chosen:updated");
        }

        $('.NewHeaderRecord, .NewDetailRecord').find('input:text,select').each(function () {
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
        //$('.NewDetailRecord, .NewHeaderRecord').find('select').val('').trigger('chosen:updated');
        $('.NewDetailRecord, .NewHeaderRecord').find('select').prop('disabled', false).trigger("chosen:updated");
        if ($(".dropzone").length > 0) {
            Dropzone.forElement('.dropzone').removeAllFiles(true);
            $(".dz-preview").remove();
            $(".dz-default").show();
        }
        setOnCufex_Resize();
    });

    $('.BackHeader').click(function () {
        //if ($(".HeaderGridView").find(".GridContainer").data("resizemode") != "overflow") setOnCufex_Resize();
        //else {
        //    if ($(".HeaderGridView").find(".mCSB_container").width() >= $(".MainHeader").width()) setOnCufex_Resize();
        //}
        setOnCufex_Resize();
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
        $(".NewRecord, .btnSave, .BackHeader,.CarrierEventsBtn").hide();
        $('.btnAddNew,.btnQuickEntry').show();
        $('.btnAddNew').parent("td").prev("td").show();
        $('.btnExport,.btnImport,.btnRefresh,.VerticalSep').show();
        $(".MainPageDesc").html($(".MainPageDesc").data("text"));

        if ($(window).width() <= 550) {
            $(".btnDelete").parent("td").css("padding-right", "10px");
            $(".btnDelete").parent("td").css("margin-bottom", "10px");
            $(".btnActions").parent("td").css("padding-right", "0px");
        }

        if ($(".dropzone").length > 0) {
            Dropzone.forElement('.dropzone').removeAllFiles(true);
            $(".dz-preview").remove();
            $(".dz-default").show();
        }


        setTimeout(function () {
            InitColResizable();
        }, 300);

        if ($(".HiddenDetailLink").length > 0) {
            window.history.pushState('object or string', "0", $(".HiddenDetailLink").val());
        }
    });

    $('.btnNew').click(function () {
        $('.MyDetailRecordID').val(0);
        $(".DetailsGridView,.btnDeleteDetail,.btnExportDetails,.btnRefreshDetails,.VerticalSep2").hide();
        $(".VerticalSep2").parent("td").prev("td").hide();
        $(".VerticalSep2").parent("td").next("td").hide();
        $(".btnRefreshDetails").parent("td").next("td").hide();
        $(".BackDetail").parent("td").next("td").hide();
        $(this).hide();
        $(this).parent("td").next("td").hide();
        $(".NewDetailRecord").fadeIn();
        $(".BackDetail").show();
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
        var MyFacility = $(".NewHeaderRecord").find('.InputFacility').val().toString();
        MyFacility = MyFacility.substring(MyFacility.lastIndexOf(",") + 1);
        var MyOwner = $(".NewHeaderRecord").find('.InputStorerKey').val().toString();
        MyOwner = MyOwner.substring(MyOwner.lastIndexOf(",") + 1);
        GetSkuDropDown(MyDropDown, MyFacility, MyOwner, "");

        setOnCufex_Resize();
    });

    $('.BackDetail').click(function () {
        $('.MyDetailRecordID').val(0);

        $(".DetailsGridView").fadeIn();
        //if ($(".DetailsGridView").find(".GridContainer").data("resizemode") == "overflow" && MyWidthDetails != 0) {
        //    $(".DetailsGridView").find(".mCSB_container").width(MyWidthDetails);
        //}
        $(".DetailsGridView").mCustomScrollbar("scrollTo", "first");
        $(".DetailsGridView").mCustomScrollbar("update");
        $('.btnNew,.btnDeleteDetail,.btnExportDetails,.btnRefreshDetails,.VerticalSep2').show();
        $(".VerticalSep2").parent("td").prev("td").show();
        $(".VerticalSep2").parent("td").next("td").show();
        $(".btnRefreshDetails").parent("td").next("td").show();
        $(".BackDetail").parent("td").next("td").show();
        $(".btnNew").parent("td").next("td").show();
        $(this).hide();
        $(".NewDetailRecord").hide();
        $(".MainPageDetailTitle").html($(".MainPageDetailTitle").data("text"));

        if ($(".DetailsGridView").find(".GridContainer").data("resizemode") == "fit") setOnCufex_Resize();

        setTimeout(function () { InitColResizable(); }, 300);

        SetMasterResize();
    });

    $(".NewHeaderRecord").resizable({
        handles: "s"
    });

    if ($('.MainPageTitle').attr("data-id") == "Warehouse_PO" || $('.MainPageTitle').attr("data-id") == "Warehouse_ASN" || $('.MainPageTitle').attr("data-id") == "Warehouse_SO" || $('.MainPageTitle').attr("data-id") == "Warehouse_OrderManagement") {
        $(".NewHeaderRecord").find('.InputFacility').on('change', function () {
            var MyDropDown = $(".NewDetailRecord").find(".InputDetailsSku");
            var MyFacility = $(this).val().toString();
            MyFacility = MyFacility.substring(MyFacility.lastIndexOf(",") + 1);
            var MyOwner = $(".NewHeaderRecord").find('.InputStorerKey').val().toString();
            MyOwner = MyOwner.substring(MyOwner.lastIndexOf(",") + 1);
            GetSkuDropDown(MyDropDown, MyFacility, MyOwner, "");
        });
        $(".NewHeaderRecord").find('.InputStorerKey').on('change', function () {
            var MyDropDown = $(".NewDetailRecord").find(".InputDetailsSku");
            var MyFacility = $(".NewHeaderRecord").find('.InputFacility').val().toString();
            MyFacility = MyFacility.substring(MyFacility.lastIndexOf(",") + 1);
            var MyOwner = $(this).val().toString();
            MyOwner = MyOwner.substring(MyOwner.lastIndexOf(",") + 1);
            GetSkuDropDown(MyDropDown, MyFacility, MyOwner, "");
        });
        $(".RecordHeader").find('.InputFacility').on('change', function () {
            var MyDropDown = $(".RecordsContainer_Inside").find(".InputDetailsSku");
            var MyFacility = $(this).val().toString();
            MyFacility = MyFacility.substring(MyFacility.lastIndexOf(",") + 1);
            var MyOwner = $(".RecordHeader").find('.InputStorerKey').val().toString();
            MyOwner = MyOwner.substring(MyOwner.lastIndexOf(",") + 1);
            GetSkuDropDown(MyDropDown, MyFacility, MyOwner, "");
        });
        $(".RecordHeader").find('.InputStorerKey').on('change', function () {
            var MyDropDown = $(".RecordsContainer_Inside").find(".InputDetailsSku");
            var MyFacility = $(".RecordHeader").find('.InputFacility').val().toString();
            MyFacility = MyFacility.substring(MyFacility.lastIndexOf(",") + 1);
            var MyOwner = $(this).val().toString();
            MyOwner = MyOwner.substring(MyOwner.lastIndexOf(",") + 1);
            GetSkuDropDown(MyDropDown, MyFacility, MyOwner, "");
        });
    }

    $(".NewDetailRecord").find('.InputAutoPostBackDetails').on('change', function () {
        var myValue = $(this).val().toString();
        AutoPostBackDetails($(".NewDetailRecord"), myValue.substring(myValue.lastIndexOf(",") + 1));
    });

    $('.CloseACPopup').click(function () {
        $('.Adjust_Columns_PopUp').fadeOut();
    });

    if ($('.MainPageTitle').length > 0) {
        if ($('.MainPageTitle').attr("data-id") == "PROFILEDETAIL") {
            LoadItems();
        }
        DisplayDropDowns();
    }

    if (!$(".GridContainer").hasClass("GridPopup") && $(".HiddenID").val() == 0 && $(".HiddenID").length > 0) GetUserConfiguration();

    $('.InputAutoPostBack').on('change', function () {
        var myValue = $(this).val().toString();
        AutoPostBack(myValue.substring(myValue.lastIndexOf(",") + 1));
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
        $(".NewRecord").find('.InputDetailsSku').on('change', function () {
            if ($('.NewDetailRecord:visible').length > 0) {
                var val = $(this).val().toString();
                val = val.substring(val.lastIndexOf(",") + 1);
                var pack = $(this).find("option[value='" + val + "']").data("pack");
                var packdesc = $(this).find("option[value='" + val + "']").data("packdescr");
                var PackDropDown = $(".NewRecord").find(".InputDetailsPackKey");
                if (PackDropDown.find("option[value='" + pack + "']").length == 0) {
                    PackDropDown.append("<option value='" + pack + "' data-value1=" + packdesc + ">" + pack + "</option>");
                }
                PackDropDown.val(pack).trigger("chosen:updated");
                AutoPostBackDetails($(".NewDetailRecord"), pack);
            }
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
        var index = $(".RecordsContainer_Inside:visible").length;
        $('.RecordsContainer_Inside').eq(0).clone().insertAfter($('.RecordsContainer_Inside').eq(index)).slideDown();

        setOnCufex_Resize();
        $('.MyContainerPopup').mCustomScrollbar("scrollTo", $(".RecordsContainer_Inside").eq(index + 1));
        $('.RecordsContainer_Inside').eq(index + 1).find('.chosen-container').remove();
        $('.RecordsContainer_Inside').eq(index + 1).find('.chosen-select').chosen();
        $('.RecordsContainer_Inside').eq(index + 1).find('.chosen-container').css("width", "100%");
        $('.RecordsContainer_Inside').eq(index + 1).find('.chosen-select').trigger("chosen:updated");
        $('.RecordsContainer_Inside').eq(index + 1).find('input:text,select').each(function () {
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


        $('.RecordsContainer_Inside').eq(index + 1).find(".InputDetailsToLoc").val("STAGE").trigger("chosen:updated");
        $('.RecordsContainer_Inside').eq(index + 1).find('select').prop('disabled', false).trigger("chosen:updated");
        $('.RecordsContainer_Inside').eq(index + 1).find('.InputDetailsUOM').empty().trigger("chosen:updated");

        $('.btnDeleteDtl').click(function () {
            var $this = $(this);
            $this.parent('.RecordsContainer_Inside').remove();
            setOnCufex_Resize();
        });

        $('.RecordsContainer_Inside').eq(index + 1).find(".datepicker").removeClass('hasDatepicker');
        $('.RecordsContainer_Inside').eq(index + 1).find(".datepicker").datepicker();

        $(".RecordsContainer_Inside").find('.InputAutoPostBackDetails').on('change', function () {
            var myValue = $(this).val().toString();
            AutoPostBackDetails($(this).closest(".RecordsContainer_Inside:visible"), myValue.substring(myValue.lastIndexOf(",") + 1));
        });

        if ($('.MainPageTitle').attr("data-id") == "Warehouse_ASN" || $('.MainPageTitle').attr("data-id") == "Warehouse_SO" || $('.MainPageTitle').attr("data-id") == "Warehouse_OrderManagement") {
            $('.InputDetailsSku').on('change', function () {
                if ($('.RecordsContainer_Inside:visible').length > 0) {
                    var val = $(this).val().toString();
                    val = val.substring(val.lastIndexOf(",") + 1);
                    var pack = $(this).find("option[value='" + val + "']").data("pack");
                    var packdesc = $(this).find("option[value='" + val + "']").data("packdescr");
                    var PackDropDown = $(this).closest(".RecordsContainer_Inside:visible").find(".InputDetailsPackKey");
                    if (PackDropDown.find("option[value='" + pack + "']").length == 0) {
                        PackDropDown.append("<option value='" + pack + "' data-value1=" + packdesc + ">" + pack + "</option>");
                    }
                    PackDropDown.val(pack).trigger("chosen:updated");
                    AutoPostBackDetails($(this).closest(".RecordsContainer_Inside:visible"), pack);
                }
            });
        }

        if ($('.MainPageTitle').attr("data-id") == "Warehouse_OrderManagement") {
            $('.InputStorerKey, .InputConsigneeKey, .InputDetailsSku').on('change', function () {
                if ($('.RecordsContainer_Inside:visible').length > 0) {
                    SetPriceAndCurrency();
                }
            });
        }

        $('.RecordsContainer_Inside:visible').find(".SearchDropDown").click(function (e) {
            var MyClass = $(".NewHeaderRecord:visible").length > 0 ? $(".NewHeaderRecord") : $(".RecordHeader");
            var MyRequiredFields = $(this).data("requiredfields").split(",")
            var MyRequiredFieldsName = $(this).data("requiredfieldsname").split(",")
            var MyField1 = "", MyField2 = ""; MyField1Value = "", MyField2Value = "", MyField1Name = "", MyField2Name = "", ErrorMsg = "";

            if (MyRequiredFields.length > 0) MyField1 = MyRequiredFields[0];
            if (MyRequiredFields.length > 1) MyField2 = MyRequiredFields[1];

            if (MyRequiredFieldsName.length > 0) MyField1Name = MyRequiredFieldsName[0];
            if (MyRequiredFieldsName.length > 1) MyField2Name = MyRequiredFieldsName[1];

            if (MyField1 != "") {
                MyField1Value = MyClass.find(MyField1).val();
                if (MyField1 == ".InputFacility") MyField1Value = MyClass.find(MyField1).find("option[value='" + MyField1Value + "']").attr("data-value1");
            }
            if (MyField2 != "") MyField2Value = MyClass.find(MyField2).val();

            if ((MyField1Value == "" || MyField1Value == null) && MyRequiredFields.length > 0) {
                ErrorMsg += MyField1Name + " must be defined <br/>"
            }

            if ((MyField2Value == "" || MyField2Value == null) && MyRequiredFields.length > 1) {
                ErrorMsg += MyField2Name + " must be defined <br/>"
            }

            if (ErrorMsg != "") {
                swal({
                    title: "Search",
                    text: ErrorMsg.replace(/<br\s*\/?>/gim, "\n"),
                    type: 'error',
                    confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                    showCancelButton: false
                });
            }
            else {
                $(".SearchDropDown").removeClass("Active");
                $(this).addClass("Active");
                var top = $(window).height() - 600;
                top = top > 0 ? top / 2 : 0;

                var left = $(window).width() - 900;
                left = left > 0 ? left / 2 : 0;

                window.open($(this).data("url").replace(MyField1, MyField1Value).replace(MyField2, MyField2Value), "_blank", "width=900,height=600,top=" + top + ",left=" + left);
            }
        });

        SetDropDownSingleSelection();
        SearchDropDown(".InputDetailsSku");
    });

    $('.btnActions').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        $(".ActionHiddenButtons").slideToggle();
    });

    $('.ClosePopup').click(function () {
        $('.RecordsContainer_Inside:visible').remove();
        $('.FloatRecordField').find('input:password').removeClass("Error");
        $(".ErrorIcon").remove();
        $('.New_Modify_Record_PopUp').fadeOut();
        $('.MyRecordID').val(0);
        $(".jscolor").css("background-color", "#FAFAFA");
        setTimeout(function () {
            $(".PortalLogo").attr("src", sAppPath + "images/Cufex_Images/logo.png");
        }, 500);
        if ($(".HiddenDetailLink").length > 0) {
            window.history.pushState('object or string', "0", $(".HiddenDetailLink").val());
        }
    });

    $('.SaveRecordNow').click(function () {
        if ($('.MainPageTitle').attr("data-id") == "PORTALUSERS") {
            if (!$("input[type='password']").hasClass("Error")) {
                SaveItems();
            }
        } else {
            SaveItems();
        }
    });

    $('.btnSave').click(function () {
        if ($('.MainPageTitle').attr("data-id") == "ChangePassword") {
            if (!$("input[type='password']").hasClass("Error")) {
                SaveItems();
            }
        }
        else {
            SaveItemsNew();
        }
    });

    $('.btnExport,.btnExportDetails').click(function () {
        var $MyGrid = $(".HeaderGridView:visible").length > 0 ? $(".HeaderGridView") : $(".DetailsGridView");
        if ($MyGrid.find('.PagingNumbers').html() == "0 of 0") {
            swal({
                title: "Export",
                text: "No Data to Export",
                type: 'error',
                confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                showCancelButton: false
            });
            return;
        }
        if ($(".HeaderGridView:visible").length > 0) ExportItems();
        else ExportItemsDetails();
    });

    $('.btnImport').click(function () {
        if ($(".ImportFileUpload").length > 0) {
            $(".ImportFileUpload").val("");
            $(".ImportFileUpload").click();
        }
    });

    $(".ImportFileUpload").change(function () {
        if ($(this).val() != "") {
            var ErrorMsg = "";
            var fileExactSize = "";

            var regex = /^([a-zA-Z0-9\s_\\.\-:])+(.csv)$/;
            if (!regex.test($(this).val().toLowerCase())) ErrorMsg = "Please upload a valid CSV file";

            if (ErrorMsg == "") {
                var FileName = $(this).val().split('\\').pop();
                var FileScreenName = FileName.substring(0, FileName.indexOf("-"));
                if (FileScreenName != $(".MainPageTitle").data("id")) ErrorMsg = "The imported file is not related to this screen";
            }

            if (ErrorMsg == "") {
                var fileSize = this.files[0].size;
                var fSExt = new Array('Bytes', 'KB', 'MB', 'GB'),
                    i = 0; while (fileSize > 900) { fileSize /= 1024; i++; }
                fileExactSize = (Math.round(fileSize * 100) / 100) + ' ' + fSExt[i];
                var isValidSize = true;
                if (fileExactSize.indexOf("MB") >= 0 || fileExactSize.indexOf("GB") >= 0) {
                    if (fileExactSize.indexOf("MB") >= 0) {
                        if (parseInt(fileExactSize.substring(0, fileExactSize.indexOf(" "))) > parseInt($(".HiddenFileImportLimit").val())) {
                            isValidSize = false;
                        }
                    } else {
                        isValidSize = false;
                    }
                }
                if (!isValidSize) ErrorMsg = "File is too big (" + fileExactSize + "). Max file size: " + $(".HiddenFileImportLimit").val() + " MB";
            }

            if (ErrorMsg != "") {
                swal({
                    title: "Import",
                    text: ErrorMsg,
                    type: 'error',
                    confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                    showCancelButton: false
                });
                return;
            }
            ImportItems(fileExactSize);
        }
    });

    $('.btnRefresh,.btnRefreshDetails').click(function () {
        if ($(".HeaderGridView:visible").length > 0) {
            isFirstLoad = false;
            CurrentPage = 0;
            SortBy = "id desc";

            if ($('#SortBy').length > 0) SortBy = $('#SortBy').val();
            $(".HeaderGridView").find('.SearchClass').val("");
            $(".HeaderGridView").find('.SortUp,.SortDown').removeClass("Active");
            $(".HeaderGridView").find('.Arrow-Left-Back').addClass("Disabled");
            $(".HeaderGridView").find('.Arrow-Left-Back-First').addClass("Disabled");
            $(".HeaderGridView").find('.Arrow-Right-Forward').addClass("Disabled");
            $(".HeaderGridView").find('.Arrow-Right-Forward-Last').addClass("Disabled");
            SearchQuery = "";
            LoadItems();
        }
        else {
            CurrentPageDetails = 0;
            SortByDetails = "id desc";
            if ($('#SortBy').length > 0) SortByDetails = $('#SortBy').val();
            $(".DetailsGridView").find('.SearchClass').val("");
            $(".DetailsGridView").find('.SortUp,.SortDown').removeClass("Active");
            $(".HeaderGridView").find('.Arrow-Left-Back').addClass("Disabled");
            $(".HeaderGridView").find('.Arrow-Left-Back-First').addClass("Disabled");
            $(".HeaderGridView").find('.Arrow-Right-Forward').addClass("Disabled");
            $(".HeaderGridView").find('.Arrow-Right-Forward-Last').addClass("Disabled");
            SearchQueryDetails = "";
            LoadItemsDetails();
        }
    });

    $(".datepicker").datepicker();

    $(".checkRadio").checkboxradio({
        icon: false
    });

    $(".chosen-select").chosen({
        disable_search_threshold: 10,
        width: "100%",
        search_contains: false
    });

    $(".chosen-select").on("chosen:showing_dropdown", function () {
        $(".content_4").mCustomScrollbar("disable");
    });

    $(".chosen-select").on("chosen:hiding_dropdown", function (e) {
        $(".content_4").mCustomScrollbar("update");
    });

    $(".NewHeaderRecord").find(".chosen-select").on("chosen:showing_dropdown", function () {
        $(".NewHeaderRecord").removeClass("u-overflowHidden");
    });

    var isHidden = true;

    $(".GridCell").find(".chosen-select").on("chosen:showing_dropdown", function (e) {
        isHidden = false;
        $(".content_3").mCustomScrollbar("disable")
        $(this).parents("div").css("overflow", "visible");
        $(this).siblings(".chosen-container").find(".search-field").show();
        setTimeout(function () { InitColResizable(); isHidden = true; }, 300);
    });

    $(".GridCell").find(".chosen-select").on("chosen:hiding_dropdown", function (e) {
        if (isHidden) {
            $(".content_3").mCustomScrollbar("update");
            $(this).parents("div").css("overflow", "");
            setTimeout(function () { InitColResizable(); }, 300);
            if ($(this).val() == "") return;
            $(this).siblings(".chosen-container").find(".search-field").hide();
        }
    });

    $(".GridCell").find(".chosen-select").on('change', function () {
        if ($(this).val() == "") $(this).siblings(".chosen-container").find(".search-field").show();
    });

    if ($(".MyTab").length > 0) {
        MoveTabLine(0);
        $('.MyTab').click(function () {
            $('.MyTab').removeClass("Active");
            $(this).addClass("Active");
            MoveTabLine($(this).index());

            if ($('.MainPageTitle').attr("data-id") == "PROFILEDETAIL") {
                $('.GridContainer').addClass("DisplayNone");
                $('.btnDelete').addClass("DisplayNone");
                $('.btnDelete').parent("td").prev("td").hide();
                var TabID = $(this).data("id");
                if (TabID == "Actions") $('.GridActions').removeClass("DisplayNone");
                else if (TabID == "Reports") $('.GridReports').removeClass("DisplayNone");
                else if (TabID == "Dashboards") {
                    $('.GridDashboards').removeClass("DisplayNone");
                    $('.btnDelete').removeClass("DisplayNone");
                    $('.btnDelete').parent("td").prev("td").show();
                    setOnCufex_Resize();
                }
                CurrentPage = 0;
                $(".SearchClass").val("");
                SearchQuery = "";
                SortBy = "id desc";
                LoadItems();
                setTimeout(function () { InitColResizable(); }, 300);
            }
            SetMasterResize();
        });
    }

    if ($(".HiddenID").val() != 0 && $(".HiddenID").length > 0) {
        AvoidWebServiceRaceCondition = 0;
        var MyID = $(".HiddenID").val();
        $(".HiddenID").val(0);
        if ($(".NewRecord").length > 0) DisplayItemNew(MyID, '');
        else DisplayItem(MyID, '');
    } else {
        setTimeout(function () {
            $(".preloader").fadeOut();
        }, 1500);
    }

    $(".textRecordStylePassword").blur(function () {
        $(this).removeClass("Error");
        $(this).siblings(".ErrorIcon").remove();
        if ($(this).val() != "") {
            if (!$(this).val().match(/^(?=.*?[A-Z])(?=(.*[a-z]){1,})(?=(.*[\d]){1,})(?=(.*[\W]){1,})(?!.*\s).{10,}$/)) {
                $(this).addClass("Error");
                $(this).after("<div class='ErrorIcon' title='Password must be at least 10 characters, have one upper case letter, one lower case letter, one special character and one base 10 digits (0 to 9)'></div>");
            }
        }
    });

    $(".InputPortalLogo").change(function () {
        readURL(this);
    });

    function readURL(input) {
        if (input.files && input.files[0]) {
            var file = input.files[0];
            var fileType = file["type"];
            var validImageTypes = ["image/gif", "image/jpeg", "image/png"];
            if ($.inArray(fileType, validImageTypes) < 0) {
                swal({
                    title: "Logo Upload",
                    text: "Please upload a valid image file",
                    type: 'error',
                    confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                    showCancelButton: false
                });
                $(".PortalLogo").attr("src", sAppPath + "images/Cufex_Images/logo.png");
                $(input).val("");
                return;
            }
            var reader = new FileReader();
            reader.onload = function (e) {
                $('.PortalLogo').attr('src', e.target.result);
            }
            reader.readAsDataURL(file);
        }
    }

    $(".NewRecord, .RecordHeader").find(".SearchDropDown").click(function (e) {
        var MyClass = $(".NewHeaderRecord:visible").length > 0 ? $(".NewHeaderRecord") : $(".RecordHeader");
        var MyRequiredFields = $(this).data("requiredfields").split(",")
        var MyRequiredFieldsName = $(this).data("requiredfieldsname").split(",")
        var MyField1 = "", MyField2 = ""; MyField1Value = "", MyField2Value = "", MyField1Name = "", MyField2Name = "", ErrorMsg = "";

        if (MyRequiredFields.length > 0) MyField1 = MyRequiredFields[0];
        if (MyRequiredFields.length > 1) MyField2 = MyRequiredFields[1];

        if (MyRequiredFieldsName.length > 0) MyField1Name = MyRequiredFieldsName[0];
        if (MyRequiredFieldsName.length > 1) MyField2Name = MyRequiredFieldsName[1];

        if (MyField1 != "") {
            MyField1Value = MyClass.find(MyField1).val();
            if (MyField1 == ".InputFacility") MyField1Value = MyClass.find(MyField1).find("option[value='" + MyField1Value + "']").attr("data-value1");
        }
        if (MyField2 != "") MyField2Value = MyClass.find(MyField2).val();

        if ((MyField1Value == "" || MyField1Value == null) && MyRequiredFields.length > 0) {
            ErrorMsg += MyField1Name + " must be defined <br/>"
        }

        if ((MyField2Value == "" || MyField2Value == null) && MyRequiredFields.length > 1) {
            ErrorMsg += MyField2Name + " must be defined <br/>"
        }

        if (ErrorMsg != "") {
            swal({
                title: "Search",
                text: ErrorMsg.replace(/<br\s*\/?>/gim, "\n"),
                type: 'error',
                confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                showCancelButton: false
            });
        }
        else {
            $(".SearchDropDown").removeClass("Active");
            $(this).addClass("Active");
            var top = $(window).height() - 600;
            top = top > 0 ? top / 2 : 0;

            var left = $(window).width() - 900;
            left = left > 0 ? left / 2 : 0;

            window.open($(this).data("url").replace(MyField1, MyField1Value).replace(MyField2, MyField2Value), "_blank", "width=900,height=600,top=" + top + ",left=" + left);
        }
    });

    SetDropDownSingleSelection();
}

function SetDropDownSingleSelection() {
    var prevValue = 0, val = 0;

    $(".chosen-select[data-mode='single']").change(function () {
        var val2 = $(this).siblings(".chosen-container").find(".search-choice").find("span").html().toUpperCase();
        val = $(this).val();
        if (val.length > 1) {
            if (val.toString().split(",")[1] == val2) {
                $(this).val(val[0]).trigger("chosen:updated");
                return;
            }
        }
        (val && val.length > 1) && $(this).val([prevValue = val[1] !== prevValue ? val[1] : val[0]]).trigger("chosen:updated");
    });
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
    var PageFirstRowNo = 1 + MyPage * MyNumberOfRecordsInPage;
    var PageLastRowNo = (MyPage + 1) * MyNumberOfRecordsInPage > TableSize ? TableSize : (MyPage + 1) * MyNumberOfRecordsInPage
    $("." + $this).find('.PagingNumbers').html(PageFirstRowNo + "-" + PageLastRowNo + " of " + TableSize);
    if (TableSize > MyNumberOfRecordsInPage) {
        $("." + $this).find('.GridResults').hide().slice(MyPage * MyNumberOfRecordsInPage, (MyPage + 1) * MyNumberOfRecordsInPage).show();
    }
}

function PageTableTab($this) {
    var TableSize = $("." + $this).find(".GridResults").size();
    var PageFirstRowNo = 1 + (CurrentPage * NumberOfRecordsInPage);
    var PageLastRowNo = (CurrentPage + 1) * NumberOfRecordsInPage > TableSize ? TableSize : (CurrentPage + 1) * NumberOfRecordsInPage
    $('.PagingNumbers').html(PageFirstRowNo + "-" + PageLastRowNo + " of " + TableSize);
    if (TableSize > NumberOfRecordsInPage) {
        $("." + $this).find('.GridResults').hide().slice(CurrentPage * NumberOfRecordsInPage, (CurrentPage + 1) * NumberOfRecordsInPage).show();
    }
}

function SetGridActions() {
    function SortTable(n, dir) {
        var table, rows, switching, i, x, y, shouldSwitch, switchcount = 0;
        var GridIndex = $(".HeaderGridView:visible").length > 0 ? 0 : 1;
        var GridName = "GridContainer";
        if ($(".MyTab").length > 0) GridName = "Grid" + $(".MyTab.Active").data("id");
        table = document.getElementsByClassName(GridName)[GridIndex];
        switching = true;

        while (switching) {
            switching = false;
            rows = table.rows;
            for (i = 2; i < rows.length - 1; i++) {
                shouldSwitch = false;
                x = rows[i].getElementsByClassName("GridCell")[n];
                y = rows[i + 1].getElementsByClassName("GridCell")[n];

                var xhtml = $.trim(x.innerHTML);
                var yhtml = $.trim(y.innerHTML);

                if (xhtml.indexOf("<a") >= 0 && xhtml.indexOf("</a>") >= 0) {
                    xhtml = $.trim(xhtml.split(">")[1].split("</a")[0]);
                }

                if (yhtml.indexOf("<a") >= 0 && yhtml.indexOf("</a>") >= 0) {
                    yhtml = $.trim(yhtml.split(">")[1].split("</a")[0]);
                }

                var z = $.trim(xhtml.split("<br>")[0]);
                var z1 = $.trim(xhtml.replace("<br>", ""));
                var t = $.trim(yhtml.split("<br>")[0]);
                var t1 = $.trim(yhtml.replace("<br>", ""));

                if (dir == "asc") {
                    if (isDate(z) && isDate(t)) {
                        if (new Date(z1) > new Date(t1)) {
                            shouldSwitch = true;
                            break;
                        }
                    }
                    else {
                        if (xhtml.toLowerCase() > yhtml.toLowerCase()) {
                            shouldSwitch = true;
                            break;
                        }
                    }
                } else if (dir == "desc") {
                    if (isDate(z) && isDate(t)) {
                        if (new Date(z1) < new Date(t1)) {
                            shouldSwitch = true;
                            break;
                        }
                    }
                    else {
                        if (xhtml.toLowerCase() < yhtml.toLowerCase()) {
                            shouldSwitch = true;
                            break;
                        }
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

    function isDate(val) {
        if (!val.match(/^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)dd$/)) return false;
        var d = new Date(val);
        return !isNaN(d.valueOf());
    }

    $(".GridContainer").on('click', '.GridHead[data-id]', function (e) {
        var dir;
        var $MyGrid = $(".HeaderGridView:visible").length > 0 ? $(".HeaderGridView") : $(".DetailsGridView");
        var GridName = $(".HeaderGridView:visible").length > 0 ? "HeaderGridView" : "DetailsGridView";

        if ($("MyTab").length > 0) {
            $MyGrid = $(".Grid" + $(".MyTab.Active").data("id"));
            GridName = "Grid" + $(".MyTab.Active").data("id");
        }

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
            if ($("MyTab").length > 0) PageTableTab(GridName);
            else PageTable(GridName);
            $('.Arrow-Left-Back-First').trigger("click");
        }
    });

    $(".GridContainer").on('click', '.SortUp', function (e) {
        e.stopPropagation();
        var $MyGrid = $(".HeaderGridView:visible").length > 0 ? $(".HeaderGridView") : $(".DetailsGridView");
        var GridName = $(".HeaderGridView:visible").length > 0 ? "HeaderGridView" : "DetailsGridView";

        if ($("MyTab").length > 0) {
            $MyGrid = $(".Grid" + $(".MyTab.Active").data("id"));
            GridName = "Grid" + $(".MyTab.Active").data("id");
        }

        $MyGrid.find(".SortUp,.SortDown").removeClass("Active");
        $(this).addClass("Active");

        if ($(".HeaderGridView:visible").length > 0) SortBy = $(this).parent().parent().attr("data-id") + " asc";
        else SortByDetails = $(this).parent().parent().attr("data-id") + " asc";

        if ($(".GridResults").length > 0) {
            SortTable($(this).closest(".GridCell").index(), "asc");
            if ($("MyTab").length > 0) PageTableTab(GridName);
            else PageTable(GridName);
            $('.Arrow-Left-Back-First').trigger("click");
        }

    });

    $(".GridContainer").on('click', '.SortDown', function (e) {
        e.stopPropagation();
        var $MyGrid = $(".HeaderGridView:visible").length > 0 ? $(".HeaderGridView") : $(".DetailsGridView");
        var GridName = $(".HeaderGridView:visible").length > 0 ? "HeaderGridView" : "DetailsGridView";

        if ($("MyTab").length > 0) {
            $MyGrid = $(".Grid" + $(".MyTab.Active").data("id"));
            GridName = "Grid" + $(".MyTab.Active").data("id");
        }

        $MyGrid.find(".SortUp,.SortDown").removeClass("Active");
        $(this).addClass("Active");

        if ($(".HeaderGridView:visible").length > 0) SortBy = $(this).parent().parent().attr("data-id") + " desc";
        else SortByDetails = $(this).parent().parent().attr("data-id") + " desc";

        if ($(".GridResults").length > 0) {
            SortTable($(this).closest(".GridCell").index(), "desc");
            if ($("MyTab").length > 0) PageTableTab(GridName);
            else PageTable(GridName);
            $('.Arrow-Left-Back-First').trigger("click");
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

        setOnCufex_Resize();
        InitColResizable();
        setTimeout(function () {
            SetUserConfigution();
            setTimeout(function () {
                $(".GridCell").find(".chosen-container").remove();

                $(".GridCell").find(".chosen-select").chosen({
                    disable_search_threshold: 10,
                    width: "100%",
                    search_contains: false
                });

                var isHidden = true;

                $(".GridCell").find(".chosen-select").on("chosen:showing_dropdown", function (e) {
                    isHidden = false;
                    $(".content_3").mCustomScrollbar("disable")
                    $(this).parents("div").css("overflow", "visible");
                    $(this).siblings(".chosen-container").find(".search-field").show();
                    setTimeout(function () { InitColResizable(); isHidden = true; }, 300);
                });

                $(".GridCell").find(".chosen-select").on("chosen:hiding_dropdown", function (e) {
                    if (isHidden) {
                        $(".content_3").mCustomScrollbar("update");
                        $(this).parents("div").css("overflow", "");
                        setTimeout(function () { InitColResizable(); }, 300);
                        if ($(this).val() == "") return;
                        $(this).siblings(".chosen-container").find(".search-field").hide();
                    }
                });

                $(".GridCell").find(".chosen-select").on('change', function () {
                    if ($(this).val() == "") $(this).siblings(".chosen-container").find(".search-field").show();
                });

                $("body").on('keydown', '.GridCell .chosen-search-input', function (e) {
                    if (e.which === 13 && $(this).closest('.chosen-container').find('.chosen-single-with-drop').length === 0) {
                        setTimeout(function () {
                            $(".GridSearch").trigger("click");
                        }, 300);
                    }
                });
            }, 1000);
        }, 500);
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
            $(".HeaderGridView").find('.chosen-select').val("").trigger("chosen:updated");
            SearchQuery = "";
        }
        else {
            $(".DetailsGridView").find('.SearchClass').val("");
            $(".DetailsGridView").find('.chosen-select').val("").trigger("chosen:updated");
            SearchQueryDetails = "";
        }
    });

    $(".GridContainer").on('click', '.GridSearch', function () {
        var SearchCount;
        if ($(".HeaderGridView:visible").length > 0) {
            SearchQuery = "";
            $(".HeaderGridView").find('.SearchClass').each(function () {
                if ($(this).val() != "") {
                    SearchQuery = SearchQuery + (SearchQuery != "" ? "," : "") + $(this).attr("data-id") + ":" + $(this).val().toString().replace(",", ";");
                }
            });
            $('.Arrow-Left-Back-First').trigger("click");
            isFirstLoad = false;
            if ($(".HeaderGridView").find(".GridContainer").hasClass("GridPopup")) {
                LoadItemsPopup();
            }
            else {
                LoadItems();
            }
        }
        else {
            SearchQueryDetails = "";
            $(".DetailsGridView").find('.SearchClass').each(function () {
                if ($(this).val() != "") {
                    SearchQueryDetails = SearchQueryDetails + (SearchQueryDetails != "" ? "," : "") + $(this).attr("data-id") + ":" + $(this).val().toString().replace(",", ";");
                }
            });
            $('.Arrow-Left-Back-First').trigger("click");
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
                            SearchQuery = $(this).attr("data-id") + ":" + $(this).val().toString().replace(",", ";");
                        }
                    }
                    else {
                        if ($(this).val() != "") {
                            SearchQuery = SearchQuery + "," + $(this).attr("data-id") + ":" + $(this).val().toString().replace(",", ";");
                        }
                    }
                });
                $('.Arrow-Left-Back-First').trigger("click");
                isFirstLoad = false;
                if ($(".HeaderGridView").find(".GridContainer").hasClass("GridPopup")) {
                    LoadItemsPopup();
                }
                else {
                    LoadItems();
                }
            }
            else {
                $(".DetailsGridView").find('.SearchClass').each(function () {
                    if (SearchQueryDetails == "") {
                        if ($(this).val() != "") {
                            SearchQueryDetails = $(this).attr("data-id") + ":" + $(this).val().toString().replace(",", ";");
                        }
                    }
                    else {
                        if ($(this).val() != "") {
                            SearchQueryDetails = SearchQueryDetails + "," + $(this).attr("data-id") + ":" + $(this).val().toString().replace(",", ";");
                        }
                    }
                });
                $('.Arrow-Left-Back-First').trigger("click");
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
            if ($("MyTab").length > 0) PageTableTab("Grid" + $(".MyTab.Active").data("id"));
            else PageTable("HeaderGridView");
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
            if ($("MyTab").length > 0) PageTableTab("Grid" + $(".MyTab.Active").data("id"));
            else PageTable("HeaderGridView");
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
            if ($("MyTab").length > 0) PageTableTab("Grid" + $(".MyTab.Active").data("id"));
            else PageTable("HeaderGridView");
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
            if ($("MyTab").length > 0) PageTableTab("Grid" + $(".MyTab.Active").data("id"));
            else PageTable("HeaderGridView");
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

    $("body").on('keydown', '.GridCell .chosen-search-input', function (e) {
        if (e.which === 13 && $(this).closest('.chosen-container').find('.chosen-single-with-drop').length === 0) {
            setTimeout(function () {
                $(".GridSearch").trigger("click");
            }, 300);
        }
    });
}

function SetDropZone() {
    if ($(".dropzone").length > 0) {
        myDropzone = new Dropzone(".dropzone", {
            url: sAppPath + "WebServices/FileUpload.ashx",
            addRemoveLinks: $(".HiddenCanRemoveOwnFiles").val() == 0 && $(".HiddenCanRemoveOwnFiles").val() == 0 ? false : true,
            maxFilesize: $(".HiddenFileUploadLimit").val(),
            autoProcessQueue: false,
            success: function (file, response) {
                file.previewElement.classList.add("dz-success");
            },
            error: function (file, response) {
                file.previewElement.classList.add("dz-error");
            },
            removedfile: function (file) {
                if (file.status != "queued" && file.status != "canceled" && file.status != "error" && file.status != "added" && file.status != "success") {
                    var success = UploadedFileAction(file.name, file.size, "delete");
                    if (success) file.previewElement.remove();
                } else {
                    file.previewElement.remove();
                }
                if ($(".dz-preview").length == 0) $(".dz-default").show();
            },
            init: function () {
                this.on('addedfile', function (file) {
                    if ($(".HiddenCanUploadFiles").val() == 0) {
                        swal({
                            title: "File Upload",
                            text: "You do not have permission to upload files",
                            type: 'error',
                            confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                            showCancelButton: false
                        });
                        this.removeFile(file);
                    }
                    else {
                        $(".dz-default").hide();
                    }
                });
                this.on("error", function (file, message) {
                    if (file.status != "canceled") {
                        swal({
                            title: "File Upload",
                            text: message,
                            type: 'error',
                            confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                            showCancelButton: false
                        });
                        this.removeFile(file);
                    }
                });
            }
        });
    }
}

function SetUITemplate() {
    if ($(".HiddenLoginBackgroundColor").val() != "") {
        $(".CufexBG").css({
            "background-image": "none",
            "background-color": $(".HiddenLoginBackgroundColor").val()
        });
    }

    if ($(".HiddenScreenBackgroundColor").val() != "") {
        $(".NormalDiv1118Max,.MainTabs,.GiveMeHeight,.MyAbso_Record_PopUpContainer,.RecordsContainer_Inside,.MyAbso_Adjust_Columns_PopUpContainer").css({ "background-color": $(".HiddenScreenBackgroundColor").val() });
    }

    if ($(".HiddenButtonBackgroundColor").val() != "") {
        $(".Arrow-Left-Back,.Arrow-Left-Back-First,.Arrow-Right-Forward,.Arrow-Right-Forward-Last,.SortUp,.SortDown,.btnExport,.btnImport,.btnRefresh,.btnExportDetails,.btnRefreshDetails,.BackBtn,.SaveRecordNow,.ClosePopup,.btnDeleteDtl,.CloseACPopup").css({ "background-color": $(".HiddenButtonBackgroundColor").val() });
    }

    if ($(".HiddenTextBackgroundColor").val() != "") {
        $(".MenuItemName,.MyTitleHead,.R_PopupTitle,.FloatRecordTitleNew,.Details_FloatRecordTitleNew").css({ "background-color": $(".HiddenTextBackgroundColor").val() });
        $(".MenuSubItem").find("a").css({ "background-color": $(".HiddenTextBackgroundColor").val() });
        $(".MainPageTitle").parent("div").css({ "background-color": $(".HiddenTextBackgroundColor").val() });

        $(".FloatRecordTitle,.Details_FloatRecordTitle,.GridContentCell").each(function () {
            var DivHTML = $(this).html();
            $(this).html("<span style='background-color:" + $(".HiddenTextBackgroundColor").val() + "'>" + DivHTML + "</span>");
        });

        $(".NoResults").find(".GridContentCell[data-id=1]").each(function () {
            var DivHTML = $(this).html();
            $(this).html("<span style='background-color:" + $(".HiddenTextBackgroundColor").val() + "'>" + DivHTML + "</span>");
        });
    }

    if ($(".HiddenGridBackgroundColor").val() != "") {
        $(".GridRow").css({ "background-color": $(".HiddenGridBackgroundColor").val() });
    }
}

function SetCarrierEvents() {
    $('.CarrierEventsBtn').unbind('click').click(function () {
        if (!$(".CarrierEventsBtn").hasClass("Inactive")) {
            if (NtorKermelElraceCondition == 0) {
                disable_scroll();
                NtorKermelElraceCondition = 1;
                $(".CarrierEventsMenuContainer").addClass("Active");
                $(".CarrierEventsMenu").css({ "left": $(window).width() - $(".CarriersDiv").width() });
                $(".CarriersDiv").height($(".CarrierEventsMenuContainer").height() - $(".CarrierEventsHeader").outerHeight() - 30);
                setTimeout(function () {
                    NtorKermelElraceCondition = 0;
                }, 300);
            }
        }
    });

    $('.CloseCarrierEvents').unbind('click').click(function () {
        if (NtorKermelElraceCondition == 0) {
            NtorKermelElraceCondition = 1;
            $(".CarrierEventsMenuContainer").removeClass("Active");
            $(".CarrierEventsMenu").css({ "left": "100%" });
            $(".CarriersDiv").height(0);
            $(".CarrierEventInfo").hide();
            $('.CarrierDiv').removeClass("Active");
            $(this).addClass("Active");
            $(".CarrierIndicator").css({ "top": 0, "height": $(".CarrierDiv").eq(0).outerHeight() });
            setTimeout(function () {
                enable_scroll();
                NtorKermelElraceCondition = 0;
            }, 300);
        }
    });
}

function DeleteItems(MyItems) {
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;
        console.log("Delete Items API Start Time: " + GetTime());

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
            console.log("Couldn't Delete Your Record");
        }
        if (success) {
            AvoidWebServiceRaceCondition = 0;
            $(".BackHeader").trigger("click");
            $(".HeaderGridView").mCustomScrollbar("scrollTo", "first");
            $(".HeaderGridView").mCustomScrollbar("update");
            isFirstLoad = true;
            LoadItems();
        }
        console.log("Delete Items API End Time: " + GetTime());
    }

    function OnLoadError(response) {
        console.log(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function DeleteItemsDetails(MyItems) {
    if (AvoidWebServiceRaceCondition == 0) {
        console.log("Delete Items Details API Start Time: " + GetTime());
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
            console.log("Couldn't Delete Your Record");
        }
        if (success) {
            AvoidWebServiceRaceCondition = 0;
            isFirstLoad = true;
            LoadItemsDetails();
        }
        console.log("Delete Items Details API End Time: " + GetTime());
    }

    function OnLoadError(response) {
        console.log(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function UploadedFileAction(FileName, FileSize, ActivityType) {
    var success = true;
    AvoidWebServiceRaceCondition = 0;
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;

        var MyKey = "";
        if ($('.MainPageTitle').attr("data-id") == "Warehouse_PO") MyKey = $(".NewHeaderRecord").find(".InputPOKey").val();
        else if ($('.MainPageTitle').attr("data-id") == "Warehouse_ASN") MyKey = $(".NewHeaderRecord").find(".InputReceiptKey").val();
        else if ($('.MainPageTitle').attr("data-id") == "Warehouse_SO") MyKey = $(".NewHeaderRecord").find(".InputOrderKey").val();
        else if ($('.MainPageTitle').attr("data-id") == "Warehouse_OrderManagement") MyKey = $(".NewHeaderRecord").find(".InputOrderManagKey").val();

        $.ajax({
            url: sAppPath + "WebServices/FileUpload.ashx",
            type: "POST",
            data: {
                "Facility": $(".NewHeaderRecord").find(".InputFacility ").siblings(".chosen-container").find(".search-choice").find("span").html(),
                "Key": MyKey,
                "SearchTable": $('.MainPageTitle').attr("data-id"),
                "FileName": FileName,
                "FileSize": FileSize,
                "ActivityType": ActivityType,
            },
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }

    function OnLoadSuccess(response) {
        var responseStr = response.split("~~~");
        var error = responseStr[0];
        if (error != "") {
            success = false;
            swal({
                title: "Delete File",
                text: error,
                type: 'error',
                confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                showCancelButton: false
            });
        }
        setTimeout(function () {
            AvoidWebServiceRaceCondition = 0;
        }, 300);
    }

    function OnLoadError(response) {
        success = false;
        console.log(response.error);
        setTimeout(function () {
            AvoidWebServiceRaceCondition = 0;
        }, 300);
    }
    return success;
}

function SaveItems() {
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;
        console.log("Save Item API Start Time: " + GetTime());
        $(".preloader").fadeIn();

        var pageUrl = sAppPath + 'WebServices/SaveItems.ashx';

        var data = new FormData();
        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
        data.append("MyID", $('.MyRecordID').val());
        data.append("DetailsCount", $('.RecordsContainer_Inside:visible').length);
        if ($(".InputPortalLogo").length > 0) data.append("LogoFile", $(".InputPortalLogo").get(0).files[0]);

        if ($('.MainPageTitle').attr("data-id") != "ChangePassword") {
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
        } else {
            data.append("Field_OriginalPassword", $('.NewHeaderRecord').find('.InputOriginalPassword').val());
            data.append("Field_NewPassword", $('.NewHeaderRecord').find('.InputNewPassword').val());
            data.append("Field_ConfirmPassword", $('.NewHeaderRecord').find('.InputConfirmPassword').val());
        }

        $('.MyDetailsFields').each(function () {
            var myField = $(this).val();
            var myFieldValue = "";
            $('.RecordsContainer_Inside:visible').each(function (e) {
                if (e == 0) {
                    myFieldValue += $(this).find('.InputDetails' + myField).val();
                }
                else {
                    myFieldValue += "~~~" + $(this).find('.InputDetails' + myField).val();
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
            console.log("Couldn't Save Your Record");
        }
        if (success) {
            AvoidWebServiceRaceCondition = 0;
            isFirstLoad = true;
            if (obj.url !== undefined) location.href = obj.url;
            else LoadItems();
        }
        console.log("Save Item API End Time: " + GetTime());
    }

    function OnLoadError(response) {
        console.log(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function SaveItemsNew() {
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;
        console.log("Save Item New API Start Time: " + GetTime());
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
            console.log("Couldn't Save Your Record");
        }
        if (success) {
            AvoidWebServiceRaceCondition = 0;
            if ($(".dropzone").length > 0) {
                myDropzone.options.parallelUploads = $(".dz-preview").length;
                myDropzone.options.params = {
                    Facility: obj.Facility,
                    Key: obj.key,
                    SearchTable: $('.MainPageTitle').attr("data-id"),
                    ActivityType: "upload"
                }
                myDropzone.processQueue();
            }
            //$('.BackHeader').trigger("click");
            $(".HeaderGridView").mCustomScrollbar("scrollTo", "first");
            $(".HeaderGridView").mCustomScrollbar("update");
            //isFirstLoad = true;
            //LoadItems();
            if ($(".MyRecordID").val() == 0) {
                DisplayItemNew(obj.serialkey, obj.queryurl);
            }
            else {
                if ($(".DetailsGridView:visible").length == 0 && $(".MyDetailRecordID").length > 0) {
                    SaveItemsDetails(obj.serialkey, obj.queryurl);
                }
                else {
                    DisplayItemNew(obj.serialkey, obj.queryurl);
                }
            }
        }
        console.log("Save Item New API End Time: " + GetTime());
    }

    function OnLoadError(response) {
        console.log(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function SaveItemsDetails(DisplayID, QueryURL) {
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;
        console.log("Save Item Details API Start Time: " + GetTime());
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
            console.log("Couldn't Save Your Record");
        }
        if (success) {
            AvoidWebServiceRaceCondition = 0;
            $('.BackDetail').trigger("click");
            $(".DetailsGridView").mCustomScrollbar("scrollTo", "first");
            $(".DetailsGridView").mCustomScrollbar("update");
            DisplayItemNew(DisplayID, QueryURL);
            //LoadItemsDetails();
            $(".NewHeaderRecord").find(".InputStatus").val(obj.Status);
        }
        console.log("Save Item Details API End Time: " + GetTime());
    }

    function OnLoadError(response) {
        console.log(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function DisplayItem(DisplayID, QueryURL) {
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;

        console.log("Display Item API Start Time: " + GetTime());

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
                        else if (myclass.attr("type") == "file") {
                            if (myvalue != "") {
                                $(".PortalLogo").attr("src", sAppPath + "DynamicImages/LogosImages/" + myvalue);
                            }
                        }
                        else if ($(myclass).attr("type") == "checkbox") {
                            myclass.prop("checked", myvalue);
                        }
                        else if ($(myclass).attr("type") == "password") {
                            myclass.attr("type", "text");
                            myclass.attr("value", myvalue);
                            setTimeout(function () {
                                myclass.attr("type", "password");
                            }, 100);
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
                                var MyValueArr = myvalue.split(',');
                                $.each(MyValueArr, function (i) {
                                    if (myclass.find("option[value='" + MyValueArr[i] + "']").length == 0) {
                                        myclass.append("<option value='" + MyValueArr[i] + "'>" + MyValueArr[i] + "</option>");
                                    }
                                });
                                myclass.val(myvalue.split(',')).trigger("chosen:updated");
                                clearInterval(myInterval);
                            }
                        }, 100);
                    }
                });

                if (obj.ReadOnlyFields != "") {
                    var MyReadOnlyValues = obj.ReadOnlyFields;
                    var MyReadOnlyItems = MyReadOnlyValues.split("~~~");
                    $.each(MyReadOnlyItems, function (i) {
                        var myclass = "";
                        var MyReadOnlyValues = MyReadOnlyItems[i];
                        if (MyReadOnlyValues.length > 0) { myclass = $(".RecordHeader").find('.Input' + $.trim(MyReadOnlyValues)); }
                        if (myclass.is("input")) { myclass.prop("disabled", true); }
                        else if (myclass.is("select")) { myclass.prop('disabled', true).trigger("chosen:updated"); }
                    });
                }

                if ($(".jscolor").length > 0) {
                    $('.jscolor').each(function () {
                        $(this).css({ "background-color": $(this).val() })
                    });
                }

                $('.AddDetailsBtn').hide();
                $(".ErrorIcon").remove();
                $('.MyRecordID').val(DisplayID);
                $('.FloatRecordField').find('input:password').val('');


                $('.New_Modify_Record_PopUp').fadeIn(function () {
                    SetMasterResize();
                    $('.AddDetailsBtn').show();
                    $('.FloatRecordField').find('input:password').removeClass("Error");
                    $('.btnDeleteDtl').click(function () {
                        var $this = $(this);
                        $this.parent('.RecordsContainer_Inside').remove();
                        setOnCufex_Resize();
                    });
                });

                if (obj.DetailsCount > 0) {
                    setTimeout(function () {
                        var MyDropDown = $(".RecordsContainer_Inside").find(".InputDetailsSku");
                        var MyFacility = $(".RecordHeader").find('.InputFacility').val().toString();
                        MyFacility = MyFacility.substring(MyFacility.lastIndexOf(",") + 1);
                        var MyOwner = $(".RecordHeader").find('.InputStorerKey').val().toString();
                        MyOwner = MyOwner.substring(MyOwner.lastIndexOf(",") + 1);
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
                                                var MyValueArr = myvalue[e].split(',');
                                                $.each(MyValueArr, function (i) {
                                                    if (myclass.eq(e).find("option[value='" + MyValueArr[i] + "']").length == 0) {
                                                        myclass.eq(e).append("<option value='" + MyValueArr[i] + "'>" + MyValueArr[i] + "</option>");
                                                    }
                                                });
                                                myclass.eq(e).val(myvalue[e].split(',')).trigger("chosen:updated");
                                                clearInterval(myInterval);
                                            }
                                        }, 3000);
                                    }
                                    else {
                                        var MyValueArr = myvalue[e].split(',');
                                        $.each(MyValueArr, function (i) {
                                            if (myclass.eq(e).find("option[value='" + MyValueArr[i] + "']").length == 0) {
                                                myclass.eq(e).append("<option value='" + MyValueArr[i] + "'>" + MyValueArr[i] + "</option>");
                                            }
                                        });
                                        myclass.eq(e).val(myvalue[e].split(',')).trigger("chosen:updated");
                                    }
                                }
                            }
                        });
                    });

                    if (obj.ReadOnlyDetailsFields != "") {
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
                    }

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
            console.log("This record does not exist");
        }

        if (success) {
            $('.preloader').fadeOut(300, function () {
                AvoidWebServiceRaceCondition = 0;
            });
        }

        console.log("Display Item API End Time: " + GetTime());
    }

    function OnLoadError(response) {
        console.log(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function DisplayItemNew(DisplayID, QueryURL) {
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;
        console.log("Display Item New API Start Time: " + GetTime());
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
                $(".CarrierEventsBtn").hide();
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
                                var MyValueArr = myvalue.split(',');
                                $.each(MyValueArr, function (i) {
                                    if (myclass.find("option[value='" + MyValueArr[i] + "']").length == 0) {
                                        myclass.append("<option value='" + MyValueArr[i] + "'>" + MyValueArr[i] + "</option>");
                                    }
                                });
                                myclass.val(myvalue.split(',')).trigger("chosen:updated");
                                clearInterval(myInterval);
                            }
                        }, 100);
                    }
                });

                if (obj.ReadOnlyFields != "") {
                    var MyReadOnlyValues = obj.ReadOnlyFields;
                    var MyReadOnlyItems = MyReadOnlyValues.split("~~~");
                    $.each(MyReadOnlyItems, function (i) {
                        var myclass = "";
                        var MyReadOnlyValues = MyReadOnlyItems[i];
                        if (MyReadOnlyValues.length > 0) { myclass = $(".NewHeaderRecord").find(".Input" + MyReadOnlyValues.trim()); }
                        if (myclass.is("input:text")) { myclass.prop("disabled", true); }
                        else if (myclass.is("select")) { myclass.prop('disabled', true).trigger("chosen:updated"); }
                    });
                }

                if (obj.SavedFiles != "") {
                    if ($(".dropzone").length > 0) $(".dz-preview").remove();
                    var MyFiles = obj.SavedFiles.split(";;;");
                    $.each(MyFiles, function (i) {
                        var myFileSize = "";
                        var myFile = "";
                        var myFileExtension = "";
                        var isImage = false;
                        var ItsMyFile = true;

                        var MyFilesValues = MyFiles[i].split(":::");
                        if (MyFilesValues.length > 0) {
                            myFile = MyFilesValues[0].trim();
                            myFileExtension = myFile.substring(myFile.toLowerCase().lastIndexOf(".") + 1);
                            isImage = myFileExtension == "png" || myFileExtension == "tiff" || myFileExtension == "jpg" || myFileExtension == "jpeg" || myFileExtension == "gif";
                        }
                        if (MyFilesValues.length > 1) { myFileSize = MyFilesValues[1]; }
                        if (MyFilesValues.length > 2) { ItsMyFile = MyFilesValues[2] == "0" ? true : false; }

                        var mockFile = { name: myFile, size: myFileSize };
                        myDropzone.options.addedfile.call(myDropzone, mockFile);
                        if (isImage) {
                            myDropzone.options.thumbnail.call(myDropzone, mockFile, sAppPath + "DynamicFiles/FileManagement/" + myFile);
                        }
                        myDropzone.emit("complete", mockFile);

                        var a = document.createElement('a');
                        a.setAttribute('href', sAppPath + "DynamicFiles/FileManagement/" + myFile);
                        a.setAttribute('class', "dz-remove download");
                        a.setAttribute("data-filename", myFile);
                        a.setAttribute("data-filesize", myFileSize);
                        if (isImage) a.setAttribute('target', '_blank');
                        a.innerHTML = "View file";
                        $(".dz-preview").eq(i).find(".dz-remove").after(a);
                        $(".dz-default").hide();

                        if ($(".HiddenCanViewOwnFiles").val() == 0 && $(".HiddenCanViewAllFiles").val() == 0) {
                            $(".dz-preview").eq(i).find(".dz-remove download").hide();
                        }
                        else if ($(".HiddenCanViewOwnFiles").val() == 1 && $(".HiddenCanViewAllFiles").val() == 0) {
                            if (!ItsMyFile) $(".dz-preview").eq(i).find(".dz-remove download").hide();
                        }

                        if ($(".HiddenCanRemoveOwnFiles").val() == 0 && $(".HiddenCanRemoveAllFiles").val() == 0) {
                            $(".dz-preview").eq(i).find(".dz-remove").hide();
                        }
                        else if ($(".HiddenCanRemoveOwnFiles").val() == 1 && $(".HiddenCanRemoveAllFiles").val() == 0) {
                            if (!ItsMyFile) $(".dz-preview").eq(i).find(".dz-remove").hide();
                        }
                    });

                    $(".dz-remove.download").click(function () {
                        UploadedFileAction($(this).attr("data-filename"), $(this).attr("data-filesize"), "view");
                    });
                }

                $('.MyRecordID').val(DisplayID);
                $(".ActionsDetails,.DetailsGridView").show();
                $(".NewDetailRecord").hide();

                if ($('.MainPageTitle').attr("data-id") == "Warehouse_OrderTracking") {
                    setTimeout(function () { $(".CarrierEventsBtn").show(); }, 300);
                    if (obj.CarrierEventsNbr == 0) {
                        $(".CarrierEventsBtn").addClass("Inactive");
                        $(".CarrierEventsNbrCircle").hide();
                    }
                    else {
                        $(".CarrierEventsBtn").removeClass("Inactive");
                        $(".CarrierEventsNbrCircle").show();
                        $(".CarrierEventsNbr").html(obj.CarrierEventsNbr);
                        $(".CarriersDivRecords").html(obj.CarrierRecords);
                        $(".CarriersDiv").height($(".CarrierEventsMenuContainer").height() - $(".CarrierEventsHeader").outerHeight() - 30);

                        $('.CarrierDiv').unbind("click").bind("click", function (e) {
                            if (!$(this).hasClass("Active")) {
                                $('.CarrierDiv').removeClass("Active");
                                $(this).addClass("Active");
                                var Height = $(".CarrierDiv").eq(0).outerHeight();
                                var Index = $(this).index();
                                $(".CarrierIndicator").animate({
                                    "top": (Height + 20) * Index,
                                    "height": $(this).outerHeight()
                                }, 300);

                                $(".CarrierEventsMenu").css({ "left": $(window).width() - $(".CarriersDiv").width() - $(".CarrierEventInfo").outerWidth() });
                                $(".CarrierEventInfo").hide();
                                setTimeout(function () {
                                    $(".CarrierEventInfo").fadeIn();
                                    SetMasterResize();
                                    $(".ArticlesDiv").height($(".CarrierEventsMenuContainer").height() - $(".CarrierEventInfo1").outerHeight() - 30);
                                }, 300);

                                $(".CarrierEventDescription").html("<div class='EventDescriptionBullet Category" + $(this).data("category") + "'></div>" + $(this).data("eventdescription"));
                                $(".CarrierEventDescription").addClass("Category" + $(this).data("category"));
                                $(".CarrierEventDate").html($(this).data("eventdate"));

                                $(".CarrierConsignmentDate").html("Created on: <span>" + $(this).data("consignmentdate") + "</span>")
                                $(".CarrierItems").html("Items: <span>" + $(this).data("items") + "</span>")
                                $(".CarrierCube").html("Cube: <span>" + $(this).data("cube") + "</span>")
                                $(".CarrierWeight").html("Weight: <span>" + $(this).data("weight") + "</span>")

                                GetCarrierDetails("Articles");
                            }
                        });
                    }
                }

                if (QueryURL != "") {
                    var MyLink = $('.HiddenDetailLink').val();
                    window.history.pushState('object or string', DisplayID, MyLink + QueryURL);
                }
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
            console.log("This record does not exist");
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

        console.log("Display Item New API End Time: " + GetTime());
    }

    function OnLoadError(response) {
        console.log(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function DisplayItemDetails(DisplayID) {
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;
        console.log("Display Item Details New API Start Time: " + GetTime());
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
                                var MyValueArr = myvalue.split(',');
                                $.each(MyValueArr, function (i) {
                                    if (myclass.find("option[value='" + MyValueArr[i] + "']").length == 0) {
                                        myclass.append("<option value='" + MyValueArr[i] + "'>" + MyValueArr[i] + "</option>");
                                    }
                                });
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
            console.log("This record does not exist");
        }

        if (success) {
            $('.preloader').fadeOut(300, function () {
                AvoidWebServiceRaceCondition = 0;
            });
        }

        console.log("Display Item Details New API End Time: " + GetTime());
    }

    function OnLoadError(response) {
        console.log(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function LoadItems() {
    if ($('.MainPageTitle').attr("data-id") != "ChangePassword") {
        if (AvoidWebServiceRaceCondition == 0) {
            AvoidWebServiceRaceCondition = 1;
            $(".HeaderGridView").find('.GridResults').remove();
            $(".HeaderGridView").find('.NoResults').hide();
            if (!isFirstLoad) $('.preloader').fadeIn();
            isFirstLoad = false;

            var pageUrl = sAppPath + 'WebServices/GetItems.ashx';

            var TabName = "";
            if ($(".MyTab").length > 0) TabName = $(".MyTab.Active").data("id");
            var QueryUrlStr = $(".QueryUrlStr").val();

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
                $(".HeaderGridView").find('.PagingNumbers').html("0 of 0");
                if ($.trim(obj.Records) !== '') {

                    if (TabName == "Actions") {
                        $(".GridActions").find('.SearchStyle').after(obj.Records);
                    }
                    else if (TabName == "Reports") {
                        $(".GridReports").find('.SearchStyle').after(obj.Records);
                    }
                    else if (TabName == "Dashboards") {
                        $(".GridDashboards").find('.SearchStyle').after(obj.Records);
                    }
                    else {
                        $(".HeaderGridView").find('.SearchStyle').after(obj.Records);
                    }

                    SetUITemplate();

                    if (TabName == "") {
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
                        PageTableTab("Grid" + TabName);
                        MaxPages = Math.ceil($(".Grid" + TabName).find('.GridResults').size() / NumberOfRecordsInPage);
                    }

                    $('.Arrow-Left-Back-First').trigger("click");

                    setTimeout(function () { InitColResizable(); }, 300);

                    if (MaxPages == 1 || CurrentPage == MaxPages - 1) {
                        $(".HeaderGridView").find(".Arrow-Right-Forward").addClass("Disabled");
                        $(".HeaderGridView").find(".Arrow-Right-Forward-Last").addClass("Disabled");
                    } else {
                        $(".HeaderGridView").find(".Arrow-Right-Forward").removeClass("Disabled");
                        $(".HeaderGridView").find(".Arrow-Right-Forward-Last").removeClass("Disabled");
                    }

                    $(".HeaderGridView").find(".GridContainer").on('click', '.editStyleNew', function () {
                        var myID = $(this).attr("data-id");
                        var myQueryURL = $(this).attr("data-queryurl");
                        if ($(".NewRecord").length > 0) myQueryURL = "";
                        DisplayItem(myID, myQueryURL);
                    });

                    $(".HeaderGridView").find(".GridContainer").on('click', '.editStyle', function () {
                        if ($('.MainPageTitle').attr("data-id") != "PROFILES") {
                            var myID = $(this).attr("data-id");
                            var myQueryURL = $(this).attr("data-queryurl");
                            DisplayItemNew(myID, myQueryURL);
                        }
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

                    if ($('.MainPageTitle').attr("data-id") == "PORTALUSERS") {
                        $('.chkSelectGrdActive').change(function () {
                            UpdateUserActive($(this));
                        });
                    }

                } else {
                    $(".HeaderGridView").find('.NoResults').show();
                }
            } else {
                $(".HeaderGridView").find('.NoResults').show();
            }

            $('.preloader').fadeOut(300, function () {
                AvoidWebServiceRaceCondition = 0;
                SetMasterResize();
            });
        }

        function OnLoadError(response) {
            console.log(response.error);
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
        var Key, StorerKey
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
        else if ($('.MainPageTitle').attr("data-id") == "Warehouse_OrderTracking") {
            Key = $('.NewHeaderRecord').find(".InputExternOrderKey").val();
            StorerKey = $('.NewHeaderRecord').find(".InputStorerKey").val();
        }
        data.append("Key", Key);
        data.append("StorerKey", StorerKey);
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
            $(".DetailsGridView").find('.PagingNumbers').html("0 of 0");
            if ($.trim(obj.Records) !== '') {

                $(".DetailsGridView").find('.SearchStyle').after(obj.Records);

                SetUITemplate();

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

                $('.Arrow-Left-Back-First').trigger("click");

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
            setOnCufex_Resize();
        });
    }

    function OnLoadError(response) {
        console.log(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

function LoadItemsPopup() {
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;
        $(".HeaderGridView").find('.GridResults').remove();
        $(".HeaderGridView").find('.NoResults').hide();
        if (!isFirstLoad) $('.preloader').fadeIn();
        isFirstLoad = false;

        var pageUrl = sAppPath + 'WebServices/GetItemsPopup.ashx';

        var QueryUrlStr = $(".QueryUrlStr").val();

        var data = new FormData();
        data.append("SearchQuery", SearchQuery);
        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
        data.append("SortBy", SortBy);
        data.append("QueryUrlStr", QueryUrlStr);

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
            $(".HeaderGridView").find('.PagingNumbers').html("0 of 0");
            if ($.trim(obj.Records) !== '') {

                $(".HeaderGridView").find('.SearchStyle').after(obj.Records);

                SetUITemplate();

                PageTable("HeaderGridView");
                MaxPages = Math.ceil($(".HeaderGridView").find('.GridResults').size() / NumberOfRecordsInPage);

                $('.Arrow-Left-Back-First').trigger("click");

                if (MaxPages == 1 || CurrentPage == MaxPages - 1) {
                    $(".HeaderGridView").find(".Arrow-Right-Forward").addClass("Disabled");
                    $(".HeaderGridView").find(".Arrow-Right-Forward-Last").addClass("Disabled");
                } else {
                    $(".HeaderGridView").find(".Arrow-Right-Forward").removeClass("Disabled");
                    $(".HeaderGridView").find(".Arrow-Right-Forward-Last").removeClass("Disabled");
                }

                $(".HeaderGridView").find(".GridContainer").on('click', '.selectStyle', function () {
                    if (window.opener != null && !window.opener.closed) {
                        var value = $.trim($(this).parent("td").siblings("td[data-id=1]").html().replace("<span>", "").replace("<span", ""));
                        var value1 = $.trim($(this).parent("td").siblings("td[data-id=2]").html().replace("<span>", "").replace("<span", ""));
                        var pack = "", packdescr = "";
                        var SearchDropDown = window.opener.$(".SearchDropDown.Active");
                        var ChosenSelect = SearchDropDown.siblings(window.opener.$(".chosen-select"));
                        if (ChosenSelect.attr("class").indexOf("InputDetailsSku") >= 0) {
                            pack = $.trim($(this).parent("td").siblings("td[data-id=3]").html().replace("<span>", "").replace("<span", ""));
                            packdescr = $.trim($(this).parent("td").siblings("td[data-id=4]").html().replace("<span>", "").replace("<span", ""));
                        }
                        var MyDataValues = "";
                        if (ChosenSelect.find("option[value='" + value + "']").length == 0) {
                            if (ChosenSelect.find("option[data-value1]").length > 0) MyDataValues = "data-value1='" + value1 + "'";
                            if (ChosenSelect.attr("class").indexOf("InputDetailsSku") >= 0) {
                                MyDataValues += " data-pack='" + pack + "' data-packdescr='" + packdescr + "'";
                            }
                            ChosenSelect.append("<option value='" + value + "'" + MyDataValues + ">" + value + "</option>");
                            ChosenSelect.siblings(".chosen-container").find("option").remove();
                        }
                        ChosenSelect.val(value).trigger("chosen:updated");

                        if (window.opener.$(".NewRecord:visible").length > 0) window.opener.$(".NewRecord").find('.InputDetailsSku').trigger("change");
                        else window.opener.$(".RecordsContainer_Inside:visible").find(".InputDetailsSku").trigger("change");
                    }
                    window.close();
                });

            } else {
                $(".HeaderGridView").find('.NoResults').show();
            }
        } else {
            $(".HeaderGridView").find('.NoResults').show();
        }

        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
            SetMasterResize();
        });
    }

    function OnLoadError(response) {
        console.log(response.error);
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
                                var MyDataValues = "";
                                var MySelectValue = "";
                                var ItemArrNew = ItemArr[i].split("~~~");
                                for (j = 0; j < ItemArrNew.length; j++) {
                                    if (j == 0) MySelectValue = ItemArrNew[j];
                                    else MyDataValues += " data-value" + j + "='" + ItemArrNew[j] + "'";
                                }
                                $(myclass).append("<option value='" + MySelectValue + "'" + MyDataValues + ">" + MySelectValue + "</option>");
                            }
                        }
                        $(myclass).trigger("chosen:updated");
                        SearchDropDown('.Input' + $.trim(MyItemsValues[0]));
                    }
                });
            }
        } else {
            console.log("Couldn't Display Drop Downs");
        }

        AvoidWebServiceRaceCondition = 0;
    }

    function OnLoadError(response) {
        console.log(response.error);
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
                                var MyDataValues = "";
                                var MySelectValue = "";
                                var ItemArrNew = ItemArr[i].split("~~~");
                                for (j = 0; j < ItemArrNew.length; j++) {
                                    if (j == 0) MySelectValue = ItemArrNew[j];
                                    else MyDataValues += " data-value" + j + "='" + ItemArrNew[j] + "'";
                                }
                                myclass.append("<option value='" + MySelectValue + "'" + MyDataValues + ">" + MySelectValue + "</option>");
                            }
                        }
                        myclass.trigger("chosen:updated");
                        SearchDropDown('.Input' + $.trim(MyItemsValues[0]));
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
                                        var MyDataValues = "";
                                        var MySelectValue = "";
                                        var ItemArrNew = ItemArr[i].split("~~~");
                                        for (j = 0; j < ItemArrNew.length; j++) {
                                            if (j == 0) MySelectValue = ItemArrNew[j];
                                            else MyDataValues += " data-value" + j + "='" + ItemArrNew[j] + "'";
                                        }
                                        myclass.eq(e).append("<option value='" + MySelectValue + "'" + MyDataValues + ">" + MySelectValue + "</option>");
                                    }
                                }
                                myclass.eq(e).trigger("chosen:updated");
                                if (myclass.eq(e).attr("class").indexOf("InputDetailsToLoc") >= 0) {
                                    if (myclass.eq(e).find("option[value='STAGE']").length == 0) {
                                        myclass.eq(e).append("<option value='STAGE'>STAGE</option>");
                                    }
                                    myclass.eq(e).val("STAGE").trigger("chosen:updated");
                                }
                                SearchDropDown('.InputDetails' + $.trim(MyItemsDetailsValues[0]));
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
                                    var MyDataValues = "";
                                    var MySelectValue = "";
                                    var ItemArrNew = ItemArr[i].split("~~~");
                                    for (j = 0; j < ItemArrNew.length; j++) {
                                        if (j == 0) MySelectValue = ItemArrNew[j];
                                        else MyDataValues += " data-value" + j + "='" + ItemArrNew[j] + "'";
                                    }
                                    myclass.append("<option value='" + MySelectValue + "'" + MyDataValues + ">" + MySelectValue + "</option>");
                                }
                            }
                            myclass.trigger("chosen:updated");
                            if (myclass.attr("class").indexOf("InputDetailsToLoc") >= 0) {
                                if (myclass.find("option[value='STAGE']").length == 0) {
                                    myclass.append("<option value='STAGE'>STAGE</option>");
                                }
                                myclass.val("STAGE").trigger("chosen:updated");
                            }
                            SearchDropDown('.InputDetails' + $.trim(MyItemsDetailsValues[0]));
                        }
                    }
                });
            }
        } else {
            console.log("Couldn't Display Drop Downs");
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
        console.log(response.error);
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
                            var MyDataValues = "";
                            var MySelectValue = "";
                            var ItemArrNew = ItemArr[i].split("~~~");
                            for (j = 0; j < ItemArrNew.length; j++) {
                                if (j == 0) MySelectValue = ItemArrNew[j];
                                else MyDataValues += " data-value" + j + "='" + ItemArrNew[j] + "'";
                            }
                            $(myclass).append("<option value='" + MySelectValue + "'" + MyDataValues + ">" + MySelectValue + "</option>");
                        }
                    }
                    $(myclass).trigger("chosen:updated");
                    SearchDropDown('.InputDetails' + $.trim(MyItemsValues[0]));
                }
            });
        } else {
            console.log("Couldn't Display Drop Downs");
        }
        AvoidWebServiceRaceCondition = 0;
        IsAutoPostBackDetails = false;
    }

    function OnLoadError(response) {
        console.log(response.error);
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
                    var MyDropDownDefaultValues = obj.DropDownFieldsDefaultValues;
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
                                    var MyDataValues = "";
                                    var MySelectValue = "";
                                    var ItemArrNew = ItemArr[i].split("~~~");
                                    for (j = 0; j < ItemArrNew.length; j++) {
                                        if (j == 0) MySelectValue = ItemArrNew[j];
                                        else MyDataValues += " data-value" + j + "='" + ItemArrNew[j] + "'";
                                    }
                                    $(myclass).append("<option value='" + MySelectValue + "'" + MyDataValues + ">" + MySelectValue + "</option>");
                                }
                                var MyDefaultValuesArr = MyDropDownDefaultValues.split(",");
                                for (i = 0; i < MyDefaultValuesArr.length; i++) {
                                    $(myclass).find("option").eq(i).attr("data-pack", MyDefaultValuesArr[i].split("~~~")[0]);
                                    $(myclass).find("option").eq(i).attr("data-packdescr", MyDefaultValuesArr[i].split("~~~")[1]);
                                }
                            }
                            if (MySelectedValue != "") $(myclass).val(MySelectedValue);
                            $(myclass).trigger("chosen:updated");
                            SearchDropDown(".InputDetailsSku");
                        }
                    });
                }
            } else {
                console.log("Couldn't Load Items");
            }

            if (showPreloader) {
                $('.preloader').fadeOut(300, function () {
                    AvoidWebServiceRaceCondition = 0;
                });
            }
        }

        function OnLoadError(response) {
            console.log(response.error);
            $('.preloader').fadeOut(300, function () {
                AvoidWebServiceRaceCondition = 0;
            });
        }
    }
}

function SearchDropDown(myclass) {
    $(myclass).each(function () {
        var $this = $(this);
        $this.siblings(".chosen-container").find('.chosen-choices input').autocomplete({
            source: function (request, response) {
                $('ul.chosen-results').empty();

                var SearchFound = false;
                $this.find("option").each(function (e) {
                    if ($(this).val().toLowerCase().startsWith(request.term.toLowerCase())) {
                        SearchFound = true;
                        $('ul.chosen-results').append("<li class='" + ($this.val().indexOf($(this).val()) >= 0 ? "result-selected" : "active-result") + "' data-option-array-index='" + e + "'><em>" + $(this).val().substring(0, request.term.length) + "</em>" + $(this).val().substring(request.term.length) + "</li>");
                    }
                    else {
                        if ($(this).attr("data-value1") != null) {
                            if ($(this).attr("data-value1").toLowerCase().startsWith(request.term.toLowerCase())) {
                                SearchFound = true;
                                $('ul.chosen-results').append("<li class='active-result' data-option-array-index='" + e + "'>" + $(this).val() + "</li>");
                            }
                        }

                        if ($(this).attr("data-value2") != null) {
                            if ($(this).attr("data-value2").toLowerCase().startsWith(request.term.toLowerCase()) && !SearchFound) {
                                SearchFound = true;
                                $('ul.chosen-results').append("<li class='active-result' data-option-array-index='" + e + "'>" + $(this).val() + "</li>");
                            }
                        }
                    }
                });

                if (!SearchFound) $('ul.chosen-results').append("<li class='no-results'>No results match " + request.term + "</li>");

            }
        });
    });
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
            console.log("Couldn't Set Price and Currency");
        }
        AvoidWebServiceRaceCondition = 0;
    }
    function OnLoadError(response) {
        console.log(response.error);
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
                setTimeout(function () {
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
                }, 500);
            }
        } else {
            console.log("Couldn't Execute Action");
        }

        if (success) {
            AvoidWebServiceRaceCondition = 0;
            if ($(".NewRecord:visible").length > 0) DisplayItemNew($(".MyRecordID").val(), '');
            else {
                isFirstLoad = true;
                LoadItems();
            }
        }
    }

    function OnLoadError(response) {
        console.log(response.error);
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
        var success = true;
        if (Object.keys(obj).length > 0) {
            if (obj.Error != null) {
                success = false;
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
            console.log("Couldn't Update Permission");
        }
        if (success) {
            AvoidWebServiceRaceCondition = 0;
        }
    }
    function OnLoadError(response) {
        console.log(response.error);
        AvoidWebServiceRaceCondition = 0;
    }
}

function UpdateUserActive(MyCheckbox) {
    AvoidWebServiceRaceCondition = 0;
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;

        var pageUrl = sAppPath + 'WebServices/UpdateUserActive.ashx';

        var data = new FormData();
        data.append("MyID", MyCheckbox.data("id"));
        data.append("Active", MyCheckbox.is(':checked'));

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
        var success = true;
        if (Object.keys(obj).length > 0) {
            if (obj.Error != null) {
                success = false;
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
                MyCheckbox.closest('.GridResults').find(".chkSelectGrdActive").prop("checked", MyCheckbox.is(':checked'));
                $(".RecordHeader").find(".InputActive").val(MyCheckbox.is(':checked') ? 1 : 0);
            }
        } else {
            console.log("Couldn't Update User Active");
        }
        if (success) {
            AvoidWebServiceRaceCondition = 0;
        }
    }
    function OnLoadError(response) {
        console.log(response.error);
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
        data.append("SearchTable", $(".MainPageTitle").length > 0 ? $('.MainPageTitle').attr("data-id") : "");

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
                var ColumnsNamesArr = obj.ColumnsNames.split(",");
                var ColumnsPrioritiesArr = obj.ColumnsPriorities.split(",");
                var ColumnsHiddenArr = obj.ColumnsHidden.split(",");
                var ColumnsWidthesArr = obj.ColumnsWidthes.split(",");
                var GridTypesArr = obj.GridTypes.split(",");
                var timer = obj.MenuOpen == "0" ? 500 : 0;

                setTimeout(function () {
                    var $MyGrid;
                    MyWidth = 0;
                    MyWidthDetails = 0;

                    for (var i = 0; i < ColumnsNamesArr.length; i++) {
                        var width = ColumnsHiddenArr[i] == "0" ? ColumnsWidthesArr[i] : 0;
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

                    ShowHideColumns($(".HeaderGridView").find(".GridRow"));
                    ChangeColumnsOrder($(".HeaderGridView").find(".GridRow"));

                    ShowHideColumns($(".DetailsGridView").find(".GridRow"));
                    ChangeColumnsOrder($(".DetailsGridView").find(".GridRow"));

                    $(".GridCell").find(".chosen-container").remove();

                    $(".GridCell").find(".chosen-select").chosen({
                        disable_search_threshold: 10,
                        width: "100%",
                        search_contains: false
                    });

                    var isHidden = true;

                    $(".GridCell").find(".chosen-select").on("chosen:showing_dropdown", function (e) {
                        isHidden = false;
                        $(".content_3").mCustomScrollbar("disable")
                        $(this).parents("div").css("overflow", "visible");
                        $(this).siblings(".chosen-container").find(".search-field").show();
                        setTimeout(function () { InitColResizable(); isHidden = true; }, 300);
                    });

                    $(".GridCell").find(".chosen-select").on("chosen:hiding_dropdown", function (e) {
                        if (isHidden) {
                            $(".content_3").mCustomScrollbar("update");
                            $(this).parents("div").css("overflow", "");
                            setTimeout(function () { InitColResizable(); }, 300);
                            if ($(this).val() == "") return;
                            $(this).siblings(".chosen-container").find(".search-field").hide();
                        }
                    });

                    $(".GridCell").find(".chosen-select").on('change', function () {
                        if ($(this).val() == "") $(this).siblings(".chosen-container").find(".search-field").show();
                    });

                    SetDropDownSingleSelection();

                    $("body").on('keydown', '.GridCell .chosen-search-input', function (e) {
                        if (e.which === 13 && $(this).closest('.chosen-container').find('.chosen-single-with-drop').length === 0) {
                            setTimeout(function () {
                                $(".GridSearch").trigger("click");
                            }, 300);
                        }
                    });

                    $(".HeaderGridView").find(".GridRow").not(".NoResults").each(function () {
                        var $thisChild = $(this).children(".GridCell[data-priority]");
                        $thisChild.removeClass("borderRight0");
                        $thisChild.eq($thisChild.length - 1).addClass("borderRight0");
                    });

                    $(".DetailsGridView").find(".GridRow").not(".NoResults").each(function () {
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
            console.log("Couldn't Get User Configuration");
        }

        AvoidWebServiceRaceCondition = 0;
        setTimeout(function () { InitColResizable(); }, 1000);
    }
    function OnLoadError(response) {
        console.log(response.error);
        AvoidWebServiceRaceCondition = 0;
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

        if ($(".GridAdjust").length > 0) {
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
        }

        var pageUrl = sAppPath + 'WebServices/UserConfiguration.ashx';

        var data = new FormData();
        data.append("ActionType", "set");
        data.append("SearchTable", $(".MainPageTitle").length > 0 ? $('.MainPageTitle').attr("data-id") : "");
        data.append("MenuOpen", $(".MenuPin").hasClass("Pinned") ? 1 : 0);
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
            console.log("Couldn't Set User Configuration");
        }
        setTimeout(function () {
            AvoidWebServiceRaceCondition = 0;
            $(".preloader").fadeOut();
        }, 300);
    }
    function OnLoadError(response) {
        console.log(response.error);
        setTimeout(function () {
            AvoidWebServiceRaceCondition = 0;
            $(".preloader").fadeOut();
        }, 300);
    }
}

function ExportItems() {
    AvoidWebServiceRaceCondition = 0;
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;

        var ColumnsNames = "";
        $(".MyFields[data-columnname]").each(function () {
            ColumnsNames += $(this).val() + ",";
        });

        var pageUrl = sAppPath + "WebServices/ExportItems.ashx";

        var TabName = "";
        if ($(".MyTab").length > 0) TabName = $(".MyTab.Active").data("id");
        var QueryUrlStr = $(".QueryUrlStr").val();

        var data = new FormData();
        data.append("SearchQuery", SearchQuery);
        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
        data.append("ColumnsNames", ColumnsNames.slice(0, -1));
        data.append("SortBy", SortBy);
        data.append("QueryUrlStr", QueryUrlStr);
        data.append("TabName", TabName);

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/Export",
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }
    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        if (Object.keys(obj).length > 0) {
            if (obj.Error != "") {
                swal({
                    title: "Export",
                    text: obj.Error,
                    type: 'error',
                    confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                    showCancelButton: false
                });
                AvoidWebServiceRaceCondition = 0;
            } else {
                var contentType = 'text/csv';
                var csvExport = "\ufeff" + obj.CSVData;
                var csvFile = new Blob([csvExport], { type: contentType });
                var hiddenElement = document.createElement('a');
                hiddenElement.href = URL.createObjectURL(csvFile);
                hiddenElement.download = obj.FileName;
                hiddenElement.dataset.downloadurl = [contentType, hiddenElement.download, hiddenElement.href].join(':');

                if (obj.ExportRowsLimit > 0) {
                    swal({
                        title: "Export Rows Limit",
                        text: "Only " + obj.ExportRowsLimit + " records will be exported",
                        type: 'warning',
                        confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                        showCancelButton: true
                    },
                        function (isConfirm) {
                            if (isConfirm) {
                                hiddenElement.click();
                            }
                            else {
                                AvoidWebServiceRaceCondition = 0;
                            }
                        });
                } else {
                    hiddenElement.click();
                }
            }
        } else {
            console.log("Couldn't Export Items");
        }

        setTimeout(function () {
            AvoidWebServiceRaceCondition = 0;
        }, 300);
    }
    function OnLoadError(response) {
        console.log(response.error);
        setTimeout(function () {
            AvoidWebServiceRaceCondition = 0;
        }, 300);
    }
}

function ExportItemsDetails() {
    AvoidWebServiceRaceCondition = 0;
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;

        var ColumnsNames = "";
        $(".MyDetailsFields[data-columnname]").each(function () {
            ColumnsNames += $(this).val() + ",";
        });

        var pageUrl = sAppPath + "WebServices/ExportItemsDetails.ashx";

        var data = new FormData();
        data.append("SearchQuery", SearchQueryDetails);
        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
        data.append("ColumnsNames", ColumnsNames.slice(0, -1));
        data.append("SortBy", SortByDetails);
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

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/Export",
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }
    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        if (Object.keys(obj).length > 0) {
            if (obj.Error != "") {
                swal({
                    title: "Export",
                    text: obj.Error,
                    type: 'error',
                    confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                    showCancelButton: false
                });
                AvoidWebServiceRaceCondition = 0;
            } else {
                var contentType = 'text/csv';
                var csvExport = "\ufeff" + obj.CSVData;
                var csvFile = new Blob([csvExport], { type: contentType });
                var hiddenElement = document.createElement('a');
                hiddenElement.href = URL.createObjectURL(csvFile);
                hiddenElement.download = obj.FileName;
                hiddenElement.dataset.downloadurl = [contentType, hiddenElement.download, hiddenElement.href].join(':');

                if (obj.ExportRowsLimit > 0) {
                    swal({
                        title: "Export Rows Limit",
                        text: "Only " + obj.ExportRowsLimit + " records will be exported",
                        type: 'warning',
                        confirmButtonColor: $('.AlertconfirmButtonColor').val(),
                        showCancelButton: true
                    },
                        function (isConfirm) {
                            if (isConfirm) {
                                hiddenElement.click();
                            }
                            else {
                                AvoidWebServiceRaceCondition = 0;
                            }
                        });
                } else {
                    hiddenElement.click();
                }
            }
        } else {
            console.log("Couldn't Export Items");
        }

        setTimeout(function () {
            AvoidWebServiceRaceCondition = 0;
        }, 300);
    }
    function OnLoadError(response) {
        console.log(response.error);
        setTimeout(function () {
            AvoidWebServiceRaceCondition = 0;
        }, 300);
    }
}

function ImportItems(fileSize) {
    AvoidWebServiceRaceCondition = 0;
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;

        $(".preloader").fadeIn();

        var pageUrl = sAppPath + 'WebServices/ImportItems.ashx';

        var data = new FormData();
        data.append("SearchTable", $('.MainPageTitle').attr("data-id"));
        data.append("ImportFile", $(".ImportFileUpload").get(0).files[0]);
        data.append("ImportFileSize", fileSize);

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/Import",
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }
    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        var success = true;
        if (Object.keys(obj).length > 0) {
            if (obj.Error != null) {
                success = false;
                setTimeout(function () {
                    swal({
                        title: "Import",
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
            console.log("Couldn't Import Items");
        }

        if (success) {
            AvoidWebServiceRaceCondition = 0;
            isFirstLoad = true;
            LoadItems();
        }
    }

    function OnLoadError(response) {
        console.log(response.error);
        $('.preloader').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
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

function OpenMenu() {
    if ($(".widthMenu:visible").length > 0) {
        if (NtorKermelElraceCondition == 0) {
            NtorKermelElraceCondition = 1;
            ClearConfigTimer();
            MenuClosed = false;
            $(".widthMenu,.MenuContainer,.widthContent").removeClass("Closed");
            $(".AllMenuItems").hide();
            $("table.u-marginAuto").fadeOut();
            setTimeout(function () {
                $(".MainLogoContainer,.MenuItemName,.MenuSubItem,.MenuSubSubItem,.MenuItemStyleSel,.u-pl-xs-20,.u-pb-xs-30").removeClass("Closed");
                $(".OpenMenu").addClass("Closed");
                $(".AllMenuItems").fadeIn();
                setOnCufex_Resize();
                $("table.u-marginAuto").fadeIn();
                InitColResizable();
                SetConfigTimer();
                NtorKermelElraceCondition = 0;
            }, 300);
        }
    }
}

function CloseMenu(resize) {
    if ($(".widthMenu:visible").length > 0) {
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
            onResize: ResizeColumns,
            gripInnerHtml: "<div class='grip'></div>",
            draggingClass: "dragging"
        });

        //var ColNoResize = $(".GridAdjust").children(".GridCell:not([data-id])").length;
        //$('.JCLRgrips').find(".JCLRgrip:lt(" + ColNoResize + ")").remove();

        function ResizeColumns() {
            if ($(".GridContainer").data("resizemode") == "overflow") {
                if ($(".HeaderGridView:visible").length > 0) {
                    $(".HeaderGridView").find(".PagingContainer").width($(".HeaderGridView").find(".GridContainer").innerWidth());
                    $(".HeaderGridView").mCustomScrollbar("update");
                }
                else {
                    $(".DetailsGridView").find(".PagingContainer").width($(".DetailsGridView").find(".GridContainer").innerWidth());
                    $(".DetailsGridView").mCustomScrollbar("update");
                }
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

    $(".widthMenu").mouseenter(function (e) {
        e.stopPropagation();
        if (MenuClosed) OpenMenu();
    });

    $(".widthMenu").mouseleave(function (e) {
        e.stopPropagation();
        if (!$('.MenuPin').hasClass("Pinned") && !MenuClosed) {
            CloseMenu(false);
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
            SetConfigTimer();
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

function MoveTabLine(index) {
    $(".MyTabLine").animate({
        width: $(".MyTab").eq(index).outerWidth() + "px",
        left: ($(".MyTab").eq(index).offset().left - $(".widthMenu").width()) + "px"
    }, 200);
}

function GetTime() {
    var dt = new Date();
    return dt.getHours() + ":" + dt.getMinutes() + ":" + dt.getSeconds();
}

function GetCarrierDetails(CarrierDetailType) {
    if (AvoidWebServiceRaceCondition == 0) {
        AvoidWebServiceRaceCondition = 1;
        $(".preloader2").fadeIn();
        var pageUrl = sAppPath + 'WebServices/CarrierEvents.ashx';

        var data = new FormData();
        data.append("CarrierDetailType", CarrierDetailType);
        data.append("ExternalOrderKey", $(".CarrierDiv.Active").data("externalorderkey"));
        data.append("StorerKey", $(".CarrierDiv.Active").data("storerkey"));
        data.append("ConsignmentID", $(".CarrierDiv.Active").data("consignmentid"));
        data.append("Category", $(".CarrierDiv.Active").data("category"));
        //data.append("ArticleID", $(".ArticleDiv.Active").data("externaloderkey"));

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            data: data,
            url: pageUrl + "/GetCarrierEventsDetails",
            success: OnLoadSuccess,
            error: OnLoadError
        });
    }
    function OnLoadSuccess(response) {
        var obj = jQuery.parseJSON(response);
        if (Object.keys(obj).length > 0) {

            if (obj.CarrierDetailsRecords != "") {
                $(".ArticlesDivRecords").html(obj.CarrierDetailsRecords);
            }
        } else {
            console.log("Couldn't Get Carrier Details");
        }

        $('.TrackingHistoryDiv').unbind("click").bind("click", function (e) {
            var $this = $(this);
            if (!$this.hasClass("Active")) {
                $('.TrackingHistoryDiv').not($this).siblings(".EventsDiv").slideUp(function () {
                    setTimeout(function () {
                        $('.TrackingHistoryDiv').not($this).removeClass("Active");
                    }, 400);
                });
                $this.addClass("Active");
                $this.siblings(".EventsDiv").slideDown();
            }
            else {
                $this.siblings(".EventsDiv").slideUp(function () {
                    $this.removeClass("Active");
                });
            }
        });

        $('.preloader2').fadeOut(300, function () {
            setOnCufex_Resize();
            AvoidWebServiceRaceCondition = 0;
        });
    }
    function OnLoadError(response) {
        console.log(response.error);
        $('.preloader2').fadeOut(300, function () {
            AvoidWebServiceRaceCondition = 0;
        });
    }
}

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

    if ($(window).width() <= 860 && !MenuClosed) CloseMenu(false);

    if ($(window).width() <= 550) {
        if ($(".btnSave:visible").length > 0) {
            $(".btnDelete").parent("td").css("padding-right", "0px");
            $(".btnDelete").parent("td").css("margin-bottom", "10px");
            $(".btnActions").parent("td").css("padding-right", "10px");
            $(".ActionHiddenButtons").css("right", "unset");
        }
        else {
            $(".btnDelete").parent("td").css("padding-right", "10px");
            $(".btnDelete").parent("td").css("margin-bottom", "0px");
            if ($(".NewRecord").length > 0) $(".btnActions").parent("td").css("padding-right", "0px");
            else $(".btnActions").parent("td").css("padding-right", "10px");
            $(".ActionHiddenButtons").css("right", "0");
        }
    } else {
        $(".btnDelete").parent("td").css("padding-right", "0px");
        $(".ActionHiddenButtons").css("right", "0");
    }


    if ($(".MyTab").length > 0) {
        MoveTabLine($(".MyTab.Active").index());
    }

    $(".content_3").mCustomScrollbar("update");
    $(".content_3").width(0);
    $(".content_3").width($('.MainHeader').width());
    $(".content_3").find('.mCSB_container').width($('.MainHeader').width());

    $(".content_4").mCustomScrollbar("update");
}