using Nasa.ConsoleApp;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;

public class Program
{
    public static async void Main(string[] args)
    {
        await GetNasaPhotos();
    }


    public static async Task GetNasaPhotos()
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri("https://api.nasa.gov/");

            httpClient.DefaultRequestHeaders.Add("ApiKey", "O7ef7LYc6WIqDzc5zwh1VH38l8eZZ6t0Bu1IDsVh");

            HttpResponseMessage response = await httpClient.GetAsync("mars-photos/api/v1/rovers/curiosity/photos?sol=1000&api_key=DEMO_KEY");

            if (response.IsSuccessStatusCode)
            {

                var content = await response.Content.ReadAsStringAsync();
                var photoResponse = JsonConvert.DeserializeObject<NasaResponse>(content);
                if (photoResponse != null && photoResponse.photos.Count > 0)
                {
                    foreach (var image in photoResponse.photos)
                    {
                        string localPath = "C:\\Users\\vamsh\\source\\repos\\Nasa.Web.App\\Nasa.ConsoleApp\\Images";
                        await DownloadImageAsync(image.img_src, localPath);
                    }
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
    }

    public static async Task DownloadImageAsync(string imageUrl, string outputPath)
    {
        using (var httpClient = new HttpClient())
        {
            byte[] imageData = await httpClient.GetByteArrayAsync(imageUrl);
            using (FileStream fs = new FileStream(outputPath, FileMode.Create))
            {
                await fs.WriteAsync(imageData, 0, imageData.Length);
            }
        }
    }
}