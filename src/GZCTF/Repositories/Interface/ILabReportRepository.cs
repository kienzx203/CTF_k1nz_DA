using GZCTF.Models.Data;

namespace GZCTF.Repositories.Interfaces;

public interface ILabReportRepository
{
    Task<LabReport?> GetByStudentAsync(int sessionId, Guid studentId, CancellationToken ct = default);

    Task<LabReport> SubmitAsync(int sessionId, Guid studentId, int? attachmentId, string? note, CancellationToken ct = default);

    Task<List<LabReport>> ListBySessionAsync(int sessionId, int skip = 0, int take = 50, CancellationToken ct = default);

    Task GradeAsync(int reportId, Guid graderId, int score, string? feedback, CancellationToken ct = default);
}
