TT.Views.Editor.Main = Backbone.View.extend({
    id: 'editor',
    initialize: function () {
        this.Header = new TT.Views.Editor.Header({ model: this.model });
        this.Tabs = {
            Itinerary: new TT.Views.Editor.Itinerary({ model: this.model, edit: this.options.edit })
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

        this.AddToTripButton = $(this.make('a', { 'class': 'add-activity' }, '+ Add an Activity'));

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
        setTimeout(this.scrollToActive, 50);
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

        this.ActivityForm = new TT.Views.Editor.ActivityForm({ model: this.editing });
        this.ActivityForm.on('activitysaved', this.renderActivityList).render().$el.prependTo(this.el);
        this.ActivityForm.setFocus();
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

            $(this.make('a', { 'class': 'add-activity', title: 'Add an activity on this day' }, '+ Add')).prependTo(dateLi).data('date', date);

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
    sortUpdate: function (e, ui) {
        var firstDate = this.activityList.children('.date').first();
        var tmpDate = null;
        if (firstDate.data('date')) {
            tmpDate = new Date(firstDate.data('date'));
            tmpDate.setDate(tmpDate.getDate() - 1);
        }
        var tmpOrderNumber = 1;

        _.each(this.activityList.children('li'), function (activityLI) {
            var $li = $(activityLI);
            if ($li.is('.date')) {
                tmpDate = $li.data('date');
                tmpOrderNumber = 1;
            } else if ($li.is('.activity')) {
                var activity = $li.data('activity');
                var activityMovedId = $(ui.item).data('activity').id;
                var thisActivityMoved = activity.id == activityMovedId;
                activity.save({ BeginAt: tmpDate, OrderNumber: tmpOrderNumber++, Moved: thisActivityMoved });
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
    tagName: 'form',
    editTemplate: '<form class="activity-form"><div class="title"><label for="activity-form-title">Title</label><input name="Title" id="activity-form-title" tabindex="1" placeholder="enter a title for your activity"></div><div class="when"><label for="activity-form-begin-date">When?</label><input name="BeginDate" id="activity-form-begin-date" tabindex="2"></div><div class="source-url"><label for="activity-form-source-url">Source URL</label><input name="SourceURL" id="activity-form-source-url" tabindex="3" placeholder="http://"></div><div class="notes"><label for="activity-form-notes">Notes</label><textarea name="Notes" id="activity-form-notes" tabindex="4" placeholder="add notes and details..."></textarea><ol></ol></div><div class="buttons"><button type="submit" class="save" tabindex="5">Save</button><button type="button" class="delete" tabindex="6">Delete</button></div></form>',
    initialize: function () {
        this.model.on('change:BeginAt', function () {
            var bdi = this.$el.find('[name="BeginDate"]');
            bdi.datepicker('setDate', TT.Util.ToDatePicker(this.model.get('BeginAt')));
        }, this);
    },
    events: {
        'submit': 'submit',
        'click .delete': 'deleteActivity'
    },
    setFocus: function () {
        var self = this;
        setTimeout(function () { self.$el.find('[name="Title"]').focus(); }, 10);
    },
    submit: function (e) {
        e.preventDefault();
        var self = this;

        var oldBeginAt = this.model.get('BeginAt');
        var bdi = this.$el.find('[name="BeginDate"]');
        var newBeginAt = TT.Util.FromDatePicker(bdi.datepicker('getDate'));

        this.model.set({
            Title: this.$el.find('[name="Title"]').val(),
            SourceURL: this.$el.find('[name="SourceURL"]').val(),
            BeginAt: newBeginAt,
            Note: this.$el.find('[name="Notes"]').val()
        });

        if (!TT.Util.SameDate(oldBeginAt, newBeginAt)) {
            this.model.collection.moveToEnd(this.model);
        }

        this.model.save(null, {
            success: function () {
                self.trigger('activitysaved');
                self.render();
                self.model.unset('Note', { silent: true });
                $(self.make('div', { 'class': 'msg success' }, 'Activity Saved!')).appendTo(self.el);
                var another = $('<a class="add-activity another">add another activity</a>').appendTo(self.el);
                another.data('date', self.model.get('BeginAt'));
                setTimeout(function () { another.effect('highlight', { color: '#86D0FE' }, 3000); }, 1000);
            },
            error: function () {
                $(self.make('div', { 'class': 'msg error' }, "Woah, there was a problem! Please try again.")).appendTo(self.el);
            }
        });
    },
    deleteActivity: function () {
        this.model.collection.remove(this.model);
    },
    noteLITemplate: _.template('<li><div class="when"><%= n.RelativeTime %></div><a class="who" href="<%= n.User.DetailsURL %>" target="blank"><%= n.User.Name %></a> <div class="text"><%= n.Text %></div></li>'),
    render: function () {
        
        if (!this.model) {
            this.$el.html('<h3>No Activity Selected</h3>');
            return this;
        }

        this.$el.html(this.editTemplate);

        var NotesOL = this.$el.find('ol');
        this.model.get('Notes').each(function (note) {
            var noteHTML = this.noteLITemplate({ n: note.attributes });
            NotesOL.append(noteHTML);
        }, this);

        this.$el.find('[name="Title"]').val(TT.Util.Decode(this.model.get('Title')));
        this.$el.find('[name="BeginDate"]').datepicker().datepicker('setDate', TT.Util.ToDatePicker(this.model.get('BeginAt')));

        this.$el.find('[name="SourceURL"]').val(TT.Util.Decode(this.model.get('SourceURL')));
        if (!this.model.get('SourceURL')) this.$el.find('.source-url').hide();

        return this;
    }
});