using System.Text.Json;

namespace hackation_high_edge.Extensions;

public static class HttpResponseMessageExtension
{
    public static async Task<T> GetObjectAsync<T>(this HttpResponseMessage message)
    {
        return await message.Content.ReadFromJsonAsync<T>();
    }
}