using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MyApp.Models;
using MyApp.Service;
using CommunityToolkit.Mvvm.Input; // si tu utilises MVVM Toolkit (recommandé)
using Microsoft.Maui.Controls;

namespace MyApp.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _email;
        private string _password;
        private string _errorMessage;
        private bool _isBusy;

        public event PropertyChangedEventHandler PropertyChanged;

        private UsersService _userService = new UsersService();

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand NavigateToSignupCommand { get; }
        public ICommand NavigateToForgotPasswordCommand { get; }
        public LoginViewModel()
        {
            LoginCommand = new AsyncRelayCommand(OnLogin);
            NavigateToSignupCommand = new AsyncRelayCommand(OnNavigateToSignup);
            NavigateToForgotPasswordCommand = new AsyncRelayCommand(OnNavigateToForgotPassword);
        }

        private async Task OnLogin()
        {
            IsBusy = true;
            ErrorMessage = "";

            var user = await _userService.AuthenticateAsync(Email, Password);

            if (user != null)
            {
                Preferences.Set("IsLoggedIn", true);
                Globals.CurrentUser = user;
                await Shell.Current.GoToAsync("//MainView");
            }
            else
            {
                ErrorMessage = "Incorrect email or password.";
            }

            IsBusy = false;
        }

        private async Task OnNavigateToSignup()
        {
              await Shell.Current.GoToAsync("SignupPage");
        }

        private async Task OnNavigateToForgotPassword()
        {
            // Naviguer vers la page de réinitialisation du mot de passe
            await Shell.Current.GoToAsync("ForgotPasswordPage");
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
