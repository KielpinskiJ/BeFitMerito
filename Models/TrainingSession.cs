using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BeFit.Models
{
    /// <summary>
    /// Represents a training session with start and end time.
    /// </summary>
    public class TrainingSession
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Data rozpoczęcia jest wymagana.")]
        [Display(Name = "Data i czas rozpoczęcia", Description = "Kiedy rozpoczęła się sesja treningowa")]
        [DataType(DataType.DateTime)]
        public DateTime StartDateTime { get; set; }

        [Required(ErrorMessage = "Data zakończenia jest wymagana.")]
        [Display(Name = "Data i czas zakończenia", Description = "Kiedy zakończyła się sesja treningowa")]
        [DataType(DataType.DateTime)]
        public DateTime EndDateTime { get; set; }

        [StringLength(200, ErrorMessage = "Notatki mogą mieć maksymalnie 200 znaków.")]
        [Display(Name = "Notatki", Description = "Opcjonalne uwagi do sesji")]
        public string? Notes { get; set; }

        // Foreign key for user - automatically assigned
        [Required]
        public string UserId { get; set; } = string.Empty;

        // Navigation property
        public virtual IdentityUser? User { get; set; }

        // Navigation property for exercises in this session
        public virtual ICollection<SessionExercise> SessionExercises { get; set; } = new List<SessionExercise>();

        /// <summary>
        /// Custom validation to ensure EndDateTime is after StartDateTime.
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDateTime <= StartDateTime)
            {
                yield return new ValidationResult(
                    "Data zakończenia musi być późniejsza niż data rozpoczęcia.",
                    new[] { nameof(EndDateTime) });
            }
        }
    }
}

