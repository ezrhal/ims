using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMS.Shared.Models;

public class SupplierModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string supplier { get; set; }
    public string address { get; set; }

    public SupplierModel()
    {
        supplier = "";
        address = "";
    }
}
