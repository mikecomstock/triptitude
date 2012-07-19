TT.Views.PlaceSearchDialog = Backbone.View.extend({
    initialize: function () {

        var geocoder = new google.maps.Geocoder();

        var input = $('<input>').appendTo(this.el);
        var source = function (request, response) {
            geocoder.geocode({ 'address': request.term }, function (results, status) {

                results.forEach(function (r) {
                    r.label = r.formatted_address;
                });

                response(results);
            });
        };

        input.autocomplete({ source: source, autoFocus: true });
    },
    events: {
        'autocompleteselect': 'autocompleteselect'
    },
    autocompleteselect: function (event, ui) {
        this.$el.data('item', ui.item);
        this.trigger('select', ui.item);
    },
    render: function () {

        var m = $('<div style="height: 300px;">map goes here</div>').appendTo(this.el);

        var mapOptions = {
            center: new google.maps.LatLng(-34.397, 150.644),
            zoom: 8,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        var map = new google.maps.Map(m[0], mapOptions);

        /////////////////
        var pyrmont = new google.maps.LatLng(-33.8665433, 151.1956316);
        var request = {
            location: pyrmont,
            radius: '500',
            query: 'starbucks near san francisco'
        };

        service = new google.maps.places.PlacesService(map);

        service.textSearch(request, function (results, status) {
            console.log('results', results);
            if (status == google.maps.places.PlacesServiceStatus.OK) {
                for (var i = 0; i < results.length; i++) {
                    var place = results[i];
//                    createMarker(results[i]);
                }
            }
        });

        return this;
    }
});