using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Model
{
    public class Product
    {

        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string Group { get; set; } = string.Empty;

        public int Stock { get; set; }

        public int Price { get; set; }

        public string Description { get; set; } = string.Empty;

        public string? Picture { get; set; }
    }
}



