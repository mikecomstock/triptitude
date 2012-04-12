TT.Models.Activity = Backbone.Model.extend({
    idAttribute: 'ID',
    urlRoot: '/activities',
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
        return 'created title';
    }
});

TT.Collections.Activities = Backbone.Collection.extend({
    model: TT.Models.Activity,
    url: '/activities',
    comparator: function (activity) {
        return activity.get('OrderNumber');
    },
    dates: function () {

        var allBeginAts = this.pluck('BeginAt');
        var nonNull = _.reject(allBeginAts, function (d) { return !d; });
        var min = _.min(nonNull), max = _.max(nonNull);
        min = new Date(min.getFullYear(), min.getMonth(), min.getDate());
        max = new Date(max.getFullYear(), max.getMonth(), max.getDate());

        var dates = [];
        for (var d = new Date(min); d <= max; d.setDate(d.getDate() + 1)) {
            dates.push(new Date(d));
        }

        var hasNull = !_.isEmpty(_.filter(allBeginAts, function (d) { return d == null; }));
        if (hasNull) dates.push(null);

        return dates;
    },
    onDate: function (d) {

        if (!d) {
            return this.models.filter(function (activity) {
                var aDate = activity.get('BeginAt');
                return !aDate;
            });
        }

        var year = d.getFullYear(), month = d.getMonth(), date = d.getDate();
        return this.models.filter(function (activity) {
            var aDate = activity.get('BeginAt');
            return aDate ? aDate.getFullYear() == year && aDate.getMonth() == month && aDate.getDate() == date : false;
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
        var activitiesCollection = new TT.Collections.Activities(this.get('Activities'));
        this.set('Activities', activitiesCollection);
    },
    parse: function (resp, xhr) {
        var activitiesCollection = new TT.Collections.Activities(resp.Activities);
        resp.Activities = activitiesCollection;
        return resp;
    }
});

TT.Collections.Trips = Backbone.Collection.extend({
    model: TT.Models.Trip
});

TT.Models.User = Backbone.Model.extend({
    idAttribute: 'ID',
    initialize: function () {

        //TODO: move this to parse, and call parse from init
        // convert object array into collection
        var tripsCollection = new TT.Collections.Trips(this.get('Trips'));
        this.set('Trips', tripsCollection);
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