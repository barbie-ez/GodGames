using GodGames.Worker;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSerilog((_, cfg) => cfg.ReadFrom.Configuration(builder.Configuration));
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
