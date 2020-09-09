var Pirobox = false;
var ToucheEvent = false;
var Responsive = true;
var ScrollSpeed = false;

//ASK NADER BEFORE CHANGING ANYTHING BELOW THIS COMMENT
var isMobile = (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent));
var isTablet = (/iPad/i.test(navigator.userAgent));
var isIOS = (/iPhone|iPad|iPod/i.test(navigator.userAgent));
var sAppPath = $('.sAppPath').val();

var myClickEvent = ('ontouchstart' in document.documentElement ? "click" : "click");

$(document).ready(function () {
    MainLoad();
});

$(window).on("load", function () {
    SetMasterResize();
});


var prm = Sys.WebForms.PageRequestManager.getInstance();
prm.add_endRequest(function () { MainLoad(); });

function GetWindowObj() {
    var MyWindow;
    if (parseInt($('.isInIframe').val()) == 1) MyWindow = window.top;
    else MyWindow = window;

    return MyWindow;
}
function GotoParentIfInIfram(OpenNewWindow, Link, width, height) {
    var MyWindow = GetWindowObj();
    var myhref = Link;
    myhref = myhref ? myhref.match("javascript:") : false;

    if (myhref) MyWindow = window;

    if (OpenNewWindow) {
        MyWindow.open(Link, '', 'toolbar=0, status=0, width=' + width + ', height=' + height);
    } else {
        MyWindow.location = Link;
    }
}

