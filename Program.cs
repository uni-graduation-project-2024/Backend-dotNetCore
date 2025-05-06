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
using Hangfire;


var builder = WebApplication.CreateBuilder(args);
/////////////////
builder.Services.AddSignalR();

//AddAutoMapper
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// إضافة خدمات CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});


builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("defaultDbContext"));
});



//jwtSettings
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped(typeof(IDataRepository<>), typeof(DataRepository<>));

var secretKey = builder.Configuration["JwtSettings:SecretKey"];
if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new InvalidOperationException("JWT Secret Key is missing or empty in configuration.");
}
var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
         var secretKey = builder.Configuration["JwtSettings:SecretKey"];
       // var secretKey = _configuration["JwtSettings:SecretKey"];
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("JWT Secret Key is missing or empty in configuration.");
        }
        var key = Encoding.UTF8.GetBytes(secretKey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
       
            RoleClaimType = "role"
            
        };
    
    });


builder.Logging.AddConsole();
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()); // Or other sinks like files, databases, etc.


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddHangfire(config =>
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSqlServerStorage(builder.Configuration.GetConnectionString("defaultDbContext")));


builder.Services.AddHangfireServer();
builder.Services.AddScoped<DailyResetService>();
builder.Services.AddScoped<GroupService>();




var app = builder.Build();
//<summary>


app.UseHangfireDashboard();
app.MapHangfireDashboard();


RecurringJob.AddOrUpdate<LeagueService>(
            "reset-monthly-xp",
            x => x.ProcessMonthlyLeague(),
            Cron.Monthly);


//RecurringJob.AddOrUpdate<GroupService>(
//    "weekly-group-assignment",
//    service => service.AssignUsersToGroupsTest(),
//    Cron.Weekly(DayOfWeek.Saturday, 0, 0),
//    new RecurringJobOptions
//    {
//        TimeZone = TimeZoneInfo.Local
//    });

RecurringJob.AddOrUpdate<GroupService>(
    "test-group-assignment",
    service => service.AssignUsersToGroupsTest(),
    "*/30 * * * *", 
    new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.Local
    });


//https://localhost:7078/hangfire HangfireDashboard
using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider.GetRequiredService<DailyResetService>();
    RecurringJob.AddOrUpdate("daily-reset", () => service.ResetDailyChallenges(), "0 0 * * *");
}
//</summary>
//maha
//RecurringJob.AddOrUpdate<GroupService>(
//    job => job.AssignUsersToGroups(),
//    Cron.Weekly(DayOfWeek.Saturday, 0, 0));






if (string.IsNullOrEmpty(app.Environment.WebRootPath))
{
    app.Environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
}//==builder.WebHost.UseWebRoot("wwwroot");
app.UseStaticFiles();














// استدعاء دالة SeedAdmin لإضافة Admin إذا لم يكن موجودًا
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    DatabaseSeeder.SeedAdmin(context);
}

app.UseCors("AllowAll"); // Apply CORS policy

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseRouting();
app.UseWebSockets();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseEndpoints(endpoints =>
{    });


app.MapControllers();

app.Run();

// دالة SeedAdmin لإضافة Admin تلقائيًا إذا لم يكن موجودًا
public static class DatabaseSeeder
{
    
    public static void SeedAdmin(DataContext context)
    {
        if (!context.Admin.Any())
        {
            var password = "adminadmin"; // Use a strong password
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



    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

   


}
