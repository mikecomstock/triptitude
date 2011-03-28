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
        values: [2, 10],
        min: 1,
        max: 20,
        step: 1,
        slide: function (event, ui) {
            $(this).siblings('.label').html($(this).slider("values", 0) + ' - ' + $(this).slider("values", 1) + ' days');
        }
    });

    $('[data-action="edit-transportation"]').click(function () {
        var id = $(this).attr('data-id');
        $.get('/transportations/edit/' + id, function (data) {
            CreateTransportationsModal(data);
        });
    });
});

function BindDestinationAutocomplete(context) {
    $('.destination-autocomplete', context).autocomplete({
        source: "/destinations/search",
        select: function (event, ui) {
            var hiddenFieldName = $(this).attr('data-hidden-field-name');
            $('input[name="' + hiddenFieldName + '"]', $(this).closest("form")).val(ui.item.id);
            var autoSubmit = $(this).attr('data-auto-submit');
            console.log(autoSubmit);
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
        height: 300,
        modal: false,
        resizable: false,
        buttons: [
            { text: 'Delete', click: function () {
                var test = confirm('Delete?');
                if (test)
                    window.location.href = "/transportations/delete/" + id
            }
            },
            { text: 'Save', click: function () { $(this).submit(); } }
        ]
    });

    BindDestinationAutocomplete(dialog);
}