global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;

global using MyApp.View;
global using MyApp.ViewModel;
global using MyApp.Model;
global using MyApp.Service;

global using CommunityToolkit.Mvvm;
global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Maui;

// dans cette classe on met toutes les vars listes etc
// ce sont des varibales qui sont utiliser dans tous le projet

public class Globals
{
    public static List<Product> MyProducts = new()
    {
        new Product { Id = "1", Name = "Orange", Group="Fruit", Stock = 10, Price = 100 },
        new Product { Id = "2", Name = "Pomme",Group="Fruit", Stock = 20, Price = 150 },
        new Product { Id = "3", Name = "Tomate",Group="légume", Stock = 5, Price = 200 },
        new Product { Id = "4", Name = "Concombre", Group = "légume", Stock = 50, Price = 250 },
        new Product { Id = "5", Name = "Fraise", Group="Fruit" ,Stock = 30, Price = 300 },
    };
}

