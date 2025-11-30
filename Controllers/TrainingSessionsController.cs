using System.Security.Claims;
using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Controllers
{
    /// <summary>
    /// Controller for managing training sessions.
    /// All operations require authentication and are restricted to user's own sessions.
    /// </summary>
    [Authorize]
    public class TrainingSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainingSessionsController(ApplicationDbContext context)
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

        // GET: TrainingSessions
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var sessions = await _context.TrainingSessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.StartDateTime)
                .ToListAsync();

            return View(sessions);
        }

        // GET: TrainingSessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            var trainingSession = await _context.TrainingSessions
                .Include(s => s.SessionExercises)
                    .ThenInclude(se => se.ExerciseType)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (trainingSession == null)
            {
                return NotFound();
            }

            return View(trainingSession);
        }

        // GET: TrainingSessions/Create
        public IActionResult Create()
        {
            // Round to nearest minute for cleaner display
            var now = DateTime.Now;
            var roundedNow = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            
            var session = new TrainingSession
            {
                StartDateTime = roundedNow,
                EndDateTime = roundedNow.AddHours(1)
            };
            return View(session);
        }

        // POST: TrainingSessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StartDateTime,EndDateTime,Notes")] TrainingSession trainingSession)
        {
            // Automatically assign user ID before validation
            trainingSession.UserId = GetUserId();
            
            // Clear any UserId validation errors since we set it programmatically
            ModelState.Remove("UserId");

            // Custom validation for dates
            if (trainingSession.EndDateTime <= trainingSession.StartDateTime)
            {
                ModelState.AddModelError("EndDateTime", "Data zakończenia musi być późniejsza niż data rozpoczęcia.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(trainingSession);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trainingSession);
        }

        // GET: TrainingSessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            var trainingSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (trainingSession == null)
            {
                return NotFound();
            }

            return View(trainingSession);
        }

        // POST: TrainingSessions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDateTime,EndDateTime,Notes")] TrainingSession trainingSession)
        {
            if (id != trainingSession.Id)
            {
                return NotFound();
            }

            var userId = GetUserId();

            // Verify ownership
            var existingSession = await _context.TrainingSessions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (existingSession == null)
            {
                return NotFound();
            }

            // Keep the original user ID
            trainingSession.UserId = userId;
            
            // Clear any UserId validation errors since we set it programmatically
            ModelState.Remove("UserId");

            // Custom validation for dates
            if (trainingSession.EndDateTime <= trainingSession.StartDateTime)
            {
                ModelState.AddModelError("EndDateTime", "Data zakończenia musi być późniejsza niż data rozpoczęcia.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainingSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingSessionExists(trainingSession.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trainingSession);
        }

        // GET: TrainingSessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            var trainingSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (trainingSession == null)
            {
                return NotFound();
            }

            return View(trainingSession);
        }

        // POST: TrainingSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();
            var trainingSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (trainingSession == null)
            {
                return NotFound();
            }

            _context.TrainingSessions.Remove(trainingSession);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainingSessionExists(int id)
        {
            var userId = GetUserId();
            return _context.TrainingSessions.Any(e => e.Id == id && e.UserId == userId);
        }
    }
}

