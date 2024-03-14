using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMS.Shared.Models;

public class BalanceMaterialModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string projectid { get; set; }
    public string itemid { get; set; }
    public string item { get; set; }
    public string description { get; set; }
    public string unit { get; set; }
    public double unitcost { get; set; }
    public double quantity { get; set; }
}
