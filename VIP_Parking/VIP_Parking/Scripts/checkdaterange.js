$(function () {

    jQuery.validator.addMethod('checkdaterange', function (value, element, params) {
        var currentDate = new Date();
        if (Date.parse(value) >= currentDate) {
            return false;
        }
        return true;
    }, '');

    jQuery.validator.unobtrusive.adapters.add('checkdaterange', function (options) {
        options.rules['checkdaterange'] = {};
        options.messages['checkdaterange'] = options.message;
    });

}(jQuery));