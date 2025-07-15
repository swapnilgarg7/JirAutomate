using JirAutomate.Models;
using MongoDB.Driver;

namespace JirAutomate.Services;

public class UserService
{
    private readonly IMongoCollection<User> _users;

    public UserService(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDBSettings:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDBSettings:DatabaseName"]);
        _users = db.GetCollection<User>(config["MongoDBSettings:CollectionName"]);
    }

    public List<User> GetAll() => _users.Find(_ => true).ToList();

    public User? GetByEmail(string email) =>
        _users.Find(u => u.Email.ToLower() == email.ToLower()).FirstOrDefault();

    public User? GetById(string id) =>
        _users.Find(u => u.Id == id).FirstOrDefault();

    public User Create(User user)
    {
        _users.InsertOne(user);
        return user;
    }

    public void Update(string id, User user) =>
        _users.ReplaceOne(u => u.Id == id, user);

    public void Delete(string id) =>
        _users.DeleteOne(u => u.Id == id);
}
