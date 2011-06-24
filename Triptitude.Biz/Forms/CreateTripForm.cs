using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Triptitude.Biz.Repos;

namespace Triptitude.Biz.Forms
{
    public class CreateTripForm : IValidatableObject
    {
        public string Name { get; set; }
        public string DestinationName { get; set; }
        public int? DestinationId { get; set; }
        public int NumberOfDays { get { return 35; } }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
                yield return new ValidationResult("Name is required.", new[] { "name" });

            if (Name != null && Name.Length > 50)
                yield return new ValidationResult("Name is too long. Please use less than 50 characters.", new[] { "name" });

            if (!DestinationId.HasValue || new DestinationsRepo().Find(DestinationId.Value) == null)
                yield return new ValidationResult("Destination not found. Start typing a city name, and select from the options provided.", new[] { "destination" });
        }
    }
}
