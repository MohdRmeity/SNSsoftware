/*
 * --------------------------------------------------------------------
 * Simple Password Strength Checker
 * by Siddharth S, www.ssiddharth.com, hello@ssiddharth.com
 * for Net Tuts, www.net.tutsplus.com
 * Version: 1.0, 05.10.2009
 * --------------------------------------------------------------------
 */

$(window).on("load", function () { PassLoad(); });
var prm = Sys.WebForms.PageRequestManager.getInstance();
prm.add_endRequest(function () { PassLoad(); });

var strPassword;
var charPassword;
var complexity = $("#complexity");

var minPasswordLength = 6;
var baseScore = 0, score = 0;

var num = {};
num.Excess = 0;
num.Upper = 0;
num.Numbers = 0;
num.Symbols = 0;

var bonus = {};
bonus.Excess = 3;
bonus.Upper = 4;
bonus.Numbers = 5;
bonus.Symbols = 5;
bonus.Combo = 0;
bonus.FlatLower = 0;
bonus.FlatNumber = 0;

function PassLoad() {
    $(".inputPassword").bind("keyup", checkVal);
}

function checkVal() {
    init();

    if (charPassword.length >= minPasswordLength) {
        baseScore = 50;
        analyzeString();
        calcComplexity();
    }
    else {
        baseScore = 0;
    }

    outputResult();
}

function init() {
    strPassword = $(".inputPassword").val();
    charPassword = strPassword.split("");

    num.Excess = 0;
    num.Upper = 0;
    num.Numbers = 0;
    num.Symbols = 0;
    bonus.Combo = 0;
    bonus.FlatLower = 0;
    bonus.FlatNumber = 0;
    baseScore = 0;
    score = 0;
}

function analyzeString() {
    for (i = 0; i < charPassword.length; i++) {
        if (charPassword[i].match(/[A-Z]/g)) { num.Upper++; }
        if (charPassword[i].match(/[0-9]/g)) { num.Numbers++; }
        if (charPassword[i].match(/(.*[!,@,#,$,%,^,&,*,?,_,~])/)) { num.Symbols++; }
    }

    num.Excess = charPassword.length - minPasswordLength;

    if (num.Upper && num.Numbers && num.Symbols) {
        bonus.Combo = 25;
    }

    else if ((num.Upper && num.Numbers) || (num.Upper && num.Symbols) || (num.Numbers && num.Symbols)) {
        bonus.Combo = 15;
    }

    if (strPassword.match(/^[\sa-z]+$/)) {
        bonus.FlatLower = -15;
    }

    if (strPassword.match(/^[\s0-9]+$/)) {
        bonus.FlatNumber = -35;
    }
}

function calcComplexity() {
    score = baseScore + (num.Excess * bonus.Excess) + (num.Upper * bonus.Upper) + (num.Numbers * bonus.Numbers) + (num.Symbols * bonus.Symbols) + bonus.Combo + bonus.FlatLower + bonus.FlatNumber;
}

function outputResult() {
    if ($(".inputPassword").val() == "") {
        //complexity.html("Enter a random value").removeClass("weak strong stronger strongest").addClass("default");
        $('.PasswordText').html('Weak');
        checkLength(0);
    }
    else if (charPassword.length < minPasswordLength) {
        $('.PasswordText').html('Weak');
        //complexity.html("At least " + minPasswordLength+ " characters please!").removeClass("strong stronger strongest").addClass("weak");
        checkLength(2);
    }
    else if (score < 50) {
        $('.PasswordText').html('Weak');
        //complexity.html("Weak!").removeClass("strong stronger strongest").addClass("weak");
        checkLength(3);
    }
    else if (score >= 50 && score < 75) {
        //complexity.html("Average!").removeClass("stronger strongest").addClass("strong");
        $('.PasswordText').html('Average');
        checkLength(4);
    }
    else if (score >= 75 && score < 100) {
        // complexity.html("Strong!").removeClass("strongest").addClass("stronger");
        $('.PasswordText').html('Good');
        checkLength(5);
    }
    else if (score >= 100) {
        // complexity.html("Secure!").addClass("strongest");
        $('.PasswordText').html('Secure');
        checkLength(6);
    }

    /* $("#details").html("Base Score :<span class=\"value\">" + baseScore  + "</span>"
                   + "<br />Length Bonus :<span class=\"value\">" + (num.Excess*bonus.Excess) + " ["+num.Excess+"x"+bonus.Excess+"]</span> "
                   + "<br />Upper case bonus :<span class=\"value\">" + (num.Upper*bonus.Upper) + " ["+num.Upper+"x"+bonus.Upper+"]</span> "
                   + "<br />Number Bonus :<span class=\"value\"> " + (num.Numbers*bonus.Numbers) + " ["+num.Numbers+"x"+bonus.Numbers+"]</span>"
                   + "<br />Symbol Bonus :<span class=\"value\"> " + (num.Symbols*bonus.Symbols) + " ["+num.Symbols+"x"+bonus.Symbols+"]</span>"
                   + "<br />Combination Bonus :<span class=\"value\"> " + bonus.Combo + "</span>"
                   + "<br />Lower case only penalty :<span class=\"value\"> " + bonus.FlatLower + "</span>"
                   + "<br />Numbers only penalty :<span class=\"value\"> " + bonus.FlatNumber + "</span>"
                   + "<br />Total Score:<span class=\"value\"> " + score  + "</span>" );

                   */
}

function checkLength(obj) {
    $('.passwordBox').css('background-color', '#c9c9c9');
    if (obj > 0) {
        $('.passwordChecker').show();
        for (i = 1; i <= obj; i++) {
            $('.passwordBox' + i).css('background-color', '#42dc3e');
        }
    }
    else {
        $('.passwordChecker').hide();
    }
}