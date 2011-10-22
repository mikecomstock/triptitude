using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Triptitude.Biz.Forms
{
    public class TripSettingsForm : IValidatableObject
    {
        public string Name { get; set; }
        public string BeginDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
                yield return new ValidationResult("Name is required.", new[] { "name" });

            if (Name != null && Name.Length > 50)
                yield return new ValidationResult("Name is too long. Please use less than 50 characters.", new[] { "name" });

            DateTime tmp;
            if (!string.IsNullOrWhiteSpace(BeginDate) && !DateTime.TryParse(BeginDate, out tmp))
                yield return new ValidationResult("Please enter a valid date.", new[] { "begindate" });
        }
    }
}