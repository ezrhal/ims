using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace IMS.Shared.Models
{
    [BsonIgnoreExtraElements]
    public class ItemModel
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
		[BsonElement("type")]
		public int typeid { get; set; }
		public string item { get; set; }
		public string description { get; set; }
		public string unit { get; set; }
		public double unitcost { get; set; }
		public int isactive { get; set; }

		public ItemModel()
		{
			Id = "";
			typeid = 1;
			item = "";
			description = "";
			unit = "";
			unitcost = 0;
			isactive = 1;
		}

	}
}

