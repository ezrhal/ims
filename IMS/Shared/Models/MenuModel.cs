using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMS.Shared.Models;

[BsonIgnoreExtraElements]
public class MenuModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string parentid { get; set; }
    public string menu { get; set; }
    public string path { get; set; }
    public int isactive { get; set; }
    public string cssclass { get; set; }
    public string style { get; set; }
    public string icon { get; set; }
    public List<MenuModel> submenu { get; set; } = new();

    public MenuModel()
    {
        Id = "";
        parentid = "";
        menu = "";
        path = "";
        isactive = 1;
        cssclass = "";
        style = "";
        icon = "";
        submenu = new();
    }
    
}