using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    /// <summary>
    /// Represents an exercise type (e.g., squat, bench press, deadlift).
    /// </summary>
    public class ExerciseType
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa ćwiczenia jest wymagana.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nazwa musi mieć od 2 do 100 znaków.")]
        [Display(Name = "Nazwa ćwiczenia", Description = "Nazwa typu ćwiczenia, np. Przysiad, Wyciskanie")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Opis może mieć maksymalnie 500 znaków.")]
        [Display(Name = "Opis", Description = "Opcjonalny opis ćwiczenia")]
        public string? Description { get; set; }

        // Navigation property
        public virtual ICollection<SessionExercise> SessionExercises { get; set; } = new List<SessionExercise>();
    }
}

