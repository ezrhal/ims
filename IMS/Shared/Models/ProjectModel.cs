using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMS.Shared.Models
{
	[BsonIgnoreExtraElements]
	public class ProjectModel
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
		public string projectname { get; set; }
		public string location { get; set; }
		public string[]? geolocation { get; set; }
		public DateTime datecreated { get; set; }
		public DateTime? targetdate { get; set; }
		public int? userid { get; set; }
		public int isactive { get; set; } = 1;
		public List<WorkItemModel> workitems { get; set; }
		public GeoDataModel geodata { get; set; } = new();
		public double projectcost { get; set; } = 0;
		public string projectno { get; set; }

		public ProjectModel()
		{
			Id = "";
			projectname = "";
			location = "";
			datecreated = DateTime.Now;
			isactive = 1;
			workitems = new();
			projectno = "";
		}
	}

	[BsonIgnoreExtraElements]
	public class MaterialsModel
	{
		[BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
		public string itemid { get; set; }
		public string item { get; set; }
		public string unit { get; set; }
		public double unitcost { get; set; }
		public double? quantity { get; set; }
		public string description { get; set; }
		[BsonIgnoreIfDefault]
		public double amount { get; set; } = 0;

		public string workitemid { get; set; } = "";

		public MaterialsModel()
		{
			Id = ObjectId.GenerateNewId().ToString();
		}
	}

	[BsonIgnoreExtraElements]
    public class EquipmentModel
    {
		[BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
        public string itemid { get; set; }
        public string item { get; set; }
        public string description { get; set; }
        public string unit { get; set; }
        public double unitcost { get; set; }
        public double? quantity { get; set; }
		public double? hours { get; set; }
		public double? days { get; set; }
		[BsonIgnoreIfDefault]
		public double amount { get; set; } = 0;
		

		public EquipmentModel()
		{
			Id = ObjectId.GenerateNewId().ToString();
		}

	}

	[BsonIgnoreExtraElements]
    public class LaborModel
    {
		[BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
        public string itemid { get; set; }
        public string item { get; set; }
        public string unit { get; set; }
        public double unitcost { get; set; }
        public double? quantity { get; set; }
		public double? days { get; set; }
        public string description { get; set; }
		[BsonIgnoreIfDefault]
		public double amount { get; set; } = 0;

		public LaborModel()
		{
			Id = ObjectId.GenerateNewId().ToString();
		}
    }
}

