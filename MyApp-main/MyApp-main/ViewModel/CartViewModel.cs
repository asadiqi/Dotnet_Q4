using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace MyApp.ViewModel
{
    public partial class CartViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ProductInCart> cartItems = new(Globals.Cart);

        public string TotalPriceText => $"Total: {CalculateTotal()} €";

        private int CalculateTotal()
        {
            var total = 0;
            foreach (var item in Globals.Cart)
            {
                var product = Globals.MyProducts.FirstOrDefault(p => p.Id == item.ProductId);
                if (product != null)
                    total += product.Price * item.Quantity;
            }
            return total;
        }

        public CartViewModel()
        {
            // Charger le panier depuis les préférences lors de l'initialisation du ViewModel
            LoadCartFromPreferences();
        }

        // Charge le panier depuis les préférences
        public void LoadCartFromPreferences()
        {
            var cartJson = Preferences.Get("Cart", string.Empty);
            if (!string.IsNullOrEmpty(cartJson))
            {
                // Désérialisation du JSON pour récupérer le panier
                Globals.Cart = JsonSerializer.Deserialize<List<ProductInCart>>(cartJson) ?? new List<ProductInCart>();
            }
            else
            {
                Globals.Cart = new List<ProductInCart>();  // Panier vide si aucune donnée n'est trouvée
            }

            // Mettre à jour la collection d'articles
            CartItems = new ObservableCollection<ProductInCart>(Globals.Cart);
        }

        [RelayCommand]
        public async Task ConfirmOrder()
        {
            if (!CartItems.Any())
            {
                await Application.Current.MainPage.DisplayAlert("Panier vide", "Ajoutez des produits avant de commander.", "OK");
                return;
            }

            // Validation de la commande
            await Application.Current.MainPage.DisplayAlert("Commande confirmée", "Votre commande a été validée avec succès !", "OK");

            // Vider le panier local et dans les préférences
            Globals.Cart.Clear();
            SaveCartToPreferences();  // Mise à jour du panier vide dans les préférences
            CartItems.Clear();
            OnPropertyChanged(nameof(TotalPriceText));
        }

        private void SaveCartToPreferences()
        {
            var cartJson = JsonSerializer.Serialize(Globals.Cart);
            Preferences.Set("Cart", cartJson);  // Sauvegarde du panier vide dans les préférences
        }
    }
}