function MainLoad() {
    SetMainEvents();
    if (ToucheEvent) AddTouchHandler();
    if (Pirobox) SetPiroBox();
    if (ScrollSpeed) SetScrollSpeed();

    if (Responsive) {
        SetMasterResize();
        $(window).resize(function () {
            SetMasterResize();
        });
    }
}
function SetScrollSpeed() {
    var platform = navigator.platform.toLowerCase();
    if (platform.indexOf('win') == 0 || platform.indexOf('linux') == 0) {
        if ($.browser.webkit) {
            $.srSmoothscroll({
                // defaults
                step: 145,
                speed: 400,
                ease: 'swing'
            });
        }
    }
}
function SetPiroBox() {
    if ($(".pirobox_gall").length > 0 || $(".pirobox_gall1").length > 0) {
        $.piroBox_ext({
            piro_speed: 700,
            bg_alpha: 0.5,
            piro_scroll: true,
            piro_drag: false,
            piro_nav_pos: 'bottom'
        });
    }
}
function SetMainEvents() {
    $('.GoToChildOnClick').unbind(myClickEvent).bind(myClickEvent, function () {
        GotoParentIfInIfram(false, $(this).find('a').attr("href"), 0, 0);
    });
    $('.GoToSecondChildOnClick').unbind(myClickEvent).bind(myClickEvent, function () {
        var myHref = "";
        if ($(this).find('a').length > 1) myHref = $(this).find('a').eq(1).attr("href");
        else myHref = $(this).find('a').attr("href");

        GotoParentIfInIfram(false, myHref, 0, 0);
    });
    $('.GoToInputOnClick').unbind(myClickEvent).bind(myClickEvent, function () {
        GotoParentIfInIfram(false, $(this).children('input').val(), 0, 0);
    });
    if (isMobile || isIOS) $('.FixedImage').removeClass("FixedImage");

    $('.CoverImage').css({ backgroundSize: "cover" });
    $('.ContainImage').css({ backgroundSize: "contain" });
    $('.OpenHandStyle').mousedown(function () {
        $(this).css({ "cursor": "-moz-grabbing" });
        $(this).css({ "cursor": "-webkit-grabbing" });
    }).mouseup(function () {
        $(this).css({ "cursor": "-moz-grab" });
        $(this).css({ "cursor": "-webkit-grab" });
    });
}
function SetMasterResize() {
    var WindowHeight = parseInt($(window).height());
    var WindowWidth = $(window).width();

    //$('.GetFullHeightAtLeast').css({ "min-height": WindowHeight + "px" });

    $('.GetFullHeightAtLeast').css({ "min-height": WindowHeight - parseInt($('.WhiteHead').height()) + "px" });

    $('.GetFullHeightAtLeastRemoveFooter').css({ "min-height": WindowHeight - parseInt($('.DivFooter').height()) + "px" });
    $('.GetFullHeight').css({ "height": WindowHeight + "px" });
    $('.GetFullHeightAtMax').css({ "max-height": WindowHeight + "px" });
    $('.GetFullHeightAtLeastRemoveHeaderAndFooter').css({ "min-height": WindowHeight - parseInt($('.DivFooter').height()) - parseInt($('.DivHeader').height()) + "px" });
    $('.GetFullHeightFullScreen').css({ "min-height": WindowHeight - 1 + "px" });
    $('.GetFullHeightAtLeastRemoveHeader').css({ "min-height": WindowHeight - parseInt($('.DivHeader').height()) + "px" });
    $('.GetFullHeightRemoveHeader').css({ "height": WindowHeight - parseInt($('.DivHeader').height()) + "px" });
    $('.GetFullHeightForPopup').css({ "height": WindowHeight - parseInt($('.WhiteHead').height()) - 200 + "px" });

    $('.GetFullHeightRemoveHeaderRemoveFollow').css({ "height": parseInt($('.fixedInMenu ').height()) - parseInt($('.FollowUsMenu').height()) + "px" });
    $('.GetFullHeightAtMaxRemoveHeader').css({ "max-height": WindowHeight - parseInt($('.DivHeader').height()) + "px" });
    $('.GetFullHeightRemoveCufex').css({ "height": WindowHeight - parseInt($('.CufexLogo').height()) + "px" });
    $('.GetFullHeightRemoveFooter').css({ "height": WindowHeight - parseInt($('.DivFooter').height()) + "px" });
    $('.ReplaceHeaderHeight').css({ "height": parseInt($('.DivHeader').height()) + "px" });
    $('.SearchDivRemoveOverFlow').css('width', WindowWidth * 0.95 + "px");

    $('.iWantMyChildrenFloatHeight').each(function () {
        $(this).css({ "height": parseInt($(this).children().height()) + "px" });
        $(this).parents('.iWantMyChildrenFloatHeight').each(function () {
            $(this).css({ "height": parseInt($(this).children().height()) + "px" });
        });
    });

    $('.GetFullHeightRemoveHeaderAndFooter').css({ "height": WindowHeight - parseInt($('.DivFooter').height()) - parseInt($('.DivHeader').height()) + "px" });
    $('.GetFullHeightRemoveHeaderAndFooter').css({ "min-height": 500 - parseInt($('.DivFooter').height()) + "px" });

    $('.ReplaceFooterHeight').css({ "height": parseInt($('.DivFooter').height()) + "px" });
    $('.iWantMyParentHeight').each(function () {
        $(this).css({ "height": parseInt($(this).parent().height()) + "px" });
    });
    $('.VerticalMiddle').each(function () {
        $(this).css({ "top": (parseInt($(this).parent().height()) - parseInt($(this).height())) / 2 + "px" });
    });

    $('.GetMyParentWidth').css({ "width": "100%" });

    $('.GetMyParentWidth').each(function () {
        $(this).css({ "width": parseInt($(this).parent().width()) + "px" });
    });

    $('.GetMyChildrenHeight').each(function () {
        $(this).css({ "height": parseInt($(this).children().height()) + "px" });
    });
}
function BtnClick(ValidationGroup) {
    var val = Page_ClientValidate(ValidationGroup);
    if (!val) {
        for (var i = 0; i < Page_Validators.length; i++) {
            var myPageID = "#" + Page_Validators[i].controltovalidate;
            var myPageIDClass = "." + Page_Validators[i].controltovalidate;
            if ($(myPageID).attr('class') == 'FileBrows') {
                $(myPageIDClass).removeClass("InputError");
            }
            if ($(myPageID).attr('data-val') == 'Cmb') {
                $(myPageID).css({ "border-bottom": "1px solid #0068ff" });
            }
            else {
                $(myPageID).removeClass("control_validation_error").removeClass("txtError");
            }
            $(myPageID).parent().removeClass("borderColorCustomError");
        }

        for (var j = 0; j < Page_Validators.length; j++) {
            if (!Page_Validators[j].isvalid) {
                var myPageID2 = "#" + Page_Validators[j].controltovalidate;
                var myPageIDClass2 = "." + Page_Validators[j].controltovalidate;
                if ($(myPageID2).attr('class') == 'FileBrows') {
                    $(myPageIDClass2).addClass("InputError");
                }
                if ($(myPageID2).attr('data-val') == 'Cmb') {
                    $(myPageID2).css({ "border-bottom": "1px red solid" });
                }
                else {
                    $(myPageID2).addClass("control_validation_error").addClass("txtError");
                }
            }
        }
    }
    Page_BlockSubmit = false;

    return val;
}
function AddTouchHandler() {
    document.addEventListener("touchstart", touchHandler, true);
    document.addEventListener("touchmove", touchHandler, true);
    document.addEventListener("touchend", touchHandler, true);
    document.addEventListener("touchcancel", touchHandler, true);
}
function touchHandler(event) {
    var touch = event.changedTouches[0];

    var simulatedEvent = document.createEvent("MouseEvent");
    simulatedEvent.initMouseEvent({
        touchstart: "click",
        touchmove: "mousemove",
        touchend: "mouseup"
    }[event.type], true, true, window, 1,
        touch.screenX, touch.screenY,
        touch.clientX, touch.clientY, false,
        false, false, false, 0, null);

    touch.target.dispatchEvent(simulatedEvent);
    event.preventDefault();
}

