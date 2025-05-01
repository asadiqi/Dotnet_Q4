using MyApp.ViewModel;

namespace MyApp.View
{
    public partial class SignupPage : ContentPage
    {
        // Constructor
        public SignupPage()
        {
            InitializeComponent();
            BindingContext = new SignupViewModel(); // Lier le ViewModel à la page
        }
    }
}
