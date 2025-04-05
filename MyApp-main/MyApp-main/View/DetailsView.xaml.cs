using MyApp.ViewModel;

namespace MyApp.View;

public partial class DetailsView : ContentPage
{
    readonly DetailsViewModel viewModel;
	public DetailsView(DetailsViewModel viewModel)
	{
		this.viewModel = viewModel;
		InitializeComponent();
		BindingContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        BindingContext = null;
        viewModel.RefreshPage();    // Réinitialise la observablecollection
        BindingContext = viewModel;
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        viewModel.ClosePage();
    }

    private async void MyAnimatedButton_Clicked(object sender, EventArgs e)
    {
        await MyAnimatedButton.ScaleTo(1.1, 100);
        await MyAnimatedButton.ScaleTo(1.0, 100);


        // Ne navigue que si la validation a réussi (c'est-à-dire que les champs sont remplis)
        if (viewModel.IsValid())
        {
            await Shell.Current.GoToAsync(nameof(AllProductsView)); // va vers la page AllProduct apres ajout d'un produit 
        }
    }
    private void OnSimulateScanClicked(object sender, EventArgs e)
    {
        // Simule la réception d'un code-barres
        viewModel.MyScanner.SerialBuffer.Enqueue("987654321");
    }



}