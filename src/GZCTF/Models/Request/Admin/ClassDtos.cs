using System.ComponentModel.DataAnnotations;

namespace GZCTF.Models.Request.Admin;

public record ClassCreateDto(
    [Required, MaxLength(64)] string Code,
    [Required, MaxLength(128)] string Name,
    string? Description,
    [Required] Guid TeacherId
);

public record ClassUpdateDto(
    [Required, MaxLength(128)] string Name,
    string? Description
);

public record AddStudentsDto(
    [Required] Guid[] StudentIds
);

// Dùng cho list
public record ClassSummaryDto(
    int Id,
    string Code,
    string Name,
    Guid TeacherId,
    string TeacherName,
    int MemberCount,
    int SessionCount,
    DateTimeOffset CreatedUtc
);

// Dùng cho chi tiết
public record ClassMemberDto(Guid StudentId, string? UserName, string? Email);
public record ClassSessionDto(int Id, string Title, DateTimeOffset? StartUtc, DateTimeOffset? EndUtc, int ChallengeCount, int ReportCount);

public record ClassDetailDto(
    int Id,
    string Code,
    string Name,
    string? Description,
    Guid TeacherId,
    string TeacherName,
    DateTimeOffset CreatedUtc,
    List<ClassMemberDto> Members,
    List<ClassSessionDto> Sessions
);
