using Google.Protobuf;
using Grpc.Core;
using Secrets.Lib;
using Straonit.HighEdge.Core.Distribution;
using Straonit.HighEdge.Core.Persistence;
using Straonit.HighEdge.Infrastructure.Service;
using GetSecretResponse = Secrets.Lib.GetSecretResponse;
using Response = Secrets.Lib.Response;

namespace Straonit.HighEdge.Infrastructure.Grpc;

public class GrpcServer : SecretsService.SecretsServiceBase
{
    private readonly IDbContext _context;

    public GrpcServer(IDbContext context) => (_context) = (context);

    public override async Task<Response> CreateSecret(CreateSecretMessage request, ServerCallContext context)
    {
        try
        {
            var response = await _context.CreateSecretPart(new CreateSecretRequest()
            {
                Id = request.Id,
                X = request.X.ToByteArray(),
                Y = request.Y.ToByteArray()
            });

            return new Response()
            {
                IsSuccess = response
            };
        }
        catch
        {
            return new Response()
            {
                IsSuccess = false
            };
        }
    }

    public override async Task<Response> DeleteSecret(DeleteSecretMessage request, ServerCallContext context)
    {
        try
        {
            await _context.DeleteSecretPart(new DeleteSecretRequest()
            {
                Id = request.Id,
            });

            return new Response
            {
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            return new Response
            {
                IsSuccess = false
            };
        }
    }

    public override async Task<GetSecretResponse> GetSecret(GetSecretMessage request, ServerCallContext context)
    {
        try
        {
            var response = await _context.GetSecretPart(new GetSecretRequest()
            {
                Id = request.Id,
            });

            return new GetSecretResponse()
            {
                IsFound = true,
                X = ByteString.CopyFrom(response.X),
                Y = ByteString.CopyFrom(response.Y),
            };
        }
        catch
        {
            return new GetSecretResponse()
            {
                IsFound = false
            };
        }
    }

    public override async Task<Response> PutSecret(PutSecretMessage request, ServerCallContext context)
    {
        var response = await _context.UpdateSecretPart(new UpdateSecretRequest()
        {
            Id = request.Id,
            X = request.X.ToByteArray(),
            Y = request.Y.ToByteArray()
        });

        return new Response()
        {
            IsSuccess = response
        };
    }
}