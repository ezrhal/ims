using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMS.Shared.Models
{
	public class LoginModel
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        //[Required]
        public string Username { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        public string Password { get; set; }

        public LoginModel()
        {
            Username = "";
            Password = "";
        }
    }
}

