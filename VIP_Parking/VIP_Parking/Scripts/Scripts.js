$(function () {
    $(".datepicker").datepicker({
        showOn: "button",
        buttonImage: "http://jqueryui.com/resources/demos/datepicker/images/calendar.gif",
        buttonImageOnly: true,
        minDate: 0
    });
    $("#decline").click(function () {
        var c = confirm("Are you sure you want to decline this reservation?");
        if (!c)
            return false;
    });
    $("#approve").click(function () {
        var c = confirm("Are you sure you want to approve this reservation?");
        if (!c)
            return false;
    });
    $(".datepicker_anystart").datepicker({
        showOn: "button",
        buttonImage: "http://jqueryui.com/resources/demos/datepicker/images/calendar.gif",
        buttonImageOnly: true
    });
    
    var clipboard = new Clipboard('.btn');

    clipboard.on('success', function (e) {
        $('#clipboard_status').text("Table was copied to clipboard");
        e.clearSelection();
    });

    clipboard.on('error', function (e) {
        $('#clipboard_status').text("Table was not copied to clipboard. Please try again.");
    });
});