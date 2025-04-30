namespace MyApp.View;

public partial class GraphView : ContentPage
{
    readonly GraphViewModel viewModel;

    public GraphView(GraphViewModel viewModel)
    {
        this.viewModel = viewModel;
        InitializeComponent();
        BindingContext = viewModel;
    }

    


}
