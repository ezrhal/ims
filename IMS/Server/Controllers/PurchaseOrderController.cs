using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using IMS.Server.Services;
using IMS.Shared;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace IMS.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class PurchaseOrderController : Controller
{
    IMongoDBService _db;

    public PurchaseOrderController(IMongoDBService db)
    {
        _db = db;
    }

    [HttpGet("getsuppliers")]
    public async Task<List<SupplierModel>> GetSuppliers()
    {
        return await _db.GetSuppliers();
    }
    
    [HttpGet("getpos")]
    public async Task<List<POModel>> GetPOs(string projectid = "")
    {
        return await _db.GetPOs(projectid);
    }

    [HttpPost("savepo")]
    public async Task<string> SavePO(List<string> paramList)
    {
        POModel po = Newtonsoft.Json.JsonConvert.DeserializeObject<POModel>(paramList[0].ToString());
        string prid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[1].ToString());
        string isempty = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[2].ToString());

        return await _db.SavePO(po, prid, isempty);
    }

    [HttpGet("getpo")]
    public async Task<POModel> GetPO(string poid, string prid)
    {
        return await _db.GetPO(poid, prid);
    }

    [HttpGet("getpritems")]
    public async Task<List<PRItemModel>> GetPRItems(string prid)
    {
        return await _db.GetPRItems(prid);
    }

    [HttpPost("savepoitems")]
    public async Task<string> SavePOItems(List<string> paramList)
    {
        string poid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[0].ToString());
        string prid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[1].ToString());
        List<POItemModel> items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<POItemModel>>(paramList[2].ToString());
        List<POItemModel> existing = Newtonsoft.Json.JsonConvert.DeserializeObject<List<POItemModel>>(paramList[3].ToString());

        return await _db.SavePOItems(poid, prid, items, existing);
    }

    [HttpPost("deletepoitem")]
    public async Task<string> DeletePOItem(List<string> paramList)
    {
        string prid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[0].ToString());
        string poid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[1].ToString());
        string id = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[2].ToString());

        return await _db.DeletePOItem(prid, poid, id);
    }

    [HttpPost("editquantity")]
    public async Task<string> EditItem(List<string> paramList)
    {
        string prid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[0].ToString());
        string poid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[1].ToString());
        POItemModel item = Newtonsoft.Json.JsonConvert.DeserializeObject<POItemModel>(paramList[2].ToString());

        return await _db.UpdatePOItem(prid, poid, item);
    }

    [HttpPost("submitpo")]
    public async Task<string> SubmitPO(List<string> paramList)
    {
        string prid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[0].ToString());
        string poid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[1].ToString());
       
        return await _db.SubmitPO(prid, poid);
    }

    [HttpPost("deletepo")]
    public async Task<string> DeletePO(List<string> paramList)
    {
        string prid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[0].ToString());
        string poid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[1].ToString());

        return await _db.DeletePO(prid, poid);
    }

    [HttpPost("editpo")]
    public async Task<string> EditPO(List<string> paramList)
    {
        POModel po = Newtonsoft.Json.JsonConvert.DeserializeObject<POModel>(paramList[0].ToString());
        return await _db.EditPO(po);
    }

}