TT.Views.Editor.Main = Backbone.View.extend({
    id: 'editor',
    initialize: function () {
        this.Header = new TT.Views.Editor.Header({ model: this.model });
        this.Tabs = {
            Itinerary: new TT.Views.Editor.Itinerary({ model: this.model, edit: this.options.edit }),
            PackingList: new TT.Views.Editor.PackingList({ model: this.model }),
            Settings: new TT.Views.Editor.Settings({ model: this.model })
        };
        this.CurrentTab = this.Tabs.Itinerary;
    },
    render: function () {
        var self = this;
        this.$el.empty();
        this.$el.append(this.Header.render().el);

        var tabContainer = $(this.make('div', { 'class': 'tab-container' })).appendTo(this.el);
        _.each(this.Tabs, function (tab) {
            tab.render();
            if (tab != self.CurrentTab) tab.$el.hide();
            tabContainer.append(tab.el);
        });

        return this;
    }
});

TT.Views.Editor.Header = Backbone.View.extend({
    tagName: 'header',
    render: function () {
        var tripName = this.model.get('Name');
        $('<h1>').text(tripName).appendTo(this.el);
        return this;
    }
});

TT.Views.Editor.Itinerary = Backbone.View.extend({
    id: 'editor-itinerary',
    initialize: function () {
        this.activityList = $(this.make('ul', { 'class': 'activity-list' }))
            .sortable({ placeholder: 'sort-placeholder' });

        this.AddToTripButton = $(this.make('div', { 'class': 'add-activity' }, '+ Add an Activity'));

        this.activities = this.model.get('Activities').on('remove', this.activityRemoved, this);
        this.editing = this.options.edit || this.activities.first();

        _.bindAll(this);
    },
    events: {
        'click .activity': 'activitySelected',
        'click .add-activity': 'addActivity',
        'sortupdate': 'sortUpdate'
    },
    activitySelected: function (e) {
        this.activityList.find('.selected').removeClass('selected');
        var li = $(e.currentTarget).addClass('selected');
        var activity = li.data('activity');
        this.editing = activity;
        this.renderForm();
    },
    activityRemoved: function (activity, collection, options) {
        activity.destroy();
        if (activity == this.editing) {
            this.editing = null;
            this.renderForm();
        }

        this.renderActivityList();
    },
    addActivity: function (e) {
        var date = $(e.currentTarget).data('date');

        var newActivity = new TT.Models.Activity({ TripID: this.model.id, BeginAt: date });
        this.model.get('Activities').add(newActivity).moveToEnd(newActivity);
        this.editing = newActivity;
        this.renderActivityList();
        this.renderForm();
    },
    render: function () {
        this.AddToTripButton.prependTo(this.el);
        this.renderForm();
        this.renderActivityList();
        setTimeout(this.scrollToActive, 50);
        return this;
    },
    renderForm: function () {
        //TODO: unbind the zombie form before removing (though normally unbinding zombies gets you eaten)
        if (this.ActivityForm) this.ActivityForm.remove();

        this.ActivityForm = new TT.Views.Editor.ActivityForm({ model: this.editing })
            .on('activitysaved', this.renderActivityList)
            .render().$el.prependTo(this.el);
    },
    renderActivityList: function () {
        var self = this;

        var top = this.activityList.scrollTop();
        this.activityList.empty();

        var dates = this.activities.dates();
        _.each(dates, function (date) {
            var dateLi = $(this.make('li', { 'class': 'date' }))
                .data('date', date)
                .appendTo(this.activityList);
            var dateText = date ? TT.Util.FormatDate(date) : 'Unscheduled Activities';
            dateLi.text(dateText);

            $(this.make('div', { 'class': 'add-activity' }, '+')).prependTo(dateLi).data('date', date);

            var dateActivities = self.activities.onDate(date);
            dateActivities = _.sortBy(dateActivities, function (a) { return a.get('OrderNumber'); });
            _.each(dateActivities, function (activity) {
                var li = $(this.make('li', { 'class': 'activity' })).appendTo(this.activityList);
                li.data('activity', activity);
                li.append(this.make('h4', null, activity.createTitle()));

                if (activity == self.editing)
                    li.addClass('selected');
            }, this);

        }, this);

        this.activityList.insertAfter(this.AddToTripButton);
        this.activityList.scrollTop(top);

    },
    sortUpdate: function () {
        var firstDate = this.activityList.children('.date').first();
        var tmpDate = new Date(firstDate.data('date'));
        tmpDate.setDate(tmpDate.getDate() - 1);
        var tmpOrderNumber = 1;

        _.each(this.activityList.children('li'), function (activityLI) {
            var $li = $(activityLI);
            if ($li.is('.date')) {
                tmpDate = $li.data('date');
                tmpOrderNumber = 1;
            } else if ($li.is('.activity')) {
                var activity = $li.data('activity');

                activity.set({ BeginAt: tmpDate, OrderNumber: tmpOrderNumber++ });
                activity.save();
            }
        }, this);

        this.renderActivityList();
    },
    scrollToActive: function () {
        var editingElement = this.activityList.find('.selected');
        this.activityList.scrollTop(editingElement.offset().top - 300);
    }
});

