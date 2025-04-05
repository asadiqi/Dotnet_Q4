using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.ViewModel;

public partial class AllProductsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Product> products = new();

    [ObservableProperty]
    private ObservableCollection<Product> filteredProducts = new();

    private string searchText = string.Empty;

    public void LoadProducts()
    {
        Products = new ObservableCollection<Product>(Globals.MyProducts);
        FilteredProducts = Products; // Par défaut, afficher tous les produits
    }

    // Commande pour supprimer un produit
    [RelayCommand]
    public async Task DeleteProduct(string productId)
    {
        var productToDelete = Globals.MyProducts.FirstOrDefault(p => p.Id == productId);
        if (productToDelete != null)
        {
            Globals.MyProducts.Remove(productToDelete);
            Products = new ObservableCollection<Product>(Globals.MyProducts);  // Rafraîchir la liste affichée
            FilteredProducts = Products;  // Rafraîchir également la liste filtrée
            await new JSONServices().SetProducts(); // Sauvegarde les changements sur le serveur
        }
    }




    // Méthode pour filtrer les produits par nom et groupe
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

    // Met à jour le texte de recherche
    public void SetSearchText(string text)
    {
        searchText = text;
        FilterProducts();
    }

    [RelayCommand]
    public async Task DeleteAllProducts()
    {
        var confirmation = await Application.Current.MainPage.DisplayAlert
 (
             "Confirmation",
             "Are you sure you want to delete all products?",
             "Yes",
             "No");


        if (confirmation)
        {
            // Vider la liste des produits
            Globals.MyProducts.Clear();
            Products.Clear();
            FilteredProducts.Clear();

            // Sauvegarder les changements sur le serveur
            await new JSONServices().SetProducts();

            // Vous pouvez également afficher un message de succès ou rafraîchir la vue
            await Application.Current.MainPage.DisplayAlert("✅ Success", "All products have been deleted.", "OK");
        }
    }

}