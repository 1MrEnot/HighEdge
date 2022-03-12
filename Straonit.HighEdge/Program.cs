using System;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Straonit.HighEdge.Ioc;
using Straonit.HighEdge.Models;
using Straonit.HighEdge.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var config = builder.Configuration;

builder.Services.AddRedisDatabase(config);
builder.Services.AddShamirServices();

//builder.Services.AddTransient(sp => sp.GetRequiredService<StatusChecker>());
builder.Services.AddTransient<StatusChecker>();

// Environment.SetEnvironmentVariable("CLUSTER_CONFIG", "/home/v-user/Desktop/HighEdge/Straonit.HighEdge/config.json");
var clusterConfigJson = File.ReadAllText(Environment.GetEnvironmentVariable("CLUSTER_CONFIG"));
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

