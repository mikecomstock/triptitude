var superDialog;

$(function () {

    /****************/
    /* Super Dialog */
    /****************/

    superDialog = $('#super-dialog');
    superDialogOverlay = $('#super-dialog-overlay');
    superDialogOverlay.click(function () { superDialog.hide(); superDialogOverlay.hide(); });
    $('.cancel', superDialog).live('click', function (e) { e.preventDefault(); superDialog.hide(); superDialogOverlay.hide(); });
    $('*').live('keyup', function (e) { if (e.which == 27) { superDialog.hide(); superDialogOverlay.hide(); } });

    $('#dialog-menu li', superDialog).live('click', function (e) {
        var dataPageName = $(this).attr('data-page');
        var currentPage = $('li, .dialog-page');
        currentPage.removeClass('selected-page');
        var newPage = $('[data-page="' + dataPageName + '"]', superDialog);
        newPage.addClass('selected-page');
        $('.focus', newPage).first().focus();
        scrollToBottom($('.notes', superDialog));
    });

    /* Delete Confirmations */
    $('.confirm-delete').live('click', function (e) {
        var confirmed = confirm('Delete?');
        if (confirmed) {
            var data_url = $(this).attr('data-url');
            window.location.replace(data_url);
        } else {
            e.preventDefault();
        }
    });

    $('.trip-row-map-link').click(function () {
        var tripId = $(this).attr('data-trip-id');
        var name = $(this).attr('data-trip-name');

        var container = $('<div></div>');
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

    $('#note-dialog select', superDialog).live('change', function () {
        var select = $(this);
        var activityId = select.val();
        CreateActivityModal('note', '/activities/edit/' + activityId + '?selectedtab=notes');
    });

    $('.add-activity').live('click', function () {
        var activityType = $(this).attr('data-activity-type');
        var url = '/activities/create?type=' + activityType;

        switch (activityType) {
            case 'transportation':
                break;
            case 'place':
                var referenceId = $(this).attr('data-reference-id') || '';
                url += '&referenceid=' + referenceId;
                break;
            case 'hotel':
                var hotelId = $(this).attr('data-hotel-id');
                url += '&hotelid=' + hotelId;
                break;
        }
        CreateActivityModal(activityType, url);
    });

    $('.add-note').live('click', function () {
        CreateActivityModal("note", '/notes/create');
    });

    $('.add-packing-item').live('click', function () {
        CreateActivityModal("packing-item", '/packing/create');
    });

    $('.packing-list-item.owned').click(function () {
        var id = $(this).data('id');
        CreateActivityModal("packing-item", '/packing/edit/' + id);
    });

    $('.trip-day .activity.owned').click(function () {
        var activityId = $(this).attr('data-activity-id');
        var activityType = $(this).attr('data-activity-type');
        CreateActivityModal(activityType, '/activities/edit/' + activityId);
    });

    $('.add-to-trip').live('click', function (e) {
        e.preventDefault();
        var place = $(e.target).data('place');
        if (!place) {
            place = {
                id: $(this).data('id'),
                reference: $(this).data('reference'),
                name: $(this).data('name')
            };
        }
        CreateActivityModal('place', '/activities/create?type=place', place);
    });
});

function BindPlaceAutocomplete(context) {
    var a = $('.place-autocomplete', context);
    a.googAutocomplete();
}

function CreateActivityModal(activityType, url, place) {
    $.get(url, function (data) {
        $('#trip-bar-menu li').children('ul').hide();
        $('.content', superDialog).html(data);
        superDialog.show();
        superDialogOverlay.show();
        $('.focus', superDialog).focus();
        $('input.day-input', superDialog).attr('autocomplete', 'off');
        BindPlaceAutocomplete(superDialog);
        scrollToBottom($('.notes', superDialog));

        if (place) {
            $('input[name="name"]', superDialog).val(place.name);
            $('input[name="googid"]', superDialog).val(place.id);
            $('input[name="googreference"]', superDialog).val(place.reference);
        }
    });
}

(function ($) {

    $.fn.googAutocomplete = function () {
        this.each(function () {
            var $input = $(this);
            $input.keypress(function (e) { if (e.which == 13) e.preventDefault(); });
            var autocomplete = new google.maps.places.Autocomplete($input.get(0));

            google.maps.event.addListener(autocomplete, 'place_changed', function () {
                var place = autocomplete.getPlace();

                var $googReferenceField = $('[name="' + $input.attr('data-goog-reference-field') + '"]');
                var $googIdField = $('[name="' + $input.attr('data-goog-id-field') + '"]');

                $googReferenceField.val(place.reference);
                $googIdField.val(place.id);

                var autosubmit = $input.attr('data-auto-submit') == 'true';
                if (autosubmit) $input.closest('form').submit();
            });

            var mapId = $input.attr('data-map-id');
            if (mapId) {
                $mapDiv = $('#' + mapId);

                var center = new google.maps.LatLng(25, -30);
                var map = new google.maps.Map($mapDiv.get(0), { mapTypeId: google.maps.MapTypeId.ROADMAP, center: center, zoom: 1 });
                var marker = new google.maps.Marker({ map: map });
                autocomplete.bindTo('bounds', map);

                // var currLat = $('#latitude', superDialog).val();
                // var currLng = $('#longitude', superDialog).val();
                // var point = new google.maps.LatLng(currLat, currLng);
                // var marker = new google.maps.Marker({ position: point, map: map });

                google.maps.event.addListener(autocomplete, 'place_changed', function () {
                    var place = autocomplete.getPlace();
                    if (place.geometry.viewport) {
                        map.fitBounds(place.geometry.viewport);
                    } else {
                        map.setCenter(place.geometry.location);
                        map.setZoom(16);
                    }

                    marker.setPosition(place.geometry.location);
                });

            }
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


/**
* Equal Heights Plugin
* Equalize the heights of elements. Great for columns or any elements
* that need to be the same size (floats, etc).
* 
* Version 1.0
* Updated 12/10/2008
*
* Copyright (c) 2008 Rob Glazebrook (cssnewbie.com) 
*
* Usage: $(object).equalHeights([minHeight], [maxHeight]);
* 
* Example 1: $(".cols").equalHeights(); Sets all columns to the same height.
* Example 2: $(".cols").equalHeights(400); Sets all cols to at least 400px tall.
* Example 3: $(".cols").equalHeights(100,300); Cols are at least 100 but no more
* than 300 pixels tall. Elements with too much content will gain a scrollbar.
* 
*/

//(function ($) {
//    $.fn.equalHeights = function (minHeight, maxHeight) {
//        tallest = (minHeight) ? minHeight : 0;
//        this.each(function () {
//            if ($(this).height() > tallest) {
//                tallest = $(this).height();
//            }
//        });
//        if ((maxHeight) && tallest > maxHeight) tallest = maxHeight;
//        return this.each(function () {
//            $(this).height(tallest).css("overflow", "auto");
//        });
//    };
//})(jQuery);

/***********************************************************/

function scrollToBottom($element) {
    if ($element) {
        $element.prop({ scrollTop: $element.prop('scrollHeight') });
    }
}