TT.Models.Activity = Backbone.Model.extend({
    idAttribute: 'ID',
    urlRoot: '/activities',
    defaults: { Title:'', 'OrderNumber': 0 },
    initialize: function () {
        this.set('BeginAt', TT.Util.ParseDate(this.get('BeginAt')));
        this.set('Places', new TT.Collections.Places(this.get('Places')));
        this.set('Notes', new TT.Collections.Notes(this.get('Notes')));
    },
    parse: function (resp, xhr) {
        resp.Places = new TT.Collections.Places(resp.Places);
        resp.Notes = new TT.Collections.Notes(resp.Notes);
        resp.BeginAt = TT.Util.ParseDate(resp.BeginAt);
        //        console.log('activity.parse', 'resp:', resp, 'xhr:', xhr);
        return resp;
    },
    createTitle: function () {
        if (this.get('Title'))
            return this.get('Title');

        if (this.get('IsTransportation')) {
            var title = this.get('TransportationTypeName') ? this.get('TransportationTypeName') : 'Transportation';
            if (this.has('Places'))
                this.get('Places').each(function (place) {
                    title += place.get('SortIndex') == 0 ? ' from ' : ' to ';
                    title += place.get('Name');
                });
            return title;
        }
        return 'Untitled Activity';
    }
});

TT.Collections.Activities = Backbone.Collection.extend({
    model: TT.Models.Activity,
    url: '/activities',
    comparator: function (activity) {
        return activity.get('OrderNumber');
    },
    moveToEnd: function (activity) {
        activity.set('OrderNumber', 0, { silent: true });
        var activityDate = activity.get('BeginAt');
        var activitiesOnDate = this.onDate(activityDate);
        var maxOrder = activitiesOnDate.length == 0 ? 0 : _.max(activitiesOnDate, function (a) { return a.get('OrderNumber'); }).get('OrderNumber');
        activity.set('OrderNumber', maxOrder + 1);
    },
    dates: function () {

        var allBeginAts = this.pluck('BeginAt');
        var nonNull = _.reject(allBeginAts, function (d) { return !d; });
        var min = TT.Util.DatePart(_.min(nonNull)),
            max = TT.Util.DatePart(_.max(nonNull));

        var dates = [];
        for (var d = new Date(min); d <= max; d.setDate(d.getDate() + 1)) {
            dates.push(new Date(d));
        }

        var hasNull = !_.isEmpty(_.filter(allBeginAts, function (d) { return d == null; }));
        if (hasNull) dates.push(null);

        return dates;
    },
    onDate: function (d) {
        return this.models.filter(function (activity) {
            var aDate = activity.get('BeginAt');
            return TT.Util.SameDate(d, aDate);
        });
    }
});

TT.Models.Place = Backbone.Model.extend({
    idAttribute: 'ID'
});

TT.Collections.Places = Backbone.Collection.extend({ model: TT.Models.Place });

TT.Models.Note = Backbone.Model.extend({ idAttribute: 'ID' });
TT.Collections.Notes = Backbone.Collection.extend({ model: TT.Models.Note });

TT.Models.Trip = Backbone.Model.extend({
    idAttribute: 'ID',
    urlRoot: '/trips',
    initialize: function () {
        // convert object array into collection
        this.set('Activities', new TT.Collections.Activities(this.get('Activities')));
    },
    parse: function (resp, xhr) {
        resp.Activities = new TT.Collections.Activities(resp.Activities);
        return resp;
    }
});

TT.Collections.Trips = Backbone.Collection.extend({
    model: TT.Models.Trip
});

TT.Models.User = Backbone.Model.extend({
    idAttribute: 'ID',
    initialize: function () {
        //TODO: do this same thing in parse
        // convert object array into collection
        this.set('Trips', new TT.Collections.Trips(this.get('Trips')));
    },
    getCurrentTrip: function () {
        return this.get('Trips').get(this.get('DefaultTripID'));
    },
    ownsActivity: function (activity) {
        console.log('user.ownsActivity', activity);
        var tripId = activity.get('Trip').id;
        console.log('tripId:', tripId);
        return true;
    }
});