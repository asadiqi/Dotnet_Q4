using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Model
{
    public class ProductInCart
    {
        public string ProductId { get; set; }   // ID du produit
        public string Name { get; set; }        // Nom du produit
        public int Quantity { get; set; }       // Quantité de ce produit dans le panier
    }

}
