using Straonit.HighEdge.Core.Configuration;
using Straonit.HighEdge.Core.Distribution;

namespace Straonit.HighEdge.Middlewares;

public class PingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ClusterConfig _clusterConfig;
    private readonly IPingService _pingService;
 
    public PingMiddleware(RequestDelegate next, ClusterConfig clusterConfig,IPingService pingService)
    {
        this._next = next;
        _clusterConfig = clusterConfig;
        _pingService = pingService;
    }
 
    public async Task InvokeAsync(HttpContext context)
    {
        var pingNodesCount = await _pingService.PingNodes(_clusterConfig.Nodes);
        if (pingNodesCount < _clusterConfig.RequiredNodesCount)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Недостаточно работающих нод");
        }
        else
        {
            await _next.Invoke(context);
        }
    }
}