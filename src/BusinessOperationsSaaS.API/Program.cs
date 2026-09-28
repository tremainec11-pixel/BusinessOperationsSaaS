using System.IdentityModel.Tokens.Jwt;
using System.Text;

using Microsoft.OpenApi;

using BusinessOperationsSaaS.Infrastructure.Subscriptions.Services;
using BusinessOperationsSaaS.Application.Subscriptions.Services;
using BusinessOperationsSaaS.Application.Subscriptions.Interfaces;
using BusinessOperationsSaaS.Infrastructure.Subscriptions;
using BusinessOperationsSaaS.Application.Financials.Interfaces;
using BusinessOperationsSaaS.Application.Dashboard.Interfaces;
using BusinessOperationsSaaS.Infrastructure.Financials;
using BusinessOperationsSaaS.Infrastructure.Dashboard;
using BusinessOperationsSaaS.Application.Products.Interfaces;
using BusinessOperationsSaaS.Infrastructure.Products;
using BusinessOperationsSaaS.Application.Tasks.Interfaces;
using BusinessOperationsSaaS.Infrastructure.Tasks;
using BusinessOperationsSaaS.Application.Employees.Interfaces;
using BusinessOperationsSaaS.Infrastructure.Employees;
using BusinessOperationsSaaS.Application.Authentication.Interfaces;
using BusinessOperationsSaaS.Application.Authentication.Settings;
using BusinessOperationsSaaS.Infrastructure.Authentication;
using BusinessOperationsSaaS.Infrastructure.Data;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Configuration.AddUserSecrets<Program>(optional: true);

var configuredJwtKey = builder.Configuration["JwtSettings:Key"];

Console.WriteLine(
    $"JWT Key loaded: {!string.IsNullOrWhiteSpace(configuredJwtKey)}"
);

Console.WriteLine(
    $"JWT Key length: {configuredJwtKey?.Length ?? 0}"
);

Console.WriteLine(
    $"Stripe SecretKey loaded: {!string.IsNullOrWhiteSpace(
        builder.Configuration["StripeSettings:SecretKey"]
    )}"
);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BusinessOperationsSaaS API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);

builder.Services.AddSingleton<PasswordHasherService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IFinancialService, FinancialService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<ISubscriptionLimitService, SubscriptionLimitService>();
builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddScoped<StripeWebhookService>();

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "JwtSettings configuration is missing."
    );

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.Key)
        ),

        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/api/health", () => Results.Ok(new { status = "healthy" }));

app.Run();


