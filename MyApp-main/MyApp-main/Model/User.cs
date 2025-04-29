using BCrypt.Net;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyApp.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }

        // Stocke le mot de passe hashé
        public string PasswordHash { get; set; }

        // Méthode pour définir le mot de passe (hashé)
        public void SetPassword(string password)
        {
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Méthode pour vérifier le mot de passe
        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }
    }
}
