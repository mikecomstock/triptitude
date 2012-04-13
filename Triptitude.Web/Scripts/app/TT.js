var TT = {
    Util: {
        ParseDate: function (jsonDate) {
            return jsonDate ? new Date(parseInt(jsonDate.substr(6))) : null;
        },
        FormatDate: function (date) {
            return $.datepicker.formatDate('MM dd, yy', date);
        },
        DatePart: function (date) {
            return !date ? date : new Date(date.getFullYear(), date.getMonth(), date.getDate());
        },
        SameDate: function (date1, date2) {
            if (_.isDate(date1) && _.isDate(date2))
                return date1.getTime() == date2.getTime();
            else
                return date1 == date2;
        }
    },
    Models: {},
    Collections: {},
    Helpers: {},
    Routers: {},
    Views: {
        Editor: {}
    },
    init: function () {
        //var header = new T.Views.Header({ el: $('#header') });
    }
};