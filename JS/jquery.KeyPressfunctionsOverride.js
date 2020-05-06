jQuery.fn.KeyPressfunctionsOverride = function (options) {
    $BackSpaceControl = false;
    $(window).bind('beforeunload', function (e) {
        if ($BackSpaceControl) {
            $BackSpaceControl = false;
            return "Are you sure you want to leave";
            e.preventDefault();
        }
    });
    $(document).keydown(function (e) {
        $BackSpaceControl = (e.keyCode == 8 && (e.target || e.srcElement).nodeName.toLowerCase() != 'input') ? true : false;
    });

    $(window).keypress(function (event) {
        if (!(event.which == 115 && event.ctrlKey) && !(event.which == 19)) return true;
        if ($('.SaveBtn').length > 0) {
            $('.SaveBtn').trigger('click');
            event.preventDefault();
            return false;
        }        
    });

}