TT.Views.Editor.ActivityForm = Backbone.View.extend({
    className: 'activity-form',
    initialize: function () {
        this.TitleInput = $(this.make('input', { name: 'Title', id: 'activity-form-title', placeholder: 'enter a title for your activity' }));
        this.SourceURLInput = $(this.make('input', { name: 'SourceURL', id: 'activity-form-source-url' }));

        this.BeginDateInput = $(this.make('input', { name: 'BeginDate', id: 'activity-form-begin-date' }));
        this.EndDateInput = $(this.make('input', { name: 'EndDate', id: 'activity-form-end-date' }));

        if (this.model) {
            this.model.on('change:BeginAt', function () {
                this.BeginDateInput.datepicker('setDate', TT.Util.ToDatePicker(this.model.get('BeginAt')));
            }, this);
        }

        this.TagsInput = $(this.make('input', { Name: 'Tags', id: 'activity-form-tags' }));
        this.PlacesInput = $(this.make('input', { Name: 'Places', id: 'activity-form-places', placeholder: 'add a place...' }));
        this.NotesInput = $(this.make('textarea', { Name: 'Notes', id: 'activity-form-notes', placeholder: 'add a note...' }));

        this.SaveButton = $(this.make('button', { type: 'submit', 'class': 'save' }, 'Save'));
        this.DeleteButton = $(this.make('button', { type: 'submit', 'class': 'delete' }, 'Delete'));

        _.bindAll(this);
    },
    events: {
        'click .save': 'save',
        'click .delete': 'deleteActivity'
    },
    save: function () {
        var self = this;

        var oldBeginAt = this.model.get('BeginAt');
        var newBeginAt = TT.Util.FromDatePicker(this.BeginDateInput.datepicker('getDate'));

        this.model.set({
            Title: this.TitleInput.val(),
            SourceURL: this.SourceURLInput.val(),
            BeginAt: newBeginAt
        });

        if (!TT.Util.SameDate(oldBeginAt, newBeginAt)) {
            this.model.collection.moveToEnd(this.model);
        }

        this.model.save(null, {
            success: function () {
                var msg = $(self.make('div', { 'class': 'msg success' }, "Activity Saved!")).appendTo(self.el);
                setTimeout(function () { msg.fadeOut(1000); }, 3000);
                self.trigger('activitysaved');
            },
            error: function () {
                var msg = $(self.make('div', { 'class': 'msg error' }, "Woah, there was a problem! Please try again.")).appendTo(self.el);
                setTimeout(function () { msg.fadeOut(1000); }, 3000);
            }
        });
    },
    deleteActivity: function () {
        this.model.collection.remove(this.model);
    },
    render: function () {
        var self = this;
        if (!this.model) {
            this.$el.html('<h3>No Activity Selected</h3>');
            return this;
        }

        var decodedTitle = $('<div>').html(this.model.get('Title')).text();
        var beginDate = this.model.get('BeginAt');
        //        var endDate = this.model.get('EndAt');

        var p = {};
        var newP = function (cssClass) { return $('<div>').appendTo(self.el).addClass(cssClass); };

        p.Title = newP('title');
        $(this.make('label', { 'for': this.TitleInput.attr('id') }, 'Title')).appendTo(p.Title);
        this.TitleInput.val(decodedTitle).appendTo(p.Title);
        setTimeout(function () {
            self.TitleInput.focus();
        }, 50);

        p.SourceURL = newP('source-url');
        var sourceURL = this.model.get('SourceURL') || '';
        $(this.make('label', { 'for': this.SourceURLInput.attr('id') }, 'Source URL')).appendTo(p.SourceURL);
        this.SourceURLInput.val(sourceURL).appendTo(p.SourceURL);

        p.When = newP('when');
        $(this.make('label', { 'for': this.BeginDateInput.attr('id') }, 'When')).appendTo(p.When);
        var options = {
            onSelect: function (selectedDate) {
                var option = this.id == "activity-form-begin-date" ? "minDate" : "maxDate",
                    instance = $(this).data("datepicker"),
                    date = $.datepicker.parseDate(
                        instance.settings.dateFormat ||
                            $.datepicker._defaults.dateFormat,
                        selectedDate, instance.settings);
                $('#activity-form-begin-date, #activity-form-end-date').not(this).datepicker("option", option, date);
            }
        };
        this.BeginDateInput.appendTo(p.When).datepicker(options).datepicker('setDate', TT.Util.ToDatePicker(beginDate));
        //        this.EndDateInput.appendTo(p.When).datepicker(options).datepicker('setDate', endDate);

        p.Tags = newP('tags');
        $(self.make('label', { 'for': this.TagsInput.attr('id') }, 'Tags')).appendTo(p.Tags);
        this.TagsInput.val(this.model.get('TagString') || '').appendTo(p.Tags);

        p.Places = newP('places');
        $(self.make('label', { 'for': '' }, 'Places')).appendTo(p.Places);
        var placesUL = $(this.make('ul')).appendTo(p.Places);
        var places = this.model.get('Places');
        places.each(function (place) {
            $(self.make('li')).text(place.get('Name')).appendTo(placesUL);
        });
        var placeInputLI = $('<li>').append(this.PlacesInput.val('')).appendTo(placesUL);
        this.PlacesInput.on('keydown', function (e) {
            if (e.which != 13) return;
            var placeText = self.PlacesInput.val();
            $(self.make('li')).text(placeText).insertBefore(placeInputLI);
            self.PlacesInput.val('');
        });

        p.Notes = newP('notes');
        $(self.make('label', { 'for': this.NotesInput.attr('id') }, 'Notes')).appendTo(p.Notes);
        var NotesUL = $(this.make('ul')).appendTo(p.Notes);
        var Notes = this.model.get('Notes');
        var noteLITemplate = '<a class="who" href="/users/<%= note.get("User").ID %>" target="blank"><%= note.get("User").Email %></a> <div><%= note.get("Text") %></div>';
        Notes.each(function (note) {
            var noteHTML = _.template(noteLITemplate, { note: note });
            $(self.make('li', null, noteHTML)).appendTo(NotesUL);
        });

        $('<li>').append(this.NotesInput.val('')).appendTo(NotesUL);

        var buttonContainer = $(this.make('div', { 'class': 'buttons' })).appendTo(this.el);
        this.SaveButton.appendTo(buttonContainer);
        this.DeleteButton.appendTo(buttonContainer);

        return this;
    }
});


TT.Views.Editor.PackingList = Backbone.View.extend({
    id: 'editor-packing-list',
    render: function () {
        this.$el.text('b');
        return this;
    }
});

TT.Views.Editor.Settings = Backbone.View.extend({
    id: 'editor-settings',
    render: function () {
        this.$el.text('c');
        return this;
    }
});