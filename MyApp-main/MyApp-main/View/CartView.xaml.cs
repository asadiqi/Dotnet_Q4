using MyApp.ViewModel;
using Microsoft.Maui.Controls;
using System;

namespace MyApp.View;
public partial class CartView : ContentPage
{
    public CartView()
    {
        InitializeComponent();
        BindingContext = new CartViewModel(); // Assurez-vous que le ViewModel est bien initialisé
    }
}
