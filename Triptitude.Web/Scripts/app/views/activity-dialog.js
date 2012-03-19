TT.Views.ActivityDialog = Backbone.View.extend({
    id: 'super-dialog-container',
    initialize: function () {
        this.superDialog = $(this.make('div', { id: 'super-dialog' }));
        $(document).on('keyup', { thisView: this }, this.bodyKeyup);
    },
    bodyKeyup: function (e) {
        if (e.which == 27) {
            $(document).off('keyup', e.data.thisView.bodyKeyup);
            e.data.thisView.remove();
        }
    },
    render: function () {
        
        var overlay = this.make('div', { id: 'super-dialog-overlay' });
        this.$el.append(overlay);

        this.superDialog.html('<p>ActivityDialog</p>');
        this.$el.append(this.superDialog);

        return this;
    }
});