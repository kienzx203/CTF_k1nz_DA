using GZCTF.Models.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class SeedUsers
{
    public static async Task RunAsync(IServiceProvider sp)
    {
        var um = sp.GetRequiredService<UserManager<UserInfo>>();

        // Admin mặc định
        var admin = await um.Users.FirstOrDefaultAsync(u => u.UserName == "Admin");
        if (admin == null)
        {
            admin = new UserInfo
            {
                UserName = "Admin",
                Email = "admin@gzti.me",
                Role = Role.Admin, // <--- GÁN ROLE Ở ĐÂY
                EmailConfirmed = true,
                RegisterTimeUtc = DateTimeOffset.UtcNow
            };
            var ok = await um.CreateAsync(admin, "Admin@2022");
            if (!ok.Succeeded)
                throw new Exception(string.Join("; ", ok.Errors.Select(e => e.Description)));
        }
        else if (admin.Role != Role.Admin)
        {
            admin.Role = Role.Admin;
            await um.UpdateAsync(admin);
        }

        // Gợi ý: seed 1 teacher + 1 student (tuỳ bạn)
        var t = await um.Users.FirstOrDefaultAsync(u => u.UserName == "teacher1");
        if (t == null)
        {
            t = new UserInfo { UserName = "teacher1", Email = "t1@ex.com", Role = Role.Teacher, EmailConfirmed = true };
            await um.CreateAsync(t, "Teacher@2022");
        }
        var s = await um.Users.FirstOrDefaultAsync(u => u.UserName == "student1");
        if (s == null)
        {
            s = new UserInfo { UserName = "student1", Email = "s1@ex.com", Role = Role.User, EmailConfirmed = true };
            await um.CreateAsync(s, "Student@2022");
        }
    }
}
