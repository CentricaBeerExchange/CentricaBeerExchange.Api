using CentricaBeerExchange.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfiguration config = builder.Configuration;

JwtSettings jwtSettings = ReadConfigSection<JwtSettings>("JwtSettings");
AuthSecrets authSecrets = ReadConfigSection<AuthSecrets>("AuthSecrets");

builder.Services
    .AddSingleton(jwtSettings)
    .AddSingleton(authSecrets);

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
    .AddSwaggerGen();

builder.Services
    .AddScoped<ITokenService, TokenService>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger()
   .UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

T ReadConfigSection<T>(string section)
{
    T? model = config.GetSection(section).Get<T>(opt => opt.BindNonPublicProperties = true);
    return model ?? throw new InvalidOperationException($"Could not read section '{section}' as {typeof(T).Name}");
}
