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
                Console.WriteLine("✅ Données récupérées du serveur : " + MyList.Count + " produits.");
            }
            else
            {
                Console.WriteLine($"❌ Erreur lors de la récupération des données : {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception lors de la récupération des données : {ex.Message}");
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
                Console.WriteLine("✅ Fichier JSON envoyé avec succès !");
                return true;
            }
            else
            {
                Console.WriteLine("❌ Erreur lors de l'envoi du fichier JSON : " + response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Exception lors de l'envoi du fichier JSON : " + ex.Message);
            return false;
        }
    }

    // le données sont accessible via https://185.157.245.38:5000/json?FileName=MyProducts.json

}
