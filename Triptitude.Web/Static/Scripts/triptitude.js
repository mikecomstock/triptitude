﻿$(function () {

    $('input').placeholder();

    BindDestinationAutocomplete(null);

    $('.confirm-delete').click(function (e) {
        var test = confirm('Delete?');
        if (!test) { e.preventDefault(); }
    });

    $('#search').submit(function (e) {
        var destinationId = $('#search input[name="destinationid"]').val();
        if (destinationId == '') {
            e.preventDefault();
        }
    });

    $('#trip-bar-menu li').hover(function (hoverData) {
        $(this).children('ul').show();

    }, function (hoverData) {
        $(this).children('ul').hide();
    });

    $('.add-transportation-link').click(function (clickData) {
        var tripId = $(this).attr('data-trip-id');
        $.get('/itineraryitems/addtransportation?tripid=' + tripId, function (getData) {
            CreateTransportationsModal(getData);
        });
    });

    $('.add-website-link').click(function (clickData) {
        var tripId = $(this).attr('data-trip-id');
        $.get('/itineraryitems/addwebsite?tripid=' + tripId, function (getData) {
            CreateWebsiteModal(getData);
        });
    });

    $('.add-tag-link').click(function (clickData) {
        var tripId = $(this).attr('data-trip-id');
        $.get('/itineraryitems/adddestinationtag?tripid=' + tripId, function (getData) {
            CreateDestinationTagModal(getData);
        });
    });

    $('.add-hotel-link').click(function (clickData) {
        var hotelId = $(this).attr('data-hotel-id');
        $.get('/itineraryitems/addhotel?hotelid=' + hotelId, function (data) {
            CreateHotelModal(data);
        });
    });

    $('.trip-row-map-link').click(function () {
        var tripId = $(this).attr('data-trip-id');
        var name = $(this).attr('data-trip-name');
        var linkUrl = $(this).attr('data-link-url');

        var container = $('<div></div>');
        container.attr('data-trip-id', tripId);
        container.dialog({
            title: name,
            width: 540,
            height: 400,
            resizable: false
        });
        drawMap(container);
    });

    $('.distance-slider').slider({
        value: 100,
        min: 0,
        max: 200,
        range: 'min',
        step: 10,
        slide: function (event, ui) {
            $(this).siblings('.label').html('within ' + $(this).slider("value") + ' miles');
        }
    });

    $('.trip-length-slider').slider({
        range: true,
        values: [2, 10],
        min: 1,
        max: 20,
        step: 1,
        slide: function (event, ui) {
            $(this).siblings('.label').html($(this).slider("values", 0) + ' - ' + $(this).slider("values", 1) + ' days');
        }
    });

    $('.trip-day-itinerary-item.transportation').click(function () {
        var id = $(this).attr('data-id');
        $.get('/itineraryitems/edittransportation/' + id, function (data) {
            CreateTransportationsModal(data);
        });
    });

    $('.trip-day-itinerary-item.website').click(function () {
        var id = $(this).attr('data-id');
        $.get('/itineraryitems/editwebsite?itineraryitemid=' + id, function (data) {
            CreateWebsiteModal(data);
        });
    });

    $('.trip-day-itinerary-item.hotel').click(function () {
        var id = $(this).attr('data-id');
        $.get('/itineraryitems/edithotel?itineraryitemid=' + id, function (data) {
            CreateHotelModal(data);
        });
    });

    $('.trip-day-itinerary-item.destination-tag').click(function () {
        var id = $(this).attr('data-id');
        $.get('/itineraryitems/editdestinationtag?itineraryitemid=' + id, function (data) {
            CreateDestinationTagModal(data);
        });
    });
});

function BrowserAutocompleteOff() {
    $('input.day-input').attr('autocomplete', 'off');
}

function BindDestinationAutocomplete(context) {
    $('.destination-autocomplete', context).autocomplete({
        source: "/destinations/search",
        select: function (event, ui) {
            var hiddenFieldName = $(this).attr('data-hidden-field-name');
            $('input[name="' + hiddenFieldName + '"]', $(this).closest("form")).val(ui.item.id);
            var autoSubmit = $(this).attr('data-auto-submit');

            if (autoSubmit == 'true')
                $(this).closest("form").submit();
        }
    });
}

function CreateTransportationsModal(data) {
    var dialog = $(data);
    var id = $('[name="id"]', dialog).val();

    var buttons = [];
    if (id) {
        buttons.push({ text: 'Delete', click: function () {
            var confirmed = confirm('Delete?');
            if (confirmed)
                window.location.href = "/itineraryitems/deletetransportation/" + id
        }
        });
    }
    buttons.push({ text: 'Save', click: function () { $(this).submit(); } });

    dialog.dialog({
        title: 'Transportation',
        dialogClass: 'transportation-dialog',
        width: 450,
        resizable: false,
        buttons: buttons
    });
    BrowserAutocompleteOff();
    BindDestinationAutocomplete(dialog);
}

