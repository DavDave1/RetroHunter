using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace RetroHunter.Utils
{
    // From https://docs.avaloniaui.net/docs/guides/data-binding/how-to-bind-image-files
    public static class ImageHelper
    {
        public static Bitmap LoadFromResource(Uri resourceUri)
        {
            return new Bitmap(AssetLoader.Open(resourceUri));
        }
        public static async Task<Bitmap?> LoadFromWeb(Uri url, CancellationToken ct = default)
        {
            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetAsync(url, ct);
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsByteArrayAsync(ct);
                return new Bitmap(new MemoryStream(data));
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"An error occurred while downloading image '{url}' : {ex.Message}");
                return null;
            }
        }
    }
}
