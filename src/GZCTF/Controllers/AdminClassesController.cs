using GZCTF.Middlewares;
using GZCTF.Models;
using GZCTF.Models.Data;
using GZCTF.Models.Request.Admin;
using GZCTF.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GZCTF.Controllers;

[ApiController]
[Route("api/admin/classes")]
public class AdminClassesController(
    IClassRepository classes,
    ILabSessionRepository sessions,
    UserManager<UserInfo> userManager,
    AppDbContext db
) : ControllerBase
{
    // --------- Helpers ---------
    private static ClassSummaryDto ToSummary(CourseClass c) =>
        new(
            c.Id,
            c.Code,
            c.Name,
            c.TeacherId,
            c.Teacher?.UserName ?? "Unknown",
            c.Members?.Count ?? 0,
            c.Sessions?.Count ?? 0,
            c.CreatedUtc
        );

    private static ClassDetailDto ToDetail(CourseClass c) =>
        new(
            c.Id,
            c.Code,
            c.Name,
            c.Description,
            c.TeacherId,
            c.Teacher?.UserName ?? "Unknown",
            c.CreatedUtc,
            (c.Members ?? new()).Select(m => new ClassMemberDto(
                m.StudentId,
                m.Student?.UserName,
                m.Student?.Email
            )).ToList(),
            (c.Sessions ?? new()).Select(s => new ClassSessionDto(
                s.Id,
                s.Title,
                s.StartUtc, s.EndUtc,
                s.Challenges?.Count ?? 0,
                s.Reports?.Count ?? 0
            )).ToList()
        );

    private async Task<bool> TeacherExistsAndValidAsync(Guid teacherId, CancellationToken ct)
    {
        var u = await userManager.FindByIdAsync(teacherId.ToString());
        if (u is null)
            return false;
        // Cho phép Admin làm teacher của lớp (linh hoạt)
        return u.Role is Role.Teacher or Role.Admin;
    }

    // --------- Endpoints ---------

    /// <summary> Tạo lớp học mới </summary>
    [HttpPost]
    [RequireAdmin]
    public async Task<ActionResult<ClassDetailDto>> Create([FromBody] ClassCreateDto dto, CancellationToken ct)
    {
        // Chuẩn hoá code
        var code = dto.Code.Trim();

        if (await classes.ExistsCodeAsync(code, ct))
            return Conflict($"Class code '{code}' already exists.");

        if (!await TeacherExistsAndValidAsync(dto.TeacherId, ct))
            return BadRequest("TeacherId không hợp lệ hoặc user không có role Teacher/Admin.");

        var entity = new CourseClass
        {
            Code = code,
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            TeacherId = dto.TeacherId
        };

        entity = await classes.CreateAsync(entity, ct);

        // Nạp đầy đủ điều hướng để trả chi tiết
        var full = await db.Set<CourseClass>()
            .Include(c => c.Teacher)
            .Include(c => c.Members).ThenInclude(m => m.Student)
            .Include(c => c.Sessions)
            .FirstAsync(c => c.Id == entity.Id, ct);

        return Ok(ToDetail(full));
    }

    /// <summary> Danh sách lớp theo teacher + search + phân trang </summary>
    [HttpGet]
    [RequireAdmin]
    public async Task<ActionResult<List<ClassSummaryDto>>> List(
        [FromQuery] Guid? teacherId,
        [FromQuery] string? q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        IQueryable<CourseClass> query = db.Set<CourseClass>()
            .Include(c => c.Teacher)
            .Include(c => c.Members)
            .Include(c => c.Sessions)
            .AsNoTracking();

        if (teacherId is { } tid)
            query = query.Where(c => c.TeacherId == tid);

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            // Npgsql: ILike; nếu DB khác dùng ToLowerContains
            query = query.Where(c =>
                EF.Functions.ILike(c.Code, $"%{term}%")
                || EF.Functions.ILike(c.Name, $"%{term}%"));
        }

        var list = await query
            .OrderByDescending(c => c.CreatedUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return Ok(list.Select(ToSummary).ToList());
    }

    /// <summary> Lấy chi tiết lớp </summary>
    [HttpGet("{id:int}")]
    [RequireAdmin]
    public async Task<ActionResult<ClassDetailDto>> Detail([FromRoute] int id, CancellationToken ct)
    {
        var cls = await db.Set<CourseClass>()
            .Include(c => c.Teacher)
            .Include(c => c.Members).ThenInclude(m => m.Student)
            .Include(c => c.Sessions)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (cls is null)
            return NotFound("Class not found.");
        return Ok(ToDetail(cls));
    }

    /// <summary> Cập nhật tên/mô tả lớp </summary>
    [HttpPut("{id:int}")]
    [RequireAdmin]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ClassUpdateDto dto, CancellationToken ct)
    {
        var cls = await db.Set<CourseClass>().FirstOrDefaultAsync(c => c.Id == id, ct);
        if (cls is null)
            return NotFound("Class not found.");

        cls.Name = dto.Name.Trim();
        cls.Description = dto.Description?.Trim();

        await classes.UpdateAsync(cls, ct);
        return NoContent();
    }

    /// <summary> Xoá lớp </summary>
    [HttpDelete("{id:int}")]
    [RequireAdmin]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        await classes.DeleteAsync(id, ct);
        return NoContent();
    }

    /// <summary> Gán teacher cho lớp </summary>
    [HttpPost("{id:int}/teacher/{teacherId:guid}")]
    [RequireAdmin]
    public async Task<IActionResult> SetTeacher([FromRoute] int id, [FromRoute] Guid teacherId, CancellationToken ct)
    {
        if (!await TeacherExistsAndValidAsync(teacherId, ct))
            return BadRequest("TeacherId không hợp lệ hoặc user không có role Teacher/Admin.");

        var cls = await db.Set<CourseClass>().FirstOrDefaultAsync(c => c.Id == id, ct);
        if (cls is null)
            return NotFound("Class not found.");

        cls.TeacherId = teacherId;
        await classes.UpdateAsync(cls, ct);

        return NoContent();
    }

    /// <summary> Thêm nhiều sinh viên vào lớp </summary>
    [HttpPost("{id:int}/students")]
    [RequireAdmin]
    public async Task<ActionResult<object>> AddStudents([FromRoute] int id, [FromBody] AddStudentsDto dto, CancellationToken ct)
    {
        var clsExists = await db.Set<CourseClass>().AnyAsync(c => c.Id == id, ct);
        if (!clsExists)
            return NotFound("Class not found.");

        var ids = dto.StudentIds?.Distinct().ToArray() ?? [];
        if (ids.Length == 0)
            return BadRequest("Danh sách StudentIds rỗng.");

        // Chỉ add user tồn tại (bỏ qua id không tồn tại)
        var exists = await db.Users
            .Where(u => ids.Contains(u.Id))
            .Select(u => u.Id)
            .ToListAsync(ct);

        int added = 0;
        foreach (var sid in exists)
        {
            await classes.AddStudentAsync(id, sid, ct);
            added++;
        }

        return Ok(new { requested = ids.Length, existed = exists.Count, added });
    }

    /// <summary> Xoá sinh viên khỏi lớp </summary>
    [HttpDelete("{id:int}/students/{studentId:guid}")]
    [RequireAdmin]
    public async Task<IActionResult> RemoveStudent([FromRoute] int id, [FromRoute] Guid studentId, CancellationToken ct)
    {
        await classes.RemoveStudentAsync(id, studentId, ct);
        return NoContent();
    }
}
