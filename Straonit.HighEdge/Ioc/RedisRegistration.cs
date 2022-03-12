namespace Straonit.HighEdge.Ioc;

using Core.Persistence;
using Infrastructure;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

public static class RedisRegistration
{
    public static IServiceCollection AddRedisDatabase(this IServiceCollection serviceCollection,
        ConfigurationManager config)
    {
        serviceCollection.Configure<RedisConfig>(config.GetSection("REDIS"));

        serviceCollection.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var config = sp.GetRequiredService<IOptions<RedisConfig>>();
            return ConnectionMultiplexer.Connect(
                new ConfigurationOptions
                {
                    EndPoints = { config.Value.ConnectionString },
                    AbortOnConnectFail = false,
                    Password = config.Value.Pass
                });
        });

        serviceCollection.AddTransient(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
        serviceCollection.AddTransient<IDbContext, RedisDbContext>();

        return serviceCollection;
    }
}