using MyApp.Service;
using MyApp.Models;

namespace MyApp.View;

public partial class LoginPage : ContentPage
{

    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel();
    }

   
}
