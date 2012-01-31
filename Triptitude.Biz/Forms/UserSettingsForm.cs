using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Forms
{
    public class UserSettingsForm : IValidatableObject
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            Email = Email ?? string.Empty;
            bool emailMightBeValid = Regex.IsMatch(Email, @"^\S+@\S+\.\S+$");
            if (!emailMightBeValid)
                yield return new ValidationResult("Email address is invalid.", new[] { "email" });

            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
                yield return new ValidationResult("Password is too short.", new[] { "password" });

            if (string.IsNullOrWhiteSpace(FirstName) || FirstName.Length < 1)
                yield return new ValidationResult("First name is required.", new[] { "firstname" });

            if (string.IsNullOrWhiteSpace(LastName) || LastName.Length < 1)
                yield return new ValidationResult("Last name is required.", new[] { "lastname" });
        }

        public static UserSettingsForm CreateFrom(User user)
        {
            UserSettingsForm form = new UserSettingsForm
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
            return form;
        }
    }
}