using System.Security.Claims;
using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Controllers
{
    /// <summary>
    /// Controller for managing exercises within training sessions.
    /// All operations require authentication and are restricted to user's own exercises.
    /// </summary>
    [Authorize]
    public class SessionExercisesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SessionExercisesController(ApplicationDbContext context)
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

        /// <summary>
        /// Checks if the user owns the specified training session.
        /// </summary>
        private async Task<bool> UserOwnsSession(int sessionId)
        {
            var userId = GetUserId();
            return await _context.TrainingSessions.AnyAsync(s => s.Id == sessionId && s.UserId == userId);
        }

        /// <summary>
        /// Prepares SelectLists for exercise types and user's training sessions.
        /// </summary>
        private async Task PrepareSelectLists(int? selectedExerciseTypeId = null, int? selectedSessionId = null)
        {
            var userId = GetUserId();

            ViewBag.ExerciseTypeId = new SelectList(
                await _context.ExerciseTypes.OrderBy(e => e.Name).ToListAsync(),
                "Id",
                "Name",
                selectedExerciseTypeId);

            ViewBag.TrainingSessionId = new SelectList(
                await _context.TrainingSessions
                    .Where(s => s.UserId == userId)
                    .OrderByDescending(s => s.StartDateTime)
                    .Select(s => new
                    {
                        s.Id,
                        DisplayName = s.StartDateTime.ToString("dd.MM.yyyy HH:mm") + " - " + s.EndDateTime.ToString("HH:mm")
                    })
                    .ToListAsync(),
                "Id",
                "DisplayName",
                selectedSessionId);
        }

        // GET: SessionExercises
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var exercises = await _context.SessionExercises
                .Include(s => s.ExerciseType)
                .Include(s => s.TrainingSession)
                .Where(s => s.TrainingSession!.UserId == userId)
                .OrderByDescending(s => s.TrainingSession!.StartDateTime)
                .ToListAsync();

            return View(exercises);
        }

        // GET: SessionExercises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            var sessionExercise = await _context.SessionExercises
                .Include(s => s.ExerciseType)
                .Include(s => s.TrainingSession)
                .FirstOrDefaultAsync(m => m.Id == id && m.TrainingSession!.UserId == userId);

            if (sessionExercise == null)
            {
                return NotFound();
            }

            return View(sessionExercise);
        }

        // GET: SessionExercises/Create
        public async Task<IActionResult> Create(int? sessionId)
        {
            await PrepareSelectLists(selectedSessionId: sessionId);
            
            var model = new SessionExercise();
            if (sessionId.HasValue)
            {
                model.TrainingSessionId = sessionId.Value;
            }
            
            return View(model);
        }

        // POST: SessionExercises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExerciseTypeId,TrainingSessionId,Weight,Sets,Reps,Notes")] SessionExercise sessionExercise)
        {
            // Verify ownership of the session
            if (!await UserOwnsSession(sessionExercise.TrainingSessionId))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Add(sessionExercise);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "TrainingSessions", new { id = sessionExercise.TrainingSessionId });
            }

            await PrepareSelectLists(sessionExercise.ExerciseTypeId, sessionExercise.TrainingSessionId);
            return View(sessionExercise);
        }

        // GET: SessionExercises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            var sessionExercise = await _context.SessionExercises
                .Include(s => s.TrainingSession)
                .FirstOrDefaultAsync(s => s.Id == id && s.TrainingSession!.UserId == userId);

            if (sessionExercise == null)
            {
                return NotFound();
            }

            await PrepareSelectLists(sessionExercise.ExerciseTypeId, sessionExercise.TrainingSessionId);
            return View(sessionExercise);
        }

        // POST: SessionExercises/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ExerciseTypeId,TrainingSessionId,Weight,Sets,Reps,Notes")] SessionExercise sessionExercise)
        {
            if (id != sessionExercise.Id)
            {
                return NotFound();
            }

            var userId = GetUserId();

            // Verify ownership of the original exercise
            var existingExercise = await _context.SessionExercises
                .Include(s => s.TrainingSession)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id && s.TrainingSession!.UserId == userId);

            if (existingExercise == null)
            {
                return NotFound();
            }

            // Verify ownership of the new session (if changed)
            if (!await UserOwnsSession(sessionExercise.TrainingSessionId))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sessionExercise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionExerciseExists(sessionExercise.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "TrainingSessions", new { id = sessionExercise.TrainingSessionId });
            }

            await PrepareSelectLists(sessionExercise.ExerciseTypeId, sessionExercise.TrainingSessionId);
            return View(sessionExercise);
        }

        // GET: SessionExercises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            var sessionExercise = await _context.SessionExercises
                .Include(s => s.ExerciseType)
                .Include(s => s.TrainingSession)
                .FirstOrDefaultAsync(m => m.Id == id && m.TrainingSession!.UserId == userId);

            if (sessionExercise == null)
            {
                return NotFound();
            }

            return View(sessionExercise);
        }

        // POST: SessionExercises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();
            var sessionExercise = await _context.SessionExercises
                .Include(s => s.TrainingSession)
                .FirstOrDefaultAsync(s => s.Id == id && s.TrainingSession!.UserId == userId);

            if (sessionExercise == null)
            {
                return NotFound();
            }

            var sessionId = sessionExercise.TrainingSessionId;
            _context.SessionExercises.Remove(sessionExercise);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "TrainingSessions", new { id = sessionId });
        }

        private bool SessionExerciseExists(int id)
        {
            return _context.SessionExercises.Any(e => e.Id == id);
        }
    }
}

