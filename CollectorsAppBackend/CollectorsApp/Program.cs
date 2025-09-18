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


// Configure logging providers used by the app
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

// CORS policy allowing the local frontend origins
builder.Services.AddCors(
        options => options.AddPolicy(
        name: "CorsPolicy", policy =>
        {
            if (builder.Environment.IsDevelopment())
            {

                policy.WithOrigins("http://localhost:3000", "https://localhost:3000", "http://127.0.0.1:3000", "https://127.0.0.1:3000")
                    // Allow localhost/127.0.0.1 on any scheme during development
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }
            else
            {
                policy.WithOrigins(/*frontend url*/)
                .WithMethods("GET", "PUT", "POST", "DELETE", "PATCH")
                .AllowAnyHeader()
                .AllowCredentials();
            }
        }
    )
);

// Load secrets from Google Secret Manager into configuration
builder.Configuration.AddGoogleSecretManager();

builder.Services.AddMemoryCache();
builder.Services.AddControllers();

// Secret storage service (Google) registered as singleton
builder.Services.AddSingleton<IGoogleSecretStorageVault, GoogleSecretStorageVault>();

// Resolve secrets from configuration (populated by Google Secret Manager provider)
string conn = builder.Configuration["GoogleSecretStorage:Resolved:DB-STRING"]
    ?? builder.Configuration["GoogleSecretStorage:Secrets:DB-STRING"] // fallback if provider not configured
    ?? throw new InvalidOperationException("Database connection string secret not found in configuration.");
string jwtKey = builder.Configuration["GoogleSecretStorage:Resolved:JWT_KEY"]
    ?? builder.Configuration["GoogleSecretStorage:Secrets:JWT_KEY"]
    ?? throw new InvalidOperationException("JWT key secret not found in configuration.");

// Authentication: configure JWT bearer token validation
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

// Authorization policies and handlers
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

// Database context using MySQL; connection string sourced from secret storage
builder.Services.AddDbContext<appDatabaseContext>(
    options => 
    { 
        options.UseMySQL(conn);
    });

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddSwaggerGen();

// Repositories (DI registrations for data access abstractions)
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

// App services
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