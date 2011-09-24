﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Triptitude.Biz.Repos;

namespace Triptitude.Biz.Forms
{
    public class CreateTripForm : IValidatableObject
    {
        public string FromName { get; set; }
        public string FromId { get; set; }

        public string ToName { get; set; }
        public string ToId { get; set; }

        public string NumberOfDays { get; set; }
        public int NumDays
        {
            get
            {
                int tmp = 0;
                if (!string.IsNullOrWhiteSpace(NumberOfDays))
                    int.TryParse(NumberOfDays, out tmp);

                return tmp;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(FromId))
                yield return new ValidationResult("Place not found. Start typing and select from the options provided.", new[] { "from" });

            if (string.IsNullOrWhiteSpace(ToId))
                yield return new ValidationResult("Place not found. Start typing and select from the options provided.", new[] { "to" });

            int tmp;
            if (string.IsNullOrWhiteSpace(NumberOfDays) || !int.TryParse(NumberOfDays, out tmp) || tmp < 1 || tmp > 365)
                yield return new ValidationResult("Number of days is not valid. Please enter a number between 1 and 365.", new[] { "numberofdays" });
        }
    }
}
