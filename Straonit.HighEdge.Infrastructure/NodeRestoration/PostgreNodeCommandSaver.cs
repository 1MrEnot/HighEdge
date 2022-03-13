namespace Straonit.HighEdge.Infrastructure.NodeRestoration;

using Core.NodeRestoration;
using Core.SplitSecret;
using Npgsql;

public class PostgreNodeCommandSaver : INodeCommandSaver
{
    private bool _dbCreated;
    private readonly NpgsqlConnection _npgsql;
    private const string InsertCreate = "INSERT INTO saved_commands VALUES ($1, $2, $3, $4, $5, $6)";

    public PostgreNodeCommandSaver(NpgsqlConnection npgsql)
    {
        _npgsql = npgsql;
        _npgsql.Open();
    }

    public async Task WriteCreateCommand(PartOfSecret partOfSecret, string nodeUrl)
    {
        await EnsureCreated();

        await using var createCmd = new NpgsqlCommand(InsertCreate, _npgsql);
        createCmd.Parameters.Add(new NpgsqlParameter())

        await createCmd.ExecuteNonQueryAsync();
    }

    public async Task WriteUpdateCommand(PartOfSecret partOfSecret, string nodeUrl)
    {
        await EnsureCreated();
    }

    public async Task WriteDeleteCommand(string key, string nodeUrl)
    {
        await EnsureCreated();
    }

    private Task EnsureCreated()
    {
        if(_dbCreated)
            return Task.CompletedTask;

        return CreateTables();
    }

    private Task CreateTables()
    {

        _dbCreated = true;
        return Task.CompletedTask;
    }
}