var TT = {
    Util: {
        ParseDate: function (jsonDate) {
            //console.log('ParseDate', jsonDate);
            if (jsonDate) {
                var re = /-?\d+/;
                var m = re.exec(jsonDate);
                var d = new Date(parseInt(m[0]));
                return d;
            } else {
                return null;
            }
        },
        FormatDate: function (date) {
            // date is in UTC but datepicker takes local, so convert to local
            var local = new Date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate());
            var s = $.datepicker.formatDate('MM dd, yy - D', local);
            return s;
        },
        DatePart: function (date) {
            if (_.isDate(date)) return TT.Util.LocalToUTC(new Date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate()));
            else return null;
        },
        LocalToUTC: function (date) {
            date.setTime(date.getTime() - date.getTimezoneOffset() * 60 * 1000);
            return date;
        },
        SameDate: function (date1, date2) {
            if (_.isDate(date1) && _.isDate(date2))
                return date1.getUTCDate() == date2.getUTCDate() && date1.getUTCMonth() == date2.getUTCMonth() && date1.getUTCFullYear() == date2.getUTCFullYear();
            else
                return date1 == date2;
        },
        FromDatePicker: function (date) {
            // datepicker returns in local time, so subtract the offset
            date.setTime(date.getTime() - date.getTimezoneOffset() * 60 * 1000);
            return date;
        },
        ToDatePicker: function (date) {
            // date is in UTC but datepicker takes local, so convert to local
            var local = new Date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate());
            return local;
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