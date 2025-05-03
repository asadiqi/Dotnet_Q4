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

        _ = LoadProductsOnStartup();
    }

    private async Task LoadProductsOnStartup()
    {
        IsBusy = true;
        Globals.MyProducts = await MyJSONService.GetProducts();
        await RefreshPage();
        IsBusy = false;
    }

    // -------------------- NAVIGATION COMMANDS --------------------

    [RelayCommand]
    private async Task GoToDetails(string id)
    {
        if (!IsAdmin())
        {
            await ShowAccessDenied();
            return;
        }

        IsBusy = true;

        await Shell.Current.GoToAsync("DetailsView", true, new Dictionary<string, object>
        {
            { "selectedAnimal", id }
        });

        IsBusy = false;
    }

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

    [RelayCommand]
    private async Task GoToCart()
    {
        if (IsAdmin())
        {
            await ShowAlert("🛑 Nothing to Show", "As an administrator, you don't need to access the Basket.");
        }
        else
        {
            await Shell.Current.GoToAsync(nameof(CartView));
        }
    }

    [RelayCommand]
    private async Task Logout()
    {
        Preferences.Remove("IsLoggedIn");
        Preferences.Remove("UserId");
        Preferences.Remove("UserRole");

        Globals.CurrentUser = null;

        await Shell.Current.GoToAsync("//LoginPage");
    }

    // -------------------- CSV COMMANDS --------------------

    [RelayCommand]
    private async Task PrintToCSV()
    {
        IsBusy = true;
        await MyCSVServices.PrintData(Globals.MyProducts);
        IsBusy = false;
    }

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
                duplicateProducts.Add(product);
            else
                Globals.MyProducts.Add(product);
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

    // -------------------- UI DATA --------------------

    internal async Task RefreshPage()
    {
        MyObservableList.Clear();

        if (Globals.MyProducts == null || Globals.MyProducts.Count == 0)
        {
            Globals.MyProducts = await MyJSONService.GetProducts();
        }

        foreach (var product in Globals.MyProducts)
        {
            MyObservableList.Add(product);
        }
    }

    // -------------------- HELPERS --------------------

    private static bool IsAdmin() =>
        Globals.CurrentUser?.Role?.ToLower() == "admin";

    private static bool IsProductListEmpty() =>
        Globals.MyProducts == null || Globals.MyProducts.Count == 0;

    private static Task ShowAlert(string title, string message) =>
        Application.Current.MainPage.DisplayAlert(title, message, "OK");

    private static Task ShowAccessDenied() =>
        ShowAlert("🚫 Access Denied", "This section is reserved for admins only.");

    private async Task HandleDuplicates(List<Product> duplicateProducts)
    {
        if (!duplicateProducts.Any())
        {
            await ShowAlert("✅ Success", "The product(s) have been successfully loaded from the CSV file.");
            return;
        }

        var duplicateInfo = new StringBuilder();
        foreach (var p in duplicateProducts)
            duplicateInfo.AppendLine($"ID: {p.Id}, Name: {p.Name}");

        bool replace = await Application.Current.MainPage.DisplayAlert(
            "⚠️ Duplicate Product IDs Detected",
            $"The following product(s) already exist in the collection with the same ID:\n{duplicateInfo}\nDo you want to replace them?",
            "Replace", "Ignore");

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
