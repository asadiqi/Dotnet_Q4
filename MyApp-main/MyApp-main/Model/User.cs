using BCrypt.Net;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace MyApp.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }  // Identifiant unique MongoDB

        public string FirstName { get; set; }  // Prénom de l'utilisateur
        public string LastName { get; set; }   // Nom de l'utilisateur
        public string Email { get; set; }      // Adresse email de l'utilisateur

        public string PasswordHash { get; set; }  // Mot de passe haché

        public string Role { get; set; } = "user";  // Rôle de l'utilisateur : "user" ou "admin"

        // Liste des produits ajoutés au panier de l'utilisateur
        public List<ProductInCart> Products { get; set; } = new List<ProductInCart>();

        // Méthode pour définir et hacher le mot de passe
        public void SetPassword(string password)
        {
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Méthode pour vérifier si le mot de passe correspond au hachage
        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }
    }
}
