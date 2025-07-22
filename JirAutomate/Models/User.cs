using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JirAutomate.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("role")]
    public string Role { get; set; } = "user";

    [BsonElement("jiraDomain")]
    public string JiraDomain { get; set; } = string.Empty;

    [BsonElement("projectKey")]
    public string ProjectKey { get; set; } = string.Empty;

    [BsonElement("jiraEmail")]
    public string JiraEmail { get; set; } = string.Empty;

    [BsonElement("jiraApi")]
    public string JiraApi { get; set; } = string.Empty;

}
