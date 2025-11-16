using GZCTF.Models;
using GZCTF.Models.Data;
using GZCTF.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GZCTF.Repositories;

public class LabSessionRepository(AppDbContext db) : ILabSessionRepository
{
    public async Task<LabSession?> GetAsync(int sessionId, bool includeChallenges = false, bool includeReports = false, CancellationToken ct = default)
    {
        IQueryable<LabSession> q = db.Set<LabSession>();

        if (includeChallenges)
            q = q.Include(s => s.Challenges).ThenInclude(sc => sc.Exercise);

        if (includeReports)
            q = q.Include(s => s.Reports).ThenInclude(r => r.Student);

        return await q.FirstOrDefaultAsync(s => s.Id == sessionId, ct);
    }

    public Task<List<LabSession>> ListByClassAsync(int classId, CancellationToken ct = default) =>
        db.Set<LabSession>()
          .AsNoTracking()
          .Where(s => s.ClassId == classId)
          .OrderByDescending(s => s.StartUtc ?? DateTimeOffset.MinValue)
          .ThenBy(s => s.Id)
          .ToListAsync(ct);

    public async Task<LabSession> CreateAsync(LabSession entity, CancellationToken ct = default)
    {
        await db.Set<LabSession>().AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(LabSession entity, CancellationToken ct = default)
    {
        db.Set<LabSession>().Update(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int sessionId, CancellationToken ct = default)
    {
        var affected = await db.Set<LabSession>().Where(s => s.Id == sessionId).ExecuteDeleteAsync(ct);
        if (affected == 0)
        {
            var entity = await db.Set<LabSession>().FindAsync([sessionId], ct);
            if (entity is null)
                return;
            db.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }

    public async Task AddExercisesAsync(int sessionId, IEnumerable<(int exerciseId, int weight, int order, bool required)> items, CancellationToken ct = default)
    {
        var toAdd = new List<LabSessionChallenge>();
        foreach (var (exerciseId, weight, order, required) in items)
        {
            var exists = await db.Set<LabSessionChallenge>()
                .AnyAsync(sc => sc.SessionId == sessionId && sc.ExerciseId == exerciseId, ct);
            if (exists)
                continue;

            toAdd.Add(new LabSessionChallenge
            {
                SessionId = sessionId,
                ExerciseId = exerciseId,
                Weight = weight,
                Order = order,
                Required = required
            });
        }

        if (toAdd.Count == 0)
            return;
        await db.Set<LabSessionChallenge>().AddRangeAsync(toAdd, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task RemoveExerciseAsync(int sessionId, int exerciseId, CancellationToken ct = default)
    {
        var affected = await db.Set<LabSessionChallenge>()
            .Where(sc => sc.SessionId == sessionId && sc.ExerciseId == exerciseId)
            .ExecuteDeleteAsync(ct);

        if (affected == 0)
        {
            var sc = await db.Set<LabSessionChallenge>()
                .FirstOrDefaultAsync(x => x.SessionId == sessionId && x.ExerciseId == exerciseId, ct);
            if (sc is null)
                return;
            db.Remove(sc);
            await db.SaveChangesAsync(ct);
        }
    }
}
