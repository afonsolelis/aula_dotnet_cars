using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services.Auth;
using Volkswagen.Dashboard.Services.Cars;
using Volkswagen.Dashboard.Services.CQRS.Handlers;
using Volkswagen.Dashboard.WebApi.GraphQL;
using Volkswagen.Dashboard.WebApi.Grpc;

// gRPC requer HTTP/2; este switch permite HTTP/2 sem TLS em desenvolvimento
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var key = Encoding.ASCII.GetBytes("d8cf9a98-bfb2-4e0a-85b3-7c94f8e908ad");

var builder = WebApplication.CreateBuilder(args);

// ── MongoDB ──────────────────────────────────────────────────────────────────
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection(MongoDbSettings.SectionName));

var mongoSettings = builder.Configuration
    .GetSection(MongoDbSettings.SectionName)
    .Get<MongoDbSettings>() ?? new MongoDbSettings();

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoSettings.ConnectionString));
builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoSettings.DatabaseName);
});

// ── Serviços de domínio ───────────────────────────────────────────────────────
builder.Services.AddScoped<ICarsRepository, CarsRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICarsService, CarsService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IMongoSchemaInitializer, MongoSchemaInitializer>();

// ── MediatR (CQRS) ────────────────────────────────────────────────────────────
// Varre o assembly de Services para registrar todos os IRequestHandler<,>
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetCarsQueryHandler).Assembly));

// ── gRPC ──────────────────────────────────────────────────────────────────────
builder.Services.AddGrpc();

// ── GraphQL (HotChocolate) ────────────────────────────────────────────────────
builder.Services
    .AddGraphQLServer()
    .AddQueryType<CarQuery>()
    .AddMutationType<CarMutation>()
    .AddType<CarType>();

// ── OpenTelemetry + Prometheus ────────────────────────────────────────────────
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("volkswagen-dashboard-api"))
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation())
    .WithMetrics(m => m
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddPrometheusExporter());

// ── REST ──────────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── JWT ───────────────────────────────────────────────────────────────────────
IdentityModelEventSource.ShowPII = true;

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var schemaInitializer = scope.ServiceProvider.GetRequiredService<IMongoSchemaInitializer>();
    await schemaInitializer.InitializeAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// ── Endpoints ─────────────────────────────────────────────────────────────────
app.MapPrometheusScrapingEndpoint();        // GET /metrics  (Prometheus scrape)
app.MapControllers();                       // REST  → /api/car, /api/user
app.MapGrpcService<CarGrpcService>();       // gRPC  → /car.CarService/*
app.MapGraphQL();                           // GraphQL → /graphql  (+ Banana Cake Pop IDE)

app.Run();

// Expõe Program para WebApplicationFactory nos testes de integração
public partial class Program { }
