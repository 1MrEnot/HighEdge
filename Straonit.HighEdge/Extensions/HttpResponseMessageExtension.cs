using System.Text.Json;

namespace hackation_high_edge.Extensions;

public static class HttpResponseMessageExtension
{
    public static async Task<T> GetObjectAsync<T>(this HttpResponseMessage message)
    {
        using var contentStream =
              await message.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync<T>(contentStream);
    }
}