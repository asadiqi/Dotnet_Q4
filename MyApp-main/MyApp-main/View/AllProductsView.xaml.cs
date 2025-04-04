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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await (BindingContext as MainViewModel)?.RefreshPage();
        }


        // M�thode pour revenir � la page principale
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            // Utilisation de la route pour revenir � la page MainView
            await Shell.Current.GoToAsync("//MainView");
        }
    }
}
