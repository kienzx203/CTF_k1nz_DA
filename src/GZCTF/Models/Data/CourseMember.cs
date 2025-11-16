using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GZCTF.Models.Data;

/// <summary>
/// Join table: student membership of a class.
/// </summary>
[PrimaryKey(nameof(ClassId), nameof(StudentId))]
public class CourseMember
{
    [Required]
    public int ClassId { get; set; }
    public CourseClass Class { get; set; } = null!;

    [Required]
    public Guid StudentId { get; set; }
    public UserInfo Student { get; set; } = null!;

    public DateTimeOffset JoinedUtc { get; set; } = DateTimeOffset.UtcNow;
}
