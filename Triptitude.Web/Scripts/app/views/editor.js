TT.Views.Editor.Main = Backbone.View.extend({
    id: 'editor',
    initialize: function () {

        this.Header = new TT.Views.Editor.Header({ model: this.model });

        this.Tabs = {
            Itinerary: new TT.Views.Editor.Itinerary({ model: this.model }),
            PackingList: new TT.Views.Editor.PackingList({ model: this.model }),
            Settings: new TT.Views.Editor.Settings({ model: this.model })
        };

        this.CurrentTab = this.Tabs.Itinerary;

    },
    render: function () {

        this.$el.empty();

        this.$el.append(this.Header.render().el);

        var tabContainer = $(this.make('div', { class: 'tab-container' })).appendTo(this.el);
        var self = this;
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
        this.activities.on('all', this.renderActivityList, this);
    },
    events: {
        'click .activity': 'activitySelected'
    },
    activitySelected: function (e) {
        this.activityList.find('.selected').removeClass('selected');
        var li = $(e.currentTarget).addClass('selected');
        var activity = li.data('activity');

        //todo: change this:
        this.ActivityForm.remove();
        console.log('this.ActivityForm');
        console.log(this.ActivityForm);
        // creates a zombie from the old form. TODO: unbind the zombie (though normall unbinding zombies gets you eaten)
        this.ActivityForm = new TT.Views.Editor.ActivityForm({ model: activity });
        this.ActivityForm.render().$el.appendTo(this.el);

    },
    render: function () {
        this.renderActivityList();
        return this;
    },
    renderActivityList: function () {
        this.activityList.empty();
        var self = this;
        this.activities.each(function (activity) {
            var li = $(self.make('li', { class: 'activity' })).appendTo(self.activityList);
            li.data('activity', activity);
            li.append(self.make('h4', null, activity.createTitle()));
        });
        this.activityList.prependTo(this.el);
    }
});

TT.Views.Editor.ActivityForm = Backbone.View.extend({
    className: 'activity-form',
    initialize: function () {
        this.TitleInput = $(this.make('input', { name: 'Title' }));
        this.SaveButton = $(this.make('button', { type: 'submit', class: 'save' }, 'Save'));
        this.DeleteButton = $(this.make('button', { type: 'submit', class: 'delete' }, 'Delete'));
    },
    events: {
        'click .save': 'save',
        'click .delete': 'destroy'
    },
    save: function () {
        this.model.set('Title', this.TitleInput.val());
        console.log(this.model);
        this.model.save();
        console.log('save!');
    },
    destroy: function () {
        console.log('destroy!');
        this.model.destroy();
    },
    render: function () {
        this.$el.text(this.model.get('Title'));
        this.TitleInput.val(this.model.get('Title')).appendTo(this.el);

        this.SaveButton.appendTo(this.el);
        this.DeleteButton.appendTo(this.el);
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