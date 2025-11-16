using System.ComponentModel.DataAnnotations;

namespace GZCTF.Models.Data;

/// <summary>
/// A course class (subject / group) owned by a teacher.
/// </summary>
public class CourseClass
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Class code (unique), e.g. "CTF101-24.1"
    /// </summary>
    [Required, MaxLength(64)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Class name
    /// </summary>
    [Required, MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description
    /// </summary>
    [MaxLength(512)]
    public string? Description { get; set; }

    /// <summary>
    /// Teacher (owner) of the class
    /// </summary>
    [Required]
    public Guid TeacherId { get; set; }
    public UserInfo Teacher { get; set; } = null!;

    /// <summary>
    /// Created at (UTC)
    /// </summary>
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;

    public List<CourseMember> Members { get; set; } = [];
    public List<LabSession> Sessions { get; set; } = [];
    public List<ClassExercise> PracticeExercises { get; set; } = new();
}
