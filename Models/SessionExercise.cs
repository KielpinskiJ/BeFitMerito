using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    /// <summary>
    /// Represents an exercise performed during a training session.
    /// Links ExerciseType with TrainingSession and contains performance details.
    /// </summary>
    public class SessionExercise
    {
        public int Id { get; set; }

        // Foreign key for ExerciseType
        [Required(ErrorMessage = "Typ ćwiczenia jest wymagany.")]
        [Display(Name = "Typ ćwiczenia")]
        public int ExerciseTypeId { get; set; }

        // Foreign key for TrainingSession
        [Required(ErrorMessage = "Sesja treningowa jest wymagana.")]
        [Display(Name = "Sesja treningowa")]
        public int TrainingSessionId { get; set; }

        [Required(ErrorMessage = "Obciążenie jest wymagane.")]
        [Range(0, 1000, ErrorMessage = "Obciążenie musi być między 0 a 1000 kg.")]
        [Display(Name = "Obciążenie (kg)", Description = "Użyte obciążenie w kilogramach")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Liczba serii jest wymagana.")]
        [Range(1, 100, ErrorMessage = "Liczba serii musi być między 1 a 100.")]
        [Display(Name = "Liczba serii", Description = "Ile serii zostało wykonanych")]
        public int Sets { get; set; }

        [Required(ErrorMessage = "Liczba powtórzeń jest wymagana.")]
        [Range(1, 1000, ErrorMessage = "Liczba powtórzeń musi być między 1 a 1000.")]
        [Display(Name = "Liczba powtórzeń", Description = "Ile powtórzeń w każdej serii")]
        public int Reps { get; set; }

        [StringLength(200, ErrorMessage = "Notatki mogą mieć maksymalnie 200 znaków.")]
        [Display(Name = "Notatki", Description = "Opcjonalne uwagi do ćwiczenia")]
        public string? Notes { get; set; }

        // Navigation properties
        public virtual ExerciseType? ExerciseType { get; set; }
        public virtual TrainingSession? TrainingSession { get; set; }
    }
}

