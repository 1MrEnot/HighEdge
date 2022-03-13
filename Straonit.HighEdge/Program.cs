using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Npgsql;
using Straonit.HighEdge.Core;
using Straonit.HighEdge.Core.Configuration;
using Straonit.HighEdge.Core.Distribution;
using Straonit.HighEdge.Core.NodeRestoration;
using Straonit.HighEdge.Infrastructure.Grpc;
using Straonit.HighEdge.Infrastructure.NodeRestoration;
using Straonit.HighEdge.Infrastructure.Service;
using Straonit.HighEdge.Ioc;
using Straonit.HighEdge.Services;
using Straonit.HighEdge.Services.Implementations;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var config = builder.Configuration;

builder.Services.AddLogging(logging =>
{
    logging.AddSimpleConsole(options => options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ");
});
builder.Services.AddRedisDatabase(config);
builder.Services.AddShamirServices();

//builder.Services.AddTransient(sp => sp.GetRequiredService<StatusChecker>());
builder.Services.AddTransient<StatusChecker>();
builder.Services.AddTransient<ISecretService, SecretService>();
builder.Services.AddTransient<IRollBack, RollBackService>();
builder.Services.AddSingleton<INodeCommandSaver, PostgreNodeCommandSaver>();

var conn = new NpgsqlConnection(Environment.GetEnvironmentVariable("POSTGRES_CONNECTION"));
builder.Services.AddSingleton(conn);

// builder.Services.AddHostedService<LongRunningService>();
// builder.Services.AddSingleton<BackgroundWorkerQueue>();


var clusterConfigJson = File.ReadAllText(Environment.GetEnvironmentVariable("CLUSTER_CONFIG"));
Console.WriteLine(clusterConfigJson);
var clusterConfig = JsonSerializer.Deserialize<ClusterConfig>(clusterConfigJson);
Console.WriteLine(clusterConfig.Nodes.Count);


builder.Services.AddSingleton<ClusterConfig>(clusterConfig);
builder.Services.AddSingleton<RollBackConfig>();
builder.Services.AddTransient<DistributedSecretSerivce>();

builder.Services.AddHttpClient();
builder.Services.AddGrpc();

builder.Services.AddSingleton<TaskService>();

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

