using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Logging;
using Microsoft.EntityFrameworkCore;
using SubPayment.Data;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var config = builder.Configuration;
    
        var connectionString =
            builder.Configuration["mssqlconnection:ConnectionString"];
        
using (SqlConnection conn = new(connectionString))
{
    conn.Open();
    Console.WriteLine("Connected successfully!");
            using SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "IF DB_ID('MyDatabase') IS NULL CREATE DATABASE MyDatabase;";
            cmd.ExecuteNonQuery();
        }
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
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = ctx =>
                    {
                        Console.WriteLine("Auth failed."); 
                        Console.WriteLine("Exception: " + ctx.Exception); 
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddHttpClient<FlutterwaveSandboxService>();
        builder.Services.AddHttpClient<AfricasTalkingService>();
        builder.Services.AddAuthorization();
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        IdentityModelEventSource.ShowPII = true;

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
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