using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Suframa.RecepcaoRD.API.Autenticacao;
using Suframa.RecepcaoRD.API.Extensions;
using Suframa.RecepcaoRD.Application.Configuracao;
using Suframa.RecepcaoRD.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AutenticacaoAmbienteOpcoes>(
  builder.Configuration.GetSection(AutenticacaoAmbienteOpcoes.NomeSecao));

builder.Services.AddCorsFromConfiguration(builder.Configuration);

string? recepcaoDb = builder.Configuration.GetConnectionString("RecepcaoDB");
if (string.IsNullOrWhiteSpace(recepcaoDb))
{
  throw new InvalidOperationException(
    "ConnectionStrings:RecepcaoDB não está definida ou está vazia. " +
    "Defina em appsettings.json, appsettings.{Ambiente}.json ou variáveis de ambiente. " +
    "Em desenvolvimento local, use ASPNETCORE_ENVIRONMENT=Development para carregar appsettings.Development.json.");
}

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddAutenticacaoInfraestrutura();
builder.Services.AddApplicationServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ServicoRespostaCookieAutenticacao>();

builder.Services.AddDbContext<RecepcaoRDDbContext>(options =>
{
    options
        .UseOracle(recepcaoDb)
        .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(Suframa.RecepcaoRD.API.Extensions.CorsServiceCollectionExtensions.DefaultCorsPolicyName);

app.UseAuthorization();

app.MapControllers();

app.Run();
