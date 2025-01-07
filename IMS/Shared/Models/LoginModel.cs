using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMS.Shared.Models
{
    [BsonIgnoreExtraElements]
	public class LoginModel
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        //[Required]
        [BsonElement("Username")]
        public string Username { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        [BsonElement("Password")]
        public string Password { get; set; }

        [BsonElement("menus")]
        public List<MenuModel> menus { get; set; }

        public LoginModel()
        {
            Id = "";
            Username = "";
            Password = "";
            menus = new();
        }
    }
}

