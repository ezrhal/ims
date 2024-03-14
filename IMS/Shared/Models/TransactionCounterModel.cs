using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMS.Shared.Models;

[BsonIgnoreExtraElements]
public class TransactionCounterModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string transaction { get; set; }
    public int transactioncount { get; set; }
}
