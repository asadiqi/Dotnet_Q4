using MyApp.ViewModel;

namespace MyApp.View
{
    public partial class AllProductsView : ContentPage
    {
        public AllProductsView()
        {
            InitializeComponent();

            // Lier la page � MainViewModel
            BindingContext = new MainViewModel(new JSONServices(), new CSVServices());

            // Rafra�chir la liste des produits au d�marrage
            _ = ((MainViewModel)BindingContext).RefreshPage();
        }
    }
}
