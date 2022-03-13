using Straonit.HighEdge.Core.Exceptions;

namespace Straonit.HighEdge.Infrastructure;

using Core.Distribution;
using Core.Persistence;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Protobuf;

public class RedisDbContext : IDbContext
{
    private readonly IDatabase _database;
    private readonly ProtobufSerializer _serializer;

    public RedisDbContext(IDatabase database)
    {
        _database = database;
        _serializer = new ProtobufSerializer();
    }
    
    public async Task<GetSecretPartResponse> GetSecretPart(GetSecretRequest request)
    {
        if (!_database.KeyExists(request.Id))
        {
            return null;
            throw new KeyNotExistsException($"Ключ {request.Id} не найден");
        }
        if(string.IsNullOrEmpty(_database.StringGet(request.Id)))
        System.Console.WriteLine("null");

        var secret = await _database.StringGetAsync(request.Id);
        var pointModel = _serializer.Deserialize<RedisPointModel>(secret);
        return new GetSecretPartResponse(request.Id, pointModel.X, pointModel.Y);
    }

    public async Task<bool> CreateSecretPart(CreateSecretRequest request)
    {
        var pointModel = new RedisPointModel(request.X, request.Y);
        var bytes = _serializer.Serialize(pointModel);
        return await _database.StringSetAsync(request.Id, bytes);
    }

    public async Task DeleteSecretPart(DeleteSecretRequest request)
    {
        await _database.KeyDeleteAsync(request.Id);
    }

    public async Task<bool> UpdateSecretPart(UpdateSecretRequest request)
    {
        await DeleteSecretPart(new DeleteSecretRequest
        {
            Id = request.Id
        });

        return await CreateSecretPart(new CreateSecretRequest
        {
            Id = request.Id,
            X = request.X,
            Y = request.Y
        });
    }
}