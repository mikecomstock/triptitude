TT.Views.InviteDialog = Backbone.View.extend({
    id: 'invite-dialog-container',
    initialize: function () {
        this.tripID = this.options.tripID;
        this.nameInput = $(this.make('input', { placeholder: 'Name' }));
        this.emailInput = $(this.make('input', { placeholder: 'Email' }));
        this.submitButton = $(this.make('input', { type: 'submit', 'class': 'submit' }));
    },
    events: {
        'click .submit': 'submit'
    },
    submit: function () {
        console.log('submitted!');
    },
    render: function () {
        this.$el.text('Invite Someone to trip ' + this.tripID );

        this.$el.append(this.nameInput);
        this.$el.append(this.emailInput);
        this.$el.append(this.submitButton);

        return this;
    }
});