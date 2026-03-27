using GodGames.AI;
using GodGames.Application;
using GodGames.Infrastructure;
using GodGames.Worker;
using GodGames.Application.Jobs;
using Hangfire;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSerilog((_, cfg) => cfg.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddAI(builder.Configuration);

builder.Services.AddHangfireServer();
builder.Services.AddScoped<WorldTickJob>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
