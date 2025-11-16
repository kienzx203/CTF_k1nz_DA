using GZCTF.Contracts.Student;
using GZCTF.Middlewares; // RequireUser
using GZCTF.Models;
using GZCTF.Models.Data;
using GZCTF.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GZCTF.Controllers;

[ApiController]
[Route("api/student")]
[RequireUser] // chỉ user đã đăng nhập. Quyền membership kiểm riêng từng action.
public class StudentController(
    UserManager<UserInfo> userManager,
    AppDbContext db,
    ILabReportRepository reportRepo,
    ILabSessionRepository sessionRepo
) : ControllerBase
{
    private async Task<UserInfo> MeAsync()
    {
        var me = await userManager.GetUserAsync(User);
        if (me is null)
            throw new UnauthorizedAccessException("Not authenticated");
        return me;
    }

    private Task<bool> IsMemberAsync(int classId, Guid studentId, CancellationToken ct) =>
        db.ClassMembers.AsNoTracking().AnyAsync(m => m.ClassId == classId && m.StudentId == studentId, ct);

    private async Task<(LabSession? s, CourseClass? c)> LoadSessionAndClassAsync(int sessionId, CancellationToken ct)
    {
        var s = await db.LabSessions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == sessionId, ct);
        if (s is null)
            return (null, null);
        var c = await db.Classes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == s.ClassId, ct);
        return (s, c);
    }

    private static bool IsOpen(LabSession s, DateTimeOffset nowUtc)
    {
        if (s.StartUtc is not null && nowUtc < s.StartUtc.Value)
            return false;
        if (s.EndUtc is not null && nowUtc > s.EndUtc.Value)
            return false;
        return true;
    }

    // --------- Lớp của tôi ---------
    [HttpGet("classes")]
    public async Task<ActionResult<IEnumerable<MyClassDto>>> MyClasses([FromQuery] string? q, CancellationToken ct)
    {
        var me = await MeAsync();

        // Lấy membership trước
        var classIds = await db.ClassMembers.AsNoTracking()
            .Where(m => m.StudentId == me.Id)
            .Select(m => m.ClassId)
            .ToListAsync(ct);

        if (classIds.Count == 0)
            return Ok(Array.Empty<MyClassDto>());

        var baseQuery = db.Classes
            .Include(c => c.Teacher)
            .Where(c => classIds.Contains(c.Id));

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            baseQuery = baseQuery.Where(c =>
                EF.Functions.ILike(c.Code, $"%{term}%") || EF.Functions.ILike(c.Name, $"%{term}%"));
        }

        var list = await baseQuery.AsNoTracking()
            .OrderByDescending(c => c.CreatedUtc)
            .ToListAsync(ct);

        var ids = list.Select(c => c.Id).ToArray();

        var memberCounts = await db.ClassMembers.AsNoTracking()
            .Where(m => ids.Contains(m.ClassId))
            .GroupBy(m => m.ClassId).Select(g => new { g.Key, C = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.C, ct);

        var sessionCounts = await db.LabSessions.AsNoTracking()
            .Where(s => ids.Contains(s.ClassId))
            .GroupBy(s => s.ClassId).Select(g => new { g.Key, C = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.C, ct);

        var result = list.Select(c => new MyClassDto(
            c.Id, c.Code, c.Name,
            c.TeacherId, c.Teacher?.UserName ?? "Unknown",
            c.CreatedUtc,
            memberCounts.GetValueOrDefault(c.Id, 0),
            sessionCounts.GetValueOrDefault(c.Id, 0)
        ));

        return Ok(result);
    }

    // --------- Danh sách buổi của 1 lớp ---------
    [HttpGet("classes/{classId:int}/sessions")]
    public async Task<ActionResult<IEnumerable<StudentSessionRowDto>>> SessionsOfClass([FromRoute] int classId, CancellationToken ct)
    {
        var me = await MeAsync();
        if (!await IsMemberAsync(classId, me.Id, ct))
            return Forbid();

        var sessions = await sessionRepo.ListByClassAsync(classId, ct);
        var sessionIds = sessions.Select(s => s.Id).ToArray();

        // Các report của chính SV
        var myReports = await db.LabReports.AsNoTracking()
            .Where(r => r.StudentId == me.Id && sessionIds.Contains(r.SessionId))
            .Select(r => r.SessionId)
            .ToListAsync(ct);
        var myReportSet = new HashSet<int>(myReports);

        // Tổng report của buổi
        var reportCounts = await db.LabReports.AsNoTracking()
            .Where(r => sessionIds.Contains(r.SessionId))
            .GroupBy(r => r.SessionId).Select(g => new { g.Key, C = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.C, ct);

        var now = DateTimeOffset.UtcNow;

        var result = sessions.Select(s => new StudentSessionRowDto(
            s.Id, s.Title, s.Description,
            s.StartUtc, s.EndUtc,
            IsOpen(s, now),
            myReportSet.Contains(s.Id),
            reportCounts.GetValueOrDefault(s.Id, 0)
        ));

        return Ok(result);
    }

    // --------- Chi tiết 1 buổi (kèm danh sách chall) ---------
    [HttpGet("sessions/{sessionId:int}")]
    public async Task<ActionResult<StudentSessionDetailDto>> SessionDetail([FromRoute] int sessionId, CancellationToken ct)
    {
        var me = await MeAsync();

        var (s, c) = await LoadSessionAndClassAsync(sessionId, ct);
        if (s is null || c is null)
            return NotFound();
        if (!await IsMemberAsync(c.Id, me.Id, ct))
            return Forbid();

        var full = await sessionRepo.GetAsync(sessionId, includeChallenges: true, includeReports: false, ct);
        if (full is null)
            return NotFound();

        var now = DateTimeOffset.UtcNow;
        var dto = new StudentSessionDetailDto(
            full.Id, full.ClassId, full.Title, full.Description,
            full.StartUtc, full.EndUtc,
            IsOpen(full, now),
            Challenges: new()
        );

        if (full.Challenges is not null)
        {
            // Sắp xếp theo Order
            foreach (var sc in full.Challenges.OrderBy(x => x.Order))
            {
                var ex = sc.Exercise; // ExerciseChallenge
                dto.Challenges.Add(new StudentSessionChallengeDto(
                    sc.ExerciseId,
                    ex.Title,
                    ex.Category.ToString(), // Nếu muốn enum → đổi DTO sang ChallengeCategory
                    sc.Weight, sc.Order, sc.Required
                ));
            }
        }

        return Ok(dto);
    }

    // --------- Lấy báo cáo của chính mình ---------
    [HttpGet("sessions/{sessionId:int}/report")]
    public async Task<ActionResult<MyReportDto>> MyReport([FromRoute] int sessionId, CancellationToken ct)
    {
        var me = await MeAsync();
        var (s, c) = await LoadSessionAndClassAsync(sessionId, ct);
        if (s is null || c is null)
            return NotFound();
        if (!await IsMemberAsync(c.Id, me.Id, ct))
            return Forbid();

        var r = await reportRepo.GetByStudentAsync(sessionId, me.Id, ct);
        if (r is null)
            return Ok(null);

        var dto = new MyReportDto(
            r.Id, r.SessionId, r.StudentId,
            r.AttachmentId, r.Note,
            r.SubmittedUtc,
            r.Score, r.Feedback, r.GradedBy?.UserName, r.GradedUtc
        );
        return Ok(dto);
    }

    // --------- Nộp/ghi đè báo cáo ---------
    [HttpPost("sessions/{sessionId:int}/report")]
    public async Task<IActionResult> Submit([FromRoute] int sessionId, [FromBody] SubmitReportDto dto, CancellationToken ct)
    {
        var me = await MeAsync();
        var (s, c) = await LoadSessionAndClassAsync(sessionId, ct);
        if (s is null || c is null)
            return NotFound();
        if (!await IsMemberAsync(c.Id, me.Id, ct))
            return Forbid();

        // Chặn nộp ngoài giờ (tuỳ policy)
        var now = DateTimeOffset.UtcNow;
        if (s.EndUtc is not null && now > s.EndUtc.Value)
            return Problem(statusCode: StatusCodes.Status403Forbidden, detail: "Buổi thực hành đã kết thúc.");

        // Cho phép ghi đè: repository đã làm idempotent
        await reportRepo.SubmitAsync(sessionId, me.Id, dto.AttachmentId, dto.Note, ct);
        return NoContent();
    }
}
