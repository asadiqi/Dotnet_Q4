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


    // Gestion du changement de texte dans la barre de recherche
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var viewModel = (AllProductsViewModel)BindingContext;
        viewModel.SetSearchText(e.NewTextValue);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((AllProductsViewModel)BindingContext).LoadProductsCommand.Execute(null);
    }



}