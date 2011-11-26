var superDialog;

$(function () {

    moveScroller();
    $('input').placeholder();
    $('.focus').first().focus();
    $('.date-picker').datepicker();
    $('.place-autocomplete').googAutocomplete();
    $('.tag-autocomplete').tagAutocomplete();
    $('.item-autocomplete').itemAutocomplete();

    $('#search').submit(function (e) {
        var val = $('input[name="googreference"]', $(this)).val();
        if (val == '') e.preventDefault();
    });

    $('#trip-bar-menu li').hover(
        function () { $(this).children('ul').show(); },
        function () { $(this).children('ul').hide(); }
    );

    superDialog = $('#super-dialog');
    superDialogOverlay = $('#super-dialog-overlay');
    superDialogOverlay.click(function () { CloseSuperDialog(); });
    $('*').live('keyup', function (e) { if (e.which == 27) { CloseSuperDialog(); } });

    $('.confirm-delete').live('click', function (e) {
        var confirmed = confirm('Delete?');
        if (confirmed) {
            var data_url = $(this).data('url');
            window.location.replace(data_url);
        } else {
            e.preventDefault();
        }
    });

    $('.trip-row-map-link').click(function () {
        var tripId = $(this).data('trip-id');
        var name = $(this).data('trip-name');

        var container = $(document.createElement('div'));
        container.dialog({
            title: name,
            width: 540,
            height: 400,
            resizable: false
        });

        $.get('/maps/trip/' + tripId, function (mapData) {
            drawMap(container, mapData);
        });
    });

    $('.super-dialog-link').live('click', function (e) {
        e.preventDefault();
        var url = $(this).attr('href');
        OpenSuperDialog(url);
    });

    $('.packing-list-item.owned').click(function (e) {
        if (!$(e.target).is('a')) {
            var id = $(this).data('id');
            OpenSuperDialog('/packing/edit/' + id);
        }
    });

    $('.trip-day .activity.owned').click(function (e) {
        if (!$(e.target).is('a')) {
            var activityId = $(this).data('activity-id');
            OpenSuperDialog('/activities/edit/' + activityId);
        }
    });

    $('.add-to-trip').live('click', function (e) {

        if (e.target != this && $(e.target).is('a')) return;

        e.preventDefault();
        var link = $(this);
        var type = link.data('type');
        if (type == 'packing-list-item') {
            OpenSuperDialog('/packing/create', function () {
                superDialog.find('input[name="name"]').val(link.data('name'));
                superDialog.find('input[name="tagstring"]').val(link.data('tag'));
            });
        } else {
            var place = $(e.target).data('place');
            if (!place) {
                place = {
                    id: $(this).data('id'),
                    reference: $(this).data('reference'),
                    name: $(this).data('name')
                };
            }
            OpenSuperDialog('/activities/create?type=place', function () {
                var placeInputParagraph = superDialog.find('[name="name"]').parent();
                placeInputParagraph.find('input[name="googid"]').val(place.id);
                placeInputParagraph.find('input[name="googreference"]').val(place.reference);
                placeInputParagraph.hide();
                $(document.createElement('p')).addClass('place').text(place.name).insertBefore(placeInputParagraph);
            });
        }
    });
});

