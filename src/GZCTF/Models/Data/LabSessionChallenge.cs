using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GZCTF.Models.Data;

/// <summary>
/// Mapping: which exercise challenges are included in a session.
/// </summary>
[PrimaryKey(nameof(SessionId), nameof(ExerciseId))]
public class LabSessionChallenge
{
    [Required]
    public int SessionId { get; set; }
    public LabSession Session { get; set; } = null!;

    [Required]
    public int ExerciseId { get; set; }
    public ExerciseChallenge Exercise { get; set; } = null!;

    /// <summary>
    /// Weight for score aggregation
    /// </summary>
    public int Weight { get; set; } = 1;

    /// <summary>
    /// Display order
    /// </summary>
    public int Order { get; set; } = 0;

    /// <summary>
    /// Whether required to pass
    /// </summary>
    public bool Required { get; set; } = false;
}
