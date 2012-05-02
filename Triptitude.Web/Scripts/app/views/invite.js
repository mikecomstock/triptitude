TT.Views.InviteDialog = Backbone.View.extend({
    id: 'invite-dialog-container',
    initialize: function () {
        this.tripID = this.options.tripID;
        this.fNameInput = $(this.make('input', { placeholder: 'First Name' }));
        this.lNameInput = $(this.make('input', { placeholder: 'Last Name' }));
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

        this.$el.append(this.fNameInput);
        this.$el.append(this.lNameInput);
        this.$el.append(this.emailInput);
        this.$el.append(this.submitButton);

        return this;
    }
});