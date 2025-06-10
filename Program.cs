using Microsoft.EntityFrameworkCore;
using Learntendo_backend.Data;
using Learntendo_backend.configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Cryptography;
using Learntendo_backend.Models;
using System.Text;
using Learntendo_backend.Mapping;
using Learntendo_backend.Services;
using Learntendo_backend.Hubs;
using Hangfire;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy to allow access from specified origins only
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5500", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Allow cookies and authorization headers
    });
});

// Add DbContext with SQL Server connection
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("defaultDbContext"));
});

// Add AutoMapper configuration
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Register application services
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped(typeof(IDataRepository<>), typeof(DataRepository<>));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<DailyResetService>();
builder.Services.AddScoped<GroupService>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

if (string.IsNullOrWhiteSpace(secretKey))
    throw new InvalidOperationException("JWT Secret Key is missing or empty in configuration.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        // Enable reading JWT token from query string for SignalR
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                // Only extract token if request is for the SignalR hub
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/ChatHub"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

// Register IUserIdProvider to identify UserId for each SignalR connection based on claims
builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

// Add SignalR, Controllers, and Razor Pages support
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// Configure Hangfire
builder.Services.AddHangfire(config =>
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSqlServerStorage(builder.Configuration.GetConnectionString("defaultDbContext")));

builder.Services.AddHangfireServer();

// Configure Serilog logging
builder.Logging.AddConsole();
builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
                 .ReadFrom.Services(services)
                 .Enrich.FromLogContext()
                 .WriteTo.Console());

// Configure Swagger with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Your API Title",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token like this: Bearer {your token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Enable CORS
app.UseCors("AllowFrontend");
var app = builder.Build();

// Enable CORS
app.UseCors("AllowFrontend");

app.UseRouting();

app.UseRouting();
// Enable Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Enable Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();
// Map SignalR Hub endpoint
app.MapHub<ChatHub>("/ChatHub");

// Map SignalR Hub endpoint
app.MapHub<ChatHub>("/ChatHub");
app.MapHub<LeaderboardHub>("/leaderboardHub");

// Map controller routes
app.MapControllers();

// Enable Hangfire Dashboard
app.UseHangfireDashboard();
app.MapHangfireDashboard();

// Schedule recurring Hangfire jobs
RecurringJob.AddOrUpdate<LeagueService>(
            "reset-monthly-xp",
            x => x.ProcessMonthlyLeague(),
            Cron.Monthly);

RecurringJob.AddOrUpdate<GroupService>(
    "test-group-assignment",
    service => service.AssignUsersToGroupsTest(),
    "*/30 * * * *",
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });

// Schedule daily reset job for DailyResetService
using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider.GetRequiredService<DailyResetService>();
    RecurringJob.AddOrUpdate("daily-reset", () => service.ResetDailyChallenges(), "0 0 * * *");
}

// Enable static files and HTTPS redirection
app.UseStaticFiles();
app.UseHttpsRedirection();

// Use Swagger only in Development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Seed the admin user if not present in the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    DatabaseSeeder.SeedAdmin(context);
}

// Run the application
app.Run();




// Seeder class to create an admin user if none exists
public static class DatabaseSeeder
{
    public static void SeedAdmin(DataContext context)
    {
        if (!context.Admin.Any())
        {
            var password = "adminadmin"; // Use a strong password in production
            byte[] passwordHash, passwordSalt;

            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            var admin = new Admin
            {
                Email = "admin@example.com",
                Username = "Admin",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            try
            {
                context.Admin.Add(admin);
                context.SaveChanges();
                Console.WriteLine("Admin saved successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving to the database: " + ex.Message);
            }
        }
    }
}
