using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using IMS.Shared.Models;
using System.Reflection;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Metrics;
using IMS.Shared;
using Microsoft.AspNetCore.Mvc;


namespace IMS.Server.Services
{
    public class MongoDBService : IMongoDBService
    {
        private IMongoDatabase db;
        private IMongoCollection<ItemModel> _collection;

        public MongoDBService(IConfiguration configuration)
        {
            //var client = new MongoClient(configuration.GetConnectionString("MongoConnection"));
            var client = new MongoClient(configuration.GetConnectionString("AtlasMongoDB"));
            db = client.GetDatabase("IMS");
            _collection = db.GetCollection<ItemModel>("Items");
        }

        #region ITEMS

        public async Task<List<ItemModel>> GetItemsByType(int typeid)
        {
            return _collection.Find(e => e.typeid.Equals(typeid) && e.isactive.Equals(1)).ToList();
        }

        public async Task<string> SaveMaterial(ItemModel item)
        {
            try
            {
                _collection.InsertOne(item);
                return item.Id.ToString();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
             
        }

        public async Task<ItemModel> GetItem(string id)
        {
            return _collection.Find(e => e.Id.Equals(id)).FirstOrDefault();
        }

        public async Task<int> UpdateItem(ItemModel item)
        {
            _collection.ReplaceOne(f => f.Id.Equals(item.Id), item);
            return 1;
        }

        public async Task<int> DeleteItem(string[] ids)
        {
            string strid = "";

            foreach(string id in ids)
            {
                strid += "ObjectId('" + id + "')" + ",";
            }

            var str = "[{'_id': { $in: [" + strid + "] }}]";
            var update = new BsonDocument("$set", new BsonDocument().Set("isactive", 0) );
            BsonDocument filter = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<List<BsonDocument>>(str).First();

            _collection.UpdateManyAsync(filter, update);
            return 1;
        }

        #endregion

        #region UNIT

        public async Task<List<UnitModel>> GetUnits(int itemtype)
        {
            if (itemtype == 3)
            {
                return db.GetCollection<UnitModel>("Unit").Find(e => e.isactive.Equals(1) && e.itemtype.Equals(itemtype)).ToList();
            }
            else
            {
                return db.GetCollection<UnitModel>("Unit").Find(e => e.isactive.Equals(1)).ToList();
            }
            
        }



        #endregion

        #region PROJECT

        public async Task<List<ProjectModel>> GetProjects()
        {
            return db.GetCollection<ProjectModel>("ProjectDetails").Find(q => true).ToList();
        }

        public async Task<string> SaveProject(ProjectModel project)
        {
            try
            {
                project.datecreated = DateTime.Now;
                project.projectno = GenerateTransactionNo("PRO");
                db.GetCollection<ProjectModel>("Projects").InsertOne(project);
                return project.Id.ToString();
            }
            catch (Exception e)
            {
                return e.ToString();
            }

        }

        public async Task<ProjectModel> GetProject(string id)
        {
          
            //var result1 = db.GetCollection<BsonDocument>("Projects").Aggregate<BsonDocument>(bsondoc).ToList().FirstOrDefault();

            //return BsonSerializer.Deserialize<ProjectModel>(result1); 
            ProjectModel project = db.GetCollection<ProjectModel>("ProjectDetails").Find(q => q.Id.Equals(id)).FirstOrDefault();

            if (project.workitems != null)
            {
                project.workitems.RemoveAll(q => q.isactive == 0);
            }
            
            return project;
        } 

        public async Task<string> UpdateProject(ProjectModel project)
        {
            try
            {
                BsonDocument filter = new BsonDocument();
                filter = filter.Set("_id", new BsonObjectId(new ObjectId(project.Id)));

                BsonDocument updateQry = new BsonDocument();
                
                updateQry = updateQry.Set("projectname", project.projectname);
                updateQry = updateQry.Set("geodata.address", project.geodata.address);
                updateQry = updateQry.Set("geodata.Lat", project.geodata.Lat);
                updateQry = updateQry.Set("geodata.Lng", project.geodata.Lng);
                updateQry = updateQry.Set("geodata.pluscode", project.geodata.pluscode);

                var update = new BsonDocument("$set", updateQry);
                var result = db.GetCollection<ProjectModel>("Projects").UpdateOneAsync(filter, update);

                return "0";
            }
            catch(Exception e)
            {
                return e.ToString();
            }
            
        }

        public async Task<string> RemoveProject(string Id)
        {
            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(Id)));