var keys = { 32: 1, 33: 1, 34: 1, 35: 1, 36: 1, 37: 1, 38: 1, 39: 1, 40: 1 };

function preventDefault(e) {
    e = e || window.event;
    if (e.preventDefault)
        e.preventDefault();
    e.returnValue = false;
}

//function keydown(e) {
//    for (var i = keys.length; i--;) {
//        if (e.keyCode === keys[i]) {
//            preventDefault(e);
//            return;
//        }
//    }
//}

function wheel(e) {
    preventDefault(e);
}

//function disable_scroll() {
//    if (window.addEventListener) {
//        window.addEventListener('DOMMouseScroll', wheel, false);
//    }
//    window.onmousewheel = document.onmousewheel = wheel;
//    document.onkeydown = keydown;
//}

//function enable_scroll() {
//    if (window.removeEventListener) {
//        window.removeEventListener('DOMMouseScroll', wheel, false);
//    }
//    window.onmousewheel = document.onmousewheel = document.onkeydown = null;
//}

function preventDefaultForScrollKeys(e) {
    if (keys[e.keyCode]) {
        preventDefault(e);
        return false;
    }
}

// modern Chrome requires { passive: false } when adding event
var supportsPassive = false;
try {
    window.addEventListener("test", null, Object.defineProperty({}, 'passive', {
        get: function () { supportsPassive = true; }
    }));
} catch (e) { console.log(e.message); }

var wheelOpt = supportsPassive ? { passive: false } : false;
var wheelEvent = 'onwheel' in document.createElement('div') ? 'wheel' : 'mousewheel';

// call this to Disable
function disable_scroll() {
    window.addEventListener('DOMMouseScroll', preventDefault, false); // older FF
    window.addEventListener(wheelEvent, preventDefault, wheelOpt); // modern desktop
    window.addEventListener('touchmove', preventDefault, wheelOpt); // mobile
    window.addEventListener('keydown', preventDefaultForScrollKeys, false);
}

// call this to Enable
function enable_scroll() {
    window.removeEventListener('DOMMouseScroll', preventDefault, false);
    window.removeEventListener(wheelEvent, preventDefault, wheelOpt);
    window.removeEventListener('touchmove', preventDefault, wheelOpt);
    window.removeEventListener('keydown', preventDefaultForScrollKeys, false);
}