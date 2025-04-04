using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.ViewModel;

public partial class AllProductsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Product> products = new();

    public void LoadProducts()
    {
        Products = new ObservableCollection<Product>(Globals.MyProducts);
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
            await new JSONServices().SetProducts(); // Sauvegarde les changements sur le serveur
        }
    }
}
