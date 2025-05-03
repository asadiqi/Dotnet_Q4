using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace MyApp.ViewModel
{
    public partial class CartViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ProductInCart> cartItems;

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
            LoadCartFromPreferences();
        }

        public void LoadCartFromPreferences()
        {
            var cartJson = Preferences.Get("Cart", string.Empty);
            if (!string.IsNullOrEmpty(cartJson))
            {
                Globals.Cart = JsonSerializer.Deserialize<List<ProductInCart>>(cartJson) ?? new List<ProductInCart>();
            }
            else
            {
                Globals.Cart = new List<ProductInCart>();
            }

            CartItems = new ObservableCollection<ProductInCart>(Globals.Cart);
        }

        [RelayCommand]
        public async Task ConfirmOrder()
        {
            if (!CartItems.Any())
            {
                await Application.Current.MainPage.DisplayAlert("Empty Cart", "Please add products before placing an order.", "OK");
                return;
            }

            var currentUser = Globals.CurrentUser;
            if (currentUser != null)
            {
                var userService = new UsersService();
                var user = await userService.GetUserByIdAsync(currentUser.Id); // reload from DB

                if (user != null)
                {
                    foreach (var item in Globals.Cart)
                    {
                        var itemId = item.ProductId.Trim();

                        var existingProduct = user.Products
                            .FirstOrDefault(p => p.ProductId.Trim() == itemId);

                        if (existingProduct != null)
                        {
                            existingProduct.Quantity += item.Quantity;
                        }
                        else
                        {
                            user.Products.Add(new ProductInCart
                            {
                                ProductId = item.ProductId,
                                Name = item.Name,
                                Quantity = item.Quantity
                            });
                        }
                    }

                    await userService.UpdateUserCartAsync(user.Id, user.Products);
                    Globals.CurrentUser.Products = user.Products; // update local cache
                }
            }

            await Application.Current.MainPage.DisplayAlert("Order Confirmed", "Your order has been successfully confirmed.", "OK");

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
                $"Do you want to remove {product.Name} from your Basket?",
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
            Preferences.Set("Cart", cartJson);
        }
    }
}
