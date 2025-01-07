using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMS.Shared.Models;

[BsonIgnoreExtraElements]
public class RoleModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public int roleid { get; set; }
    public string role { get; set; }
    public List<MenuModel> menus { get; set; }
    public int isactive { get; set; }

    public RoleModel()
    {
        Id = "";
        roleid = 0;
        role = "User";
        isactive = 1;
    }
}