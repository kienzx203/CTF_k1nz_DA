using System.ComponentModel.DataAnnotations;

namespace GZCTF.Contracts.Teacher;

public record AttachPracticeItemDto(
    [Required] int ExerciseId,
    bool Enabled,
    DateTimeOffset? VisibleFromUtc,
    DateTimeOffset? DueUtc,
    int Order
);
public record AttachPracticeDto([Required] List<AttachPracticeItemDto> Items);

public record PracticeExerciseRowDto(
    int ExerciseId,
    string Title,
    string? Category,
    bool Enabled,
    DateTimeOffset? VisibleFromUtc,
    DateTimeOffset? DueUtc,
    int Order
);

public record PracticeExerciseUpdateDto(
    bool Enabled,
    DateTimeOffset? VisibleFromUtc,
    DateTimeOffset? DueUtc,
    int Order
);
