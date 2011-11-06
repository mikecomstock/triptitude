triptitude =
	page_load: ->
		$('input').placeholder()
		$('.focus').first().focus()
		$('.date-picker').datepicker()

		$('#search').bind 'submit', (e) ->
			val = $('input[name="googreference"]', $(this)).val()
			e.preventDefault() if val == ''

		BindPlaceAutocomplete(null);
		if (navigator.platform != 'iPad' && navigator.platform != 'iPhone' && navigator.platform != 'iPod')
	        moveScroller()

		$('#trip-bar-menu li').hover(
			(->$(this).children('ul').show()),
			(-> $(this).children('ul').hide())
		)
		
$ -> triptitude.page_load()
	 