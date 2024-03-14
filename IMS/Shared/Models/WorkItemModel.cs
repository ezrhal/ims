using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMS.Shared.Models
{
    [BsonIgnoreExtraElements]
    public class WorkItemModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string workitemid { get; set; }
        public string itemno { get; set; }
        public string description { get; set; }
        public string partno { get; set; }
        public int isactive { get; set; }

        [BsonIgnoreIfDefault]
        public double totalmaterials { get; set; }
         [BsonIgnoreIfDefault]
        public double totalequipment { get; set; }
         [BsonIgnoreIfDefault]
        public double totallabor { get; set; }
         [BsonIgnoreIfDefault]
        public double totalamount { get; set; }
        
        public List<MaterialsModel>? materials { get; set; }
        public List<EquipmentModel>? equipment { get; set; }
        public List<LaborModel>? labor { get; set; }

        public WorkItemModel()
        {
            Id = "";
            workitemid = "";
            itemno = "";
            description = "";
            partno = "";
            isactive = 1;
            totalmaterials = 0;
            totalequipment = 0;
            totallabor = 0;
            totalamount = 0;
            materials = new();
            equipment = new();
            labor = new();
        }
    }

    public class PartModel
    {
        public string Id { get; set; }
        public string itemno { get; set; }
        public string description { get; set; }
    }
	
}