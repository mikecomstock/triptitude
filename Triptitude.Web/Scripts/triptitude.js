$(function () {
    var searchForm = $('#search');

    $('input').placeholder();

    $('.confirm-delete').click(function (e) {
        var test = confirm('Delete?');
        if (!test) { e.preventDefault(); }
    });

    searchForm.submit(function (e) {
        var destinationId = $('#search input[name="destinationid"]').val();
        if (destinationId == '') {
            e.preventDefault();
        }
    });

    $('.destination-autocomplete').autocomplete({
        source: "/destinations/search",
        select: function (event, ui) {
            $('input[name="destinationid"]', $(this).closest("form")).val(ui.item.id);
            $(this).closest("form").submit();
        }
    });

    $('.trip-day-itinerary-item .create-note-link').click(function (clickData) {
        var itineraryItemId = $(this).attr('data-itinerary-item-id');
        $.get('/notes/create?itineraryItemId=' + itineraryItemId, function (data) {
            CreateNoteModal(data);
        });
    });

    $('.itinerary-item-note .edit-note-link').click(function (clickData) {
        var noteId = $(this).attr('data-note-id');
        $.get('/notes/edit/' + noteId, function (data) {
            CreateNoteModal(data);
        });
    });

    function CreateNoteModal(data) {
        $(data).dialog({
            title: 'Your note',
            dialogClass: 'note-dialog',
            width: 450,
            height: 300,
            modal: true,
            buttons: [{
                text: 'Save',
                click: function () { $(this).submit(); }
            }]
        });
    }

    $('.trip-row-map-link').click(function () {
        var name = $(this).attr('data-trip-name');
        var mapUrl = $(this).attr('data-image-url');
        var linkUrl = $(this).attr('data-link-url');

        $('<a class="trip-map-dialog-link" href="' + linkUrl + '"><img src="' + mapUrl + '" height="300" width="500" /></a>').dialog({
            title: name,
            modal: false,
            width: 540,
            resizable: false
        });
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
        values: [2,10],
        min: 1,
        max: 20,
        step: 1,
        slide: function (event, ui) {
            $(this).siblings('.label').html($(this).slider("values", 0) + ' - ' + $(this).slider("values", 1) + ' days');
        }
    });
});