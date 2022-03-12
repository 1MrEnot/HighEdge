using System.Text.Json;
using Straonit.HighEdge.Core;
using Straonit.HighEdge.Core.Configuration;
using Straonit.HighEdge.Core.Distribution;
using Straonit.HighEdge.Infrastructure.Grpc;
using Straonit.HighEdge.Infrastructure.Service;
using Straonit.HighEdge.Ioc;
using Straonit.HighEdge.Services.Implementations;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// var connection = ConnectionMultiplexer.Connect("localhost:6379");

// builder.Services.AddOpenTelemetryTracing(builder =>
// {
//     builder.AddAspNetCoreInstrumentation()                
//         .AddRedisInstrumentation(connection);

// }).AddConsoleExporter();

var config = builder.Configuration;

builder.Services.AddRedisDatabase(config);
builder.Services.AddShamirServices();

//builder.Services.AddTransient(sp => sp.GetRequiredService<StatusChecker>());
builder.Services.AddTransient<StatusChecker>();
builder.Services.AddTransient<ISecretService, SecretService>();

var clusterConfigJson = File.ReadAllText(@"C:\Users\Max\RiderProjects\HighEdge\Straonit.HighEdge\config.json");
var clusterConfig = JsonSerializer.Deserialize<ClusterConfig>(clusterConfigJson);
builder.Services.AddSingleton<ClusterConfig>(clusterConfig);
builder.Services.AddTransient<DistributedSecretSerivce>();

builder.Services.AddHttpClient();
builder.Services.AddGrpc();

AppContext.SetSwitch(
    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapGrpcService<GrpcServer>();

app.MapControllers();

app.Run();

