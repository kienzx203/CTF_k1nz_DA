using GZCTF.Models;
using GZCTF.Models.Data;

namespace GZCTF.Repositories.Interfaces;

public interface IClassRepository
{
    Task<CourseClass?> GetAsync(int classId, bool includeMembers = false, bool includeSessions = false, CancellationToken ct = default);

    Task<List<CourseClass>> ListByTeacherAsync(
        Guid teacherId,
        string? q = null,
        int skip = 0,
        int take = 50,
        CancellationToken ct = default);

    Task<bool> ExistsCodeAsync(string code, CancellationToken ct = default);

    Task<CourseClass> CreateAsync(CourseClass entity, CancellationToken ct = default);
    Task UpdateAsync(CourseClass entity, CancellationToken ct = default);
    Task DeleteAsync(int classId, CancellationToken ct = default);

    Task AddStudentAsync(int classId, Guid studentId, CancellationToken ct = default);
    Task RemoveStudentAsync(int classId, Guid studentId, CancellationToken ct = default);
    Task<bool> IsMemberAsync(int classId, Guid studentId, CancellationToken ct = default);
    Task<CourseMember?> GetMembershipAsync(int classId, Guid studentId, CancellationToken ct = default);
}
