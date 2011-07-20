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

    //    $('.distance-slider').slider({
    //        value: 100,
    //        min: 0,
    //        max: 200,
    //        range: 'min',
    //        step: 10,
    //        slide: function (event, ui) {
    //            $(this).siblings('.label').html('within ' + ui.value + ' miles');
    //        }
    //    });

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

    /****************/
    /* Hotel Search */
    /****************/

    $('.distance-slider', '#hotel-search-form').slider({
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
    $('#hotel-search-form').submit(function (event) {
        event.preventDefault();
        $.get("/hotels/search", $(this).serialize(), function (data) {
            $('.panel-content').html(data);
        });
    });

    /****************/
    /* Place Search */
    /****************/

    $('.distance-slider', '#place-search-form').slider({
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

    $('#place-search-form').submit(function (event) {
        event.preventDefault();
        $.get("/places/search", $(this).serialize(), function (data) {
            $('.panel-content').html(data);
        });
    });

    /****************/

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
            case 'activity':
                $.get('/activities/adddestinationtag', function (data) { CreateActivityModal(data, "Activity", "destination-tag"); });
                break;
            case 'place':
                var placeId = $(this).attr('data-place-id');
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
            case 'activity':
                $.get('/activities/editdestinationtag?activityid=' + activityId, function (data) { CreateActivityModal(data, "Activity", "destination-tag"); });
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

function CreateActivityModal(data, title, dialogClass) {
    var dialog = $(data);
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
        dialogClass: dialogClass + '-dialog',
        width: 450,
        resizable: false,
        buttons: buttons
    });

    $('input.day-input').attr('autocomplete', 'off');
    BindDestinationAutocomplete(dialog);
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

        $.each(mapData.destinationTags, function (i, item) {

            var point = new google.maps.LatLng(item.Lat, item.Lon);
            var marker = new google.maps.Marker({ position: point, map: map, title: item.Name });
            marker.infoHtml = item.InfoHtml
            bounds.extend(point);

            google.maps.event.addListener(marker, 'mouseover', function () {
                infoWindow.setContent(marker.infoHtml);
                infoWindow.open(map, marker);
            });
        });

        map.fitBounds(bounds);
    });
}