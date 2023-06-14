using GrpcServerCommon;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.UseRouting();

// add useendpoints
app.UseEndpoints(endpoints => endpoints.MapGrpcService<IngestorService>());

app.Run();
