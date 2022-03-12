namespace Straonit.HighEdge.Extensions;

using System.Text.Json;

public static class HttpResponseMessageExtension
{
    public static async Task<T> GetObjectAsync<T>(this HttpResponseMessage message)
    {
        using var contentStream =
              await message.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync<T>(contentStream);
    }
}