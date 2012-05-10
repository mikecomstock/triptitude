var superDialog;

function SetBindings() {
    $('input').placeholder();
    $('.focus').first().focus();
    $('.date-picker').datepicker();
    //    $('.place-autocomplete').googAutocomplete();
    //    $('.tag-autocomplete').tagAutocomplete();
    //    $('.item-autocomplete').itemAutocomplete();
    //    $('#trip-search-form').submit(T.TripSearchSubmit);
};

$(function () {

    moveScroller();
    SetBindings();

    $('.place-search-input').each(function () {
        var v = new TT.Views.PlaceSearchInput({ el: this });
    });

    $('#search').submit(function (e) {
        var val = $('input[name="googreference"]', $(this)).val();
        if (val == '') e.preventDefault();
    });

    $('#trip-bar-menu li').hover(
        function () { $(this).children('ul').show(); },
        function () { $(this).children('ul').hide(); }
    );

    $('body > header').on('click', 'a.sign-up', function (e) {
        e.preventDefault();

        $.get('/templates/users/new.html', function (template) {
            var overlay = TT.Util.ShowOverlay();
            overlay.append(template);

            overlay.on('submit', 'form', function (f) {
                f.preventDefault();
                var form = $(f.currentTarget);
                var user = new TT.Models.User({
                    firstname: form.find('[name="firstname"]').val(),
                    lastname: form.find('[name="lastname"]').val(),
                    email: form.find('[name="email"]').val(),
                    password: form.find('[name="password"]').val()
                });
                user.save(null, {
                    success: function (model) {
                        window.location = '/my/trips';
                    },
                    error: function (model, response) {
                        var message = $.parseJSON(response.responseText).message;
                        alert(message);
                    }
                });
            });

        });
    });

    //    superDialog = $('#super-dialog');
    //    superDialogOverlay = $('#super-dialog-overlay');
    //    superDialogOverlay.click(function () { CloseSuperDialog(); });
    //    $('*').live('keyup', function (e) { if (e.which == 27) { CloseSuperDialog(); } });

    $('.confirm-delete').live('click', function (e) {
        var confirmed = confirm('Delete?');
        if (confirmed) {
            var data_url = $(this).data('url');
            window.location.replace(data_url);
        } else {
            e.preventDefault();
        }
    });

    //    $('.trip-row-map-link').live('click', function () {
    //        var tripId = $(this).data('trip-id');
    //        var name = $(this).data('trip-name');

    //        var container = $(document.createElement('div'));
    //        container.dialog({
    //            title: name,
    //            width: 540,
    //            height: 400,
    //            resizable: false
    //        });

    //        $.get('/maps/trip/' + tripId, function (mapData) {
    //            drawMap(container, mapData);
    //        });
    //    });

    $('.super-dialog-link').live('click', function (e) {
        e.preventDefault();
        var tripID = $(e.currentTarget).data('trip-id');
        var trip = new TT.Models.Trip({ ID: tripID });

        trip.fetch({
            success: function (model, response) {
                console.log('trip.fetch success, results is:', model);

                var newActivity = new TT.Models.Activity({ Title: '', TripID: tripID });
                trip.get('Activities').add(newActivity).moveToEnd(newActivity);

                console.log('editing activity:', newActivity, 'for trip:', model);
                openEditor(model, newActivity);
            },
            error: function (model, response) {
                console.log('error!', 'model:', model, 'response:', response);
            }
        });

        //        var url = $(this).attr('href');
        //        OpenSuperDialog(url);

        //        var activity = new TT.Models.Activity();
        //        var dialog = new TT.Views.ActivityDialog({ model: activity });
        //        dialog.render();
        //        $('#trip-bar').append(dialog.el);

    });

    //    $('.editing .packing-list-item').live('click', function (e) {
    //        if ($(e.target).is('a')) return;
    //        if ($(this).parent().is('.suggestions')) return;

    //        var id = $(this).data('id');
    //        OpenSuperDialog('/packing/edit/' + id);
    //    });

    var openEditor = function (trip, activity) {
        console.log('openEditor');
        var body = $('body');

        var close = function () {
            overlay.remove();
            editorContent.remove();
            location.reload();
        };

        var overlay = $('<div id="editor-overlay">').appendTo(body).on('click', close);
        var editorContent = $('<div id="editor-container">').appendTo(body);
        $('<div id="editor-close">').text('×').attr('title', 'Close').on('click', close).appendTo(editorContent);
        var editorElement = $('<div id="editor">').appendTo(editorContent);
        editorElement.text('loading...');

        var editor = new TT.Views.Editor.Main({ el: $('#editor'), model: trip, edit: activity });
        editor.render();
    };


    $('.activity').live('click', function (e) {

        if ($(this).parents('#editor').length > 0) return;

        if (!$(e.target).is('a')) {
            var activityId = $(this).data('activity-id');
            var activity = new TT.Models.Activity({ ID: activityId });
            //            console.log('activity to fetch:', activity, 'with id:', activityId);
            activity.fetch({
                success: function (model, response) {
                    //console.log('activity.fetch success', 'model', model, 'response', response);
                    if (model.get('Trip').UserOwnsTrip) {
                        var trip = new TT.Models.Trip({ ID: activity.get('Trip').ID });
                        //console.log('user owns trip, so now going to load trip', trip);

                        trip.fetch({
                            success: function (model, response) {
                                console.log('trip.fetch success, results is:', model);
                                var editing = model.get('Activities').get(activityId);
                                console.log('editing activity:', editing, 'for trip:', model);
                                openEditor(model, editing);
                            },
                            error: function (model, response) {
                                console.log('error!', 'model:', model, 'response:', response);
                            }
                        });

                        //                        var editor = new TT.Views.Editor.Main({ el: $('#editor'), model: currentUserModel, edit: newActivity });
                        //editor.render();
                    } else {
                        alert("You don't own this activity!");
                    }
                }
            });

            //OpenSuperDialog('/activities/edit/' + activityId);
            //openEditor(activityId);
        }
    });

    //    $('.add-to-trip').live('click', function (e) {

    //        if (e.target != this && $(e.target).is('a')) return;

    //        e.preventDefault();
    //        var link = $(this);
    //        var type = link.data('type');
    //        if (type == 'packing-list-item') {
    //            OpenSuperDialog('/packing/create', function () {
    //                superDialog.find('input[name="name"]').val(link.data('name'));
    //                superDialog.find('input[name="tagstring"]').val(link.data('tag'));
    //            });
    //        } else {
    //            var place = $(e.target).data('place');
    //            if (!place) {
    //                place = {
    //                    id: $(this).data('id'),
    //                    reference: $(this).data('reference'),
    //                    name: $(this).data('name')
    //                };
    //            }
    //            OpenSuperDialog('/activities/create?type=place', function () {
    //                var placeInputParagraph = superDialog.find('[name="name"]').parent();
    //                placeInputParagraph.find('input[name="googid"]').val(place.id);
    //                placeInputParagraph.find('input[name="googreference"]').val(place.reference);
    //                placeInputParagraph.find('input[name="googname"]').val(place.name).trigger('change');
    //                placeInputParagraph.hide();
    //                $(document.createElement('p')).addClass('place').text(place.name).insertBefore(placeInputParagraph);
    //            });
    //        }
    //    });
});

