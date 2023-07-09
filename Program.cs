using AnimeVnInfoBackend.DataContext;
using AnimeVnInfoBackend.Models;
using AnimeVnInfoBackend.Repositories;
using AnimeVnInfoBackend.Services;
using AnimeVnInfoBackend.Utilities.Assemblies;
using AnimeVnInfoBackend.Utilities.AuthorizePolicies;
using AnimeVnInfoBackend.Utilities.TokenProviders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddCors(option => option.AddDefaultPolicy(policy => policy
    .WithOrigins(configuration.GetValue<string>("ClientUrl") ?? "", configuration.GetValue<string>("SubClientUrl") ?? "")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowedToAllowWildcardSubdomains()
));

//services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection") ?? ""));
//services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("AzureConnection")));
services.AddIdentity<User, Role>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders()
        .AddTokenProvider<TwoFactorCodeTokenProvider<User>>("TwoFactorCode");

services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

services.AddAutoMapper(typeof(AnimeMapperProfile).Assembly);

// ===== Add Jwt Authentication ========

services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = configuration.GetValue<string>("ServerUrl"),
        ValidAudience = configuration.GetValue<string>("ClientUrl"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("SecretKey") ?? "")),
        ClockSkew = TimeSpan.Zero,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true
    };
});
//services.AddAuthorization(cfg =>
//{
//    cfg.AddPolicy("Admin", policy => policy.RequireClaim("type", "Admin"));
//    cfg.AddPolicy("User", policy => policy.RequireClaim("type", "User"));
//    cfg.AddPolicy("Moderator", policy => policy.RequireClaim("type", "Moderator"));
//});

services.AddSingleton<IAuthorizationPolicyProvider, AuthorizePolicy>();
services.AddTransient<IAuthorizationHandler, AdminAuthorizeHandler>();
services.AddTransient<IAuthorizationHandler, ModeratorAuthorizeHandler>();
services.AddTransient<IAuthorizationHandler, UserAuthorizeHandler>();

// Register services

services.AddScoped<IAdminNavigationService, AdminNavigationService>();
services.AddScoped<IAnimeImageService, AnimeImageService>();
services.AddScoped<IAnimeListStatusService, AnimeListStatusService>();
services.AddScoped<IAnimeService, AnimeService>();
services.AddScoped<IAnimeSourceService, AnimeSourceService>();
services.AddScoped<IAnimeStatusService, AnimeStatusService>();
services.AddScoped<IAnimeTypeService, AnimeTypeService>();
services.AddScoped<ICharacterImageService, CharacterImageService>();
services.AddScoped<ICharacterService, CharacterService>();
services.AddScoped<IGenreService, GenreService>();
services.AddScoped<IJobService, JobService>();
services.AddScoped<ILanguageService, LanguageService>();
services.AddScoped<ILoggerService, LoggerService>();
services.AddScoped<ILogService, LogService>();
services.AddScoped<IProducerImageService, ProducerImageService>();
services.AddScoped<IProducerService, ProducerService>();
services.AddScoped<IRatingService, RatingService>();
services.AddScoped<IRelationTypeService, RelationTypeService>();
services.AddScoped<IReviewScoreService, ReviewScoreService>();
services.AddScoped<IRoleService, RoleService>();
services.AddScoped<ISeasonService, SeasonService>();
services.AddScoped<IStaffImageService, StaffImageService>();
services.AddScoped<IStaffService, StaffService>();
services.AddScoped<IStudioImageService, StudioImageService>();
services.AddScoped<IStudioService, StudioService>();
services.AddScoped<IUserInfoService, UserInfoService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IValidateService, ValidateService>();

// Register repositories

services.AddScoped<IAdminNavRepository, AdminNavigationRepository>();
services.AddScoped<IAnimeImageRepository, AnimeImageRepository>();
services.AddScoped<IAnimeListStatusRepository, AnimeListStatusRepository>();
services.AddScoped<IAnimeRepository, AnimeRepository>();
services.AddScoped<IAnimeSourceRepository, AnimeSourceRepository>();
services.AddScoped<IAnimeStatusRepository, AnimeStatusRepository>();
services.AddScoped<IAnimeTypeRepository, AnimeTypeRepository>();
services.AddScoped<ICharacterImageRepository, CharacterImageRepository>();
services.AddScoped<ICharacterRepository, CharacterRepository>();
services.AddScoped<IGenreRepository, GenreRepository>();
services.AddScoped<IJobRepository, JobRepository>();
services.AddScoped<ILanguageRepository, LanguageRepository>();
services.AddScoped<ILoggerRepository, LoggerRepository>();
services.AddScoped<IProducerImageRepository, ProducerImageRepository>();
services.AddScoped<IProducerRepository, ProducerRepository>();
services.AddScoped<IRatingRepository, RatingRepository>();
services.AddScoped<IRelationTypeRepository, RelationTypeRepository>();
services.AddScoped<IReviewScoreRepository, ReviewScoreRepository>();
services.AddScoped<IRoleRepository, RoleRepository>();
services.AddScoped<ISeasonRepository, SeasonRepository>();
services.AddScoped<IStaffImageRepository, StaffImageRepository>();
services.AddScoped<IStaffRepository, StaffRepository>();
services.AddScoped<IStudioImageRepository, StudioImageRepository>();
services.AddScoped<IStudioRepository, StudioRepository>();
services.AddScoped<IUserInfoRepository, UserInfoRepository>();
services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
