namespace MyApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Assurez-vous que le Shell est bien initialisé avant d'y accéder
        MainPage = new AppShell();

        // Appeler la méthode après l'initialisation complète
        Routing.RegisterRoute("MainView", typeof(View.MainView));
    }

    protected override void OnStart()
    {
        base.OnStart();
        CheckIfUserIsLoggedIn(); // Appel ici après l'initialisation du Shell
    }

    private async void CheckIfUserIsLoggedIn()
    {
        bool isLoggedIn = Preferences.Get("IsLoggedIn", false);

        if (isLoggedIn)
        {
            // Utiliser la bonne route avec la syntaxe correcte
            await Shell.Current.GoToAsync("//MainView");
        }
        else
        {
            // Utiliser la bonne route pour LoginPage
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
