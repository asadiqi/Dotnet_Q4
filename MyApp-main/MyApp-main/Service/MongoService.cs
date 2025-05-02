using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MyApp.Models;
using BCrypt.Net;

namespace MyApp.Service
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UsersService()
        {
            var client = new MongoClient("mongodb://student:IAmTh3B3st@185.157.245.38:5003");
            var database = client.GetDatabase("AMShop");
            _usersCollection = database.GetCollection<User>("users");
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _usersCollection.Find(_ => true).ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> AddUserAsync(User newUser, string password)
        {
            var existingUser = await _usersCollection.Find(u => u.Email == newUser.Email).FirstOrDefaultAsync();
            if (existingUser != null)
                return false;

            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            await _usersCollection.InsertOneAsync(newUser);
            return true;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return user;
            return null;
        }

        public async Task UpdateUserCartAsync(string userId, List<ProductInCart> updatedCart)
        {
            var update = Builders<User>.Update.Set(u => u.Products, updatedCart);
            await _usersCollection.UpdateOneAsync(u => u.Id == userId, update);
        }


        public async Task DeleteUserAsync(string id)
        {
            await _usersCollection.DeleteOneAsync(u => u.Id == id);
        }
    }
}
