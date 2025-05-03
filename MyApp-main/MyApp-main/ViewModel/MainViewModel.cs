using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text;

namespace MyApp.ViewModel;

public partial class MainViewModel : BaseViewModel
{
    public ObservableCollection<Product> MyObservableList { get; } = [];

    private readonly JSONServices MyJSONService;
    private readonly CSVServices MyCSVServices;

    public MainViewModel(JSONServices myJSONService, CSVServices myCSVServices)
    {
        MyJSONService = myJSONService;
        MyCSVServices = myCSVServices;

        // Charger les produits au démarrage
        _ = LoadProductsOnStartup();
    }

    // Méthode appelée au démarrage pour charger les produits depuis le service JSON
    private async Task LoadProductsOnStartup()
    {
        IsBusy = true; // Indique que l'opération est en cours
        Globals.MyProducts = await MyJSONService.GetProducts(); // Charge les produits
        await RefreshPage(); // Rafraîchit la liste des produits
        IsBusy = false; // Fin de l'opération
    }

    // Commande pour naviguer vers la page de détails d'un produit
    [RelayCommand]
    private async Task GoToDetails(string id)
    {
        // Vérifie si l'utilisateur est un administrateur
        if (!IsAdmin())
        {
            // Si non, affiche une alerte d'accès refusé
            await ShowAccessDenied();
            return;
        }

        IsBusy = true;

        await Shell.Current.GoToAsync("DetailsView", true, new Dictionary<string, object>
        {
            { "selectedAnimal", id } // Passe l'ID comme paramètre
        });

        IsBusy = false;
    }

    // Commande pour naviguer vers la page "AllProductsView" (liste de tous les produits)
    [RelayCommand]
    private async Task GoToAllProducts()
    {
        if (IsProductListEmpty())
        {
            await ShowAlert("⚠️ Alert", "There are no products available.");
            return;
        }

        await Shell.Current.GoToAsync(nameof(AllProductsView));
    }

    // Commande pour naviguer vers la page "GraphView" (vue graphique des produits)
    [RelayCommand]
    private async Task GoToGraph()
    {
        IsBusy = true;

        if (IsProductListEmpty())
        {
            await ShowAlert("⚠️ Alert", "Product list is empty, there is no graphe available.");
        }
        else
        {
            await Shell.Current.GoToAsync("GraphView", true);
        }

        IsBusy = false;
    }

    // Commande pour naviguer vers la page du panier
    [RelayCommand]
    private async Task GoToCart()
    {
        // Si l'utilisateur est un administrateur, il ne devrait pas avoir besoin d'accéder au panier
        if (IsAdmin())
        {
            await ShowAlert("🛑 Nothing to Show", "As an administrator, you don't need to access the Basket.");
        }
        else
        {
            await Shell.Current.GoToAsync(nameof(CartView));
        }
    }

    // Commande pour se déconnecter
    [RelayCommand]
    private async Task Logout()
    {
        // Retirer les informations de connexion stockées
        Preferences.Remove("IsLoggedIn");
        Preferences.Remove("UserId");
        Preferences.Remove("UserRole");

        Globals.CurrentUser = null;

        // Naviguer vers la page de login
        await Shell.Current.GoToAsync("//LoginPage");
    }

    // Commande pour imprimer les produits en format CSV
    [RelayCommand]
    private async Task PrintToCSV()
    {
        IsBusy = true;
        await MyCSVServices.PrintData(Globals.MyProducts); // Imprime les produits
        IsBusy = false;
    }

    // Commande pour charger les produits depuis un fichier CSV
    [RelayCommand]
    private async Task LoadFromCSV()
    {
        if (!IsAdmin())
        {
            await ShowAccessDenied();
            return;
        }

        IsBusy = true;

        var newProducts = await MyCSVServices.LoadData();
        var duplicateProducts = new List<Product>();

        foreach (var product in newProducts)
        {
            var existing = Globals.MyProducts.FirstOrDefault(p => p.Id == product.Id);
            if (existing != null)
                duplicateProducts.Add(product); // Si déjà présent, ajouter à la liste des doublons
            else
                Globals.MyProducts.Add(product); // Sinon, ajouter à la liste principale
        }

        await HandleDuplicates(duplicateProducts);

        await RefreshPage();

        bool success = await MyJSONService.SetProducts();
        if (!success)
        {
            await ShowAlert("❌ Error", "Failed to upload updated product list to server.");
        }

        IsBusy = false;
    }

    // Méthode pour rafraîchir la liste des produits
    internal async Task RefreshPage()
    {
        MyObservableList.Clear(); // Vider la liste actuelle

        if (Globals.MyProducts == null || Globals.MyProducts.Count == 0)
        {
            Globals.MyProducts = await MyJSONService.GetProducts();
        }

        foreach (var product in Globals.MyProducts)
        {
            MyObservableList.Add(product);
        }
    }

    // Vérifie si l'utilisateur est un administrateur
    private static bool IsAdmin() =>
        Globals.CurrentUser?.Role?.ToLower() == "admin";

    private static bool IsProductListEmpty() =>
        Globals.MyProducts == null || Globals.MyProducts.Count == 0;

    // Méthode pour afficher une alerte
    private static Task ShowAlert(string title, string message) =>
        Application.Current.MainPage.DisplayAlert(title, message, "OK");

    // Méthode pour afficher un message d'accès refusé
    private static Task ShowAccessDenied() =>
        ShowAlert("🚫 Access Denied", "This section is reserved for admins only.");

    // Méthode pour gérer les produits en doublon lors du chargement CSV
    private async Task HandleDuplicates(List<Product> duplicateProducts)
    {
        if (!duplicateProducts.Any()) // Aucun doublon trouvé
        {
            await ShowAlert("✅ Success", "The product(s) have been successfully loaded from the CSV file.");
            return;
        }

        // Créer une chaîne pour afficher les doublons
        var duplicateInfo = new StringBuilder();
        foreach (var p in duplicateProducts)
            duplicateInfo.AppendLine($"ID: {p.Id}, Name: {p.Name}");

        // Demander à l'utilisateur s'il souhaite remplacer les doublons
        bool replace = await Application.Current.MainPage.DisplayAlert(
            "⚠️ Duplicate Product IDs Detected",
            $"The following product(s) already exist in the collection with the same ID:\n{duplicateInfo}\nDo you want to replace them?",
            "Replace", "Ignore");

        // Si l'utilisateur choisit de remplacer les doublons
        if (replace)
        {
            foreach (var dp in duplicateProducts)
            {
                var existing = Globals.MyProducts.First(p => p.Id == dp.Id);
                existing.Name = dp.Name;
                existing.Group = dp.Group;
                existing.Stock = dp.Stock;
                existing.Price = dp.Price;
            }

            await ShowAlert("✅ Success", "The product(s) have been successfully replaced.");
        }
        else
        {
            await ShowAlert("ℹ️ Info", "The duplicate product(s) were ignored.");
        }
    }
}