//function OpenSuperDialog(url, callback) {

//    $('#trip-bar-menu li').children('ul').hide();
//    superDialogOverlay.show();

//    // Show loading screen if it's taking too long
//    var loaded = false;
//    setTimeout(function () { if (!loaded) superDialog.html('<header><h3>Loading Options...</h3></header>').show(); }, 500);

//    $.get(url, function (result) {
//        loaded = true;
//        superDialog.html(result).show()
//        superDialog.find('.focus').focus()
//        superDialog.find('input.day-input').attr('autocomplete', 'off');
//        superDialog.find('.cancel').click(function (e) { e.preventDefault(); CloseSuperDialog(); });
//        superDialog.find('.place-autocomplete').googAutocomplete();
//        superDialog.find('.tag-autocomplete').tagAutocomplete();
//        superDialog.find('.item-autocomplete').itemAutocomplete();

//        scrollToBottom(superDialog.find('.notes'));

//        initializeActivityDialog();
//        initializeTransportationDialog();
//        initializeNoteDialog();

//        if (callback) callback();

//        superDialog.find('#dialog-menu li').click(function (e) {
//            var dataPageName = $(this).data('page');
//            var currentPage = $('li, .dialog-page');
//            currentPage.removeClass('selected-page');
//            var newPage = $('[data-page="' + dataPageName + '"]', superDialog);
//            newPage.addClass('selected-page');
//            $('.focus', newPage).first().focus();
//            scrollToBottom($('.notes', superDialog));
//        });