function OpenSuperDialog(url, callback) {
    $.get(url, function (result) {
        $('#trip-bar-menu li').children('ul').hide();
        superDialog.html(result);
        superDialog.show();
        superDialogOverlay.show();
        superDialog.find('.focus').focus();
        superDialog.find('input.day-input').attr('autocomplete', 'off');
        superDialog.find('.cancel').click(function (e) { e.preventDefault(); CloseSuperDialog(); });
        superDialog.find('.place-autocomplete').googAutocomplete();
        superDialog.find('.tag-autocomplete').tagAutocomplete();
        superDialog.find('.item-autocomplete').itemAutocomplete();

        scrollToBottom(superDialog.find('.notes'));

        if (callback) callback();

        superDialog.find('#dialog-menu li').click(function (e) {
            var dataPageName = $(this).data('page');
            var currentPage = $('li, .dialog-page');
            currentPage.removeClass('selected-page');
            var newPage = $('[data-page="' + dataPageName + '"]', superDialog);
            newPage.addClass('selected-page');
            $('.focus', newPage).first().focus();
            scrollToBottom($('.notes', superDialog));
        });

        superDialog.find('#note-dialog select').change(function () {
            var select = $(this);
            var activityId = select.val();
            OpenSuperDialog('/activities/edit/' + activityId + '?selectedtab=notes');
        });

        superDialog.find('form').submit(function (e) {
            e.preventDefault();
            var form = $(this);

            var requiredFields = form.find('.required');
            requiredFields.each(function () {
                var field = $(this);
                if (field.val().length == 0) {
                    field.parent().addClass('invalid');
                } else {
                    field.parent().removeClass('invalid');
                }
            });

            var invalidFields = form.find('.invalid');

            if (invalidFields.size() > 0) {
                var firstField = invalidFields.first().find('input[type="text"]');
                firstField.focus();
            } else {
                var formData = form.serialize();
                var formAction = form.prop('action');
                $.post(formAction, formData)
                    .success(function (response) {
                        superDialog.data('changes', true);
                        if (response.replace) {
                            superDialog.html(response.replace);
                        } else {
                            var onPlanningPage = $('#trips-details, #trips-packinglist').size() > 0;
                            if (onPlanningPage) {
                                location.reload();
                            } else if (response.redirect) {
                                window.location.href = response.redirect;
                            } else {
                                $('.buttons').html('<div class="saved">Saved!</div>');
                                setTimeout(function () { CloseSuperDialog(); }, 1000);
                            }
                        }
                    });
            }
        });
    });
}

function CloseSuperDialog() {
    if (!superDialog.is(':visible')) return;

    var currentlyViewingTripId = $('body').data('id');
    var currentlyEditingTripId = $('#trip-bar').data('trip-id');

    if (superDialog.data('changes') && (currentlyViewingTripId == currentlyEditingTripId)) {
        location.reload();
    } else {
        superDialog.data('changes', false);
        superDialog.hide();
        superDialogOverlay.hide();
        superDialog.empty();
    }
}

(function ($) {
    $.fn.tagAutocomplete = function () {
        this.each(function () {
            var input = $(this);
            input.autocomplete({
                source: function (request, response) {
                    $.getJSON('/tags/search', request, function (data) {
                        response(data);
                    });
                }
            });
        });
    };
    $.fn.itemAutocomplete = function () {
        this.each(function () {
            var input = $(this);
            input.autocomplete({
                source: function (request, response) {
                    $.getJSON('/items/search', request, function (data) {
                        response(data);
                    });
                }
            });
        });
    };
})(jQuery);

(function ($) {
    $.fn.googAutocomplete = function () {
        this.each(function () {
            var $input = $(this);
            var $googReferenceField = $('[name="' + $input.data('goog-reference-field') + '"]');
            var $googIdField = $('[name="' + $input.data('goog-id-field') + '"]');
            var $googNameField = $('[name="' + $input.data('goog-name-field') + '"]');
            var mapDiv = $input.data('map-id') ? $('#' + $input.data('map-id')) : null;

            $input.keypress(function (e) { if (e.which == 13) e.preventDefault(); });

            $input.change(function () {
                if ($.trim($input.val()) == '' || $input.val() != $googNameField.val()) {
                    $googIdField.val('');
                    $googReferenceField.val('');
                    $googNameField.val('');
                    if (mapDiv) mapDiv.attr('src', '');
                    setTimeout(function () { $input.val(''); }, 200);
                }
            });

            $input.focus(function () {
                if (mapDiv && mapDiv.attr('src') != '') {
                    mapDiv.parent().show();
                    mapDiv.parent().position({ my: 'left center', at: 'right center', of: $input, offset: '10px 0' });
                }
            });

            $input.blur(function () {
                if (mapDiv) {
                    mapDiv.parent().hide();
                }
            });

            var autocomplete = new google.maps.places.Autocomplete($input.get(0));
            google.maps.event.addListener(autocomplete, 'place_changed', function () {

                var place = autocomplete.getPlace();

                $googReferenceField.val(place.reference);
                $googIdField.val(place.id);
                $googNameField.val($input.val());

                var autosubmit = $input.data('auto-submit');
                if (autosubmit) {
                    $input.closest('form').submit();
                } else {
                    if (mapDiv) {
                        var href = 'http://maps.googleapis.com/maps/api/staticmap?sensor=false';
                        href += '&size=' + mapDiv.width() + 'x' + mapDiv.height();
                        href += '&center=' + place.geometry.location.toUrlValue();
                        href += '&markers=size:mid%7Ccolor:blue%7C' + place.geometry.location.toUrlValue();
                        if (place.geometry.viewport) {
                            href += '&visible=' + place.geometry.viewport.getSouthWest().toUrlValue() + '|' + place.geometry.viewport.getNorthEast().toUrlValue();
                        } else {
                            href += '&zoom=15';
                        }
                        mapDiv.attr('src', href);
                        mapDiv.parent().show();
                        mapDiv.parent().position({ my: 'left center', at: 'right center', of: $input, offset: '10px 0' });
                    }
                }
            });
        });
    };
})(jQuery);

