$(function () {

    $('input').placeholder();

    BindDestinationAutocomplete(null);

    $('.confirm-delete').click(function (e) {
        var test = confirm('Delete?');
        if (!test) { e.preventDefault(); }
    });

    $('#search').submit(function (e) {
        var destinationId = $('input[name="destinationid"]', $(this)).val();
        if (destinationId == '') {
            e.preventDefault();
        }
    });

    $('#trip-bar-menu li').hover(function (hoverData) {
        $(this).children('ul').show();
    }, function (hoverData) {
        $(this).children('ul').hide();
    });

    //    $('.trip-length-slider').slider({
    //        range: true,
    //        values: [2, 10],
    //        min: 1,
    //        max: 20,
    //        step: 1,
    //        slide: function (event, ui) {
    //            $(this).siblings('.label').html($(this).slider("values", 0) + ' - ' + $(this).slider("values", 1) + ' days');
    //        }
    //    });

    /****************/
    /* Hotel Search */
    /****************/

    $('.distance-slider', '.hotels #search-form').slider({
        value: 10,
        min: 1,
        max: 50,
        range: 'min',
        step: 1,
        slide: function (event, ui) {
            $(this).siblings('.label').html('within ' + ui.value + ' miles');
        },
        change: function (event, ui) {
            $(this).siblings('input').val(ui.value);
            $(this).closest('form').submit();
        }
    });
    $('.hotels #search-form').submit(function (event) {
        event.preventDefault();
        $.get("/hotels/search", $(this).serialize(), function (data) {
            $('.panel-content').html(data);
        });
    });

    /****************/
    /* Place Search */
    /****************/

    $('.distance-slider', '.places #search-form').slider({
        value: 10,
        min: 1,
        max: 50,
        range: 'min',
        step: 1,
        slide: function (event, ui) {
            $(this).siblings('.label').html('within ' + ui.value + ' miles');
        },
        change: function (event, ui) {
            $(this).siblings('input').val(ui.value);
        }
    });

    $('.places #search-form').submit(function (event) {
        event.preventDefault();
        $.get("/places/search", $(this).serialize(), function (data) {
            $('.panel-content').html(data);
        });
    });

    /****************/

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

    /****************/

    $('.add-activity').live('click', function (clickData) {
        var activityType = $(this).attr('data-activity-type');

        switch (activityType) {
            case 'transportation':
                $.get('/activities/addtransportation', function (data) { CreateActivityModal(data, "Transportation", "transportation"); });
                break;
            case 'website':
                $.get('/activities/addwebsite', function (data) { CreateActivityModal(data, "Website", "website"); });
                break;
            case 'place':
                var placeId = $(this).attr('data-place-id') || '';
                $.get('/activities/addplace?placeid=' + placeId, function (data) { CreateActivityModal(data, "Place", "place"); });
                break;
            case 'hotel':
                var hotelId = $(this).attr('data-hotel-id');
                $.get('/activities/addhotel?hotelid=' + hotelId, function (data) { CreateActivityModal(data, "Hotel", "hotel"); });
                break;
        }
    });

    $('.trip-day .activity').click(function () {
        var activityId = $(this).attr('data-activity-id');
        var activityType = $(this).attr('data-activity-type');

        switch (activityType) {
            case 'transportation':
                $.get('/activities/edittransportation?activityid=' + activityId, function (data) { CreateActivityModal(data, "Transportation", "transportation") });
                break;
            case 'website':
                $.get('/activities/editwebsite?activityid=' + activityId, function (data) { CreateActivityModal(data, "Website", "website"); });
                break;
            case 'place':
                var placeId = $(this).attr('data-place-id');
                $.get('/activities/editplace?activityid=' + activityId, function (data) { CreateActivityModal(data, "Place", "place"); });
                break;
            case 'hotel':
                var hotelId = $(this).attr('data-hotel-id');
                $.get('/activities/edithotel?activityid=' + activityId, function (data) { CreateActivityModal(data, "Hotel", "hotel"); });
                break;
        }
    });
});

function BindDestinationAutocomplete(context) {
    $('.destination-autocomplete', context).autocomplete({
        delay: 50,
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

var dialog;

function CreateActivityModal(data, title, activityType) {
    if (dialog) {
        dialog.dialog('destroy');
    }

    dialog = $(data);
    var activityId = $('[name="activityid"]', dialog).val();

    var buttons = [];
    if (activityId) {
        buttons.push({ text: 'Delete', click: function () {
            var confirmed = confirm('Delete?');
            if (confirmed)
                window.location.href = "/activities/delete?activityid=" + activityId
        }
        });
    }
    buttons.push({ text: 'Save', click: function () { $(this).submit(); } });

    dialog.dialog({
        title: title,
        dialogClass: activityType + '-dialog',
        width: 450,
        resizable: false,
        position: ['center',80],
        buttons: buttons
    });

    $('input.day-input').attr('autocomplete', 'off');
    BindDestinationAutocomplete(dialog);

    if (activityType == 'place')
        drawPlaceDialogMap(dialog);
}

function drawPlaceDialogMap(dialog) {
    var myOptions = { mapTypeId: google.maps.MapTypeId.ROADMAP };
    var map = new google.maps.Map($('.place-map', dialog).get(0), myOptions);
    var bounds = new google.maps.LatLngBounds();

    var marker;
    var currLat = $('#latitude', dialog).val();
    var currLng = $('#longitude', dialog).val();
    if (currLat != '' && currLng != '') {
        var point = new google.maps.LatLng(currLat, currLng);
        marker = new google.maps.Marker({ position: point, map: map });
        bounds.extend(point);
    }
    map.fitBounds(bounds);
    map.setZoom(13);

    google.maps.event.addListener(map, 'click', function (event) {
        if (marker != null) marker.setMap(null);
        marker = new google.maps.Marker({ position: event.latLng, map: map });
        $('#latitude', dialog).val(event.latLng.lat());
        $('#longitude', dialog).val(event.latLng.lng());
    });
}

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
        marker.infoHtml = item.InfoHtml

        if (item.ExtendBounds == true) bounds.extend(point);

        google.maps.event.addListener(marker, 'mouseover', function () {
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