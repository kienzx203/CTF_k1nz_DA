using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GZCTF.Models.Data;

/// <summary>
/// Practice mapping: which exercises are available to a class.
/// </summary>
[PrimaryKey(nameof(ClassId), nameof(ExerciseId))]
public class ClassExercise
{
    [Required] public int ClassId { get; set; }
    public CourseClass Class { get; set; } = null!;

    [Required] public int ExerciseId { get; set; }
    public ExerciseChallenge Exercise { get; set; } = null!;

    public bool Enabled { get; set; } = true;
    public DateTimeOffset? VisibleFromUtc { get; set; }
    public DateTimeOffset? DueUtc { get; set; }

    public int Order { get; set; } = 0;
}
