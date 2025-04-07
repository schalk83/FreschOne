using System.ComponentModel.DataAnnotations;

namespace FreschOne.Models
{
    public class foProcessSteps : IValidatableObject
    {
        public long ID { get; set; }

        [Required(ErrorMessage = "Process is required.")]
        public long ProcessID { get; set; }

        [Required(ErrorMessage = "Step number is required.")]
        [Range(1, 1000, ErrorMessage = "Step number must be between 1 and 1000.")]
        public decimal StepNo { get; set; }

        [Required(ErrorMessage = "Step description is required.")]
        [StringLength(255)]
        public string StepDescription { get; set; }

        public long? AssignGroupID { get; set; }

        public long? AssignUserID { get; set; }

        public bool Active { get; set; } = true;

        // Optional navigation/display properties
        public string? GroupName { get; set; }
        public string? UserName { get; set; }
        public string? ProcessName { get; set; }

        // ✅ Custom validation logic
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((AssignGroupID == null && AssignUserID == null) ||
                (AssignGroupID != null && AssignUserID != null))
            {
                yield return new ValidationResult(
                    "You must assign either a Group or a User, but not both.",
                    new[] { nameof(AssignGroupID), nameof(AssignUserID) });
            }
        }
    }
}
