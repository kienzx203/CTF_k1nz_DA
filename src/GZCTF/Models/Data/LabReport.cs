using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GZCTF.Models.Data;

/// <summary>
/// Student report submission for a lab session.
/// </summary>
[Index(nameof(StudentId), nameof(SessionId), IsUnique = true)]
public class LabReport
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int SessionId { get; set; }
    public LabSession Session { get; set; } = null!;

    [Required]
    public Guid StudentId { get; set; }
    public UserInfo Student { get; set; } = null!;

    /// <summary>
    /// Optional file attachment (reuse Attachment entity)
    /// </summary>
    public int? AttachmentId { get; set; }
    public Attachment? Attachment { get; set; }

    /// <summary>
    /// Optional student note
    /// </summary>
    public string? Note { get; set; }

    public DateTimeOffset SubmittedUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Grading (by teacher)
    /// </summary>
    public int? Score { get; set; }
    public string? Feedback { get; set; }
    public Guid? GradedById { get; set; }
    public UserInfo? GradedBy { get; set; }
    public DateTimeOffset? GradedUtc { get; set; }
}
