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

            var currentUser = Globals.CurrentUser;
            if (currentUser != null)
            {
                var userService = new UsersService();
                var user = await userService.GetUserByIdAsync(currentUser.Id);
                if (user != null)
                {
                    // Cloner le panier actuel
                    var clonedCart = Globals.Cart.Select(p => new ProductInCart
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Quantity = p.Quantity
                    }).ToList();

                    // Ajouter les produits du panier au panier existant de l'utilisateur (sans écraser)
                    foreach (var item in clonedCart)
                    {
                        var existingProduct = user.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                        if (existingProduct != null)
                        {
                            // Si le produit existe déjà, augmente la quantité
                            existingProduct.Quantity += item.Quantity;
                        }
                        else
                        {
                            // Sinon, ajoute le produit au panier
                            user.Products.Add(item);
                        }
                    }

                    // Mettre à jour le panier de l'utilisateur dans la base de données
                    await userService.UpdateUserCartAsync(user.Id, user.Products);
                    Globals.CurrentUser.Products = user.Products;
                }
            }

            await Application.Current.MainPage.DisplayAlert("Order Confirmed", "Your order has been successfully validated!", "OK");

            // Réinitialiser le panier local
            Globals.Cart.Clear();
            SaveCartToPreferences();
            CartItems.Clear();
            OnPropertyChanged(nameof(TotalPriceText));
        }


        [RelayCommand]
        public async Task RemoveFromCart(ProductInCart product)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Remove Product",
                $"Do you want to remove {product.Name} from your cart?",
                "Yes", "No");

            if (confirm)
            {
                Globals.Cart.Remove(product);
                CartItems.Remove(product);
                SaveCartToPreferences();
                OnPropertyChanged(nameof(TotalPriceText));
            }
        }


        private void SaveCartToPreferences()
        {
            var cartJson = JsonSerializer.Serialize(Globals.Cart);
            Preferences.Set("Cart", cartJson);  // Sauvegarde du panier vide dans les préférences
        }
    }
}