            BsonDocument updateQry = new BsonDocument();
            
            updateQry = updateQry.Set("isactive", 0);
            var update = new BsonDocument("$set", updateQry);
            var result = db.GetCollection<ProjectModel>("Projects").UpdateOneAsync(filter, update);

            return "0";
        }

        public async Task<string> SaveWorkItemProject(WorkItemModel workitem, string id, string isempty)
        {
            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(id)));

            BsonDocument updateQry = new BsonDocument();
            //BsonDocument workItemElements = new BsonDocument();
            // BsonDocument bsonElements = ;

            // foreach(BsonElement val in bsonElements)
            // {
            //     workItemElements = workItemElements.Set(val.Name, val.Value);
            // }

            if (isempty == "0")
            {
                List<BsonDocument> workitemArray = new();
                workitemArray.Add(workitem.ToBsonDocument());

                updateQry = updateQry.Set("workitems", new BsonArray(workitemArray.Select(i => i.ToBsonDocument())));
            }
            else
            {
                updateQry = updateQry.Set("workitems", workitem.ToBsonDocument());
            }
            
            var update = new BsonDocument((isempty == "0" ? "$set" : "$push"), updateQry);
            var result = db.GetCollection<ProjectModel>("Projects").UpdateOneAsync(filter, update);

            return "0";
        }

        public static BsonDocument[] JsonStringToBsonDocumentPipeline(string jsonString)
        {
            // Deserialize the JSON string into a list of BsonDocuments.
            var pipeline = BsonSerializer.Deserialize<List<BsonDocument>>(jsonString);

            // Convert the list of BsonDocuments into an array of BsonDocuments.
            var bsonDocumentArray = pipeline.ToArray();

            // Return the array of BsonDocuments.
            return bsonDocumentArray;
        }

        public async Task<string> AddMaterial(BsonDocument material, string id, string workitemid, string isempty, string type)
        {
            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(id)));
            filter = filter.Set("workitems._id", new BsonObjectId(new ObjectId(workitemid)));

            BsonDocument updateQry = new BsonDocument();
            //BsonDocument workItemElements = new BsonDocument();
            // BsonDocument bsonElements = ;

            // foreach(BsonElement val in bsonElements)
            // {
            //     workItemElements = workItemElements.Set(val.Name, val.Value);
            // }

            if (isempty == "0")
            {
                List<BsonDocument> materialArray = new();
                materialArray.Add(material);
                //materialArray.Select(i => i.ToBsonDocument())
                updateQry = updateQry.Set("workitems.$." + type, new BsonArray(materialArray.Select(i => i.ToBsonDocument())));
            }
            else
            {
                updateQry = updateQry.Set("workitems.$." + type, material);
            }
            
            var update = new BsonDocument((isempty == "0" ? "$set" : "$push"), updateQry);
            var result = db.GetCollection<ProjectModel>("Projects").UpdateOneAsync(filter, update);

            return "0";
        }

        public async Task<string> AddEquipment(EquipmentModel material, string id, string workitemid, string isempty, string type)
        {
            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(id)));
            filter = filter.Set("workitems._id", new BsonObjectId(new ObjectId(workitemid)));

            BsonDocument updateQry = new BsonDocument();
            //BsonDocument workItemElements = new BsonDocument();
            // BsonDocument bsonElements = ;

            // foreach(BsonElement val in bsonElements)
            // {
            //     workItemElements = workItemElements.Set(val.Name, val.Value);
            // }

            if (isempty == "0")
            {
                List<BsonDocument> materialArray = new();
                materialArray.Add(material.ToBsonDocument());
                //materialArray.Select(i => i.ToBsonDocument())
                updateQry = updateQry.Set("workitems.$." + type, new BsonArray(material.ToBsonDocument()));
            }
            else
            {
                updateQry = updateQry.Set("workitems.$." + type, material.ToBsonDocument());
            }
            
            var update = new BsonDocument((isempty == "0" ? "$set" : "$push"), updateQry);
            var result = await db.GetCollection<ProjectModel>("Projects").UpdateOneAsync(filter, update);

            if (result.ModifiedCount > 0)
            {
                var x = 0;

            }

            return "0";
        }

        public async Task<string> RemoveMaterial(string id, string workitemid, string type, string itemid)
        {
            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(id)));
            filter = filter.Set("workitems._id", new BsonObjectId(new ObjectId(workitemid)));

            BsonDocument updateQry = new BsonDocument();
            BsonDocument item = new();
            item.Set("_id", new BsonObjectId(new ObjectId(itemid)));

            updateQry = updateQry.Set("workitems.$." + type, item );

            var update = new BsonDocument("$pull", updateQry);
            var result = db.GetCollection<ProjectModel>("Projects").UpdateOneAsync(filter, update);

            return "0";
        }

        public async Task<string> UpdateQuantity(string id, string workitemid, string type, string itemid, double quantity)
        {
            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(id)));
            filter = filter.Set("workitems._id", new BsonObjectId(new ObjectId(workitemid)));


            BsonDocument updateQry = new BsonDocument();

            updateQry = updateQry.Set("workitems.$." + type + ".$[material].quantity", quantity);
            var update = new BsonDocument("$set", updateQry);

            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("material._id", new BsonObjectId(new ObjectId(itemid)))
                )
            };

            var updateOptions = new UpdateOptions()
            {
                ArrayFilters = arrayFilters
            };

            var result = db.GetCollection<ProjectModel>("Projects").UpdateOneAsync(filter, update, updateOptions);

            return "0";
        }

        public async Task<string> UpdateQuantityHours(string id, string workitemid, string type, string itemid, double quantity, double hours)
        {
            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(id)));
            filter = filter.Set("workitems._id", new BsonObjectId(new ObjectId(workitemid)));


            BsonDocument updateQry = new BsonDocument();
            
            updateQry = updateQry.Set("workitems.$." + type + ".$[material].quantity", quantity);

            if (type != "materials")
            {
                string hrs = (type == "equipment" ? "hours" : "days");
                updateQry = updateQry.Set("workitems.$." + type + ".$[material]." + hrs, hours);
            }

            var update = new BsonDocument("$set", updateQry);

            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("material._id", new BsonObjectId(new ObjectId(itemid)))
                )
            };

            var updateOptions = new UpdateOptions()
            {
                ArrayFilters = arrayFilters
            };

            var result = db.GetCollection<ProjectModel>("Projects").UpdateOneAsync(filter, update, updateOptions);

            return "0";
        }

        public async Task<string> DeleteWorkItem(string id, string workitemid)
        {
            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(id)));
            filter = filter.Set("workitems._id", new BsonObjectId(new ObjectId(workitemid)));


            BsonDocument updateQry = new BsonDocument();
            
            updateQry = updateQry.Set("workitems.$.isactive", 0);

            var update = new BsonDocument("$set", updateQry);

            var result = db.GetCollection<ProjectModel>("Projects").UpdateOneAsync(filter, update);

            return "0";
        }


        #endregion
   
        #region WORKITEMS

        public async Task<List<WorkItemModel>> GetWorkItems()
        {
            return db.GetCollection<WorkItemModel>("Workitems").Find(e => e.isactive.Equals(1)).ToList();
        }

        public async Task<List<WorkCategoryModel>> GetCategories()
        {
            return db.GetCollection<WorkCategoryModel>("Workcategory").Find(e => e.isactive.Equals(1)).ToList();
        }

        public async Task<string> SaveWorkitem(WorkItemModel workitem)
        {
            db.GetCollection<WorkItemModel>("Workitems").InsertOne(workitem);
            return workitem.Id;
        }

        public async Task<string> EditWorkItem(WorkItemModel workitem)
        {
            db.GetCollection<WorkItemModel>("Workitems").ReplaceOne(f => f.Id.Equals(workitem.Id), workitem);
            return workitem.Id;
        }

        public async Task<int> DeleteWorkItem(string id)
        {
            string strid = "ObjectId('" + id + "')";

            var str = "[{'_id': { $in: [" + strid + "] }}]";
            var update = new BsonDocument("$set", new BsonDocument().Set("isactive", 0) );
            BsonDocument filter = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<List<BsonDocument>>(str).First();

            db.GetCollection<WorkItemModel>("Workitems").UpdateOneAsync(filter, update);
            return 1;
        }

        #endregion

        #region PURCHASE REQUEST

        public async Task<List<PRModel>> GetPRs(string projectid)
        {
            //remove submitted filter
            if (projectid.Length > 0)
            {
                return db.GetCollection<PRModel>("PurchaseRequest").Find(q => q.isactive.Equals(1) && q.projectid.Equals(projectid)).ToList();
            }
            else
            {
                return db.GetCollection<PRModel>("PurchaseRequest").Find(q => q.isactive.Equals(1)).ToList();
            }
            
        }

        public async Task<PRModel> GetPR(string id)
        {
            return db.GetCollection<PRModel>("PurchaseRequest").Find(q => q.Id.Equals(id)).FirstOrDefault();
        }

        public async Task<List<ProjectModel>> GetProjectname()
        {

            var simpleProjection = Builders<ProjectModel>.Projection
                .Include(u => u.Id)
                .Include(u => u.projectname);
            var result = db.GetCollection<ProjectModel>("Projects").Find(q => q.isactive.Equals(1)).Project(simpleProjection).ToList();

            return new List<ProjectModel>(result.Select(q => BsonSerializer.Deserialize<ProjectModel>(q)).ToList());

            // var pipeline = new[]
            // {
            //     new BsonDocument("$project", new BsonDocument
            //     {
            //         { "_id", 1 },
            //         { "projectname", 1 }
            //     })
            // };

            // return db.GetCollection<ProjectModel>("Projects").Aggregate<ProjectModel>(pipeline).ToList();
        }

        public async Task<string> SavePR(PRModel pr)
        {
            try
            {
                pr.transactionno = GenerateTransactionNo("PR");
                pr.dateprepared = DateTime.Now;
                db.GetCollection<PRModel>("PurchaseRequest").InsertOne(pr);
                return pr.Id;
            }
            catch(Exception e)
            {
                return e.ToString();
            }

        }

        public async Task<List<MaterialsModel>> GetMaterialsQuantity(string projectid, string itemid)
        {
            if (itemid.Length > 0)
            {
                return db.GetCollection<MaterialsModel>("MaterialsQuantity").Find(q => q.Id.Equals(projectid) && q.itemid.Equals(itemid)).ToList();
            }
            else
            {
                return db.GetCollection<MaterialsModel>("MaterialsQuantity").Find(q => q.Id.Equals(projectid)).ToList();
            }
            
        }

        public async Task<string> SavePRItem(string id, PRItemModel pritem, string isempty)
        {
            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(id)));

            BsonDocument updateQry = new BsonDocument();
            
            if (isempty == "0")
            {
                
               // List<BsonDocument> itemArray = new();
               // itemArray.Add(pritem.ToBsonDocument());
                //materialArray.Select(i => i.ToBsonDocument())
                 List<BsonDocument> itemArray = new();
                itemArray.Add(pritem.ToBsonDocument());

                updateQry = updateQry.Set("items", new BsonArray(itemArray.Select(i => i.ToBsonDocument())));
            }
            else
            {
                updateQry = updateQry.Set("items", pritem.ToBsonDocument());
            }
            
            var update = new BsonDocument((isempty == "0" ? "$set" : "$push"), updateQry);
            var result = await db.GetCollection<PRModel>("PurchaseRequest").UpdateOneAsync(filter, update);

            if (result.ModifiedCount > 0)
            {
                var x = 0;

            }

            return "0";
        }

        public async Task<string> RemovePRItem(string id, string pritemid)
        {
            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(id)));

            BsonDocument updateQry = new BsonDocument();
            BsonDocument item = new();
            item.Set("_id", new BsonObjectId(new ObjectId(pritemid)));

            updateQry = updateQry.Set("items", item );

            var update = new BsonDocument("$pull", updateQry);
            var result = db.GetCollection<PRModel>("PurchaseRequest").UpdateOneAsync(filter, update);

            return "0";
        }

        public async Task<string> UpdatePRItem(string id, string pritemid, double quantity)
        {
            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(id)));
            filter = filter.Set("items._id", new BsonObjectId(new ObjectId(pritemid)));


            BsonDocument updateQry = new BsonDocument();
            
            updateQry = updateQry.Set("items.$.quantity", quantity);

            var update = new BsonDocument("$set", updateQry);

            var result = db.GetCollection<PRModel>("PurchaseRequest").UpdateOneAsync(filter, update);

            return "0";
        }

        public async Task<TotalProcuredModel> GetTotalProcured(string  projectid, string  itemid)
        {
            TotalProcuredModel totalProcuredModel = new();

            try
            {
                totalProcuredModel = db.GetCollection<TotalProcuredModel>("ItemTotalProcured").Find(q => q.projectid.Equals(projectid) && q.itemid.Equals(itemid)).FirstOrDefault();
            }
            catch{}
            return totalProcuredModel;
        }

        public async Task<string> DeletePR(string id)
        {
            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(id)));


            BsonDocument updateQry = new BsonDocument();
            
            updateQry = updateQry.Set("isactive", 0);

            var update = new BsonDocument("$set", updateQry);

            var result = db.GetCollection<PRModel>("PurchaseRequest").UpdateOneAsync(filter, update);

            return "0";
        }

        public async Task<string> SubmitPR(string id)
        {
            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(id)));


            BsonDocument updateQry = new BsonDocument();
            
            updateQry = updateQry.Set("submitted", 1);

            var update = new BsonDocument("$set", updateQry);

            var result = db.GetCollection<PRModel>("PurchaseRequest").UpdateOneAsync(filter, update);

            return "0";
        }

        #endregion

        #region PURCHASE ORDER

        public async Task<List<SupplierModel>> GetSuppliers()
        {
            var result = db.GetCollection<SupplierModel>("Suppliers").Find(q => true).ToList();
            return result;
        }

        public async Task<List<POModel>> GetPOs(string projectid)
        {
            var result = db.GetCollection<POModel>("PurchaseOrder").Find(q => q.projectid.Equals(projectid)).ToList();
            return result;
        }

        public async Task<string> SavePO(POModel po, string prid, string isempty)
        {
            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(prid)));

            po.dateprepared = DateTime.Now;
            po.pono = GenerateTransactionNo("PO");

            BsonDocument updateQry = new BsonDocument();
            
            if (isempty == "0")
            {
                
               // List<BsonDocument> itemArray = new();
               // itemArray.Add(pritem.ToBsonDocument());
                //materialArray.Select(i => i.ToBsonDocument())
                 List<BsonDocument> itemArray = new();
                itemArray.Add(po.ToBsonDocument());

                updateQry = updateQry.Set("PO", new BsonArray(itemArray.Select(i => i.ToBsonDocument())));
            }
            else
            {
                updateQry = updateQry.Set("PO", po.ToBsonDocument());
            }
            
            var update = new BsonDocument((isempty == "0" ? "$set" : "$push"), updateQry);
            var result = await db.GetCollection<POModel>("PurchaseRequest").UpdateOneAsync(filter, update);

            return po.Id;
        }

        public async Task<POModel> GetPO(string poid)
        {
            return db.GetCollection<POModel>("PurchaseOrder").Find(q => q.Id.Equals(poid)).FirstOrDefault();
        }

        public async Task<List<PRItemModel>> GetPRItems(string prid)
        {
            //return db.GetCollection<PRItemModel>("PRItems").Find(q => q.Id.Equals(prid)).ToList();

            List<PRItemModel> pritems =  db.GetCollection<PRItemModel>("PRItems").Find(q => q.Id.Equals(prid)).ToList();
            List<PRItemModel> poitems = db.GetCollection<PRItemModel>("POItems").Find(q => q.Id.Equals(prid)).ToList();

            var startDict = pritems.GroupBy(r => new {r.Id, r.itemid}).ToDictionary(g => g.Key, g => g.First());
            var endDict = poitems.GroupBy(r => new {r.Id, r.itemid}).ToDictionary(g => g.Key, g => g.First());

            foreach (var key in startDict.Keys.Union(endDict.Keys)) {
                var hasStart = startDict.TryGetValue(key, out var start);
                var hasEnd = endDict.TryGetValue(key, out var end);
                if (hasStart && hasEnd) {
                    double quantity = (double)pritems.First(q => q.Id.Equals(key.Id) && q.itemid.Equals(key.itemid)).quantity;
                    double totalpo = (double)poitems.First(q => q.Id.Equals(key.Id) && q.itemid.Equals(key.itemid)).quantity;

                    quantity = quantity - totalpo;

                    pritems.First(q => q.Id.Equals(key.Id) && q.itemid.Equals(key.itemid)).quantity = quantity;
                } 
            }

            pritems.RemoveAll(q => q.quantity <= 0);
            return pritems;
        }

        public async Task<string> SavePOItems(string poid, string prid, List<POItemModel> items, List<POItemModel> existingitms)
        {

            var startDict = items.GroupBy(r => new {r.itemid}).ToDictionary(g => g.Key, g => g.First());
            var endDict = existingitms.GroupBy(r => new {r.itemid}).ToDictionary(g => g.Key, g => g.First());
            List<string> itemids = new();

            foreach (var key in startDict.Keys.Union(endDict.Keys)) {
                var hasStart = startDict.TryGetValue(key, out var start);
                var hasEnd = endDict.TryGetValue(key, out var end);
                if (hasStart && hasEnd) {
                    double currentquantity = (double)existingitms.First(q => q.itemid.Equals(key.itemid)).quantity;
                    double addquantity = (double)items.First(q => q.itemid.Equals(key.itemid)).quantity;

                    existingitms.First(q => q.itemid.Equals(key.itemid)).quantity = currentquantity + addquantity;
                    itemids.Add(key.itemid);
                    items.First(q => q.itemid.Equals(key.itemid)).Id = "";

                }
                else if (hasStart && !hasEnd)
                {
                    items.First(q => q.itemid.Equals(key.itemid)).Id = ObjectId.GenerateNewId().ToString();
                }
            }

            items.RemoveAll(q => q.Id.Equals(""));

            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(prid)));
            filter = filter.Set("PO._id", new BsonObjectId(new ObjectId(poid)));

            var update = Builders<POItemModel>.Update.PushEach("PO.$.items", items);
            
            var result = db.GetCollection<POItemModel>("PurchaseRequest").UpdateOne(filter, update);
            
            foreach(POItemModel existingitem in existingitms)
            {
                if (itemids.Contains(existingitem.itemid))
                {
                    BsonDocument updateQry = new BsonDocument();
                    updateQry = updateQry.Set("PO.$.items.$[material].quantity", existingitem.quantity);
                    var updt = new BsonDocument("$set", updateQry);

                    var arrayFilters = new List<ArrayFilterDefinition>
                    {
                        new BsonDocumentArrayFilterDefinition<BsonDocument>(
                            new BsonDocument("material._id", new BsonObjectId(new ObjectId(existingitem.Id)))
                        )
                    };

                    var updateOptions = new UpdateOptions()
                    {
                        ArrayFilters = arrayFilters
                    };

                    result = await db.GetCollection<POItemModel>("PurchaseRequest").UpdateOneAsync(filter, updt, updateOptions);

                }
            }

          
            // var update = Builders<POItemModel>.Update.PushEach("PO.$[elem].items", items, position: -1);

            // var arrayFilters = new List<ArrayFilterDefinition>
            // {
            //     new JsonArrayFilterDefinition<BsonDocument>("{'elem._id': {$eq: '" + new BsonObjectId(new ObjectId(poid)) + "'}}")
            // };

            // var options = new UpdateOptions
            // {
            //     ArrayFilters = arrayFilters
            // };

            // var result = db.GetCollection<POItemModel>("PurchaseRequest").UpdateOne(filter, update, options);

            return "0";
        }

        public async Task<string> DeletePOItem(string prid, string poid, string id)
        {
            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(prid)));
            filter = filter.Set("PO._id", new BsonObjectId(new ObjectId(poid)));

            BsonDocument updateQry = new BsonDocument();
            BsonDocument item = new();
            item.Set("_id", new BsonObjectId(new ObjectId(id)));

            updateQry = updateQry.Set("PO.$.items", item );

            var update = new BsonDocument("$pull", updateQry);
            var result = db.GetCollection<POItemModel>("PurchaseRequest").UpdateOneAsync(filter, update);
            return "0";
        }

        public async Task<string> UpdatePOItem(string prid, string poid, POItemModel poitem)
        {
            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(prid)));
            filter = filter.Set("PO._id", new BsonObjectId(new ObjectId(poid)));


            BsonDocument updateQry = new BsonDocument();

            updateQry = updateQry.Set("PO.$.items.$[material].quantity", poitem.quantity);
            updateQry = updateQry.Set("PO.$.items.$[material].price", poitem.price);
            var update = new BsonDocument("$set", updateQry);

            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("material._id", new BsonObjectId(new ObjectId(poitem.Id)))
                )
            };

            var updateOptions = new UpdateOptions()
            {
                ArrayFilters = arrayFilters
            };

            var result = db.GetCollection<PRModel>("PurchaseRequest").UpdateOneAsync(filter, update, updateOptions);

            return "0";
        }

        public async Task<string> SubmitPO(string prid, string poid)
        {
            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(prid)));
            filter = filter.Set("PO._id", new BsonObjectId(new ObjectId(poid)));


            BsonDocument updateQry = new BsonDocument();
            
            updateQry = updateQry.Set("PO.$.submitted", 1);

            var update = new BsonDocument("$set", updateQry);

            var result = db.GetCollection<POModel>("PurchaseRequest").UpdateOneAsync(filter, update);

            return "0";
        }

        #endregion

        #region FUNCTIONS

        private string GenerateTransactionNo(string type)
        {
            TransactionCounterModel counter = db.GetCollection<TransactionCounterModel>("Transaction Counter").Find(q => q.transaction.Equals(type)).FirstOrDefault();   
            int current = counter.transactioncount;
            counter.transactioncount++;

            BsonDocument filter = new BsonDocument();
            filter = filter.Set("_id", new BsonObjectId(new ObjectId(counter.Id)));

            BsonDocument updateQry = new BsonDocument();
            updateQry = updateQry.Set("transactioncount", counter.transactioncount);
            var update = new BsonDocument("$set", updateQry);
            var result = db.GetCollection<TransactionCounterModel>("Transaction Counter").UpdateOneAsync(filter, update);

            current++;

            string transaction = counter.transaction + "-" + DateTime.Now.ToString("yy") + "-" + current.ToString("D4");

            return transaction;
        }


        public async Task<List<BalanceMaterialModel>> GetBalanceMaterials(string projectid)
        {
            List<BalanceMaterialModel> materials =  db.GetCollection<BalanceMaterialModel>("MaterialsQuantity").Find(q => q.projectid.Equals(projectid)).ToList();
            List<TotalProcuredModel> totalp = db.GetCollection<TotalProcuredModel>("ItemTotalProcured").Find(q => q.projectid.Equals(projectid)).ToList();

            var startDict = materials.GroupBy(r => new {r.projectid, r.itemid}).ToDictionary(g => g.Key, g => g.First());
            var endDict = totalp.GroupBy(r => new {r.projectid, r.itemid}).ToDictionary(g => g.Key, g => g.First());

            foreach (var key in startDict.Keys.Union(endDict.Keys)) {
                var hasStart = startDict.TryGetValue(key, out var start);
                var hasEnd = endDict.TryGetValue(key, out var end);
                if (hasStart && hasEnd) {
                    double totalquantity = materials.First(q => q.projectid.Equals(key.projectid) && q.itemid.Equals(key.itemid)).quantity;
                    double totalpr = totalp.First(q => q.projectid.Equals(key.projectid) && q.itemid.Equals(key.itemid)).totalprocured;

                    totalquantity = totalquantity - totalpr;

                    materials.First(q => q.projectid.Equals(key.projectid) && q.itemid.Equals(key.itemid)).quantity = totalquantity;
                } 
            }

            return materials;

        }

        #endregion

        //TRANSFER TO REPORT SERVICE
        public async Task<List<WorkItemModel>> GetReportProject(string projectid)
        {
            ProjectModel project = db.GetCollection<ProjectModel>("ProjectDetails").Find(q => q.Id.Equals(projectid)).FirstOrDefault();

            if (project.workitems != null)
            {
                project.workitems.RemoveAll(q => q.isactive == 0);
            }
            
            return project.workitems;
        }

        public async Task<List<MaterialsModel>> GetMaterials(string projectid, string workitemid)
        {
            ProjectModel project = db.GetCollection<ProjectModel>("ProjectDetails").Find(q => q.Id.Equals(projectid)).FirstOrDefault();

            if (project.workitems != null)
            {
                WorkItemModel workitem = project.workitems.Find(q => q.Id.Equals(workitemid));
               return workitem.materials;
            }
            
            return null;
        }

        public async Task<List<EquipmentModel>> GetEquipment(string projectid, string workitemid)
        {
            ProjectModel project = db.GetCollection<ProjectModel>("ProjectDetails").Find(q => q.Id.Equals(projectid)).FirstOrDefault();

            if (project.workitems != null)
            {
                WorkItemModel workitem = project.workitems.Find(q => q.Id.Equals(workitemid));
               return workitem.equipment;
            }
            
            return null;
        }

        public async Task<List<LaborModel>> GetLabor(string projectid, string workitemid)
        {
            ProjectModel project = db.GetCollection<ProjectModel>("ProjectDetails").Find(q => q.Id.Equals(projectid)).FirstOrDefault();

             if (project.workitems != null)
            {
                WorkItemModel workitem = project.workitems.Find(q => q.Id.Equals(workitemid));
               return workitem.labor;
            }
            
            return null;
        }

    }
}

