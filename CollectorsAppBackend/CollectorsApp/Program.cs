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
using CollectorsApp.Middleware.RateLimiters;
using Microsoft.AspNetCore.RateLimiting; 

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
        options.UseMySQL(conn, mySqlOptions =>
        {
            mySqlOptions.CommandTimeout(60);
            mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
        });
    });

// Bind email settings
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


// Rate limiting registrations
builder.Services.AddLoginRateLimiter();
builder.Services.AddCrudRateLimiters(); // Policies only; automatic assignment occurs after MapControllers.

// HSTS enforcement
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
    options.ExcludedHosts.Add("localhost");
    options.ExcludedHosts.Add("127.0.0.1");
    options.ExcludedHosts.Add("[::1]");
});

//Enforce HTTPS redirection (308). With forwarded headers this respects the original scheme.
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
});

// Forwarded headers for reverse proxy 
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor
        | ForwardedHeaders.XForwardedProto
        | ForwardedHeaders.XForwardedHost;
    options.RequireHeaderSymmetry = false;
    options.ForwardLimit = null;

    // Cloudflared originates from localhost; trust loopback in prod.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
    if (!builder.Environment.IsDevelopment())
    {
        options.KnownProxies.Add(IPAddress.Loopback);
        options.KnownProxies.Add(IPAddress.IPv6Loopback);
    }
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    //Mime types can be added as needed
});

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    // Apply any pending migrations, create DB if not exists if development
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<appDatabaseContext>();
        db.Database.Migrate();
    }
}
// Global exception handling middleware
app.UseProblemDetails();

// Forwarded headers support (for reverse proxy scenarios)
app.UseForwardedHeaders();

app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    // Swagger enabled in Development environment
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // HSTS only in Production
    app.UseHsts();
}

// Redirect HTTP -> HTTPS (after ForwardedHeaders so scheme is corrected)

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    // Content Security Policy (adjust as needed), restrict sources to load resources from to self, help prevent XSS data injection attacks
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self'; script-src 'self'; style-src 'self'; object-src 'none';";

    // Prevent MIME type sniffing, 
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";

    // Prevent clickjacking
    context.Response.Headers["X-Frame-Options"] = "DENY";

    await next();
});


app.UseRouting();

// CORS must run before auth when it needs to handle preflight
app.UseCors("CorsPolicy");

// Request logging middleware for controllers
///TO FIX!!! INTERFIERS WITH CORS
///app.UseControllerLoggingMiddleware(); 

// Authentication/Authorization order matters: authenticate first, then authorize
app.UseAuthentication();
app.UseAuthorization();

/// Middleware to extract name from json body for rate limiting on Authentication endpoint
app.UseLoginUsernameExtraction();

app.UseRateLimiter();

// Map controllers and automatically apply default rate limiting policies for write operations
var controllerEndpoints = app.MapControllers();
controllerEndpoints.Add(endpointBuilder =>
{
    // Skip if endpoint explicitly defines its own rate limiting or disables it
    if (endpointBuilder.Metadata.OfType<EnableRateLimitingAttribute>().Any() ||
        endpointBuilder.Metadata.OfType<DisableRateLimitingAttribute>().Any())
    {
        return;
    }

    var httpMeta = endpointBuilder.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault();
    if (httpMeta == null) return;

    var methods = httpMeta.HttpMethods;

    string? policy = null;
    if (methods.Contains("POST")) policy = "PostPolicy";
    else if (methods.Contains("PUT")) policy = "PutPolicy";
    else if (methods.Contains("PATCH")) policy = "PatchPolicy";
    else if (methods.Contains("DELETE")) policy = "DeletePolicy";

    if (policy != null)
    {
        endpointBuilder.Metadata.Add(new EnableRateLimitingAttribute(policy));
    }
});

app.Run();