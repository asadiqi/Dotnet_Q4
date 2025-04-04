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
                await Application.Current.MainPage.DisplayAlert("✅ Succès", "Les données ont été enregistrées avec succès sur le serveur.", "OK");
                await CheckServerData();  // Vérifier les données après l'envoi
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

    internal async Task CheckServerData()
    {
        IsBusy = true;

        var serverProducts = await MyJSONService.GetProducts();

        if (serverProducts.Count > 0)
        {
            await Application.Current.MainPage.DisplayAlert("🔍 Vérification", $"{serverProducts.Count} produits ont bien été enregistrés sur le serveur.", "OK");
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("⚠️ Attent<ion", "Aucune donnée trouvée sur le serveur après l'envoi. Vérifiez votre connexion.", "OK");
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
