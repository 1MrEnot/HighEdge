namespace Straonit.HighEdge.Infrastructure.NodeRestoration;

using Core.NodeRestoration;
using Core.SplitSecret;
using Microsoft.Extensions.Logging;
using Npgsql;

public class PostgreNodeCommandSaver : INodeCommandSaver
{
    private bool _dbCreated;
    private readonly NpgsqlConnection _npgsql;
    private readonly ILogger<PostgreNodeCommandSaver> _logger;

    private const string InsertCreate = "INSERT INTO saved_commands VALUES (@timestamp, @hostname, @key, 1, @x, @y)";
    private const string InsertDelete = "INSERT INTO saved_commands (timestamp, hostname, key, command_type) VALUES (@timestamp, @hostname, @key, 2)";
    private const string CreateTable = @"create table if not exists saved_commands
    (
        timestamp    timestamp not null,
        hostname     varchar   not null,
        key          varchar   not null,
        command_type integer   not null,
        x            bytea,
        y            bytea
    );
    alter table saved_commands
        owner to postgres;";

    public PostgreNodeCommandSaver(NpgsqlConnection npgsql, ILogger<PostgreNodeCommandSaver> logger)
    {
        _npgsql = npgsql;
        _logger = logger;
        _npgsql.Open();
        CreateTables().GetAwaiter().GetResult();
    }

    public async Task WriteCreateCommand(string key, PartOfSecret partOfSecret, string nodeUrl)
    {
        await EnsureCreated();

        await using var createCmd = new NpgsqlCommand(InsertCreate, _npgsql);
        createCmd.Parameters.Add(new NpgsqlParameter("timestamp", DateTime.UtcNow));
        createCmd.Parameters.Add(new NpgsqlParameter("hostname", nodeUrl));
        createCmd.Parameters.Add(new NpgsqlParameter("key", key));
        createCmd.Parameters.Add(new NpgsqlParameter("x", partOfSecret.X.ToByteArray()));
        createCmd.Parameters.Add(new NpgsqlParameter("y", partOfSecret.Y.ToByteArray()));
        await createCmd.ExecuteNonQueryAsync();
    }

    public async Task WriteDeleteCommand(string key, string nodeUrl)
    {
        await EnsureCreated();

        await using var createCmd = new NpgsqlCommand(InsertDelete, _npgsql);
        createCmd.Parameters.Add(new NpgsqlParameter("timestamp", DateTime.UtcNow));
        createCmd.Parameters.Add(new NpgsqlParameter("hostname", nodeUrl));
        createCmd.Parameters.Add(new NpgsqlParameter("key", key));

        await createCmd.ExecuteNonQueryAsync();
    }

    private Task EnsureCreated()
    {
        if(_dbCreated)
            return Task.CompletedTask;

        return CreateTables();
    }

    private async Task CreateTables()
    {
        try
        {
            await using var createTableCmd = new NpgsqlCommand(CreateTable, _npgsql);
            createTableCmd.ExecuteNonQuery();
            _dbCreated = true;
            _logger.LogInformation("saved_commands table created");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to create saved_commands table");
        }
    }
}