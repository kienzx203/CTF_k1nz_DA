namespace GZCTF.Contracts.Admin;

public record ClassCreateRequest(string Code, string Name, string? Term, string? Description, Guid? OwnerId);
public record ClassUpdateRequest(string Name, string? Term, string? Description, Guid? OwnerId, bool IsArchived);

public record AddMembersRequest(IReadOnlyList<Guid> UserIds);

public record ClassListItemDto(int Id, string Code, string Name, string? Term, bool IsArchived, int TeacherCount, int StudentCount, DateTimeOffset CreatedAtUtc);

public record MemberDto(Guid UserId, string UserName, string? RealName, string? Email, string RoleInClass, string Status);

public record ClassDetailDto(
    int Id, string Code, string Name, string? Term, string? Description, bool IsArchived,
    Guid? OwnerId, string? OwnerName,
    DateTimeOffset CreatedAtUtc, DateTimeOffset? UpdatedAtUtc,
    IReadOnlyList<MemberDto> Teachers,
    IReadOnlyList<MemberDto> Students
);
