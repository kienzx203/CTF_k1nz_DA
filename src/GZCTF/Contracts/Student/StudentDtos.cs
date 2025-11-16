using System.ComponentModel.DataAnnotations;

namespace GZCTF.Contracts.Student;

// --- Output: lớp của tôi ---
public record MyClassDto(
    int Id, string Code, string Name,
    Guid TeacherId, string TeacherName,
    DateTimeOffset CreatedUtc,
    int MemberCount, int SessionCount
);

// --- Output: danh sách buổi của lớp ---
public record StudentSessionRowDto(
    int Id, string Title, string? Description,
    DateTimeOffset? StartUtc, DateTimeOffset? EndUtc,
    bool IsOpen,            // còn trong thời gian (hoặc không set time)
    bool Submitted,         // SV đã nộp báo cáo?
    int ReportCount         // tổng số báo cáo của buổi (để biết độ sôi động)
);

// --- Output: chi tiết 1 buổi ---
public record StudentSessionDetailDto(
    int Id, int ClassId, string Title, string? Description,
    DateTimeOffset? StartUtc, DateTimeOffset? EndUtc,
    bool IsOpen,
    List<StudentSessionChallengeDto> Challenges
);

public record StudentSessionChallengeDto(
    int ExerciseId, string Title, string? Category, int Weight, int Order, bool Required
);

// --- Report ---
public record MyReportDto(
    int Id, int SessionId, Guid StudentId,
    int? AttachmentId, string? Note,
    DateTimeOffset SubmittedUtc,
    int? Score, string? Feedback, string? GradedByName, DateTimeOffset? GradedUtc
);

// --- Submit report ---
public record SubmitReportDto(
    int? AttachmentId,       // dùng Attachment đã upload qua endpoint sẵn có của hệ thống
    string? Note
);
