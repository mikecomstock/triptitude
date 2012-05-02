using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Forms
{
    public class TripSettingsForm : IValidatableObject
    {
        public string Name { get; set; }
        public UserTrip.UserTripVisibility Visibility { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
                yield return new ValidationResult("Name is required.", new[] { "name" });

            if (Name != null && Name.Length > 50)
                yield return new ValidationResult("Name is too long. Please use less than 50 characters.", new[] { "name" });
        }
    }
}