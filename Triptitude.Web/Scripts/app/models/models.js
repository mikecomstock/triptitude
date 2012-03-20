TT.Models.Activity = Backbone.Model.extend({
    idAttribute: "ID",
    defaults: {
        'Title': 'some activity title'
    }
});

TT.Collections.Activities = Backbone.Collection.extend({ model: TT.Models.Activity });

TT.Models.Trip = Backbone.Model.extend({
    idAttribute: "ID"
});

TT.Collections.Trips = Backbone.Collection.extend({
    model: TT.Models.Trip
});

TT.Models.User = Backbone.Model.extend({
    idAttribute: "ID",
    getCurrentTrip: function () {
        return this.get('Trips').get(this.get('CurrentTripID'));
    }
});