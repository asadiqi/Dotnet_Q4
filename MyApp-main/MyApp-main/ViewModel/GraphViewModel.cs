using Microcharts;
using Microsoft.Maui.Graphics;
using SkiaSharp;
using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.ViewModel;

public partial class GraphViewModel : ObservableObject
{
    [ObservableProperty]
    public partial Chart MyObservableChart { get; set; } = new BarChart(); // Utilisation d'un BarChart pour les diagrammes en bâtonnets

    // Cette méthode est utilisée pour mettre à jour les données du graphique.
    public void UpdateChart(List<Product> products)
    {
        var entries = products.Select(p => new ChartEntry(p.Stock)
        {
            Label = p.Name,
            ValueLabel = p.Stock.ToString(),
            Color = SKColor.Parse("#b455b6")
        }).ToArray();

        MyObservableChart = new BarChart { Entries = entries };
    }

    public GraphViewModel()
    {
        // Initialisation avec des produits fictifs si nécessaire
        UpdateChart(Globals.MyProducts); // Chargement des produits actuels
    }

    internal void RefreshPage()
    {
        UpdateChart(Globals.MyProducts); // Rafraîchir avec les produits actuels
    }
}
