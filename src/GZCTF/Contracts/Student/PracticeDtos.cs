namespace GZCTF.Contracts.Student;

public record PracticeCardDto(
    int ExerciseId, string Title, string? Category,
    bool Enabled, bool IsVisibleNow, DateTimeOffset? VisibleFromUtc, DateTimeOffset? DueUtc,
    int Order
);

public record PracticeDetailDto(
    int ExerciseId, string Title, string? Category,
    string? Description, // nếu muốn show mô tả ngắn, bạn có thể lấy từ ExerciseChallenge
    bool Enabled, bool IsVisibleNow, DateTimeOffset? VisibleFromUtc, DateTimeOffset? DueUtc
);
