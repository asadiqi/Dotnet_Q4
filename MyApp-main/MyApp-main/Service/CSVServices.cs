using CommunityToolkit.Maui.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Service
{
    public class CSVServices
    {
        public async Task<List<Product>> LoadData()
        {
            List<Product> list = new List<Product>();

            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Sélectionnez un fichier CSV"
                });

                if (result == null)
                {
                    await Application.Current.MainPage.DisplayAlert("⚠️ Alert", "No file selected. Please select a CSV file.", "OK");
                    return list;
                }

                if (!result.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    await Application.Current.MainPage.DisplayAlert("⚠️ Alert", "The selected file is not a CSV file. Please select a CSV file.", "OK");
                    return list;
                }

                var lines = await File.ReadAllLinesAsync(result.FullPath, Encoding.UTF8);

                if (lines.Length == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("⚠️ Alert", "The CSV file is empty. Please select a file with data.", "OK");
                    return list;
                }

                if (!lines[0].Contains(';'))
                {
                    await Application.Current.MainPage.DisplayAlert("❌ Error", "Invalid format: the file must use semicolons (;) as separators.", "OK");
                    return list;
                }

                var headers = lines[0].Split(';');
                var properties = typeof(Product).GetProperties();

                for (int i = 1; i < lines.Length; i++)
                {
                    try
                    {
                        Product obj = new Product();
                        var values = lines[i].Split(';');

                        for (int j = 0; j < headers.Length; j++)
                        {
                            var property = properties.FirstOrDefault(p => p.Name.Equals(headers[j], StringComparison.OrdinalIgnoreCase));

                            if (property != null && j < values.Length)
                            {
                                object value = Convert.ChangeType(values[j], property.PropertyType);
                                property.SetValue(obj, value);
                            }
                        }

                        // Déterminer l'image du produit en fonction du groupe
                        if (obj.Group != null)
                        {
                            var groupLower = obj.Group.ToLower();

                            if (groupLower.Contains("fruit"))
                            {
                                obj.Picture = "fruit.png";
                            }
                            else if (groupLower.Contains("légume") || groupLower.Contains("legume"))
                            {
                                obj.Picture = "vegetable.png";
                            }
                            else
                            {
                                obj.Picture = "other.png";
                            }
                        }

                        list.Add(obj);
                    }
                    catch (Exception ex)
                    {
                        await Application.Current.MainPage.DisplayAlert("❌ Line Error", $"Error processing line {i}: {ex.Message}", "OK");
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("❌ Error", $"Error loading the CSV file: {ex.Message}", "OK");
                return list;
            }
        }

        public async Task PrintData<T>(List<T> data)
        {
            try
            {
                if (data == null || data.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("⚠️ Alert", "No data to export.", "OK");
                    return;
                }

                var csv = new StringBuilder();
                var properties = typeof(T).GetProperties();

                // Ajouter les en-têtes du CSV
                csv.AppendLine(string.Join(";", properties.Select(p => p.Name)));

                // Ajouter les données de chaque élément
                foreach (var item in data)
                {
                    var values = properties.Select(p => p.GetValue(item)?.ToString() ?? "");
                    csv.AppendLine(string.Join(";", values));
                }

                // Enregistrer le fichier CSV
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));
                var fileSaverResult = await FileSaver.Default.SaveAsync("Collection.csv", stream);

                if (!fileSaverResult.IsSuccessful)
                {
                    await Application.Current.MainPage.DisplayAlert("❌ Error", "Failed to save the CSV file.", "OK");
                    return;
                }

                // ✅ Succès
                await Application.Current.MainPage.DisplayAlert("✅ Success", "The CSV file has been successfully exported.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("❌ Error", $"Error saving the CSV file: {ex.Message}", "OK");
            }
        }
    }
}
