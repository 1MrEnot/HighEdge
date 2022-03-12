using System.Text.Json;
using hackation_high_edge;
using hackation_high_edge.Models;
using hackation_high_edge.Service;
using OpenTelemetry;
using OpenTelemetry.Instrumentation.StackExchangeRedis;
using OpenTelemetry.Trace;
using StackExchange.Redis;

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

builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(new ConfigurationOptions
{
    EndPoints = { $"{config.GetValue<string>("Redis:Host")}:{config.GetValue<int>("Redis:Port")}" },
    Ssl = true,
    AbortOnConnectFail = false,
}));

builder.Services.AddTransient(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
//builder.Services.AddTransient(sp => sp.GetRequiredService<StatusChecker>());
builder.Services.AddTransient<StatusChecker>();

Environment.SetEnvironmentVariable("ClusterConfig", "/home/v-user/Desktop/HighEdge/Straonit.HighEdge/config.json");
var clusterConfigJson = File.ReadAllText(Environment.GetEnvironmentVariable("ClusterConfig"));
//System.Console.WriteLine("Cluster config JSON: "+clusterConfigJson);
var clusterConfig = JsonSerializer.Deserialize<ClusterConfig>(clusterConfigJson);
//System.Console.WriteLine(clusterConfig.NodesCount);
builder.Services.AddSingleton<ClusterConfig>(clusterConfig);

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
