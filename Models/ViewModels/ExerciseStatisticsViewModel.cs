namespace BeFit.Models.ViewModels
{
    /// <summary>
    /// View model for displaying exercise statistics.
    /// </summary>
    public class ExerciseStatisticsViewModel
    {
        public string ExerciseName { get; set; } = string.Empty;
        public int TimesPerformed { get; set; }
        public int TotalReps { get; set; }
        public decimal AverageWeight { get; set; }
        public decimal MaxWeight { get; set; }
    }

    /// <summary>
    /// Container for user statistics page.
    /// </summary>
    public class UserStatisticsViewModel
    {
        public List<ExerciseStatisticsViewModel> ExerciseStats { get; set; } = new();
        public int TotalSessions { get; set; }
        public int TotalExercises { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}

