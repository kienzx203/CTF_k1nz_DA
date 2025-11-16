using GZCTF.Contracts.Student;
using GZCTF.Middlewares; // RequireUser
using GZCTF.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GZCTF.Controllers;

[ApiController]
[Route("api/student/classes/{classId:int}/practice")]
[RequireUser]
public class StudentPracticeController(
    UserManager<UserInfo> userManager,
    AppDbContext db
) : ControllerBase
{
    private async Task<UserInfo> MeAsync()
    {
        return await userManager.GetUserAsync(User) ?? throw new UnauthorizedAccessException();
    }

    private Task<bool> IsMemberAsync(int classId, Guid studentId, CancellationToken ct) =>
        db.ClassMembers.AsNoTracking().AnyAsync(m => m.ClassId == classId && m.StudentId == studentId, ct);

    private static bool IsVisibleNow(bool enabled, DateTimeOffset? from, DateTimeOffset? due, DateTimeOffset now)
    {
        if (!enabled)
            return false;
        if (from is not null && now < from.Value)
            return false;
        if (due is not null && now > due.Value)
            return false;
        return true;
    }

    /// <summary>Danh sách bài luyện tập của lớp (áp dụng thời gian hiển thị)</summary>
    [HttpGet("exercises")]
    public async Task<ActionResult<IEnumerable<PracticeCardDto>>> List([FromRoute] int classId, CancellationToken ct)
    {
        var me = await MeAsync();
        if (!await IsMemberAsync(classId, me.Id, ct))
            return Forbid();

        var rows = await db.ClassExercises
            .AsNoTracking()
            .Include(x => x.Exercise)
            .Where(x => x.ClassId == classId)
            .OrderBy(x => x.Order).ThenBy(x => x.ExerciseId)
            .ToListAsync(ct);

        var now = DateTimeOffset.UtcNow;
        var result = rows.Select(x => new PracticeCardDto(
            x.ExerciseId,
            x.Exercise.Title,
            x.Exercise.Category.ToString(),
            x.Enabled,
            IsVisibleNow(x.Enabled, x.VisibleFromUtc, x.DueUtc, now),
            x.VisibleFromUtc,
            x.DueUtc,
            x.Order
        ));

        return Ok(result);
    }

    /// <summary>Chi tiết 1 bài luyện tập</summary>
    [HttpGet("exercises/{exerciseId:int}")]
    public async Task<ActionResult<PracticeDetailDto>> Detail([FromRoute] int classId, [FromRoute] int exerciseId, CancellationToken ct)
    {
        var me = await MeAsync();
        if (!await IsMemberAsync(classId, me.Id, ct))
            return Forbid();

        var ce = await db.ClassExercises
            .AsNoTracking()
            .Include(x => x.Exercise)
            .FirstOrDefaultAsync(x => x.ClassId == classId && x.ExerciseId == exerciseId, ct);

        if (ce is null)
            return NotFound();

        var now = DateTimeOffset.UtcNow;
        var dto = new PracticeDetailDto(
            ce.ExerciseId,
            ce.Exercise.Title,
            ce.Exercise.Category.ToString(),
            ce.Exercise.Description, // nếu field này tồn tại; nếu không có thì bỏ
            ce.Enabled,
            IsVisibleNow(ce.Enabled, ce.VisibleFromUtc, ce.DueUtc, now),
            ce.VisibleFromUtc,
            ce.DueUtc
        );

        return Ok(dto);
    }
}
