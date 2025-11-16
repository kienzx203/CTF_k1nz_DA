using GZCTF.Models;
using GZCTF.Models.Data;
using GZCTF.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GZCTF.Repositories;

public class LabReportRepository(AppDbContext db) : ILabReportRepository
{
    public Task<LabReport?> GetByStudentAsync(int sessionId, Guid studentId, CancellationToken ct = default) =>
        db.Set<LabReport>()
          .AsNoTracking()
          .Include(r => r.Attachment)
          .Include(r => r.GradedBy)
          .FirstOrDefaultAsync(r => r.SessionId == sessionId && r.StudentId == studentId, ct);

    public async Task<LabReport> SubmitAsync(int sessionId, Guid studentId, int? attachmentId, string? note, CancellationToken ct = default)
    {
        // Unique (StudentId, SessionId)
        var existing = await db.Set<LabReport>()
            .FirstOrDefaultAsync(r => r.SessionId == sessionId && r.StudentId == studentId, ct);

        if (existing is null)
        {
            var entity = new LabReport
            {
                SessionId = sessionId,
                StudentId = studentId,
                AttachmentId = attachmentId,
                Note = note,
                SubmittedUtc = DateTimeOffset.UtcNow
            };
            await db.Set<LabReport>().AddAsync(entity, ct);
            await db.SaveChangesAsync(ct);
            return entity;
        }
        else
        {
            existing.AttachmentId = attachmentId;
            existing.Note = note;
            existing.SubmittedUtc = DateTimeOffset.UtcNow;
            db.Update(existing);
            await db.SaveChangesAsync(ct);
            return existing;
        }
    }

    public Task<List<LabReport>> ListBySessionAsync(int sessionId, int skip = 0, int take = 50, CancellationToken ct = default) =>
        db.Set<LabReport>()
          .AsNoTracking()
          .Where(r => r.SessionId == sessionId)
          .Include(r => r.Student)
          .OrderByDescending(r => r.SubmittedUtc)
          .Skip(skip).Take(take)
          .ToListAsync(ct);

    public async Task GradeAsync(int reportId, Guid graderId, int score, string? feedback, CancellationToken ct = default)
    {
        var report = await db.Set<LabReport>().FirstOrDefaultAsync(r => r.Id == reportId, ct)
                     ?? throw new KeyNotFoundException("Report not found.");

        report.Score = score;
        report.Feedback = feedback;
        report.GradedById = graderId;
        report.GradedUtc = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
    }
}
