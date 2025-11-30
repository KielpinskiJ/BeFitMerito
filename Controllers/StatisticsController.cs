using System.Security.Claims;
using BeFit.Data;
using BeFit.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Controllers
{
    /// <summary>
    /// Controller for displaying user exercise statistics.
    /// Shows data from the last 4 weeks only.
    /// </summary>
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the current user's ID.
        /// </summary>
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        // GET: Statistics
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var fourWeeksAgo = DateTime.Now.AddDays(-28);
            var now = DateTime.Now;

            // Get all sessions from the last 4 weeks for this user
            var recentSessions = await _context.TrainingSessions
                .Include(s => s.SessionExercises)
                    .ThenInclude(se => se.ExerciseType)
                .Where(s => s.UserId == userId && s.StartDateTime >= fourWeeksAgo)
                .ToListAsync();

            // Calculate statistics per exercise type
            var exerciseStats = recentSessions
                .SelectMany(s => s.SessionExercises)
                .GroupBy(se => se.ExerciseType?.Name ?? "Nieznane")
                .Select(g => new ExerciseStatisticsViewModel
                {
                    ExerciseName = g.Key,
                    TimesPerformed = g.Count(),
                    TotalReps = g.Sum(se => se.Sets * se.Reps),
                    AverageWeight = g.Any() ? Math.Round(g.Average(se => se.Weight), 2) : 0,
                    MaxWeight = g.Any() ? g.Max(se => se.Weight) : 0
                })
                .OrderByDescending(s => s.TimesPerformed)
                .ToList();

            var viewModel = new UserStatisticsViewModel
            {
                ExerciseStats = exerciseStats,
                TotalSessions = recentSessions.Count,
                TotalExercises = recentSessions.Sum(s => s.SessionExercises.Count),
                FromDate = fourWeeksAgo,
                ToDate = now
            };

            return View(viewModel);
        }
    }
}

