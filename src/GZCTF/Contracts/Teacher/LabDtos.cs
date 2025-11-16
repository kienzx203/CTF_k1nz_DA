using System.ComponentModel.DataAnnotations;

namespace GZCTF.Contracts.Teacher;

public record LabSessionEditDto(
    [Required, MaxLength(128)] string Title,
    string? Description,
    DateTimeOffset? StartUtc,
    DateTimeOffset? EndUtc
);

public record SessionExerciseDto(
    [Required] int ExerciseId,
    int Weight,
    int Order,
    bool Required
);

public record AddExercisesDto([Required] List<SessionExerciseDto> Items);

public record GradeDto(
    [Range(0, 100)] int Score,
    string? Feedback
);

// Output DTOs
public record TeacherClassSummaryDto(
    int Id, string Code, string Name,
    DateTimeOffset CreatedUtc,
    int SessionCount, int MemberCount
);

public record SessionChallengeDto(
    int ExerciseId, string Title, string? Category, int Weight, int Order, bool Required
);

public record SessionDetailDto(
    int Id, int ClassId, string Title, string? Description,
    DateTimeOffset? StartUtc, DateTimeOffset? EndUtc,
    List<SessionChallengeDto> Challenges,
    int ReportCount
);

public record ReportSummaryDto(
    int Id, Guid StudentId, string? StudentName,
    DateTimeOffset SubmittedUtc,
    int? Score, string? Feedback, string? GradedBy
);
