using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyApp.Service;
using MyApp.Models;  // Ajoute le namespace de ton modèle User
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
    }
}
