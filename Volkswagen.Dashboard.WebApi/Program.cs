using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services.Auth;
using Volkswagen.Dashboard.Services.Cars;

var key = Encoding.ASCII.GetBytes("d8cf9a98-bfb2-4e0a-85b3-7c94f8e908ad");

var builder = WebApplication.CreateBuilder(args);
var configBuilder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

IConfigurationRoot configuration = configBuilder.Build();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICarsService, CarsService>();
builder.Services.AddScoped<ICarsRepository>(_ => new CarsRepository(configuration["DB_CONFIG"]));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository>(_ => new UserRepository(configuration["DB_CONFIG"]));

IdentityModelEventSource.ShowPII = true;

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = "https://dev-k6lyhltkodwyu0y4.us.auth0.com/";
    options.Audience = "dotne_cars";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "https://dev-k6lyhltkodwyu0y4.us.auth0.com/",
        ValidAudience = "dotne_cars"
    };
});

builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
           
app.UseCors("AllowAll"); 

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
