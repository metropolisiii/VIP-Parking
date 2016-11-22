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
});