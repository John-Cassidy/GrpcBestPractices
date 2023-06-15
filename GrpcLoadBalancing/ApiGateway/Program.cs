using ApiGateway;
using Grpc.Net.Client.Balancer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IGrpcClientWrapper, GrpcClientWrapper>();

var addresses = builder.Configuration.GetSection("ServerAddresses").Get<List<string>>();

// add the DNS resolver StaticResolverFactory
//builder.Services.AddSingleton<ResolverFactory>(new StaticResolverFactory(addr => addresses.Select(a => new DnsEndPoint(a.Replace("//", string.Empty).Split(':')[1], int.Parse(a.Split(':')[2]))).ToArray()));
builder.Services.AddSingleton<ResolverFactory>(new StaticResolverFactory(addr => addresses.Select(a => new BalancerAddress(a.Replace("//", string.Empty).Split(':')[1], int.Parse(a.Split(':')[2]))).ToArray()));

var app = builder.Build();

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
