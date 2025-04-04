using MyApp.ViewModel;

namespace MyApp.View;

public partial class AllProductsView : ContentPage
{
    public AllProductsView(AllProductsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        ((AllProductsViewModel)BindingContext).LoadProducts();
    }
}