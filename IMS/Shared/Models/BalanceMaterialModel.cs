using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMS.Shared.Models;

[BsonIgnoreExtraElements]
public class BalanceMaterialModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonIgnoreIfDefault]
    [BsonIgnoreIfNull]
    public string projectid { get; set; }
    [BsonIgnoreIfDefault]
    [BsonIgnoreIfNull]
    public string workitemid { get; set; }
    public string itemid { get; set; }
    public string item { get; set; }
    public string description { get; set; }
    public string unit { get; set; }
    public double unitcost { get; set; }
    public double quantity { get; set; }
    [BsonIgnoreIfDefault]
    [BsonIgnoreIfNull]
    public double prquantity { get; set; }
    [BsonIgnoreIfDefault]
    [BsonIgnoreIfNull]
    public double balancequantity { get; set; }
    public BalanceMaterialModel()
    {
        projectid = "";
        workitemid = "";
        prquantity = 0;
        balancequantity = 0;
    }
}
