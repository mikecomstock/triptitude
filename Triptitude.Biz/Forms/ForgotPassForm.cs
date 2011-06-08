using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Biz.Forms
{
    public class ForgotPassForm : IValidatableObject
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            User findByEmail = new UsersRepo().FindByEmail(Email);
            if (findByEmail == null)
                yield return new ValidationResult("We don't have your address on file.", new[] { "email" });
        }
    }
}