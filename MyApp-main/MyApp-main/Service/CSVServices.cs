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
                // Afficher la boîte de dialogue pour choisir un fichier CSV
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Sélectionnez un fichier CSV"
                });

                // Si aucun fichier n'est sélectionné
                if (result == null)
                {
                    await Application.Current.MainPage.DisplayAlert("⚠️ Alert", "No file selected. Please select a CSV file.", "OK");
                    return list;
                }

                // Vérifier si le fichier sélectionné est bien un CSV
                if (!result.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    await Application.Current.MainPage.DisplayAlert("⚠️ Alert", "The selected file is not a CSV file. Please select a CSV file.", "OK");
                    return list;
                }

                // Lire les lignes du fichier CSV
                var lines = await File.ReadAllLinesAsync(result.FullPath, Encoding.UTF8);

                // Vérifier si le fichier est vide
                if (lines.Length == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("⚠️ Alert", "The CSV file is empty. Please select a file with data.", "OK");
                    return list;
                }
                // Vérifier que le séparateur est bien un semicolons
                if (!lines[0].Contains(';'))
                {
                    await Application.Current.MainPage.DisplayAlert("❌ Error", "Invalid format: the file must use semicolons (;) as separators.", "OK");
                    return list;
                }

                var headers = lines[0].Split(';');
                var properties = typeof(Product).GetProperties();

                // Traiter chaque ligne du CSV (sauf la première qui contient les en-têtes)
                for (int i = 1; i < lines.Length; i++)
                {
                    try
                    {
                        Product obj = new Product();
                        var values = lines[i].Split(';');

                        // Associer chaque valeur aux propriétés correspondantes
                        for (int j = 0; j < headers.Length; j++)
                        {
                            try
                            {
                                var property = properties.FirstOrDefault(p => p.Name.Equals(headers[j], StringComparison.OrdinalIgnoreCase));

                                if (property != null && j < values.Length)
                                {
                                    // Essayer de convertir et affecter la valeur
                                    object value = Convert.ChangeType(values[j], property.PropertyType);
                                    property.SetValue(obj, value);
                                }
                            }
                            catch (Exception ex)
                            {
                                // Si une erreur se produit pour une propriété, on affiche un message d'erreur pour cette propriété
                                Console.WriteLine($"Erreur de conversion pour la propriété {headers[j]} : {ex.Message}");
                                await Application.Current.MainPage.DisplayAlert("❌ Conversion Error", $"Unable to convert the value for the property {headers[j]}.", "OK");
                            }
                        }

                        list.Add(obj); // Ajouter l'objet produit à la liste
                    }
                    catch (Exception ex)
                    {
                        // Si une erreur se produit pour une ligne, on l'affiche mais on continue avec les autres lignes
                        Console.WriteLine($"Erreur lors du traitement de la ligne {i}: {ex.Message}");
                        await Application.Current.MainPage.DisplayAlert("❌ Line Error", $"Error processing line {i}: {ex.Message}", "OK");
                    }
                }

                // Affichage d'un popup de succès lorsque le chargement des produits est réussi
                await Application.Current.MainPage.DisplayAlert("✅ Success", "The products have been successfully loaded from the CSV file.", "OK");

                return list;
            }
            catch (Exception ex)
            {
                // Si une erreur se produit lors de la lecture du fichier ou de l'interaction avec l'utilisateur
                await Application.Current.MainPage.DisplayAlert("❌ Error", $"Error loading the CSV file: {ex.Message}", "OK");
                return list; // Retourner une liste vide en cas d'erreur
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
