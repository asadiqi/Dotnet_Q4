using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

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

        [RelayCommand]
        public async Task ConfirmOrder()
        {
            if (!CartItems.Any())
            {
                await Application.Current.MainPage.DisplayAlert("Panier vide", "Ajoutez des produits avant de commander.", "OK");
                return;
            }

            // Ici tu pourrais envoyer la commande au serveur, stocker en base de données, etc.
            await Application.Current.MainPage.DisplayAlert("Commande confirmée", "Votre commande a été validée avec succès !", "OK");

            Globals.Cart.Clear();
            CartItems.Clear();
            OnPropertyChanged(nameof(TotalPriceText));
        }
    }
}
