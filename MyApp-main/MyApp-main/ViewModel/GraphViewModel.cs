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
            ValueLabel = $"Price: {product.Price} €",
            Color = GetProductColor(product.Group) // Détermination de la couleur en fonction du groupe
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
            ValueLabel = $"{product.Stock} units",
            Color = GetProductColor(product.Group) // Détermination de la couleur en fonction du groupe
        }).ToList();

        return new BarChart
        {
            Entries = entries,
            LabelTextSize = 15,
            MaxValue = Globals.MyProducts.Max(p => p.Stock) + 10,
        };
    }

    private SKColor GetProductColor(string group)
    {
        var normalizedGroup = group?.Trim().ToLower();

        // Déterminer la couleur en fonction du groupe du produit
        return normalizedGroup switch
        {
            "fruit" => SKColor.Parse("#FFA500"), // Orange pour fruits
            "légume" => SKColor.Parse("#2ECC71"), // Vert pour légumes
            _ => SKColor.Parse("#5c1f0a"), //  pour les autres
        };
    }
}
