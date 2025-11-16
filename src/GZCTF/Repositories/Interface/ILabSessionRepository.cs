using GZCTF.Models.Data;

namespace GZCTF.Repositories.Interfaces;

public interface ILabSessionRepository
{
    Task<LabSession?> GetAsync(int sessionId, bool includeChallenges = false, bool includeReports = false, CancellationToken ct = default);

    Task<List<LabSession>> ListByClassAsync(int classId, CancellationToken ct = default);

    Task<LabSession> CreateAsync(LabSession entity, CancellationToken ct = default);
    Task UpdateAsync(LabSession entity, CancellationToken ct = default);
    Task DeleteAsync(int sessionId, CancellationToken ct = default);

    // Quản lý bài trong buổi
    Task AddExercisesAsync(int sessionId, IEnumerable<(int exerciseId, int weight, int order, bool required)> items, CancellationToken ct = default);
    Task RemoveExerciseAsync(int sessionId, int exerciseId, CancellationToken ct = default);
}
