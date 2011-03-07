$(function () {
    
    $('input').placeholder();
    
    $('#search form').submit(function () {
        doSearch();
        return false;
    });
    $('#search form input').keyup(function () {
        doSearch();
    });

    var timerid;
    function doSearch() {
        var form = this;
        clearTimeout(timerid);
        timerid = setTimeout(function () {
            $('#search-results').load('/search', $('#search form').serialize());
        }, 500);
    }

});