function drawMap(container, mapData) {

    var myOptions = { mapTypeId: google.maps.MapTypeId.ROADMAP };
    var map = new google.maps.Map(container.get(0), myOptions);
    var bounds = new google.maps.LatLngBounds();
    var infoWindow = new google.maps.InfoWindow();

    var directionsPolylineOptions = { strokeColor: "#0066FF", strokeOpacity: 0.6, strokeWeight: 5 };
    var directionsService = new google.maps.DirectionsService();

    $.each(mapData.markers, function (i, item) {

        var point = new google.maps.LatLng(item.Latitude, item.Longitude);
        var marker = new google.maps.Marker({ position: point, map: map, title: item.Name });
        marker.infoHtml = item.InfoHtml;
        if (item.ExtendBounds == true) bounds.extend(point);

        google.maps.event.addListener(marker, 'click', function () {
            infoWindow.setContent(marker.infoHtml);
            infoWindow.open(map, marker);
        });
    });

    $.each(mapData.polyLines, function (i, item) {

        var fromPoint = new google.maps.LatLng(item.From.Latitude, item.From.Longitude);
        var toPoint = new google.maps.LatLng(item.To.Latitude, item.To.Longitude);

        if (item.From.ExtendBounds == true) bounds.extend(fromPoint);
        if (item.To.ExtendBounds == true) bounds.extend(toPoint);

        if (item.PathType == 'directions') {
            var request = { origin: fromPoint, destination: toPoint, travelMode: google.maps.DirectionsTravelMode.DRIVING };
            directionsService.route(request, function (result, status) {
                var directionsDisplay = new google.maps.DirectionsRenderer({ suppressInfoWindows: true, suppressMarkers: true, polylineOptions: directionsPolylineOptions, preserveViewport: true });
                directionsDisplay.setMap(map);
                if (status == google.maps.DirectionsStatus.OK) {
                    directionsDisplay.setDirections(result);
                }
            });
        } else {
            var transPath = new google.maps.Polyline({ path: [fromPoint, toPoint], strokeColor: "#0066FF", strokeOpacity: 0.6, strokeWeight: 5, geodesic: true });
            transPath.setMap(map);
        }

    });

    map.fitBounds(bounds);
}

function moveScroller() {
    var a = function () {
        var b = $(window).scrollTop();
        var sa = $("#scrollanchor");
        var d = sa.offset().top;
        var c = $("#trip-bar-container");
        if (b > d) {
            var height = c.outerHeight(true);
            sa.css({ height: height + "px" });
            c.addClass('at-top');
        } else {
            if (b <= d) {
                c.removeClass('at-top');
                sa.css({ height: "0" });
            }
        }
    };
    $(window).scroll(a);
    a();
}

function scrollToBottom($element) {
    if ($element) {
        $element.prop({ scrollTop: $element.prop('scrollHeight') });
    }
}

log = function (a) {
    if (window['console'] && console['log']) {
        console.log(a);
    }
};

var T = {};

