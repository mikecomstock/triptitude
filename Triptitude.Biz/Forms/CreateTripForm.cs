using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Forms
{
    //public class CreateTripForm : IValidatableObject
    //{
    //    public string ToName { get; set; }
    //    public string ToGoogReference { get; set; }
    //    public string ToGoogId { get; set; }
    //    public UserTrip.UserTripVisibility Visibility { get; set; }

    //    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    //    {
    //        if (string.IsNullOrWhiteSpace(ToGoogReference))
    //            yield return new ValidationResult("Place not found. Start typing and select from the options provided.", new[] { "to" });
    //    }
    //}

    public class NewCreateTripForm : IValidatableObject
    {
        public string Name { get; set; }
        public string Destination { get; set; }
        public UserTrip.UserTripVisibility Visibility { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name) && string.IsNullOrWhiteSpace(Destination))
                yield return new ValidationResult("Please enter a name for your trip.", new[] { "name" });
        }
    }
}
