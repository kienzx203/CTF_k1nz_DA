using GZCTF.Contracts.Teacher;
using GZCTF.Middlewares; // RequireTeacher
using GZCTF.Models;
using GZCTF.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GZCTF.Controllers;

[ApiController]
[Route("api/teacher/classes/{classId:int}/practice")]
[RequireTeacher]
public class TeacherPracticeController(
    IClassExerciseRepository repo,
    UserManager<UserInfo> userManager,
    AppDbContext db
) : ControllerBase
{
    private async Task<UserInfo> MeAsync()
    {
        var me = await userManager.GetUserAsync(User) ?? throw new UnauthorizedAccessException();
        return me;
    }

    private Task<bool> OwnsClassAsync(int classId, Guid teacherId, CancellationToken ct) =>
        db.Classes.AsNoTracking().AnyAsync(c => c.Id == classId && c.TeacherId == teacherId, ct);

    /// <summary>Danh sách bài practice đã gán vào lớp</summary>
    [HttpGet("exercises")]
    public async Task<ActionResult<IEnumerable<PracticeExerciseRowDto>>> List([FromRoute] int classId, CancellationToken ct)
    {
        var me = await MeAsync();
        if (!await OwnsClassAsync(classId, me.Id, ct))
            return Forbid();

        var rows = await repo.ListByClassAsync(classId, ct);
        var result = rows.Select(x => new PracticeExerciseRowDto(
            x.ExerciseId,
            x.Exercise.Title,
            x.Exercise.Category.ToString(), // enum -> string
            x.Enabled,
            x.VisibleFromUtc,
            x.DueUtc,
            x.Order
        ));

        return Ok(result);
    }

    /// <summary>Gán nhiều bài vào lớp</summary>
    [HttpPost("exercises")]
    public async Task<IActionResult> Attach([FromRoute] int classId, [FromBody] AttachPracticeDto dto, CancellationToken ct)
    {
        var me = await MeAsync();
        if (!await OwnsClassAsync(classId, me.Id, ct))
            return Forbid();
        if (dto.Items is null || dto.Items.Count == 0)
            return BadRequest("Items rỗng");

        // Lọc exercise tồn tại (tuỳ chọn: nếu có OwnerId cho ExerciseChallenge, lọc OwnerId == me.Id)
        var ids = dto.Items.Select(i => i.ExerciseId).Distinct().ToArray();
        var existed = await db.ExerciseChallenges.AsNoTracking()
            .Where(e => ids.Contains(e.Id))
            .Select(e => e.Id).ToListAsync(ct);

        var items = dto.Items
            .Where(i => existed.Contains(i.ExerciseId))
            .Select(i => (i.ExerciseId, i.Enabled, i.VisibleFromUtc, i.DueUtc, i.Order));

        await repo.AddAsync(classId, items, ct);
        return NoContent();
    }

    /// <summary>Cập nhật 1 assignment</summary>
    [HttpPut("exercises/{exerciseId:int}")]
    public async Task<IActionResult> Update([FromRoute] int classId, [FromRoute] int exerciseId, [FromBody] PracticeExerciseUpdateDto dto, CancellationToken ct)
    {
        var me = await MeAsync();
        if (!await OwnsClassAsync(classId, me.Id, ct))
            return Forbid();

        var ce = await repo.GetAsync(classId, exerciseId, ct);
        if (ce is null)
            return NotFound();

        ce.Enabled = dto.Enabled;
        ce.VisibleFromUtc = dto.VisibleFromUtc;
        ce.DueUtc = dto.DueUtc;
        ce.Order = dto.Order;

        await repo.UpdateAsync(ce, ct);
        return NoContent();
    }

    /// <summary>Huỷ gán 1 bài khỏi lớp</summary>
    [HttpDelete("exercises/{exerciseId:int}")]
    public async Task<IActionResult> Detach([FromRoute] int classId, [FromRoute] int exerciseId, CancellationToken ct)
    {
        var me = await MeAsync();
        if (!await OwnsClassAsync(classId, me.Id, ct))
            return Forbid();

        await repo.RemoveAsync(classId, exerciseId, ct);
        return NoContent();
    }
}
