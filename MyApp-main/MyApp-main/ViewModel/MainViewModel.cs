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

        await Shell.Current.GoToAsync("DetailsView", true, new Dictionary<string,object>
        {
            {"selectedAnimal",id}
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
            await Application.Current.MainPage.DisplayAlert("Alerte", "Il n'y a pas de produit disponible.", "OK");
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

        await Shell.Current.GoToAsync("GraphView", true);

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

        Globals.MyProducts = await MyCSVServices.LoadData();
       
        await RefreshPage();

        IsBusy = false;
    }

    [RelayCommand]
    internal async Task UploadJson()
    {
        IsBusy = true;

        try
        {


            bool success = await MyJSONService.SetProducts();

            if (success)
            {
                var serverProducts = await MyJSONService.GetProducts();

                string message =  $"{serverProducts.Count} produits ont bien été enregistrés sur le serveur.";

                await Application.Current.MainPage.DisplayAlert("✅ Succès", message, "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("❌ Erreur", "L'enregistrement des données a échoué.", "OK");
            }
        
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("❌ Erreur", $"Erreur lors de l'enregistrement des données sur le serveur : {ex.Message}", "OK");
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

}
