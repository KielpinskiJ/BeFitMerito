using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeFit.Data.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteExerciseType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionExercises_ExerciseTypes_ExerciseTypeId",
                table: "SessionExercises");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionExercises_ExerciseTypes_ExerciseTypeId",
                table: "SessionExercises",
                column: "ExerciseTypeId",
                principalTable: "ExerciseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionExercises_ExerciseTypes_ExerciseTypeId",
                table: "SessionExercises");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionExercises_ExerciseTypes_ExerciseTypeId",
                table: "SessionExercises",
                column: "ExerciseTypeId",
                principalTable: "ExerciseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
