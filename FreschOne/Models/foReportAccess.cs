using System.ComponentModel.DataAnnotations;

namespace FreschOne.Models
{
    public class foReportAccess : IValidatableObject
    {
        public long ID { get; set; }

        [Required(ErrorMessage = "Report is required.")]
        public long ReportID { get; set; }

        public long? UserID { get; set; }
        public long? GroupID { get; set; }

        public string? UserName { get; set; }
        public string? GroupName { get; set; }

        public bool Active { get; set; } = true;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((UserID == null && GroupID == null) || (UserID != null && GroupID != null))
            {
                yield return new ValidationResult(
                    "Please select either a User or a Group, but not both.",
                    new[] { nameof(UserID), nameof(GroupID) });
            }
        }
    }
}
