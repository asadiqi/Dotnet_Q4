namespace MyApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(DetailsView), typeof(DetailsView));
            Routing.RegisterRoute(nameof(GraphView), typeof(GraphView));
            Routing.RegisterRoute(nameof(AllProductsView), typeof(AllProductsView));
        }
    }
}
