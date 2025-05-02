using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyApp.Service;

public class JSONServices
{
    private const string BaseUrl = "https://185.157.245.38:5000/json";
    private const string FileName = "MyProducts.json";

    internal async Task<List<Product>> GetProducts()
    {
        var url = $"{BaseUrl}?FileName={FileName}";
        List<Product> MyList = new();

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        using HttpClient _httpClient = new(handler);

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStreamAsync();
                MyList = JsonSerializer.Deserialize<List<Product>>(content) ?? new List<Product>();
            }
           
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("❌ Error!", $"Exception occurred while retrieving data: {ex.Message}", "OK");
        }

        return MyList;
    }

    internal async Task<bool> SetProducts()
    {
        var url = BaseUrl;
        MemoryStream mystream = new();

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        using HttpClient _httpClient = new(handler);

        try
        {
            JsonSerializer.Serialize(mystream, Globals.MyProducts, new JsonSerializerOptions { WriteIndented = true });
            mystream.Position = 0;

            var fileContent = new ByteArrayContent(mystream.ToArray());
            var content = new MultipartFormDataContent
            {
                { fileContent, "file", FileName }
            };

            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("❌ Error!", $"Exception occurred while uploading the JSON file: {ex.Message}", "OK");
            return false;
        }
    }

   
}
