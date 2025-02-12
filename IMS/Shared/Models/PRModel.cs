﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMS.Shared.Models;

[BsonIgnoreExtraElements]
public class PRModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string projectid { get; set; }
    public string projectname { get; set; }
    public DateTime? dateprepared { get; set; }
    public int? userid { get; set; }
    public int isactive { get; set; }
    public List<PRItemModel> items { get; set; }
    [BsonIgnoreIfDefault]
	public double amount { get; set; } = 0;
    public string transactionno { get; set; }
    public List<StatusModel>? statuslogs { get; set; }
    public int submitted { get; set; }
    public List<POModel> PO { get; set; }
    public string? workitemid { get; set; }
    public string? workitem { get; set; }
    public string charges { get; set; }

    public PRModel()
    {
        Id = "";
        isactive = 1;
        items = new();
        PO = new();
        projectid = "";
        projectname = "";
        userid = 0;
        isactive = 1;
        amount = 0;
        transactionno = "";
        submitted = 0;
        charges = "";
    }


}

[BsonIgnoreExtraElements]
public class PRItemModel
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

    public PRItemModel()
    {
        Id = ObjectId.GenerateNewId().ToString();
        unitcost = 0;
       
        itemid = "";
        item = "";
        description = "";
        unit = "";

    }
}

[BsonIgnoreExtraElements]
public class POModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string pono { get; set; }
    [BsonIgnoreIfNull]
    [BsonIgnoreIfDefault]
    public string projectid { get; set; } = "";
    [BsonIgnoreIfNull]
    public string prid { get; set; } = null;
    [BsonIgnoreIfNull]
    public string prno { get; set; } = null;
    public DateTime? dateprepared { get; set; }
    public string supplierid { get; set; }
    public string? supplier { get; set; }
    public string? supplieraddress { get; set; }
    public int isactive { get; set; }
    public string? userid { get; set; }
    [BsonIgnoreIfDefault]
    public double? amount { get; set; }
    public int submitted { get; set; }
    public List<POItemModel> items { get; set; }

    //CV
    public string? cvno { get; set; }
    public string? checkno { get; set; }
    public string? checkdate { get; set; }
    public DateTime? cvdate { get; set; }
    public string? payee { get; set; }
    public string? payeeaddress { get; set; }
    public int? isrecieved { get; set; }
    public int cvsubmitted { get; set; }

    public DateTime? datereceived { get; set; }

    [BsonIgnoreIfDefault]
    [BsonIgnoreIfNull]
    public string projectname { get; set; } = "";

    public POModel()
    {
        Id = ObjectId.GenerateNewId().ToString();
        pono = "";
        isactive = 1;
        amount = 0;
        supplierid = "";
        supplier = "";
        supplieraddress = "";
        cvsubmitted = 0;

        items = new();
    }
}

[BsonIgnoreExtraElements]
public class POItemModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string itemid { get; set; }
    public string item { get; set; }
    public string description { get; set; }
    public string unit { get; set; }
    public double price { get; set; }
    public double? quantity { get; set; }
    [BsonIgnoreIfDefault]
    [BsonIgnoreIfNull]
    public double unitcost { get; set; }

    public POItemModel()
    {
        Id = ObjectId.GenerateNewId().ToString();
        price = 0;
        unitcost = 0;
    }
}

public class POLogModel()
{
    public string status { get; set; }
    public string remarks { get; set; }
    public DateTime logdate { get; set; }
    public string user { get; set; }
}

