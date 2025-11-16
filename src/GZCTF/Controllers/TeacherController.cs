using System.Security.Claims;
using GZCTF.Contracts.Teacher;
using GZCTF.Middlewares;                // RequireTeacher
using GZCTF.Models;
using GZCTF.Models.Data;
using GZCTF.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GZCTF.Controllers;

[ApiController]
[Route("api/teacher")]
[RequireTeacher] // Admin cũng qua được nhờ rank mapping
public class TeacherController(
    IClassRepository classRepo,
    ILabSessionRepository labRepo,
    ILabReportRepository reportRepo,
    UserManager<UserInfo> userManager,
    AppDbContext db
) : ControllerBase
{
    private async Task<UserInfo> CurrentUserAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
            throw new UnauthorizedAccessException("Not authenticated");
        return user;
    }

    private Task<bool> OwnsClassAsync(int classId, Guid teacherId, CancellationToken ct) =>
        db.Classes.AsNoTracking().AnyAsync(c => c.Id == classId && c.TeacherId == teacherId, ct);

    private async Task<bool> OwnsSessionAsync(int sessionId, Guid teacherId, CancellationToken ct)
    {
        return await db.LabSessions
            .AsNoTracking()
            .AnyAsync(s => s.Id == sessionId && db.Classes.Any(c => c.Id == s.ClassId && c.TeacherId == teacherId), ct);
    }

    // -------- Classes --------

    /// <summary>Lớp tôi phụ trách (summary)</summary>
    [HttpGet("classes")]
    public async Task<ActionResult<IEnumerable<TeacherClassSummaryDto>>> MyClasses(
        [FromQuery] string? q,
        CancellationToken ct)
    {
        var me = await CurrentUserAsync();

        var list = await db.Classes
            .AsNoTracking()
            .Where(c => c.TeacherId == me.Id)
            .Where(c => string.IsNullOrWhiteSpace(q) ||
                        EF.Functions.ILike(c.Code, $"%{q.Trim()}%") ||
                        EF.Functions.ILike(c.Name, $"%{q.Trim()}%"))
            .OrderByDescending(c => c.CreatedUtc)
            .ToListAsync(ct);

        var ids = list.Select(x => x.Id).ToArray();

        var sessionCounts = await db.LabSessions.AsNoTracking()
            .Where(s => ids.Contains(s.ClassId))
            .GroupBy(s => s.ClassId).Select(g => new { g.Key, C = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.C, ct);

        var memberCounts = await db.ClassMembers.AsNoTracking()
            .Where(m => ids.Contains(m.ClassId))
            .GroupBy(m => m.ClassId).Select(g => new { g.Key, C = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.C, ct);

        var result = list.Select(c => new TeacherClassSummaryDto(
            c.Id, c.Code, c.Name, c.CreatedUtc,
            sessionCounts.GetValueOrDefault(c.Id, 0),
            memberCounts.GetValueOrDefault(c.Id, 0)
        ));

        return Ok(result);
    }

    // -------- Sessions (by class) --------

    /// <summary>Danh sách buổi thực hành của 1 lớp</summary>
    [HttpGet("classes/{classId:int}/sessions")]
    public async Task<ActionResult<IEnumerable<SessionDetailDto>>> ListSessions([FromRoute] int classId, CancellationToken ct)
    {
        var me = await CurrentUserAsync();
        if (!await OwnsClassAsync(classId, me.Id, ct))
            return Forbid();

        var sessions = await labRepo.ListByClassAsync(classId, ct);

        // Lấy report count theo lô
        var sessionIds = sessions.Select(s => s.Id).ToArray();
        var reportCounts = await db.LabReports.AsNoTracking()
            .Where(r => sessionIds.Contains(r.SessionId))
            .GroupBy(r => r.SessionId).Select(g => new { g.Key, C = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.C, ct);

        var result = new List<SessionDetailDto>(sessions.Count);
        foreach (var s in sessions)
        {
            result.Add(new SessionDetailDto(
                s.Id, s.ClassId, s.Title, s.Description,
                s.StartUtc, s.EndUtc,
                Challenges: new(),                      // danh sách rỗng (chỉ summary)
                ReportCount: reportCounts.GetValueOrDefault(s.Id, 0)
            ));
        }

        return Ok(result);
    }

    /// <summary>Tạo buổi thực hành</summary>
    [HttpPost("classes/{classId:int}/sessions")]
    public async Task<ActionResult<SessionDetailDto>> CreateSession([FromRoute] int classId, [FromBody] LabSessionEditDto dto, CancellationToken ct)
    {
        var me = await CurrentUserAsync();
        if (!await OwnsClassAsync(classId, me.Id, ct))
            return Forbid();

        var entity = new LabSession
        {
            ClassId = classId,
            Title = dto.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            StartUtc = dto.StartUtc,
            EndUtc = dto.EndUtc
        };

        entity = await labRepo.CreateAsync(entity, ct);

        var result = new SessionDetailDto(
            entity.Id, entity.ClassId, entity.Title, entity.Description,
            entity.StartUtc, entity.EndUtc,
            new(), 0
        );

        return CreatedAtAction(nameof(GetSession), new { sessionId = entity.Id }, result);
    }

    /// <summary>Chi tiết buổi (kèm challenges + reportCount)</summary>
    [HttpGet("sessions/{sessionId:int}")]
    public async Task<ActionResult<SessionDetailDto>> GetSession([FromRoute] int sessionId, CancellationToken ct)
    {
        var me = await CurrentUserAsync();
        if (!await OwnsSessionAsync(sessionId, me.Id, ct))
            return Forbid();

        var s = await labRepo.GetAsync(sessionId, includeChallenges: true, includeReports: false, ct);
        if (s is null)
            return NotFound();

        var reportCount = await db.LabReports.AsNoTracking().CountAsync(r => r.SessionId == sessionId, ct);

        // Map challenges
        var challengeDtos = new List<SessionChallengeDto>();
        if (s.Challenges is not null)
        {
            foreach (var sc in s.Challenges.OrderBy(x => x.Order))
            {
                // Lấy thông tin exercise tối thiểu
                var ex = sc.Exercise;
                challengeDtos.Add(new SessionChallengeDto(
                    sc.ExerciseId,
                    ex.Title, ex.Category.ToString(),
                    sc.Weight, sc.Order, sc.Required
                ));
            }
        }

        var dto = new SessionDetailDto(
            s.Id, s.ClassId, s.Title, s.Description,
            s.StartUtc, s.EndUtc,
            challengeDtos, reportCount
        );

        return Ok(dto);
    }

    /// <summary>Cập nhật buổi</summary>
    [HttpPut("sessions/{sessionId:int}")]
    public async Task<ActionResult> UpdateSession([FromRoute] int sessionId, [FromBody] LabSessionEditDto dto, CancellationToken ct)
    {
        var me = await CurrentUserAsync();
        if (!await OwnsSessionAsync(sessionId, me.Id, ct))
            return Forbid();

        var s = await db.LabSessions.FirstOrDefaultAsync(x => x.Id == sessionId, ct);
        if (s is null)
            return NotFound();

        s.Title = dto.Title.Trim();
        s.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
        s.StartUtc = dto.StartUtc;
        s.EndUtc = dto.EndUtc;

        await labRepo.UpdateAsync(s, ct);
        return NoContent();
    }

    /// <summary>Xoá buổi</summary>
    [HttpDelete("sessions/{sessionId:int}")]
    public async Task<ActionResult> DeleteSession([FromRoute] int sessionId, CancellationToken ct)
    {
        var me = await CurrentUserAsync();
        if (!await OwnsSessionAsync(sessionId, me.Id, ct))
            return Forbid();

        await labRepo.DeleteAsync(sessionId, ct);
        return NoContent();
    }

    // -------- Manage Exercises in session --------

    /// <summary>Gắn nhiều challenge vào buổi</summary>
    [HttpPost("sessions/{sessionId:int}/exercises")]
    public async Task<ActionResult> AddExercises([FromRoute] int sessionId, [FromBody] AddExercisesDto dto, CancellationToken ct)
    {
        var me = await CurrentUserAsync();
        if (!await OwnsSessionAsync(sessionId, me.Id, ct))
            return Forbid();

        if (dto.Items is null || dto.Items.Count == 0)
            return BadRequest(new { message = "Danh sách Items rỗng" });

        // (Tuỳ chọn) Lọc chỉ giữ exercise tồn tại
        var exIds = dto.Items.Select(i => i.ExerciseId).Distinct().ToArray();
        var existed = await db.ExerciseChallenges.AsNoTracking()
            .Where(e => exIds.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync(ct);

        var tuples = dto.Items
            .Where(i => existed.Contains(i.ExerciseId))
            .Select(i => (i.ExerciseId, i.Weight, i.Order, i.Required));

        await labRepo.AddExercisesAsync(sessionId, tuples, ct);
        return NoContent();
    }

    /// <summary>Gỡ challenge khỏi buổi</summary>
    [HttpDelete("sessions/{sessionId:int}/exercises/{exerciseId:int}")]
    public async Task<ActionResult> RemoveExercise([FromRoute] int sessionId, [FromRoute] int exerciseId, CancellationToken ct)
    {
        var me = await CurrentUserAsync();
        if (!await OwnsSessionAsync(sessionId, me.Id, ct))
            return Forbid();

        await labRepo.RemoveExerciseAsync(sessionId, exerciseId, ct);
        return NoContent();
    }

    // -------- Reports & Grading --------

    /// <summary>Danh sách báo cáo của buổi</summary>
    [HttpGet("sessions/{sessionId:int}/reports")]
    public async Task<ActionResult<IEnumerable<ReportSummaryDto>>> ListReports([FromRoute] int sessionId, CancellationToken ct)
    {
        var me = await CurrentUserAsync();
        if (!await OwnsSessionAsync(sessionId, me.Id, ct))
            return Forbid();

        var list = await reportRepo.ListBySessionAsync(sessionId, 0, 200, ct);
        var result = list.Select(r => new ReportSummaryDto(
            r.Id, r.StudentId, r.Student?.UserName,
            r.SubmittedUtc, r.Score, r.Feedback, r.GradedBy?.UserName
        ));

        return Ok(result);
    }

    /// <summary>Chấm điểm 1 báo cáo</summary>
    [HttpPost("reports/{reportId:int}/grade")]
    public async Task<ActionResult> Grade([FromRoute] int reportId, [FromBody] GradeDto dto, CancellationToken ct)
    {
        var me = await CurrentUserAsync();

        // Check quyền sở hữu qua session của report
        var r = await db.LabReports
            .Include(x => x.Session)
            .FirstOrDefaultAsync(x => x.Id == reportId, ct);

        if (r is null)
            return NotFound();
        if (!await OwnsSessionAsync(r.SessionId, me.Id, ct))
            return Forbid();

        await reportRepo.GradeAsync(reportId, me.Id, dto.Score, dto.Feedback, ct);
        return NoContent();
    }
}
