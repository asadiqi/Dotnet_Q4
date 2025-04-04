using MyApp.ViewModel;

namespace MyApp.View
{
    public partial class AllProductsView : ContentPage
    {
        public AllProductsView()
        {
            InitializeComponent();

            // Lier la page à MainViewModel
            BindingContext = new MainViewModel(new JSONServices(), new CSVServices());

            // Rafraîchir la liste des produits au démarrage
            _ = ((MainViewModel)BindingContext).RefreshPage();
        }
    }
}
