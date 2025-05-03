using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyApp.ViewModel;

public partial class AllProductsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Product> products = new();

    [ObservableProperty]
    private ObservableCollection<Product> filteredProducts = new();

    [ObservableProperty]
    private bool isAdmin = Globals.CurrentUser?.Role == "admin";

    private string searchText = string.Empty;

    public AllProductsViewModel()
    {
        LoadProducts();
    }

    // Chargement des produits
    [RelayCommand]
    public void LoadProducts()
    {
        Products = new ObservableCollection<Product>(Globals.MyProducts);
        FilteredProducts = Products;
    }

    // Filtrage des produits par nom, id ou groupe
    public void FilterProducts()
    {
        var filtered = Products.AsEnumerable();

        if (!string.IsNullOrEmpty(searchText))
        {
            filtered = filtered.Where(p =>
                p.Id.ToLower().Contains(searchText.ToLower()) ||
                p.Name.ToLower().Contains(searchText.ToLower()) ||
                p.Group.ToLower().Contains(searchText.ToLower())
            );
        }

        FilteredProducts = new ObservableCollection<Product>(filtered);
    }

    // Mise à jour du texte de recherche
    public void SetSearchText(string text)
    {
        searchText = text;
        FilterProducts();
    }

    // Suppression d'un produit
    [RelayCommand]
    public async Task DeleteProduct(string productId)
    {
        var productToDelete = Globals.MyProducts.FirstOrDefault(p => p.Id == productId);
        if (productToDelete != null)
        {
            Globals.MyProducts.Remove(productToDelete);
            UpdateProductLists();
            await new JSONServices().SetProducts(); // Sauvegarde des changements sur le serveur
        }
    }

    // Suppression de tous les produits
    [RelayCommand]
    public async Task DeleteAllProducts()
    {
        var confirmation = await Application.Current.MainPage.DisplayAlert(
            "Confirmation",
            "Are you sure you want to delete all products?",
            "Yes", "No"
        );

        if (confirmation)
        {
            Globals.MyProducts.Clear();
            UpdateProductLists();
            await new JSONServices().SetProducts();
            await Application.Current.MainPage.DisplayAlert("✅ Success", "All products have been deleted.", "OK");
        }
    }

    // Confirmation avant suppression d'un produit
    [RelayCommand]
    public async Task ConfirmAndDeleteProduct(string productId)
    {
        bool confirm = await Application.Current.MainPage.DisplayAlert("ℹ️ Confirmation", "Are you sure you want to delete this product?", "Yes", "No");

        if (confirm)
        {
            await DeleteProduct(productId);
        }
    }

    // Navigation vers la vue de modification d'un produit
    [RelayCommand]
    public async Task NavigateToEdit(string productId)
    {
        var parameters = new Dictionary<string, object>
        {
            { "selectedProduct", productId }
        };

        await Shell.Current.GoToAsync("DetailsView", parameters);
    }

    // Ajouter un produit au panier
    [RelayCommand]
    public async Task AddToCart(Product product)
    {
        var existingInCart = Globals.Cart.FirstOrDefault(p => p.ProductId == product.Id);
        if (existingInCart != null)
        {
            existingInCart.Quantity++;
        }
        else
        {
            Globals.Cart.Add(new ProductInCart
            {
                ProductId = product.Id,
                Name = product.Name,
                Quantity = 1
            });
        }

        SaveCartToPreferences();
        await Application.Current.MainPage.DisplayAlert("✅ Product Added", $"{product.Name} has been added to the cart.", "OK");
    }

    // Sauvegarde du panier dans les préférences
    private void SaveCartToPreferences()
    {
        var cartJson = JsonSerializer.Serialize(Globals.Cart);
        Preferences.Set("Cart", cartJson);  // Stockage du panier dans les préférences
    }

    // Mise à jour des listes des produits (principalement après des suppressions)
    private void UpdateProductLists()
    {
        Products = new ObservableCollection<Product>(Globals.MyProducts);
        FilteredProducts = Products;
    }
}