//        superDialog.find('form').submit(function (e) {
//            e.preventDefault();
//            var form = $(this);

//            var requiredFields = form.find('.required');
//            requiredFields.each(function () {
//                var field = $(this);
//                if (field.val().length == 0) {
//                    field.parent().addClass('invalid');
//                } else {
//                    field.parent().removeClass('invalid');
//                }
//            });

//            var invalidFields = form.find('.invalid');

//            if (invalidFields.size() > 0) {
//                var firstField = invalidFields.first().find('input[type="text"]');
//                firstField.focus();
//            } else {
//                var formData = form.serialize();
//                var formAction = form.prop('action');
//                $.post(formAction, formData)
//                    .success(function (response) {
//                        superDialog.data('changes', true);
//                        if (response.replace) {
//                            superDialog.html(response.replace);
//                        } else {
//                            var onPlanningPage = $('#trips-details, #trips-packinglist').size() > 0;
//                            if (onPlanningPage) {
//                                location.reload();
//                            } else if (response.redirect) {
//                                window.location.href = response.redirect;
//                            } else {
//                                $('.buttons').html('<div class="saved">Saved!</div>');
//                                setTimeout(function () { CloseSuperDialog(); }, 1000);
//                            }
//                        }
//                    });
//            }
//        });
//    });
//}

//function CloseSuperDialog() {
//    if (!superDialog.is(':visible')) return;

//    var currentlyViewingTripId = $('body').data('id');
//    var currentlyEditingTripId = $('#trip-bar').data('trip-id');

//    if (superDialog.data('changes') && (currentlyViewingTripId == currentlyEditingTripId)) {
//        location.reload();
//    } else {
//        superDialog.data('changes', false);
//        superDialog.hide();
//        superDialogOverlay.hide();
//        superDialog.empty();
//    }
//}

//function initializeActivityDialog() {
//    var dialog = superDialog.find('#place-dialog');
//    if (dialog.length == 0) return;

//    dialog.on('click', '#change-title a', function (e) {
//        e.preventDefault();
//        showTitleFieldset();
//    });

//    dialog.on('keyup change', 'input,select', function () { updateTitle(); });

//    var showTitleFieldset = function () {
//        var titleFieldset = dialog.find('#title-fieldset').show();
//        titleFieldset.find('input').focus();
//        $(this).parent().hide();
//        dialog.find('#change-title').hide();
//    };
//    if (dialog.find('input#title').val() != '') showTitleFieldset();

//    var updateTitle = function () {
//        var tagVal = dialog.find('#tagstring').val();
//        var googVal = dialog.find('#googname').val();

//        var calculatedTitle = tagVal;
//        if (tagVal != '' && googVal != '')
//            calculatedTitle += ' at ';

//        calculatedTitle += googVal;

//        // Override with title field, if available
//        var titleInputVal = dialog.find('#title').val();
//        if (titleInputVal != '') calculatedTitle = titleInputVal;

//        calculatedTitle = calculatedTitle == '' ? 'Place / Activity' : calculatedTitle;
//        dialog.find('h3').text(calculatedTitle);
//    };
//    updateTitle();
//};

//function initializeTransportationDialog() {
//    var dialog = superDialog.find('#transportation-dialog');
//    if (dialog.length == 0) return;

//    dialog.on('click', '#change-title a', function (e) {
//        e.preventDefault();
//        showTitleFieldset();
//    });

//    dialog.on('keyup change', 'input,select', function () { updateTitle(); });

//    var showTitleFieldset = function () {
//        var titleFieldset = dialog.find('#title-fieldset').show();
//        titleFieldset.find('input').focus();
//        $(this).parent().hide();
//        dialog.find('#change-title').hide();
//    };
//    if (dialog.find('input#title').val() != '') showTitleFieldset();

//    var updateTitle = function () {

//        var selectedType = dialog.find('#transportationtypeid option:selected');
//        var typeVal = selectedType.val() == '' ? 'Transportation' : selectedType.text();
//        var calculatedTitle = typeVal;

//        var fromValue = dialog.find('#fromgoogname').val();
//        calculatedTitle += fromValue == '' ? '' : ' from ' + fromValue;

//        var toValue = dialog.find('#togoogname').val();
//        calculatedTitle += toValue == '' ? '' : ' to ' + toValue;

