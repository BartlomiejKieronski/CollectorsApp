using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CollectorsApp.Data;
using CollectorsApp.Repository;
using CollectorsApp.Models;
using CollectorsApp.Policies;
using Microsoft.AspNetCore.Authorization;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Encryption;
using CollectorsApp.Services.Email;
using CollectorsApp.Services.Utility;
using System.Threading.RateLimiting;
using System.Text.Json;
using CollectorsApp.Services.Token;
using CollectorsApp.Services.Cookie;
using CollectorsApp.Services.Authentication;
using CollectorsApp.Services.Security;
using CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces;
using CollectorsApp.Repository.AnalyticsRepositories;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

builder.Services.AddCors(
    options => options.AddPolicy(
        name: "CorsPolicy", policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        }
    )
);

builder.Services.AddMemoryCache();
builder.Services.AddControllers();

builder.Services.AddSingleton<IGoogleSecretStorageVault, GoogleSecretStorageVault>();

using var serviceProvider = builder.Services.BuildServiceProvider();
var secretService = serviceProvider.GetRequiredService<IGoogleSecretStorageVault>();
string conn = await secretService.GetSecretsAsync(builder.Configuration["GoogleSecretStorage:Secrets:DB-STRING"]);
string jwtKey = await secretService.GetSecretsAsync(builder.Configuration["GoogleSecretStorage:Secrets:JWT_KEY"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateActor = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization(options => { 
    
    options.AddPolicy("ResourceOwner",
        p => p.Requirements.Add(new ResourceOwnerRequirement()));
        
    options.AddPolicy("EntityOwner",
        p => p.Requirements.Add(new EntityOwnerRequirement()));

});

builder.Services.AddSingleton<IAuthorizationHandler, ResourceOwnerHandler>();
builder.Services.AddScoped<IAuthorizationHandler, EntityOwnerHandler>();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<appDatabaseContext>(
    options => 
    { 
        options.UseMySQL(conn);
    });

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddTransient<IEmailSenderService, EmailSenderService>();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICollectableItemsRepository, CollectableItemsRepository>();
builder.Services.AddScoped<ICollectionRepository, CollectionRepository>();
builder.Services.AddScoped<IImagePathRepository, ImagePathRepository>();
builder.Services.AddScoped<IAuthorizationRepository, Authorization>();
builder.Services.AddScoped<IPwdReset, PwdReset>();
builder.Services.AddScoped<IDataHash, DataHash>();
builder.Services.AddScoped<IAesEncryption, AesEncryption>();
builder.Services.AddScoped<IUserClaims, UserClaims>();
builder.Services.AddScoped<IUserAesDecode, UserAesDecode>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminCommentsRepository, AdminCommentsRepository>();
builder.Services.AddScoped<IAPILogRepository, APILogRepository>();


/// Rate limiting middleware for logging in
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;  

    options.AddPolicy("LoginPolicy", context =>
    {
        var username = context.Items["username"] as string ?? "anon";
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var key = $"{username}|{ip}";
        return RateLimitPartition.GetSlidingWindowLimiter(key, _ => new SlidingWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(5),
            SegmentsPerWindow = 5,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0

        });
    });
});


var app = builder.Build();

app.UseForwardedHeaders();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

/// Middleware to extract name from json body for rate limiting on Authentication endpoint
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path == "/api/Authentication"
        && ctx.Request.Method == "POST"
        && ctx.Request.ContentType?.Contains("application/json") == true)
    {
        ctx.Request.EnableBuffering();
        using var reader = new StreamReader(ctx.Request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        ctx.Request.Body.Position = 0;

        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("name", out var nameProp))
            {
                ctx.Items["username"] = nameProp.GetString()!;
            }
        }
        catch (JsonException)
        {

        }
    }
    await next();
});

app.UseRateLimiter();

app.MapControllers();

app.Run();
