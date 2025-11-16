using GZCTF.Models;
using GZCTF.Models.Data;
using GZCTF.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GZCTF.Repositories;

public class ClassRepository(AppDbContext db) : IClassRepository
{
    public async Task<CourseClass?> GetAsync(int classId, bool includeMembers = false, bool includeSessions = false, CancellationToken ct = default)
    {
        IQueryable<CourseClass> q = db.Set<CourseClass>();

        if (includeMembers)
            q = q.Include(c => c.Members).ThenInclude(m => m.Student);
        if (includeSessions)
            q = q.Include(c => c.Sessions);

        return await q.FirstOrDefaultAsync(c => c.Id == classId, ct);
    }

    public async Task<List<CourseClass>> ListByTeacherAsync(Guid teacherId, string? qSearch = null, int skip = 0, int take = 50, CancellationToken ct = default)
    {
        var q = db.Set<CourseClass>().AsNoTracking().Where(c => c.TeacherId == teacherId);

        if (!string.IsNullOrWhiteSpace(qSearch))
        {
            var term = qSearch.Trim();
            q = q.Where(c => EF.Functions.ILike(c.Code, $"%{term}%") || EF.Functions.ILike(c.Name, $"%{term}%"));
        }

        return await q.OrderByDescending(c => c.CreatedUtc).Skip(skip).Take(take).ToListAsync(ct);
    }

    public Task<bool> ExistsCodeAsync(string code, CancellationToken ct = default)
    {
        var norm = code.Trim();
        return db.Set<CourseClass>().AnyAsync(c => c.Code == norm, ct);
    }

    public async Task<CourseClass> CreateAsync(CourseClass entity, CancellationToken ct = default)
    {
        entity.Code = entity.Code.Trim();
        entity.Name = entity.Name.Trim();

        if (await ExistsCodeAsync(entity.Code, ct))
            throw new InvalidOperationException("Class code already exists.");

        await db.Set<CourseClass>().AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(CourseClass entity, CancellationToken ct = default)
    {
        db.Set<CourseClass>().Update(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int classId, CancellationToken ct = default)
    {
        // EF Core 7+ supports ExecuteDeleteAsync; fallback to load-remove if provider doesn’t support.
        var affected = await db.Set<CourseClass>().Where(c => c.Id == classId).ExecuteDeleteAsync(ct);
        if (affected == 0)
        {
            var entity = await db.Set<CourseClass>().FindAsync([classId], ct);
            if (entity is null)
                return;
            db.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }

    public async Task AddStudentAsync(int classId, Guid studentId, CancellationToken ct = default)
    {
        var exists = await db.Set<CourseMember>().AnyAsync(m => m.ClassId == classId && m.StudentId == studentId, ct);
        if (exists)
            return;

        await db.Set<CourseMember>().AddAsync(new CourseMember { ClassId = classId, StudentId = studentId }, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task RemoveStudentAsync(int classId, Guid studentId, CancellationToken ct = default)
    {
        var affected = await db.Set<CourseMember>()
            .Where(m => m.ClassId == classId && m.StudentId == studentId)
            .ExecuteDeleteAsync(ct);

        if (affected == 0)
        {
            var m = await db.Set<CourseMember>().FirstOrDefaultAsync(m => m.ClassId == classId && m.StudentId == studentId, ct);
            if (m is null)
                return;
            db.Remove(m);
            await db.SaveChangesAsync(ct);
        }
    }

    public Task<bool> IsMemberAsync(int classId, Guid studentId, CancellationToken ct = default) =>
        db.Set<CourseMember>().AnyAsync(m => m.ClassId == classId && m.StudentId == studentId, ct);

    public Task<CourseMember?> GetMembershipAsync(int classId, Guid studentId, CancellationToken ct = default) =>
        db.Set<CourseMember>().AsNoTracking().FirstOrDefaultAsync(m => m.ClassId == classId && m.StudentId == studentId, ct);
}
