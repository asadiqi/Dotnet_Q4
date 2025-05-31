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

    partial void OnIdChanged(string? value)
    {
        RefreshPage();
    }


    [ObservableProperty]
    public partial string? Name { get; set; }

    [ObservableProperty]
    public partial string? Group { get; set; }

    [ObservableProperty]
    public partial int Stock { get; set; }

    [ObservableProperty]
    public partial int Price { get; set; }

    [ObservableProperty]
    public partial string? Description { get; set; }

    private string? _picture;

    // Propriété Picture qui retourne une image par défaut si aucune image n'est définie
    public string Picture
    {
        get
        {
            if (string.IsNullOrEmpty(_picture))
                return GetDefaultImage();

            // Vérifie si le fichier image existe réellement
            var imagePath = Path.Combine(FileSystem.AppDataDirectory, _picture);
            if (File.Exists(imagePath))
                return _picture;

            return GetDefaultImage(); // si l'image n'existe pas physiquement
        }
        set => SetProperty(ref _picture, value);
    }

    // Retourne une image par défaut selon le groupe du produit
    private string GetDefaultImage()
    {
        if (string.IsNullOrEmpty(Group))
            return "outher.png";

        var groupLower = Group.ToLower();

        if (groupLower.Contains("fruit"))
            return "fruit.png";
        else if (groupLower.Contains("légume") || groupLower.Contains("legume"))
            return "vegetable.png";
        else
            return "other.png";
    }



    public DeviceOrientationService MyScanner;
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
        MyScanner.SerialBuffer.Enqueue("1234567890"); // tout le code-barres
    }

    private void OnSerialDataReception(object sender, EventArgs arg)
    {
        if (sender is DeviceOrientationService.QueueBuffer MyLocalBuffer && MyLocalBuffer.Count > 0)
        {
            var sb = new StringBuilder();

            while (MyLocalBuffer.Count > 0)
            {
                var data = MyLocalBuffer.Dequeue()?.ToString();
                if (data != null)
                    sb.Append(data);
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Id = sb.ToString();
            });
        }
    }

    // Rafraîchit les informations du produit selon l'ID
    internal void RefreshPage()
    {
        var product = Globals.MyProducts.FirstOrDefault(p => p.Id == Id);
        if (product != null)
        {
            Name = product.Name;
            Group = product.Group;
            Stock = product.Stock;
            Price = product.Price;
            Description= product.Description;
            Picture = product.Picture;
        }
    }

    internal void ClosePage()
    {
        MyScanner.SerialBuffer.Changed -= OnSerialDataReception;
        MyScanner.ClosePort();
    }


    // Commande pour changer les paramètres d'un objet
    [RelayCommand]
    internal async Task ChangeObjectParameters()
    {
        if (string.IsNullOrWhiteSpace(Id))
        {
            await Application.Current.MainPage.DisplayAlert("❌ Error", "The ID field cannot be empty or contain letters, it must contain only digits.", "OK");
            return;
        }


        if (!Id.All(char.IsDigit))
        {
            await Application.Current.MainPage.DisplayAlert("❌ Error", "The ID field must contain only digits.", "OK");
            return;
        }


        if (string.IsNullOrWhiteSpace(Name))
        {
            await Application.Current.MainPage.DisplayAlert("❌ Error", "The Name field cannot be empty.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Group))
        {
            await Application.Current.MainPage.DisplayAlert("❌ Error", "The Group field cannot be empty.", "OK");
            return;
        }

        if (Stock <= 0)
        {
            await Application.Current.MainPage.DisplayAlert("❌ Error", "Stock must be greater than 0 and it must contain only digits.", "OK");
            return;
        }


        if (Price <= 0)
        {
            await Application.Current.MainPage.DisplayAlert("❌ Error", "Price must be greater than 0 and it must contain only digits.", "OK");
            return;
        }



        if (string.IsNullOrWhiteSpace(Description))
        {
            await Application.Current.MainPage.DisplayAlert("❌ Error", "The Description field cannot be empty.", "OK");
            return;
        }



        var existingProduct = Globals.MyProducts.FirstOrDefault(p => p.Id == Id);

        if (existingProduct != null)
        {
            existingProduct.Name = Name ?? string.Empty;
            existingProduct.Group = Group ?? string.Empty;
            existingProduct.Stock = Stock;
            existingProduct.Price = Price;
            existingProduct.Description = Description;


        }
        else
        {
            Globals.MyProducts.Add(new Product
            {
                Id = Id ?? Guid.NewGuid().ToString(),
                Name = Name ?? string.Empty,
                Group = Group ?? string.Empty,
                Stock = Stock,
                Price = Price,
                Description = Description
            });
        }

        await new JSONServices().SetProducts(); //envoie les données vers serveur automatique 
    }



    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Id) &&          // Vérifie que l'ID n'est pas vide
               Id.All(char.IsDigit) &&                   // Vérifie que l'ID contient uniquement des chiffres
               !string.IsNullOrWhiteSpace(Name) &&       // Vérifie que le nom n'est pas vide
               !string.IsNullOrWhiteSpace(Group) &&      // Vérifie que le groupe n'est pas vide
               Stock > 0 &&                                // Vérifie que le stock est supérieur à 0
               Price > 0 &&                            // Vérifie que le prix est supérieur à 0
              !string.IsNullOrWhiteSpace(Description);
    }

    // Commande pour soumettre un produit après validation
    [RelayCommand]
    internal async Task SubmitProduct()
    {
        if (!IsValid())
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields correctly.", "OK");
            return;
        }

        var existingProduct = Globals.MyProducts.FirstOrDefault(p => p.Id == Id);
        
        // Vérifie si le produit existe déjà
        if (existingProduct != null)
        {
            bool replace = await Application.Current.MainPage.DisplayAlert(
                "⚠️ Duplicate Product",
                $"A product with the ID {Id} already exists:\n\nName: {existingProduct.Name}\nGroup: {existingProduct.Group}\nPrice: {existingProduct.Price} €\nStock: {existingProduct.Stock}\nStock: {existingProduct.Description}\n\nDo you want to replace it with the new data?",
                "Yes, replace",
                "No, cancel");

            if (!replace)
            {
                return;
            }

            // Remplace les informations du produit existant
            existingProduct.Name = Name ?? string.Empty;
            existingProduct.Group = Group ?? string.Empty;
            existingProduct.Stock = Stock;
            existingProduct.Price = Price;
            existingProduct.Description = Description ?? string.Empty;
            existingProduct.Picture = Picture;
        }
        else
        {
            // Ajoute un nouveau produit
            Globals.MyProducts.Add(new Product
            {
                Id = Id ?? Guid.NewGuid().ToString(),
                Name = Name ?? string.Empty,
                Group = Group ?? string.Empty,
                Stock = Stock,
                Price = Price,
                Description = Description ?? string.Empty,
                Picture = Picture
            });
        }

        await new JSONServices().SetProducts();  // Sauvegarde les modifications sur le serveur


        await Shell.Current.GoToAsync(nameof(AllProductsView)); // Navigation vers la vue des produits

    }

    // Commande pour sélectionner une image
    [RelayCommand]
    internal async Task SelectImage()
    {
        try
        {
            // Demander à l'utilisateur de choisir une image<
            var result = await MediaPicker.PickPhotoAsync();

            if (result != null)
            {
                // Si une image a été sélectionnée, mettre à jour la propriété Picture
                Picture = result.FullPath; // Ou utiliser result.FileName si vous préférez
            }
        }
        catch (Exception ex)
        {
            // Gérer les erreurs ici, si l'utilisateur annule ou autre problème
            await Application.Current.MainPage.DisplayAlert("Error", "An error occurred while selecting the image: " + ex.Message, "OK");
        }
    }
}