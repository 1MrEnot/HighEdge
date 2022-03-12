namespace Straonit.HighEdge.Infrastructure;

public class RedisConfig
{
    public string Host { get; set; }

    public string Port { get; set; }

    public string ConnectionString => $"{Host}:{Port}";
}