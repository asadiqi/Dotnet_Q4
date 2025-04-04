using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MyApp.Model;

namespace MyApp.ViewModel;

public partial class AllProductsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Product> products = new();

    public void LoadProducts()
    {
        Products = new ObservableCollection<Product>(Globals.MyProducts);
    }
}