using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Handlers;
using Questao5.Application.Queries.Requests;
using Questao5.Infrastructure.Database.CommandStore;
using Questao5.Infrastructure.Database.QueryStore;
using Questao5.Infrastructure.Sqlite;
using System.Reflection;
using Questao5.Infrastructure.Database.CommandStore.Requests;
using Questao5.Infrastructure.Database.QueryStore.Requests;
using SQLitePCL;

Batteries_V2.Init();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configuração do SQLite
builder.Services.AddSingleton(new DatabaseConfig { 
    Name = "Data Source=database.sqlite;Cache=Shared"
});
builder.Services.AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>();

//Mediatr
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddTransient<IRequestHandler<MovimentoRequest, object>, MovimentoHandler>();
builder.Services.AddTransient<IRequestHandler<SaldoRequest, object>, SaldoHandler>();

// Command Store e Query Store
builder.Services.AddScoped<IMovimentoCommandStore, MovimentoCommandStore>();
builder.Services.AddScoped<ISaldoQueryStore, SaldoQueryStore>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Inicialização do banco de dados SQLite
using (var scope = app.Services.CreateScope())
{
    var databaseBootstrap = scope.ServiceProvider.GetRequiredService<IDatabaseBootstrap>();
    databaseBootstrap.Setup();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Informações úteis:
// Tipos do Sqlite - https://www.sqlite.org/datatype3.html