(function() {
  var triptitude;
  triptitude = {
    page_load: function() {
      $('input').placeholder();
      $('.focus').first().focus();
      $('.date-picker').datepicker();
      $('#search').bind('submit', function(e) {
        var val;
        val = $('input[name="googreference"]', $(this)).val();
        if (val === '') {
          return e.preventDefault();
        }
      });
      BindPlaceAutocomplete(null);
      if (navigator.platform !== 'iPad' && navigator.platform !== 'iPhone' && navigator.platform !== 'iPod') {
        moveScroller();
      }
      return $('#trip-bar-menu li').hover((function() {
        return $(this).children('ul').show();
      }), (function() {
        return $(this).children('ul').hide();
      }));
    }
  };
  $(function() {
    return triptitude.page_load();
  });
}).call(this);
