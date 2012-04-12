var TT = {
    Util: {
        ParseDate: function (jsonDate) {
            return jsonDate ? new Date(parseInt(jsonDate.substr(6))) : null;
        },
        FormatDate: function (date) {
            return $.datepicker.formatDate('MM dd, yy', date);
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