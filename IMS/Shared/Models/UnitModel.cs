using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMS.Shared.Models
{
	public class UnitModel
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
		public string unit { get; set; }
		[BsonElement("type")]
		public int itemtype {get; set;}
		public int isactive { get; set; }

		public UnitModel()
		{
			Id = "";
			unit = "";
			itemtype = 0;
			isactive = 1;
		}
	}
}

