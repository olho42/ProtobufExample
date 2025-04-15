using TaskApp.Server.Models;
using TaskApp.Server.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
builder.Services.AddSingleton<PBTaskStore>();
builder.Services.AddSingleton<PBTaskServiceImplementation>();
var app = builder.Build();
app.MapGrpcService<PBTaskServiceImplementation>();
app.MapGet("/", () => "gRPC service running.");
app.Run();