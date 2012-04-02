TT.Models.Activity = Backbone.Model.extend({
    idAttribute: 'ID',
    urlRoot: '/activities',
    initialize: function () {
        //this.parse();
        //        var placeCollection = new TT.Collections.Places(this.get('Places'));
        //        this.set('Places', placeCollection);
    },
    parse: function (resp, xhr) {
        console.log('activity.parse', 'resp:', resp, 'xhr:', xhr);

        //        var placeCollection = new TT.Collections.Places(resp.Places);
        //        this.set('Places', placeCollection);

        //        delete resp.Places;
        return resp;
    },
    createTitle: function () {
        if (this.get('Title'))
            return this.get('Title');

        if (this.get('IsTransportation')) {
            var title = this.get('TransportationTypeName') ? this.get('TransportationTypeName') : 'Transportation';
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
    url: '/activities'
});

TT.Models.Place = Backbone.Model.extend({
    idAttribute: 'ID'
});

TT.Collections.Places = Backbone.Collection.extend({ model: TT.Models.Place });

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