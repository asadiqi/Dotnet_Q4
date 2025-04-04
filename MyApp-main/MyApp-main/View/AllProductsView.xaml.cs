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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await (BindingContext as MainViewModel)?.RefreshPage();
        }


        // Méthode pour revenir à la page principale
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            // Utilisation de la route pour revenir à la page MainView
            await Shell.Current.GoToAsync("//MainView");
        }
    }
}
