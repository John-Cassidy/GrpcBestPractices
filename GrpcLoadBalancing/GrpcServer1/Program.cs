using GrpcServerCommon;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.UseRouting();

app.MapGrpcService<IngestorService>();

app.Run();
