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
}