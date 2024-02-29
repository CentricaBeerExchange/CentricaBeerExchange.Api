using CentricaBeerExchange.Api.Middleware;
using CentricaBeerExchange.DataAccess;
using CentricaBeerExchange.DataAccess.Handlers;
using CentricaBeerExchange.Services;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using System.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

JwtSettings jwtSettings = ReadConfigSection<JwtSettings>("JwtSettings");
string mysqlConnectionString = builder.Configuration.GetConnectionString("MySql")
    ?? throw new ArgumentException(nameof(mysqlConnectionString));

builder.Services
    .AddSingleton(jwtSettings);

// Add services to the container.
builder.Services
    .AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(jwtSettings.KeyBytes),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorization();

builder.Services
    .AddControllers();
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(opt =>
    {
        opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = JwtBearerDefaults.AuthenticationScheme
        });
        opt.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                },
                Array.Empty<string>()
            }
        });
    });

builder.Services
    .AddScoped<IAuthService, AuthService>()
    .AddScoped<ITokenService, TokenService>()
    .AddScoped<IEmailService, EmailService>()
    .AddScoped<ICodeGenerationService, CodeGenerationService>()
    .AddSingleton<ITimeProvider, CentricaBeerExchange.Services.TimeProvider>();

builder.Services
    .AddTransient<IAuthRepository, AuthRepository>()
    .AddTransient<IProfileRepository, ProfileRepository>()
    .AddTransient<IStylesRepository, StylesRepository>()
    .AddTransient<IBreweriesRepository, BreweriesRepository>()
    .AddTransient<IBeersRepository, BeersRepository>()
    .AddTransient<ITastingsRepository, TastingsRepository>()
    .AddTransient<ITastingParticipantsRepository, TastingParticipantsRepository>()
    .AddTransient<ITastingVotesRepository, TastingVotesRepository>();

SqlMapper.AddTypeHandler(new DateTimeTypeHandler());
SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());

builder.Services
    .AddTransient<IDbConnection>(_ => new MySqlConnection(mysqlConnectionString));

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger()
   .UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AuthValidationMiddleware>();

app.MapControllers();

app.Run();

T ReadConfigSection<T>(string section)
{
    T? model = builder.Configuration.GetSection(section).Get<T>(opt => opt.BindNonPublicProperties = true);
    return model ?? throw new InvalidOperationException($"Could not read section '{section}' as {typeof(T).Name}");
}
