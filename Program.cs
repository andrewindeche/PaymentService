using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SubPayment.Data;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var config = builder.Configuration;
    
        var connectionString =
            builder.Configuration["mssqlconnection:ConnectionString"];
        builder.Services.AddDbContext<UserDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddScoped<IUserRepository, SqlUserRepository>();
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {   
                var keyString = builder.Configuration["Jwt:Key"]!.Trim(); 
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"]?.Trim(),
                    ValidAudience = builder.Configuration["Jwt:Audience"]?.Trim(),
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddHttpClient<FlutterwaveSandboxService>();
        builder.Services.AddSingleton<AfricasTalkingService>();
        builder.Services.AddAuthorization();
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        using (var scope = app.Services.CreateScope()) 
        { 
            var db = scope.ServiceProvider.GetRequiredService<UserDbContext>(); 
            db.Database.Migrate();
            await DbSeeder.SeedAdminAsync(scope.ServiceProvider);
        }

        //app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGet("/health", () => "OK");

        app.MapGet("/secure", () => "You are authenticated!")
           .RequireAuthorization();

        app.MapControllers();
        app.Run();
    }
}