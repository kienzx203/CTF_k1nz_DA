using GZCTF.Models;
using GZCTF.Models.Data;
using GZCTF.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GZCTF.Repositories;

public class ClassExerciseRepository(AppDbContext db) : IClassExerciseRepository
{
    public Task<List<ClassExercise>> ListByClassAsync(int classId, CancellationToken ct = default) =>
        db.ClassExercises.AsNoTracking()
          .Include(x => x.Exercise)
          .Where(x => x.ClassId == classId)
          .OrderBy(x => x.Order)
          .ThenBy(x => x.ExerciseId)
          .ToListAsync(ct);

    public Task<ClassExercise?> GetAsync(int classId, int exerciseId, CancellationToken ct = default) =>
        db.ClassExercises
          .Include(x => x.Exercise)
          .FirstOrDefaultAsync(x => x.ClassId == classId && x.ExerciseId == exerciseId, ct);

    public async Task AddAsync(int classId, IEnumerable<(int exerciseId, bool enabled, DateTimeOffset? visibleFromUtc, DateTimeOffset? dueUtc, int order)> items, CancellationToken ct = default)
    {
        var exists = await db.ClassExercises.AsNoTracking()
            .Where(x => x.ClassId == classId)
            .Select(x => x.ExerciseId)
            .ToListAsync(ct);
        var set = new HashSet<int>(exists);

        var toAdd = new List<ClassExercise>();
        foreach (var (exerciseId, enabled, visibleFromUtc, dueUtc, order) in items)
        {
            if (set.Contains(exerciseId))
                continue;
            toAdd.Add(new ClassExercise
            {
                ClassId = classId,
                ExerciseId = exerciseId,
                Enabled = enabled,
                VisibleFromUtc = visibleFromUtc,
                DueUtc = dueUtc,
                Order = order
            });
        }

        if (toAdd.Count == 0)
            return;
        await db.ClassExercises.AddRangeAsync(toAdd, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(ClassExercise entity, CancellationToken ct = default)
    {
        db.ClassExercises.Update(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(int classId, int exerciseId, CancellationToken ct = default)
    {
        var affected = await db.ClassExercises
            .Where(x => x.ClassId == classId && x.ExerciseId == exerciseId)
            .ExecuteDeleteAsync(ct);

        if (affected == 0)
        {
            var e = await db.ClassExercises.FirstOrDefaultAsync(x => x.ClassId == classId && x.ExerciseId == exerciseId, ct);
            if (e is null)
                return;
            db.Remove(e);
            await db.SaveChangesAsync(ct);
        }
    }
}
