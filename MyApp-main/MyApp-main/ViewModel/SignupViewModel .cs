using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyApp.Service;
using MyApp.Models;  // Ajoute le namespace de ton modèle User
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyApp.ViewModel
{
    public partial class SignupViewModel : ObservableObject
    {
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _password;
        private bool _isBusy;
        private string _errorMessage;

        private readonly UsersService _usersService;  // Le service des utilisateurs

        // Properties
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        // Command to register the user
        public ICommand RegisterCommand { get; }

        public SignupViewModel()
        {
            _usersService = new UsersService();  // Initialisation du service des utilisateurs
            RegisterCommand = new AsyncRelayCommand(OnRegister);
        }

        // Méthode d'enregistrement
        private async Task OnRegister()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            ErrorMessage = string.Empty;  // Réinitialisation du message d'erreur

            // Vérification des champs
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "All fields are required.";
                IsBusy = false;
                return;
            }

            // Validation de l'email avec regex
            if (!IsValidEmail(Email))
            {
                ErrorMessage = "Invalid email format.";
                IsBusy = false;
                return;
            }

            // Validation du mot de passe avec regex
            if (!IsValidPassword(Password))
            {
                ErrorMessage = "Password must be at least 8 characters long and include an uppercase letter, a lowercase letter, and a special character.";
                IsBusy = false;
                return;
            }

            // Création de l'utilisateur
            var newUser = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email
            };

            // Définition du mot de passe haché
            newUser.SetPassword(Password);

            // Appel du service pour ajouter l'utilisateur
            var success = await _usersService.AddUserAsync(newUser, Password);

            if (success)
            {
                // Si l'utilisateur est enregistré avec succès, redirection vers la page de connexion
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                ErrorMessage = "The email is already in use.";
            }

            IsBusy = false;
        }

        // Méthode de validation de l'email
        private bool IsValidEmail(string email)
        {
            // Expression régulière pour valider l'email
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        // Méthode de validation du mot de passe
        private bool IsValidPassword(string password)
        {
            // Expression régulière pour valider le mot de passe
            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_]).{8,}$";
            return Regex.IsMatch(password, passwordPattern);
        }
    }
}
