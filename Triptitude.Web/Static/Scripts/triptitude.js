var superDialog;

$(function () {

    if (navigator.platform != 'iPad' && navigator.platform != 'iPhone' && navigator.platform != 'iPod') {
        moveScroller();
    }

    $('input').placeholder();
    $('.focus').first().focus();
    $('.date-picker').datepicker();

    BindDestinationAutocomplete(null);

    $('#search').submit(function (e) {
        var destinationId = $('input[name="destinationid"]', $(this)).val();
        if (destinationId == '') {
            e.preventDefault();
        }
    });

    $('#trip-bar-menu li').hover(
        function () { $(this).children('ul').show(); },
        function () { $(this).children('ul').hide(); }
    );


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
    /* Super Dialog */
    /****************/

    superDialog = $('#super-dialog');
    superDialogOverlay = $('#super-dialog-overlay');
    $('.cancel', superDialog).live('click', function (e) { e.preventDefault(); superDialog.hide(); superDialogOverlay.hide(); });
    $('*').live('keyup', function (e) { if (e.which == 27) { superDialog.hide(); superDialogOverlay.hide(); } });


    /* Delete Confirmations */
    $('.confirm-delete').live('click', function (e) {
        var test = confirm('Delete?');
        if (test) {
            var data_url = $(this).attr('data-url');
            window.location.replace(data_url);
        } else {
            e.preventDefault();
        }
    });

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

    $('#super-dialog.note select').live('change', function () {
        var select = $(this);
        var activityId = select.val();
        CreateActivityModal('note', '/activities/edit/' + activityId + '?selectedtab=notes');
    });

    /****************/

    $('.add-activity').live('click', function () {
        var activityType = $(this).attr('data-activity-type');
        var url = '/activities/create?type=' + activityType;

        switch (activityType) {
            case 'transportation':
                break;
            case 'place':
                var placeId = $(this).attr('data-place-id') || '';
                url += '&placeid=' + placeId;
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

    $('.trip-day .activity').click(function () {
        var activityId = $(this).attr('data-activity-id');
        var activityType = $(this).attr('data-activity-type');
        CreateActivityModal(activityType, '/activities/edit/' + activityId);
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

function CreateActivityModal(activityType, url) {
    $.get(url, function (data) {
        $('#trip-bar-menu li').children('ul').hide();
        $('.content', superDialog).html(data);
        superDialog.attr('class', activityType);
        superDialog.show();
        superDialogOverlay.show();
        $('.focus', superDialog).focus();
        $('input.day-input', superDialog).attr('autocomplete', 'off');
        BindDestinationAutocomplete(superDialog);

        if ($('.place-map').length > 0)
            drawPlaceDialogMap();

        scrollToBottom($('.notes', superDialog));
    });
}

$('#dialog-menu li', superDialog).live('click', function (e) {
    var dataPageName = $(this).attr('data-page');
    var currentPage = $('li, .dialog-page');
    currentPage.removeClass('selected-page');
    var newPage = $('[data-page="' + dataPageName + '"]', superDialog);
    newPage.addClass('selected-page');
    $('.focus', newPage).first().focus();
    scrollToBottom($('.notes', superDialog));
});

function drawPlaceDialogMap() {
    var myOptions = { mapTypeId: google.maps.MapTypeId.ROADMAP };
    var map = new google.maps.Map($('.place-map', superDialog).get(0), myOptions);
    var bounds = new google.maps.LatLngBounds();

    var marker;
    var currLat = $('#latitude', superDialog).val();
    var currLng = $('#longitude', superDialog).val();
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
        $('#latitude', superDialog).val(event.latLng.lat());
        $('#longitude', superDialog).val(event.latLng.lng());
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
        marker.infoHtml = item.InfoHtml;
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

(function ($) {
    $.fn.equalHeights = function (minHeight, maxHeight) {
        tallest = (minHeight) ? minHeight : 0;
        this.each(function () {
            if ($(this).height() > tallest) {
                tallest = $(this).height();
            }
        });
        if ((maxHeight) && tallest > maxHeight) tallest = maxHeight;
        return this.each(function () {
            $(this).height(tallest).css("overflow", "auto");
        });
    };
})(jQuery);

/***********************************************************/

function scrollToBottom(jqueryElement) {
    if (jqueryElement) {
        jqueryElement.prop({ scrollTop: jqueryElement.prop('scrollHeight') });
    }
}