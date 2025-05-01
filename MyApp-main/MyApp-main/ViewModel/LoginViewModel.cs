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

        public LoginViewModel()
        {
            LoginCommand = new AsyncRelayCommand(OnLogin);
        }

        private async Task OnLogin()
        {
            IsBusy = true;
            ErrorMessage = "";

            var user = await _userService.AuthenticateAsync(Email, Password);

            if (user != null)
            {
                await Shell.Current.GoToAsync("//MainView");
            }
            else
            {
                ErrorMessage = "Email ou mot de passe incorrect.";
            }

            IsBusy = false;
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
