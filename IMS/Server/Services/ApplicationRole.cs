
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using AspNetCore.Identity.Mongo.Model;

namespace IMS.Server.Models.Identity
{
    [BsonIgnoreExtraElements]
    public class MongoIdentityUser : MongoUser
    {



    }

    [BsonIgnoreExtraElements]
    public class MongoIdentityRole : MongoRole
    {


    }

    [BsonIgnoreExtraElements]
    public class MongoSiteUser : MongoUser
    {
    }
}

