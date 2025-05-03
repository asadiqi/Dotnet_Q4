namespace MyApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
        Routing.RegisterRoute("MainView", typeof(View.MainView)); // Inscrire la route pour la page principale
    }

    protected override void OnStart()
    {
        base.OnStart();
        CheckIfUserIsLoggedIn();
    }

    private async void CheckIfUserIsLoggedIn()
    {
        bool isLoggedIn = Preferences.Get("IsLoggedIn", false);

        if (isLoggedIn)
        {
            // Récupérer les informations de l'utilisateur depuis les préférences
            string userId = Preferences.Get("UserId", string.Empty);
            string userRole = Preferences.Get("UserRole", string.Empty);

            // Charger l'utilisateur depuis le service en utilisant l'ID (MongoDB)
            var userService = new UsersService();
            var user = await userService.GetUserByIdAsync(userId);

            if (user != null)
            {
                // Restaurer l'utilisateur dans Globals
                Globals.CurrentUser = user;

                // Naviguer vers la page principale si l'utilisateur existe
                await Shell.Current.GoToAsync("//MainView");
            }
            else
            {
                // Si l'utilisateur n'est pas trouvé, déconnecter et aller à la page de login
                Preferences.Remove("IsLoggedIn");
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
        else
        {
            // Si l'utilisateur n'est pas connecté, aller à la page de login
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
