using MyApp.ViewModel;
using Microsoft.Maui.Controls;
using System;

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

    // Méthode pour afficher le popup de confirmation
    public async Task ShowDeleteConfirmationDialog(string productId)
    {
        bool isConfirmed = await DisplayAlert("Confirmation", "Are you sure you want to delete this product?", "Yes", "No");

        if (isConfirmed)
        {
            // Appel de la commande DeleteProductCommand avec l'ID du produit
            var viewModel = (AllProductsViewModel)BindingContext;
            await viewModel.DeleteProduct(productId);
        }
    }

    // Gestion de l'événement du bouton Delete
    private async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var productId = (string)button.CommandParameter;

        // Affiche la confirmation avant de supprimer
        await ShowDeleteConfirmationDialog(productId);
    }

    // Gestion du changement de texte dans la barre de recherche
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var viewModel = (AllProductsViewModel)BindingContext;
        viewModel.SetSearchText(e.NewTextValue);
    }


    private async void OnModifyButtonClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var productId = (string)button.CommandParameter;

        // Récupérer le produit à partir de son Id
        var product = Globals.MyProducts.FirstOrDefault(p => p.Id == productId);

        if (product != null)
        {
            // Navigation vers la page de détails avec les données du produit
            var parameters = new Dictionary<string, object>
        {
            { "selectedProduct", productId }
        };

            await Shell.Current.GoToAsync("DetailsView", parameters);
        }
    }

    private async void OnDeleteAllButtonClicked(object sender, EventArgs e)
    {
        var viewModel = (AllProductsViewModel)BindingContext;
        await viewModel.DeleteAllProducts();

        // utilisation de relaycommand
    }

}