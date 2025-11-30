using BeFit.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ExerciseType> ExerciseTypes { get; set; }
        public DbSet<TrainingSession> TrainingSessions { get; set; }
        public DbSet<SessionExercise> SessionExercises { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure ExerciseType
            builder.Entity<ExerciseType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Configure TrainingSession
            builder.Entity<TrainingSession>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Notes).HasMaxLength(200);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure SessionExercise
            builder.Entity<SessionExercise>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Weight).HasPrecision(10, 2);
                entity.Property(e => e.Notes).HasMaxLength(200);

                entity.HasOne(e => e.ExerciseType)
                    .WithMany(et => et.SessionExercises)
                    .HasForeignKey(e => e.ExerciseTypeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.TrainingSession)
                    .WithMany(ts => ts.SessionExercises)
                    .HasForeignKey(e => e.TrainingSessionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
