using Microcharts;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MyApp.ViewModel;

public partial class GraphViewModel : ObservableObject
{
    public Chart ProductChart { get; }
    public Chart StockChart { get; } // Ajout du graphique pour le stock

    public GraphViewModel()
    {
        ProductChart = GeneratePriceChart();
        StockChart = GenerateStockChart(); // Création du deuxième graphique
    }

    private Chart GeneratePriceChart()
    {
        var entries = Globals.MyProducts.Select(product => new ChartEntry(product.Price)
        {
            Label = product.Name,
            ValueLabel = $"Prix: {product.Price} €",
            Color = SKColor.Parse(product.Group == "Fruit" ? "#FFA500" : "#2ECC71") // Orange pour fruits, Vert pour légumes
        }).ToList();
        return new BarChart
        {
            Entries = entries,
            LabelTextSize = 15,
            MaxValue = Globals.MyProducts.Max(p => p.Price) + 50,
        };
    }

    private Chart GenerateStockChart()
    {
        var entries = Globals.MyProducts.Select(product => new ChartEntry(product.Stock)
        {
            Label = product.Name,
            ValueLabel = $"{product.Stock} unités",
            Color = SKColor.Parse(product.Group == "Fruit" ? "#FFA500" : "#2ECC71") // Orange pour fruits, Vert pour légumes
        }).ToList();

        return new BarChart
        {
            Entries = entries,
            LabelTextSize = 15,
            MaxValue = Globals.MyProducts.Max(p => p.Stock) + 10,
        };
    }


}
