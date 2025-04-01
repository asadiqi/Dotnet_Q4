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

    public static List<Product> MyProducts = new();

}