using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Data.SqlClient;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var config = builder.Configuration;

        var connectionString =
            builder.Configuration.GetSection("mssqlconnection")
                                .GetValue<string>("ConnectionString");

        using (SqlConnection conn = new(connectionString)) 
            { 
                conn.Open(); 
                Console.WriteLine("Connected successfully!"); 
            }

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                    ),
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddHttpClient<FlutterwaveSandboxService>();
        builder.Services.AddHttpClient<AfricasTalkingService>();
        builder.Services.AddAuthorization();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGet("/health", () => "OK");

        app.MapGet("/secure", () => "You are authenticated!")
           .RequireAuthorization();

        app.MapControllers();
        app.Run();
    }
}