function CreateWebsiteModal(data) {
    var dialog = $(data);
    var itineraryitemid = $('[name="itineraryitemid"]', dialog).val();

    var buttons = [];
    if (itineraryitemid) {
        buttons.push({ text: 'Delete', click: function () {
            var confirmed = confirm('Delete?');
            if (confirmed)
                window.location.href = "/itineraryitems/deletewebsite?itineraryitemid=" + itineraryitemid
        }
        });
    }
    buttons.push({ text: 'Save', click: function () { $(this).submit(); } });

    dialog.dialog({
        title: 'Website',
        dialogClass: 'website-dialog',
        width: 450,
        resizable: false,
        buttons: buttons
    });
    BrowserAutocompleteOff();
}

function CreateHotelModal(data) {
    var dialog = $(data);
    var itineraryitemid = $('[name="itineraryitemid"]', dialog).val();

    var buttons = [];
    if (itineraryitemid) {
        buttons.push({ text: 'Delete', click: function () {
            var confirmed = confirm('Delete?');
            if (confirmed)
                window.location.href = "/itineraryitems/deletehotel?itineraryitemid=" + itineraryitemid
        }
        });
    }
    buttons.push({ text: 'Save', click: function () { $(this).submit(); } });

    dialog.dialog({
        title: 'Hotel',
        dialogClass: 'hotel-dialog',
        width: 450,
        resizable: false,
        buttons: buttons
    });
    BrowserAutocompleteOff();
}

function CreateDestinationTagModal(data) {
    var dialog = $(data);
    var itineraryitemid = $('[name="itineraryitemid"]', dialog).val();

    var buttons = [];
    if (itineraryitemid) {
        buttons.push({ text: 'Delete', click: function () {
            var confirmed = confirm('Delete?');
            if (confirmed)
                window.location.href = "/itineraryitems/deletedestinationtag?itineraryitemid=" + itineraryitemid
        }
        });
    }
    buttons.push({ text: 'Save', click: function () { $(this).submit(); } });

    dialog.dialog({
        title: 'Activity',
        dialogClass: 'destination-tag-dialog',
        width: 450,
        resizable: false,
        buttons: buttons
    });
    BindDestinationAutocomplete(dialog);
    BrowserAutocompleteOff();
}

function drawMap(container) {

    var myOptions = { mapTypeId: google.maps.MapTypeId.ROADMAP };
    var map = new google.maps.Map(container.get(0), myOptions);
    var bounds = new google.maps.LatLngBounds();
    var tripId = container.attr('data-trip-id');
    var infoWindow = new google.maps.InfoWindow();

    var directionsPolylineOptions = { strokeColor: "#0066FF", strokeOpacity: 0.6, strokeWeight: 5 };
    var directionsService = new google.maps.DirectionsService();

    $.get('/maps/trip/' + tripId, function (mapData) {

        $.each(mapData.trans, function (i, item) {

            var fromPoint = new google.maps.LatLng(item.From.Lat, item.From.Lon);
            var toPoint = new google.maps.LatLng(item.To.Lat, item.To.Lon);
            var fromMarker = new google.maps.Marker({ position: fromPoint, map: map, title: item.From.Name });
            fromMarker.infoHtml = item.From.InfoHtml;
            var toMarker = new google.maps.Marker({ position: toPoint, map: map, title: item.To.Name });
            toMarker.infoHtml = item.To.InfoHtml;

            bounds.extend(fromPoint);
            bounds.extend(toPoint);

            if (item.PathType == 'road') {
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
                transPath.infoHtml = 'path';
            }

            google.maps.event.addListener(fromMarker, 'mouseover', function () {
                infoWindow.setContent(fromMarker.infoHtml);
                infoWindow.open(map, fromMarker);
            });
            google.maps.event.addListener(toMarker, 'mouseover', function () {
                infoWindow.setContent(toMarker.infoHtml);
                infoWindow.open(map, toMarker);
            });
        });

        $.each(mapData.hotels, function (i, item) {

            var hotelPoint = new google.maps.LatLng(item.Lat, item.Lon);
            var hotelMarker = new google.maps.Marker({ position: hotelPoint, map: map, title: item.Name });
            hotelMarker.infoHtml = item.InfoHtml
            bounds.extend(hotelPoint);

            google.maps.event.addListener(hotelMarker, 'mouseover', function () {
                infoWindow.setContent(hotelMarker.infoHtml);
                infoWindow.open(map, hotelMarker);
            });
        });

        map.fitBounds(bounds);
    });
}