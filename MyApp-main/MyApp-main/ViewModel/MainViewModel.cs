using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyApp.ViewModel;

public partial class MainViewModel : BaseViewModel
{
    public ObservableCollection<Product> MyObservableList { get; } = [];
    JSONServices MyJSONService;
    CSVServices MyCSVServices;

    public MainViewModel(JSONServices MyJSONService, CSVServices MyCSVServices)
    {
        this.MyJSONService = MyJSONService;
        this.MyCSVServices = MyCSVServices;

        // Charger les produits dès l'ouverture de l'application
        _ = LoadProductsOnStartup();
    }

    private async Task LoadProductsOnStartup()
    {
        IsBusy = true;

        // Charger les produits depuis le serveur
        Globals.MyProducts = await MyJSONService.GetProducts();

        // Mettre à jour la liste affichée
        await RefreshPage();

        IsBusy = false;
    }
    [RelayCommand]
    internal async Task GoToDetails(string id)
    {
        IsBusy = true;

        if (Globals.CurrentUser?.Role?.ToLower() != "admin")
        {
            await Application.Current.MainPage.DisplayAlert("🚫 Access Denied", "This section is reserved for admins only.", "OK");
            IsBusy = false;
            return;
        }

        await Shell.Current.GoToAsync("DetailsView", true, new Dictionary<string, object>
    {
        { "selectedAnimal", id }
    });

        IsBusy = false;
    }


    [RelayCommand]
    async Task GoToAllProducts()
    {
        // Vérifier si la liste des produits est vide
        if (Globals.MyProducts == null || Globals.MyProducts.Count == 0)
        {
            // Afficher un popup informant l'utilisateur qu'il n'y a pas de produits
            await Application.Current.MainPage.DisplayAlert("⚠️ Alert", "There are no products available.", "OK");
        }
        else
        {
            // Continuer avec l'action prévue (par exemple, naviguer vers la page des produits)
            await Shell.Current.GoToAsync(nameof(AllProductsView));
        }
    }



    [RelayCommand]
    internal async Task GoToGraph()
    {
        IsBusy = true;

        // Vérifier si la liste de produits est vide
        if (Globals.MyProducts == null || Globals.MyProducts.Count == 0)
        {
            // Afficher un pop-up si la liste est vide
            await Application.Current.MainPage.DisplayAlert("⚠️ Alert", "Product list is empty, there is no graphe available.", "OK");
        }
        else
        {
            // Naviguer vers la page GraphView si des produits existent
            await Shell.Current.GoToAsync("GraphView", true);
        }
        IsBusy = false;
    }
    [RelayCommand]
    internal async Task PrintToCSV()
    {
        IsBusy = true;

        await MyCSVServices.PrintData(Globals.MyProducts);

        IsBusy = false;
    }


    [RelayCommand]
    internal async Task LoadFromCSV()
    {
        IsBusy = true;

        if (Globals.CurrentUser?.Role?.ToLower() != "admin")
        {
            await Application.Current.MainPage.DisplayAlert("🚫 Access Denied", "This section is reserved for admins only.", "OK");
            IsBusy = false;
            return;
        }

        // Charger les nouveaux produits depuis le fichier CSV
        var newProducts = await MyCSVServices.LoadData();

        // Liste pour les produits avec un ID déjà existant
        var duplicateProducts = new List<Product>();

        // Parcours de la liste des nouveaux produits
        foreach (var product in newProducts)
        {
            // Vérifier si un produit avec le même ID existe déjà dans la liste
            var existingProduct = Globals.MyProducts.FirstOrDefault(p => p.Id == product.Id);

            if (existingProduct != null)
            {
                // Ajouter à la liste des produits en doublon
                duplicateProducts.Add(product);
            }
            else
            {
                // Si le produit n'existe pas, on l'ajoute à la liste
                Globals.MyProducts.Add(product);
            }
        }

        if (duplicateProducts.Any())
        {
            var duplicateInfo = new StringBuilder();
            foreach (var product in duplicateProducts)
            {
                duplicateInfo.AppendLine($"ID: {product.Id}, Name: {product.Name}");
            }

            bool replace = await Application.Current.MainPage.DisplayAlert(
                "⚠️ Duplicate Product IDs Detected",
                $"The following product(s) already exist in the collection with the same ID, but their attributes (Name, Price, Stock) may differ from the new products:\n{duplicateInfo.ToString()}\nDo you want to replace the existing products with the new data?",
                "Replace",
                "Ignore");

            if (replace)
            {
                foreach (var duplicateProduct in duplicateProducts)
                {
                    var existingProduct = Globals.MyProducts.FirstOrDefault(p => p.Id == duplicateProduct.Id);
                    if (existingProduct != null)
                    {
                        existingProduct.Name = duplicateProduct.Name;
                        existingProduct.Group = duplicateProduct.Group;
                        existingProduct.Stock = duplicateProduct.Stock;
                        existingProduct.Price = duplicateProduct.Price;
                    }
                }

                await Application.Current.MainPage.DisplayAlert("✅ Success", "The product(s) have been successfully replaced.", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("ℹ️ Info", "The duplicate product(s) were ignored.", "OK");
            }
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("✅ Success", "The product(s) have been successfully loaded from the CSV file.", "OK");
        }

        await RefreshPage(); // Rafraîchir la vue avec la liste mise à jour

        // Sauvegarder les changements sur le serveur
        bool success = await MyJSONService.SetProducts();
        if (!success)
        {
            await Application.Current.MainPage.DisplayAlert("❌ Error", "Failed to upload updated product list to server.", "OK");
        }


        IsBusy = false;
    }




    internal async Task RefreshPage()
    {
        MyObservableList.Clear();

        if (Globals.MyProducts == null || Globals.MyProducts.Count == 0)
        {
            // Récupérer les produits si la liste est vide
            Globals.MyProducts = await MyJSONService.GetProducts();
        }

        foreach (var item in Globals.MyProducts)
        {
            MyObservableList.Add(item);
        }
    }

    [RelayCommand]
    
    private async Task Logout()
    {
        // Supprimer les informations de l'utilisateur de Preferences
        Preferences.Remove("IsLoggedIn");
        Preferences.Remove("UserId");
        Preferences.Remove("UserRole");

        // Réinitialiser l'utilisateur global
        Globals.CurrentUser = null;

        // Naviguer vers la page de connexion
        await Shell.Current.GoToAsync("//LoginPage");
    }


    // Commande pour aller à la page du panier
    [RelayCommand]
    private async Task GoToCart()
    {
        // Naviguer vers la page CartView
        await Shell.Current.GoToAsync(nameof(CartView));
    }


}
