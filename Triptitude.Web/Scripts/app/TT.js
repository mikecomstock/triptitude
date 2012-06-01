var TT = {
    Models: {},
    Collections: {},
    Helpers: {},
    Routers: {},
    Views: {
        Editor: {}
    },
    ClickEventType: ((document.ontouchstart !== null) ? 'click' : 'touchstart'),
    init: function () { }
};

TT.Util = {
    ParseDate: function(jsonDate) {
        if (jsonDate) {
            var re = /-?\d+/ ;
            var m = re.exec(jsonDate);
            var d = new Date(parseInt(m[0]));
            return d;
        } else {
            return null;
        }
    },
    FormatDate: function(date) {
        // date is in UTC but datepicker takes local, so convert to local
        var local = new Date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate());
        var s = $.datepicker.formatDate('MM dd, yy - D', local);
        return s;
    },
    DatePart: function(date) {
        if (_.isDate(date)) return TT.Util.LocalToUTC(new Date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate()));
        else return null;
    },
    LocalToUTC: function(date) {
        date.setTime(date.getTime() - date.getTimezoneOffset() * 60 * 1000);
        return date;
    },
    SameDate: function(date1, date2) {
        if (_.isDate(date1) && _.isDate(date2))
            return date1.getUTCDate() == date2.getUTCDate() && date1.getUTCMonth() == date2.getUTCMonth() && date1.getUTCFullYear() == date2.getUTCFullYear();
        else
            return date1 == date2;
    },
    FromDatePicker: function(date) {
        if (!date) return;
        // datepicker returns in local time, so subtract the offset
        date.setTime(date.getTime() - date.getTimezoneOffset() * 60 * 1000);
        return date;
    },
    ToDatePicker: function(date) {
        if (!date) return;
        // date is in UTC but datepicker takes local, so convert to local
        var local = new Date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate());
        return local;
    },
    ShowOverlay: function() {
        $('.overlay').remove();
        var overlay = $('<div>').addClass('overlay').appendTo($('body'));
        overlay.on(TT.ClickEventType, function(e) {
            if ($(e.target).hasClass('overlay')) overlay.remove();
        });
        return overlay;
    }
};

TT.Util.PL = {
    Hide: function(element) {
        element.addClass('hidden');
        element.hide();
    },
    Show: function(element) {
        element.removeClass('hidden');
        element.show();
    },
    PackingListClick: function(e) {
        e.preventDefault();
        var el = $(e.currentTarget).closest('li');
        var tag = el.data('tag');
        var item = el.data('item');
        var currentQuantity = parseInt(el.data('quantity'));
        var newQuantity = $(e.currentTarget).is('.inc') ? currentQuantity + 1 : currentQuantity - 1;
        if (newQuantity < 0) newQuantity = 0;

        el.data('quantity', newQuantity);
        el.find('.quantity').text(newQuantity);

        var data = { tag: tag, item: item, quantity: newQuantity };

        var pi = new Backbone.Model();
        pi.urlRoot = '/packing';
        pi.save(data, {
            error: function() {
                alert('An error has occured!');
                el.data('quantity', currentQuantity);
                el.find('.quantity').text(currentQuantity);
            }
        });
    }
};