$(function () {

    $('input').placeholder();

    //    $('#search form').submit(function () {
    //        doSearch();
    //        return false;
    //    });
    //    $('#search form input').keyup(function () {
    //        doSearch();
    //    });

    //    var timerid;
    //    function doSearch() {
    //        var form = this;
    //        clearTimeout(timerid);
    //        timerid = setTimeout(function () {
    //            $('#search-results').load('/search', $('#search form').serialize());
    //        }, 500);
    //    }
    $('.trip-day-itinerary-item .create-note-link').click(function (clickData) {
        var itineraryItemId = $(this).attr('data-itinerary-item-id');
        alert(itineraryItemId);
    });

    $('.itinerary-item-note .edit-note-link').click(function (clickData) {
        var noteId = $(this).attr('data-note-id');
        $.get('/Notes/Edit/' + noteId, function (data) {
            $(data).dialog({
                title: 'Your note',
                dialogClass: 'note-dialog',
                width: 450,
                height: 300,
                modal: true,
                buttons: [{
                    text: "Save",
                    click: function () { $(this).submit(); }
                }]
            });
        });
    });
});