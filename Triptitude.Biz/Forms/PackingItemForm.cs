using System.ComponentModel.DataAnnotations;

namespace Triptitude.Biz.Forms
{
    public class PackingItemForm
    {
        public int? PackingItemId { get; set; }
        public int TripId { get; set; }
        public int? ActivityId { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        public string Note { get; set; }
        public int Visibility_Id { get; set; }
        public string TagString { get; set; }
    }
}