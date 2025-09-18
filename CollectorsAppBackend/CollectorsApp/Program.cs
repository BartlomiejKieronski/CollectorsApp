using CollectorsApp.Data;
using CollectorsApp.Mappings;
using CollectorsApp.Middleware;
using CollectorsApp.Models;
using CollectorsApp.Policies;
using CollectorsApp.Repository;
using CollectorsApp.Repository.AnalyticsRepositories;
using CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Authentication;
using CollectorsApp.Services.Cookie;
using CollectorsApp.Services.Email;
using CollectorsApp.Services.Encryption;
using CollectorsApp.Services.Security;
using CollectorsApp.Services.Security.Configuration;
using CollectorsApp.Services.Token;
using CollectorsApp.Services.User;
using CollectorsApp.Services.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;

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

//authentication
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

// Authorization handlers
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

// AutoMapper with defined mapping profile
builder.Services.AddAutoMapper(e =>
{
    e.AddProfile<MappingProfile>();
});

builder.Services.AddDbContext<appDatabaseContext>(
    options => 
    { 
        options.UseMySQL(conn);
    });

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddSwaggerGen();

//repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICollectableItemRepository, CollectableItemRepository>();
builder.Services.AddScoped<ICollectionRepository, CollectionRepository>();
builder.Services.AddScoped<IImagePathRepository, ImagePathRepository>();
builder.Services.AddScoped<IAuthorizationRepository, AuthorizationRepository>();
builder.Services.AddScoped<IPwdResetRepository, PwdResetRepository>();
builder.Services.AddScoped<IUserClaimsRepository, UserClaimsRepository>();
builder.Services.AddScoped<IAdminCommentRepository, AdminCommentRepository>();
builder.Services.AddScoped<IAPILogRepository, APILogRepository>();
builder.Services.AddScoped<IUserPreferencesRepository,UserPreferencesRepository>();
builder.Services.AddScoped<IUserConsentRepository, UserConsentRepository>();

//services
builder.Services.AddTransient<IEmailSenderService, EmailSenderService>();
builder.Services.AddScoped<IDataHash, DataHash>();
builder.Services.AddScoped<IAesEncryption, AesEncryption>();
builder.Services.AddScoped<IUserAesDecode, UserAesDecode>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

/// Rate limiting middleware for logging in
builder.Services.AddLoginRateLimiter();

var app = builder.Build();


app.UseProblemDetails();
app.UseForwardedHeaders();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

// Request logging middleware for controllers
app.UseControllerLoggingMiddleware();
app.UseAuthentication();
app.UseAuthorization();

/// Middleware to extract name from json body for rate limiting on Authentication endpoint
app.UseLoginUsernameExtraction();

app.UseRateLimiter();

app.MapControllers();

app.Run();