﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Triptitude.Biz.Forms
{
    public class UserSettingsForm : IValidatableObject
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            Email = Email ?? string.Empty;
            bool emailMightBeValid = Regex.IsMatch(Email, @"^\S+@\S+\.\S+$");
            if (!emailMightBeValid)
                yield return new ValidationResult("Email address is invalid.", new[] { "email" });

            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
                yield return new ValidationResult("Password must be at least 6 characters.", new[] { "password" });
        }
    }
}