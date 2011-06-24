using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Triptitude.Biz.Repos;

namespace Triptitude.Biz.Forms
{
    public class CreateTripForm : IValidatableObject
    {
        //public string Name { get; set; }
        public string FromName { get; set; }
        public int? FromId { get; set; }

        public string ToName { get; set; }
        public int? ToId { get; set; }

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
            //if (string.IsNullOrWhiteSpace(Name))
            //    yield return new ValidationResult("Name is required.", new[] { "name" });

            //if (Name != null && Name.Length > 50)
            //    yield return new ValidationResult("Name is too long. Please use less than 50 characters.", new[] { "name" });

            if (!FromId.HasValue || new DestinationsRepo().Find(FromId.Value) == null)
                yield return new ValidationResult("Destination not found. Start typing a city name, and select from the options provided.", new[] { "from" });

            if (!ToId.HasValue || new DestinationsRepo().Find(ToId.Value) == null)
                yield return new ValidationResult("Destination not found. Start typing a city name, and select from the options provided.", new[] { "to" });

            int tmp;
            if (string.IsNullOrWhiteSpace(NumberOfDays) || !int.TryParse(NumberOfDays, out tmp) || tmp < 1 || tmp > 365)
                yield return new ValidationResult("Number of days is not valid. Please enter a number between 1 and 365.", new[] { "numberofdays" });
        }
    }
}
