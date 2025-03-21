using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyApp.Service;

public class JSONServices
{
    internal async Task<List<StrangeAnimal>> GetStrangeAnimals()
    {
        List<StrangeAnimal> MyList = new();

        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MyAnimals.json");
        
        try
        {
            using var stream = File.Open(filePath, FileMode.Open);

            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();
            MyList = JsonSerializer.Deserialize<List<StrangeAnimal>>(contents) ?? new List<StrangeAnimal>();
        }
        catch (Exception ex) 
        {
            
        }
        
        return MyList ?? new List<StrangeAnimal>();
    }

    internal async Task SetStrangeAnimals()
    {
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MyAnimals.json");
        
        try
        {
            using FileStream fileStream = File.Create(filePath);

            JsonSerializer.Serialize(fileStream, Globals.MyStrangeAnimals);
            fileStream.Dispose();
        }
        catch (Exception ex)
        {
            
        }
    }
}
