using Microsoft.AspNetCore.Identity;
using SubPayment.Data;
using SubPayment.Models;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var hasher = new PasswordHasher<User>();

        var admin = await userRepo.GetByUsernameAsync("admin");
        if (admin != null)
            return;

        var user = new User
        {   
            Username = "admin",
            Role = "Admin",
            IsPremium = false
        };

        user.PasswordHash = hasher.HashPassword(user, "password");

        await userRepo.AddAsync(user);

        Console.WriteLine("✅ Admin user seeded");
    }
}
