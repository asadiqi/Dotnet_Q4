using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Input;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FilePicker = Microsoft.Maui.Storage.FilePicker;

namespace MyApp.Service;

public partial  class JSONServices
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


    public async Task<List<Product>> LoadFromLocalJsonAsync()
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select a JSON file",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".json" } },
                { DevicePlatform.macOS, new[] { ".json" } },
                { DevicePlatform.iOS, new[] { "public.json" } },
                { DevicePlatform.Android, new[] { "application/json" } }
            })
            });

            if (result != null)
            {
                using var stream = await result.OpenReadAsync();
                var products = await JsonSerializer.DeserializeAsync<List<Product>>(stream);
                products ??= new List<Product>();

                // 🔁 Appliquer une image par défaut si Picture est vide
                foreach (var product in products)
                {
                    if (string.IsNullOrEmpty(product.Picture))
                    {
                        var group = product.Group?.ToLower();
                        if (group != null)
                        {
                            if (group.Contains("fruit"))
                                product.Picture = "fruit.png";
                            else if (group.Contains("légume") || group.Contains("legume"))
                                product.Picture = "vegetable.png";
                            else
                                product.Picture = "other.png";
                        }
                        else
                        {
                            product.Picture = "other.png";
                        }
                    }
                }

                return products;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("❌ Error", $"Failed to load JSON: {ex.Message}", "OK");
        }

        return new List<Product>();


    }


    [RelayCommand]
    public async Task PrintToJSON(List<Product> products)
    {
        try
        {
            if (products == null || products.Count == 0)
            {
                await Shell.Current.DisplayAlert("⚠️ Alert", "No data to export.", "OK");
                return;
            }

            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(products, jsonOptions);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);

            using var stream = new MemoryStream(jsonBytes);

            var fileSaverResult = await FileSaver.Default.SaveAsync("Products.json", stream);

            if (!fileSaverResult.IsSuccessful)
            {
                await Shell.Current.DisplayAlert("❌ Error", "Failed to save the JSON file.", "OK");
                return;
            }

            await Shell.Current.DisplayAlert("✅ Success", "The JSON file has been successfully exported.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("❌ Error", $"Error saving the JSON file: {ex.Message}", "OK");
        }
    }

}
