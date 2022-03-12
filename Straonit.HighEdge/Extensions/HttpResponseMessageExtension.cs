namespace Straonit.HighEdge.Extensions;

using System.Text.Json;

public static class HttpResponseMessageExtension
{
    public static async Task<T> GetObjectAsync<T>(this HttpResponseMessage message)
    {
        return await message.Content.ReadFromJsonAsync<T>();
    }
}