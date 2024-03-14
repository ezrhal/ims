using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMS.Shared.Models
{
    public class WorkCategoryModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string workcategory { get; set; }
        public int isactive { get; set; }
    }
}