//        // Override with title field, if available
//        var titleInputVal = dialog.find('#title').val();
//        if (titleInputVal != '') calculatedTitle = titleInputVal;

//        dialog.find('h3').text(calculatedTitle);
//    };
//    updateTitle();
//};

//function initializeNoteDialog() {
//    var dialog = superDialog.find('#note-dialog');
//    if (dialog.length == 0) return;

//    dialog.on('change', 'select', function () {
//        var activityId = $(this).val();
//        OpenSuperDialog('/activities/edit/' + activityId + '?selectedtab=notes');
//    });
//};

(function ($) {
    $.fn.tagAutocomplete = function () {
        this.each(function () {
            var input = $(this);
            input.autocomplete({
                source: function (request, response) {
                    $.getJSON('/tags/search', request, function (data) {
                        response(data);
                    });
                },
                select: function () {
                    if (input.is('.auto-submit')) {
                        input.closest('form').submit();
                    }
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

                var autosubmit = $input.data('auto-submit') || $input.is('.auto-submit');
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

//function scrollToBottom($element) {
//    if ($element) {
//        $element.prop({ scrollTop: $element.prop('scrollHeight') });
//    }
//}

//log = function (a) {
//    if (window['console'] && console['log']) {
//        console.log(a);
//    }
//};

//var T = {};

//T.TripSearchSubmit = function (e) {
//    e.preventDefault();
//    var form = $(this);
//    $.get('/trips/searchresults', form.serialize(), function (data) {
//        log(data);
//        $('#trip-search-results').html(data);
//    });
//    log('trip search');
//    log(form);
//};

//T.NearbyPlaces = function () {

//    var map;
//    var searchService;
//    var searchForm = $('#placeSearch');
//    var infowindow;
//    var placeList = $('#place-rows');
//    var noResults;

//    function initialize() {
//        var m = $('#map');
//        var lat = m.data('lat');
//        var lng = m.data('lng');
//        var centerOn = new google.maps.LatLng(lat, lng);

//        map = new google.maps.Map(document.getElementById('map'), {
//            mapTypeId: google.maps.MapTypeId.ROADMAP,
//            center: centerOn,
//            zoom: 14
//        });
//        google.maps.event.addListener(map, 'idle', function () { doSearch(); });

//        searchService = new google.maps.places.PlacesService(map);

//        infowindow = new google.maps.InfoWindow({ zIndex: 10000 });
//        google.maps.event.addListener(infowindow, 'closeclick', function () { clearActive(); });

//        searchForm.submit(function (e) {
//            e.preventDefault();
//            clearActive();
//            doSearch();
//        });
//    }

//    function doSearch() {

//        var keyword = searchForm.find('#keyword').val();
//        var types = searchForm.find('#types').val() == "" ? null : [searchForm.find('#types').val()];

//        var options = { bounds: map.getBounds(), keyword: keyword, types: types };
//        searchService.search(options, function (results, status) {

//            if (noResults) {
//                noResults.remove();
//                noResults = null;
//            }

//            placeList.find('li:not(.active)').each(function (i, li) {
//                var $li = $(li);
//                var place = $li.data('place');
//                place.marker.setMap(null);
//                $li.remove();
//            });

//            switch (status) {
//                case google.maps.places.PlacesServiceStatus.OK:
//                    {
//                        var activePlace = placeList.find('li.active').data('place');
//                        $.each(results, function (i, place) {
//                            // Can't compare objects here, use IDs instead.
//                            if (activePlace == null || place.id != activePlace.id) {
//                                createMarker(place);
//                                createListItem(place);
//                            }
//                        });
//                        break;
//                    }
//                case google.maps.places.PlacesServiceStatus.ZERO_RESULTS:
//                    {
//                        noResults = $(document.createElement('div')).text('No results found');
//                        noResults.insertAfter(placeList);
//                        break;
//                    }
//                default:
//                    {
//                        noResults = $(document.createElement('div')).text('An error has occured. Please edit your search and try again.');
//                        noResults.insertAfter(placeList);
//                        break;
//                    }
//            }
//        });
//    }

//    function createMarker(place) {
//        var marker = new google.maps.Marker({ map: map, position: place.geometry.location });
//        google.maps.event.addListener(marker, 'click', function () { setActive(place); });
//        place.marker = marker;
//    }

//    function createListItem(place) {
//        var li = $(document.createElement('li'))
//            .text(place.name)
//            .appendTo(placeList)
//            .click(function (e) { if (e.target == this) { setActive(place); } })
//            .data('place', place);
//        place.li = li;
//        $('<a class="add-to-trip" rel="nofollow">+ Add to Trip</a>').data('place', place).appendTo(li);
//    }

//    function getInfoWindowContent(place) {
//        var content = $(document.createElement('div')).addClass('info-window');
//        $('<a class="title" rel="nofollow"></a>')
//            .text(place.name)
//            .data('place', place)
//            .attr('href', '/places/redirect?googReference=' + place.reference + '&googId=' + place.id)
//            .appendTo(content);
//        $('<a class="add-to-trip" rel="nofollow">+ Add to Trip</a>').data('place', place).appendTo(content);
//        return content;
//    }

//    function setActive(place) {
//        clearActive();
//        placeList.find('li.active').removeClass('active');
//        place.li.addClass('active');

//        var infoWindowContent = getInfoWindowContent(place);
//        infowindow.setContent(infoWindowContent[0]);
//        infowindow.open(map, place.marker);
//    }

//    function clearActive() {
//        infowindow.close();
//        placeList.find('.active').removeClass('active');
//    }

//    var mapPinning = function () {
//        var b = $(window).scrollTop() + 90;
//        var sa = $("#mapanchor");
//        var d = sa.offset().top;
//        var c = $("#map");
//        c.width(c.width()); // fixes an overlay issue
//        if (b > d) {
//            var height = c.outerHeight(true);
//            sa.css({ height: height + "px" });
//            c.addClass('at-top');
//        } else {
//            if (b <= d) {
//                c.removeClass('at-top');
//                sa.css({ height: "0" });
//            }
//        }
//    };

//    initialize();
//    $(window).scroll(mapPinning);
//    mapPinning();
//};

/**
* jQuery Masonry v2.0.111015
* A dynamic layout plugin for jQuery
* The flip-side of CSS Floats
* http://masonry.desandro.com
*
* Licensed under the MIT license.
* Copyright 2011 David DeSandro
*/
//(function (a, b, c) { var d = b.event, e; d.special.smartresize = { setup: function () { b(this).bind("resize", d.special.smartresize.handler) }, teardown: function () { b(this).unbind("resize", d.special.smartresize.handler) }, handler: function (a, b) { var c = this, d = arguments; a.type = "smartresize", e && clearTimeout(e), e = setTimeout(function () { jQuery.event.handle.apply(c, d) }, b === "execAsap" ? 0 : 100) } }, b.fn.smartresize = function (a) { return a ? this.bind("smartresize", a) : this.trigger("smartresize", ["execAsap"]) }, b.Mason = function (a, c) { this.element = b(c), this._create(a), this._init() }; var f = ["position", "height"]; b.Mason.settings = { isResizable: !0, isAnimated: !1, animationOptions: { queue: !1, duration: 500 }, gutterWidth: 0, isRTL: !1, isFitWidth: !1 }, b.Mason.prototype = { _filterFindBricks: function (a) { var b = this.options.itemSelector; return b ? a.filter(b).add(a.find(b)) : a }, _getBricks: function (a) { var b = this._filterFindBricks(a).css({ position: "absolute" }).addClass("masonry-brick"); return b }, _create: function (c) { this.options = b.extend(!0, {}, b.Mason.settings, c), this.styleQueue = [], this.reloadItems(); var d = this.element[0].style; this.originalStyle = {}; for (var e = 0, g = f.length; e < g; e++) { var h = f[e]; this.originalStyle[h] = d[h] || "" } this.element.css({ position: "relative" }), this.horizontalDirection = this.options.isRTL ? "right" : "left", this.offset = {}; var i = b(document.createElement("div")); this.element.prepend(i), this.offset.y = Math.round(i.position().top), this.options.isRTL ? (i.css({ "float": "right", display: "inline-block" }), this.offset.x = Math.round(this.element.outerWidth() - i.position().left)) : this.offset.x = Math.round(i.position().left), i.remove(); var j = this; setTimeout(function () { j.element.addClass("masonry") }, 0), this.options.isResizable && b(a).bind("smartresize.masonry", function () { j.resize() }) }, _init: function (a) { this._getColumns(), this._reLayout(a) }, option: function (a, c) { b.isPlainObject(a) && (this.options = b.extend(!0, this.options, a)) }, layout: function (a, c) { var d, e, f, g, h, i; for (var j = 0, k = a.length; j < k; j++) { d = b(a[j]), e = Math.ceil(d.outerWidth(!0) / this.columnWidth), e = Math.min(e, this.cols); if (e === 1) this._placeBrick(d, this.colYs); else { f = this.cols + 1 - e, g = []; for (i = 0; i < f; i++) h = this.colYs.slice(i, i + e), g[i] = Math.max.apply(Math, h); this._placeBrick(d, g) } } var l = {}; l.height = Math.max.apply(Math, this.colYs) - this.offset.y; if (this.options.isFitWidth) { var m = 0, j = this.cols; while (--j) { if (this.colYs[j] !== this.offset.y) break; m++ } l.width = (this.cols - m) * this.columnWidth - this.options.gutterWidth } this.styleQueue.push({ $el: this.element, style: l }); var n = this.isLaidOut ? this.options.isAnimated ? "animate" : "css" : "css", o = this.options.animationOptions, p; for (j = 0, k = this.styleQueue.length; j < k; j++) p = this.styleQueue[j], p.$el[n](p.style, o); this.styleQueue = [], c && c.call(a), this.isLaidOut = !0 }, _getColumns: function () { var a = this.options.isFitWidth ? this.element.parent() : this.element, b = a.width(); this.columnWidth = this.options.columnWidth || this.$bricks.outerWidth(!0) || b, this.columnWidth += this.options.gutterWidth, this.cols = Math.floor((b + this.options.gutterWidth) / this.columnWidth), this.cols = Math.max(this.cols, 1) }, _placeBrick: function (a, b) { var c = Math.min.apply(Math, b), d = 0; for (var e = 0, f = b.length; e < f; e++) if (b[e] === c) { d = e; break } var g = { top: c }; g[this.horizontalDirection] = this.columnWidth * d + this.offset.x, this.styleQueue.push({ $el: a, style: g }); var h = c + a.outerHeight(!0), i = this.cols + 1 - f; for (e = 0; e < i; e++) this.colYs[d + e] = h }, resize: function () { var a = this.cols; this._getColumns(), this.cols !== a && this._reLayout() }, _reLayout: function (a) { var b = this.cols; this.colYs = []; while (b--) this.colYs.push(this.offset.y); this.layout(this.$bricks, a) }, reloadItems: function () { this.$bricks = this._getBricks(this.element.children()) }, reload: function (a) { this.reloadItems(), this._init(a) }, appended: function (a, b, c) { if (b) { this._filterFindBricks(a).css({ top: this.element.height() }); var d = this; setTimeout(function () { d._appended(a, c) }, 1) } else this._appended(a, c) }, _appended: function (a, b) { var c = this._getBricks(a); this.$bricks = this.$bricks.add(c), this.layout(c, b) }, remove: function (a) { this.$bricks = this.$bricks.not(a), a.remove() }, destroy: function () { this.$bricks.removeClass("masonry-brick").each(function () { this.style.position = "", this.style.top = "", this.style.left = "" }); var c = this.element[0].style; for (var d = 0, e = f.length; d < e; d++) { var g = f[d]; c[g] = this.originalStyle[g] } this.element.unbind(".masonry").removeClass("masonry").removeData("masonry"), b(a).unbind(".masonry") } }, b.fn.imagesLoaded = function (a) { function h() { --e <= 0 && this.src !== f && (setTimeout(g), d.unbind("load error", h)) } function g() { a.call(b, d) } var b = this, d = b.find("img").add(b.filter("img")), e = d.length, f = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw=="; e || g(), d.bind("load error", h).each(function () { if (this.complete || this.complete === c) { var a = this.src; this.src = f, this.src = a } }); return b }; var g = function (a) { this.console && console.error(a) }; b.fn.masonry = function (a) { if (typeof a == "string") { var c = Array.prototype.slice.call(arguments, 1); this.each(function () { var d = b.data(this, "masonry"); if (!d) g("cannot call methods on masonry prior to initialization; attempted to call method '" + a + "'"); else { if (!b.isFunction(d[a]) || a.charAt(0) === "_") { g("no such method '" + a + "' for masonry instance"); return } d[a].apply(d, c) } }) } else this.each(function () { var c = b.data(this, "masonry"); c ? (c.option(a || {}), c._init()) : b.data(this, "masonry", new b.Mason(a, this)) }); return this } })(window, jQuery);