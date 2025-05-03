
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProjectManagementSystem.Middleware;
using ProjectManagementSystem.Repositories;
using ProjectManagementSystem.Security;
using ProjectManagementSystem.Services;
using AuthenticationService = ProjectManagementSystem.Services.AuthenticationService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "ccl.lk",
            ValidAudience = "ccl.lk",
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromHexString(SecurityConstants.PrivateKey))
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<UserGroupRepository>();
builder.Services.AddScoped<ProjectRepository>();
builder.Services.AddScoped<TaskRepository>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<ProjectsService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<AuthenticationMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
