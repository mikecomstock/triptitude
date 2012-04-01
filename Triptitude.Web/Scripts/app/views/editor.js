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
    edit: function (activity) {
        // TODO: add code to switch to the Itinerary tab
        this.Tabs.Itinerary.edit(activity);
    },
    render: function () {

        this.$el.empty();

        this.$el.append(this.Header.render().el);

        var tabContainer = $(this.make('div', { class: 'tab-container' })).appendTo(this.el);

        _.each(this.Tabs, function (tab) {
            tab.render();
            tab.$el.hide();
            tabContainer.append(tab.el);
        });

        this.CurrentTab.$el.show();

        return this;
    }
});

TT.Views.Editor.Header = Backbone.View.extend({
    tagName: 'header',
    render: function () {
        var currentTrip = this.model.getCurrentTrip();
        var tripName = currentTrip.get('Name');
        $('<h1>').text(tripName).appendTo(this.el);
        $('<p>').text(this.model.get('Email')).appendTo(this.el);
        return this;
    }
});

TT.Views.Editor.Itinerary = Backbone.View.extend({
    id: 'editor-itinerary',
    initialize: function () {

        this.ActivityForm = new TT.Views.Editor.ActivityForm();
        this.activityList = $(this.make('ul', { class: 'activities' }));

        this.activities = this.model.getCurrentTrip().get('Activities');
        this.editing = this.options.edit || this.activities.first();

        this.activities.on('destroy', this.activityDestroyed, this);
        this.activities.on('change', this.renderActivityList, this);
    },
    events: {
        'click .activity': 'activitySelected'
    },
    activitySelected: function (e) {
        this.activityList.find('.selected').removeClass('selected');
        var li = $(e.currentTarget).addClass('selected');
        var activity = li.data('activity');
        this.editing = activity;
        this.renderForm();
    },
    activityDestroyed: function (activity) {
        if (activity == this.editing) {
            this.editing = this.activities.last();
            this.renderForm();
        }
        this.renderActivityList();
    },
    renderForm: function () {
        //todo: change this! creates a zombie from the old form. TODO: unbind the zombie (though normall unbinding zombies gets you eaten)
        this.ActivityForm.remove();
        this.ActivityForm = new TT.Views.Editor.ActivityForm({ model: this.editing });
        this.ActivityForm.render().$el.appendTo(this.el);
    },
    render: function () {
        console.log('render');
        this.renderForm();
        this.renderActivityList();
        return this;
    },
    renderActivityList: function () {
        //console.log('renderactivitylist');

        this.activityList.empty();
        var self = this;
        this.activities.each(function (activity) {
            var li = $(self.make('li', { class: 'activity' })).appendTo(self.activityList);
            li.data('activity', activity);
            li.append(self.make('h4', null, activity.createTitle()));

            if (activity == self.editing)
                li.addClass('selected');
        });
        this.activityList.prependTo(this.el);
    }
});

TT.Views.Editor.ActivityForm = Backbone.View.extend({
    className: 'activity-form',
    initialize: function () {

        this.TitleInput = $(this.make('input', { name: 'Title', id: 'activity-form-title' }));
        this.BeginDateInput = $(this.make('input', { name: 'BeginDate', id: 'activity-form-begin-date' }));
        this.EndDateInput = $(this.make('input', { name: 'EndDate', id: 'activity-form-end-date' }));
        this.SourceURLInput = $(this.make('input', { name: 'SourceURL', id: 'activity-form-source-url' }));

        this.SaveButton = $(this.make('button', { type: 'submit', class: 'save' }, 'Save'));
        this.DeleteButton = $(this.make('button', { type: 'submit', class: 'delete' }, 'Delete'));

    },
    events: {
        'click .save': 'save',
        'click .delete': 'destroy'
    },
    save: function () {
        var self = this;

        this.model.set('Title', this.TitleInput.val());
        this.model.set('BeginAt', this.BeginDateInput.val());
        this.model.set('EndAt', this.EndDateInput.val());
        this.model.set('SourceURL', this.SourceURLInput.val());

        //console.log('saving model', this.model);
        this.model.save(null, {
            success: function () {
                var msg = $(self.make('div', { class: 'msg success' }, "Activity Saved!")).appendTo(self.el);
                setTimeout(function () { msg.fadeOut(1000); }, 3000);
            },
            error: function () {
                var msg = $(self.make('div', { class: 'msg error' }, "Woah, there was a problem! Please try again.")).appendTo(self.el);
                setTimeout(function () { msg.fadeOut(1000); }, 3000);
            }
        });
    },
    destroy: function () {
        //console.log('destroying model', this.model);
        this.model.destroy();
    },
    render: function () {
        var self = this;
        var decodedTitle = $('<div>').html(this.model.get('Title')).text();
        var beginDate = this.model.get('BeginAt');
        var endDate = this.model.get('EndAt');

        var p = {};
        var newP = function (cssClass) { return $('<p>').appendTo(self.el).addClass(cssClass); };

        p.Title = newP('title');
        $(this.make('label', { 'for': this.TitleInput.attr('id') }, 'Title')).appendTo(p.Title);
        this.TitleInput.val(decodedTitle).appendTo(p.Title);

        p.When = newP('when');
        $(this.make('label', { 'for': this.BeginDateInput.attr('id') }, 'When')).appendTo(p.When);
        var options = {
            onSelect: function (selectedDate) {
                console.log('onSelect', this);
                var option = this.id == "activity-form-begin-date" ? "minDate" : "maxDate",
                    instance = $(this).data("datepicker"),
                    date = $.datepicker.parseDate(
                        instance.settings.dateFormat ||
                            $.datepicker._defaults.dateFormat,
                        selectedDate, instance.settings);
                $('#activity-form-begin-date, #activity-form-end-date').not(this).datepicker("option", option, date);
            }
        };
        this.BeginDateInput.appendTo(p.When).datepicker(options).datepicker('setDate', beginDate);
        this.EndDateInput.appendTo(p.When).datepicker(options).datepicker('setDate', endDate);

        p.SourceURL = newP('source-url');
        var sourceURL = this.model.get('SourceURL');
        $(this.make('label', { 'for': this.SourceURLInput.attr('id') }, 'Source URL')).appendTo(p.SourceURL);
        this.SourceURLInput.val(sourceURL).appendTo(p.SourceURL);

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