T.NearbyPlaces = function () {

    var map;
    var searchService;
    var searchForm = $('#placeSearch');
    var infowindow;
    var placeList = $('.place-rows');
    var noResults;

    function initialize() {
        var m = $('#map');
        var lat = m.data('lat');
        var lng = m.data('lng');
        var centerOn = new google.maps.LatLng(lat, lng);

        map = new google.maps.Map(document.getElementById('map'), {
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            center: centerOn,
            zoom: 14
        });
        google.maps.event.addListener(map, 'idle', function () { doSearch(); });

        searchService = new google.maps.places.PlacesService(map);

        infowindow = new google.maps.InfoWindow({ zIndex: 10000 });
        google.maps.event.addListener(infowindow, 'closeclick', function () { clearActive(); });

        searchForm.submit(function (e) {
            e.preventDefault();
            clearActive();
            doSearch();
        });
    }

    function doSearch() {
        var keyword = searchForm.find('#keyword').val();
        var options = { bounds: map.getBounds(), keyword: keyword };
        searchService.search(options, function (results, status) {

            if (noResults) {
                noResults.remove();
                noResults = null;
            }

            placeList.find('li:not(.active)').each(function (i, li) {
                var $li = $(li);
                var place = $li.data('place');
                place.marker.setMap(null);
                $li.remove();
            });

            switch (status) {
                case google.maps.places.PlacesServiceStatus.OK:
                    {
                        var activePlace = placeList.find('li.active').data('place');
                        $.each(results, function (i, place) {
                            // Can't compare objects here, use IDs instead.
                            if (activePlace == null || place.id != activePlace.id) {
                                createMarker(place);
                                createListItem(place);
                            }
                        });
                        break;
                    }
                case google.maps.places.PlacesServiceStatus.ZERO_RESULTS:
                    {
                        noResults = $(document.createElement('div')).text('No results found');
                        noResults.insertAfter(placeList);
                        break;
                    }
                default:
                    {
                        noResults = $(document.createElement('div')).text('An error has occured. Please edit your search and try again.');
                        noResults.insertAfter(placeList);
                        break;
                    }
            }
        });
    }

    function createMarker(place) {
        var marker = new google.maps.Marker({ map: map, position: place.geometry.location });
        google.maps.event.addListener(marker, 'click', function () { setActive(place); });
        place.marker = marker;
    }

    function createListItem(place) {
        var li = $(document.createElement('li')).appendTo(placeList)
            .click(function (e) { if (e.target == this) { setActive(place); } })
            .data('place', place);
        place.li = li;
        $('<a class="title" rel="nofollow"></a>')
            .text(place.name)
            .data('place', place)
            .attr('href', '/places/redirect?googReference=' + place.reference + '&googId=' + place.id)
            .appendTo(li);
        $('<a class="add-to-trip" rel="nofollow">+ Add to Trip</a>').data('place', place).appendTo(li);
    }

    function getInfoWindowContent(place) {
        var content = $(document.createElement('div')).addClass('info-window');
        $('<a class="title" rel="nofollow"></a>')
            .text(place.name)
            .data('place', place)
            .attr('href', '/places/redirect?googReference=' + place.reference + '&googId=' + place.id)
            .appendTo(content);
        $('<a class="add-to-trip" rel="nofollow">+ Add to Trip</a>').data('place', place).appendTo(content);
        return content;
    }

    function setActive(place) {
        clearActive();
        placeList.find('li.active').removeClass('active');
        place.li.addClass('active');

        var infoWindowContent = getInfoWindowContent(place);
        infowindow.setContent(infoWindowContent[0]);
        infowindow.open(map, place.marker);
    }

    function clearActive() {
        infowindow.close();
        placeList.find('.active').removeClass('active');
    }

    var mapPinning = function () {
        var b = $(window).scrollTop() + 90;
        var sa = $("#mapanchor");
        var d = sa.offset().top;
        var c = $("#map");
        c.width(c.width()); // fixes an overlay issue
        if (b > d) {
            var height = c.outerHeight(true);
            sa.css({ height: height + "px" });
            c.addClass('at-top');
        } else {
            if (b <= d) {
                c.removeClass('at-top');
                sa.css({ height: "0" });
            }
        }
    };

    google.maps.event.addDomListener(window, 'load', initialize);
    $(window).scroll(mapPinning);
    mapPinning();
};