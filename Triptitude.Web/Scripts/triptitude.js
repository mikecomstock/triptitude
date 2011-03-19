$(function () {

    $('input').placeholder();

    $('.confirm-delete').click(function (e) {
        var test = confirm('Delete?');
        if (!test) { e.preventDefault(); }
    });

    $('#search input[name="s"]').autocomplete({
        source: "/destinations/search",
        select: function (event, ui) {
            $('#search input[name="destinationid"]').val(ui.item.id);
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
});