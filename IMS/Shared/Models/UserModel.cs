using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMS.Shared.Models;

[BsonIgnoreExtraElements]
public class UserModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string firstname { get; set; }
    public string lastname { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string role { get; set; }
    public string roleid { get; set; }
    public List<MenuModel> menu { get; set; }

    public UserModel()
    {
        Id = "";
        firstname = "";
        lastname = "";
        Username = "";
        Password = "";
        role = "";
        roleid = "";
        menu = new();
    }
}