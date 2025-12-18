using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using POS_TranVietTraLam_Fresher_BLL.Defines;
using POS_TranVietTraLam_Fresher_BLL.DTO.CommonDTO;
using POS_TranVietTraLam_Fresher_BLL.Hubs;
using POS_TranVietTraLam_Fresher_DAL.Context;
using POS_TranVietTraLam_Fresher_DAL.Defines;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===== Controllers =====
builder.Services.AddControllers();
builder.Services.AddSignalR();
// ===== Supabase =====
var supabaseConfig = builder.Configuration.GetSection("Supabase");
var url = supabaseConfig["Url"];
var anonKey = supabaseConfig["Key"];

builder.Services.AddSingleton(provider => new Supabase.Client(url!, anonKey!));

// ===== PostgreSQL =====
builder.Services.AddDbContext<POSContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            npgsqlOptions.CommandTimeout(60);
        });
});

// ===== DI Repositories & Services =====
builder.Services.Scan(scan => scan
    .FromAssemblies(typeof(IAuthService).Assembly, typeof(IUserRepository).Assembly)
    .AddClasses()
    .AsImplementedInterfaces()
    .WithScopedLifetime());

// ===== IHttpContextAccessor & AuthenticatedUser =====
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthenticatedUser, AuthenticatedUser>();

builder.Services.AddHttpClient();

// ===== AppSettings =====
builder.Services.Configure<PayOSSettings>(builder.Configuration.GetSection("PayOS"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));

// ===== MemoryCache =====
builder.Services.AddMemoryCache();

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins(
                "https://pos-lamtran213-ui.vercel.app",
                "http://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // SignalR cần
    });
});


// ===== JWT Authentication =====
var accessKey = builder.Configuration["Jwt:AccessSecretKey"]!;
var issuerConfig = builder.Configuration["Jwt:Issuer"]!;
var audienceConfig = builder.Configuration["Jwt:Audience"]!;
var key = Encoding.UTF8.GetBytes(accessKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = issuerConfig,
            ValidAudience = audienceConfig,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                context.HttpContext.Response.Headers.Add("X-JWT-Error", context.Exception.GetType().Name);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// ===== Swagger =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "POS.Lamtran213 API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Paste JWT token here with 'Bearer ' prefix",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
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

// ===== Build App =====
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "POS.Lamtran213 API v1"));

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<POSHubs>("/posHub");
app.MapControllers();

app.Run();
