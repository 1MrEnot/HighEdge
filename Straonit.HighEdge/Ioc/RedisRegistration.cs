namespace Straonit.HighEdge.Ioc;

using Core.Persistence;
using Infrastructure;
using StackExchange.Redis;

public static class RedisRegistration
{
    public static IServiceCollection AddRedisDatabase(this IServiceCollection serviceCollection,
        ConfigurationManager config)
    {
        var redisConntectionString = $"{config.GetValue<string>("Redis:Host")}:{config.GetValue<int>("Redis:Port")}";
        serviceCollection.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConntectionString));

        serviceCollection.AddTransient(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
        serviceCollection.AddTransient<IDbContext, RedisDbContext>();

        return serviceCollection;
    }
}