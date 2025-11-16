using GZCTF.Models.Data;

namespace GZCTF.Repositories.Interfaces;

public interface IClassExerciseRepository
{
    Task<List<ClassExercise>> ListByClassAsync(int classId, CancellationToken ct = default);
    Task<ClassExercise?> GetAsync(int classId, int exerciseId, CancellationToken ct = default);

    Task AddAsync(int classId, IEnumerable<(int exerciseId, bool enabled, DateTimeOffset? visibleFromUtc, DateTimeOffset? dueUtc, int order)> items, CancellationToken ct = default);
    Task UpdateAsync(ClassExercise entity, CancellationToken ct = default);
    Task RemoveAsync(int classId, int exerciseId, CancellationToken ct = default);
}
