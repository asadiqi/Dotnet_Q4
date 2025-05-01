using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyApp.ViewModel
{
    public class ForgotPasswordViewModel : INotifyPropertyChanged
    {
        private string _email;
        private string _statusMessage;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public ICommand ResetPasswordCommand { get; }

        public ForgotPasswordViewModel()
        {
            ResetPasswordCommand = new AsyncRelayCommand(OnResetPassword);
        }

        private async Task OnResetPassword()
        {
            StatusMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Email))
            {
                StatusMessage = "Email is required.";
                return;
            }

            // Regex pour vérifier une adresse email valide
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            if (!emailRegex.IsMatch(Email))
            {
                StatusMessage = "Please enter a valid email address.";
                return;
            }

            // Simule un envoi (attente factice)
            await Task.Delay(500);

            StatusMessage = "We send you an email please check your inbox and follow the steps to reset your password.";
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
