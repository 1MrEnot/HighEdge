using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Straonit.HighEdge.Core;
using Straonit.HighEdge.Core.Configuration;
using Straonit.HighEdge.Core.Distribution;
using Straonit.HighEdge.Core.Persistence;
using Straonit.HighEdge.Infrastructure;
using Straonit.HighEdge.Infrastructure.Grpc;
using Straonit.HighEdge.Infrastructure.Service;
using Straonit.HighEdge.Ioc;
using Straonit.HighEdge.Middlewares;
using Straonit.HighEdge.Services;
using Straonit.HighEdge.Services.Implementations;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var config = builder.Configuration;

builder.Services.AddRedisDatabase(config);
builder.Services.AddShamirServices();

//builder.Services.AddTransient(sp => sp.GetRequiredService<StatusChecker>());
builder.Services.AddTransient<StatusChecker>();
builder.Services.AddTransient<ISecretService, SecretService>();
builder.Services.AddTransient<IRollBack, RollBackService>();
// builder.Services.AddTransient<IPingService, PingService>();

// builder.Services.AddHostedService<LongRunningService>();
// builder.Services.AddSingleton<BackgroundWorkerQueue>();


var clusterConfigJson = File.ReadAllText(Environment.GetEnvironmentVariable("CLUSTER_CONFIG"));
System.Console.WriteLine(clusterConfigJson);
var clusterConfig = JsonSerializer.Deserialize<ClusterConfig>(clusterConfigJson);
System.Console.WriteLine(clusterConfig.Nodes.Count);

builder.Services.AddSingleton<ClusterConfig>(clusterConfig);
builder.Services.AddSingleton<RollBackConfig>();
builder.Services.AddTransient<DistributedSecretSerivce>();

builder.Services.AddHttpClient();
builder.Services.AddGrpc();

builder.Services.AddSingleton(sp => sp.GetRequiredService<TaskService>());

// builder.Services.AddControllersWithViews();

AppContext.SetSwitch(
    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);



builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 80, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });

    options.Listen(IPAddress.Any, 82, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseMiddleware<PingMiddleware>();

app.UseAuthorization();
app.MapGrpcService<GrpcServer>();

app.MapControllers();

app.Run();

