using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.ViewModel;

[QueryProperty(nameof(Id), "selectedProduct")]
public partial class DetailsViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string? Id { get; set; }
    [ObservableProperty]
    public partial string? Name { get; set; }
    [ObservableProperty]
    public partial string? Group { get; set; }
    [ObservableProperty]
    public partial int Stock { get; set; }
    [ObservableProperty]
    public partial int Price { get; set; }

    readonly DeviceOrientationService MyScanner;
    IDispatcherTimer emulator = Application.Current.Dispatcher.CreateTimer();

    public DetailsViewModel(DeviceOrientationService myScanner)
    {
        this.MyScanner = myScanner;
        MyScanner.OpenPort();
        myScanner.SerialBuffer.Changed += OnSerialDataReception;

        emulator.Interval = TimeSpan.FromSeconds(1);
        emulator.Tick += (s, e) => AddCode();
    }

    private void AddCode()
    {
        MyScanner.SerialBuffer.Enqueue("B");
    }

    private void OnSerialDataReception(object sender, EventArgs arg)
    {
        DeviceOrientationService.QueueBuffer MyLocalBuffer = (DeviceOrientationService.QueueBuffer)sender;

        if (MyLocalBuffer.Count > 0)
        {
            // Traitement des données reçues
        }
    }

    internal void RefreshPage()
    {
        foreach (var item in Globals.MyProducts)
        {
            if (Id == item.Id)
            {
                Name = item.Name;
                Group = item.Group;
                Stock = item.Stock;
                Price = item.Price;
                break;
            }
        }
    }

    internal void ClosePage()
    {
        MyScanner.SerialBuffer.Changed -= OnSerialDataReception;
        MyScanner.ClosePort();
    }

    [RelayCommand]
    internal void ChangeObjectParameters()
    {
        foreach (var item in Globals.MyProducts)
        {
            if (item.Id == Id)
            {
                item.Name = Name ?? string.Empty;
                item.Group = Group ?? string.Empty;
                item.Stock = Stock;
                item.Price = Price;
            }
        }
    }
}
