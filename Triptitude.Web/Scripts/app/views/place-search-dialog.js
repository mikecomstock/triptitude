TT.Views.PlaceSearchDialog = Backbone.View.extend({
    initialize: function () {

        this.iconSettings = {
            path: google.maps.SymbolPath.CIRCLE,
            scale: 5,
            fillColor: '#81BC2E',
            fillOpacity: 1,
            strokeOpacity: 1,
            strokeColor: '#81BC2E'
        };

        this.selectedIconSettings = {
            path: google.maps.SymbolPath.CIRCLE,
            scale: 5,
            fillColor: '#0190E9',
            fillOpacity: 1,
            strokeOpacity: 1,
            strokeColor: '#0190E9'
        };

        _.bindAll(this);
    },
    events: {
        'submit': 'submitSearchForm',
        'click .selection': 'placeSelected'
    },
    placeSelected: function () {
        this.trigger('place-selected', this.activePlace);
    },
    submitSearchForm: function (e) {
        e.preventDefault();

        var search = this.$el.find('[name="search"]').val();
        var near = this.$el.find('[name="near"]').val();
        var query = search + (near ? ' near ' + near : '');

        if (query) {
            var searchRequest = { bounds: this.map.getBounds(), query: query };
            this.doSearch(searchRequest);
        }
    },
    render: function () {

        var self = this;

        require(['text!/Templates/place-dialog.html'], function (template) {

            self.$el.html(template);
            self.$el.find('[name="search"]').focus();

            var m = self.$el.find('.map');

            var mapOptions = {
                zoom: 1,
                center: new google.maps.LatLng(0, 0),
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };

            self.map = new google.maps.Map(m[0], mapOptions);
            self.service = new google.maps.places.PlacesService(self.map);

        });

        return this;
    },
    doSearch: function (request) {

        var self = this;

        this.service.textSearch(request, function (results, status) {

            self.removePlaces();

            if (status == google.maps.places.PlacesServiceStatus.OK) {

                var allBounds = new google.maps.LatLngBounds();

                for (var i = 0; i < results.length; i++) {
                    var place = results[i];
                    allBounds.extend(place.geometry.location);
                    self.addPlace(place);
                }

                if (results.length == 1) {

                    if (results[0].geometry.viewport) {
                        self.map.fitBounds(results[0].geometry.viewport);
                    } else {
                        self.map.setCenter(results[0].geometry.location);
                        self.map.setZoom(7);
                    }
                    self.setActivePlace(results[0]);

                } else {
                    self.map.fitBounds(allBounds);
                }

            }

        });

    },
    liTemplate: _.template('<li> <strong><%= name %></strong> <%= formatted_address %>  </li>'),
    markersArray: [],
    removePlaces: function () {
        this.$el.find('.place-list li').remove();

        for (i in this.markersArray) {
            this.markersArray[i].setMap(null);
        }
        this.markersArray.length = 0;

        this.activePlace = null;
        this.$el.find('.buttons').hide();
    },
    addPlace: function (place) {
        var self = this;

        var marker = new google.maps.Marker({
            map: this.map,
            position: place.geometry.location,
            icon: this.iconSettings
        });
        place.marker = marker;
        this.markersArray.push(marker);

        var placeList = this.$el.find('.place-list');

        var li = $(this.liTemplate(place)).appendTo(placeList);
        place.li = li;
        li.on('click', function (e) {
            self.setActivePlace(place);
        });

        google.maps.event.addListener(marker, 'click', function () {
            self.setActivePlace(place);
        });

    },
    setActivePlace: function (place) {

        if (this.activePlace == place) {
            this.map.setCenter(place.geometry.location);
            return;
        }

        if (this.activePlace) {
            this.activePlace.marker.setIcon(this.iconSettings);
            this.activePlace.li.removeClass('selected');
        }
        this.activePlace = place;

        place.li.addClass('selected');
        var buttons = this.$el.find('.buttons');
        buttons.find('.selection').html('Use <em>' + place.name + '</em>');
        buttons.show();

        place.marker.setIcon(this.selectedIconSettings);

        if (!this.map.getBounds().contains(place.marker.getPosition())) {
            this.map.setCenter(place.geometry.location);
        }

    }
});