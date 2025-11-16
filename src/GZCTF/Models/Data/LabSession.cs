using System.ComponentModel.DataAnnotations;

namespace GZCTF.Models.Data;

/// <summary>
/// A lab session under a course class.
/// </summary>
public class LabSession
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ClassId { get; set; }
    public CourseClass Class { get; set; } = null!;

    [Required, MaxLength(128)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1024)]
    public string? Description { get; set; }

    public DateTimeOffset? StartUtc { get; set; }
    public DateTimeOffset? EndUtc { get; set; }

    public List<LabSessionChallenge> Challenges { get; set; } = [];
    public List<LabReport> Reports { get; set; } = [];
}
