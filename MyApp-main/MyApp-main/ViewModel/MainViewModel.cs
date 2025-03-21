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

        IsBusy = false;
    }
    
    internal async Task RefreshPage()
    {
        MyObservableList.Clear ();

        if(Globals.MyProducts.Count == 0) Globals.MyProducts = await MyJSONService.GetProducts();

        foreach (var item in Globals.MyProducts)
        {
            MyObservableList.Add(item);
        }
    }
}
