$(function () {

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

    $('.create-transportation-link').click(function (clickData) {
        var tripId = $(this).attr('data-trip-id');
        $.get('/transportations/create?tripid=' + tripId, function (getData) {
            CreateTransportationsModal(getData);
        });
    });

    $('.create-website-link').click(function (clickData) {
        var tripId = $(this).attr('data-trip-id');
        $.get('/websites/create?tripid=' + tripId, function (getData) {
            CreateWebsiteModal(getData);
        });
    });

    $('.trip-day-itinerary-item .create-note-link').click(function (clickData) {
        var itineraryItemId = $(this).attr('data-itinerary-item-id');
        $.get('/notes/create?itineraryItemId=' + itineraryItemId, function (data) {
            CreateNoteModal(data);
        });
    });

    $('.itinerary-item-note .edit-note-link').click(function (clickData) {
        clickData.preventDefault();
        var noteId = $(this).attr('data-note-id');
        $.get('/notes/edit/' + noteId, function (data) {
            CreateNoteModal(data);
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
            modal: false,
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
        $.get('/transportations/edit/' + id, function (data) {
            CreateTransportationsModal(data);
        });
    });

    $('.trip-day-itinerary-item.website').click(function () {
        var id = $(this).attr('data-id');
        $.get('/websites/edit?itineraryitemid=' + id, function (data) {
            CreateWebsiteModal(data);
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

function CreateNoteModal(data) {
    $(data).dialog({
        title: 'Your note',
        dialogClass: 'note-dialog',
        width: 450,
        height: 300,
        modal: false,
        buttons: [{
            text: 'Save',
            click: function () { $(this).submit(); }
        }]
    });
}

function CreateTransportationsModal(data) {
    var dialog = $(data);
    var id = $('[name="id"]', dialog).val();

    dialog.dialog({
        title: 'Transportation',
        dialogClass: 'transportation-dialog',
        width: 450,
        height: 430,
        modal: false,
        resizable: false,
        buttons: [
            { text: 'Delete', click: function () {
                var confirmed = confirm('Delete?');
                if (confirmed)
                    window.location.href = "/transportations/delete/" + id
            }
            },
            { text: 'Save', click: function () { $(this).submit(); } }
        ]
    });
    BrowserAutocompleteOff();
    BindDestinationAutocomplete(dialog);
}

function CreateWebsiteModal(data) {
    var dialog = $(data);
    var id = $('[name="itineraryitemid"]', dialog).val();
    dialog.dialog({
        title: 'Website',
        dialogClass: 'website-dialog',
        width: 450,
        height: 260,
        model: false,
        resizable: false,
        buttons: [
            { text: 'Delete', click: function () {
                var confirmed = confirm('Delete?');
                if (confirmed)
                    window.location.href = "/websites/delete?itineraryitemid=" + id
            }
            },
            { text: 'Save', click: function () { $(this).submit(); } }
        ]
    });
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